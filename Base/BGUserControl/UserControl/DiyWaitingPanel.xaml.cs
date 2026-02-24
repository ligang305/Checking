using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CMW.Common.Utilities;
using BGModel;
using BG_Entities;

namespace BGUserControl
{
    /// <summary>
    /// DiyWaitingPanel.xaml 的交互逻辑
    /// </summary>
    public partial class DiyWaitingPanel : UserControl
    {
        public DiyWaitingPanel()
        {
            InitializeComponent();
            try
            {
                ReadyTimer.Interval = 1000;
                ReadyTimer.Elapsed += ReadyTimer_Elapsed;
                InitStoryBoard();
                sb.Begin(BackImage, true);
            }
            catch (Exception ex)
            {
                CommonDeleget.WriteLogAction(ex.Message, LogType.ApplicationError, true);
            }
        }

        private void ReadyTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
            {
                lblTime.Content = $@"({--TotalTime}S)";
                if (TotalTime <= 0)
                {
                    ReadyTimer.Stop();      
                }
            }));
        }

        Timer ReadyTimer = new Timer();
        private int TotalTime;
        /// <summary>
        /// 表示当前的等待状态
        /// </summary>
        public SignalModel CurrentStatus
        {
            get
            {
                return (SignalModel)GetValue(CurrentStatusProperty);
            }
            set { SetValue(CurrentStatusProperty, value); }
        }

        public static readonly DependencyProperty CurrentStatusProperty =
            DependencyProperty.Register("CurrentStatus", typeof(SignalModel), typeof(DiyWaitingPanel),
                new PropertyMetadata(new SignalModel(), new PropertyChangedCallback(OnValueChange)));
        /// <summary>
        /// 设置文本
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiyWaitingPanel Panel = d as DiyWaitingPanel;
            if (Panel != null && Panel.CurrentStatus != null)
            {
                Panel.lblStatus.Content = Panel.CurrentStatus.SignalValue;
            }
        }

        public void SetContent(string Content)
        {
            Application.Current?.Dispatcher.BeginInvoke(new Action(() => { this.lblStatus.Content = Content; }));
        }
        public void StartTimer(int time)
        {
            TotalTime = time;
            ReadyTimer.Start();
        }
        //public void StopTimer()
        //{
        //    TotalTime = 0;
        //    ReadyTimer.Stop();
        //}
        Storyboard sb;

        private void InitStoryBoard()
        {
            DependencyProperty[] propertyChain = new DependencyProperty[]
            {
                Border.RenderTransformProperty,
                RotateTransform.AngleProperty
            };
            BackImage.RenderTransformOrigin = new Point(0.5, 0.5);
            BackImage.RenderTransform = new RotateTransform();
            sb = new Storyboard();
            DoubleAnimation da = new DoubleAnimation()
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(1000)),
                From = 0,
                By = 360,
            };
            Storyboard.SetTargetProperty(da, new PropertyPath("(0).(1)", propertyChain));
            sb.Children.Add(da);
            sb.Duration = new Duration(TimeSpan.FromMilliseconds(1000));
            sb.RepeatBehavior = RepeatBehavior.Forever;
            sb.Stop(BackImage);
        }
    }

}
