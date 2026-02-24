using System;
using System.Windows.Controls;

namespace BG_Entities
{
    public class PageNavigationEventArgs : EventArgs
    {
        public PageInfo Info { get; private set; }

        public PageNavigationEventArgs(PageInfo page)
        {
            Info = page;
        }
    }

    /// <summary>
    /// 页信息
    /// </summary>
    public class PageInfo
    {
        public string Title { get; private set; }

        public string MenuKey { get; private set; }

        public string PageKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="menuKey">要显示的菜单页的Key</param>
        /// <param name="pageKey">要显示的配置页的Key</param>
        /// <param name="title">窗口将要显示的标题</param>
        public PageInfo(string menuKey, string pageKey, string title)
        {
            MenuKey = menuKey;
            PageKey = pageKey;
            Title = title;
        }
    }

    /// <summary>
    /// 接口：用于一个窗口中的Page导航
    /// </summary>
    public interface IPageNavigationService
    {
        event EventHandler<PageNavigationEventArgs> PageNavigated;

        PageInfo CurrentPage { get; }

        string CurrentPageKey { get; }

        /// <summary>
        /// 设置窗口中的Frame对象，通过此对象实现导航
        /// </summary>
        /// <param name="menuFrame"></param>
        /// <param name="pageFrame">默认显示的Page</param>
        void SetFrame(Frame menuFrame, Frame pageFrame);

        /// <summary>
        /// 显示指定的菜单和页：包括菜单页和设置页
        /// </summary>
        /// <param name="navigateToPage"></param>
        /// <param name="param"></param>
        void ShowPage(PageInfo navigateToPage, object param = null);

        /// <summary>
        /// 显示指定的页：包括设置页，不更新菜单，也不更新标题
        /// </summary>
        /// <param name="pageKey"></param>
        /// <param name="param"></param>
        void ShowPage(string pageKey, object param = null);

        void GoBack();
    }
}
