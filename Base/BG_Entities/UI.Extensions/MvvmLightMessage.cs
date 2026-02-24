using System;
using System.Windows;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Forms;

namespace BG_Entities
{
    /// <summary>
    /// Mvvmlight的消息机制，用于view和viewmodel交换消息
    /// 打开窗口消息。view注册消息，viewmodel处发送消息
    /// view管理窗口，viewmodel通过消息负责窗口的打开和关闭，
    /// </summary>
    public class OpenWindowMessage : MessageBase
    {
        /// <summary>
        /// 宿主窗口，将作为新打开窗口的所有者Owner
        /// </summary>
        public string ParentWindowKey { get; private set; }

        /// <summary>
        /// 将要打开的新窗口的Key
        /// </summary>
        public string ToOpenWindowKey { get; private set; }

        public object Parameter { get; private set; }

        public OpenWindowMessage(string parentWindowKey, string toOpenWindowKey, object param = null)
        {
            ParentWindowKey = parentWindowKey;
            ToOpenWindowKey = toOpenWindowKey;
            Parameter = param;
        }
    }

    public class CloseWindowMessage : MessageBase
    {
        /// <summary>
        /// 要关闭的窗口的Key
        /// </summary>
        public string WindowKey { get; private set; }

        public object Parameter { get; private set; }

        public CloseWindowMessage(string winKey, object param = null)
        {
            WindowKey = winKey;
            Parameter = param;
        }
    }
    /// <summary>
    /// 打开一个需要接受返回值的窗体
    /// </summary>
    public class ShowMessageDialogWindowMessageAction : NotificationMessageAction<DialogResult>
    {
        public string ParentWindowKey { get; private set; }
        public string WindowKey { get; private set; }

        public MessageBoxButton Buttons { get; private set; }

        public string Caption { get; private set; }

        public ShowMessageDialogWindowMessageAction(string parentWindowKey, string windowKey, string caption, string message, MessageBoxButton button, Action<DialogResult> action)
            : base(message, action)
        {
            ParentWindowKey = parentWindowKey;
            WindowKey = windowKey;
            Caption = caption;
            Buttons = button;
        }
    }
}
