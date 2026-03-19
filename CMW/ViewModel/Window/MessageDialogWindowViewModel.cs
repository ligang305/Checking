using System.Windows;
using System.Windows.Input;
using BG_Entities;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace CMW
{
    class MessageDialogWindowViewModel : WindowViewModelBase
    {
        private string _caption;
        public RelayCommand<KeyEventArgs> PreviewKeyDownCommand { get; private set; }
        public RelayCommand OkCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }
        public RelayCommand NoCommand { get; private set; }
        public RelayCommand YesCommand { get; private set; }

        /// <summary>
        /// 视图模型对应的对话框窗口对象
        /// </summary>
        private Window DialogWindow { get; set; }

        /// <summary>
        /// 对话框关闭后，返回的用户操作结果
        /// </summary>
        public MessageBoxResult MessageResult { get; private set; }

        /// <summary>
        /// 对话框窗口中显示的按钮类型
        /// </summary>
        private MessageBoxButton Buttons { get; set; }

        public string Caption
        {
            get { return _caption; }
            set
            {
                _caption = value;
                RaisePropertyChanged("Caption");
            }
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                RaisePropertyChanged("Message");
            }
        }

        private Visibility _showYes;

        public Visibility ShowYes
        {
            get { return _showYes; }
            set
            {
                _showYes = value;
                RaisePropertyChanged("ShowYes");
            }
        }

        private Visibility _showOk;

        public Visibility ShowOk
        {
            get { return _showOk; }
            set
            {
                _showOk = value;
                RaisePropertyChanged("ShowOk");
            }
        }

        private Visibility _showNo;

        public Visibility ShowNo
        {
            get { return _showNo; }
            set
            {
                _showNo = value;
                RaisePropertyChanged("ShowNo");
            }
        }

        private Visibility _showCancel;

        public Visibility ShowCancel
        {
            get { return _showCancel; }
            set
            {
                _showCancel = value;
                RaisePropertyChanged("ShowCancel");
            }
        }


        private string _ok = Common.Utilities.CommonDeleget.UpdateStatusNameAction("OK");

        public string ok
        {
            get { return _ok; }
            set
            {
                _ok = value;
                RaisePropertyChanged("ok");
            }
        }
        private string _yes = Common.Utilities.CommonDeleget.UpdateStatusNameAction("yes");

        public string yes
        {
            get { return _yes; }
            set
            {
                _yes = value;
                RaisePropertyChanged("yes");
            }
        }

        private string _cancel = Common.Utilities.CommonDeleget.UpdateStatusNameAction("Cancel");

        public string cancel
        {
            get { return _cancel; }
            set
            {
                _cancel = value;
                RaisePropertyChanged("cancel");
            }
        }
        private string _no = Common.Utilities.CommonDeleget.UpdateStatusNameAction("no");

        public string no
        {
            get { return _no; }
            set
            {
                _no = value;
                RaisePropertyChanged("no");
            }
        }
        public MessageDialogWindowViewModel(Window dialog, string caption, string message,
            MessageBoxButton button = MessageBoxButton.OK)
        {
            Buttons = button;
            DialogWindow = dialog;
            PreviewKeyDownCommand = new RelayCommand<KeyEventArgs>(OnPreviewKeyDown);
            OkCommand = new RelayCommand(OkCommandExecute);
            YesCommand = new RelayCommand(YesCommandExecute);
            NoCommand = new RelayCommand(NoCommandExecute);
            CancelCommand = new RelayCommand(CancelCommandExecute);

            Caption = string.IsNullOrEmpty(caption) ? "Message" : caption;
            Message = message;

            ShowYes = (button == MessageBoxButton.YesNo || button == MessageBoxButton.YesNoCancel)
                ? Visibility.Visible
                : Visibility.Collapsed;

            ShowOk = (button == MessageBoxButton.OK || button == MessageBoxButton.OKCancel)
                ? Visibility.Visible
                : Visibility.Collapsed;

            ShowNo = (button == MessageBoxButton.YesNo || button == MessageBoxButton.YesNoCancel)
                ? Visibility.Visible
                : Visibility.Collapsed;

            ShowCancel = (button == MessageBoxButton.OKCancel || button == MessageBoxButton.YesNoCancel)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void OkCommandExecute()
        {
            MessageResult = MessageBoxResult.OK;
            CloseWindow();
        }

        private void NoCommandExecute()
        {
            MessageResult = MessageBoxResult.No;
            CloseWindow();
        }

        private void YesCommandExecute()
        {
            MessageResult = MessageBoxResult.Yes;
            CloseWindow();
        }

        private void CancelCommandExecute()
        {
            MessageResult = MessageBoxResult.Cancel;
            CloseWindow();
        }

        private void CloseWindow()
        {
            DialogWindow.Close();
            this.DialogWindow = null;
        }

        private void OnPreviewKeyDown(KeyEventArgs keyEventArgs)
        {
            // 执行基类的 PreviewKeyDown 处理
            base.OnPreviewKeyDown(keyEventArgs);
            if (keyEventArgs.Handled)
            {
                // 基类已经处理，返回
                return;
            }

            switch (keyEventArgs.Key)
            {
                // F1
                case Key.F1:
                case Key.Return:
                    if (Buttons == MessageBoxButton.OK || Buttons == MessageBoxButton.OKCancel)
                    {
                        OkCommandExecute();
                        keyEventArgs.Handled = true;
                    }
                    else if (Buttons == MessageBoxButton.YesNo || Buttons == MessageBoxButton.YesNoCancel)
                    {
                        YesCommandExecute();
                        keyEventArgs.Handled = true;
                    }

                    break;
                // F2
                case Key.F2:
                case Key.Escape:
                    if (Buttons == MessageBoxButton.OKCancel || Buttons == MessageBoxButton.YesNoCancel)
                    {
                        CancelCommandExecute();
                        keyEventArgs.Handled = true;
                    }

                    break;
                // F3
                case Key.F3:
                    if (Buttons == MessageBoxButton.YesNo || Buttons == MessageBoxButton.YesNoCancel)
                    {
                        NoCommandExecute();
                        keyEventArgs.Handled = true;
                    }

                    break;
            }
        }
    }
}
