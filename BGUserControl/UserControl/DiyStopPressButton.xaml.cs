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

using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonFunc;
using static CMW.Common.Utilities.CommonDeleget;
using CMW.Common.Utilities;
using BG_Entities;

namespace BGUserControl
{
    /// <summary>
    /// DiyStopPressButton.xaml 的交互逻辑
    /// </summary>
    public partial class DiyStopPressButton : UserControl
    {
        public DiyStopPressButton()
        {
            InitializeComponent();
            Loaded += DiyStopPressButton_Loaded;
        }

        private void DiyStopPressButton_Loaded(object sender, RoutedEventArgs e)
        {
            PressStop.Source = bitImgDic[HandStatus.StopPressOn];
        }

        /// <summary>
        /// 表示图片数据源
        /// </summary>
        public BitmapImage DiyImageSource
        {
            get
            {
                return (BitmapImage)GetValue(DiyImageSourceProperty);
            }
            set { SetValue(DiyImageSourceProperty, value); }
        }

        public static readonly DependencyProperty DiyImageSourceProperty =
            DependencyProperty.Register("DiyImageSource", typeof(BitmapImage), typeof(DiyStopPressButton),
                 new PropertyMetadata(null, new PropertyChangedCallback(OnDiyImageSourceValueChange)));
        /// <summary>
        /// 设置文本框长度
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnDiyImageSourceValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiyStopPressButton btnPress = d as DiyStopPressButton;
            if (btnPress != null && btnPress.DiyImageSource != null)
            {
                btnPress.PressStop.Source = btnPress.DiyImageSource;
            }
        }

        private bool _isPressOn = false;
        /// <summary>
        /// 是否按下了急停
        /// </summary>
        public bool isPressOn
        {
            get { return _isPressOn; }
            set { _isPressOn = value;
                if(!_isPressOn)
                {
                    PressStop.Source = bitImgDic[HandStatus.StopPressOn];
                }
                else
                {
                    PressStop.Source = bitImgDic[HandStatus.StopPressOff];
                }
            }
        }

        /// <summary>
        /// 按下急停图片进行变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string Content = isPressOn? UpdateStatusNameAction("SureReset"): UpdateStatusNameAction("SureStop");
            //BG_STOP.Show(UpdateStatusNameAction("Warning"),"12312321");
            //if (MessageBox.Show(Content, UpdateStatusNameAction("Warning"), MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            //{
                try
                {
                    SetCommand(CommandDic[Command.SpeedStop], !isPressOn);
                    isPressOn = !isPressOn;
                }
                catch (Exception ex)
                {
                    WriteLogAction(ex.Message, LogType.ApplicationError, true);
                }
            //}
        }
        /// <summary>
        /// 鼠标移入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
               PressStop.Source = bitImgDic[HandStatus.StopPressOffMouseEnter];
            }
            catch (Exception ex)
            {
                WriteLogAction(ex.Message,LogType.ApplicationError, true);
            }
        }
        /// <summary>
        /// 鼠标移出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                //如果点击了急停
                if (isPressOn)
                {
                    PressStop.Source = bitImgDic[HandStatus.StopPressOff];
                }
                //如果没点击急停
                else
                {
                    PressStop.Source = bitImgDic[HandStatus.StopPressOn];
                }
            }
            catch (Exception ex)
            {
                WriteLogAction(ex.Message, LogType.ApplicationError, true);
            }
        }
    }
}
