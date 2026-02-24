using BG_Services;
using CMW.Common.Utilities;
using BGModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
using BGUserControl;

namespace BGUserControl
{
    /// <summary>
    /// RayThumbCtl.xaml 的交互逻辑
    /// </summary>
    public partial class RayThumbCtl : UserControl
    {
        RayThumbCtlMvvm rayThumbCtlMvvm = null;
        public RayThumbCtl()
        {
            InitializeComponent();
            rayThumbCtlMvvm = new RayThumbCtlMvvm(RayCanvas);
            DataContext = rayThumbCtlMvvm;
        }
        /// <summary>
        /// 表示当前的等待状态
        /// </summary>
        [Bindable(true)]
        public  IEnumerable DataSource
        {
            get
            {
                return (IEnumerable)GetValue(DataSourceProperty);
            }
            set { SetValue(DataSourceProperty, value); }
        }
        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(IEnumerable), typeof(RayThumbCtl),
                new PropertyMetadata(new List<Tuple<Point, Point>>(), new PropertyChangedCallback(OnValueChange)));
        /// <summary>
        /// 设置文本
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RayThumbCtl rayThumbCtl = d as RayThumbCtl;
            if (rayThumbCtl != null && rayThumbCtl.DataSource != null)
            {
                rayThumbCtl.rayThumbCtlMvvm.SetPoint(rayThumbCtl.DataSource as List<Tuple<Point, Point>>);
            }
        }

        /// <summary>
        /// 表示当前的等待状态
        /// </summary>
        [Bindable(true)]
        public BitmapImage RayBackgroundImage
        {
            get
            {
                return (BitmapImage)GetValue(RayBackageImageProperty);
            }
            set { SetValue(RayBackageImageProperty, value); }
        }
        public static readonly DependencyProperty RayBackageImageProperty =
            DependencyProperty.Register("RayBackageImage", typeof(BitmapImage), typeof(RayThumbCtl),
                new PropertyMetadata(new BitmapImage(), new PropertyChangedCallback(OnRayBackgroundImageValueChange)));
        /// <summary>
        /// 设置文本
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnRayBackgroundImageValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RayThumbCtl rayThumbCtl = d as RayThumbCtl;
            if (rayThumbCtl != null && rayThumbCtl.RayBackgroundImage != null)
            {
                rayThumbCtl.rayThumbCtlMvvm.SetRayImageBackage(rayThumbCtl.RayBackgroundImage);
            }
        }
    }

    public class RayThumbCtlMvvm : BaseMvvm
    {
        Storyboard myStoryBoard = new Storyboard();
        Canvas RayCanvas = new Canvas();
        List<Tuple<Point, Point>> _PointList = new List<Tuple<Point, Point>>();
        Visibsliy _SmallRayImage = new Visibsliy() { IsShow = Visibility.Hidden };
        public Visibsliy SmallRayImage { get { return _SmallRayImage; } set { _SmallRayImage = value; RaisePropertyChanged("SmallRayImage"); } }
        public List<Tuple<Point, Point>> PointList { get { return _PointList; } set { _PointList = value; RaisePropertyChanged("PointList"); } }
        public static bool IsRay = false;

        public RayThumbCtlMvvm(Canvas _RayCanvas)
        {
            InitRayImage();
            RayCanvas = _RayCanvas;
            LocalDefaultPoint();
        }

        public void SetPoint(List<Tuple<Point,Point>> _OutPoints)
        {
            PointList = _OutPoints;
        }
        public void SetRayImageBackage(BitmapImage bitmapImage)
        {
            SmallRayImage.ImageBitmapSource = bitmapImage;
        }
        private void LocalDefaultPoint()
        {
            PointList = new List<Tuple<Point, Point>>()
                {
                    Tuple.Create<Point, Point>(new Point(150,120),new Point(80,130)),
                    Tuple.Create<Point, Point>(new Point(150,120),new Point(80,140)),
                    Tuple.Create<Point, Point>(new Point(150,120),new Point(80,150)),
                    Tuple.Create<Point, Point>(new Point(150,120),new Point(80,160)),
                    Tuple.Create<Point, Point>(new Point(150,120),new Point(80,170)),
                    Tuple.Create<Point, Point>(new Point(150,120),new Point(80,180)),
                    Tuple.Create<Point, Point>(new Point(150,120),new Point(80,200)),
                    Tuple.Create<Point, Point>(new Point(150,120),new Point(80,220)),
                    Tuple.Create<Point, Point>(new Point(150,120),new Point(80,240)),
                    Tuple.Create<Point, Point>(new Point(150,120),new Point(90,250)),
                    Tuple.Create<Point, Point>(new Point(150,120),new Point(100,254)),
                    Tuple.Create<Point, Point>(new Point(150,120),new Point(110,254)),
                    Tuple.Create<Point, Point>(new Point(150,120),new Point(120,254)),
                    Tuple.Create<Point, Point>(new Point(150,120),new Point(130,254)),
                    Tuple.Create<Point, Point>(new Point(150,120),new Point(140,254)),
                    Tuple.Create<Point, Point>(new Point(150,120),new Point(150,254)),
                    Tuple.Create<Point, Point>(new Point(150,120),new Point(160,254)),
                    Tuple.Create<Point, Point>(new Point(150,120),new Point(170,254)),
                    Tuple.Create<Point, Point>(new Point(150,120),new Point(180,254)),
                    Tuple.Create<Point, Point>(new Point(150,120),new Point(190,254)),
                    Tuple.Create<Point, Point>(new Point(150,120),new Point(200,250)),
                    Tuple.Create<Point, Point>(new Point(150,120),new Point(210,240)),
                    Tuple.Create<Point, Point>(new Point(150,120),new Point(210,220)),
                    Tuple.Create<Point, Point>(new Point(150,120),new Point(210,210)),
                    Tuple.Create<Point, Point>(new Point(150,120),new Point(210,200)),
                    Tuple.Create<Point, Point>(new Point(150,120),new Point(210,190)),
                    Tuple.Create<Point, Point>(new Point(150,120),new Point(210,180)),
                    Tuple.Create<Point, Point>(new Point(150,120),new Point(210,170)),
                    Tuple.Create<Point, Point>(new Point(150,120),new Point(210,160)),
                    Tuple.Create<Point, Point>(new Point(150,120),new Point(210,150)),
                    Tuple.Create<Point, Point>(new Point(150,120),new Point(210,140)),
                    Tuple.Create<Point, Point>(new Point(150,120),new Point(210,130)),
                };
        }

        public void InitRayImage()
        {
            SmallRayImage.ImageBitmapSource = new BitmapImage(new Uri("../Image/Ray.png", UriKind.RelativeOrAbsolute));
            myStoryBoard.RepeatBehavior = RepeatBehavior.Forever;
        }

        public void Ray()
        {
            try
            {
                if (IsRay) return;
                if (PointList == null) return;

                IsRay = true;
                foreach (Tuple<Point, Point> item in PointList)
                {
                    RayAnimation(item.Item1, item.Item2);
                }
                Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                {
                    myStoryBoard.Begin();   //开始动画
                }));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void StopRay()
        {
            Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsRay) return;
                IsRay = false;
                myStoryBoard.Children.Clear();
                RayCanvas.Children.Clear();
                myStoryBoard.Stop();
            }));
        }

        private void RayAnimation(Point from, Point to)
        {
            Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
            {
                Line myLine = new Line();
                myLine.StrokeDashArray = new DoubleCollection() {0.1,0.1};
                myLine.Stroke = System.Windows.Media.Brushes.Red;
                myLine.StrokeThickness = 3;
                myLine.X1 = from.X;   //必须要设置Line的起点X1，Y1，X2,Y2则不需要设置
                myLine.Y1 = from.Y;
                RayCanvas.Children.Add(myLine);   //添加到Canvas1中

                DoubleAnimation animationX = new DoubleAnimation();   //两个动画，分别负责myLine.X2和myLine.Y2的变化
                DoubleAnimation animationY = new DoubleAnimation();
                animationX.Duration = TimeSpan.FromMilliseconds(1000);
                animationY.Duration = TimeSpan.FromMilliseconds(1000);
                animationX.From = from.X;
                animationX.To = to.X;
                animationY.From = from.Y;
                animationY.To = to.Y;
                Storyboard.SetTarget(animationX, myLine);   //设置Animation的目标
                Storyboard.SetTarget(animationY, myLine);
                Storyboard.SetTargetProperty(animationX, new System.Windows.PropertyPath("(Line.X2)"));  //指定目标的属性
                Storyboard.SetTargetProperty(animationY, new System.Windows.PropertyPath("(Line.Y2)"));
                myStoryBoard.Children.Add(animationX);   //添加到StoryBoard中
                myStoryBoard.Children.Add(animationY);
            }));
        }

        protected override void ConnectionStatus(bool ConnectStatus)
        {
            if (!ConnectStatus || !BoostingControllerManager.GetInstance().IsRayOut())
            {
                StopRay();
                return;
            }
            Ray();
        }
    }
}
