namespace BG_Entities
{
    /// <summary>
    /// Metro对话框的执行结果：分别表示用户点击了对应的处理按键
    /// </summary>
    public enum DialogResult
    {
        Ok,
        No,
        Cancel
    }

    /// <summary>
    /// 输入对话框的结果：包括用户操作的结果，以及
    /// </summary>
    public class InputDialogResult
    {
        public InputDialogResult(DialogResult result, string input = null)
        {
            DialogResult = result;
            Input = input;
        }

        public DialogResult DialogResult { get; private set; }

        public string Input { get; private set; }
    }

    /// <summary>
    /// 输入对话框的结果：包括用户操作的结果，以及
    /// </summary>
    public class PasswordDialogResult
    {
        public PasswordDialogResult(DialogResult result, string password = null)
        {
            DialogResult = result;
            Password = password;
        }

        public DialogResult DialogResult { get; private set; }

        public string Password { get; private set; }
    }

    /// <summary>
    /// sjw添加输入对话框的结果：包括用户操作的结果，以及
    /// </summary>
    public class DisplaySysCalResult
    {
        public DisplaySysCalResult(DialogResult result, string windowkey = null)
        {
            DialogResult = result;
            Windowkey = windowkey;
        }

        public DialogResult DialogResult { get; private set; }

        public string Windowkey { get; private set; }
    }
}
