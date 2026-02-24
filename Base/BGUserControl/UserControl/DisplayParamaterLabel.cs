using CMW.Common.Utilities;
using BGModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using static CMW.Common.Utilities.CommonFunc;

namespace BGUserControl
{
    public class DisplayParamaterLabel : BaseWindows
    {
        public DisplayParamaterLabel()
        {

        }
        /// <summary>
        /// 属性，用来接收外部传输进来的数据对象
        /// </summary>
        public double DisplayParamaterLabelWidth
        {
            get
            {
                return (double)GetValue(DisplayParamaterLabelWidthProperty);
            }
            set { SetValue(DisplayParamaterLabelWidthProperty, value); }
        }

        /// <summary>
        /// 数据源依赖属性
        /// </summary>
        public static readonly DependencyProperty DisplayParamaterLabelWidthProperty =
            DependencyProperty.Register("DisplayParamaterLabelWidth", typeof(double), typeof(DisplayParamaterLabel),
                 new PropertyMetadata(250.0, new PropertyChangedCallback(OnWidthValueChange)));

        /// <summary>
        /// 属性，用来接收外部传输进来的数据对象
        /// </summary>
        public double DisplayParamaterLabelHeight
        {
            get
            {
                return (double)GetValue(DisplayParamaterLabelHeightProperty);
            }
            set { SetValue(DisplayParamaterLabelHeightProperty, value); }
        }

        /// <summary>
        /// 数据源依赖属性
        /// </summary>
        public static readonly DependencyProperty DisplayParamaterLabelHeightProperty =
            DependencyProperty.Register("DisplayParamaterLabelHeight", typeof(double), typeof(DisplayParamaterLabel),
                 new PropertyMetadata(38.0, new PropertyChangedCallback(OnHeightValueChange)));


        Border MainBorder = new Border() 
        { 
            MaxWidth = 250,
            MaxHeight = 38,
            CornerRadius = new CornerRadius(4), 
            VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch, 
            BorderThickness = new Thickness(0),
            Margin = new Thickness(0, 5, 0, 0)
        };
        Grid ContentGrid = new Grid() { VerticalAlignment = System.Windows.VerticalAlignment.Stretch, HorizontalAlignment = HorizontalAlignment.Stretch };

        public DisplayParamaterLabel(ParamaterLabel _HitchModellList) : base()
        {
            Loaded += DisplayParamaterLabel_Loaded;
            ContentGrid = MakeGridPanel(_HitchModellList);
            MainBorder.Child = ContentGrid;
            Content = MainBorder;
        }

        private void DisplayParamaterLabel_Loaded(object sender, RoutedEventArgs e)
        {
            MainBorder.Background = (SolidColorBrush)this.FindResource("MainPageParamaterBG");
        }

        /// <summary>
        /// 创建Grid
        /// </summary>
        /// <returns></returns>
        public Grid MakeGridPanel(ParamaterLabel _ParamaterLabel)
        {
            Grid _ContentGrid = InitContentGrid();

            InitLabel(_ParamaterLabel, _ContentGrid);

            InitTitle(_ParamaterLabel, _ContentGrid);
            return _ContentGrid;
        }

        private void InitTitle(ParamaterLabel _ParamaterLabel, Grid _ContentGrid)
        {
            Label lblContent = new Label()
            {
                Style = (Style)this.FindResource("PopupLabel"),
                Background = Brushes.White,
                Width = 100,
                Height = 26
            };
            lblContent.SetBinding(Label.ContentProperty, new Binding("ParamaterValue")
            { Source = _ParamaterLabel, Mode = BindingMode.TwoWay });
            lblContent.SetBinding(Label.ForegroundProperty, new Binding("ParamaterForeColor")
            { Source = _ParamaterLabel, Mode = BindingMode.TwoWay });
            lblContent.SetBinding(Label.FontSizeProperty,new Binding("ParamaterFontSize") { Source = _ParamaterLabel, Mode = BindingMode.TwoWay });
            Grid.SetColumn(lblContent, 1);
            Grid.SetRow(lblContent, 0);
            _ContentGrid.Children.Add(lblContent);
        }

        private void InitLabel(ParamaterLabel _ParamaterLabel, Grid _ContentGrid)
        {
            TextBlock lbl = new TextBlock()
            {
                Style = (Style)this.FindResource("PopupTextBlock"),
                Background = StrToBrush("#00FFFFFF"),
                Foreground = Brushes.White,TextTrimming = TextTrimming.CharacterEllipsis,
                HorizontalAlignment = HorizontalAlignment.Center,VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(lbl, 0);
            Grid.SetRow(lbl, 0);
            _ContentGrid.Children.Add(lbl);
            lbl.SetBinding(TextBlock.TextProperty, new Binding("ParamaterName") 
            { Source = _ParamaterLabel, Mode = BindingMode.TwoWay });
            lbl.SetBinding(TextBlock.ToolTipProperty, new Binding("ParamaterName")
                { Source = _ParamaterLabel, Mode = BindingMode.TwoWay });
            lbl.SetBinding(TextBlock.FontSizeProperty, new Binding("ParamaterNameFontSize")
            { Source = _ParamaterLabel, Mode = BindingMode.TwoWay });
        }

        private static Grid InitContentGrid()
        {
            Grid _ContentGrid = new Grid()
            {
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            _ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            _ContentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            _ContentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(120, GridUnitType.Pixel) });
            return _ContentGrid;
        }

        /// <summary>
        /// 高度
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnHeightValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DisplayParamaterLabel DisplayParamaterLabel = d as DisplayParamaterLabel;
            if (DisplayParamaterLabel != null && DisplayParamaterLabel.DisplayParamaterLabelHeight != null)
            {
                DisplayParamaterLabel.MainBorder.Height = DisplayParamaterLabel.DisplayParamaterLabelHeight;
            }
        }

        /// <summary>
        /// 宽度
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnWidthValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DisplayParamaterLabel DisplayParamaterLabel = d as DisplayParamaterLabel;
            if (DisplayParamaterLabel != null && DisplayParamaterLabel.DisplayParamaterLabelWidth != null)
            {
                DisplayParamaterLabel.MainBorder.Width = DisplayParamaterLabel.DisplayParamaterLabelWidth;
            }
        }
    }
}
