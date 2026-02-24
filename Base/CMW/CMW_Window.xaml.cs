using BG_Entities;
using BG_Services;
using BG_WebServices;
using BG_WorkFlow;
using BGLogs;
using BGModel;
using BGUserControl;
using CMW.Common.Utilities;
using CMW.ViewModel;
using ExeBaseModules;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.KeyBoard;
using System.ComponentModel.Composition;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Windows.Interop;
using DialogResult = BG_Entities.DialogResult;
using Application = System.Windows.Application;
using BGUserControl.MainWindowUserControl;
namespace CMW
{
    /// <summary>
    /// CMW_Window.xaml 的交互逻辑
    /// </summary>
    public partial class CMW_MainModule : MetroWindow
    {
        MainViewModel mainViewModel;
        bool IsCalibration = false;
        public CMW_MainModule()
        {
            InitializeComponent();
            /*
            if (ConfigServices.GetInstance().localConfigModel.IsLogin)
            {
                Login();
            }
            */
            NotifyIconHelp.GetIns().initNotifyIcon(this);
            Loaded += CMW_MainModule_Loaded;
            SizeChanged += CMW_MainModule_SizeChanged; //CMW_MainModule_SizeChanged
            Closed += CMW_MainModule_Closed;
            Messenger.Default.Register<OpenWindowMessage>(this, OnOpenWindowMessageAction);
            Messenger.Default.Register<ShowMessageDialogWindowMessageAction>(this, ShowMessageDialogWindow);
        }


        private void ReSizeWindow()
        {
            try
            {
                DisplayMonitor.RefreshActualScreens();
                this.ResizeMode = ResizeMode.CanResize; // ResizeMode
                Log.GetDistance().WriteInfoLogs($@"DisplayMonitor.ActualScreens_{DisplayMonitor.ActualScreens.Count()}");
                if (DisplayMonitor.ActualScreens.Count() >= 2)
                {
                    this.Width = DisplayMonitor.ActualScreens.Sum(q => q.Bounds.Width);
                    this.Height = SystemParameters.VirtualScreenHeight;
                }
                else
                {
                    this.Width = DisplayMonitor.ActualScreens.Sum(q => q.Bounds.Width);
                    this.Height = SystemParameters.PrimaryScreenHeight;
                }
                Log.GetDistance().WriteInfoLogs($@"Width_{this.Width}.Height_{this.Height}");
                this.ResizeMode = ResizeMode.NoResize;
                this.Top = 0.0;
                this.Left = 0.0;
            }
            catch (Exception ex)
            {
                Log.GetDistance().WriteInfoLogs(ex.Message);
                Log.GetDistance().WriteInfoLogs(ex.StackTrace);
            }
        }

        private void CMW_MainModule_SizeChanged(object sender, SizeChangedEventArgs e)
        { 
            ReSizeWindow();
            //this.WindowState = WindowState.Minimized;
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }
        protected IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // 在此处处理消息
            switch (msg)
            {
                case (int)DisplayMonitor.WM_DISPLAYCHANGE:
                    CMW_MainModule_SizeChanged(this, null);
                    break;
            }

            return IntPtr.Zero;
        }
        private void CMW_MainModule_Closed(object sender, EventArgs e)
        {
            SystemStartStopController.GetIns().Stop();
            System.Diagnostics.Process.GetCurrentProcess().Kill(); 
        }
        bool isFirstLoad = false;
        private void CMW_MainModule_Loaded(object sender, RoutedEventArgs e)
        {
#if DEBUG
            //KeyBoard.FullOrMin(this);
#else
            //KeyBoard.FullOrMin(this);
#endif
            mainViewModel = (MainViewModel)this.DataContext;
            mainViewModel.topStatusMonitorPanel = topPanel;
            mainViewModel.centerStatusControlPanel = centerPanel;
            mainViewModel.LoadedEventCommand.Execute(mainViewModel);
            
            if (!isFirstLoad)
            {
                isFirstLoad = true;
                if (ConfigServices.GetInstance().localConfigModel.IsUserBS)
                {
                    BS2000Panel bS2000Panel = new BS2000Panel()
                    {
                        VerticalAlignment = VerticalAlignment.Stretch,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                    };
                    mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    Grid.SetColumn(bS2000Panel, 1);
                    mainGrid.Children.Add(bS2000Panel); // ColumnDefinition
                }
            }
            
            InitTestMode();
        }

        /*
         每次打开CMW将测试模式置为false
         */
       
        private void InitTestMode() {
            CMW.Common.Utilities.Common.SetCommand(CommandDic[Command.TestMode], false);
        }

        private void OnOpenWindowMessageAction(OpenWindowMessage msg)
        {
            var dispatcher = this.Dispatcher;
            dispatcher?.Invoke(() =>
            {
                if (msg.ParentWindowKey != WindowKeys.MainWindowKey) // it is of a false string
                {
                    return;
                }

                // 打开系统菜单窗口，并且根据参数，初始化配置窗口的默认配置页
                if (msg.ToOpenWindowKey == WindowKeys.SystemReadyConditionWindow)
                {
                    try
                    {
                        string ModulesName = msg.Parameter as string; 
                        if (mainViewModel.HitchModels.Keys.Any(q=>q.Contains(ModulesName)))
                        {
                            foreach (KeyValuePair<string, ObservableCollection<object>> kvp in mainViewModel.HitchModels)
                            {
                                Console.WriteLine($"Key-------> {kvp.Key}");
                            }
                            ShowDiySafeCondition(ModulesName);
                            return;
                        }
                        if (mainViewModel.BSHitchModels != null && mainViewModel.BSHitchModels.Keys.Contains(ModulesName))
                        {
                            ShowDiySafeCondition(ModulesName, true);
                            return;
                        }
                        switch (ModulesName)
                        {
                            case "MainPage":
                                ViewModelLocator.Locator.CurrentModule?.Close();
                                break;
                            default:
                                ShowModules(ModulesName, controlVersion);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonDeleget.WriteLogAction(ex.Message, LogType.ApplicationError);
                    }
                }
                // 打开系统菜单窗口，并且根据参数，初始化配置窗口的默认配置页
                if (msg.ToOpenWindowKey == WindowKeys.SystemMenuWindowKey)
                {
                    if (msg.Parameter is PageInfo page)
                    {
                        ShowDialogInClientArea(new SystemMenuWindow(page));
                    }
                }
                else if (msg.ToOpenWindowKey == WindowKeys.CalibrationWindowKey)
                {
                    var window = new CalibrationWindow()
                    {
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        Owner = this
                    };
                    window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    // 设置窗口的宽和高，与图像显示区域一致，刚好覆盖图像显示区域
                    window.MinWidth = 600;
                    window.MinHeight = 400;
                    // 设置窗口的左上角与主窗口对齐
                    window.Top = topPanel.ActualHeight;
                    window.Left = 0;
                    window.Owner = this;
                    // window.ShowDialog();
                    window.Show();

                    //DialogResult result = BG_Entities.DialogResult.No;
                    //switch (window.MessageResult)
                    //{
                    //    case MessageBoxResult.Yes:
                    //    case MessageBoxResult.OK:
                    //        {
                    //            result = BG_Entities.DialogResult.Ok;
                    //            break;
                    //        }
                    //    case MessageBoxResult.No:
                    //        {
                    //            result = BG_Entities.DialogResult.No;
                    //            break;
                    //        }
                    //    case MessageBoxResult.Cancel:
                    //        {
                    //            result = BG_Entities.DialogResult.Cancel;
                    //            break;
                    //        }
                    //    default:
                    //        {
                    //            result = BG_Entities.DialogResult.Cancel;
                    //            break;
                    //        }
                    //}
                    ////msg.Execute(result);
                }
            });
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
                            WindowStartupLocation = WindowStartupLocation.CenterScreen,
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
                            WindowStartupLocation = WindowStartupLocation.CenterScreen,
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

        /// <summary>
        /// 在主窗口的客户端区域，显示对话框窗口，对话框窗口将占据整个客户区域
        /// </summary>
        /// <param name="window"></param>
        private void ShowDialogInClientArea(Window window)
        {
            window.WindowStartupLocation = WindowStartupLocation.Manual;

            // 设置窗口的宽和高，与图像显示区域一致，刚好覆盖图像显示区域
            window.MinWidth = window.MaxWidth = this.ActualWidth;
            window.MinHeight = window.MaxHeight =
                this.ActualHeight - topPanel.ActualHeight;

            // 设置窗口的左上角与主窗口对齐
            window.Top = topPanel.ActualHeight;
            window.Left = 0;

            window.Owner = this;
            window.ShowDialog();
        }

        /// <summary>
        /// ShowModuels
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="cv"></param>
        protected void ShowModules(string Name, ControlVersion cv)
        {
            bool isFind = FindModules(Name, cv);
            if (isFind) ShowModules();
        }
        /// <summary>
        /// 找到模块
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="cv"></param>
        /// <returns></returns>
        protected bool FindModules(string Name, ControlVersion cv)
        {
            if (ViewModelLocator.Locator.UserControlPlugins.FirstOrDefault(q => Name == q.Metadata.Name) == null)
            {
                BG_MESSAGEBOX.Show(UpdateStatusNameAction("Tip"), UpdateStatusNameAction("ModulesLost"));
                return false;
            }
            else
            {
                //Messenger.Default.Send(new OpenWindowMessage(WindowKeys.MainWindowKey, WindowKeys.SystemMenuWindowKey,
                //new PageInfo(WindowKeys.MainMenuKey, WindowKeys.SettingPage, WindowKeys.SystemMenuWindowTitle)));
                ViewModelLocator.Locator.CurrentModule?.Close();
                ViewModelLocator.Locator.CurrentModule = ViewModelLocator.Locator.UserControlPlugins.First(q => Name == (q.Metadata.Name)).Value;
                ViewModelLocator.Locator.CurrentModule.SetCarVersion(cv);
                ViewModelLocator.Locator.CurrentModule.SetSelectTabName(string.Empty); // = string.Empty;
                return true;
            }
        }

        /// <summary>
        /// 显示模块
        /// </summary>
        protected void ShowModules()
        {
            //ViewModelLocator.Locator.CurrentModule?.Close();
            ViewModelLocator.Locator.CurrentModule.Show(new MetroWindow()
            {
                Owner = Application.Current?.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                WindowStyle = WindowStyle.None,
                UseNoneWindowStyle = true,
                ResizeMode = ResizeMode.NoResize,
                Background = (SolidColorBrush)this.TryFindResource("newBlue"),
                AllowsTransparency = true
            });
        }
        /// <summary>
        /// 弹出状态面板
        /// </summary>
        /// <param name="diyTag"></param>
        private void ShowDiySafeCondition(string diyTag, bool isBs = false)
        {
            ViewModelLocator.Locator.CurrentModule?.Close();
            ViewModelLocator.Locator.CurrentModule = new DiySafeCondition();
            ObservableCollection<object> ItemSource = new ObservableCollection<object>();
            if (!isBs)
            {
                if (mainViewModel.HitchModels.Keys.Any(q => q.Contains(diyTag)))
                {
                    ItemSource = mainViewModel.HitchModels.FirstOrDefault(q=>q.Key.Contains(diyTag)).Value;
                }
            }
            else
            {
                
                if (mainViewModel.BSHitchModels !=null && mainViewModel.BSHitchModels.ContainsKey(diyTag))
                {
                    ItemSource = mainViewModel.BSHitchModels[diyTag];
                }
            }

            (ViewModelLocator.Locator.CurrentModule as DiySafeCondition).ItemSource = new ObservableCollection<object>(ItemSource.ToArray());
            (ViewModelLocator.Locator.CurrentModule as DiySafeCondition).Title = diyTag;
            ViewModelLocator.Locator.CurrentModule.Show(
                    new MetroWindow()
                    {
                        Owner = Application.Current?.MainWindow,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        WindowStyle = WindowStyle.None,
                        UseNoneWindowStyle = true,
                        ResizeMode = ResizeMode.NoResize,
                        Background = (SolidColorBrush)this.TryFindResource("newBlue"),
                        AllowsTransparency = true
                    }
                    );
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

        
    }
}
