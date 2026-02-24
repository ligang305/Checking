using BGModel;
using BGUserControl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BG_Services;
using BG_Entities;
using BG_WorkFlow;
using CMW.Common.Utilities;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.KeyBoard;
using static CMW.Common.Utilities.Common;
namespace CMW
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login
    {

        #region 静态变量
        HookHandlerDelegate proc = new HookHandlerDelegate(HookCallback);
        App _App = (Application.Current as App);
        #endregion

        public Login()
        {
            try
            {
                InitializeComponent();
                this.Width = SystemParameters.PrimaryScreenWidth;
                this.ResizeMode = System.Windows.ResizeMode.NoResize;
                Loaded += Login_Loaded;
                Closed += Login_Closed;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            InitLanguageDataSource();
        }
        /// <summary>
        /// 初始化语言下拉数据源
        /// </summary>
        private void InitLanguageDataSource()
        {
            ddlLanguage.ItemsSource = LanguageServices.GetInstance().LanguageList;
            ddlLanguage.DisplayMemberPath = "languageName";
            ddlLanguage.SelectedValuePath = "LanguageKey";
            ddlLanguage.SelectionChanged += DdlLanguage_SelectionChanged;
            try
            {
                foreach (var item in ddlLanguage.Items)
                {
                    if ((item as LanguageModel).LanguageKey ==ConfigServices.GetInstance().localConfigModel.LANGUAGE)
                    {
                        ddlLanguage.SelectedItem = item;
                        break;
                    }
                }
            }
            catch
            {
                //Log.GetDistance().WriteInfoLogs(ex.Message);
            }
        }

        private void DdlLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox mi = sender as ComboBox;
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //Log.GetDistance().WriteInfoLogs($"更新配置文件,设置语言为{mi.SelectedValue as string}");
            CommonDeleget.UpdateConfigs("language", mi.SelectedValue as string, Section.SOFT);
            ConfigServices.GetInstance().localConfigModel.LANGUAGE = mi.SelectedValue as string;
            BuryingPoint($"Setting Language:{ConfigServices.GetInstance().localConfigModel.LANGUAGE}");
            
            if (ButtonInvoke.SwitchLanguageEvent != null && ButtonInvoke.SwitchLanguageEvent.GetInvocationList().Count() != 0)
            {
                ButtonInvoke.SwitchLanguageEvent?.Invoke(mi.SelectedValue as string);
            }
            //lblSoftName.Content = UpdateStatusNameAction("MonitorStation");
        }

        private void Login_Closed(object sender, EventArgs e)
        {

        }

        private void Login_Loaded(object sender, RoutedEventArgs e)
        {
#if DEBUG
#else
            KeyBoard.FullOrMinMutiScreen(this);
#endif

            //MakePro();
            //lblSoftName.Content = UpdateStatusNameAction("MonitorStation"); 
            InitLogin();
            LoginResultEvent -= LoginResult;
            LoginResultEvent += LoginResult;
        }
        /// <summary>
        /// 使软件全屏
        /// </summary>
        public void MakePro()
        {
            using (Process curPro = Process.GetCurrentProcess())
            using (ProcessModule curMod = curPro.MainModule)
            {
                SetWindowsHookExW(WH_KEYBOARD_LL, proc, GetModuleHandle(curMod.ModuleName), 0);
            }
        }
        /// <summary>
        /// 钩子屏蔽键盘按键
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wparam"></param>
        /// <param name="lparam"></param>
        /// <returns></returns>
        private static int HookCallback(int nCode, IntPtr wparam, ref KBDLLHOOKSTRUCT lparam)
        {
            if (nCode >= 0 && (wparam == (IntPtr)WM_KEYDOWN || wparam == (IntPtr)WM_SYSKEYDOWN))
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
                            Process.GetCurrentProcess().Kill();
                            //Application.Current?.Shutdown();
                        }
                        break;
                    default:
                        break;
                }
            }));
        }
        /// <summary>
        /// 初始化登录
        /// </summary>
        private void InitLogin()
        {
            var LoginModulesName = Command.LoginModules.ToString();
            if (SystemStartStopController.GetIns().Plugins.FirstOrDefault(q => q.Metadata.Name == LoginModulesName) == null)
            {
                MessageBox.Show("软件出错！即将退出！");
                Application.Current?.Shutdown();
            }
            else
            {
                _App.CurrentModule?.Close();
                _App.CurrentModule = SystemStartStopController.GetIns().Plugins.First((q => q.Metadata.Name == LoginModulesName)).Value;
                _App.CurrentModule.SetCarVersion(controlVersion);
                _App.CurrentModule.Show(this);
                var uc = _App.CurrentModule as UserControl;
                if (uc != null)
                {
                    uc.Background = new ImageBrush(new BitmapImage(new Uri("pack://Application:,,,/Properties/LoginTextBackGround.png", UriKind.RelativeOrAbsolute))) { Stretch = Stretch.Uniform };
                }
                LoginContent.Content = _App.CurrentModule;
            }
        }
    }
}
