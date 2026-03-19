
using BG_Entities;
using BG_Services;
using BGDAL;
using BGLogs;
using BGModel;
using BGUserControl;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.ImageImportDll;
using Timer = System.Timers.Timer;

namespace CMW
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        internal IUserControlView CurrentModule;
        Timer _timer = new Timer();
        public static string Language = string.Empty;
        Dictionary<string, string> paramConfigDic = new Dictionary<string, string>();
        //全局的命令对象，放不到Common中，不然会交叉引用，所以就放到APP中进行全局变量用来访问
        public List<CommandPlc> CommandPlcList = new List<CommandPlc>();
        public List<Bg_ControlVersion> ControlVersionList = new List<Bg_ControlVersion>();
        List<ResourceDictionary> dictionaryList = new List<ResourceDictionary>();

        public App()
        {
            BindAction();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SystemStartStopController.GetIns().Start();
            BindAction();
            /*
            AppDomain.CurrentDomain.UnhandledException += (s, ef) => //捕获 AppDomain 中未处理的异常
            {
                Exception ex = ef.ExceptionObject as Exception;
                MessageBox.Show("AppDomain崩溃: " + ex.Message);
            };
            */
            

            //注册Application_Error
            this.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
            TaskScheduler.UnobservedTaskException += new EventHandler<UnobservedTaskExceptionEventArgs>(TaskScheduler_UnobservedTaskException);
            CommonDeleget.AggregateExceptionCatched += CommonDeleget_AggregateExceptionCatched;
            CommonDeleget.WriteLogEvent += CommonDeleget_WriteLogEvent;
            MonitorDumps();
        }
        private void CommonDeleget_WriteLogEvent(string logMsg,string logType,bool isInsert = false)
        {
            Log.GetDistance().WriteInfoLogs(logMsg);
            if (isInsert)
            {
                InsertLogtoDbAction(logMsg, logType);
            }
        }

        /// <summary>
        /// 捕捉全局的异步Task的异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommonDeleget_AggregateExceptionCatched(object sender, AggregateExceptionArgs e)
        {
            Log.GetDistance().WriteInfoLogs(e.AggregateException.StackTrace);
            Log.GetDistance().WriteInfoLogs($"CommonDeleget_AggregateExceptionCatched SystemError，Error Result{e.AggregateException.StackTrace},{e.AggregateException.InnerException},{e.AggregateException.Message}");
            try
            {
                Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                {
                    //Application.Current?.Shutdown();
                    //System.Diagnostics.Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);
                }));
            }
            catch (Exception ex)
            {
                Log.GetDistance().WriteInfoLogs(ex.Message);
            }
        }

        void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            try
            {
                StringBuilder errorBuilder = new StringBuilder(); // 建议使用 StringBuilder 优化拼接

                foreach (Exception item in e.Exception.InnerExceptions)
                {
                    // 【核心修改】加入 StackTrace 以获取行号
                    // 格式：类型 | 来源 | 消息 | 行号详情(StackTrace)
                    string errorInfo = string.Format(
                        "Exception Type: {0}{1}" +
                        "Source: {2}{1}" +
                        "Message: {3}{1}" +
                        "Stack Trace (含行号): {4}{1}" +
                        "----------------------------------------{1}",
                        item.GetType(),
                        Environment.NewLine,
                        item.Source,
                        item.Message,
                        item.StackTrace); // <--- 这里包含了文件名和行号

                    errorBuilder.AppendLine(errorInfo);

                    // 写入日志
                    CommonDeleget.WriteLogAction(errorInfo, LogType.ApplicationError, true);
                }

                // 弹窗显示（注意：生产环境建议去掉弹窗或仅显示简略信息）
                MessageBox.Show("异步Task崩溃 (详见日志):\n" + errorBuilder.ToString());

                // 标记为已观察，防止进程崩溃
                e.SetObserved();
            }
            catch (Exception ex)
            {
                Log.GetDistance().WriteInfoLogs("处理未观察异常时出错: " + ex.Message);
            }
        }

        /// <summary>
        /// 异步Task 捕捉事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /*
        void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            try
            {
                string error = "";
                foreach (Exception item in e.Exception.InnerExceptions)
                {
                    string erroInfo = string.Format("Exception Type：{0}{1}From：{2}{3}Exception Inner：{4}",
                        item.GetType(), Environment.NewLine, item.Source,
                        Environment.NewLine, item.Message);
                    error += erroInfo;
                    CommonDeleget.WriteLogAction(erroInfo, LogType.ApplicationError, true);
                }
                MessageBox.Show("异步Task崩溃:" + error);
                //将异常标识为已经观察到 
                e.SetObserved();
            }
            catch (Exception ex)
            {
                Log.GetDistance().WriteInfoLogs(ex.Message);
            }
        }
        */

        //异常处理逻辑
        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                //在发出异常之后
                //停止之后停掉一些高压
                //先记录日志信息
                BG_MESSAGEBOX.Show(App.Current.MainWindow, "Error！", $"Error ,Click button to Restart SoftWare，Error Result  ： App_DispatcherUnhandledException{e.Exception.StackTrace},{e.Exception.Message}", 400, 600);

                Log.GetDistance().WriteInfoLogs($"Error ,Click Restart SoftWare，Error Result App_DispatcherUnhandledException{e.Exception.StackTrace},{e.Exception.Message}");
                SystemStartStopController.GetIns().Stop();
                StopAction();
                CarStopSystem();
                SX_Disconnect(intPtr);
                //BG_MESSAGEBOX.Show(App.Current.MainWindow, "Error！", $"Error ,Click button to Restart SoftWare，Error Result  ： App_DispatcherUnhandledException{e.Exception.StackTrace},{e.Exception.Message}", 400, 600);
                //Log.GetDistance().WriteInfoLogs($"Error ,Click Restart SoftWare，Error Result App_DispatcherUnhandledException{e.Exception.StackTrace},{e.Exception.Message}");
                MiniDumpServices.TryDump("error.dmp");
                Process.GetCurrentProcess().Kill();
                //Application.Current?.Shutdown();
#if RELEASE
                //System.Diagnostics.Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);
                //处理完后，我们需要将Handler=true表示已此异常已处理过
                //}
#endif
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Log.GetDistance().WriteInfoLogs(ex.Message);
            }
        }
        private void BindAction()
        {
  
            LoginSuccessEvent -= LoginResult;
            LoginSuccessEvent += LoginResult;

            RestartSystemAction -= RestartSystem;
            RestartSystemAction += RestartSystem;
        }
        private void LoginResult(List<string> ButtonList)
        {
            ButtonList = ButtonList;
            CurrentModule?.Close();
        }
        private void RestartSystem()
        {
            System.Diagnostics.Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        private ProcdumpManager _procdumpManager;
        // 在 App.xaml.cs 的 MonitorDumps 方法中修改启动逻辑
        private void MonitorDumps()
        {

            // 配置 procdump
            var config = new ProcdumpConfiguration
            {
                Enabled = true,
                DumpDirectory = @"D:\Dumps",
                ExceptionCodes = new string[] { },// "C0000409", "C0000005"
                DumpCount = 3,
                // 添加命令行参数标记，用于区分进程
                AdditionalArguments = "/procdump"
            };

            _procdumpManager = new ProcdumpManager(config); 

            // 订阅日志事件
            _procdumpManager.LogMessage += message =>
                Log.GetDistance().WriteInfoLogs($"[Procdump Log] {message}");

            _procdumpManager.ErrorOccurred += error =>
                Log.GetDistance().WriteInfoLogs($"[Procdump Error] {error}");

            // 启动监控
            if (!_procdumpManager.StartMonitoring())
            {
                Log.GetDistance().WriteInfoLogs("Procdump 启动失败");
            }
        }
    }
}
