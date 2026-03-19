using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
using static CMW.Common.Utilities.CommonFunc;

namespace BGUserControl
{
    /// <summary>
    /// XrayIcon.xaml 的交互逻辑
    /// </summary>
    public partial class StartingDynamoIcon : UserControl
    {
        StartingDynamoIconMvvm startingDynamoIconMvvm;

        /// <summary>
        /// 属性，用来接收外部传输进来的数据对象
        /// </summary>
        public string IconForeColor
        {
            get
            {
                return (string)GetValue(IconForeColorProperty);
            }
            set { SetValue(IconForeColorProperty, value); }
        }

        /// <summary>
        /// 数据源依赖属性
        /// </summary>
        public static readonly DependencyProperty IconForeColorProperty =
            DependencyProperty.Register("IconForeColor", typeof(string), typeof(StartingDynamoIcon),
                 new PropertyMetadata("#000000", new PropertyChangedCallback(OnValueChange)));

        /// <summary>
        /// 属性，用来接收外部传输进来的数据对象
        /// </summary>
        public string IconBkgColor
        {
            get
            {
                return (string)GetValue(IconBkgColorProperty);
            }
            set { SetValue(IconBkgColorProperty, value); }
        }

        /// <summary>
        /// 数据源依赖属性
        /// </summary>
        public static readonly DependencyProperty IconBkgColorProperty =
            DependencyProperty.Register("IconBkgColor", typeof(string), typeof(StartingDynamoIcon),
                 new PropertyMetadata("#DFE3E8", new PropertyChangedCallback(OnBkgValueChange)));


        /// <summary>
        /// 是否启动闪烁
        /// </summary>
        public bool IsShowFlash
        {
            get
            {
                return (bool)GetValue(IsShowFlashProperty);
            }
            set { SetValue(IsShowFlashProperty, value); }
        }

        /// <summary>
        /// 数据源依赖属性
        /// </summary>
        public static readonly DependencyProperty IsShowFlashProperty =
            DependencyProperty.Register("IsShowFlash", typeof(bool), typeof(StartingDynamoIcon),
                 new PropertyMetadata(false, new PropertyChangedCallback(OnFlashValueChange)));
        /// <summary>
        /// 时间用于刷新变化值，其他值如果没有变化的话不会刷新，而时间会一直更新，所以将此作为入口
        /// </summary>
        public DateTime UpdateSource
        {
            get
            {
                return (DateTime)GetValue(UpdateSourceProperty);
            }
            set { SetValue(UpdateSourceProperty, value); }
        }

        /// <summary>
        /// 数据源依赖属性
        /// </summary>
        public static readonly DependencyProperty UpdateSourceProperty =
            DependencyProperty.Register("UpdateSource", typeof(DateTime), typeof(StartingDynamoIcon),
                 new PropertyMetadata(DateTime.Now, new PropertyChangedCallback(OnUpdateTimeValueChange)));


        public StartingDynamoIcon()
        {
            InitializeComponent();
            startingDynamoIconMvvm = new StartingDynamoIconMvvm(BackImage);
            this.DataContext = startingDynamoIconMvvm;
        }

        /// <summary>
        /// 接收到值 回调的方法
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StartingDynamoIcon XrayIcon = d as StartingDynamoIcon;
            if (XrayIcon != null && XrayIcon.IconForeColor != null)
            {

                XrayIcon.startingDynamoIconMvvm.RayFrg = XrayIcon.IconForeColor;
            }
        }
        /// <summary>
        /// 接收到值 回调的方法
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnBkgValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StartingDynamoIcon XrayIcon = d as StartingDynamoIcon;
            if (XrayIcon != null && XrayIcon.IconBkgColor != null)
            {
                XrayIcon.startingDynamoIconMvvm.RayBkg = XrayIcon.IconBkgColor;
            }
        }

        /// <summary>
        /// 接收到值 回调的方法
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnFlashValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StartingDynamoIcon XrayIcon = d as StartingDynamoIcon;
            if (XrayIcon != null && XrayIcon.IsShowFlash != null)
            {
                XrayIcon.startingDynamoIconMvvm.StartFlash(XrayIcon.IsShowFlash);
            }
        }

        /// <summary>
        /// 接收到值 回调的方法
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnUpdateTimeValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StartingDynamoIcon XrayIcon = d as StartingDynamoIcon;
            if (XrayIcon != null && XrayIcon.UpdateSource != null)
            {
                XrayIcon.startingDynamoIconMvvm.StartFlash(XrayIcon.IsShowFlash);
            }
        }
    }

    public class StartingDynamoIconMvvm : ViewModelBase
    {
        bool LastFlashStatus = false;
        /// <summary>
        /// 射线图标 前景色
        /// </summary>
        private string _RayFrg = "#00FF00"; //(SolidColorBrush)Application.Current.Resources["XRayOffFrgBrush"];//(SolidColorBrush)StrToBrush("#000000");// 
        public string RayFrg
        {
            get => _RayFrg;
            set
            {
                if (_RayFrg == value)
                {
                    return;
                }
                _RayFrg = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 射线图标 背景色
        /// </summary>
        private string _RayBkg = "#ffffff";// (SolidColorBrush)Application.Current.Resources["XRayOffBkgBrush"];
        public string RayBkg
        {
            get => _RayBkg;
            set
            {
                if (_RayBkg == value)
                {
                    return;
                }
                _RayBkg = value;
                RaisePropertyChanged();
            }
        }

        private ContentControl _BackImage;

        public StartingDynamoIconMvvm(ContentControl backContent)
        {
            _BackImage = backContent;
            InitStoryBoard();
        }

        public void StartFlash(bool IsShowFlash)
        {
            if (LastFlashStatus == IsShowFlash) return;

            if (LastFlashStatus != IsShowFlash)
            {
                LastFlashStatus = IsShowFlash;
            }
            if (!LastFlashStatus)
            {
                RayFrg = "#000000"; 
                Application.Current.Dispatcher.BeginInvoke(new Action(() => { sb.Stop(_BackImage); }));  }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => { sb.Begin(_BackImage, true); }));
            }
            Task.Run(() =>
            {
                while (LastFlashStatus)
                {
                    RayFrg = "#00FF00";// "#FFFF00" : "#DFE3E8";}));
                    Thread.Sleep(250);
                    RayFrg = "#FF0000";// "#FFFF00" : "#DFE3E8";
                    Thread.Sleep(250);
                }
            });
        }
        Storyboard sb;
        private void InitStoryBoard()
        {
            DependencyProperty[] propertyChain = new DependencyProperty[]
            {
                Border.RenderTransformProperty,
                RotateTransform.AngleProperty
            };
            _BackImage.RenderTransformOrigin = new Point(0.5, 0.5);
            _BackImage.RenderTransform = new RotateTransform();
            sb = new Storyboard();
            DoubleAnimation da = new DoubleAnimation()
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(500)),
                From = 0,
                By = 360,
            };
            Storyboard.SetTargetProperty(da, new PropertyPath("(0).(1)", propertyChain));
            sb.Children.Add(da);
            sb.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            sb.RepeatBehavior = RepeatBehavior.Forever;
            sb.Stop(_BackImage);
        }
    }
}
