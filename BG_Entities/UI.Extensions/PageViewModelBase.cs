using System.Windows.Input;
using GalaSoft.MvvmLight;

namespace BG_Entities
{
    /// <summary>
    /// 菜单page基类，用于从窗口SystemMenuwindowc处响应按键消息
    /// 如果page需要处理按键消息，则需要重写基类中的按键处理函数
    /// </summary>
    public abstract class PageViewModelBase:ViewModelBase
    {
        public abstract void OnKeyDown(KeyEventArgs args);

        public virtual void OnPreviewKeyDown(KeyEventArgs args)
        {
        }

        public virtual void OnPreviewKeyUp(KeyEventArgs args)
        {
        }
    }
}
