using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using GalaSoft.MvvmLight;

namespace BG_Entities
{
    public class PageNanigationservice:IPageNavigationService
    {
        /// <summary>
        /// 记录所有page的标识字符串和地址，用于基于page名导航
        /// </summary>
        private readonly Dictionary<string, Uri> _pageDictionaries;
        /// <summary>
        /// 当前的打开的pages，用于前进后退。一般保存两个page，主菜单和子项
        /// todo:后续考虑记录父窗口
        /// </summary>
        private readonly List<PageInfo> _navigations = new List<PageInfo>(2);

        /// <summary>
        /// 当前打开的page的key
        /// </summary>
        private string _currentPageKey;
        public string CurrentPageKey
        {
            get { return _currentPageKey; }
            private set
            {
                if (_currentPageKey != value)
                {
                    _currentPageKey = value;
                }
            }
        }
        /// <summary>
        /// 记录一些参数
        /// </summary>
        public object Parameter { get; private set; }

        /// <summary>
        /// 当前page,带标题
        /// </summary>
        public PageInfo CurrentPage { get; private set; }
        /// <summary>
        /// 导航事件
        /// </summary>
        public event EventHandler<PageNavigationEventArgs> PageNavigated;

        public PageNanigationservice()
        {
            _pageDictionaries = new Dictionary<string, Uri>();
        }

        public void GoBack()
        {
            if (_navigations.Count > 1)
            {
                _navigations.RemoveAt(_navigations.Count - 1);
                ShowPage(_navigations.Last());
            }
        }

        public void ShowPage(string pageKey, object param = null)
        {
            lock (_pageDictionaries)
            {
                if (!string.IsNullOrEmpty(pageKey))
                {
                    if (!_pageDictionaries.ContainsKey(pageKey))
                        throw new ArgumentException(string.Format("No such page: {0} ", pageKey), "pageKey");

                    var frame = PageFrame;
                    var uri = _pageDictionaries[pageKey];
                    if (frame != null && frame.Source != uri)
                    {
                        var page = PageFrame.Content as Page;
                        if (page != null)
                        {
                            var vmCleanup = page.DataContext as ICleanup;
                            if (vmCleanup != null)
                            {
                                try
                                {
                                    vmCleanup.Cleanup();
                                }
                                catch (Exception exception)
                                {
                                    //Logger.LogError(exception.ToString());
                                }
                            }
                        }
                        frame.Source = _pageDictionaries[pageKey];

                        Parameter = param;

                        CurrentPageKey = pageKey;
                        if (CurrentPage == null)
                        {
                            CurrentPage = new PageInfo(string.Empty, pageKey, pageKey);
                        }
                        else
                        {
                            CurrentPage.PageKey = pageKey;
                        }

                        // 将当前页加入到页列表中
                        _navigations.Add(new PageInfo(CurrentPage.MenuKey,
                            CurrentPage.PageKey, CurrentPage.Title));

                        PageNavigated?.Invoke(this, new PageNavigationEventArgs(CurrentPage));
                    }
                }
            }
        }

        public void ShowPage(PageInfo navigateToPage, object param = null)
        {
            lock (_pageDictionaries)
            {
                if (!string.IsNullOrEmpty(navigateToPage.MenuKey))
                {
                    if (!_pageDictionaries.ContainsKey(navigateToPage.MenuKey))
                    {
                        throw new ArgumentException(string.Format("No such menu: {0} ", navigateToPage.MenuKey), "navigateToPage");
                    }

                    var frame = MenuFrame;
                    var uri = _pageDictionaries[navigateToPage.MenuKey];

                    // 如果两次的Uri相同，则不会更新
                    if (frame != null && frame.Source != uri)
                    {
                        var page = MenuFrame.Content as Page;
                        if (page != null)
                        {
                            var vmCleanup = page.DataContext as ICleanup;
                            if (vmCleanup != null)
                            {
                                try
                                {
                                    vmCleanup.Cleanup();
                                }
                                catch (Exception exception)
                                {
                                    //Logger.LogError(exception.ToString());
                                }
                            }
                        }
                        frame.Source = _pageDictionaries[navigateToPage.MenuKey];
                    }
                }
                else
                {
                    var frame = MenuFrame;
                    if (frame != null)
                    {
                        frame.Source = null;
                    }
                }

                if (!string.IsNullOrEmpty(navigateToPage.PageKey))
                {
                    if (!_pageDictionaries.ContainsKey(navigateToPage.PageKey))
                        throw new ArgumentException(string.Format("No such page: {0} ", navigateToPage.PageKey), "navigateToPage");

                    var frame = PageFrame;
                    var uri = _pageDictionaries[navigateToPage.PageKey];
                    if (frame != null && frame.Source != uri)
                    {
                        var page = PageFrame.Content as Page;
                        if (page != null)
                        {
                            var vmCleanup = page.DataContext as ICleanup;
                            if (vmCleanup != null)
                            {
                                try
                                {
                                    vmCleanup.Cleanup();
                                }
                                catch (Exception exception)
                                {
                                    //Logger.LogError(exception.ToString());
                                }
                            }
                        }
                        frame.Source = _pageDictionaries[navigateToPage.PageKey];

                        Parameter = param;

                        CurrentPageKey = navigateToPage.PageKey;

                        PageNavigated?.Invoke(this, new PageNavigationEventArgs(navigateToPage));

                        CurrentPage = navigateToPage;
                        _navigations.Add(new PageInfo(CurrentPage.MenuKey,
                            CurrentPage.PageKey, CurrentPage.Title));
                    }
                }
            }
        }



        /// <summary>
        /// 添加一个Page
        /// </summary>
        /// <param name="key">Page 名称，作为标志</param>
        /// <param name="pageType">Page的路径</param>
        public void AddPage(string key, Uri pageType)
        {
            lock (_pageDictionaries)
            {
                if (_pageDictionaries.ContainsKey(key))
                {
                    _pageDictionaries[key] = pageType;
                }
                else
                {
                    _pageDictionaries.Add(key, pageType);
                }
            }
        }

        /// <summary>
        /// 指向用于导航的Frame对象的弱引用
        /// </summary>
        private WeakReference<Frame> _menuFrame;

        private WeakReference<Frame> _pageFrame;

        /// <summary>
        /// 获取用于导航的Frame对象，可能为空
        /// </summary>
        private Frame MenuFrame
        {
            get
            {
                Frame frame;
                if (_menuFrame != null && _menuFrame.TryGetTarget(out frame))
                {
                    return frame;
                }
                return null;
            }
        }

        private Frame PageFrame
        {
            get
            {
                Frame frame;
                if (_pageFrame != null && _pageFrame.TryGetTarget(out frame))
                {
                    return frame;
                }
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="menuFrame">左侧垂直排布菜单的frame</param>
        /// <param name="pageFrame">右侧具体菜单页面的Frame</param>
        public void SetFrame(Frame menuFrame, Frame pageFrame)
        {
            _menuFrame = new WeakReference<Frame>(menuFrame);
            _pageFrame = new WeakReference<Frame>(pageFrame);
        }
    }
}
