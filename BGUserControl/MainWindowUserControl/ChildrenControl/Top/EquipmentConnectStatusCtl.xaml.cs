using BG_Entities;
using BG_Services;
using BGModel;
using BGUserControl;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
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
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;

namespace BGUserControl
{
    /// <summary>
    /// EquipmentConnectStatusCtl.xaml 的交互逻辑
    /// </summary>
    public partial class EquipmentConnectStatusCtl : UserControl
    {
        EquipmentConnectMvvm equipmentConnectMvvm = new EquipmentConnectMvvm();
        public EquipmentConnectStatusCtl()
        {
            InitializeComponent();
            DataContext = equipmentConnectMvvm;
            LoadUi();
        }

        private void LoadUi()
        {
            equipmentConnectMvvm.LoadUIText();
            equipmentConnectMvvm.LoadUIFontSize();
        }
    }

    public class EquipmentConnectMvvm : BaseMvvm
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

        public HardwareState _detectorConnection = new HardwareState();
        public HardwareState DetectorConnection
        {
            get => _detectorConnection;
            set
            {
                _detectorConnection = value;
                RaisePropertyChanged("DetectorConnection");
            }
        }

        public HardwareState _boostConnection = new HardwareState();
        public HardwareState BoostConnection
        {
            get => _boostConnection;
            set
            {
                _boostConnection = value;
                RaisePropertyChanged("BoostConnection");
            }
        }


        public EquipmentConnectMvvm()
        {
            if(controlVersion != ControlVersion.BS)
            {
                DetectorConnection.LabmForecolor = DetecotrControllerManager.GetInstance().DetectorConnection == 3 ? ForeColorKey.GreenPoliceLight : ForeColorKey.RedPoliceLight;
                DetectorConnection.IsShow = Visibility.Visible;
            }
            else
            {
                DetectorConnection.IsShow = Visibility.Collapsed;
            }
        }


        public override void LoadUIText()
        {
            PlcConnection.LabelText = UpdateStatusNameAction("ConnectionWithPlc");
            BoostConnection.LabelText = UpdateStatusNameAction("RadiationSourceOrAcceleratorReady");
            DetectorConnection.LabelText = UpdateStatusNameAction("ConnectionWithScan");
        }
        protected override void InquirePlcStatus(List<bool> StatusList)
        {

        }
        private Dispatcher UIDispatcher => Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
        protected async override void ConnectionStatus(bool ConnectionStatus)
        {
            await UIDispatcher.InvokeAsync(() =>
            {
                PlcConnection.LabmForecolor = ConnectionStatus ? ForeColorKey.GreenPoliceLight : ForeColorKey.RedPoliceLight;
                if (!ConnectionStatus)
                {
                    BoostConnection.LabmForecolor = ForeColorKey.RedPoliceLight;
                }
                else
                {
                    if (!(BoostingControllerManager.GetInstance().GetCurrentEquipmentModel() == "FH_Betratron"
                   || BoostingControllerManager.GetInstance().GetCurrentEquipmentModel() == "Russia_Betratron"))
                    {
                        BoostConnection.LabmForecolor = PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.RadiationSourceOrAcceleratorReady)
                     ? ForeColorKey.GreenPoliceLight : ForeColorKey.RedPoliceLight;
                    }
                }
            }, DispatcherPriority.Background);
        }

        protected async override void DetecotorConnection(int DetecotrConnection)
        {
            await UIDispatcher.InvokeAsync(() =>
            {
                DetectorConnection.LabmForecolor = DetecotrConnection == 3 ? ForeColorKey.GreenPoliceLight : ForeColorKey.RedPoliceLight;
            }, DispatcherPriority.Background);
        }

        protected override void AccelatorConnectionStatus(bool ConnectStatus)
        {
            if(BoostingControllerManager.GetInstance().GetCurrentEquipmentModel() == "FH_Betratron"
                || BoostingControllerManager.GetInstance().GetCurrentEquipmentModel() == "Russia_Betratron")
            {
                BoostConnection.LabmForecolor = ConnectStatus  ? ForeColorKey.GreenPoliceLight : ForeColorKey.RedPoliceLight;
            }
        }
    }
}
