using BG_Entities;
using BG_Services;
using BG_WorkFlow;
using BGDAL;
using BGModel;
using BGUserControl;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shell;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.CommonFunc;

namespace BGUserControl
{
    [Export("PageModule", typeof(IConditionView))]
    [CustomExportMetadata(1, "BSSystemSetting", "背散控制页面", "ZZW", "1.0.0")]
    public class BSSystemSetting : BSBaseModules
    {
        Grid _MainGrid = new Grid();
        DockPanel dp;
        BSSystemSettingMvvm BS_SystemSettingMvvm = new BSSystemSettingMvvm();

        [ImportMany("BS_ContentPage", typeof(BaseModules), Source = ImportSource.Local, RequiredCreationPolicy = CreationPolicy.NonShared)]
        public Lazy<BaseModules, IMetaData>[] BSContentPlugins { get; set; }

        Border _MainBorder = new Border()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Margin = new Thickness(0),
            BorderThickness = new Thickness(2),
            BorderBrush = StrToBrush("#3F96E6"),
            CornerRadius = new CornerRadius(6),
            Background = StrToBrush("#F2F2F2")
        };

        // 缓存资源引用（同步缓存）
        private Style _diyTabControlStyle;
        private Style _diyTabItemStyle;
        private LinearGradientBrush _linearBorderColor;
        private LinearGradientBrush _titleBackGround;
        private Style _diyCloseCanvasStyle;

        public BSSystemSetting() : base(ControlVersion.BS)
        {
            Loaded += BSSystemSetting_Loaded;
            Unloaded += BSSystemSetting_Unloaded;
            DataContext = BS_SystemSettingMvvm;
            BS_SystemSettingMvvm.SwitchLanguageAction -= SwitchLanguage;
            BS_SystemSettingMvvm.SwitchLanguageAction += SwitchLanguage;

            // 提前缓存资源（在构造函数中同步执行，避免重复查找）
            CacheResources();
        }

        private void BSSystemSetting_Unloaded(object sender, RoutedEventArgs e)
        {
            IsModuleOpen = false;
            ReleaseResources();
        }

        private void BSSystemSetting_Loaded(object sender, RoutedEventArgs e)
        {
            IsModuleOpen = true;
            InitContent(); // 保持同步初始化
        }

        /// <summary>
        /// 缓存资源到本地变量（同步方法）
        /// </summary>
        private void CacheResources()
        {
            _diyTabControlStyle = (Style)this.TryFindResource("DiyTabControl");
            _diyTabItemStyle = (Style)this.TryFindResource("DiyTabItemStyle");
            _linearBorderColor = (LinearGradientBrush)this.FindResource("LnearBorderColor");
            _titleBackGround = (LinearGradientBrush)this.FindResource("TitleBackGround");
            _diyCloseCanvasStyle = (Style)this.FindResource("diyCloseCanvas");
        }

        private void InitContent()
        {
            _MainGrid = InitGrid(); // 同步初始化Grid
            _MainBorder.Child = _MainGrid;
            Content = _MainBorder;
        }

        private Grid InitGrid()
        {
            Grid _MainGrid = MakeMainGrid();

            #region 顶部栏
            dp = new DockPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                LastChildFill = true,
            };
            Border bd = new Border()
            {
                BorderThickness = new Thickness(0, 0, 0, 0),
                BorderBrush = _linearBorderColor, // 使用缓存的资源
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 32,
                Background = _titleBackGround, // 使用缓存的资源
            };

            Canvas cs = new Canvas()
            {
                Style = _diyCloseCanvasStyle, // 使用缓存的资源
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 1, 10, 0),
                Width = 14,
                Height = 14
            };
            cs.PreviewMouseLeftButtonDown += Bd_PreviewMouseLeftButtonDown;

            Panel.SetZIndex(bd, 99);
            bd.Child = cs;
            DockPanel.SetDock(bd, Dock.Right);
            dp.MouseDown += Dp_MouseDown;
            dp.Children.Add(bd);
            Grid.SetColumn(dp, 0);
            Grid.SetRow(dp, 0);
            _MainGrid.Children.Add(dp);
            #endregion

            #region 内容框
            MakeContentTabControl(_MainGrid); // 同步初始化TabControl
            #endregion

            return _MainGrid;
        }

        /// <summary>
        /// 初始化TabControl（同步方法，带延迟加载优化）
        /// </summary>
        private void MakeContentTabControl(Grid mainGrid)
        {
            TabControl tc = new TabControl() { Style = _diyTabControlStyle }; // 使用缓存的资源
            var modules = BS_SystemSettingMvvm.SettingModules(); // 同步获取模块列表

            // 存储TabItem与插件名称的映射
            Dictionary<TabItem, string> tabPluginMap = new Dictionary<TabItem, string>();

            foreach (var moduleItem in modules)
            {
                TabItem tabItem = new TabItem()
                {
                    Header = moduleItem.modulesPluginsName,
                    Style = _diyTabItemStyle, // 使用缓存的资源
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                };

                // 暂存插件名称，不立即加载内容
                tabPluginMap[tabItem] = moduleItem.modulesPluginsName;
                tc.Items.Add(tabItem);
            }

            // 监听TabControl的选择变化事件（实现延迟加载）
            tc.SelectionChanged += async (s, e) =>
            {
                if (tc.SelectedItem is TabItem selectedTab)
                {
                    // 已加载过则跳过
                    if (selectedTab.Content != null && !(selectedTab.Content is TextBlock))
                        return;

                    // 显示加载提示
                    selectedTab.Content = new TextBlock { Text = "加载中..." };

                    // 异步加载插件（避免阻塞UI）
                    string pluginName = tabPluginMap[selectedTab];
                    var plugin = await Task.Run(() =>
                    {
                        var foundPlugin = BSContentPlugins?.FirstOrDefault(q => q.Metadata.Name == pluginName)?.Value;
                        if (foundPlugin != null)
                        {
                            foundPlugin.SetCarVersion(ControlVersion.BS);
                            foundPlugin.Width = GetWidth() - 20;
                            foundPlugin.Height = GetHeight() - 20;
                        }
                        return foundPlugin;
                    });

                    // 更新Tab内容
                    if (plugin != null)
                    {
                        selectedTab.Content = plugin;
                        selectedTab.Header = UpdateStatusNameAction(plugin.GetName());
                    }
                    else
                    {
                        selectedTab.Content = new TextBlock { Text = "加载失败" };
                    }
                }
            };

            DockPanel.SetDock(tc, Dock.Left);
            dp.Children.Add(tc);
        }

        private Grid MakeMainGrid()
        {
            Grid _MainGrid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(25, GridUnitType.Star) });
            return _MainGrid;
        }

        private void Dp_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                this.CurrentWindow?.DragMove();
            }
            catch (Exception ex)
            {
                CommonDeleget.WriteLogAction(ex.Message, LogType.ApplicationError, true);
            }
        }

        private void Bd_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Close();
        }

        public override string GetName()
        {
            return BoosttingSetting;
        }

        public override double GetHeight()
        {
            return 1100;
        }

        public override double GetWidth()
        {
            return 1200;
        }

        public override bool IsConnectionEquipment()
        {
            return Common.IsConnection;
        }

        public override void Show(Window _OwnerWin)
        {
            CurrentWindow = _OwnerWin;
            WindowChrome wc = WindowChrome.GetWindowChrome(CurrentWindow);
            wc = (WindowChrome)this.FindResource("WindowChromeKey");
            _OwnerWin.Width = GetWidth();
            _OwnerWin.Height = GetHeight();
            _OwnerWin.Title = GetName();
            _OwnerWin.Content = this;
            _OwnerWin.Show();
        }

        public void SwitchLanguage()
        {
            InitContent(); // 语言切换时重新初始化
        }


        // 新增资源释放方法
        private void ReleaseResources()
        {
            // 1. 清除主容器内容
            if (_MainGrid != null)
            {
                _MainGrid.Children.Clear();
                _MainGrid = null;
            }

            if (dp != null)
            {
                dp.Children.Clear();
                dp = null;
            }

            if (_MainBorder != null)
            {
                _MainBorder.Child = null;
                _MainBorder = null;
            }

            // 2. 释放插件资源
            if (BSContentPlugins != null)
            {
                foreach (var plugin in BSContentPlugins)
                {
                    if (plugin?.Value != null)
                    {
                        // 如果插件有自己的释放方法可以在这里调用
                        plugin.Value.Unloaded -= Plugin_Unloaded;
                    }
                }
                BSContentPlugins = null;
            }

            // 3. 清除数据上下文引用
            DataContext = null;

            // 4. 释放缓存的资源
            _diyTabControlStyle = null;
            _diyTabItemStyle = null;
            _linearBorderColor = null;
            _titleBackGround = null;
            _diyCloseCanvasStyle = null;

            // 5. 移除事件订阅
            Loaded -= BSSystemSetting_Loaded;
            Unloaded -= BSSystemSetting_Unloaded;
            BS_SystemSettingMvvm.SwitchLanguageAction -= SwitchLanguage;
        }

        // 新增插件卸载事件处理（如果需要）
        private void Plugin_Unloaded(object sender, RoutedEventArgs e)
        {
            var plugin = sender as BaseModules;
            if (plugin != null)
            {
                plugin.Unloaded -= Plugin_Unloaded;
                // 插件特定的清理逻辑
            }
        }

        public override void Close()
        {
            base.Close();
            ReleaseResources();
        }

    }

    public class BSSystemSettingMvvm : BaseMvvm
    {
        ModulesDal modulesDal = new ModulesDal();

        public BSSystemSettingMvvm()
        {
        }

        public List<ModulesConfig> SettingModules()
        {
            return modulesDal.GetModulesConfig(ControlVersion.BS.ToString());
        }

        public override void LoadUIText()
        {
            SwitchLanguageAction?.Invoke();
        }

        protected override void InquirePlcStatus(List<bool> StatusList)
        {

        }

        protected override void ConnectionStatus(bool ConnectionStatus)
        {

        }
    }
}