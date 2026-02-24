using BG_Entities;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace CMW
{
    /// <summary>
    /// CalibrationWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CalibrationWindow
{     

        public MessageBoxResult MessageResult
        {
            get { return ViewModel.MessageResult; }
        }
        /// <summary>
        /// 窗口对应的视图模型
        /// </summary>
        private CalibrationWindowViewModel ViewModel { get; set; }

        public CalibrationWindow()//string caption, string message,MessageBoxButton button = MessageBoxButton.OK
        {
            InitializeComponent();

            Messenger.Default.Register<CloseWindowMessage>(this, CloseWindowMessageAction);

            DataContext = ViewModel = new CalibrationWindowViewModel();//this,caption, message, button
        }

        /// <summary>
        /// 注销Messenger，并关闭窗口
        /// </summary>
        /// <param name="message"></param>
        private void CloseWindowMessageAction(CloseWindowMessage msg)
        {
            if (msg.WindowKey == WindowKeys.CalibrationWindowKey)
            {
                Messenger.Default.Unregister(this);
                this.Dispatcher?.Invoke(this.Close);
            }
        }
    }
}
