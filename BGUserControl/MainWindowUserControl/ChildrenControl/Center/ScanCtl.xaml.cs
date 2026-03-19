using BG_Entities;
using BG_Services;
using BGLogs;
using BGModel;
using BGUserControl;
using CMW.Common.Utilities;
using MahApps.Metro.Controls;
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
using static BGUserControl.DiyUploadListView;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.CommonFunc;
using static CMW.Common.Utilities.ImageImportDll;

namespace BGUserControl
{
    /// <summary>
    /// ScanCtl.xaml 的交互逻辑
    /// </summary>
    public partial class ScanCtl : UserControl
    {
        ScanCtlMvvm scanCtlMvvm = new ScanCtlMvvm();
        public ScanCtl()
        {
            InitializeComponent();
            DataContext = scanCtlMvvm;
            Loaded += ScanCtl_Loaded;
        }

        private void ScanCtl_Loaded(object sender, RoutedEventArgs e)
        {
            scanCtlMvvm.Load();
        }
    }

    public class ScanCtlMvvm:BaseMvvm
    {
        Visibsliy _scanVisible = new Visibsliy() { IsShow = Visibility.Visible };
        Visibsliy _stopScanVisible = new Visibsliy() { IsShow = Visibility.Hidden };
        public Visibsliy scanVisible { get => _scanVisible; set { _scanVisible = value; RaisePropertyChanged("scanVisible"); } }
        public Visibsliy stopScanVisible { get => _stopScanVisible; set { _stopScanVisible = value; RaisePropertyChanged("stopScanVisible"); } }
        private bool _IsScan = false;
        public bool IsScan
        {
            get => _IsScan;
            set
            {
                Application.Current.BeginInvoke(() => {
                    _IsScan = value;
                    stopScanVisible.IsShow = value ? Visibility.Visible : Visibility.Hidden;
                    scanVisible.IsShow = value ? Visibility.Hidden : Visibility.Visible;
                    RaisePropertyChanged("stopScanVisible");
                    RaisePropertyChanged("scanVisible");
                });
            }
        }
        ICommand _ScanImageCommand = null;
        public ICommand ScanImageCommand { get { return _ScanImageCommand; } set { _ScanImageCommand = value; } }

        string Original = "M21.5";
        public ScanCtlMvvm()        {
            ScanImageCommand = new ScanImageCommand(ScanImage);
      
            LoadUIText();
            LoadUIFontSize();
            InitScanStatus();
            ScanImageService.GetInstance().CancelScanWebAction += CancelScan;
            ScanImageService.GetInstance().CompleteAction += CompleteScan;
            ScanConditionService.GetInstance().SystemConditionEvent += SystemCondition;
            EnhanceScanEvent += ScanCtlMvvm_EnhanceScanEvent;
        }

        private void ScanCtlMvvm_EnhanceScanEvent(bool IsScan, object EnhanceScanCarInfo)
        {
            string Scan = IsScan ? "Start" : "Stop";
            ScanImage(Scan);
        }

        public override void LoadUIText()
        {
            scanVisible.DisplayName = UpdateStatusNameAction("StartScan");
            stopScanVisible.DisplayName = UpdateStatusNameAction("EndScan");
        }
        private void InitScanStatus()
        {
            scanVisible.Status = "Start";
            stopScanVisible.Status = "Stop";
        }

        public void Test()
        {
            Common.CommandDic[Command.StartScan] = "M21.6";
            ScanImageService.GetInstance().Start();
            IsScan = true;
            //PLCControllerManager.GetInstance().WritePositionValue("M21.6", true);
        }

        public void TestStop()
        {
            Common.CommandDic[Command.StartScan] = Common.CommandDic[Command.AutoStartScan]; 
            Stop();
            //PLCControllerManager.GetInstance().WritePositionValue("M21.6", false);
        }

        public void Start()
        {
            if (!ScanConditionService.GetInstance().CheckSafeCondition())
            {
                BG_MESSAGEBOX.Show(UpdateStatusNameAction("Tip"), UpdateStatusNameAction("SystemConditionUnReady"));
                return;
            }
            

            /*
            if (PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.TestMode))
            {
                Common.CommandDic[Command.StartScan] = Common.CommandDic[Command.AutoStartScan]; // AutoStartScan = M21.6
            }
            else
            {
                Common.CommandDic[Command.StartScan] = Original; // Original | StartScan == 21.5
            }
            */
            Common.CommandDic[Command.StartScan] = Original;
            ScanImageService.GetInstance().Start();
            IsScan = true;
        }

        public void Stop()
        {
            if (MessageBox.Show(UpdateStatusNameAction("AskScaning"), UpdateStatusNameAction("Warning"), MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    ScanImageService.GetInstance().Stop(true);
                    IsScan = false;
                }
                catch (Exception ex)
                {
                    Log.GetDistance().WriteErrorLogs(ex.StackTrace);
                }
                return;
            }
           
        }
        /*
        int a = 1;
        int b = 0;
        int c = 0;
        */
        public void ScanImage(object scanstatus)
        {
            string ScanStatus = scanstatus as string;
            switch (ScanStatus)
            {
                case "Start":
                    Start();
                    break;
                case "Stop":
                    Stop();
                    break;
                case "testImg":
                    //Test();
                    //Log.GetDistance().WriteErrorLogs("gang error log...");
                    //Log.GetDistance().WriteInfoLogs("gang info log...");
                    break;
                case "testImgStop":
                    TestStop();
                    break;
                default:
                    break;
            }
        }

        public void SystemCondition(bool IsSystemReady)
        {
            if(!IsSystemReady)
            {
                Stop();
            }
        }

        public void CancelScan()
        {
            ScrollImageServices.Service.Stop();
            IsScan = false;
        }

        public void CompleteScan()
        {
            IsScan = false;
            ScanImageService.GetInstance().Stop();
        }

        public void Load()
        {
            Original = Common.CommandDic[Command.StartScan];
        }
    }
}
