using BG_Services;
using CMW.Common.Utilities;
using BGModel;
using System;
using System.Collections.Generic;
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
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
using BG_Entities;
using BGUserControl;
using System.Diagnostics;
using BG_WorkFlow;

namespace BGUserControl
{
    /// <summary>
    /// OperatorBarCtl.xaml 的交互逻辑
    /// </summary>
    public partial class OperatorBarCtl : UserControl
    {
        OperatorBarMvvm operatorBarMvvm = new OperatorBarMvvm();
        public OperatorBarCtl()
        {
            InitializeComponent();
            if(controlVersion == ControlVersion.BS)
            {
                opreationButton.Width = 200;
                SingleClick_Calicabration_One.Visibility = Visibility.Collapsed;
            }
            DataContext = operatorBarMvvm;
        }
    }

    public class OperatorBarMvvm : BaseMvvm
    {
        private HardwareState _errorSet = new HardwareState();
        public HardwareState errorSet
        {
            get => _errorSet;
            set
            {
                _errorSet = value;
                RaisePropertyChanged("errorSet");
            }
        }

        private HardwareState _systemLock = new HardwareState();
        public HardwareState systemLock
        {
            get => _systemLock;
            set
            {
                _systemLock = value;
                RaisePropertyChanged("systemLock");
            }
        }

        private HardwareState _shutUp = new HardwareState();
        public HardwareState shutUp
        {
            get => _shutUp;
            set
            {
                _shutUp = value;
                RaisePropertyChanged("shutUp");
            }
        }
        ICommand _OperatorCommand = null;
        public ICommand OperatorCommand { get { return _OperatorCommand; } set { _OperatorCommand = value; } }

        public OperatorBarMvvm()
        {
            LoadUIText();
            LoadUI();
            LoadUIFontSize();
            LoadOperationName();
            OperatorCommand = new OperatorCommand(OperationBar);
        }

        public override void LoadUIText()
        {
            errorSet.LabelText = UpdateStatusNameAction("ErrorReset");
            systemLock.LabelText = UpdateStatusNameAction("Calibration");
            shutUp.LabelText = UpdateStatusNameAction("ShutUp");
        }

        public void LoadUI()
        {
            errorSet.ImageLogo = SystemDirectoryConfig.GetInstance().GetImage() + @"/Reset.png";
            systemLock.ImageLogo = SystemDirectoryConfig.GetInstance().GetImage() + @"/Systemlock.png";
            shutUp.ImageLogo = SystemDirectoryConfig.GetInstance().GetImage() + @"/Shotdown.png";
        }

        public void LoadOperationName()
        {
            errorSet.ModulesName = "errorSet";
            systemLock.ModulesName = "systemLock";
            shutUp.ModulesName = "shutUp";
        }
        public void OperationBar(object pc)
        {
            switch (pc)
            {
                case "errorSet":
                    Task.Run(() =>
                    {
                        PLCControllerManager.GetInstance().WritePositionValue(CommandDic[Command.HitchReset], true);
                        Thread.Sleep(200);
                        PLCControllerManager.GetInstance().WritePositionValue(CommandDic[Command.HitchReset], false);
                        if(ConfigServices.GetInstance().localConfigModel.IsUserBS && controlVersion != ControlVersion.BS)
                        {
                            EquipmentManager.GetInstance().PlcManager.WritePositionValue(EquipmentManager.GetInstance().MouduleCommandDic[Command.HitchReset], true);
                            Thread.Sleep(200);
                            EquipmentManager.GetInstance().PlcManager.WritePositionValue(EquipmentManager.GetInstance().MouduleCommandDic[Command.HitchReset], false);
                        }
                    });
                    break;
                case "systemLock":
                    if (ScrollImageServices.Service.IsScrollImage)
                    {
                        BG_MESSAGEBOX.Show(UpdateStatusNameAction("Tip"), UpdateStatusNameAction("ScanSoftShutOff"));
                        return;
                    }
                    else
                    {
                       MessengerInstance.Send(new ShowMessageDialogWindowMessageAction(WindowKeys.MainWindowKey, WindowKeys.MessageDialogWindowKey,
                       UpdateStatusNameAction("Tip"), UpdateStatusNameAction("Calibration Tip"), MessageBoxButton.OKCancel,
                       new System.Action<DialogResult>((DialogResult _DialogResult) =>
                       {
                           if (_DialogResult == DialogResult.Ok)
                           {
                               Task.Run(() =>
                               {
                                   Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                                   {
                                       // 启动系统自检窗口
                                       MessengerInstance.Send(new OpenWindowMessage(WindowKeys.MainWindowKey,
                                       WindowKeys.CalibrationWindowKey, null));
                                   }));
                               });
                           }
                       })));
                    }
                    break;
                case "shutUp":
                    if (ScrollImageServices.Service.IsScrollImage)
                    {
                        BG_MESSAGEBOX.Show(UpdateStatusNameAction("Tip"), UpdateStatusNameAction("ScanSoftShutOff"));
                        return;
                    }
                    else
                    {
                        Application.Current.Shutdown();
                        Process.GetCurrentProcess().Kill();
                        Environment.Exit(0);
                        Task.Run(() =>
                        {
                            //Application.Current.Shutdown();
                            //Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                            //{
                            //    Process.GetCurrentProcess().Kill();
                            //}));
                            //BoostingControllerManager.GetInstance().StopRay();
                            //Thread.Sleep(700);
                            //PlcService.GetInstance().Stop();
                        });
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
