using BG_Entities;
using BG_Services;
using BG_WorkFlow;
using BGModel;
using BGUserControl;
using CMW.Common.Utilities;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
using static CMW.Common.Utilities.CommonDeleget;
using System.Windows.Threading;

namespace BGUserControl
{
    /// <summary>
    /// TopStatusMonitorPanel.xaml 的交互逻辑
    /// </summary>
    public partial class TopStatusMonitorPanel : UserControl
    {
        protected TopStatusMonitorPanelMvvm topStatusMonitorPanelMvvm = new TopStatusMonitorPanelMvvm();

        /// <summary>
        /// 表示当前的等待状态
        /// </summary>
        [Bindable(true)]
        public ObservableCollection<SignalModel> DataSource
        {
            get
            {
                return (ObservableCollection<SignalModel>)GetValue(DataSourceProperty);
            }
            set { SetValue(DataSourceProperty, value); }
        }

        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(IEnumerable), typeof(TopStatusMonitorPanel),
                new PropertyMetadata(new ObservableCollection<SignalModel>(), new PropertyChangedCallback(OnValueChange)));
        /// <summary>
        /// 设置文本
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TopStatusMonitorPanel topStatusMonitorPanel = d as TopStatusMonitorPanel;
            if (topStatusMonitorPanel != null && topStatusMonitorPanel.DataSource != null)
            {
                topStatusMonitorPanel.topStatusMonitorPanelMvvm.SignalModels = topStatusMonitorPanel.DataSource;
                topStatusMonitorPanel.statusBar.DataSource = topStatusMonitorPanel.topStatusMonitorPanelMvvm.SignalModels;
            }
        }

        public TopStatusMonitorPanel()
        {
            DataContext = topStatusMonitorPanelMvvm;
            InitializeComponent();
            topStatusMonitorPanelMvvm.LoadLogo();
            topStatusMonitorPanelMvvm.InitStatusSignalList();
            StateFeedbackAction += TopStatusMonitorPanel_StateFeedbackAction;
            BSStateFeedbackAction += TopStatusMonitorPanel_BSStateFeedbackAction;
            statusBar.SetBinding(StatusBar.DataSourceProperty,new Binding(".") {Source = topStatusMonitorPanelMvvm.SignalModels });
           
        }

        private bool TopStatusMonitorPanel_BSStateFeedbackAction(bool IsSafe)
        {
            Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
            {
                topStatusMonitorPanelMvvm.ScanBSConfitionModule.LabmForecolor = IsSafe ? "#FFFFFF" : "#FFFF00";
                BSScanConditionModule.Style = IsSafe ? (Style)this.TryFindResource("diyCWMGreenBtn") : (Style)this.TryFindResource("diyCWMRedBtn");
            }));

            return IsSafe;
        }
        private Dispatcher UIDispatcher => Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
        Style GreenStyle =  (Style)Application.Current.TryFindResource("diyCWMGreenBtn");
        Style RedStyle = (Style)Application.Current.TryFindResource("diyCWMRedBtn");
        private bool TopStatusMonitorPanel_StateFeedbackAction(bool IsSafe)
        {
            Debug.WriteLine("-------------------------ganggang safe6-----------------------" + IsSafe);
            topStatusMonitorPanelMvvm.ScanConfitionModule.LabmForecolor = IsSafe ? "#FFFFFF" : "#FFFF00";
            topStatusMonitorPanelMvvm.UpdateScanConditionState(IsSafe);
            return IsSafe;
        }
    }

    public class TopStatusMonitorPanelMvvm:BaseMvvm
    {
        //private ICommand _ShowModulesCommands = null;
        //public ICommand ShowModulesCommands { get { return _ShowModulesCommands; }set { ShowModulesCommands = value; } }
        public RelayCommand<string> ShowModulesCommands { get; private set; }
        public TopStatusMonitorPanelMvvm()
        {
            LoadModulesName();
            LoadUIFontSize();
            //_ShowModulesCommands = new ShowModulesCommand(ShowModules);
            ShowModulesCommands = new RelayCommand<string>(ShowModules);
            if (ConfigServices.GetInstance().localConfigModel.IsUserBS && controlVersion != ControlVersion.BS)
            {
                BSSetting.IsShow = Visibility.Visible;
                BSControlModule.IsShow = Visibility.Visible;
            }
        }

        public void UpdateScanConditionState(bool isSafe)
        {
            Application.Current.Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.Normal,
                new Action(() =>
                {
                    ScanConditionButtonStyle =
                        (Style)Application.Current.TryFindResource(
                            isSafe ? "diyCWMGreenBtn" : "diyCWMRedBtn"
                        );
                })
            );
        }

        private Style _scanConditionButtonStyle;
        public Style ScanConditionButtonStyle
        {
            get => _scanConditionButtonStyle;
            set
            {
                _scanConditionButtonStyle = value;
                RaisePropertyChanged(nameof(ScanConditionButtonStyle));
            }
        }

        private HardwareState _scanControlModule = new HardwareState();
        public HardwareState ScanControlModule
        {
            get => _scanControlModule;
            set
            {
                _scanControlModule = value;
                RaisePropertyChanged("scanControlModule");
            }
        }

        private HardwareState _scanMonitorModule = new HardwareState();
        public HardwareState ScanMonitorModule
        {
            get => _scanMonitorModule;
            set
            {
                _scanMonitorModule = value;
                RaisePropertyChanged("ScanMonitorModule");
            }
        }

        private HardwareState _systemSetting = new HardwareState();
        public HardwareState SystemSetting
        {
            get => _systemSetting;
            set
            {
                _systemSetting = value;
                RaisePropertyChanged("SystemSetting");
            }
        }

        private HardwareState _BSSetting = new HardwareState();
        public HardwareState BSSetting
        {
            get => _BSSetting;
            set
            {
                _BSSetting = value;
                RaisePropertyChanged("BSSetting");
            }
        }
        private HardwareState _BSControlModule = new HardwareState();
        public HardwareState BSControlModule
        {
            get => _BSControlModule;
            set
            {
                _BSControlModule = value;
                RaisePropertyChanged("BSControlModule");
            }
        }

        private HardwareState _PlcStatndardSetting = new HardwareState();
        public HardwareState PlcStatndardSetting
        {
            get => _PlcStatndardSetting;
            set
            {
                _PlcStatndardSetting = value;
                RaisePropertyChanged("PlcStatndardSetting");
            }
        }

        private HardwareState _commonSettingModule = new HardwareState();
        public HardwareState CommonSettingModule
        {
            get => _commonSettingModule;
            set
            {
                _commonSettingModule = value;
                RaisePropertyChanged("CommonSettingModule");
            }
        }

        private HardwareState _traialImageMode = new HardwareState();
        public HardwareState TraialImageMode
        {
            get => _traialImageMode;
            set
            {
                _traialImageMode = value;
                RaisePropertyChanged("TraialImageMode");
            }
        }

        private HardwareState _mainPage = new HardwareState();
        public HardwareState MainPage
        {
            get => _mainPage;
            set
            {
                _mainPage = value;
                RaisePropertyChanged("MainPage");
            }
        }

        private HardwareState _SoftWareInfomation = new HardwareState();
        public HardwareState SoftWareInfomation
        {
            get => _SoftWareInfomation;
            set
            {
                _SoftWareInfomation = value;
                RaisePropertyChanged("SoftWareInfomation");
            }
        }
        private HardwareState _RepirModules = new HardwareState();
        /// <summary>
        /// 维修模块
        /// </summary>
        public HardwareState RepirModules
        {
            get => _RepirModules;
            set
            {
                _RepirModules = value;
                RaisePropertyChanged("RepirModules");
            }
        }
        private HardwareState _scanConfitionModule = new HardwareState();
        public HardwareState ScanConfitionModule
        {
            get => _scanConfitionModule;
            set
            {
                _scanConfitionModule = value;
                RaisePropertyChanged("ScanConfitionModule");
            }
        }
        private HardwareState _scanBSConfitionModule = new HardwareState();
        public HardwareState ScanBSConfitionModule
        {
            get => _scanBSConfitionModule;
            set
            {
                _scanBSConfitionModule = value;
                RaisePropertyChanged("ScanBSConfitionModule");
            }
        }
        private HardwareState _logo = new HardwareState();
        public HardwareState Logo
        {
            get => _logo;
            set
            {
                _logo = value;
                RaisePropertyChanged("Logo");
            }
        }

        ObservableCollection<SignalModel> _signalModels = new ObservableCollection<SignalModel>();
        public ObservableCollection<SignalModel> SignalModels
        {
            get => _signalModels;
            set
            {
                _signalModels = value;
                RaisePropertyChanged("SignalModels");
            }
        }
        public override void LoadUIText()
        {
            ScanControlModule.LabelText = $@"{UpdateStatusNameAction(controlVersion.ToString())}{UpdateStatusNameAction("Monitor")}";
            SystemSetting.LabelText = $@"{UpdateStatusNameAction(controlVersion.ToString())}{UpdateStatusNameAction("Control")}"; 
            BSSetting.LabelText = UpdateStatusNameAction("BSControl");
            BSControlModule.LabelText = UpdateStatusNameAction("BSMonitor");

            PlcStatndardSetting.LabelText = UpdateStatusNameAction("PlcStandard");
            CommonSettingModule.LabelText = UpdateStatusNameAction("Setting");
            TraialImageMode.LabelText = UpdateStatusNameAction("TraialImageMode");
            MainPage.LabelText = UpdateStatusNameAction("MainPage");
            ScanConfitionModule.LabelText = $@"{UpdateStatusNameAction(controlVersion.ToString())}{UpdateStatusNameAction("ScanCondition")}";
            ScanBSConfitionModule.LabelText = UpdateStatusNameAction("BSScanCondition");
            SoftWareInfomation.LabelText = UpdateStatusNameAction("SoftwareVersion");
            RepirModules.LabelText = UpdateStatusNameAction("RepirModule");
            ScanConfitionModule.LabmForecolor = "#FFFFFF";
            ScanBSConfitionModule.LabmForecolor = "#FFFFFF";
        }
        public void LoadModulesName()
        {
            MainPage.ModulesName = "MainPage";
            CommonSettingModule.ModulesName = "CommonSettingModule";
            TraialImageMode.ModulesName = "Modulesvehicle";
            SoftWareInfomation.ModulesName = "SoftDetailsModules";
            RepirModules.ModulesName = Modules.RepirModule;
            ScanConfitionModule.ModulesName = "MainPageScanConditionModule";
            ScanControlModule.ModulesName = "ControlModule";
            SystemSetting.ModulesName = "SystemSetting";
            BSControlModule.ModulesName = Modules.BSControlModule;
            BSSetting.ModulesName = "BSSystemSetting";
            ScanBSConfitionModule.ModulesName = "BSMainPageScanConditionModule";
            PlcStatndardSetting.ModulesName = "StandardStatusPanel";
        }
        public void LoadLogo()
        {
            BSControlModule.ImageLogo = SystemDirectoryConfig.GetInstance().GetImage() + @"/BSMonitor.png";
            BSSetting.ImageLogo = SystemDirectoryConfig.GetInstance().GetImage() + @"/BSControl.png";
            ScanControlModule.ImageLogo = SystemDirectoryConfig.GetInstance().GetImage() + @"/Monitor.png";
            SystemSetting.ImageLogo = SystemDirectoryConfig.GetInstance().GetImage() + @"/Control.png";
            PlcStatndardSetting.ImageLogo = SystemDirectoryConfig.GetInstance().GetImage() + @"/plc.png";
            CommonSettingModule.ImageLogo = SystemDirectoryConfig.GetInstance().GetImage() + @"/set.png";
            TraialImageMode.ImageLogo = SystemDirectoryConfig.GetInstance().GetImage() + @"/checkin.png";
            MainPage.ImageLogo = SystemDirectoryConfig.GetInstance().GetImage() + @"/main.png";
            SoftWareInfomation.ImageLogo = SystemDirectoryConfig.GetInstance().GetImage() + @"/about.png";
            RepirModules.ImageLogo = SystemDirectoryConfig.GetInstance().GetImage() + @"/Repir.png";
            Logo.ImageLogo = SystemDirectoryConfig.GetInstance().GetImage() + @"/logo.png";
            ScanConfitionModule.ImageLogo = SystemDirectoryConfig.GetInstance().GetImage() + @"/ScanCondition.png";
            ScanBSConfitionModule.ImageLogo = SystemDirectoryConfig.GetInstance().GetImage() + @"/ScanCondition.png";
        }
        public void InitStatusSignalList()
        {
            InitStatus();
        }
        public void InitStatus()
        {
            ObservableCollection<SignalModel> signalModels = new ObservableCollection<SignalModel>();
            SignalModel ScanMode = new SignalModel() { SearchAction = new Func<string>(() => { return  BoostingControllerManager.GetInstance().SearchScanMode(); }), SignalName = "ScanMode" };
            SignalModel PreheatStatusStatusModel = new SignalModel() { SignalName = "PreheatStatus", SearchAction = new Func<string>(() => { return BoostingControllerManager.GetInstance().GetRayAndPreviewHot(); }) };
            SignalModel RunStatus = new SignalModel() { SignalName = "RunStatus", SearchAction = new Func<string>(() => { return PLCControllerManager.GetInstance().GetRunStatus(); }) };
            SignalModel EnergyModeStatusModel = new SignalModel() { SignalName = "EnergyMode", SearchAction = new Func<string>(() => { return BoostingControllerManager.GetInstance().ReadDoubleOrSingle(); }) };
            SignalModel BeamStatusModel = new SignalModel()
            {
                SignalName = "BeamStatus",
                SearchAction = new Func<string>(() =>
                {
                    return BoostingControllerManager.GetInstance().IsRayOut() ? "OutOfBeam" : "NotBeaming";
                })
            };
            
            signalModels.Add(ScanMode);
            signalModels.Add(RunStatus);
            signalModels.Add(PreheatStatusStatusModel);
            signalModels.Add(EnergyModeStatusModel);
            signalModels.Add(BeamStatusModel);
            SignalModels = signalModels;
        }
        string ModulesName = string.Empty; 
        public void ShowModules(object pc)
        {
            ModulesName = pc as string;
            Trace.WriteLine("gang trace: " + ModulesName);
            if (pc as string == Modules.RepirModule)
            {
                MessengerInstance.Send(new ShowMessageDialogWindowMessageAction(WindowKeys.MainWindowKey, WindowKeys.PasswordWindowKey, 
                    UpdateStatusNameAction("Enter into Repair Mode,InputPassword"),UpdateStatusNameAction("Password"),MessageBoxButton.OKCancel, ShowModulesByPassword));
                return;
            }
            CommonDeleget.ShowModuleEvent(pc as string);

            MessengerInstance.Send(new OpenWindowMessage(WindowKeys.MainWindowKey, WindowKeys.SystemReadyConditionWindow, pc as string));
        }

        public void ShowModulesByPassword(DialogResult dialogResult)
        {
            if(dialogResult == DialogResult.Ok)
            {
                CommonDeleget.ShowModuleEvent(ModulesName);

                MessengerInstance.Send(new OpenWindowMessage(WindowKeys.MainWindowKey, WindowKeys.SystemReadyConditionWindow, ModulesName));
            }
        }
    }
}
