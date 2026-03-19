using System.Windows;

namespace CMW
{
    /// <summary>
    /// MessageDialogWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MessageDialogWindow
    {
        /// <summary>
        /// 对话框关闭后的返回值
        /// </summary>
        public MessageBoxResult MessageResult
        {
            get { return ViewModel.MessageResult; }
        }

        /// <summary>
        /// 对话框窗口对应的视图模型
        /// </summary>
        private MessageDialogWindowViewModel ViewModel { get; set; }

        public MessageDialogWindow(string caption, string message,MessageBoxButton button = MessageBoxButton.OK)
        {
            InitializeComponent();
            DataContext = ViewModel = new MessageDialogWindowViewModel(this, caption, message, button);

        }
    }
}
