using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using GalaSoft.MvvmLight.Threading;
using CMW.ViewModel;
using BG_Entities;
using CMW.Common.Utilities;

namespace CMW
{
    /// <summary>
    /// SystemMenuWindow.xaml 的交互逻辑
    /// MenuWindow是包含菜单界面的容器，包括菜单页面和菜单项内容页面和标题栏
    /// 
    /// </summary>
    public partial class SystemMenuWindow
    {
        /// <summary>
        /// 当前是否正在显示一个Metro对话框
        /// </summary>
        private bool _hasMetroDialog = false;

        public SystemMenuWindow(PageInfo page)
        {
            InitializeComponent();

            ViewModelLocator.MenuPageNavigationService.SetFrame(MenuFrame, PageFrame);
            ViewModelLocator.MenuPageNavigationService.ShowPage(page);

            // 屏蔽Page的Back键，该键用于导航的goback功能。
            NavigationCommands.BrowseBack.InputGestures.Clear();

            //放置page的Frame获取较短
            PageFrame.Focus();

            // 注册信使消息，以便于从vm中关闭此窗口
            Messenger.Default.Register<CloseWindowMessage>(this, CloseWindowMessageAction);
            Messenger.Default.Register<OpenWindowMessage>(this, OpenWindowMessageAction);
            Messenger.Default.Register<ShowMessageDialogWindowMessageAction>(this, ShowMessageDialogWindow);
           
            this.Closed += OnClosed;
            this.Closing += (sender, args) =>
            {
                if (_hasMetroDialog)
                {
                    args.Cancel = true;
                }
            };

            // 定时将窗口置顶，捕获按键消息
            //WindowFocusHelper.MakeFocus(this);
        }

        private void CloseWindowMessageAction(CloseWindowMessage msg)
        {
            if (msg.WindowKey == WindowKeys.SystemMenuWindowKey)
            {
                DispatcherHelper.CheckBeginInvokeOnUI(new System.Action(() =>
                {
                    Messenger.Default.Unregister(this);
                    this.Close();
                }));
            }
        }


        /// <summary>
        /// 弹出功能窗口
        /// </summary>
        /// <param name="msg"></param>
        private void OpenWindowMessageAction(OpenWindowMessage msg)
        {
           
        }


        /// <summary>
        /// 弹出消息对话框窗口
        /// </summary>
        private void ShowMessageDialogWindow(ShowMessageDialogWindowMessageAction msg)
        {
            var dispatcher = this.Dispatcher;
            dispatcher?.Invoke(() =>
            {
                if(msg.ParentWindowKey == WindowKeys.SystemMenuWindowKey)
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

        /// <summary>
        /// 窗口关闭事件：在窗口关闭，清理最后一个页面占用的资源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosed(object sender, EventArgs e)
        {
            if (MenuFrame.Content is Page menuPage)
            {
                if (menuPage.DataContext is ICleanup vmCleanup)
                {
                    try
                    {
                        vmCleanup.Cleanup();
                    }
                    catch (Exception exception)
                    {
                        CommonDeleget.WriteLogAction(exception.ToString(),LogType.ApplicationError);
                    }
                }
            }

            if (PageFrame.Content is Page page)
            {
                if (page.DataContext is ICleanup vmCleanup)
                {
                    try
                    {
                        vmCleanup.Cleanup();
                    }
                    catch (Exception exception)
                    {
                        CommonDeleget.WriteLogAction(exception.ToString(), LogType.ApplicationError);
                    }
                }
            }
        }

        /// <summary>
        /// 传递窗口的按键信息到page的viewmodel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SystemMenuWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (PageFrame.Content is Page content)
            {
                if (content.DataContext is PageViewModelBase vm)
                {
                    vm.OnKeyDown(e);
                }
            }
        }

        private void SystemMenuWindow_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (PageFrame.Content is Page content)
            {
                if (content.DataContext is PageViewModelBase vm)
                {
                    vm.OnPreviewKeyDown(e);
                }
            }
        }

        private void SystemMenuWindow_OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (PageFrame.Content is Page content)
            {
                if (content.DataContext is PageViewModelBase vm)
                {
                    vm.OnPreviewKeyUp(e);
                }
            }
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Unregister(this);
            this.Close();
        }
    }
}
