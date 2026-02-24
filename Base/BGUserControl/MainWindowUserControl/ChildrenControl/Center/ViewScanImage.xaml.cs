using BG_Entities;
using BG_Services;
using BG_WorkFlow;
using BGModel;
using BGUserControl.MainWindowUserControl;
using CMW.Common.Utilities;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.ImageImportDll;

namespace BGUserControl
{
    /// <summary>
    /// ViewScanImage.xaml 的交互逻辑
    /// </summary>
    public partial class ViewScanImage : UserControl
    {
        public ViewScanImage()
        {
            InitializeComponent();
            ViewScanImageViewModel viewScanImageViewModel = new ViewScanImageViewModel();
            DataContext = viewScanImageViewModel;
            viewScanImageViewModel.scanImage = RollImage;
        }
    }

    public class ViewScanImageViewModel : BaseMvvm
    {
        #region 私有变量
        DetecotrControllerManager detecotrControllerManager;
        #endregion

        #region RaisePropertyChanged


        public string _LabmForecolor;
        public string LabmForecolor { get { return _LabmForecolor; } set { _LabmForecolor = value; RaisePropertyChanged(); } }

        public string _LabelText;
        public string LabelText { get { return _LabelText; } set { _LabelText = value; RaisePropertyChanged(); } }

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

        public ScanImage scanImage;

        #endregion

        public ViewScanImageViewModel()
        {
            CreateCommands();
           
            Start();
        }


        public void Start()
        {
            IsTestMode = ConfigServices.GetInstance().localConfigModel.IsDetectorTestMode ? Visibility.Visible : Visibility.Collapsed;
            if (detecotrControllerManager == null)
            {
                detecotrControllerManager = DetecotrControllerManager.GetInstance();
            }
            DetecotorConnection(detecotrControllerManager.DetectorConnection);

        }
        public void Stop()
        {
            detecotrControllerManager.DisConnection();
            SX_Destroy(detecotrControllerManager.detectorEquipment.DetectorIntPtr);
        }


        private void CreateCommands()
        {
            ScanCommand = new RelayCommand(ScanDetector);
            DisScanCommand = new RelayCommand(DisScanDetector);
            CorrectCommand = new RelayCommand(CorrectDetector);
            SavePngCommand = new RelayCommand(SavePngExcute);
        }
        protected override void DetecotorConnection(int DetecotrConnection)
        {
            if (DetecotrConnection == 0)
            {
                IsConnected = $@"{UpdateStatusNameAction("Connecte")}";
            }
            else
            {
                IsConnected = $@"{UpdateStatusNameAction("UnConnected")}";
            }
            if (DetecotrConnection == 3)
            {
                LabelText = $"{ConfigServices.GetInstance().localConfigModel.ScanIpAddress}:{ConfigServices.GetInstance().localConfigModel.ScanPort} {ConfigServices.GetInstance().localConfigModel.ScanImagePort} - {UpdateStatusNameAction("Connected")}";
                LabmForecolor = ForeColorKey.GreenPoliceLight;
            }
            else
            {
                LabelText = $"{ConfigServices.GetInstance().localConfigModel.ScanIpAddress}:{ConfigServices.GetInstance().localConfigModel.ScanPort} {ConfigServices.GetInstance().localConfigModel.ScanImagePort} - {UpdateStatusNameAction("Connecting")}";
                LabmForecolor = ForeColorKey.RedPoliceLight;
            }
        }
        #region 手动测试

        private void ScanDetector()
        {
            if (detecotrControllerManager.DetectorConnection == 3)
            {
                SX_SetMode(intPtr, 0);
                SX_SetDirection(intPtr, PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.ScanForWard) ? 0 : 1);
                SX_Start(intPtr);
              
                ScrollImageServices.Service.Start();
            }
        }
        private void DisScanDetector()
        {
            ScrollImageServices.Service.IsScrollImage = false;
            if (detecotrControllerManager.DetectorConnection == 3)
            {
                SX_SetLight(intPtr);
                Thread.Sleep(1000);
                SX_SetDark(intPtr);

                RequestTaskAction();
                BuryingPoint($"RequestTask  End ！");
                SX_Stop(intPtr);
                //SX_Stop(detecotrControllerManager.detectorEquipment.DetectorIntPtr);
            }
            ScrollImageServices.Service.Stop();
        }
        private void CorrectDetector()
        {
            SX_SetLight(detecotrControllerManager.detectorEquipment.DetectorIntPtr);
        }
        private void SavePngExcute()
        {
            scanImage.Bigwbmp.WriteableBitmapToPng();
        }

        #endregion
    }
}
