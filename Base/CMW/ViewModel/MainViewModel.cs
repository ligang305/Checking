using BG_Entities;
using BG_WorkFlow;
using BGModel;
using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.KeyBoard;
using System.Windows;
using BGUserControl;
using ExeBaseModules;
using System.Linq;
using System.Collections;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using CMW.Common.Utilities;
using GalaSoft.MvvmLight.Command;
using BG_Services;

namespace CMW
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : BaseMvvm
    {
        App _App = (Application.Current as App);
        LogBLL logBLL = new LogBLL();
        CommandConfigBLL _CommandConfigBLL = new CommandConfigBLL(controlVersion);
        ControlVersionsBLL _ControlVersionsBLL = new ControlVersionsBLL();
        HookHandlerDelegate proc = new HookHandlerDelegate(HookCallback);
        IUserControlView ControlView;
        public ObservableCollection<StatusModel> HitchList = new ObservableCollection<StatusModel>();
        public Dictionary<string, ObservableCollection<object>> HitchModels;


        public ObservableCollection<StatusModel> BSHitchList = new ObservableCollection<StatusModel>();
        public Dictionary<string, ObservableCollection<object>> BSHitchModels;

        #region 控件
        public TopStatusMonitorPanel topStatusMonitorPanel;
        public CenterStatusControlPanel centerStatusControlPanel;
        #endregion

        #region 主页面读取插件的信息
        /// <summary>
        /// 获取右上角的状态栏
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<SignalModel> _StatusBarStatus;
        public ObservableCollection<SignalModel> StatusBarStatus
        {
            get => _StatusBarStatus;
            set
            {
                _StatusBarStatus = value;
                RaisePropertyChanged("StatusBarStatus");
            }
        }

        /// <summary>
        /// 获取急停面板信息
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, PLCPositionEnum> _EnergyStopPoints;
        public Dictionary<string, PLCPositionEnum> EnergyStopPoints
        {
            get => _EnergyStopPoints;
            set
            {
                _EnergyStopPoints = value;
                RaisePropertyChanged("EnergyStopPoints");
            }
        }

        /// <summary>
        /// 获取出束小图
        /// </summary>
        /// <returns></returns>
        private BitmapImage _RayImageSource;
        public BitmapImage RayImageSource
        {
            get => _RayImageSource;
            set
            {
                _RayImageSource = value;
                RaisePropertyChanged("RayImageSource");
            }
        }
        /// <summary>
        /// 获取出束的Potins,当出束的时候可以运行动画
        /// </summary>
        /// <returns></returns>
        private List<Tuple<Point, Point>> _RayPoints;
        public List<Tuple<Point, Point>> RayPoints
        {
            get => _RayPoints;
            set
            {
                _RayPoints = value;
                RaisePropertyChanged("RayPoints");
            }
        }

        private BitmapImage _MainImagePath;
        public BitmapImage MainImagePath
        {
            get => _MainImagePath;
            set
            {
                _MainImagePath = value;
                RaisePropertyChanged("MainImagePath");
            }
        }

        #endregion

        #region Commands

        public RelayCommand<KeyEventArgs> KeyDownCommand { get; set; }

        public RelayCommand<KeyEventArgs> PreviewKeyDownCommand { get; set; }

        public RelayCommand<KeyEventArgs> PreviewKeyUpCommand { get; set; }

        public RelayCommand<MouseButtonEventArgs> MouseDownEventCommand { get; set; }

        public RelayCommand LoadedEventCommand { get; set; }
        #endregion
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            //TopStatusMonitorPanel _topStatusMonitorPanel, CenterStatusControlPanel _centerStatusControlPanel
            //topStatusMonitorPanel = _topStatusMonitorPanel;
            //centerStatusControlPanel = _centerStatusControlPanel;
            CreateCommands();
          
        }

        private void CreateCommands()
        {
            KeyDownCommand = new RelayCommand<KeyEventArgs>(KeyDownCommandExecute);

            PreviewKeyUpCommand = new RelayCommand<KeyEventArgs>(KeyUpCommandExecute);

            LoadedEventCommand = new RelayCommand(LoadedEventCommandExecute);
        }
        #region Command Methods
        /// <summary>
        /// 用户按下按键时触发（主窗体的控件需要获取焦点才会响应 按键，所以按键接受统一放到主窗体）
        /// </summary>
        /// <param name="args"></param>
        private void KeyDownCommandExecute(KeyEventArgs args)
        {
        }
        /// <summary>
        /// 用户按下按键时触发（主窗体的控件需要获取焦点才会响应 按键，所以按键接受统一放到主窗体）
        /// </summary>
        /// <param name="args"></param>
        private void KeyUpCommandExecute(KeyEventArgs args)
        {
        }
        /// <summary>
        /// 窗口完全加载完毕后，开启系统自检等操作
        /// </summary>
        private void LoadedEventCommandExecute()
        {
            InitCommand();
            TransPosition(_App.CommandPlcList);
            InitContent();
            InitList();
            InitUserControlDataSource();
            StartCalibrationWindow();
        }
        #endregion
        /// <summary>
        /// 将命令和地址进行对应
        /// </summary>
        /// <param name="CommandPlcList"></param>
        public void TransPosition(List<CommandPlc> CommandPlcList)
        {
            CommandDic.Clear();
            foreach (var item in CommandPlcList)
            {
                if (!string.IsNullOrEmpty(item.PlcEnum))
                {
                    Command _cd = (Command)Enum.Parse(typeof(Command), item.PlcEnum, false);
                    if (!CommandDic.ContainsKey(_cd))
                    {
                        CommandDic.Add(_cd, item.PlcPosition);
                    }
                }

            }
        }
        /// <summary>
        /// 初始化命令
        /// </summary>
        private void InitCommand()
        {
            _App.ControlVersionList = _ControlVersionsBLL.GetControlVersionsBLLDataModel();
            _App.CommandPlcList = _CommandConfigBLL.GetCommandPlcDataModel(controlVersion);
        }
        private void InitContent()
        {
            try
            {
                string ModuleNames = _App.ControlVersionList.FirstOrDefault(q => q.ControlversionKey == ConfigServices.GetInstance().localConfigModel.CMW_Version)?.ControlVersionName;
                if (!string.IsNullOrEmpty(ModuleNames))
                {
                    string m_Plugins = "";
                    foreach (var item in SystemStartStopController.GetIns().Plugins)
                    {
                        m_Plugins += " " + item.Metadata.Name;
                        // Console.WriteLine(item.Metadata.Name);
                    }
                    var modules = SystemStartStopController.GetIns().Plugins.First((q => q.Metadata.Name == ModuleNames))?.Value;
                    if (modules == null) { BG_MESSAGEBOX.Show("提示", $"加载界面失败检查是否少了{ModuleNames}dll必要文件！"); return; }
                    ControlView = SystemStartStopController.GetIns().Plugins.First((q => q.Metadata.Name == ModuleNames)).Value as IUserControlView;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private void InitList()
        {
            HitchList.Clear();
            HitChModelBLL.GetInstance().GetHitchModelDataModel
              (SystemDirectoryConfig.GetInstance().GetHittingConfig(controlVersion)).Where(q => q.StatusOwner.Contains("status_Hitch_")).ToList().ForEach(q => HitchList.Add(q));

            HitchModels = HitChModelBLL.GetInstance().GetHitModelDic("MainPage", controlVersion);


            if (ConfigServices.GetInstance().localConfigModel.IsUserBS && controlVersion != ControlVersion.BS)
            {
                BSHitchList.Clear();
                HitChModelBLL.GetInstance().GetHitchModelDataModel
                  (SystemDirectoryConfig.GetInstance().GetHittingConfig(controlVersion)).Where(q => q.StatusOwner.Contains("status_Hitch_")).ToList().ForEach(q => HitchList.Add(q));
                BSHitchModels = HitChModelBLL.GetInstance().GetHitModelDic("MainPage", ControlVersion.BS);
            }
        }
        private void InitUserControlDataSource()
        {
            if (ControlView == null) return;
            StatusBarStatus = (ObservableCollection<SignalModel>)ControlView.GetStatusBarStatus();
            if(controlVersion != ControlVersion.BS && ConfigServices.GetInstance().localConfigModel.IsUserBS)
            {
                foreach (var item in GetStatusBarStatus())
                {
                    SignalModel BSItem = item as SignalModel;
                    BSItem.SignalSource = 1;
                    StatusBarStatus.Add(BSItem);
                }
            }
            EnergyStopPoints = ControlView.GetEnergyStopPanel();
            RayImageSource = ControlView.GetRayImage();
            RayPoints = ControlView.GetRayPointList();
            MainImagePath = ControlView.GetMainImage();
            topStatusMonitorPanel.DataSource = StatusBarStatus;
            centerStatusControlPanel.RayBackgroundImage = RayImageSource;

            centerStatusControlPanel.IsShowElectronFence = ConfigServices.GetInstance().localConfigModel.IsShowElctronicFence?Visibility.Visible:Visibility.Collapsed;
            centerStatusControlPanel.MainPageImageBackgroundImage = MainImagePath;
            centerStatusControlPanel.PointsDataSource = RayPoints;
            centerStatusControlPanel.DataSource = EnergyStopPoints;
        }
        private void StartCalibrationWindow()
        {
            //背散设备，betatron加速器不需要矫正，所以无需弹出矫正框
            if(controlVersion == ControlVersion.BS || controlVersion == ControlVersion.BGV5100)
            {
                return;
            }
            if ((DateTime.Now - ConfigServices.GetInstance().localConfigModel.LastCalibrationTime).TotalSeconds > 24 * 60 * 60)
            {
                Task.Factory.StartNew(() =>
                {
                    MessengerInstance.Send(new ShowMessageDialogWindowMessageAction(WindowKeys.MainWindowKey, WindowKeys.MessageDialogWindowKey,
                       UpdateStatusNameAction("Tip"), UpdateStatusNameAction("Calibration Tip"), MessageBoxButton.OKCancel,
                       new System.Action<DialogResult>((DialogResult _DialogResult) =>
                       {
                           if (_DialogResult == DialogResult.Ok)
                           {
                               // 启动系统自检窗口
                               MessengerInstance.Send(new OpenWindowMessage(WindowKeys.MainWindowKey,
                                  WindowKeys.CalibrationWindowKey, null));
                           }
                       })));
                });
            }
        }
        protected override void ConnectionStatus(bool ConnectStatus)
        {
            DateTime now = DateTime.Now;
            ButtonInvoke.InquirePlcStatusAction(GlobalRetStatus);
            Console.WriteLine($@"ConnectionAction?.Invoke(this, true):{(DateTime.Now - now).TotalMilliseconds} MainViewModel");
        }
        protected override void BSConnectionStatus(bool ConnectStatus)
        {
            BSInquirePlcStatus();
        }
        protected override void InquirePlcStatus(List<bool> StatusList)
        {
            try
            {
                if (HitchModels == null) return;
                foreach (var HitchItems in HitchModels)
                {
                    foreach (var item in HitchItems.Value)
                    {
                        StatusModel sm = item as StatusModel;
                        if (!IsConnection)
                        {
                            if (sm.StatusName.Contains("DetectorReady"))
                            {
                                //探测器需要去找扫描站的连接状态，如果判断到扫描站的状态是未连接
                                sm.StatusCode = IsScanCanScan() ? "1" : "0";
                                continue;
                            }
                            sm.StatusCode = "0";
                            continue;
                        }
                        int ItemIndex = Convert.ToInt32(sm.StatusIndex);
                        if (sm.StatusName.Contains("DetectorReady"))
                        {
                            //探测器需要去找扫描站的连接状态，如果判断到扫描站的状态是未连接
                            sm.StatusCode = IsScanCanScan() ? "1" : "0";
                            continue;
                        }
                        if (ItemIndex < GlobalRetStatus.Count)
                        {
                            if (HitchItems.Key.Equals("MainPageSafeSeriesLinkModule"))
                            {
                                sm.StatusCode = !GlobalRetStatus[ItemIndex] ? "1" : "0";
                            }
                            else
                            {
                                sm.StatusCode = GlobalRetStatus[ItemIndex] ? "1" : "0";
                            }
                        }
                        RefalshHitchItems(HitchItems);
                    }
                    RefalshHitchItems(HitchItems);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        // <summary>
        // 刷新状态项
        // </summary>
        // <param name="HitchItems"></param>
        // <returns></returns>
        private void RefalshHitchItems(KeyValuePair<string, ObservableCollection<object>> HitchItems)
        {
            if (HitchItems.Key.Contains("MainPageScanConditionModule"))
            {
                CommonDeleget.StateFeedbackEvent(HitchItems.Value.Count(q => (q as StatusModel).StatusCode == "0") == 0);
            }
        }

        /// <summary>
        /// 背散的PLC查询点位
        /// </summary>
        protected void BSInquirePlcStatus()
        {
            try
            {
                if (BSHitchModels == null) return;
                foreach (var HitchItems in BSHitchModels)
                {
                    foreach (var item in HitchItems.Value)
                    {
                        StatusModel sm = item as StatusModel;
                        if (!BSPLCControllerManager.GetInstance().IsConnect())
                        {
                            sm.StatusCode = "0";
                            continue;
                        }
                        int ItemIndex = Convert.ToInt32(sm.StatusIndex);
             
                        if (ItemIndex < EquipmentManager.GetInstance().BSGlobalRetStatus.Count)
                        {
                            if (HitchItems.Key.Equals("MainPageSafeSeriesLinkModule"))
                            {
                                sm.StatusCode = !EquipmentManager.GetInstance().BSGlobalRetStatus[ItemIndex] ? "1" : "0";
                            }
                            else
                            {
                                sm.StatusCode = EquipmentManager.GetInstance().BSGlobalRetStatus[ItemIndex] ? "1" : "0";
                            }
                        }
                        RefalshBSHitchItems(HitchItems);
                    }
                    RefalshBSHitchItems(HitchItems);
                }
            }
            catch (Exception) // what is of the election
            {

                throw;
            }
        }
        /// <summary>
        /// 刷新背散状态项
        /// </summary>
        /// <param name="HitchItems"></param>
        /// <returns></returns>
        private void RefalshBSHitchItems(KeyValuePair<string, ObservableCollection<object>> HitchItems)
        {
            if (HitchItems.Key.Equals("BSMainPageScanConditionModule"))
            {
                CommonDeleget.StateBSFeedbackEvent(HitchItems.Value.Count(q => (q as StatusModel).StatusCode == "0") == 0); 
            }
        }

        public IEnumerable GetStatusBarStatus()
        {
            ObservableCollection<SignalModel> signalModels = new ObservableCollection<SignalModel>();
            signalModels.Add(new SignalModel()
            {
                SignalName = "TopCabinRayStatus",
                SearchAction = new Func<string>(() =>
                {
                    short RayStatus = EquipmentManager.GetInstance().PlcManager.GetStatusByDIntPositionEnum(PLCPositionEnum.TopCabinRayStatus);
                    return RayStatusToText(RayStatus);
                })
            });
            signalModels.Add(new SignalModel()
            {
                SignalName = "InLetCabinRayStatus",
                SearchAction = new Func<string>(() =>
                {
                    short RayStatus = EquipmentManager.GetInstance().PlcManager.GetStatusByDIntPositionEnum(PLCPositionEnum.InLetCabinRayStatus);
                    return RayStatusToText(RayStatus);
                })
            });
            signalModels.Add(new SignalModel()
            {
                SignalName = "SideCabinRayStatus",
                SearchAction = new Func<string>(() =>
                {
                    short RayStatus = EquipmentManager.GetInstance().PlcManager.GetStatusByDIntPositionEnum(PLCPositionEnum.SideCabinRayStatus);
                    return RayStatusToText(RayStatus);
                })
            });
            signalModels.Add(new SignalModel()
            {
                SignalName = "TopCabinRaying",
                SignalType = (int)SignalModelTypeEnum.Icon,
                SearchAction = new Func<string>(() =>
                {
                    bool RayStatus = EquipmentManager.GetInstance().PlcManager.GetStatusByPositionEnum(PLCPositionEnum.TopCabinRaying);
                    return RayStatus.ToString();
                })
            });
            signalModels.Add(new SignalModel()
            {
                SignalName = "InLetCabinRaying",
                SignalType = (int)SignalModelTypeEnum.Icon,
                SearchAction = new Func<string>(() =>
                {
                    bool RayStatus = EquipmentManager.GetInstance().PlcManager.GetStatusByPositionEnum(PLCPositionEnum.InLetCabinRaying);
                    return RayStatus.ToString(); 
                })
            });
            signalModels.Add(new SignalModel()
            {
                SignalName = "SideCabinRaying",
                SignalType = (int)SignalModelTypeEnum.Icon,
                SearchAction = new Func<string>(() =>
                {
                    bool RayStatus = EquipmentManager.GetInstance().PlcManager.GetStatusByPositionEnum(PLCPositionEnum.SideCabinRaying);
                    return RayStatus.ToString();
                })
            });
            signalModels.Add(new SignalModel()
            {
                SignalName = "InletCabinStartingDynamo",
                SignalType = (int)SignalModelTypeEnum.StartingDynamoIcon,
                SearchAction = new Func<string>(() =>
                {
                    bool RayStatus = EquipmentManager.GetInstance().PlcManager.GetStatusByPositionEnum(PLCPositionEnum.InLetStartingElectricalMachinery);
                    return RayStatus.ToString();
                })
            });
            signalModels.Add(new SignalModel()
            {
                SignalName = "TopCabinStartingDynamo",
                SignalType = (int)SignalModelTypeEnum.StartingDynamoIcon,
                SearchAction = new Func<string>(() =>
                {
                    bool RayStatus = EquipmentManager.GetInstance().PlcManager.GetStatusByPositionEnum(PLCPositionEnum.TopStartingElectricalMachinery);
                    return RayStatus.ToString();
                })
            });
            signalModels.Add(new SignalModel()
            {
                SignalName = "SideCabinStartingDynamo",
                SignalType = (int)SignalModelTypeEnum.StartingDynamoIcon,
                SearchAction = new Func<string>(() =>
                {
                    bool RayStatus = EquipmentManager.GetInstance().PlcManager.GetStatusByPositionEnum(PLCPositionEnum.SideStartingElectricalMachinery);
                    return RayStatus.ToString();
                })
            });
            return signalModels;
        }

        private string RayStatusToText(short RayStatus)
        {
            switch (RayStatus)
            {
                case 0:
                    return UpdateStatusNameAction("UnPreheat");
                case 1:
                    return UpdateStatusNameAction("Preheating");
                case 2:
                    return UpdateStatusNameAction("PreheatEnding");
                default:
                    return UpdateStatusNameAction("UnPreheat");
            }
        }
    }
}