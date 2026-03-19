using BG_Services;
using CMW.Common.Utilities;
using BGModel;
using System;
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
    /// MainPageImageCtl.xaml 的交互逻辑
    /// </summary>
    public partial class MainPageImageCtl : UserControl
    {
        MainPageImageCtlMvvm mainPageImageCtlMvvm = new MainPageImageCtlMvvm();
        /// <summary>
        /// 表示当前的等待状态
        /// </summary>
        [Bindable(true)]
        public BitmapImage MainPageImageBackgroundImage
        {
            get
            {
                return (BitmapImage)GetValue(MainPageImageBackgroundImageProperty);
            }
            set { SetValue(MainPageImageBackgroundImageProperty, value); }
        }
        public static readonly DependencyProperty MainPageImageBackgroundImageProperty =
            DependencyProperty.Register("MainPageImageBackgroundImage", typeof(BitmapImage), typeof(MainPageImageCtl),
                new PropertyMetadata(new BitmapImage(), new PropertyChangedCallback(OnMainPageImageBackgroundImageValueChange)));
        /// <summary>
        /// 设置文本
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnMainPageImageBackgroundImageValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MainPageImageCtl mainPageImageCtl = d as MainPageImageCtl;
            if (mainPageImageCtl != null && mainPageImageCtl.MainPageImageBackgroundImage != null)
            {
                mainPageImageCtl.mainPageImageCtlMvvm.MainVisibly.ImageBitmapSource = mainPageImageCtl.MainPageImageBackgroundImage;
                mainPageImageCtl.mainPageImageCtlMvvm.CarVisibly.ImageBitmapSource =null;
                mainPageImageCtl.mainPageImageCtlMvvm.AccleatorVisibly.ImageBitmapSource = null;
            }
        }
        public MainPageImageCtl()
        {
            InitializeComponent();
            DataContext = mainPageImageCtlMvvm;
            SwitchMainPageImage();
            ScrollImageServices.Service.ScrollImageEvent += ScrollImage;
        }

        public void SwitchMainPageImage()
        {
            //MainImage.Source = new BitmapImage(new Uri("../Image/PassageCar.png", UriKind.RelativeOrAbsolute));
            //CarImage.Source = new BitmapImage(new Uri("../Image/Car.png", UriKind.RelativeOrAbsolute));
            //AcclaterImage.Source = new BitmapImage(new Uri("../Image/Accleator.png", UriKind.RelativeOrAbsolute));
            mainPageImageCtlMvvm.MainVisibly.ImageBitmapSource = new BitmapImage(new Uri("pack://application:,,,/BGUserControl;component/MainWindowUserControl/ChildrenControl/Image/PassageCar.png", UriKind.RelativeOrAbsolute));
            mainPageImageCtlMvvm.CarVisibly.ImageBitmapSource = new BitmapImage(new Uri("../Image/Car.png", UriKind.RelativeOrAbsolute));
            mainPageImageCtlMvvm.AccleatorVisibly.ImageBitmapSource = new BitmapImage(new Uri("../Image/Accleator.png", UriKind.RelativeOrAbsolute));
        }
        Storyboard myStoryBoard = new Storyboard();
        ThicknessAnimation da = new ThicknessAnimation();

        private void ScrollImage(bool IsScrollImage)
        {
            if(IsScrollImage)
            {
                ScaningCar();
            }
            else
            {
                myStoryBoard.Stop();
            }
        }

        private void ScaningCar()
        {
            myStoryBoard.RepeatBehavior = RepeatBehavior.Forever;
            da.From = new Thickness(0, 0, 0, 0);
            da.To = new Thickness(0, 0, 1000, 0);
            da.Duration = new Duration(TimeSpan.FromSeconds(60));
            Storyboard.SetTarget(da, CarImage);
            Storyboard.SetTargetProperty(da, new PropertyPath("Margin"));
            myStoryBoard.Children.Add(da);
            myStoryBoard.Begin();
        }

       
    }

    public class MainPageImageCtlMvvm : BaseMvvm
    {
        Visibsliy _mainVisibly = new Visibsliy() { IsShow = Visibility.Hidden };
        public Visibsliy MainVisibly { get { return _mainVisibly; } set { _mainVisibly = value; RaisePropertyChanged("MainVisibly"); } }

        Visibsliy _carVisibly = new Visibsliy() { IsShow = Visibility.Hidden };
        public Visibsliy CarVisibly { get { return _carVisibly; } set { _carVisibly = value; RaisePropertyChanged("CarVisibly"); } }
        Visibsliy _accleatorVisibly = new Visibsliy() { IsShow = Visibility.Hidden };
        public Visibsliy AccleatorVisibly { get { return _accleatorVisibly; } set { _accleatorVisibly = value; RaisePropertyChanged("AccleatorVisibly"); } }
        public MainPageImageCtlMvvm()
        {
            
        }
    }
}
