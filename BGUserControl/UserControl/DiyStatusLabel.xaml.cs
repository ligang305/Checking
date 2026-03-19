using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BGUserControl
{
    /// <summary>
    /// DiyStatusLabel.xaml 的交互逻辑
    /// </summary>
    public partial class DiyStatusLabel : UserControl
    {
        public DiyStatusLabel()
        {
            InitializeComponent();
            lblName.MaxWidth = 250;
            LambLight.Background = (LinearGradientBrush)TryFindResource("RedPoliceLight");
        }


        /// <summary>
        /// 表示前面的颜色
        /// </summary>
        public double LblTextMaxWidth
        {
            get
            {
                return (double)GetValue(lblTextMaxWidthProperty);
            }
            set { SetValue(lblTextMaxWidthProperty, value); }
        }

        public static readonly DependencyProperty lblTextMaxWidthProperty =
            DependencyProperty.Register("LblTextMaxWidth", typeof(double), typeof(DiyStatusLabel),
                 new PropertyMetadata(70.0, new PropertyChangedCallback(OnLabelWidthValueChange)));
        /// <summary>
        /// 设置文本框长度
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnLabelWidthValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiyStatusLabel statusModel = d as DiyStatusLabel;
            if (statusModel != null && statusModel.LblTextMaxWidth != null)
            {
                statusModel.lblName.MaxWidth = statusModel.LblTextMaxWidth;
            }
        }

        /// <summary>
        /// 表示前面的颜色
        /// </summary>
        public string LabmForecolor
        {
            get
            {
                return (string)GetValue(LabmForecolorProperty);
            }
            set { SetValue(LabmForecolorProperty, value); }

        }

        public static readonly DependencyProperty LabmForecolorProperty =
            DependencyProperty.Register("LabmForecolor", typeof(string), typeof(DiyStatusLabel),
                 new PropertyMetadata("RedPoliceLight", new PropertyChangedCallback(OnLambForeColorValueChange)));


        /// <summary>
        /// 表示前面的颜色
        /// </summary>
        public Brush LblTextForground
        {
            get
            {
                return (Brush)GetValue(LblTextForgroundProperty);
            }
            set { SetValue(LblTextForgroundProperty, value); }

        }

        public static readonly DependencyProperty LblTextForgroundProperty =
            DependencyProperty.Register("LblTextForground", typeof(Brush), typeof(DiyStatusLabel),
                 new PropertyMetadata(Brushes.White, new PropertyChangedCallback(OnLabelTextValueChange)));


        /// <summary>
        /// 表示标签代表什么意思
        /// </summary>
        public string LabelText
        {
            get
            {
                return (string)GetValue(LabelTextProperty);
            }
            set { SetValue(LabelTextProperty, value); }

        }

        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(DiyStatusLabel),
                 new PropertyMetadata("", new PropertyChangedCallback(OnLblNameValueChange)));

        /// <summary>
        /// 前景色颜色改变
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnLambForeColorValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiyStatusLabel statusModel = d as DiyStatusLabel;
            if (statusModel != null && statusModel.LabmForecolor != null)
            {
                statusModel.LambLight.Background = (LinearGradientBrush)statusModel.FindResource(statusModel.LabmForecolor);
            }
        }
        /// <summary>
        /// 文本前景色
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnLabelTextValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiyStatusLabel statusModel = d as DiyStatusLabel;
            if (statusModel != null && statusModel.LblTextForground != null)
            {
                statusModel.lblName.Foreground = statusModel.LblTextForground;
            }
        }


        /// <summary>
        /// 标签颜色
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnLblNameValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiyStatusLabel statusModel = d as DiyStatusLabel;
            if (statusModel != null && statusModel.LabmForecolor != null)
            {
                statusModel.lblName.Text = statusModel.LabelText;
                statusModel.lblName.ToolTip = statusModel.LabelText;
            }
        }

    }
}
