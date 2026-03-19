using BG_Entities;
using BG_Services;
using BG_WorkFlow;
using BGLogs;
using BGModel;
using CMW.Common.Utilities;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.ImageImportDll;
using Image = System.Windows.Controls.Image;

namespace BGUserControl.MainWindowUserControl.ChildrenControl.BS.BS2000
{

    /// <summary>
    /// 适用于背散的卷图查看组件
    /// </summary>
    public partial class BSView : UserControl
    {
      
        #region 依赖属性 
        /// <summary>
        /// 探测器的IP地址
        /// </summary>
        [Bindable(true)]
        public string DetectorIpAddress
        {
            get
            {
                return (string)GetValue(DetectorIpAddressProperty);
            }
            set { SetValue(DetectorIpAddressProperty, value); bSViewViewModel.DetectorIpAddress = value; }
        }

        public static readonly DependencyProperty DetectorIpAddressProperty =
            DependencyProperty.Register("DetectorIpAddress", typeof(string), typeof(BSView),
                new PropertyMetadata("127.0.0.1", new PropertyChangedCallback(OnValueChange)));

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BSView BSView = d as BSView;
            if (BSView != null && BSView.DetectorIpAddressProperty != null)
            {
                BSView.bSViewViewModel.DetectorIpAddress = BSView.DetectorIpAddress;
            }
        }
        /// <summary>
        /// 探测器指令端口
        /// </summary>
        [Bindable(true)]
        public string DetectorCommandPort
        {
            get
            {
                return (string)GetValue(DetectorCommandPortProperty);
            }
            set { SetValue(DetectorCommandPortProperty, value); bSViewViewModel.DetectorCommandPort = Convert.ToInt16(value); }
        }

        public static readonly DependencyProperty DetectorCommandPortProperty =
            DependencyProperty.Register("DetectorCommandPort", typeof(string), typeof(BSView),
                new PropertyMetadata("3000", new PropertyChangedCallback(OnDetectorCommandPortValueChange)));

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnDetectorCommandPortValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BSView BSView = d as BSView;
            if (BSView != null && BSView.DetectorCommandPortProperty != null)
            {
                BSView.bSViewViewModel.DetectorCommandPort = Convert.ToInt16(BSView.DetectorCommandPort);
            }
        }

        /// <summary>
        /// 探测器图像端口
        /// </summary>
        [Bindable(true)]
        public string DetectorImagePort
        {
            get
            {
                return (string)GetValue(DetectorImagePortProperty);
            }
            set { SetValue(DetectorImagePortProperty, value); bSViewViewModel.DetectorImagePort = Convert.ToInt16(value); }
        }

        public static readonly DependencyProperty DetectorImagePortProperty =
            DependencyProperty.Register("DetectorImagePort", typeof(string), typeof(BSView),
                new PropertyMetadata("4001", new PropertyChangedCallback(OnDetectorImagePortValueChange)));

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnDetectorImagePortValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BSView BSView = d as BSView;
            if (BSView != null && BSView.DetectorImagePortProperty != null)
            {
                BSView.bSViewViewModel.DetectorImagePort = Convert.ToInt16(BSView.DetectorImagePort);
            }
        }
        #endregion

        #region 背散路径回调事件
        public Action<BG_BS2000,int, int, string,int> BSViewScanImagePathAction;
        #endregion

        BSViewViewModel bSViewViewModel = new BSViewViewModel();
        public BSView()
        {
            InitializeComponent();
            DataContext = bSViewViewModel;
            bSViewViewModel.BSViewMvvmScanImagePathAction += BSImageScanCallback;
            Unloaded += BSView_Unloaded;
        }

        private void BSView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (bSViewViewModel != null)
            {
                bSViewViewModel.Stop(); 
            }
        }

        protected override void OnRenderSizeChanged(System.Windows.SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            this.InvalidateVisual();
            if(bSViewViewModel != null)
            {
                bSViewViewModel.OnRenderSizeChanged(sizeInfo,ImageContent);
            }
        }

        public void Start(BSPackage _bsBs2000)
        {
            if(bSViewViewModel!=null)
            {
                if (Convert.ToInt32(_bsBs2000.Bs2000.ViewCount) == 2)
                {
                    ImageContent.RowDefinitions.Add(new RowDefinition(){ Height = new GridLength(1,GridUnitType.Star)});
                    Image image = new Image()
                    {
                        Stretch = Stretch.Fill,
                    };
                    image.SetBinding(Image.SourceProperty,new Binding("BSViewImageSource2") { Source = bSViewViewModel});
                    Panel.SetZIndex(image,1000);
                    Grid.SetRow(image, 0);
                    Grid.SetRow(View1, 1);
                    Grid.SetColumn(image,1);
                    Grid.SetRowSpan(Operation, 2);
                    ImageContent.Children.Add(image);
                }
                bSViewViewModel.BSPackage = _bsBs2000;
                bSViewViewModel.Start();
            }
        }
        private void BSImageScanCallback(BG_BS2000 bG_BS2000,int status, int carrid, string path,int ViewIndex)
        {
            BSViewScanImagePathAction?.Invoke(bG_BS2000, status, carrid, path, ViewIndex);
        }
    }

    public class BSViewViewModel : BaseMvvm
    {
        // Win32 memory set function
        //[DllImport("ntdll.dll")]
        //[DllImport("coredll.dll", EntryPoint = "memset", SetLastError = false)]
        [DllImport("msvcrt.dll", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        private static extern void memset(
            IntPtr dst,
            int filler,
            int count);
        #region 私有变量
        #region 背散路径回调事件
        public Action<BG_BS2000, int, int, string,int> BSViewMvvmScanImagePathAction;
        #endregion
        DetecotrControllerManager detecotrControllerManager;
        WriteableBitmap Bigwbmp = null;

        bool BsValue = false;
    
        IntPtr _BackBuffer = IntPtr.Zero;
        int ImageHeight; int ImageWidth;

        WriteableBitmap Bigwbmp2 = null;
        IntPtr _BackBuffer2 = IntPtr.Zero;
        int ImageHeight2; int ImageWidth2;

        bool isFirstLoad = false;
        bool IsScollImage= false;

        bool Value = false;
        #endregion

        #region RaisePropertyChanged
        public BSPackage _BSPackage;
        public BSPackage BSPackage
        {
            get => _BSPackage;
            set 
            {
                _BSPackage = value;
                ViewCount = Convert.ToInt32(_BSPackage.Bs2000.ViewCount);
                //_BSPackage.StartScanEvent -= _BSPackage_StartScanEvent;
                //_BSPackage.StopScanEvent -= _BSPackage_StopScanEvent;
                //_BSPackage.StartScanEvent += _BSPackage_StartScanEvent;
                //_BSPackage.StopScanEvent += _BSPackage_StopScanEvent;
                DetectorConnectionStatus.LabelText = $"{_BSPackage.Bs2000.IPAddress}:{_BSPackage.Bs2000.CommandPort} {_BSPackage.Bs2000.DataPort}";
            }
        }

        private int _ViewCount = 1;
        public int ViewCount
        {
            get => _ViewCount;
            set { _ViewCount = value;RaisePropertyChanged(); }
        }

        public HardwareState _DetectorConnection = new HardwareState();
        public HardwareState DetectorConnectionStatus
        {
            get => _DetectorConnection;
            set
            {
                _DetectorConnection = value;
                RaisePropertyChanged("DetectorConnectionStatus");
            }
        }
        private string _IsConnected = $@"{UpdateStatusNameAction("Connecte")}";
        public string IsConnected
        {
            get { return _IsConnected; }
            set
            {
                _IsConnected = value;
                RaisePropertyChanged();
            }
        }
        
        private ImageSource _BSViewImageSource;
        public ImageSource BSViewImageSource
        {
            get { return _BSViewImageSource; }
            set
            {
                _BSViewImageSource = value;
                RaisePropertyChanged();
            }
        }

        private ImageSource _BSViewImageSource2;
        public ImageSource BSViewImageSource2
        {
            get { return _BSViewImageSource2; }
            set
            {
                _BSViewImageSource2 = value;
                RaisePropertyChanged();
            }
        }
        private string _DetectorIpAddress = "127.0.0.1";
        /// <summary>
        /// 探测器IP地址
        /// </summary>
        public string DetectorIpAddress
        {
            get { return _DetectorIpAddress; }
            set
            {
                _DetectorIpAddress = value;
                DetectorConnectionStatus.LabelText = $"{DetectorIpAddress}";
                RaisePropertyChanged();
            }
        }
        private short _DetectorCommandPort = 4002;
        /// <summary>
        /// 探测器指令端口
        /// </summary>
        public short DetectorCommandPort
        {
            get { return _DetectorCommandPort; }
            set
            {
                _DetectorCommandPort = value;
                RaisePropertyChanged();
            }
        }
        private short _DetectorImagePort = 3002;
        /// <summary>
        /// 探测器图像端口
        /// </summary>
        public short DetectorImagePort
        {
            get { return _DetectorImagePort; }
            set
            {
                _DetectorImagePort = value;
                RaisePropertyChanged();
            }
        }

        private Visibility _IsTestMode = Visibility.Visible;
        /// <summary>
        /// 是否是测试模式
        /// </summary>
        public Visibility IsTestMode
        {
            get { return _IsTestMode; }
            set
            {
                _IsTestMode = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Command
        /// <summary>
        /// 连接探测器
        /// </summary>
        public RelayCommand ConnectionCommand { get; set; }
        /// <summary>
        /// 断开探测器连接
        /// </summary>
        public RelayCommand DisConnectionCommand { get; set; }
        
        /// <summary>
        /// 开始扫描
        /// </summary>
        public RelayCommand ScanCommand { get; set; }

        /// <summary>
        /// 停止扫描
        /// </summary>
        public RelayCommand DisScanCommand { get; set; }
        
        /// <summary>
        /// 校正
        /// </summary>
        public RelayCommand CorrectCommand { get; set; }

        /// <summary>
        /// 保存Png
        /// </summary>
        public RelayCommand SavePngCommand { get; set; }
        
        #endregion

        public BSViewViewModel() 
        {
           
            CreateCommands();
        }

        public void Start()
        {


            IsTestMode =ConfigServices.GetInstance().localConfigModel.IsDetectorTestMode?Visibility.Visible:Visibility.Collapsed;

            if (detecotrControllerManager == null)
            {
                detecotrControllerManager = new DetecotrControllerManager();
            }
            ScrollImageServices.Service.ScrollImageEvent += StartScrollImage;
            detecotrControllerManager.ReflashImageAction += ReflashPreviewImage;
            detecotrControllerManager.Load(DetectorEquipmenntEnum.BSBegoodDetector, DetectorIpAddress, DetectorCommandPort.ToString(), DetectorImagePort.ToString());
            detecotrControllerManager.detectorEquipment.ScanImagePathAction += BSImageScanCallback;
            CommonDeleget.ImageXStartAndStopCallbackEvent += CommonDeleget_ImageXStartAndStopCallbackEvent;
            CommonDeleget.ImageXSetDirectionEvent += CommonDeleget_ImageXSetDirectionEvent;
            CommonDeleget.ImageXSetModeEvent += CommonDeleget_ImageXSetModeEvent;
            CommonDeleget.ImageXSetSpeedEvent -= CommonDeleget_ImageXSetSpeedEvent;
            CommonDeleget.ImageXSetSpeedEvent += CommonDeleget_ImageXSetSpeedEvent;
            detecotrControllerManager.detectorEquipment.Connection(DetectorIpAddress, DetectorCommandPort, DetectorImagePort);
            detecotrControllerManager.DetecotorConnectionAction += DetectorConnection;

            Task.Run(() => {
                while (true)
                {
                    Thread.Sleep(123);
                    CMWReady();
                }
            });
            
        }

        private void CommonDeleget_ImageXSetSpeedEvent(int Speed)
        {
            Log.GetDistance().WriteInfoLogs($@"detecotrControllerManager.IsConnection() :{Speed}");
            if (detecotrControllerManager.IsConnection())
            {
                Log.GetDistance().WriteInfoLogs($@"detecotrControllerManager?.SX_SetSpeed(Speed) :{Speed}");
                detecotrControllerManager?.SX_SetSpeed(Speed);
            }
        }

        private void CMWReady()
        {
            bool result = PlcManager.WritePositionValue(MouduleCommandDic[Command.StationReady], Value);
            Value = !Value;
        }
        private void CommonDeleget_ImageXSetModeEvent(int Mode)
        {
            if(detecotrControllerManager.IsConnection()) detecotrControllerManager?.SX_SetMode(Mode);
        }

        private void CommonDeleget_ImageXSetDirectionEvent(int Direction)
        {
            if (detecotrControllerManager.IsConnection()) detecotrControllerManager?.SX_SetDirection(Direction);
        }

        private void CommonDeleget_ImageXStartAndStopCallbackEvent(int Result)
        {
            //await Task.Run(() => {
                if (detecotrControllerManager.IsConnection())
                {
                    if (Result > 0)
                    {
                        detecotrControllerManager?.SX_Start();
                        ViewCount = Convert.ToInt32(BSPackage.Bs2000.ViewCount);
                    }
                    else
                    {
                        detecotrControllerManager?.SX_Stop();
                    }
                }
            //});
        }

        public void Stop()
        {
            /*
            ScrollImageServices.Service.ScrollImageEvent -= StartScrollImage;
            detecotrControllerManager.ReflashImageAction -= ReflashPreviewImage;
            detecotrControllerManager.DetecotorConnectionAction -= DetectorConnection;
            detecotrControllerManager.detectorEquipment.ScanImagePathAction -= BSImageScanCallback;
            detecotrControllerManager.DisConnection();
            SX_Destroy(detecotrControllerManager.detectorEquipment.DetectorIntPtr);
            */
            // 1. 注销 ScrollImage 事件
            ScrollImageServices.Service.ScrollImageEvent -= StartScrollImage;

            // 2. 注销 DetecotrControllerManager 事件
            detecotrControllerManager.ReflashImageAction -= ReflashPreviewImage;
            detecotrControllerManager.DetecotorConnectionAction -= DetectorConnection;
            detecotrControllerManager.detectorEquipment.ScanImagePathAction -= BSImageScanCallback;

            // 3. 补全：注销 Start() 中订阅的全局 CommonDeleget 事件
            CommonDeleget.ImageXStartAndStopCallbackEvent -= CommonDeleget_ImageXStartAndStopCallbackEvent;
            CommonDeleget.ImageXSetDirectionEvent -= CommonDeleget_ImageXSetDirectionEvent;
            CommonDeleget.ImageXSetModeEvent -= CommonDeleget_ImageXSetModeEvent;
            CommonDeleget.ImageXSetSpeedEvent -= CommonDeleget_ImageXSetSpeedEvent;

            // 4. 断开连接
            detecotrControllerManager.DisConnection();

            // 5. 销毁句柄（DetectorIntPtr 在 DisConnection 中已置零，此处传零安全）
            SX_Destroy(detecotrControllerManager.detectorEquipment.DetectorIntPtr);

        }

        public void OnRenderSizeChanged(System.Windows.SizeChangedInfo sizeInfo,Grid ParentContent)
        {
            if (!isFirstLoad)
            {
                int ViewCount = Convert.ToInt32(BSPackage.Bs2000.ViewCount);
                var actualHeight = ViewCount>=2? Convert.ToInt32(sizeInfo.NewSize.Height)/2: Convert.ToInt32(sizeInfo.NewSize.Height);
                ImageHeight = actualHeight < 256 ? 256 : actualHeight;
                ImageWidth = Convert.ToInt32(sizeInfo.NewSize.Width);
                Bigwbmp = new WriteableBitmap(ImageWidth , ImageHeight, 96, 96, PixelFormats.Bgr32, null);
                memset(Bigwbmp.BackBuffer, 205, Bigwbmp.BackBufferStride * ImageHeight);
                Bigwbmp.Lock();
                Bigwbmp.AddDirtyRect(new System.Windows.Int32Rect(0, 0, Convert.ToInt32(Bigwbmp.Width), actualHeight));
                Bigwbmp.Unlock();

                _BackBuffer = Bigwbmp.BackBuffer;
                BSViewImageSource = Bigwbmp;


                if(ViewCount >= 2)
                {
                    ImageHeight2 = actualHeight < 256 ? 256 : actualHeight;
                    ImageWidth2 = Convert.ToInt32(sizeInfo.NewSize.Width);
                    Bigwbmp2 = new WriteableBitmap(ImageWidth2, ImageHeight2, 96, 96, PixelFormats.Bgr32, null);
                    memset(Bigwbmp2.BackBuffer, 110, Bigwbmp2.BackBufferStride * ImageHeight2);
                    Bigwbmp2.Lock();
                    Bigwbmp2.AddDirtyRect(new System.Windows.Int32Rect(0, 0, Convert.ToInt32(Bigwbmp2.Width), actualHeight));
                    Bigwbmp2.Unlock();

                    _BackBuffer2 = Bigwbmp2.BackBuffer;
                    BSViewImageSource2 = Bigwbmp2;
                }
            }
            isFirstLoad = true;
        }
        #region 图像刷新

        /// <summary>
        /// 外部信号开始扫描
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _BSPackage_StopScanEvent(object sender, bool e)
        {

        }
        /// <summary>
        /// 外部信号停止扫描
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _BSPackage_StartScanEvent(object sender, bool e)
        {

        }
        /// <summary>
        /// 开始卷图
        /// </summary>
        /// <param name="IsScrollImage"></param>
        public void StartScrollImage(bool _IsScrollImage)
        {
            Log.GetDistance().WriteInfoLogs("ganggang clicked start scroll image...");
            IsScollImage = _IsScrollImage;
            //ImageImportDll.SX_Start(detecotrControllerManager.detectorEquipment.DetectorIntPtr);
            if (IsScollImage)
            {
                Task.Run(() =>
                {
                    while (true)
                    {
                        if (!IsScollImage) break;
                        Thread.Sleep(20);
                        Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            Bigwbmp.Lock();
                            Bigwbmp.AddDirtyRect(new System.Windows.Int32Rect(0, 0, Convert.ToInt32(Bigwbmp.Width), Convert.ToInt32(Bigwbmp.Height)));
                            Bigwbmp.Unlock();
                        }));
                        if (Convert.ToInt32(BSPackage.Bs2000.ViewCount) >= 2)   
                        {
                            Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                Bigwbmp2.Lock();
                                Bigwbmp2.AddDirtyRect(new System.Windows.Int32Rect(0, 0, Convert.ToInt32(Bigwbmp2.Width), Convert.ToInt32(Bigwbmp2.Height)));
                                Bigwbmp2.Unlock();
                            }));
                        }
                    }
                });
            
            }
        }
        #endregion

        #region 状态回调
        /// <summary>
        /// 刷新
        /// </summary>
        public void ReflashPreviewImage()
        {
            
            try
            {
                //Log.GetDistance().WriteInfoLogs("ganggang ReflashPreviewImage called...");
                SX_DrawBitmap(detecotrControllerManager.detectorEquipment.DetectorIntPtr, _BackBuffer, ImageWidth, ImageHeight);
                if (Convert.ToInt16(BSPackage.Bs2000.ViewCount) >= 2)
                {
                    SX_DrawBitmap2(detecotrControllerManager.detectorEquipment.DetectorIntPtr, _BackBuffer2, ImageWidth2, ImageHeight2);
                }
            }
            catch (Exception ex)
            {
                WriteLogAction($"{ex.StackTrace}", LogType.ImageImportDllError);
            }
            

            /*
            Application.Current?.Dispatcher.Invoke(() =>
            {
                Bigwbmp.Lock();
                SX_DrawBitmap(detecotrControllerManager.detectorEquipment.DetectorIntPtr,
                              _BackBuffer,
                              ImageWidth,
                              ImageHeight);

                Bigwbmp.AddDirtyRect(
                    new Int32Rect(0, 0, ImageWidth, ImageHeight));

                Bigwbmp.Unlock();
            });
            */
        }

        private void DetectorConnection(int Status)
        {

            /*if (Status == 0)
            {
                if (IsTestMode != Visibility.Visible)
                {
                    Task.Run(() =>
                    {
                        Thread.Sleep(2000);
                        detecotrControllerManager.Connection(DetectorIpAddress, DetectorCommandPort, DetectorImagePort);
                        DetectorConnectionStatus.LabelText = $"{DetectorIpAddress}-{UpdateStatusNameAction("Connecting")}";
                    });
                }
                IsConnected = $@"{UpdateStatusNameAction("Connecte")}";
                return;
            }
            else
            {
                IsConnected = $@"{UpdateStatusNameAction("UnConnected")}";
            }
            if (Status == 3)
            {
                DetectorConnectionStatus.LabelText = $"{_BSPackage.Bs2000.IPAddress}:{_BSPackage.Bs2000.CommandPort} {_BSPackage.Bs2000.DataPort} {_BSPackage.Bs2000.Remark} - {UpdateStatusNameAction("Connected")}";
                DetectorConnectionStatus.LabmForecolor = ForeColorKey.GreenPoliceLight;
            }
            else
            {
                DetectorConnectionStatus.LabelText = $"{_BSPackage.Bs2000.IPAddress}:{_BSPackage.Bs2000.CommandPort} {_BSPackage.Bs2000.DataPort} {_BSPackage.Bs2000.Remark} - {UpdateStatusNameAction("Connecting")}";
                DetectorConnectionStatus.LabmForecolor = ForeColorKey.RedPoliceLight;
            }*/

            
            if (Status == 0)
            {
                if (IsTestMode != Visibility.Visible)
                {
                    Task.Run(() =>
                    {
                        Thread.Sleep(2000);
                        detecotrControllerManager.Connection(DetectorIpAddress, DetectorCommandPort, DetectorImagePort);
                        DetectorConnectionStatus.LabelText = $"{DetectorIpAddress}-{UpdateStatusNameAction("Connecting")}";
                    });
                }
                IsConnected = $@"{UpdateStatusNameAction("Connecte")}";
                return;
            }
            else
            {
                IsConnected = $@"{UpdateStatusNameAction("UnConnected")}";
            }
            Application.Current.Dispatcher.InvokeAsync(() => { 
                if (Status == 3)
                {
                    DetectorConnectionStatus.LabelText = $"{_BSPackage.Bs2000.IPAddress}:{_BSPackage.Bs2000.CommandPort} {_BSPackage.Bs2000.DataPort} {_BSPackage.Bs2000.Remark} - {UpdateStatusNameAction("Connected")}";
                    DetectorConnectionStatus.LabmForecolor = ForeColorKey.GreenPoliceLight;
                }
                else
                {
                    DetectorConnectionStatus.LabelText = $"{_BSPackage.Bs2000.IPAddress}:{_BSPackage.Bs2000.CommandPort} {_BSPackage.Bs2000.DataPort} {_BSPackage.Bs2000.Remark} - {UpdateStatusNameAction("Connecting")}";
                    DetectorConnectionStatus.LabmForecolor = ForeColorKey.RedPoliceLight;
                }
            });
        
        }

        private void BSImageScanCallback(int status, int carrid, string path)
        {
            Log.GetDistance().WriteInfoLogs($@"status:{status},path:{path}");
            if (ViewCount > 0)
            {
                Log.GetDistance().WriteInfoLogs($@"BSViewMvvmScanImagePathAction,path:{path},ViewCount:{ViewCount}");
                BSViewMvvmScanImagePathAction?.Invoke(this.BSPackage.Bs2000, status, carrid, path, ViewCount);
                ViewCount--;
            }
            //if(ViewCount == 0)
            //{
            //    ViewCount = Convert.ToInt32(BSPackage.Bs2000.ViewCount);
            //}
        }
        #endregion


        private void CreateCommands()
        {
            ConnectionCommand = new RelayCommand(ConnectionDetector);
            ScanCommand = new RelayCommand(ScanDetector);
            DisScanCommand = new RelayCommand(DisScanDetector);
            CorrectCommand = new RelayCommand(CorrectDetector);
            SavePngCommand = new RelayCommand(SavePngExcute);
        }

        #region 手动测试
        private void ConnectionDetector()
        {
            if (IsConnected == $@"{UpdateStatusNameAction("Connecte")}")
            {
                if (!string.IsNullOrEmpty(DetectorIpAddress))
                {
                    detecotrControllerManager.Connection(DetectorIpAddress, DetectorCommandPort, DetectorImagePort);
                }
            }
            else
            {
                DisScanDetector();
                SX_Disconnect(detecotrControllerManager.detectorEquipment.DetectorIntPtr);
                DetectorConnectionStatus.LabelText = $"{_BSPackage.Bs2000.IPAddress}:{_BSPackage.Bs2000.CommandPort} {_BSPackage.Bs2000.DataPort} {_BSPackage.Bs2000.Remark} - {UpdateStatusNameAction("Connecting")}";
                DetectorConnectionStatus.LabmForecolor = ForeColorKey.RedPoliceLight;
            }
        }

        bool tmpValue = false;
        private void ScanDetector()
        {
            if (!IsScollImage)
            {
                IsScollImage = true;
                StartScrollImage(true);
            }

            if (detecotrControllerManager.DetectorConnection == 3)
            {
                ImageImportDll.SX_SetMode(detecotrControllerManager.detectorEquipment.DetectorIntPtr, 0);
                SX_SetDirection(intPtr, PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.ScanForWard) ? 0 : 1);
                SX_Start(detecotrControllerManager.detectorEquipment.DetectorIntPtr);
                SX_SetDark(detecotrControllerManager.detectorEquipment.DetectorIntPtr);
            }
        }
        private void DisScanDetector()
        {
            ScrollImageServices.Service.IsScrollImage = false;
            if (detecotrControllerManager.DetectorConnection == 3)
            {
                SX_Stop(detecotrControllerManager.detectorEquipment.DetectorIntPtr);
            }
        }
        private void CorrectDetector()
        {
            SX_SetLight(detecotrControllerManager.detectorEquipment.DetectorIntPtr);
        }
        private void SavePngExcute()
        {
            Bigwbmp.WriteableBitmapToPng();
        }
        
        #endregion
    }
}
