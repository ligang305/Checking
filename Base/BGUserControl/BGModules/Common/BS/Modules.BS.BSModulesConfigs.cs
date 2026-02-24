using CMW.Common.Utilities;
using BGCommunication;
using BGDAL;
using BGModel;

using BGUserControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.CommonFunc;
using BG_Entities;

namespace BGUserControl
{
    [Export("ContentPage", typeof(BaseModules))]
    [CustomExportMetadata(1, Modules.BSModulesConfigModule, "背散模块配置", "ZZW", "1.0.0")]
    public class BSModulesConfigs : BSBaseModules
    {
        Grid _MainGrid = new Grid();
        Border _MainBorder = new Border()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Margin = new Thickness(0),
            BorderThickness = new Thickness(2),
            Background = StrToBrush("#FFFFFF"),
        };
        BSModulesConfigMvvm modulesConfigMvvm;
        public BSModulesConfigs(): base(Common.controlVersion)
        {
            Loaded += BSModulesConfigs_Loaded;
        }

        #region 数据
        /// <summary>
        /// 从配置文件取出原始数据
        /// </summary>
        private void InitData()
        {
            modulesConfigMvvm = new BSModulesConfigMvvm(ControlVersion.BS);
            this.DataContext = modulesConfigMvvm;
        }
        #endregion

        #region 接口
        public override string GetName()
        {
            return UpdateStatusNameAction("ModulesConfigModule");
        }

        public override bool IsConnectionEquipment()
        {
            return Common.IsConnection;
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
            return 680;
        }

        public override double GetWidth()
        {
            return 760;
        }

        #endregion

        #region UI
        /// <summary>
        /// 初始化主Grid
        /// </summary>
        /// <returns></returns>
        private Grid InitGrid()
        {
            Grid _MainGrid = InitMainGird();
            ScrollViewer sv = ContentPanel();
            Grid.SetColumn(sv, 0);
            Grid.SetRow(sv, 1);
            Grid.SetColumnSpan(sv, 3);
            _MainGrid.Children.Add(sv);
            return _MainGrid;
        }
        /// <summary>
        /// 初始化主Grid
        /// </summary>
        /// <returns></returns>
        private static Grid InitMainGird()
        {
            Grid _MainGrid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Pixel) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30, GridUnitType.Star) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10, GridUnitType.Pixel) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30, GridUnitType.Star) });
            return _MainGrid;
        }
        /// <summary>
        /// 窗口加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BSModulesConfigs_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            InitContent();
            InitAction();
        }
        /// <summary>
        /// 初始化内容Content
        /// </summary>
        private void InitContent()
        {
            InitData();
            _MainGrid = InitGrid();
            _MainBorder.Child = _MainGrid;
            Content = _MainBorder;
        }
        /// <summary>
        /// 加载事件
        /// </summary>
        private void InitAction()
        {
            ButtonInvoke.SwitchLanguageEvent -= SwitchLanguage;
            ButtonInvoke.SwitchLanguageEvent += SwitchLanguage;
            ButtonInvoke.SwitchFontSizeEvent -= SwitchFontSize;
            ButtonInvoke.SwitchFontSizeEvent += SwitchFontSize;
        }
        /// <summary>
        /// 切换字体大小
        /// </summary>
        /// <param name="FontSize"></param>
        public void SwitchFontSize(string FontSize)
        {
            BSModulesConfigs_Loaded(null, null);
        }
        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="language"></param>
        public void SwitchLanguage(string language)
        {
            base.Base_SwitchLanguage(language);
            BSModulesConfigs_Loaded(null, null);
        }
        public ScrollViewer ContentPanel()
        {
            ScrollViewer sv = new ScrollViewer()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            Grid Param = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            Param.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70, GridUnitType.Pixel) });
            Param.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30, GridUnitType.Star) });
            Param.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(180, GridUnitType.Pixel) });
            Param.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(80, GridUnitType.Pixel) });
            Param.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10, GridUnitType.Star) });
            sv.Content = Param;
            foreach (var ModulesItems in modulesConfigMvvm.ModulesConfigs)
            {
                int ItemIndex = modulesConfigMvvm.ModulesConfigs.IndexOf(ModulesItems);
                CheckBox bd = MakeCbk();
                Label lblName = MakeLabel(ModulesItems.modulesContentPluginsName);
                lblName.Content = ModulesItems.modulesContentPluginsName;
                TextBlock lblPluginsName = MakeTextBlock(ModulesItems.modulesPluginsName);
                Label lblVersion = MakeLabel(ModulesItems.Version);
                
                Param.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50,GridUnitType.Pixel)});
                Param.Children.Add(bd); Param.Children.Add(lblName); Param.Children.Add(lblPluginsName);
                Param.Children.Add(lblVersion);
                Grid.SetRow(bd, ItemIndex); Grid.SetRow(lblName, ItemIndex); Grid.SetRow(lblPluginsName, ItemIndex);
                Grid.SetRow(lblVersion, ItemIndex);
                Grid.SetColumn(bd, 0); Grid.SetColumn(lblName, 1); Grid.SetColumn(lblPluginsName, 2); Grid.SetColumn(lblVersion, 3);
                
                lblName.HorizontalContentAlignment = lblVersion.HorizontalContentAlignment = HorizontalAlignment.Left;
                lblVersion.SetBinding(Label.ContentProperty, new Binding("Version") { Source = ModulesItems, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay });
                bd.SetBinding(CheckBox.IsCheckedProperty, new Binding("IsCheck") { Source = ModulesItems, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay });
                bd.InputBindings.Add(new MouseBinding(modulesConfigMvvm.ModulePluginCheckCommand, new MouseGesture(MouseAction.LeftClick)) { CommandParameter = ModulesItems });
            }
            return sv;
        }

        /// <summary>
        /// 制造CheckBox
        /// </summary>
        private CheckBox MakeCbk()
        {
            CheckBox cbkPlugins = new CheckBox()
            {
                Width = 30,
                Height = 30,
                BorderBrush = StrToBrush("#0000FF"),
                BorderThickness = new Thickness(1),
                Background = (LinearGradientBrush)this.FindResource("BluePoliceLight"),
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 10, 0)
            };
            return cbkPlugins;
        }
        #endregion
    }
}
