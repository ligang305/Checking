using BG_Services;
using CMW.Common.Utilities;
using BGDAL;
using BGModel;

using BGUserControl;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shell;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.CommonFunc;
using BG_Entities;
using BG_WorkFlow;

namespace BGUserControl
{
    [Export("PageModule", typeof(IConditionView))]
    [CustomExportMetadata(1, "SystemSetting", "设备控制模块", "ZZW", "1.0.0")]
    public class SystemSetting : BaseModules
    {
        Grid _MainGrid = new Grid();
        DockPanel dp;
        SystemSettingMvvm systemSettingMvvm = new SystemSettingMvvm();

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
        public SystemSetting() : base(Common.controlVersion)
        {
            Loaded += SystemSetting_Loaded;
            Unloaded += SystemSetting_Unloaded;
         
            systemSettingMvvm.SwitchLanguageAction -= SwitchLanguage;
            systemSettingMvvm.SwitchLanguageAction += SwitchLanguage;
        }
        /// <summary>
        /// 页面卸载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SystemSetting_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            IsModuleOpen = false;
            Task.Run(() => {SetCommand(CommandDic[Command.AutoMode], true);});
            ReleaseResources();
        }
        /// <summary>
        /// 页面加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SystemSetting_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            DataContext = systemSettingMvvm;
            IsModuleOpen = true;
            Task.Run(() => {SetCommand(CommandDic[Command.AutoMode], false);});
            InitContent();
        }
        /// <summary>
        /// 生成内容
        /// </summary>
        private void InitContent()
        {
            _MainBorder = new Border()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(0),
                BorderThickness = new Thickness(2),
                BorderBrush = StrToBrush("#3F96E6"),
                CornerRadius = new CornerRadius(6),
                Background = StrToBrush("#F2F2F2")
            };
            _MainGrid = InitGrid();
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
                BorderBrush = (LinearGradientBrush)this.FindResource("LnearBorderColor"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 32,
                Background = (LinearGradientBrush)this.FindResource("TitleBackGround"),
            };

            Canvas cs = new Canvas()
            {
                Style = (Style)this.FindResource("diyCloseCanvas"),
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 1, 10, 0),
                Width = 14,
                Height = 14
            };
            cs.PreviewMouseLeftButtonDown += Bd_PreviewMouseLeftButtonDown;

            Panel.SetZIndex(bd, 99);
            bd.Child = cs;
            #region 内容框
            MakeContentTabControl(_MainGrid);
            #endregion
            dp.MouseDown += Dp_MouseDown;
            dp.Children.Add(bd);
            Grid.SetColumn(dp, 0);
            Grid.SetRow(dp, 0);
            _MainGrid.Children.Add(dp);
            #endregion

            return _MainGrid;
        }
        /// <summary>
        /// 初始化TabControl组件
        /// </summary>
        /// <param name="MainGrid"></param>
        private void MakeContentTabControl(Grid MainGrid)
        {
            TabControl tc = new TabControl() { Style = (Style)this.TryFindResource("DiyTabControl") };
            List<ModulesConfig> mItems = systemSettingMvvm.SettingModules();
            foreach (var ModulesItem in systemSettingMvvm.SettingModules())
            {
                TabItem ModulesItemSetting = new TabItem()
                {
                    Header = ModulesItem.modulesPluginsName,//UpdateStatusNameAction()
                    Style = (Style)this.TryFindResource("DiyTabItemStyle"),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                };
                var ModulesItemPlugin = ContentPlugins?.First(q => q.Metadata.Name == ModulesItem.modulesPluginsName)?.Value; // BGUserControl.VJ_BetratronBoostingSetting
                ModulesItemPlugin?.SetCarVersion(cv);
                ModulesItemPlugin.Width = GetWidth()-20;
                ModulesItemPlugin.Height = GetHeight()-20;
                ModulesItemSetting.Content = ModulesItemPlugin;
                string mName = ModulesItemPlugin.GetName();
                ModulesItemSetting.Header = UpdateStatusNameAction(ModulesItemPlugin.GetName()); // Header: [背散射线机及电机控制、通用设置、背散探测器设置]
                tc.Items.Add(ModulesItemSetting);
            }
            dp.Children.Add(tc);
        }
        /// <summary>
        /// 实现主UI
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// 顶部导航栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// 关闭按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bd_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 获取名称
        /// </summary>
        /// <returns></returns>
        public override string GetName()
        {
            return BoosttingSetting;
        }
        /// <summary>
        /// 获取软件高度
        /// </summary>
        /// <returns></returns>
        public override double GetHeight()
        {
            return 1050;
        }
        /// <summary>
        /// 获取软件宽度
        /// </summary>
        /// <returns></returns>
        public override double GetWidth()
        {
            return 1200;
        }
        /// <summary>
        /// 判断是否进行连接
        /// </summary>
        /// <returns></returns>
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
        public string GetAccelatorModule()
        {
            string AccelatorMode = BoostingControllerManager.GetInstance().GetCurrentEquipmentModel();
            switch (AccelatorMode)
            {
                case AccelatorType.BegoodBoosting:
                    return Modules.BegoodBoostSettingModule;
                case AccelatorType.FH_Betratron:
                    return Modules.FH_BetratronBoostingSetting;
                case AccelatorType.Russia_Betratron:
                    return "";
                default:
                    return "";
            }
        }

        public void SwitchLanguage()
        {
            InitContent();
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
            if (ContentPlugins != null)
            {
                foreach (var plugin in ContentPlugins)
                {
                    if (plugin?.Value != null)
                    {
                        // 如果插件有自己的释放方法可以在这里调用
                        plugin.Value.Unloaded -= Plugin_Unloaded;
                    }
                }
            }

            // 3. 清除数据上下文引用
            DataContext = null;
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
    }

    public class  SystemSettingMvvm: BaseMvvm
    {
        ModulesDal modulesDal = new ModulesDal();
        
        public SystemSettingMvvm()
        {
            DetecotrControllerManager.GetInstance().DetecotorConnectionAction -= DetecotorConnection;
            DetecotrControllerManager.GetInstance().DetecotorConnectionAction += DetecotorConnection;
        }
        public List<ModulesConfig> SettingModules()
        {
            return modulesDal.GetModulesConfig(controlVersion.ToString());
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
        public void DetecotorConnection(bool DetecotrConnection)
        {
        }
    }
}
