using BG_Entities;
using BG_Services;
using BG_WorkFlow;
using BGLogs;
using BGModel;
using BGUserControl;
using CMW.Common.Utilities;
using ExeBaseModules;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.KeyBoard;

namespace CMW
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window 
    {
        App _App = (Application.Current as App);
        CommandConfigBLL _CommandConfigBLL = new CommandConfigBLL(controlVersion);
        ControlVersionsBLL _ControlVersionsBLL = new ControlVersionsBLL();
        HookHandlerDelegate proc = new HookHandlerDelegate(HookCallback);    
        public MainWindow()
        {
            Log.GetDistance().WriteInfoLogs("MainWindow Load");
            InitializeComponent();
            if (ConfigServices.GetInstance().localConfigModel.IsLogin)
            {
                Login();
            }
            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
            LoginResultEvent -= LoginResult;
            LoginResultEvent += LoginResult;

            Messenger.Default.Register<OpenWindowMessage>(this, OnOpenWindowMessageAction);
            Messenger.Default.Register<ShowMessageDialogWindowMessageAction>(this, ShowMessageDialogWindow);

            StartCalibrationWindow();
        }
        private void StartCalibrationWindow()
        {
            if ((DateTime.Now - ConfigServices.GetInstance().localConfigModel.LastCalibrationTime).TotalSeconds > 24 * 60 * 60)
            {
                Task.Factory.StartNew(() =>
                {
                    Messenger.Default.Send(new ShowMessageDialogWindowMessageAction(WindowKeys.MainWindowKey, WindowKeys.MessageDialogWindowKey,
                       UpdateStatusNameAction("Tip"), UpdateStatusNameAction("Calibration Tip"), MessageBoxButton.OKCancel,
                       new System.Action<DialogResult>((DialogResult _DialogResult) =>
                       {
                           if (_DialogResult == BG_Entities.DialogResult.Ok)
                           {
                               // 启动系统自检窗口
                               Messenger.Default.Send(new OpenWindowMessage(WindowKeys.MainWindowKey,
                                  WindowKeys.CalibrationWindowKey, null));
                           }
                       })));
                });
            }
        }
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            try
            {
                IsClickClose = true;
                IsModuleOpen = false;
                StopAction();
                Task.Run(() =>
                {
                    PlcService.GetInstance().Stop();
                    PLCControllerManager.GetInstance().DisConnect();
                    BoostingControllerManager.GetInstance().StopRay();
                    Thread.Sleep(100);
                    AccelatorService.GetInstance().Stop();
                });
            }
            catch (Exception ex)
            {
                CommonDeleget.HandTaskException(ex);
            }
            finally
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }
        /// <summary>
        /// 弹出消息对话框窗口
        /// </summary>
        private void ShowMessageDialogWindow(ShowMessageDialogWindowMessageAction msg)
        {
            var dispatcher = this.Dispatcher;
            dispatcher?.Invoke(() =>
            {
                if (msg.ParentWindowKey == WindowKeys.MainWindowKey)
                {
                    if (msg.WindowKey == WindowKeys.PasswordWindowKey)
                    {
                        var window = new PasswordDialogWindow(msg.Caption, msg.Notification, msg.Buttons)
                        {
                            WindowStartupLocation = WindowStartupLocation.CenterOwner,
                            Owner = this
                        };
                        window.ShowDialog();

                        DialogResult result = BG_Entities.DialogResult.No;
                        switch (window.MessageResult)
                        {
                            case MessageBoxResult.Yes:
                            case MessageBoxResult.OK:
                                {
                                    result = BG_Entities.DialogResult.Ok;
                                    break;
                                }
                            case MessageBoxResult.No:
                                {
                                    result = BG_Entities.DialogResult.No;
                                    break;
                                }
                            case MessageBoxResult.Cancel:
                                {
                                    result = BG_Entities.DialogResult.Cancel;
                                    break;
                                }
                            default:
                                {
                                    result = BG_Entities.DialogResult.Cancel;
                                    break;
                                }
                        }
                        msg.Execute(result);
                    }
                    else if (msg.WindowKey == WindowKeys.MessageDialogWindowKey)
                    {
                        var window = new MessageDialogWindow(msg.Caption, msg.Notification, msg.Buttons)
                        {
                            WindowStartupLocation = WindowStartupLocation.CenterOwner,
                            Owner = this
                        };
                        window.ShowDialog();
                        DialogResult result = BG_Entities.DialogResult.No;
                        switch (window.MessageResult)
                        {
                            case MessageBoxResult.Yes:
                            case MessageBoxResult.OK:
                                {
                                    result = BG_Entities.DialogResult.Ok;
                                    break;
                                }
                            case MessageBoxResult.No:
                                {
                                    result = BG_Entities.DialogResult.No;
                                    break;
                                }
                            case MessageBoxResult.Cancel:
                                {
                                    result = BG_Entities.DialogResult.Cancel;
                                    break;
                                }
                            default:
                                {
                                    result = BG_Entities.DialogResult.Cancel;
                                    break;
                                }
                        }
                        msg.Execute(result);
                    }
                }
            });
        }
        private void OnOpenWindowMessageAction(OpenWindowMessage msg)
        {
            var dispatcher = this.Dispatcher;
            dispatcher?.Invoke(() =>
            {
                if (msg.ParentWindowKey != WindowKeys.MainWindowKey)
                {
                    return;
                }
                else if (msg.ToOpenWindowKey == WindowKeys.CalibrationWindowKey)
                {
                    var window = new CalibrationWindow()
                    {
                        WindowStartupLocation = WindowStartupLocation.CenterOwner,
                        Owner = this
                    };
                    window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    // 设置窗口的宽和高，与图像显示区域一致，刚好覆盖图像显示区域
                    window.MinWidth = 600;
                    window.MinHeight = 400;
                    // 设置窗口的左上角与主窗口对齐
                    window.Top = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
                    window.Left = 0;
                    window.Owner = this;
                    window.ShowDialog();
                }
            });
        }
        /// <summary>
        /// 让电脑停止一些系统按键
        /// </summary>
        public void MakePro()
        {
            using (Process curPro = Process.GetCurrentProcess())
            using (ProcessModule curMod = curPro.MainModule)
            {
                SetWindowsHookExW(WH_KEYBOARD_LL, proc, GetModuleHandle(curMod.ModuleName), 0);
            }
        }

        private static int HookCallback(int nCode, IntPtr wparam, ref KBDLLHOOKSTRUCT lparam)
        {
            if (
             nCode >= 0
             &&
             (wparam == (IntPtr)WM_KEYDOWN
             ||
             wparam == (IntPtr)WM_SYSKEYDOWN)
             )
            {
                if (lparam.vkCode == 91 || lparam.vkCode == 164 || lparam.vkCode == 9 || lparam.vkCode == 115)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            return 0;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
#if DEBUG
#else
            KeyBoard.FullOrMin(this);
#endif
            InitCommand();
            TransPosition(_App.CommandPlcList);
            InitContent();
        }

        private void InitContent()
        {
            try
            {
                string ModuleNames = _App.ControlVersionList.FirstOrDefault(q => q.ControlversionKey == ConfigServices.GetInstance().localConfigModel.CMW_Version)?.ControlVersionName;
                if (!string.IsNullOrEmpty(ModuleNames))
                {
                    var modules = SystemStartStopController.GetIns().Plugins.First((q => q.Metadata.Name == ModuleNames))?.Value;
                    if (modules == null) { BG_MESSAGEBOX.Show("提示",$"加载界面失败检查是否少了{ModuleNames}dll必要文件！");return; }
                    
                    MainViewBox.Child = SystemStartStopController.GetIns().Plugins.First((q => q.Metadata.Name == ModuleNames)).Value as ExeBaseModule;
                }
            }
            catch(Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// 将命令和地址进行对应
        /// </summary>
        /// <param name="CommandPlcList"></param>
        public void TransPosition(List<CommandPlc> CommandPlcList)
        {
            CommandDic.Clear();
            foreach (var item in CommandPlcList)
            {
                if (!string.IsNullOrEmpty(item.PlcEnum))
                {
                    Command _cd = (Command)Enum.Parse(typeof(Command), item.PlcEnum, false);
                    if (!CommandDic.ContainsKey(_cd))
                    {
                        CommandDic.Add(_cd, item.PlcPosition);
                    }
                }

            }
        }

        /// <summary>
        /// 初始化命令
        /// </summary>
        private void InitCommand()
        {
            _App.ControlVersionList = _ControlVersionsBLL.GetControlVersionsBLLDataModel();
            _App.CommandPlcList = _CommandConfigBLL.GetCommandPlcDataModel(controlVersion);
        }

        private void Login()
        {
            Login lg = new Login();
            lg.ShowDialog();

            if (ConfigServices.GetInstance().localConfigModel.Login.LoginStatus != LoginStatus.Success)
            {
                Close();
            }
        }

        /// <summary>
        /// 登录结果
        /// </summary>
        /// <param name="IsResult"></param>
        /// <param name="ButtonList"></param>
        private void LoginResult(LoginStatus IsResult, List<string> ButtonList)
        {
            this.Dispatcher.BeginInvoke(new Action(() => {
                switch (IsResult)
                {
                    case LoginStatus.Success:
                        LoginSuccessAction(ButtonList);
                        break;
                    case LoginStatus.Faild:
                        BG_MESSAGEBOX.ShowLogin(this, UpdateStatusNameAction("Tip"), UpdateStatusNameAction("LoginFaild"));
                        break;
                    case LoginStatus.UnAuthorized:
                        BG_MESSAGEBOX.ShowLogin(this, UpdateStatusNameAction("Tip"), UpdateStatusNameAction("UnAuthorized"));
                        break;
                    case LoginStatus.LinkServerFaild:
                        BG_MESSAGEBOX.ShowLogin(this, UpdateStatusNameAction("Tip"), UpdateStatusNameAction("LinkServerFaild"));
                        break;
                    case LoginStatus.FTPServerFaild:
                        BG_MESSAGEBOX.ShowLogin(this, UpdateStatusNameAction("Tip"), UpdateStatusNameAction("FTPServerFaild"));
                        break;
                    case LoginStatus.InvalidAuthorizationCode:
                        BG_MESSAGEBOX.ShowLogin(this, UpdateStatusNameAction("Tip"), UpdateStatusNameAction("InvalidAuthorizationCode"));
                        break;
                    case LoginStatus.AuthorizationCodeExpired:
                        BG_MESSAGEBOX.ShowLogin(this, UpdateStatusNameAction("Tip"), UpdateStatusNameAction("AuthorizationCodeExpired"));
                        break;
                    case LoginStatus.TimeOut:
                        BG_MESSAGEBOX.ShowLogin(this, UpdateStatusNameAction("Tip"), UpdateStatusNameAction("TimeOut"));
                        break;
                    case LoginStatus.AuthorizationCodeNull:
                        BG_MESSAGEBOX.ShowLogin(this, UpdateStatusNameAction("Tip"), UpdateStatusNameAction("AuthorizationCodeNull"));
                        break;
                    case LoginStatus.UnSuchUser:
                        BG_MESSAGEBOX.ShowLogin(this, UpdateStatusNameAction("Tip"), UpdateStatusNameAction("UserNotExist"));
                        break;
                    case LoginStatus.PasswordError:
                        string PwdError = UpdateStatusNameAction("PasswordError");
                        BG_MESSAGEBOX.ShowLogin(this, UpdateStatusNameAction("Tip"), PwdError);
                        break;
                    case LoginStatus.Cancel:
                        string Tip = UpdateStatusNameAction("Tip");
                        string Content = UpdateStatusNameAction("CancelSure");
                        if (MessageBox.Show(Content, Tip, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            Application.Current?.Shutdown();
                        }
                        break;
                    default:
                        break;
                }
            }));
        }

    }
}
