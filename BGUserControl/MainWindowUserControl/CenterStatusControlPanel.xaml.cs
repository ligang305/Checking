using BG_Entities;
using BG_Services;
using BG_WorkFlow;
using BGModel;
using BGUserControl;
using CMW.Common.Utilities;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BGUserControl
{
    /// <summary>
    /// CenterStatusControlPanel.xaml 的交互逻辑
    /// </summary>
    public partial class CenterStatusControlPanel : UserControl
    {
        protected CenterStatusControlPanelMvvm centerStatusControlPanelMvvm = new CenterStatusControlPanelMvvm();

        /// <summary>
        /// 表示当前的等待状态
        /// </summary>
        [Bindable(true)]
        public Dictionary<string, PLCPositionEnum> DataSource
        {
            get
            {
                return (Dictionary<string, PLCPositionEnum>)GetValue(DataSourceProperty);
            }
            set { SetValue(DataSourceProperty, value); }
        }

        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(Dictionary<string, PLCPositionEnum>), typeof(CenterStatusControlPanel),
                new PropertyMetadata(new Dictionary<string, PLCPositionEnum>(), new PropertyChangedCallback(OnValueChange)));
        /// <summary>
        /// 设置文本
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CenterStatusControlPanel centerStatusControlPanel = d as CenterStatusControlPanel;
            if (centerStatusControlPanel != null && centerStatusControlPanel.DataSource != null)
            {
                centerStatusControlPanel.StopPanel.SetContent(centerStatusControlPanel.DataSource as Dictionary<string, PLCPositionEnum>);
            }
        }

        /// <summary>
        /// 表示出束小图的出束射线动画的Points的数据源
        /// </summary>
        [Bindable(true)]
        public IEnumerable PointsDataSource
        {
            get
            {
                return (IEnumerable)GetValue(PointsDataSourceProperty);
            }
            set { SetValue(PointsDataSourceProperty, value); }
        }
        public static readonly DependencyProperty PointsDataSourceProperty =
            DependencyProperty.Register("PointsDataSource", typeof(IEnumerable), typeof(CenterStatusControlPanel),
                new PropertyMetadata(new List<Tuple<Point, Point>>(), new PropertyChangedCallback(OnPointsValueChange)));
        /// <summary>
        /// 设置射线源
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnPointsValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CenterStatusControlPanel centerStatusControlPanel = d as CenterStatusControlPanel;
            if (centerStatusControlPanel != null && centerStatusControlPanel.PointsDataSource != null)
            {
                centerStatusControlPanel.RayThumb.DataSource = centerStatusControlPanel.PointsDataSource;
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
            DependencyProperty.Register("RayBackgroundImage", typeof(BitmapImage), typeof(CenterStatusControlPanel),
                new PropertyMetadata(new BitmapImage(), new PropertyChangedCallback(OnRayBackgroundImageValueChange)));
        /// <summary>
        /// 设置文本
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnRayBackgroundImageValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CenterStatusControlPanel centerStatusControlPanel = d as CenterStatusControlPanel;
            if (centerStatusControlPanel != null && centerStatusControlPanel.DataSource != null)
            {
                centerStatusControlPanel.RayThumb.RayBackgroundImage = centerStatusControlPanel.RayBackgroundImage;
            }
        }
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
            DependencyProperty.Register("MainPageImageBackgroundImage", typeof(BitmapImage), typeof(CenterStatusControlPanel),
                new PropertyMetadata(new BitmapImage(), new PropertyChangedCallback(OnMainPageImageBackgroundImageValueChange)));
        /// <summary>
        /// 设置文本
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnMainPageImageBackgroundImageValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CenterStatusControlPanel centerStatusControlPanel = d as CenterStatusControlPanel;
            if (centerStatusControlPanel != null && centerStatusControlPanel.DataSource != null)
            {
                centerStatusControlPanel.mainPageImage.MainPageImageBackgroundImage = centerStatusControlPanel.MainPageImageBackgroundImage;
            }
        }
        /// <summary>
        /// 表示当前的等待状态
        /// </summary>
        [Bindable(true)]
        public Visibility IsShowElectronFence
        {
            get
            {
                return (Visibility)GetValue(IsShowElectronFenceProperty);
            }
            set { SetValue(IsShowElectronFenceProperty, value); }
        }
        public static readonly DependencyProperty IsShowElectronFenceProperty =
            DependencyProperty.Register("IsShowElectronFence", typeof(Visibility), typeof(CenterStatusControlPanel),
                new PropertyMetadata(Visibility.Collapsed, new PropertyChangedCallback(OnIsShowElectronFenceValueChange)));
        /// <summary>
        /// 设置文本
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnIsShowElectronFenceValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CenterStatusControlPanel centerStatusControlPanel = d as CenterStatusControlPanel;
            if (centerStatusControlPanel != null && centerStatusControlPanel.DataSource != null)
            {
                centerStatusControlPanel.ElectronicFence.Visibility = centerStatusControlPanel.IsShowElectronFence;
            }
        }
        public CenterStatusControlPanel()
        {
            InitializeComponent();
            DataContext = centerStatusControlPanelMvvm;
            InitEnv();
            if(ConfigServices.GetInstance().localConfigModel.IsUserBS && Common.controlVersion != ControlVersion.BS)
            {
                RayThumb.Visibility = Visibility.Collapsed;
                BSDoseStatusBar.Visibility = Visibility.Visible;
            }
            StopPanel.SetContent(EmergenceStop());
            ElectronicFence.SetBinding(ElectronicFence.VisibilityProperty,new Binding("ElectronicFence.IsShow") { Source = centerStatusControlPanelMvvm ,Mode = BindingMode.TwoWay});
        }

        private void InitEnv()
        {
            ScanImageService.GetInstance().MessaageShowAction += delegate (string message) {
                this.Dispatcher.BeginInvoke((Action)delegate ()
                {
                    BG_MESSAGEBOX.Show(CommonDeleget.UpdateStatusNameAction("Tip"), CommonDeleget.UpdateStatusNameAction(message));
                });
            };
        }
        public Dictionary<string, PLCPositionEnum> EmergenceStop()
        {
            Dictionary<string, PLCPositionEnum> StopStr = new Dictionary<string, PLCPositionEnum>();
            StopStr.Add("DoseAlarm", PLCPositionEnum.DoseAlarm);
            StopStr.Add("NeedReset", PLCPositionEnum.Null);
            return StopStr;
        }

        private void mainPageImage_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }

    public class CenterStatusControlPanelMvvm : BaseMvvm
    {
        Visibsliy _ScanImageDoubleTipVisibsliy = new Visibsliy() { IsShow = Visibility.Hidden };
        public Visibsliy ScanImageDoubleTipVisibsliy
        {
            get => _ScanImageDoubleTipVisibsliy;
            set
            {
                _ScanImageDoubleTipVisibsliy = value;
                RaisePropertyChanged("ScanImageDoubleTipVisibsliy");
            }
        }
        Visibsliy _ScanPreview = new Visibsliy() { IsShow = Visibility.Hidden };
        public Visibsliy ScanPreview
        {
            get => _ScanPreview;
            set
            {
                _ScanPreview = value;
                RaisePropertyChanged("ScanPreview");
            }
        }
        Visibsliy _ElectronicFence = new Visibsliy() { IsShow = Visibility.Collapsed };
        public Visibsliy ElectronicFence
        {
            get => _ElectronicFence;
            set
            {
                _ElectronicFence = value;
                RaisePropertyChanged("ElectronicFence");
            }
        }
        private Dispatcher UIDispatcher => Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
        protected async override void ConnectionStatus(bool ConnectStatus)
        {
            if (ConnectStatus)
            {
                await UIDispatcher.InvokeAsync(() =>
                {
                    ScanPreview.IsShow =
                    ConfigServices.GetInstance().localConfigModel.IsShowDirection ?
                    PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.ScanMode) ?
                    Visibility.Visible :
                    ConfigServices.GetInstance().localConfigModel.IsShowDualDirection ?
                    Visibility.Visible : Visibility.Hidden : Visibility.Hidden;
                });
            }
        }
        public CenterStatusControlPanelMvvm()
        {
            ButtonInvoke.DoubleClickToOpenEvent += DoubleClickToOpen;
        }
        public void DoubleClickToOpen(bool OpenOrClose)
        {
            ScanImageDoubleTipVisibsliy.IsShow = OpenOrClose ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
