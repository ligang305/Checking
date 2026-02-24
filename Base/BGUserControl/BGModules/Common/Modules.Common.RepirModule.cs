using BG_Entities;
using BG_Services;
using BGCommunication;
using BGModel;
using BGUserControl;
using CMW.Common.Utilities;
using System;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using static CMW.Common.Utilities.ImageImportDll;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonFunc;
using static CMW.Common.Utilities.CommonDeleget;
using BG_WorkFlow;

namespace BGUserControl
{
    [Export("PageModule", typeof(IConditionView))]
    [CustomExportMetadata(1, Modules.RepirModule, "工程模式设置界面", "ZZW", "1.0.0")]
    public class RepirModule : BaseModules
    {
        Grid _MainGrid = new Grid();
        Border _MainBorder = new Border()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Margin = new Thickness(0),
            BorderThickness = new Thickness(2),
            BorderBrush = StrToBrush("#3F96E6"),
            CornerRadius = new CornerRadius(6, 6, 6, 6),
            Background = StrToBrush("#F2F2F2"),
        };
        ScrollViewer SettingTb = null;
        [ImportingConstructor]
        public RepirModule() : base(Common.controlVersion)
        {
            Loaded += RepirModule_Loaded;
            Unloaded += RepirModule_Unloaded;
            IsVisibleChanged += RepirModule_IsVisibleChanged;
        }

        private void RepirModule_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if((bool?)e.NewValue == true)
            {
                Task.Run(() => {if(CommandDic.ContainsKey(Command.Repair)) SetCommand(CommandDic[Command.Repair], true); });
            }
            else
            {
                Task.Run(() => { if (CommandDic.ContainsKey(Command.Repair)) SetCommand(CommandDic[Command.Repair], false); });
            }
        }

        public override string GetName()
        {
            return UpdateStatusNameAction("RepirModule");
        }

        public override bool IsConnectionEquipment()
        {
            return IsConnection;
        }

        public override void Show(Window _OwnerWin)
        {
            CurrentWindow = _OwnerWin;
            _OwnerWin.MaxWidth = GetWidth();
            _OwnerWin.MaxHeight = GetHeight();
            _OwnerWin.Title = GetName();
            _OwnerWin.Content = this;
            _OwnerWin.Show();
        }


        public override double GetHeight()
        {
            //return 820;
            return 1180;
        }

        public override double GetWidth()
        {
            return 1180;
        }

        private Grid InitGrid()
        {
            Grid _MainGrid = MakeMainGrid();
            #region 设置标题
            DockPanel dp = InitTitle(_MainGrid);
            #endregion

            #region 加载TabControl
            LoadParamConfig(SettingTb, dp);
            #endregion
            dp.MouseDown += Dp_MouseDown;
            return _MainGrid;
        }
        /// <summary>
        /// 初始化主面板
        /// </summary>
        /// <returns></returns>
        private static Grid MakeMainGrid()
        {
            Grid _MainGrid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Pixel) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(25, GridUnitType.Star) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(40, GridUnitType.Pixel) });
            return _MainGrid;
        }

        /// <summary>
        /// 设置软件标题
        /// </summary>
        /// <param name="_MainGrid"></param>
        /// <returns></returns>
        private DockPanel InitTitle(Grid _MainGrid)
        {
            DockPanel dp = new DockPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                LastChildFill = true
            };
            Border bd = new Border()
            {
                Margin = new Thickness(0, 0, 0, 0),
                BorderThickness = new Thickness(0, 0, 0, 0),
                BorderBrush = (LinearGradientBrush)this.FindResource("LnearBorderColor"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 32, Height = 32,
                Background = (LinearGradientBrush)this.FindResource("TitleBackGround"),
            };
            bd.PreviewMouseLeftButtonDown += Bd_PreviewMouseLeftButtonDown;
            Canvas cs = new Canvas()
            {
                Style = (Style)this.FindResource("diyCloseCanvas"),
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 1, 10, 0),
                Width = 14,
                Height = 14
            };
            bd.Child = cs;
            Panel.SetZIndex(bd,99);
            Grid.SetColumn(dp, 0);
            Grid.SetColumnSpan(dp, 2);
            Grid.SetRowSpan(dp, 2);
            Grid.SetRow(dp, 0);
            Grid.SetColumn(bd,1);
            Grid.SetRow(bd,0);
            _MainGrid.Children.Add(dp);
            _MainGrid.Children.Add(bd);
            return dp;
        }

        private void Dp_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CurrentWindow.DragMove();
        }

        private void Bd_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Close();
        }

        private void RepirModule_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            IsModuleOpen = false;
            BuryingPoint($"{UpdateStatusNameAction("GeneralModuleLeave")}");
          
        }

        private void RepirModule_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ButtonInvoke.SwitchLanguageEvent -= SwitchLanguage;
            ButtonInvoke.SwitchFontSizeEvent -= SwitchFontSize;
            BuryingPoint($"{UpdateStatusNameAction("GeneralModuleEnter")}");
            _MainGrid = InitGrid();
            Panel.SetZIndex(_MainBorder,999);
            _MainBorder.Child = _MainGrid;
            
            Content = _MainBorder;
            ButtonInvoke.SwitchLanguageEvent += SwitchLanguage;
            ButtonInvoke.SwitchFontSizeEvent += SwitchFontSize;
            IsModuleOpen = true;
        }

        public void SwitchFontSize(string FontSize)
        {
            RepirModule_Loaded(null, null);
        }
        public void SwitchLanguage(string language)
        {
            base.Base_SwitchLanguage(language);
            RepirModule_Loaded(null, null);
        }

        #region LoadParamConfig
        /// <summary>
        /// 加载参数设置模块
        /// </summary>
        /// <param name="ContentGrid"></param>
        private void LoadParamConfig(FrameworkElement Content, DockPanel dp)
        {
            TabControl tc = new TabControl() { Style = (Style)this.TryFindResource("DiyTabControl") };
            TabItem ParamConfig = new TabItem()
            {
                Header = UpdateStatusNameAction("ParamConfig"),
                Style = (Style)this.TryFindResource("DiyTabItemStyle"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
            };
            TabItem PluginSetting = new TabItem()
            {
                Header = UpdateStatusNameAction("PluginSetting"),
                Style = (Style)this.TryFindResource("DiyTabItemStyle"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
            };
            TabItem BSPluginSetting = new TabItem()
            {
                Header = UpdateStatusNameAction("BSPluginSetting"),
                Style = (Style)this.TryFindResource("DiyTabItemStyle"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
            };
            TabItem RepairSetting = new TabItem()
            {
                Header = UpdateStatusNameAction("RepairSetting"),
                Style = (Style)this.TryFindResource("DiyTabItemStyle"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
            };
            TabItem PositionsSetting = new TabItem()
            {
                Header = UpdateStatusNameAction("StandardStatusPanel"),
                Style = (Style)this.TryFindResource("DiyTabItemStyle"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
            };
            //参数设置
            var ParamConfigModel = ContentPlugins?.First(q => q.Metadata.Name == Modules.ParamConfigModule)?.Value;
            ParamConfigModel?.SetCarVersion(cv);
            ParamConfig.Content = ParamConfigModel;

            //插件设置
            var ModulesPluginModel = ContentPlugins?.First(q => q.Metadata.Name == Modules.PluginsModule)?.Value;
            ModulesPluginModel?.SetCarVersion(cv);
            PluginSetting.Content = ModulesPluginModel;


            //背散插件设置
            var BSModulesPluginModel = ContentPlugins?.First(q => q.Metadata.Name == Modules.BSModulesConfigModule)?.Value;
            BSModulesPluginModel?.SetCarVersion(ControlVersion.BS);
            BSPluginSetting.Content = BSModulesPluginModel;

            //工程参数设置
            var ModulesReparModel = ContentPlugins?.First(q => q.Metadata.Name == Modules.RepairSetting)?.Value;
            ModulesReparModel?.SetCarVersion(cv);
            RepairSetting.Content = ModulesReparModel;

            //工程参数设置
            var ModulesIOPositionModel = ContentPlugins?.First(q => q.Metadata.Name == Modules.StandardStatusPanel)?.Value;
            ModulesIOPositionModel?.SetCarVersion(cv);
            PositionsSetting.Content = ModulesIOPositionModel;

            DockPanel.SetDock(tc,Dock.Left);
            tc.Items.Add(ParamConfig);
            tc.Items.Add(PluginSetting);
            tc.Items.Add(BSPluginSetting);
            tc.Items.Add(RepairSetting);
            tc.Items.Add(PositionsSetting);
            dp.Children.Add(tc);
        }
        #endregion

    }
}
