using System;
using System.Windows;
using System.Windows.Input;
using BG_Entities;
using GalaSoft.MvvmLight.Command;
namespace CMW
{
    public class SystemMenuWindowViewModel : WindowViewModelBase
    {

        public RelayCommand<KeyEventArgs> KeyDownCommand { get; set; }

        public RelayCommand<KeyEventArgs> KeyUpCommand { get; set; }

        public RelayCommand<KeyEventArgs> PreviewKeyDownCommand { get; set; }

        public RelayCommand<KeyEventArgs> PreviewKeyUpCommand { get; set; }



        private IPageNavigationService _navigationService;

        private Visibility _mainPageButtonVisibility = Visibility.Collapsed;
        /// <summary>
        /// 主页按钮是否显示
        /// </summary>
        public Visibility MainPageButtonVisibility
        {
            get { return _mainPageButtonVisibility; }
            set
            {
                _mainPageButtonVisibility = value;
                RaisePropertyChanged();
            }
        }


        private Visibility _returnButtonVisibility = Visibility.Collapsed;
        /// <summary>
        /// 返回按钮是否显示
        /// </summary>
        public Visibility ReturnButtonVisibility
        {
            get { return _returnButtonVisibility; }
            set
            {
                _returnButtonVisibility = value;
                RaisePropertyChanged();
            }
        }

        private HorizontalAlignment _pageTitleTextBlockAlignment = HorizontalAlignment.Center;

        public HorizontalAlignment PageTitleTextBlockAlignment
        {
            get { return _pageTitleTextBlockAlignment; }
            set
            {
                _pageTitleTextBlockAlignment = value;
                RaisePropertyChanged();
            }
        }

        private bool _isMainPageButtonChecked = false;
        public bool IsMainPageButtonChecked
        {
            get { return _isMainPageButtonChecked; }
            set { _isMainPageButtonChecked = value; RaisePropertyChanged(); }
        }

        public SystemMenuWindowViewModel(IPageNavigationService service)
        {
            _navigationService = service ?? throw new ArgumentNullException("service");
            _navigationService.PageNavigated += _navigationService_PageNavigated;

            KeyDownCommand = new RelayCommand<KeyEventArgs>(KeyDownCommandExecute);
            PreviewKeyDownCommand = new RelayCommand<KeyEventArgs>(PreviewKeyDownCommandExecute);
            PreviewKeyUpCommand = new RelayCommand<KeyEventArgs>(PreviewKeyUpCommandExecute);
        }

        private void _navigationService_PageNavigated(object sender, PageNavigationEventArgs e)
        {
            IsMainPageButtonChecked = false;

            bool isMainMenu = e.Info.MenuKey == WindowKeys.MainMenuKey;
            MainPageButtonVisibility =
                isMainMenu ? Visibility.Collapsed : Visibility.Visible;
            ReturnButtonVisibility = MainPageButtonVisibility;
            PageTitleTextBlockAlignment = isMainMenu ? HorizontalAlignment.Center : HorizontalAlignment.Left;
        }

        private void PreviewKeyUpCommandExecute(KeyEventArgs args)
        {
            //处理一些界面按键，主要用于将专用键盘的按键映射为page界面中用于输入的按键
            if ((args.KeyboardDevice.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
            {

            }
        }

        private void PreviewKeyDownCommandExecute(KeyEventArgs args)
        {
            // 执行基类的 PreviewKeyDown 处理
            OnPreviewKeyDown(args);
            if (args.Handled)
            {
                // 基类已经处理，返回
                return;
            }
        }

        private void KeyDownCommandExecute(KeyEventArgs args)
        {
            var key = args.Key;

            if ((args.KeyboardDevice.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
            {
                // 用户按下F3功能键或者计算机键盘中的Esc键时，关闭设置窗口
                // 在KeyDown中处理，而不是在PreviewKeyDown中处理，是因为设置窗口中可能会出现Metro对话框；
                // 在显示Metro对话框的时候，不允许直接关闭窗口。
                if (key == Key.F3 || key == Key.Escape)
                {
                    MessengerInstance.Send(new CloseWindowMessage(WindowKeys.SystemMenuWindowKey));
                    args.Handled = true;
                }
            }
        }


        public override void Cleanup()
        {
            _navigationService.PageNavigated -= _navigationService_PageNavigated;

            //todo:执行一些清理
            base.Cleanup();
        }
    }
}
