using System.Windows;

namespace CMW
{
    /// <summary>
    /// PasswordDialogWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PasswordDialogWindow
    {
        /// <summary>
        /// 对话框关闭后的返回值
        /// </summary>
        public MessageBoxResult MessageResult
        {
            get { return ViewModel.MessageResult; }
        }

        /// <summary>
        /// 密码对话框窗口对应的视图模型
        /// </summary>
        private PasswordDialogWindowViewModel ViewModel { get; set; }

        public PasswordDialogWindow(string caption, string message,MessageBoxButton button = MessageBoxButton.OK)
        {
            InitializeComponent();
            DataContext = ViewModel = new PasswordDialogWindowViewModel(this, caption, message, button);
        }
    }
}
