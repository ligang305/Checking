using BG_Services;
using CMW.Common.Utilities;
using BGCommunication;
using BGModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static CMW.Common.Utilities.Common;
using BG_Entities;
using BG_WorkFlow;

namespace FH_BoostingController
{
    [Export("Boosting", typeof(IEquipment))]
    [CustomExportMetadata(1, "BetratronBoostingForFH_Module", "CGN. Begood", "zhuzhiwu", "1.0.0")]
    public class BetratronBoostingForFH : BoostingController<BetratronBoostingForFH>
    {
        ObservableCollection<BoostingModel> BoostingModelList = new ObservableCollection<BoostingModel>();
        Self_BoostSettingBLL sbb = new Self_BoostSettingBLL(Common.controlVersion);
        static bool isPreHot = false;
        public BetratronBoostingForFH()
        {
            BoostingModelList.Clear();
            sbb.GetBoostingModelList(Common.controlVersion).ToList().ForEach(q => BoostingModelList.Add(q));
            CommonDeleget.BetratronStopPrehotEvent += StopRay;
            CommonDeleget.BetratronRayEvent += Ray;
        }
        public override void Load(ICommonProtocol commonProtocol)
        {
            base.CurrentBetratronProrocol = commonProtocol;
            //base.CurrentBetratronProrocol.WaitRecTick = 150;
            //ConnectInterface client = new CSharpSerialClient(localConfigModel.SerialPort, localConfigModel.BaudRate, localConfigModel.Parity, localConfigModel.ByteSize, localConfigModel.StopBit);
            //base.CurrentBetratronProrocol.InitConnection(ref client);
            //base.CurrentBetratronProrocol.Connect();
        }
        public override void Load()
        {
            base.CurrentBetratronProrocol = new BetratronProtocolForFH();
            base.CurrentBetratronProrocol.WaitRecTick = 500;
            ConnectInterface client = new CSharpSerialClient(ConfigServices.GetInstance().localConfigModel.SerialPort, ConfigServices.GetInstance().localConfigModel.BaudRate,
                    ConfigServices.GetInstance().localConfigModel.Parity, ConfigServices.GetInstance().localConfigModel.ByteSize, ConfigServices.GetInstance().localConfigModel.StopBit);
            base.CurrentBetratronProrocol.InitConnection(ref client);
            base.CurrentBetratronProrocol.Connect();
        }
        /// <summary>
        /// 初始化加速器协议
        /// </summary>
        /// <param name="commonProtocol"></param>
        public override void InitBetratronProcol()
        {
            base.CurrentBetratronProrocol = new BetratronProtocolForFH();
            base.CurrentBetratronProrocol.WaitRecTick = 150;
            ConnectInterface client = new CSharpSerialClient(ConfigServices.GetInstance().localConfigModel.SerialPort, 
                ConfigServices.GetInstance().localConfigModel.BaudRate, ConfigServices.GetInstance().localConfigModel.Parity, 
                ConfigServices.GetInstance().localConfigModel.ByteSize, ConfigServices.GetInstance().localConfigModel.StopBit);
            base.CurrentBetratronProrocol.InitConnection(ref client);
            base.CurrentBetratronProrocol.Connect();
        }
        /// <summary>
        /// 出束 
        /// </summary>
        public override void Ray()
        {
           if(!Common.GlobalRetStatus[12])
            {
                CommonDeleget.MessageBoxActionAction("安全联锁未就绪，无法出束！");
                return;
            }
            BoostingModel volModel = BoostingModelList.FirstOrDefault(q => q.Name == "InjectionCurrent");
            BoostingModel HMC = BoostingModelList.FirstOrDefault(q => q.Name == "HeightEnergyMC");
            BoostingModel H = BoostingModelList.FirstOrDefault(q => q.Name == "HeightEnergy");
            BoostingModel LMC = BoostingModelList.FirstOrDefault(q => q.Name == "LowerEnergyMC");
            BoostingModel L = BoostingModelList.FirstOrDefault(q => q.Name == "LowerEnergy");
            
            if (volModel.Equals(string.Empty))
            {
                CommonDeleget.MessageBoxActionAction("电流不能为空！");
                return;
            }
            if (HMC.Equals(string.Empty))
            {
                CommonDeleget.MessageBoxActionAction("高能脉冲不能为空！");
                return;
            }
            if (H.Equals(string.Empty))
            {
                CommonDeleget.MessageBoxActionAction("高能不能为空！");
                return;
            }
            if (LMC.Equals(string.Empty))
            {
                CommonDeleget.MessageBoxActionAction("低能脉冲不能为空！");
                return;
            }
            if (L.Equals(string.Empty))
            {
                CommonDeleget.MessageBoxActionAction("低能不能为空！");
                return;
            }
            
            isPreHot = true;
            //float vol = (float)Convert.ToDouble(txtvol.Text);
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(51);//这里停51毫秒的目的是让预热的线程跳出来
                if (!isPreHot)
                {
                    return;
                }
                Thread.Sleep(100);
                    //TODO 这个出束条件需要再商量，以防万一第一次设定不对
                    //设定出束后保持50HZ的状态
                    //设定50HZ
                    (CurrentBetratronProrocol as BetratronProtocolForFH).SendCommandWaitWorkStop();
                    Thread.Sleep(11000);
                    //出束
                    (CurrentBetratronProrocol as BetratronProtocolForFH).SendCommandRadiationOn();
            });
        }
        /// <summary>
        /// 停束
        /// </summary>
        public override void StopRay()
        {
            isPreHot = false;
            Task.Factory.StartNew(() =>
            {
                (CurrentBetratronProrocol as BetratronProtocolForFH).SendCommandRadiationOff();
                Thread.Sleep(150);
                (CurrentBetratronProrocol as BetratronProtocolForFH).SendCommandRadiationOff();
                Thread.Sleep(150);
                (CurrentBetratronProrocol as BetratronProtocolForFH).SendCommandRadiationOff();
            });
        }
        /// <summary>
        /// 预热
        /// </summary>
        public virtual void WarmUp()
        {
            if (1 == 1) {
                Thread.Sleep(150);
            }
        }
        /// <summary>
        /// 预热毕
        /// </summary>
        public virtual void WarmUpEnd()
        {

        }
        /// <summary>
        /// 加速预热
        /// </summary>
        /// <returns></returns>
        public override string GetRayAndPreviewHot()
        {
            switch ((CurrentBetratronProrocol as BetratronProtocolForFH).FHParameter.BoostingSystemWorkStatus)
            {
                case WorkingJob.FH_SystemReady:
                    return "Ready";
                case WorkingJob.FH_RadiationOpenFindMax:
                    return "SearchMaxDoseRate";
                case WorkingJob.FH_RadiationCloseOfDose:
                    return "UnPreheat";
                case WorkingJob.FH_WaitWramup50 :
                    return "PreheatEnding";
                case WorkingJob.FH_10SecondReady:
                    return "Preheating";
                case WorkingJob.FH_Radiationing:
                    return "OutOfBeam";
                case WorkingJob.WJ_NULL:
                    return "NULL";
                case WorkingJob.FH_KeyOff:
                    return "KeyOff";
                default:
                    return "NULL";
            }
            return "NULL";
        }
        public override bool IsRayOut()
        {
            return (CurrentBetratronProrocol as BetratronProtocolForFH).FHParameter.BoostingSystemWorkStatus == WorkingJob.FH_Radiationing;
        }
        /// <summary>
        /// 查询状态
        /// </summary>
        public override void Inquire()
        {
            if(base.CurrentBetratronProrocol == null)
            {
                return;
            }
            if (!base.CurrentBetratronProrocol.IsConnect())
            {
                base.CurrentBetratronProrocol.DisConnect();
                base.CurrentBetratronProrocol.Connect();
                return;
            }
            (base.CurrentBetratronProrocol as BetratronProtocolForFH).InqureDoseRate(CommandFun.CF_SystemStatus);
            WJB = (base.CurrentBetratronProrocol as BetratronProtocolForFH).FHParameter.BoostingSystemWorkStatus;
        }
        /// <summary>
        /// 复位
        /// </summary>
        /// <returns></returns>
        public override bool Reset()
        {
            return false;//(CurrentBetratronProrocol as BetratronProtocolForFH).Reset();
        }
        /// <summary>
        /// 设置电流
        /// </summary>
        public override bool SetCurrentFlows(double CurrentFlows)
        {
            return false;
        }
        /// <summary>
        /// 设置高能脉冲
        /// </summary>
        public override bool SetHMC(double HLC)
        {
           return (CurrentBetratronProrocol as BetratronProtocolForFH).SendCommandHighEnergy(Convert.ToUInt16(HLC));
        }
        /// <summary>
        /// 设置低能脉冲
        /// </summary>
        public override bool SetLMC(double LMC)
        {
            return  (CurrentBetratronProrocol as BetratronProtocolForFH).SendCommandLowEnergy(Convert.ToUInt16(LMC));
        }
        /// <summary>
        /// 设置高能脉冲个数
        /// </summary>
        public override bool SetHMCNum(double LMCNum)
        {
           return  (CurrentBetratronProrocol as BetratronProtocolForFH).SendCommandHighPulses(Convert.ToUInt16(LMCNum));
        }
        /// <summary>
        /// 设置低能脉冲个数
        /// </summary>
        public override bool SetLMCNum(double LMCNum)
        {
            return (CurrentBetratronProrocol as BetratronProtocolForFH).SendCommandLowPulsees(Convert.ToUInt16(LMCNum));
        }
        /// <summary>
        /// 设置工作模式
        /// </summary>
        /// <param name="workMode"></param>
        /// <returns></returns>
        public bool SendCommandMainMagWorkFreMode(ushort workMode)
        {
           return  (CurrentBetratronProrocol as BetratronProtocolForFH).SendCommandMainMagWorkFreMode(Convert.ToUInt16(workMode));
        }
        /// <summary>
        /// 设置内部剂量
        /// </summary>
        /// <param name="workMode"></param>
        /// <returns></returns>
        public override bool SendCommandDoseInternal(ushort doseInternal)
        {
            return (CurrentBetratronProrocol as BetratronProtocolForFH).SendCommandDoseInternal(Convert.ToUInt16(doseInternal));
        }
        /// <summary>
        /// 设置注入电流
        /// </summary>
        /// <param name="inject"></param>
        /// <returns></returns>
        public override bool ExecuteInjection(ushort inject)
        {
            return (CurrentBetratronProrocol as BetratronProtocolForFH).SendCommandInjectionCurrent(inject);
        }
        /// <summary>
        /// 设置曝光时间
        /// </summary>
        /// <param name="ExposureTime"></param>
        /// <returns></returns>
        public override bool SendCommandExposureTime(ushort ExposureTime)
        {
            return (CurrentBetratronProrocol as BetratronProtocolForFH).SendCommandExposureTime(ExposureTime);
        }
        /// <summary>
        /// IGBT温度
        /// </summary>
        /// <returns></returns>
        public override string GetIGBTTemp()
        {
            return (CurrentBetratronProrocol as BetratronProtocolForFH).FHParameter.IGBTTransistors.ToString();
        }
        /// <summary>
        /// 晶闸管温度
        /// </summary>
        /// <returns></returns>
        public override string GetThyristor()
        {
            return (CurrentBetratronProrocol as BetratronProtocolForFH).FHParameter.Thyristor.ToString();
        }
        /// <summary>
        /// 注入电流
        /// </summary>
        /// <returns></returns>
        public override string GetActureInject()
        {
            return (CurrentBetratronProrocol as BetratronProtocolForFH).FHParameter.InjecttionCurrent.ToString();
        }
        /// <summary>
        /// 脉冲转换器温度
        /// </summary>
        /// <returns></returns>
        public override string GetPulseConverterTemperature()
        {
            return (CurrentBetratronProrocol as BetratronProtocolForFH).FHParameter.PulseConverter.ToString();
        }
        /// <summary>
        /// 辐射器温度
        /// </summary>
        /// <returns></returns>
        public override string GetRadiatorTemperature()
        {
            return (CurrentBetratronProrocol as BetratronProtocolForFH).FHParameter.Radiator.ToString();
        }
        /// <summary>
        /// 灯丝DAC温度
        /// </summary>
        /// <returns></returns>
        public override string GetDACValueOfFilament()
        {
            return ((CurrentBetratronProrocol as BetratronProtocolForFH).FHParameter.DACOfFilament / 150).ToString();
        }
        /// <summary>
        /// 注入DAC值
        /// </summary>
        /// <returns></returns>
        public override string GetInjectDACValue()
        {
            return (CurrentBetratronProrocol as BetratronProtocolForFH).FHParameter.DACOfInjector.ToString();
        }
        /// <summary>
        /// 约束器DAC值
        /// </summary>
        /// <returns></returns>
        public override string GetConstrainerDACValue()
        {
            return (CurrentBetratronProrocol as BetratronProtocolForFH).FHParameter.DACOfContractor.ToString();
        }
        /// <summary>
        /// 最大剂量率搜素进度
        /// </summary>
        /// <returns></returns>
        public override string MaximumDoseRateSearchRrogress()
        {
            return (CurrentBetratronProrocol as BetratronProtocolForFH).FHParameter.RSTProcess.ToString();
        }
        /// <summary>
        /// 剂量率
        /// </summary>
        /// <returns></returns>
        public override string DoseRate()
        {
            return ((CurrentBetratronProrocol as BetratronProtocolForFH).FHParameter.dr.Doserate / 100.00).ToString();
        }
        /// <summary>
        /// 获取加速器工作状态
        /// </summary>
        /// <returns></returns>
        public override WorkingJob GetWorkingJob()
        {
            if (base.CurrentBetratronProrocol == null) return WorkingJob.WJ_NULL;
            return (CurrentBetratronProrocol as BetratronProtocolForFH).FHParameter.BoostingSystemWorkStatus;
        }
        public override bool IsReady()
        {
            if (CurrentBetratronProrocol == null) return false;
            if (!CurrentBetratronProrocol.IsConnect())
            {
                return false;
            }
            if (WJB == WorkingJob.FH_KeyOff || WJB == WorkingJob.WJ_NULL || WJB == WorkingJob.FH_MainMagneticOvercurrent 
                || WJB == WorkingJob.FH_MainMagneticOvervoltage)
            {
                return false;
            }
            return true;
        }
        public override string GetCurrentEquipmentModel()
        {
            return "FH_Betratron";
        }
        public override string GetCurrentEquipmentVersion()
        {
            return "1.0.0";
        }
        public override bool SendCommandWaitWorkStop()
        {
            return (CurrentBetratronProrocol as BetratronProtocolForFH).SendCommandWaitWorkStop();
        }
        /// <summary>
        /// 设置单双能
        /// </summary>
        /// <param name="EnergyMode"></param>
        /// <returns></returns>
        public override bool SendCommandEnergyMode(ushort EnergyMode)
        {
            return (CurrentBetratronProrocol as BetratronProtocolForFH).SendCommandEnergyMode(EnergyMode);
        }
        public override bool SendCommandAfterRayWorkStatus(ushort RayAfterStatus)
        {
            return (CurrentBetratronProrocol as BetratronProtocolForFH).SendCommandAfterRayWorkStatus(RayAfterStatus);
        }

        /// <summary>
        /// 在泛华加速器到达预热之后，发送出束指令
        /// </summary>
        /// <returns></returns>
        public override bool SendCommandRadiationOn()
        {
            return (CurrentBetratronProrocol as BetratronProtocolForFH).SendCommandRadiationOn();
        }

        public override void SwitchEngerHAndI(bool CheckOrNot, bool isShowMessageBox = true, bool isSetEnergy = true)
        {
            BoostingModel HMC = BoostingModelList.FirstOrDefault(q => q.Name == "HeightEnergyMC");
            BoostingModel H = BoostingModelList.FirstOrDefault(q => q.Name == "HeightEnergy");
            BoostingModel LMC = BoostingModelList.FirstOrDefault(q => q.Name == "LowerEnergyMC");
            BoostingModel L = BoostingModelList.FirstOrDefault(q => q.Name == "LowerEnergy");

            if (CheckOrNot)
            {
                if (isSetEnergy)
                {
                    bool SendResult = BoostingControllerManager.GetInstance().SendCommandEnergyMode(0);
                    BoostingControllerManager.GetInstance().SetHMC(Convert.ToUInt16(H.Value));
                    BoostingControllerManager.GetInstance().SetHMCNum(Convert.ToUInt16(HMC.Value));
                    BoostingControllerManager.GetInstance().SetLMC(Convert.ToUInt16(L.Value));
                    BoostingControllerManager.GetInstance().SetLMCNum(Convert.ToUInt16(LMC.Value));
                    Thread.Sleep(100);
                    DetectorService.GetInstance().SX_SetEnergy(0);
                }
            }
            else
            {
                if (isSetEnergy)
                {
                    bool SendResult = BoostingControllerManager.GetInstance().SendCommandEnergyMode(1);
                    BoostingControllerManager.GetInstance().SetHMC(Convert.ToUInt16(H.Value));
                    BoostingControllerManager.GetInstance().SetHMCNum(Convert.ToUInt16(HMC.Value));
                    BoostingControllerManager.GetInstance().SetLMC(Convert.ToUInt16(L.Value));
                    BoostingControllerManager.GetInstance().SetLMCNum(Convert.ToUInt16(LMC.Value));
                    Thread.Sleep(100);
                    DetectorService.GetInstance().SX_SetEnergy(1);
                }
            }
            StopRay();
            sbb.SaveConfigDataModel(BoostingModelList);
        }
    }
}
