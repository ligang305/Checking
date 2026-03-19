using BG_Entities;
using BG_Services;
using CMW.Common.Utilities;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static CMW.Common.Utilities.Common;

namespace CMW
{
    /// <summary>
    /// 用于CMW 手动校正的窗口
    /// </summary>
    class CalibrationWindowViewModel : WindowViewModelBase
    {

        #region Prop
        /// <summary>
        /// 第一层 Key，校正流程条目；第二层 Key，条目状态；Value，该条目该状态图标的可见性属性的名称字符串
        /// </summary>
        private Dictionary<CalibrateItem, Dictionary<CalibrateItemStatus, string>> _itemStatusVisibilities;

        public RelayCommand CloseCommand { get; private set; }
        /// <summary>
        /// 视图模型对应的对话框窗口对象
        /// </summary>
        private Window DialogWindow { get; set; }

        /// <summary>
        /// 对话框关闭后，返回的用户操作结果
        /// </summary>
        public MessageBoxResult MessageResult { get; private set; }

        /// <summary>
        /// 对话框窗口中显示的按钮类型
        /// </summary>
        private MessageBoxButton Buttons { get; set; }

        private string _caption;
        public string Caption
        {
            get { return _caption; }
            set
            {
                _caption = value;
                RaisePropertyChanged("Caption");
            }
        }

        /// <summary>
        /// 射线打开 成功
        /// </summary>
        private bool _openDRXaySuccess;


        /// <summary>
        /// 射线超时或者成功
        /// </summary>
        private bool _openDRXayTimeoutSuccess = true;
        /// <summary>
        /// 当前已等 DR 射线打开 的时间（循环次数）
        /// </summary>
        private int _curWaitXayOpenTime;
        /// <summary>
        /// 等待 射线 关闭/开启 的超时时间（循环次数）
        /// </summary>
        private int _waitXayChangeTimeOut = 10;


        /// <summary>
        /// 射线关闭 成功
        /// </summary>
        private bool _closeDRXaySuccess;
        /// <summary>
        /// 射线超时或者成功
        /// </summary>
        private bool _closeDRXayTimeoutSuccess = true;

        /// <summary>
        /// 当前已等 DR 射线打开 的时间（循环次数）
        /// </summary>
        private int _curWaitXayCloseTime;
        /// <summary>
        /// 窗体标题
        /// </summary>
        private string _WindowTitle { get; set; } = CommonDeleget.UpdateStatusNameAction("Calibration");
        public string WindowTitle
        {
            get => _WindowTitle;
            set
            {
                _WindowTitle = value;
                RaisePropertyChanged("WindowTitle");
            }
        }

        /// <summary>
        /// 窗口关闭按钮显隐
        /// </summary>
        private Visibility _CloseVisibility = Visibility.Hidden;
        public Visibility CloseVisibility
        {
            get => _CloseVisibility;
            set
            {
                _CloseVisibility = value;
                RaisePropertyChanged("CloseVisibility");
            }
        }

        /// <summary>
        /// 检查连接
        /// </summary>
        private string _CheckConnection { get; set; } = CommonDeleget.UpdateStatusNameAction("CheckConnection");
        public string CheckConnection
        {
            get => _CheckConnection;
            set
            {
                _CheckConnection = value;
                RaisePropertyChanged("CheckConnection");
            }
        }
        /// <summary>
        /// 检查PLC连接
        /// </summary>
        private string _CheckPLCConnection { get; set; } = CommonDeleget.UpdateStatusNameAction("CheckConnection With PLC");
        public string CheckPLCConnection
        {
            get => _CheckPLCConnection;
            set
            {
                _CheckPLCConnection = value;
                RaisePropertyChanged("CheckPLCConnection");
            }
        }
        /// <summary>
        /// 检查连接
        /// </summary>
        private string _CheckDetectorConnection { get; set; } = CommonDeleget.UpdateStatusNameAction("CheckConnection With Detector");
        public string CheckDetectorConnection
        {
            get => _CheckDetectorConnection;
            set
            {
                _CheckDetectorConnection = value;
                RaisePropertyChanged("CheckDetectorConnection");
            }
        }
        /// <summary>
        /// 检查连接
        /// </summary>
        private string _StartScan { get; set; } = CommonDeleget.UpdateStatusNameAction("Start Scan");
        public string StartScan
        {
            get => _StartScan;
            set
            {
                _StartScan = value;
                RaisePropertyChanged("StartScan");
            }
        }
        

        /// <summary>
        /// 满度校正
        /// </summary>
        private string _AirCalibrate { get; set; } = CommonDeleget.UpdateStatusNameAction("Air Calibrate");
        public string AirCalibrate
        {
            get => _AirCalibrate;
            set
            {
                _AirCalibrate = value;
                RaisePropertyChanged("AirCalibrate");
            }
        }
        /// <summary>
        /// 本底校正
        /// </summary>
        private string _GroundCalibrate { get; set; } = CommonDeleget.UpdateStatusNameAction("Ground Calibrate");
        public string GroundCalibrate
        {
            get => _GroundCalibrate;
            set
            {
                _GroundCalibrate = value;
                RaisePropertyChanged("GroundCalibrate");
            }
        }

        /// <summary>
        /// 本底校正错误提示信息
        /// </summary>
        private string _CalGroundErrorInfo { get; set; }
        public string CalGroundErrorInfo
        {
            get => _CalGroundErrorInfo;
            set
            {
                _CalGroundErrorInfo = value;
                RaisePropertyChanged("CalGroundErrorInfo");
            }
        }
        /// <summary>
        /// 数采失败的提示信息
        /// </summary>
        private string _DataScanErrorInfo { get; set; }
        public string DataScanErrorInfo
        {
            get => _DataScanErrorInfo;
            set
            {
                _DataScanErrorInfo = value;
                RaisePropertyChanged("DataScanErrorInfo");
            }
        }
        

        /// <summary>
        /// 亮校正异常信息
        /// </summary>
        private string _calAirErrorInfo;

        public string CalAirErrorInfo
        {
            get { return _calAirErrorInfo; }
            set
            {
                _calAirErrorInfo = value;
                RaisePropertyChanged();
            }
        }

        #region Sub Item Status Icon Visibility Binding Properties
        /// <summary>
        /// 检查连接子项的可见性
        /// </summary>
        private Visibility _checkConnectionSubItemVisibility = Visibility.Collapsed;
        public Visibility CheckConnectionSubItemVisibility
        {
            get { return _checkConnectionSubItemVisibility; }
            set
            {
                _checkConnectionSubItemVisibility = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 检查连接——未开始
        /// </summary>
        private Visibility _checkConnectionNotStartVisibility = Visibility.Visible;

        public Visibility CheckConnectionNotStartVisibility
        {
            get { return _checkConnectionNotStartVisibility; }
            set
            {
                _checkConnectionNotStartVisibility = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 检查连接——执行中
        /// </summary>
        private Visibility _checkConnectionExecutingVisibility = Visibility.Collapsed;

        public Visibility CheckConnectionExecutingVisibility
        {
            get { return _checkConnectionExecutingVisibility; }
            set
            {
                _checkConnectionExecutingVisibility = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 检查连接——已完成
        /// </summary>
        private Visibility _checkConnectionCompletedVisibility = Visibility.Collapsed;

        public Visibility CheckConnectionCompletedVisibility
        {
            get { return _checkConnectionCompletedVisibility; }
            set
            {
                _checkConnectionCompletedVisibility = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 检查连接——失败
        /// </summary>
        private Visibility _checkConnectionFailedVisibility = Visibility.Collapsed;

        public Visibility CheckConnectionFailedVisibility
        {
            get { return _checkConnectionFailedVisibility; }
            set
            {
                _checkConnectionFailedVisibility = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 启动扫描子项的显隐性
        /// </summary>
        private Visibility _StartDataScanSubItemVisibility = Visibility.Collapsed;

        public Visibility StartDataScanSubItemVisibility
        {
            get { return _StartDataScanSubItemVisibility; }
            set
            {
                _StartDataScanSubItemVisibility = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 启动数据扫描完成显隐性
        /// </summary>
        private Visibility _StartDataScanSubItemCompletedVisibility = Visibility.Collapsed;

        public Visibility StartDataScanSubItemCompletedVisibility
        {
            get { return _StartDataScanSubItemCompletedVisibility; }
            set
            {
                _StartDataScanSubItemCompletedVisibility = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 启动数据扫描失败显隐性
        /// </summary>
        private Visibility _StartDataScanSubItemFailedVisibility = Visibility.Collapsed;

        public Visibility StartDataScanSubItemFailedVisibility
        {
            get { return _StartDataScanSubItemFailedVisibility; }
            set
            {
                _StartDataScanSubItemFailedVisibility = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 启动数据扫描失败信息显隐性
        /// </summary>
        private Visibility _DataScanErrorInfoVisibility = Visibility.Collapsed;

        public Visibility DataScanErrorInfoVisibility
        {
            get { return _DataScanErrorInfoVisibility; }
            set
            {
                _DataScanErrorInfoVisibility = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 启动本底校正子项的可见性
        /// </summary>
        private Visibility _calibrateGroundSubItemVisibility = Visibility.Collapsed;

        public Visibility CalibrateGroundSubItemVisibility
        {
            get { return _calibrateGroundSubItemVisibility; }
            set
            {
                _calibrateGroundSubItemVisibility = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// DR本底校正——已完成
        /// </summary>
        private Visibility _calibrateDRGroundCompletedVisibility = Visibility.Collapsed;

        public Visibility CalibrateDRGroundCompletedVisibility
        {
            get { return _calibrateDRGroundCompletedVisibility; }
            set
            {
                _calibrateDRGroundCompletedVisibility = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 本底校正——失败
        /// </summary>
        private Visibility _calibrateDRGroundFailedVisibility = Visibility.Collapsed;

        public Visibility CalibrateDRGroundFailedVisibility
        {
            get { return _calibrateDRGroundFailedVisibility; }
            set
            {
                _calibrateDRGroundFailedVisibility = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 本底校正——已取消
        /// </summary>
        private Visibility _calibrateDRGroundCancelVisibility = Visibility.Collapsed;

        public Visibility CalibrateDRGroundCancelVisibility
        {
            get { return _calibrateDRGroundCancelVisibility; }
            set
            {
                _calibrateDRGroundCancelVisibility = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 本底校正错误信息提示语显隐性
        /// </summary>
        private Visibility _CalGroundErrorInfoVisibility = Visibility.Collapsed;

        public Visibility CalGroundErrorInfoVisibility
        {
            get { return _CalGroundErrorInfoVisibility; }
            set
            {
                _CalGroundErrorInfoVisibility = value;
                RaisePropertyChanged();
            }
        }
       

        /// <summary>
        /// 启动满度校正子项的可见性
        /// </summary>
        private Visibility _calibrateAirSubItemVisibility = Visibility.Collapsed;

        public Visibility CalibrateAirSubItemVisibility
        {
            get { return _calibrateAirSubItemVisibility; }
            set
            {
                _calibrateAirSubItemVisibility = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 亮校正错误 显隐性
        /// </summary>
        private Visibility _calAirErrorInfoVisibility = Visibility.Collapsed;

        public Visibility CalAirErrorInfoVisibility
        {
            get { return _calAirErrorInfoVisibility; }
            set
            {
                _calAirErrorInfoVisibility = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region Main Item Status Icon Visibility Binding Properties

        /// <summary>
        /// 检查 Detector 连接——已完成
        /// </summary>
        private Visibility _checkDetectorConnectionCompletedVisibility = Visibility.Collapsed;

        public Visibility CheckDetectorConnectionCompletedVisibility
        {
            get { return _checkDetectorConnectionCompletedVisibility; }
            set
            {
                _checkDetectorConnectionCompletedVisibility = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 检查 Detector 连接——失败
        /// </summary>
        private Visibility _checkDetectorConnectionFailedVisibility = Visibility.Collapsed;

        public Visibility CheckDetectorConnectionFailedVisibility
        {
            get { return _checkDetectorConnectionFailedVisibility; }
            set
            {
                _checkDetectorConnectionFailedVisibility = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 检查 PLC 连接——已完成
        /// </summary>
        private Visibility _checkPLCConnectionCompletedVisibility = Visibility.Collapsed;

        public Visibility CheckPLCConnectionCompletedVisibility
        {
            get { return _checkPLCConnectionCompletedVisibility; }
            set
            {
                _checkPLCConnectionCompletedVisibility = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 检查 PLC 连接——失败
        /// </summary>
        private Visibility _checkPLCConnectionFailedVisibility = Visibility.Collapsed;

        public Visibility CheckPLCConnectionFailedVisibility
        {
            get { return _checkPLCConnectionFailedVisibility; }
            set
            {
                _checkPLCConnectionFailedVisibility = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 启动扫描——未开始
        /// </summary>
        private Visibility _startDataScanNotStartVisibility = Visibility.Visible;

        public Visibility StartDataScanNotStartVisibility
        {
            get { return _startDataScanNotStartVisibility; }
            set
            {
                _startDataScanNotStartVisibility = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 启动扫描——执行中
        /// </summary>
        private Visibility _startDataScanExecutingVisibility = Visibility.Collapsed;

        public Visibility StartDataScanExecutingVisibility
        { 
            get { return _startDataScanExecutingVisibility; }
            set
            {
                _startDataScanExecutingVisibility = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 启动扫描——已完成
        /// </summary>
        private Visibility _startDataScanCompletedVisibility = Visibility.Collapsed;

        public Visibility StartDataScanCompletedVisibility
        {
            get { return _startDataScanCompletedVisibility; }
            set
            {
                _startDataScanCompletedVisibility = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 启动扫描——失败
        /// </summary>
        private Visibility _startDataScanFailedVisibility = Visibility.Collapsed;

        public Visibility StartDataScanFailedVisibility
        {
            get { return _startDataScanFailedVisibility; }
            set
            {
                _startDataScanFailedVisibility = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 启动扫描——已取消
        /// </summary>
        private Visibility _startDataScanCancelVisibility = Visibility.Collapsed;

        public Visibility StartDataScanCancelVisibility
        {
            get { return _startDataScanCancelVisibility; }
            set
            {
                _startDataScanCancelVisibility = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// 本底校正——未开始
        /// </summary>
        private Visibility _calibrateGroundNotStartVisibility = Visibility.Visible;

        public Visibility CalibrateGroundNotStartVisibility
        {
            get { return _calibrateGroundNotStartVisibility; }
            set
            {
                _calibrateGroundNotStartVisibility = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 本底校正——执行中
        /// </summary>
        private Visibility _calibrateGroundExecutingVisibility = Visibility.Collapsed;

        public Visibility CalibrateGroundExecutingVisibility
        {
            get { return _calibrateGroundExecutingVisibility; }
            set
            {
                _calibrateGroundExecutingVisibility = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 本底校正——已完成
        /// </summary>
        private Visibility _calibrateGroundCompletedVisibility = Visibility.Collapsed;

        public Visibility CalibrateGroundCompletedVisibility
        {
            get { return _calibrateGroundCompletedVisibility; }
            set
            {
                _calibrateGroundCompletedVisibility = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 本底校正——失败
        /// </summary>
        private Visibility _calibrateGroundFailedVisibility = Visibility.Collapsed;

        public Visibility CalibrateGroundFailedVisibility
        {
            get { return _calibrateGroundFailedVisibility; }
            set
            {
                _calibrateGroundFailedVisibility = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 本底校正——已取消
        /// </summary>
        private Visibility _calibrateGroundCancelVisibility = Visibility.Collapsed;

        public Visibility CalibrateGroundCancelVisibility
        {
            get { return _calibrateGroundCancelVisibility; }
            set
            {
                _calibrateGroundCancelVisibility = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 亮度校正——未开始
        /// </summary>
        private Visibility _calibrateAirNotStartVisibility = Visibility.Visible;

        public Visibility CalibrateAirNotStartVisibility
        {
            get { return _calibrateAirNotStartVisibility; }
            set
            {
                _calibrateAirNotStartVisibility = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 亮度校正——执行中
        /// </summary>
        private Visibility _calibrateAirExecutingVisibility = Visibility.Collapsed;

        public Visibility CalibrateAirExecutingVisibility
        {
            get { return _calibrateAirExecutingVisibility; }
            set
            {
                _calibrateAirExecutingVisibility = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 亮度校正——已完成
        /// </summary>
        private Visibility _calibrateAirCompletedVisibility = Visibility.Collapsed;

        public Visibility CalibrateAirCompletedVisibility
        {
            get { return _calibrateAirCompletedVisibility; }
            set
            {
                _calibrateAirCompletedVisibility = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 亮度校正——失败
        /// </summary>
        private Visibility _calibrateAirFailedVisibility = Visibility.Collapsed;

        public Visibility CalibrateAirFailedVisibility
        {
            get { return _calibrateAirFailedVisibility; }
            set
            {
                _calibrateAirFailedVisibility = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 亮度校正——已取消
        /// </summary>
        private Visibility _calibrateAirCancelVisibility = Visibility.Collapsed;

        public Visibility CalibrateAirCancelVisibility
        {
            get { return _calibrateAirCancelVisibility; }
            set
            {
                _calibrateAirCancelVisibility = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// DR满度校正——已完成
        /// </summary>
        private Visibility _calibrateDRAirCompletedVisibility = Visibility.Collapsed;

        public Visibility CalibrateDRAirCompletedVisibility
        {
            get { return _calibrateDRAirCompletedVisibility; }
            set
            {
                _calibrateDRAirCompletedVisibility = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// DR满度校正——失败
        /// </summary>
        private Visibility _calibrateDRAirFailedVisibility = Visibility.Collapsed;

        public Visibility CalibrateDRAirFailedVisibility
        {
            get { return _calibrateDRAirFailedVisibility; }
            set
            {
                _calibrateDRAirFailedVisibility = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// DR满度校正——已取消
        /// </summary>
        private Visibility _calibrateDRAirCancelVisibility = Visibility.Collapsed;

        public Visibility CalibrateDRAirCancelVisibility
        {
            get { return _calibrateDRAirCancelVisibility; }
            set
            {
                _calibrateDRAirCancelVisibility = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #endregion

        public CalibrationWindowViewModel()
        {
            CommonDeleget.WriteLogAction("CMW.SystemMenu.CalibrationWindowViewModel Init", LogType.NormalLog);
            InitItemStatusVisibilities();
            CreateCommands();
            // 异步启动校正
            Start();
            CommonDeleget.WriteLogAction("CMW.SystemMenu.CalibrationWindowViewModel Init End", LogType.NormalLog);
        }
        private void CreateCommands()
        {
            CloseCommand = new RelayCommand(CloseCommandExcute);
        }
        private async void CloseCommandExcute()
        {
            // 成功，1秒后关闭窗体；失败，显示关闭按钮
            await Task.Factory.StartNew(() =>
            {
                // 关闭窗口
                MessengerInstance.Send(new CloseWindowMessage(WindowKeys.CalibrationWindowKey));
            });
        }


        /// 各校正流程的各状态图标的可见性属性，初始化到 Dictionary 中
        /// </summary>
        private void InitItemStatusVisibilities()
        {
            _itemStatusVisibilities = new Dictionary<CalibrateItem, Dictionary<CalibrateItemStatus, string>>();

            // 主项： 检查连接
            _itemStatusVisibilities.Add(CalibrateItem.CheckConnection, new Dictionary<CalibrateItemStatus, string>
            {
                {CalibrateItemStatus.NotStart, nameof(CheckConnectionNotStartVisibility)},
                {CalibrateItemStatus.Executing, nameof(CheckConnectionExecutingVisibility)},
                {CalibrateItemStatus.Completed, nameof(CheckConnectionCompletedVisibility)},
                {CalibrateItemStatus.Failed, nameof(CheckConnectionFailedVisibility)}
            });

            // 检查 PLC 连接
            _itemStatusVisibilities.Add(CalibrateItem.CheckPLCConnection, new Dictionary<CalibrateItemStatus, string>
            {
                {CalibrateItemStatus.Completed, nameof(CheckPLCConnectionCompletedVisibility)},
                {CalibrateItemStatus.Failed, nameof(CheckPLCConnectionFailedVisibility)}
            });

            // 检查 Detector 连接
            _itemStatusVisibilities.Add(CalibrateItem.CheckDetectorConnection, new Dictionary<CalibrateItemStatus, string>
            {
                {CalibrateItemStatus.Completed, nameof(CheckDetectorConnectionCompletedVisibility)},
                {CalibrateItemStatus.Failed, nameof(CheckDetectorConnectionFailedVisibility)}
            });


            // 主项：  启动数据采集
            _itemStatusVisibilities.Add(CalibrateItem.StartDataScanAcquisition, new Dictionary<CalibrateItemStatus, string>
            {
                {CalibrateItemStatus.NotStart, nameof(StartDataScanNotStartVisibility)},
                {CalibrateItemStatus.Executing, nameof(StartDataScanExecutingVisibility)},
                {CalibrateItemStatus.Completed, nameof(StartDataScanCompletedVisibility)},
                {CalibrateItemStatus.Failed, nameof(StartDataScanFailedVisibility)},
                {CalibrateItemStatus.Cancel, nameof(StartDataScanCancelVisibility)}
            });

            // DR数采
            _itemStatusVisibilities.Add(CalibrateItem.StartDRDataAcquisition, new Dictionary<CalibrateItemStatus, string>
            {
                {CalibrateItemStatus.Completed, nameof(StartDataScanSubItemCompletedVisibility)},
                {CalibrateItemStatus.Failed, nameof(StartDataScanSubItemFailedVisibility)}
            });


            // 主项：  启动本底校正
            _itemStatusVisibilities.Add(CalibrateItem.CalibratingGround, new Dictionary<CalibrateItemStatus, string>
            {
                {CalibrateItemStatus.NotStart, nameof(CalibrateGroundNotStartVisibility)},
                {CalibrateItemStatus.Executing, nameof(CalibrateGroundExecutingVisibility)},
                {CalibrateItemStatus.Completed, nameof(CalibrateGroundCompletedVisibility)},
                {CalibrateItemStatus.Failed, nameof(CalibrateGroundFailedVisibility)},
                {CalibrateItemStatus.Cancel, nameof(CalibrateGroundCancelVisibility)}
            });

            // DR本底
            _itemStatusVisibilities.Add(CalibrateItem.CalibratingDRGround, new Dictionary<CalibrateItemStatus, string>
            {
                {CalibrateItemStatus.Completed, nameof(CalibrateDRGroundCompletedVisibility)},
                {CalibrateItemStatus.Failed, nameof(CalibrateDRGroundFailedVisibility)},
                {CalibrateItemStatus.Cancel, nameof(CalibrateDRGroundCancelVisibility)}
            });

            // 主项：  启动满度校正
            _itemStatusVisibilities.Add(CalibrateItem.CalibratingAir, new Dictionary<CalibrateItemStatus, string>
            {
                {CalibrateItemStatus.NotStart, nameof(CalibrateAirNotStartVisibility)},
                {CalibrateItemStatus.Executing, nameof(CalibrateAirExecutingVisibility)},
                {CalibrateItemStatus.Completed, nameof(CalibrateAirCompletedVisibility)},
                {CalibrateItemStatus.Failed, nameof(CalibrateAirFailedVisibility)},
                {CalibrateItemStatus.Cancel, nameof(CalibrateAirCancelVisibility)}
            });

            // DR满度
            _itemStatusVisibilities.Add(CalibrateItem.CalibratingDRAir, new Dictionary<CalibrateItemStatus, string>
            {
                {CalibrateItemStatus.Completed, nameof(CalibrateDRAirCompletedVisibility)},
                {CalibrateItemStatus.Failed, nameof(CalibrateDRAirFailedVisibility)},
                {CalibrateItemStatus.Cancel, nameof(CalibrateDRAirCancelVisibility)}
            });
        }
        #region Calibrate Methods

        /// <summary>
        /// 启动校正流程
        /// </summary>
        private async void Start()
        {
            // 执行流程
            bool isCalibrationSuccess = await ExecuteCalibration();

            // 关闭射线  todo：在最后加一个步骤 关闭射线，考虑超时
            if (PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.OutOfBeam))
            {
                // DR
                if (!CloseXRay())
                {
                    BGLogs.Log.GetDistance().WriteInfoLogs("End Calibfration: Close DR XRay failed!");
                }
            }
           

            // 成功，1秒后关闭窗体；失败，显示关闭按钮
            await Task.Factory.StartNew(() =>
            {
                if (isCalibrationSuccess)
                {
                    Thread.Sleep(1000);
                    // 关闭窗口
                    MessengerInstance.Send(new CloseWindowMessage(WindowKeys.CalibrationWindowKey));

                    IniDll.Write(Section.PLC, "LastCalibrationTime",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else
                {
                    //如果失败了，第一步停束停高压
                    BoostingControllerManager.GetInstance().StopRay();
                    
                    CloseVisibility = Visibility.Visible;
                }
                //第一步先给PLC发送校正模式
                if (CommandDic.ContainsKey(Command.CalibrationMode))
                {
                    PLCControllerManager.GetInstance().WritePositionValue(CommandDic[Command.CalibrationMode], false);
                }
            });
        }

        /// <summary>
        /// 执行校正流程
        /// </summary>
        /// <returns></returns>
        private Task<bool> ExecuteCalibration()
        {
            return Task.Factory.StartNew(() =>
            {
                // 1、检查连接
                if (!CheckEquipmentConnection())
                {
                    // UI CheckConnection 之后全部取消
                    UpdateUIItemStatus(CalibrateItem.StartDataScanAcquisition, CalibrateItemStatus.Cancel);
                    UpdateUIItemStatus(CalibrateItem.CalibratingGround, CalibrateItemStatus.Cancel);
                    UpdateUIItemStatus(CalibrateItem.CalibratingAir, CalibrateItemStatus.Cancel);
                    // 滚动至底部
                    MessengerInstance.Send(true, "CalibrationScrollToBottom");
                    return false;
                }
                //第一步先给PLC发送校正模式
                if(CommandDic.ContainsKey(Command.CalibrationMode))
                {
                    PLCControllerManager.GetInstance().WritePositionValue(CommandDic[Command.CalibrationMode], true);
                }
                // 关闭射线  todo：在最后加一个步骤 关闭射线，考虑超时
                if (PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.OutOfBeam))
                {
                    // DR
                    if (!CloseXRay())
                    {
                        BGLogs.Log.GetDistance().WriteInfoLogs("End Calibration: Close XRay failed!");
                    }
                }

                // 延时等待一会
                Thread.Sleep(2000);

                // 2、启动数据采集
                bool dataAcquisitionResult = StartDataAcquisition();

                MessengerInstance.Send(true, "CalibrationScrollToBottom");// 滚动至底部

                // 延时等待一会儿
                Thread.Sleep(1000);

                // 3、启动满度校正
                bool calibrateAirResult = CalibrateAirValue(dataAcquisitionResult);
                MessengerInstance.Send(true, "CalibrationScrollToBottom");// 滚动至底部

                // 延时等待一会儿
                Thread.Sleep(1000);
                // 4、启动本底校正
                bool calibrateGroundResult = CalibrateGroundValue(calibrateAirResult);
                MessengerInstance.Send(true, "CalibrationScrollToBottom");// 滚动至底部

                // 5、停止扫描流程
                bool calibrateStopResult = StopDataAcquisition();

                return dataAcquisitionResult && calibrateGroundResult && calibrateAirResult;
            });
        }

        /// <summary>
        /// 检查连接
        /// </summary>
        /// <returns></returns>
        private bool CheckEquipmentConnection()
        {
            // UI 检查连接，执行
            UpdateUIItemStatus(CalibrateItem.CheckConnection, CalibrateItemStatus.Executing);

            // 检查连接
            if (PLCControllerManager.GetInstance().IsConnect() && DetecotrControllerManager.GetInstance().IsConnection())
            {
                // UI 检查连接，完成
                UpdateUIItemStatus(CalibrateItem.CheckConnection, CalibrateItemStatus.Completed);
                return true;
            }
            else
            {
                // UI 检查连接，失败
                UpdateUIItemStatus(CalibrateItem.CheckConnection, CalibrateItemStatus.Failed);
                CheckConnectionSubItemVisibility = Visibility.Visible;

                // 是否 SCS 未连接
                if (!PLCControllerManager.GetInstance().IsConnect())
                {
                    UpdateUIItemStatus(CalibrateItem.CheckPLCConnection, CalibrateItemStatus.Failed);
                }
                else
                {
                    UpdateUIItemStatus(CalibrateItem.CheckPLCConnection, CalibrateItemStatus.Completed);
                }

                // 是否 CPS 未连接
                if (!DetecotrControllerManager.GetInstance().IsConnection())
                {
                    UpdateUIItemStatus(CalibrateItem.CheckDetectorConnection, CalibrateItemStatus.Failed);
                }
                else
                {
                    UpdateUIItemStatus(CalibrateItem.CheckDetectorConnection, CalibrateItemStatus.Completed);
                }

                return false;
            }
        }

        /// <summary>
        /// 启动数据采集
        /// </summary>
        private bool StartDataAcquisition()
        {
            // UI 启动数据采集，执行
            UpdateUIItemStatus(CalibrateItem.StartDataScanAcquisition, CalibrateItemStatus.Executing);

            // 启动数采
            var drResultTask = StartDRDataAcquisition();

            // 获取启动数采结果
            bool result = drResultTask.Result;

            // 判断数采结果
            if (result)
            {
                // UI 启动数据采集，完成
                UpdateUIItemStatus(CalibrateItem.StartDataScanAcquisition, CalibrateItemStatus.Completed);
            }
            else
            {
                // UI 启动数据采集，失败
                UpdateUIItemStatus(CalibrateItem.StartDataScanAcquisition, CalibrateItemStatus.Failed);
                StartDataScanSubItemVisibility = Visibility.Visible;
            }

            return result;
        }

        /// <summary>
        /// 校正本底数据
        /// </summary>
        private bool CalibrateGroundValue(bool calibrateAirResult)
        {
            // UI 本底校正，执行
            UpdateUIItemStatus(CalibrateItem.CalibratingGround, CalibrateItemStatus.Executing);

            // 开始校正
            var drResultTask = CalibrateDRGroundValue(calibrateAirResult);

            // 判断校正结果
            if (drResultTask.Result)
            {
                // UI 本底校正，完成
                UpdateUIItemStatus(CalibrateItem.CalibratingGround, CalibrateItemStatus.Completed);
                return true;
            }
            else
            {
                if (!calibrateAirResult)
                {
                    // UI 本底校正，取消
                    UpdateUIItemStatus(CalibrateItem.CalibratingGround, CalibrateItemStatus.Cancel);
                }
                else
                {
                    // UI 本底校正，失败
                    UpdateUIItemStatus(CalibrateItem.CalibratingGround, CalibrateItemStatus.Failed);
                }
                CalibrateGroundSubItemVisibility = Visibility.Visible;

                return false;
            }
        }

        /// <summary>
        /// 校正满度数据
        /// </summary>
        private bool CalibrateAirValue(bool drDataAcquisitionResult)
        {
            // UI 满度校正，执行
            UpdateUIItemStatus(CalibrateItem.CalibratingAir, CalibrateItemStatus.Executing);
            Thread.Sleep(1000);
            // 开始校正
            var drResultTask = CalibrateDRAirValue(drDataAcquisitionResult);

            // 判断校正结果
            if (drResultTask.Result)
            {
                UpdateUIItemStatus(CalibrateItem.CalibratingAir, CalibrateItemStatus.Completed);
                return true;
            }
            else
            {
                if (!drDataAcquisitionResult)
                {
                    // UI 满度校正，取消
                    UpdateUIItemStatus(CalibrateItem.CalibratingAir, CalibrateItemStatus.Cancel);
                }
                else
                {
                    // UI 满度校正，失败
                    UpdateUIItemStatus(CalibrateItem.CalibratingAir, CalibrateItemStatus.Failed);
                }
                CalibrateAirSubItemVisibility = Visibility.Visible;

                return false;
            }
        }

        /// <summary>
        /// 停止数据采集
        /// </summary>
        private bool StopDataAcquisition()
        {
            // UI 启动数据采集，执行
            UpdateUIItemStatus(CalibrateItem.StopDataScanAcquisition, CalibrateItemStatus.Executing);

            // 启动数采
            var drResultTask = StopDRDataAcquisition();

            // 获取启动数采结果
            bool result = drResultTask.Result;

            // 判断数采结果
            if (result)
            {
                // UI 启动数据采集，完成
                UpdateUIItemStatus(CalibrateItem.StartDataScanAcquisition, CalibrateItemStatus.Completed);
            }
            else
            {
                // UI 启动数据采集，失败
                UpdateUIItemStatus(CalibrateItem.StartDataScanAcquisition, CalibrateItemStatus.Failed);
                StartDataScanSubItemVisibility = Visibility.Visible;
            }

            return result;
        }
        #endregion

        #region Calibrate Help Methods

        /// <summary>
        /// 启动DR数据采集
        /// </summary>
        /// <returns></returns>
        private Task<bool> StartDRDataAcquisition()
        {
            return Task.Factory.StartNew(() =>
            {
                ImageImportDll.SX_SetMode(ImageImportDll.intPtr, 2);
                if (ImageImportDll.SX_Start(ImageImportDll.intPtr) == 0)
                {
                    UpdateUIItemStatus(CalibrateItem.StartDRDataAcquisition, CalibrateItemStatus.Completed);
                    return true;
                }
                else
                {
                    UpdateUIItemStatus(CalibrateItem.StartDRDataAcquisition, CalibrateItemStatus.Failed);

                    DataScanErrorInfoVisibility = Visibility.Visible;
                    DataScanErrorInfo = CommonDeleget.UpdateStatusNameAction("Start DAQ failed.");
                    return false;
                }
            });
        }

        /// <summary>
        /// DR满度校正
        /// </summary>
        /// <param name="dataAcquisitionResult"></param>
        /// <returns></returns>
        private Task<bool> CalibrateDRAirValue(bool dataAcquisitionResult)
        {
            return Task.Factory.StartNew(() =>
            {
                // 是否取消
                if (!dataAcquisitionResult)
                {
                    UpdateUIItemStatus(CalibrateItem.CalibratingAir, CalibrateItemStatus.Cancel);

                    CalAirErrorInfoVisibility = Visibility.Visible;
                    CalAirErrorInfo = CommonDeleget.UpdateStatusNameAction("Calibration is canceled.");
                    return false;
                }

                // 查询 DR 射线状态 —— 如果 DR 射线打开，则此步完成；如果 DR 未打开，则等待 DR 打开状态回调
                bool isXrayOpen = PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.OutOfBeam);
                if (!isXrayOpen)
                {
                    bool IsRayResult = false;
                    BoostingControllerManager.GetInstance().Ray();
                    IsRayResult = true;
                    if (IsRayResult)
                    {
                        while (_openDRXayTimeoutSuccess)
                        {
                            Thread.Sleep(1000);
                            _curWaitXayOpenTime++;
                            if (_curWaitXayOpenTime >= _waitXayChangeTimeOut)
                            {
                                _openDRXaySuccess = false;
                                break;
                            }
                            if (PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.OutOfBeam))
                            {
                                _openDRXaySuccess = true;
                                break;
                            }
                        }


                        if (!_openDRXaySuccess)
                        {
                            // 超时，打开失败
                            UpdateUIItemStatus(CalibrateItem.CalibratingAir, CalibrateItemStatus.Failed);
                            CalAirErrorInfoVisibility = Visibility.Visible;
                            CalAirErrorInfo = CommonDeleget.UpdateStatusNameAction("Open XRay timeout.");
                            return false;
                        }
                    }
                    else
                    {
                        UpdateUIItemStatus(CalibrateItem.CalibratingAir, CalibrateItemStatus.Failed);
                        CalAirErrorInfoVisibility = Visibility.Visible;
                        CalAirErrorInfo = CommonDeleget.UpdateStatusNameAction("Send Open XRay command failed.");
                        return false;
                    }
                }

                // 延时等待一会儿
                Thread.Sleep(5000);
                // 执行校正
                if (ImageImportDll.SX_SetLight(ImageImportDll.intPtr) == 0)
                {
                    // 延时等待一会儿
                    Thread.Sleep(2000);
                    UpdateUIItemStatus(CalibrateItem.CalibratingAir, CalibrateItemStatus.Completed);
                    return true;
                }
                else
                {
                    UpdateUIItemStatus(CalibrateItem.CalibratingAir, CalibrateItemStatus.Failed);
                    CalAirErrorInfoVisibility = Visibility.Visible;
                    CalAirErrorInfo = CommonDeleget.UpdateStatusNameAction("Send calibration command failed.");
                    return false;
                }

            });
        }

        /// <summary>
        /// DR本底校正
        /// </summary>
        /// <returns></returns>
        private Task<bool> CalibrateDRGroundValue(bool calibrateAirResult)
        {
            return Task.Factory.StartNew(() =>
            {
                // 是否取消
                if (!calibrateAirResult)
                {
                    UpdateUIItemStatus(CalibrateItem.CalibratingGround, CalibrateItemStatus.Cancel);

                    CalGroundErrorInfoVisibility = Visibility.Visible;
                    CalGroundErrorInfo = CommonDeleget.UpdateStatusNameAction("Calibration is canceled.");
                    return false;
                }

                // 查询 DR 射线状态 —— 如果 DR 射线关闭，则此步完成；如果 DR 未关闭，则等待 DR 关闭状态回调
                bool isDROpen = PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.OutOfBeam);
                if (isDROpen)
                {
                    bool IsStopRayResult = false;
                    BoostingControllerManager.GetInstance().StopRay();
                    IsStopRayResult = true;
                    if (IsStopRayResult)
                    {
                        while (_closeDRXayTimeoutSuccess) 
                        {
                            Thread.Sleep(1000);
                            _curWaitXayCloseTime++;
                            if (_curWaitXayCloseTime >= _waitXayChangeTimeOut)
                            {
                                _closeDRXaySuccess = false;
                                break;
                            }
                            if (!PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.OutOfBeam))
                            {
                                _closeDRXaySuccess = true;
                                break;
                            }
                        }
                        if (!_closeDRXaySuccess)
                        {
                            // 超时，打开失败
                            UpdateUIItemStatus(CalibrateItem.CalibratingAir, CalibrateItemStatus.Failed);
                            CalGroundErrorInfoVisibility = Visibility.Visible;
                            CalGroundErrorInfo = CommonDeleget.UpdateStatusNameAction("Close XRay timeout.");
                            return false;
                        }
                    }
                    else
                    {
                        UpdateUIItemStatus(CalibrateItem.CalibratingGround, CalibrateItemStatus.Failed);
                        CalGroundErrorInfoVisibility = Visibility.Visible;
                        CalGroundErrorInfo = CommonDeleget.UpdateStatusNameAction("Send closing XRay command failed.");
                        return false;
                    }
                }
                //第一步先给PLC发送校正模式
                if (CommandDic.ContainsKey(Command.CalibrationMode))
                {
                    PLCControllerManager.GetInstance().WritePositionValue(CommandDic[Command.CalibrationMode], false);
                }
                // 延时等待一会儿
                Thread.Sleep(2000);
                // 执行校正
                if (ImageImportDll.SX_SetDark(ImageImportDll.intPtr) == 0)
                {
                    // 延时等待一会儿
                    Thread.Sleep(2000);
                    UpdateUIItemStatus(CalibrateItem.CalibratingGround, CalibrateItemStatus.Completed);
                    return true;
                }
                else
                {
                    UpdateUIItemStatus(CalibrateItem.CalibratingGround, CalibrateItemStatus.Failed);
                    CalGroundErrorInfoVisibility = Visibility.Visible;
                    CalGroundErrorInfo = CommonDeleget.UpdateStatusNameAction("Send calibration command failed.");
                    return false;
                }
            });
        }

        /// <summary>
        /// 停止数据采集
        /// </summary>
        /// <returns></returns>
        private Task<bool> StopDRDataAcquisition()
        {
            return Task.Factory.StartNew(() =>
            {
                if (ImageImportDll.SX_Stop(ImageImportDll.intPtr) == 0)
                {
                    UpdateUIItemStatus(CalibrateItem.StopDataScanAcquisition, CalibrateItemStatus.Completed);
                    return true;
                }
                else
                {
                    UpdateUIItemStatus(CalibrateItem.StopDataScanAcquisition, CalibrateItemStatus.Failed);
                    DataScanErrorInfoVisibility = Visibility.Visible;
                    DataScanErrorInfo = CommonDeleget.UpdateStatusNameAction("Start DAQ failed.");
                    return false;
                }
            });
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// 关闭射线源（执行关闭失败，则再次关闭，最多关闭三次）
        /// </summary>
        /// <param name="xRayId"></param>
        private bool CloseXRay()
        {
            int closeNum = 0;           // 已执行的关闭次数
            bool closeResult = false;   // 关闭结果

            while (closeNum < 3)
            {
                closeResult = SetCommand(CommandDic[Command.StopRay],false);
                if (closeResult)
                {
                    break;
                }

                closeNum++;
            }

            return closeResult;
        }

        /// <summary>
        /// 设置界面指定项目的执行状态
        /// </summary>
        /// <param name="item"></param>
        /// <param name="status"></param>
        private void UpdateUIItemStatus(CalibrateItem item, CalibrateItemStatus status)
        {
            if (!_itemStatusVisibilities.ContainsKey(item))
            {
                return;
            }

            var visibilities = _itemStatusVisibilities[item];

            if (visibilities == null)
            {
                return;
            }

            List<CalibrateItemStatus> itemStatuses = new List<CalibrateItemStatus>();
            foreach (var key in visibilities.Keys)
            {
                itemStatuses.Add(key);
            }

            Type type = this.GetType();
            foreach (var itemStatus in itemStatuses)
            {
                PropertyInfo propertyInfo = type.GetProperty(visibilities[itemStatus]);
                if (propertyInfo == null)
                {
                    throw new ArgumentNullException(nameof(propertyInfo));
                }
                if (itemStatus == status)
                {
                    propertyInfo.SetValue(this, Visibility.Visible);
                }
                else
                {
                    propertyInfo.SetValue(this, Visibility.Collapsed);
                }
            }
        }
        #endregion
    }
}
/// <summary>
/// 校正过程中的项目
/// </summary>
enum CalibrateItem
{
    CheckConnection = 1,
    CheckDetectorConnection = 2,
    CheckPLCConnection = 3,
    StartDataScanAcquisition = 4,
    StartDRDataAcquisition = 5,
    CalibratingGround = 6,
    CalibratingDRGround = 7,
    CalibratingAir = 8,
    CalibratingDRAir = 9,
    StopDataScanAcquisition = 10,
}
/// <summary>
/// 校正过程中的项目状态
/// </summary>
enum CalibrateItemStatus
{
    NotStart = 0,
    Executing = 1,
    Completed = 2,
    Failed = 3,
    Cancel = 4
}