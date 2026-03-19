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
    [CustomExportMetadata(1, Modules.ParamConfigModule, "参数配置", "ZZW", "1.0.0")]
    public class ParamConfigModule : BaseModules
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
        ParamConfigMvvm paramConfigMvvm;
        public ParamConfigModule(): base(Common.controlVersion)
        {
            Loaded += ParamConfig_Loaded;
            Unloaded += ParamConfig_Unloaded;
        }

        #region 数据
        /// <summary>
        /// 从配置文件取出原始数据
        /// </summary>
        private void InitData()
        {
            paramConfigMvvm = new ParamConfigMvvm();
            this.DataContext = paramConfigMvvm;
        }
        #endregion

        #region 接口
        public override string GetName()
        {
            return UpdateStatusNameAction("ParamConfig");
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
        /// 窗口卸载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ParamConfig_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }
        /// <summary>
        /// 窗口加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ParamConfig_Loaded(object sender, System.Windows.RoutedEventArgs e)
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
            ParamConfig_Loaded(null, null);
        }
        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="language"></param>
        public void SwitchLanguage(string language)
        {
            base.Base_SwitchLanguage(language);
            ParamConfig_Loaded(null, null);
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
            Param.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10, GridUnitType.Star) });
            sv.Content = Param;
            foreach (var paramConfigItems in paramConfigMvvm.ParamConfigs)
            {
                int ItemIndex = paramConfigMvvm.ParamConfigs.IndexOf(paramConfigItems);
                Border bd = MakeSmallPoint();
                Label lblName = MakeLabel(paramConfigItems.Key);
                TextBox tbName = MakeText(paramConfigItems);
                Label lblValue = MakeLabel(paramConfigItems.Value);
                Border btnModify = MakeSetButton("Modify");
                Border btnSure = MakeSetButton("Sure");
                Border btnCancel = MakeSetButton("Cancel");
                Param.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50,GridUnitType.Pixel)});
                Param.Children.Add(bd); Param.Children.Add(lblName); Param.Children.Add(tbName);
                Param.Children.Add(lblValue); Param.Children.Add(btnModify); Param.Children.Add(btnSure);
                Param.Children.Add(btnCancel);
                Grid.SetRow(bd, ItemIndex); Grid.SetRow(lblName, ItemIndex); Grid.SetRow(tbName, ItemIndex);
                Grid.SetRow(lblValue, ItemIndex); Grid.SetRow(btnModify, ItemIndex); Grid.SetRow(btnSure, ItemIndex);
                Grid.SetRow(btnCancel, ItemIndex);
                Grid.SetColumn(bd, 0); Grid.SetColumn(lblName, 1); Grid.SetColumn(tbName, 2); Grid.SetColumn(lblValue,2);
                Grid.SetColumn(btnModify, 3); Grid.SetColumn(btnSure, 3); Grid.SetColumn(btnCancel, 3);
                btnModify.Width = 70;
                btnModify.InputBindings.Add(new MouseBinding(paramConfigMvvm.ParamConfigModifyCommand, new MouseGesture(MouseAction.LeftClick)) { CommandParameter = paramConfigItems });
                btnSure.InputBindings.Add(new MouseBinding(paramConfigMvvm.ParamConfigUpdateCommand, new MouseGesture(MouseAction.LeftClick)) { CommandParameter = paramConfigItems });
                btnCancel.InputBindings.Add(new MouseBinding(paramConfigMvvm.ParamConfigCancelCommand, new MouseGesture(MouseAction.LeftClick)) { CommandParameter = paramConfigItems });
                btnSure.HorizontalAlignment = HorizontalAlignment.Left;
                btnCancel.HorizontalAlignment = HorizontalAlignment.Right;
                btnModify.HorizontalAlignment = HorizontalAlignment.Center;
                lblName.HorizontalContentAlignment = lblValue.HorizontalContentAlignment = HorizontalAlignment.Left;
                lblValue.SetBinding(Label.VisibilityProperty, new Binding("ismodifing.IsShow") { Source = paramConfigItems, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,Mode = BindingMode.TwoWay });
                btnModify.SetBinding(Label.VisibilityProperty, new Binding("ismodifing.IsShow") { Source = paramConfigItems, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay });
                btnSure.SetBinding(Label.VisibilityProperty, new Binding("ismodifing.IsShow") { Source = paramConfigItems, Converter = new VisibleConvert(), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay });
                btnCancel.SetBinding(Label.VisibilityProperty, new Binding("ismodifing.IsShow") { Source = paramConfigItems, Converter = new VisibleConvert(), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay });
                lblValue.SetBinding(Label.ContentProperty, new Binding("Value") { Source = paramConfigItems, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay });
            }
            return sv;
        }
        /// <summary>
        /// 设置Border
        /// </summary>
        private Border MakeSetButton(string tag)
        {
            Border BtnSendCommand = new Border()
            {
                Width = 50,
                Height = 30,
                Style = (Style)this.FindResource("diyBtnCarHand"),
                Tag = tag
            };
            Label lblContent = MakeLabel(tag);
            lblContent.Foreground = Brushes.White;
            BtnSendCommand.Child = lblContent;
            return BtnSendCommand;
        }
        /// <summary>
        /// 制造小圆点
        /// </summary>
        private Border MakeSmallPoint()
        {
            Border ClycleBd = new Border()
            {
                Width = 10,
                Height = 10,
                BorderBrush = StrToBrush("#0000FF"),
                BorderThickness = new Thickness(1),
                Background = (LinearGradientBrush)this.FindResource("BluePoliceLight"),
                CornerRadius = new CornerRadius(5),
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 10, 0)
            };
            return ClycleBd;
        }
        /// <summary>
        /// 生成Text
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        protected TextBox MakeText(ParamConfig paramConfigItems)
        {
            TextBox lblMax = new TextBox()
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Left,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Foreground = StrToBrush("#1A4F85"),
                Width = 180,Height = 30
            };
            lblMax.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            lblMax.SetBinding(TextBox.TextProperty,new Binding("Value") {Source = paramConfigItems, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            lblMax.SetBinding(TextBox.VisibilityProperty,new Binding("ismodifing.IsShow") {Source = paramConfigItems, Converter = new VisibleConvert(),Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, });
            return lblMax;
        }
        #endregion
    }
}
