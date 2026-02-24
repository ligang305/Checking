using System.Windows.Input;
using GalaSoft.MvvmLight;

namespace BG_Entities
{
    /// <summary>
    /// 窗体的 ViewModelBase 基类
    /// </summary>
    public abstract class WindowViewModelBase: ViewModelBase
    {
        /// <summary>
        /// 按键按下前
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnPreviewKeyDown(KeyEventArgs args)
        {
            if (Keyboard.Modifiers == ModifierKeys.Alt && args.SystemKey == Key.F4)
            {
                args.Handled = true;
            }
        }
    }
}
