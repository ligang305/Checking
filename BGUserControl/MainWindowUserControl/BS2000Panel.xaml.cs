using BG_Entities;
using BG_Services;
using BG_WorkFlow;
using BGModel;
using BGUserControl.MainWindowUserControl.ChildrenControl.BS.BS2000;
using CMW.Common.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static BGUserControl.DiyUploadListView;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.Common;
using BGLogs;


namespace BGUserControl.MainWindowUserControl
{
    /// <summary>
    /// BS2000Panel.xaml 的交互逻辑
    /// </summary>
    public partial class BS2000Panel : UserControl
    {
        List<BG_BS2000> BS_ViewList = new List<BG_BS2000>();
        BS2000PanelViewModel bS2000PanelViewModel = new BS2000PanelViewModel();
        public BS2000Panel()
        {
            InitializeComponent();
            DataContext = bS2000PanelViewModel;
            Start();
            if(Common.controlVersion != ControlVersion.BS & ConfigServices.GetInstance().localConfigModel.IsUserBS)
            {
                BSPlcService.GetInstance().ConnectionAction -= ConnectionStatus;
                BSPlcService.GetInstance().ConnectionAction += ConnectionStatus;
            }
            else
            {

                PlcService.GetInstance().ConnectionAction -= ConnectionStatus;
                PlcService.GetInstance().ConnectionAction += ConnectionStatus;
            }
        }

        protected void ConnectionStatus(object plcServices, bool ConnectStatus)
        {
            if (ConnectStatus)
            {
                DynamicGetFreez();
            }
        }

        private void Start()
        {
            BSPackageSubmitServices.Service.Start();
            InitView();
        }

        private void InitView()
        {
            BS_ViewList = BS2000BLL.GetInstance().GetBSModel();
            /*
            switch (controlVersion)
            {
                case ControlVersion.BGV6000BS:
                    BS_ViewList = BGV6000BSBLL.GetInstance().GetBSModel();
                    break;
                default:
                    BS_ViewList = BS2000BLL.GetInstance().GetBSModel();
                    break;
            }
            */
            
            BSPackageSubmitServices.Service.bSPackages.Clear();

            foreach (var bsViewItem in BS_ViewList)
            {
                BS2000.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(Convert.ToInt32(bsViewItem.ViewCount), GridUnitType.Star) });
                BSView bSView = new BSView();
                Grid.SetRow(bSView, BS_ViewList.IndexOf(bsViewItem) + 1);
                BS2000.Children.Add(bSView);
                bSView.DetectorIpAddress = bsViewItem.IPAddress;
                bSView.DetectorCommandPort = bsViewItem.CommandPort;
                bSView.DetectorImagePort = bsViewItem.DataPort;
                bSView.BSViewScanImagePathAction += BSImageScanCallback;
                BSPackage bSPackage = new BSPackage { Bs2000 = bsViewItem };
                BSPackageSubmitServices.Service.bSPackages.Add(bSPackage);
                bSView.Start(bSPackage);
            }
            
            if(Common.controlVersion != ControlVersion.BS)
            {
                BS2000.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                ViewScanImage scanImage = new ViewScanImage();
                BS2000.Children.Add(scanImage);
                Grid.SetRow(scanImage, BS_ViewList.Count+1);
            }
            
        }


        /// <summary>
        /// 这个回调判断是否各个视角的BS图片都收集齐了，如果收集齐了，就直接进行数据合并并且将数据上传
        /// </summary>
        /// <param name="bG_BS2000"></param>
        /// <param name="status"></param>
        /// <param name="carrid"></param>
        /// <param name="path"></param>
        private void BSImageScanCallback(BG_BS2000 bG_BS2000, int status, int carrid, string path, int ViewIndex)
        {
            bS2000PanelViewModel.ReceiveBS2000Image(bG_BS2000, status, carrid, path, ViewIndex);
        }

        DateTime lastDynamicTime = DateTime.Now;
        /// <summary>
        /// 动态获取频率 BS设备无需调频，只需要读取速度进行拉伸
        /// </summary>
        /// <param name="item"></param> 
        private void DynamicGetFreez()
        {
            Task.Run(() =>
            {
                try
                {
                    if (EquipmentManager.GetInstance().PlcManager.GetStatusByPositionEnum(PLCPositionEnum.DYNAMIC))
                    {
                        if ((DateTime.Now - lastDynamicTime).TotalSeconds > 10)
                        {
                            Log.GetDistance().WriteInfoLogs($@"DynamicGetFreez :{EquipmentManager.GetInstance().PlcManager.GetStatusByPositionEnum(PLCPositionEnum.DYNAMIC)}");
                            lastDynamicTime = DateTime.Now;
                            ushort carspeed = EquipmentManager.GetInstance().PlcManager.GetStatusByPositionEnum(PLCPositionEnum.ScanMode) ?
                            (ushort)(EquipmentManager.GetInstance().BSGlobalDoseStatus[0] * 3.6) : EquipmentManager.GetInstance().BSGlobalDoseStatus[0];
                            Log.GetDistance().WriteInfoLogs($@"DynamicGetFreez :{carspeed}");
                            CommonDeleget.ImageXSetSpeed(carspeed);
                        }
                    }
                }
                catch (Exception e)
                {
                    CommonDeleget.WriteLogAction(e.StackTrace, LogType.ApplicationError, true);
                }
            });
        }
    }

    public class BS2000PanelViewModel : BaseMvvm
    {
        public HardwareState _plcConnection = new HardwareState();
        public HardwareState PlcConnection
        {
            get => _plcConnection;
            set
            {
                _plcConnection = value;
                RaisePropertyChanged("PlcConnection");
            }
        }
        public HardwareState _BSImageUploadStatus = new HardwareState();
        public HardwareState BSImageUploadStatus
        {
            get => _BSImageUploadStatus;
            set
            {
                _BSImageUploadStatus = value;
                RaisePropertyChanged("BSImageUploadStatus");
            }
        }

        public BS2000PanelViewModel()
        {
            if (ConfigServices.GetInstance().localConfigModel.IsUserBS && Common.controlVersion != ControlVersion.BS)
            {
                LoadUIText();
                BSPlcService.GetInstance().ConnectionAction += BS2000PanelViewModel_ConnectionAction;
                PlcConnection.IsShow = Visibility.Visible;
            }
        }
        public override void LoadUIText()
        {
            PlcConnection.LabelText = UpdateStatusNameAction("ConnectionWithBSPlc");
        }
        private void BS2000PanelViewModel_ConnectionAction(object sender, bool ConnectionStatus)
        {
            PlcConnection.LabmForecolor = ConnectionStatus ? ForeColorKey.GreenPoliceLight : ForeColorKey.RedPoliceLight;
        }

        internal void ReceiveBS2000Image(BG_BS2000 bG_BS2000, int status, int carrid, string path, int ViewIndex = 1)
        {
            BSPackageSubmitServices.Service.BSPackagesReceiveActionQueue.Enqueue(new Action(() =>
            {
                if (File.Exists(path))
                {
                    BSPackage bsPackage = BSPackageSubmitServices.Service.bSPackages.Find(q => q.Bs2000.ID == bG_BS2000.ID);
                    if (!bsPackage.IsReady)
                    {
                        bsPackage.BsImagePath.Add(path);
                        if (bsPackage.BsImagePath.Count == Convert.ToInt32(bsPackage.Bs2000.ViewCount))
                        {
                            bsPackage.IsReady = true;
                        }
                        bsPackage.CurretScanTime = DateTime.Now;
                    }

                    ///如果设置完了，就绪的图片只有一张的话
                    ///之前是如果有且只有一个视角的一张图ready的话才启动线程，现在只要某个视角接到了图就启动线程
                    if (BSPackageSubmitServices.Service.bSPackages.Count(q => q.BsImagePath.Count()!=0) == 1)
                    {
                        //TODO 启动超时线程
                        BSPackageSubmitServices.Service.isStartOverTime = true;
                        BSPackageSubmitServices.Service.BSOverTimeThread();
                    }
                }
            }));
        }
    }

    public class BSPackageSubmitServices
    {
        /// <summary>
        /// 单实例服务
        /// </summary>
        public static BSPackageSubmitServices Service { get; private set; }

        static BSPackageSubmitServices()
        {
            Service = new BSPackageSubmitServices();
        }

        #region Scan
        ScanHelper ScanHelper = new ScanHelper();
        #endregion

        #region 用于上传数据用的参数
        private bool _PackageIsReady = false;
        public bool PackageIsReady
        {
            get
            {
                _PackageIsReady = bSPackages.Count(q => q.IsReady) == bSPackages.Count;
                return _PackageIsReady;
            }
            set
            {
                _PackageIsReady = value;
            }
        }

        public bool isStartOverTime = false;
        /// <summary>
        /// 任务队列
        /// </summary>
        public Stack<RecvMessage> BSTaskIdQueue = new Stack<RecvMessage>();
        RecvMessage CurrentScanRecvessage = null;
        public List<BSPackage> bSPackages = new List<BSPackage>();
        BlockingCollection<List<BSPackage>> BSPackagesQueue = new BlockingCollection<List<BSPackage>>();
        #endregion

        #region 用于获取数据插入到上传数据队列中的参数
        bool IsStart = true;
        public ConcurrentQueue<Action> BSPackagesReceiveActionQueue = new ConcurrentQueue<Action>();
        #endregion

        #region 当前扫描的触发事件 开始扫描|| 停止扫描 Event
        public event EventHandler<bool> StartScanEvent;
        public event EventHandler<bool> StopScanEvent;
        #endregion

        public void Start()
        {
            //BegoodServerController.GetInstance().TaskArriveAction += TaskArrive;
            IsStart = true;
            BSSubmitImageThread();
            BSReceiveImageThread();
            CMWReadyThread();
        }
        private void CMWReadyThread()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(100);
                    if (IsModuleOpen)
                    {
                        EquipmentManager.GetInstance().PlcManager.WritePositionValue(EquipmentManager.GetInstance().MouduleCommandDic[Command.AutoMode],false);
                    }
                    else
                    {
                        EquipmentManager.GetInstance().PlcManager.WritePositionValue(EquipmentManager.GetInstance().MouduleCommandDic[Command.AutoMode], true);
                    }
                }
            });
        }
        public void Stop()
        {
            IsStart = false;
        }

        /// <summary>
        /// 超时提交线程
        /// </summary>
        public void BSOverTimeThread()
        {
            if (!isStartOverTime) { bSPackages.ForEach(q => q.IsReady = false); bSPackages.ForEach(q => q.BsImagePath.Clear()); return; }
            Task.Run(() =>
            {
                DateTime dateTime = DateTime.Now;
                while (isStartOverTime)
                {
                    if ((DateTime.Now - dateTime).TotalSeconds > 10)
                    {
                        foreach (var bsItem in bSPackages)
                        {
                            if (!bsItem.IsReady)
                            {
                                bsItem.IsReady = true;
                                bsItem.IsOverTime = true;
                                bsItem.CurretScanTime = DateTime.Now;
                            }
                        }
                    }
                    //如果将数据提交了，就将超时标志设为false
                    if (PackageIsReady)
                    {
                        //通过json
                        string backageStr = CommonFunc.ObjectToJson<List<BSPackage>>(bSPackages);
                        List<BSPackage> _bSPackages = CommonFunc.JsonToObject<List<BSPackage>>(backageStr);
                        //将数据添加至上传队列中
                        BSPackagesQueue.Add(_bSPackages);
                        bSPackages.ForEach(q => q.IsReady = false);
                        bSPackages.ForEach(q => q.BsImagePath.Clear());
                        isStartOverTime = false;
                    }
                    Thread.Sleep(10);
                }
            });
        }
        /// <summary>
        /// 提交BS图片
        /// </summary>
        /// <returns></returns>
        public bool SubmitBSImage(List<BSPackage> _bSPackages)
        {
            try
            {
                foreach (var bsPackageItem in _bSPackages)
                {
                    List<string> newFilePaths = new List<string>();
                    foreach (var itemPath in bsPackageItem.BsImagePath)
                    {
                        if (File.Exists(itemPath))
                        {
                            //TODO 由于多视角背散 图 所产生的时分秒一致，从而导致文件名一致，为了上传至服务器时 不被互相覆盖，在这里将各视角背散图重新命名
                            //命名规则为 BS序号_IP_原名称
                            string OriginFileName = System.IO.Path.GetFileName(itemPath);
                            string NewFileName = $@"BS{_bSPackages.IndexOf(bsPackageItem) + 1}_{bsPackageItem.Bs2000.IPAddress.Replace('.', '_').ToString()}_{OriginFileName}";
                            string NewFileFullName = System.IO.Path.Combine($@"{System.IO.Path.GetDirectoryName(itemPath)}", NewFileName);
                            File.Move(itemPath, NewFileFullName);
                            File.Delete(itemPath);
                            newFilePaths.Add(NewFileFullName);
                        }
                    }
                    bsPackageItem.BsImagePath = newFilePaths;
                }
                var ManyViewList = _bSPackages.Where(q => Convert.ToInt32(q.Bs2000.ViewCount) >= 2).ToList();
                /// 交换图片合并好看
                if (ManyViewList.Count() == 2)
                {
                    try
                    {
                        if (ManyViewList[0].BsImagePath.Count >= 2 && ManyViewList[1].BsImagePath.Count >= 2)
                        {
                            string TempStr = ManyViewList[1].BsImagePath[1];
                            ManyViewList[1].BsImagePath[1] = ManyViewList[0].BsImagePath[1];
                            ManyViewList[0].BsImagePath[1] = TempStr;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.GetDistance().WriteErrorLogs(ex);
                    }
                }
                List<string> RawFilePath = _bSPackages.SelectMany(q => q.BsImagePath).ToList();
                string StartFilePath = RawFilePath[0];
                List<string> TempStartFile = new List<string>();
                foreach (var BSPath in RawFilePath)
                {
                    if (RawFilePath.IndexOf(BSPath) == 0)
                    {
                        continue;
                    }
                    StartFilePath = MergeScanImage(StartFilePath, BSPath, ref TempStartFile);
                    Thread.Sleep(1000);//防止前后两张图片合并时间精确到秒之后一致
                                       //foreach (var BSPath in bsPackageItem.BsImagePath)
                                       //{

                    //}
                }
                ///删除中间合并文件，最终上传的合并文件通过全局是否保留文件参数去控制是否需要删除
                foreach (var tempFilePathItem in TempStartFile)
                {
                    if (tempFilePathItem == StartFilePath) continue;
                    if (File.Exists(tempFilePathItem))
                    {
                        File.Delete(tempFilePathItem);
                    }
                }
                BGLogs.Log.GetDistance().WriteInfoLogs($"BSTASK--------MERGE IMAGE PATH: {StartFilePath}");
                //TODO 如果合并
                if (File.Exists(StartFilePath))
                {
                    Task.Run(() =>
                    {
                        UploadLocalImageToOnlineForBS<LocalUploadImage>.GetInstance().UpLoadImageOnline(_bSPackages.SelectMany(p => p.BsImagePath).ToList(), StartFilePath, ref CurrentScanRecvessage);
                        CurrentScanRecvessage = null;
                    }).Wait(1000);
                }
            }
            catch (Exception ex)
            {
                Log.GetDistance().WriteInfoLogs(ex.ToString());
            }
            return false;
        }

        public string MergeScanImage(string Path1, string Path2, ref List<string> TempStartFile)
        {
            if (string.IsNullOrEmpty(Path1) && !string.IsNullOrEmpty(Path2)) return Path2;
            if (!string.IsNullOrEmpty(Path1) && string.IsNullOrEmpty(Path2)) return Path1;
            if (string.IsNullOrEmpty(Path1) && string.IsNullOrEmpty(Path2)) return string.Empty;
            string newFilePath = $@"{SystemDirectoryConfig.GetInstance().GetBSScanImageFile()}\BS_{DateTime.Now.ToString("yyyyMMddHHmmss")}.raw";
            if(Path1 == Path2)
            {
                if (File.Exists(Path1))
                {
                    File.Copy(Path1, newFilePath);
                }
            }
            else 
            {
                int result = ImageImportDll.IX_FileMerge(Path1, Path2, newFilePath, 0);
            }
          
            TempStartFile.Add(newFilePath);
            return newFilePath;
        }

        /// <summary>
        /// 线程队列提交
        /// </summary>
        public void BSSubmitImageThread()
        {
            Task.Run(() =>
            {
                List<BSPackage> BSPackageList;
                while (IsStart)
                {
                    Thread.Sleep(20);
                    List<BSPackage> bSPackages = BSPackagesQueue.Take();
                    SubmitBSImage(bSPackages);
                }
            });
        }
        /// <summary>
        /// 线程队列接收数据
        /// </summary>
        public void BSReceiveImageThread()
        {
            Task.Run(() =>
            {
                Action BSPackageAction;
                while (IsStart)
                {
                    Thread.Sleep(20);
                    while (BSPackagesReceiveActionQueue.TryDequeue(out BSPackageAction))
                    {
                        BSPackageAction?.Invoke();
                    }
                }
            });
        }

        public void TestBSMonitorSignal()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(2000);
                    ImageXStartAndStop(1);
                    Thread.Sleep(10000);
                    ImageXStartAndStop(0);
                }
            });
        }
        private void TaskArrive(RecvMessage recvMessage)
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                MessageBox.Show("GET TaskId 2");
            });
            BSTaskIdQueue.Push(recvMessage);
            CurrentScanRecvessage = BSTaskIdQueue.Pop();
            CommonDeleget.WriteLogAction($@"Current BS Get TaskID:{CurrentScanRecvessage?.Data}",LogType.NormalLog);
            BSTaskIdQueue.Clear();
        }
    }


    /// <summary>
    /// 包含背散设备扫描图像是否就绪
    /// </summary>
    public class BSPackage
    {
        private BG_BS2000 _Bs2000;
        private bool _IsReady = false;
        private List<string> _BsImagePath = new List<string>();
        private DateTime _CurretScanTime;
        private bool _IsOverTime;
        /// <summary>
        /// 包含一个背散连接对象
        /// </summary>
        public BG_BS2000 Bs2000
        {
            get { return _Bs2000; }
            set { _Bs2000 = value; }
        }

        /// <summary>
        /// 表示此次扫描是否完成
        /// </summary>
        public bool IsReady
        {
            get => _IsReady;
            set { _IsReady = value; }
        }
        /// <summary>
        /// 包裹扫描路径
        /// </summary>
        public List<string> BsImagePath
        {
            get => _BsImagePath;
            set { _BsImagePath = value; }
        }

        /// <summary>
        /// 最新的扫描完成时间，用时间判断是否收齐
        /// </summary>
        public DateTime CurretScanTime
        {
            get => _CurretScanTime;
            set { _CurretScanTime = value; }
        }

        /// <summary>
        /// 标识是否超时，如果超时，则Ready也要设置为false
        /// </summary>
        public bool IsOverTime
        {
            get => _IsOverTime;
            set
            {
                _IsOverTime = value;
            }
        }

    }
}
