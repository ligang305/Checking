using BG_Entities;
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
using static CMW.Common.Utilities.CommonDeleget;

namespace BGUserControl
{
    /// <summary>
    /// DiyDetectorPanel.xaml 的交互逻辑
    /// </summary>
    public partial class DiyDetectorPanel : BaseWindows
    {

        public DiyDetectorPanel()
        {
            InitializeComponent();
            ButtonInvoke.SwitchLanguageEvent -= SwitchLanguage;
            ButtonInvoke.SwitchLanguageEvent += SwitchLanguage;
            ButtonInvoke.SwitchFontSizeEvent -= SwitchFontSize;
            ButtonInvoke.SwitchFontSizeEvent += SwitchFontSize;
            lblBoardIndex.Content = UpdateStatusNameAction("DetectorBoardIndex");
            lblBoardLineIndex.Content = UpdateStatusNameAction("DetectorBoardLineIndex");
        }
        
         public void SwitchFontSize(string language)
        {
            lblBoardIndex.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            lblBoardLineIndex.Content = UpdateFontSizeAction(CMWFontSize.Normal);
        }

        public void SwitchLanguage(string language)
        {
            base.Base_SwitchLanguage(language);
            lblBoardIndex.Content = UpdateStatusNameAction("DetectorBoardIndex");
            lblBoardLineIndex.Content = UpdateStatusNameAction("DetectorBoardLineIndex");
        }
        /// <summary>
        /// 表示探测器面板颜色
        /// </summary>
        public Brush StatusPreColor
        {
            get
            {
                return (Brush)GetValue(StatusPreColorProperty);
            }
            set { SetValue(StatusPreColorProperty, value); }
        }

        public static readonly DependencyProperty StatusPreColorProperty =
            DependencyProperty.Register("StatusPreColor", typeof(Brush), typeof(DiyDetectorPanel),
                 new PropertyMetadata(Brushes.Green, new PropertyChangedCallback(OnStatusPreColorChange)));
        /// <summary>
        /// 设置状态颜色
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnStatusPreColorChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiyDetectorPanel statusModel = d as DiyDetectorPanel;
            if (statusModel != null && statusModel.StatusPreColor != null)
            {
                statusModel.lblStatusColor.Background = statusModel.StatusPreColor;
            }
        }

        /// <summary>
        /// 表示探测器所处序号
        /// </summary>
        public string BoardIndex
        {
            get
            {
                return (string)GetValue(BoardIndexProperty);
            }
            set { SetValue(BoardIndexProperty, value); }
        }

        public static readonly DependencyProperty BoardIndexProperty =
            DependencyProperty.Register("BoardIndex", typeof(string), typeof(DiyDetectorPanel),
                 new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnBoardIndexChange)));
        /// <summary>
        /// 设置状态颜色
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnBoardIndexChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiyDetectorPanel statusModel = d as DiyDetectorPanel;
            if (statusModel != null && statusModel.BoardIndex != null)
            {
                statusModel.lblBoardIndexContent.Content = statusModel.BoardIndex;
            }
        }

        /// <summary>
        /// 表示探测器所处板卡序号
        /// </summary>
        public string BoardLineIndex
        {
            get
            {
                return (string)GetValue(BoardLineIndexProperty);
            }
            set { SetValue(BoardLineIndexProperty, value); }
        }

        public static readonly DependencyProperty BoardLineIndexProperty =
            DependencyProperty.Register("BoardLineIndex", typeof(string), typeof(DiyDetectorPanel),
                 new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnBoardLineIndexChange)));
        /// <summary>
        /// 设置状态颜色
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnBoardLineIndexChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiyDetectorPanel statusModel = d as DiyDetectorPanel;
            if (statusModel != null && statusModel.BoardLineIndex != null)
            {
                statusModel.lblBoardLineIndexContent.Content = statusModel.BoardLineIndex;
            }
        }

        /// <summary>
        /// 表示探测器状态
        /// </summary>
        public string BoardStatus
        {
            get
            {
                return (string)GetValue(BoardStatusProperty);
            }
            set { SetValue(BoardStatusProperty, value); }
        }

        public static readonly DependencyProperty BoardStatusProperty =
            DependencyProperty.Register("BoardStatus", typeof(string), typeof(DiyDetectorPanel),
                 new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnBoardStatusPropertyChange)));
        /// <summary>
        /// 表示探测器状态
        /// </summary>
        /// <param name="d"></param>^
        /// <param name="e"></param>
        public static void OnBoardStatusPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiyDetectorPanel statusModel = d as DiyDetectorPanel;
            if (statusModel != null && statusModel.BoardStatus != null)
            {
                statusModel.lblStatus.Content = UpdateStatusNameAction(statusModel.BoardStatus);
            }
        }
    }
}
