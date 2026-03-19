using BG_Entities;
using BG_Services;
using BG_WorkFlow;
using BGCommunication;
using BGModel;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static CMW.Common.Utilities.Common;

namespace CMW.Boosting.Russia_BetratronBoostingController
{
    [Export("Boosting", typeof(IEquipment))]
    [CustomExportMetadata(1, "Russia_BetratronBoosting_Module", "CGN. Begood", "zhuzhiwu", "1.0.0")]
    public class BetratronBoostingController : BoostingController<BetratronBoostingController>
    {
        ObservableCollection<BoostingModel> BoostingModelList = new ObservableCollection<BoostingModel>();
        Self_BoostSettingBLL sbb = new Self_BoostSettingBLL(controlVersion);
        static bool isPreHot = false;
       
        public BetratronBoostingController()
        {
            BoostingModelList.Clear();
            sbb.GetBoostingModelList(controlVersion).ToList().ForEach(q => BoostingModelList.Add(q));
            CommonDeleget.BetratronStopPrehotEvent += StopRay;
            CommonDeleget.BetratronRayEvent += Ray;
        }
        public override void Load(ICommonProtocol commonProtocol)
        {
            base.CurrentBetratronProrocol = commonProtocol;
        }
        public override void Load()
        {
            base.CurrentBetratronProrocol = new BetatronProtocol();
            base.CurrentBetratronProrocol.WaitRecTick = 150;
            ConnectInterface client = new SerialClient(ConfigServices.GetInstance().localConfigModel.SerialPort, ConfigServices.GetInstance().localConfigModel.BaudRate,
                ConfigServices.GetInstance().localConfigModel.Parity, ConfigServices.GetInstance().localConfigModel.ByteSize, ConfigServices.GetInstance().localConfigModel.StopBit);
            base.CurrentBetratronProrocol.InitConnection(ref client);
            base.CurrentBetratronProrocol.Connect();
        }
        /// <summary>
        /// 初始化加速器协议 并没有被调用
        /// </summary>
        /// <param name="commonProtocol"></param> commonProtocol
        public override void InitBetratronProcol()
        {
            base.CurrentBetratronProrocol = new BetatronProtocol();
            base.CurrentBetratronProrocol.WaitRecTick = 150;
            ConnectInterface client = new CSharpSerialClient(ConfigServices.GetInstance().localConfigModel.SerialPort,
                ConfigServices.GetInstance().localConfigModel.BaudRate, ConfigServices.GetInstance().localConfigModel.Parity, ConfigServices.GetInstance().localConfigModel.ByteSize,  ConfigServices.GetInstance().localConfigModel.StopBit);
            base.CurrentBetratronProrocol.InitConnection(ref client);
            base.CurrentBetratronProrocol.Connect();
        }
        /// <summary>
        /// 出束 
        /// </summary>
        public override void Ray()
        {
            BoostingModel volModel = BoostingModelList.FirstOrDefault(q => q.Name == "InjectionCurrent");
            BoostingModel HMC = BoostingModelList.FirstOrDefault(q => q.Name == "HeightEnergyMC");
            BoostingModel H = BoostingModelList.FirstOrDefault(q => q.Name == "HeightEnergy");
            BoostingModel LMC = BoostingModelList.FirstOrDefault(q => q.Name == "LowerEnergyMC");
            BoostingModel L = BoostingModelList.FirstOrDefault(q => q.Name == "LowerEnergy");
            if (volModel.Equals(string.Empty))
            {
                CommonDeleget.MessageBoxActionAction("电流不能为空");
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
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(60);//这里停51毫秒的目的是让停止预热的线程跳出来
                if (!isPreHot)
                {
                    return;
                }
                DeviceI di = new DeviceI();
                HAndL mhl = new HAndL();

                {
                    string RealValue = volModel.IsUseDefalut.ToLower() == "true" ? volModel.ActureValue : volModel.Value;
                    //Debug.WriteLine("-------------------------ganggang_dianliu-----------------------" + RealValue);

                    (base.CurrentBetratronProrocol as BetatronProtocol).WarmUp();
                    Thread.Sleep(170);
                    (base.CurrentBetratronProrocol as BetatronProtocol).Execute(Convert.ToInt32(RealValue), ref di);
                }
                

                Thread.Sleep(3000);
                string HValue = H.IsUseDefalut.ToLower() == "true" ? H.ActureValue : H.Value;
                string LValue = L.IsUseDefalut.ToLower() == "true" ? L.ActureValue : L.Value;
                string HMCValue = HMC.IsUseDefalut.ToLower() == "true" ? HMC.ActureValue : HMC.Value;
                string LMCValue = LMC.IsUseDefalut.ToLower() == "true" ? LMC.ActureValue : LMC.Value;
                //Debug.WriteLine("-------------------------ganggang_HL-----------------------" + HValue + " | " + LValue + "|" + HMCValue + "|" + LMCValue);
                ExecuteHandI(Convert.ToInt32(HValue), Convert.ToInt32(HMCValue),
                    Convert.ToInt32(LValue), Convert.ToInt32(LMCValue));
                Thread.Sleep(3000);
                {
                    Thread.Sleep(100);
                    (base.CurrentBetratronProrocol as BetatronProtocol).OutputBeam();
                }
            });
            sbb.SaveConfigDataModel(BoostingModelList,controlVersion);
        
        }
        /// <summary>
        /// 停束
        /// </summary>
        public override void StopRay()
        {
            isPreHot = false;
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(101);//这里停101毫秒的目的是让预热的线程跳出来
                while (WJB != WorkingJob.WJ_StopBeam)
                {
                    if (isPreHot)
                    {
                        return;
                    }
                    Thread.Sleep(50);
                    (base.CurrentBetratronProrocol as BetatronProtocol).StopBeam();
                }
            });
        }
        /// <summary>
        /// 预热中
        /// </summary>
        public override bool WarmUp()
        {
            return false;
        }
        /// <summary>
        /// 预热毕
        /// </summary>
        public override bool WarmUpEnd()
        {
            return false;
        }
        /// <summary>
        /// 加速预热
        /// </summary>
        /// <returns></returns>
        public override string GetRayAndPreviewHot()
        {
            switch (WJB)
            {
                case WorkingJob.WJ_SearchMaxDoseRate:
                    return "OutOfBeam";
                case WorkingJob.WJ_StopBeam:
                    return "UnPreheat";
                case WorkingJob.WJ_WarmUp:
                    return "PreheatEnding";
                case WorkingJob.WJ_OutBeam:
                case WorkingJob.WJ_NewOutBeam:
                    return "OutOfBeam";
                case WorkingJob.WJ_NULL:
                    return "NULL";
                case WorkingJob.WJ_ReadyWarm:
                    return "Ready";
                case WorkingJob.WJ_KeyNotOpen:
                    return "KeyOff";
                default:
                    return "NULL";
            }
            return "NULL";
        }
        /// <summary>
        /// 查询状态
        /// </summary>
        public override void Inquire()
        {
            if (base.CurrentBetratronProrocol == null) return;
            if (!base.CurrentBetratronProrocol.IsConnect())
            {
                base.CurrentBetratronProrocol?.Connect();
                return;
            }
            (CurrentBetratronProrocol as BetatronProtocol).InqurDose(ref Dose);
            Thread.Sleep(70);
            (CurrentBetratronProrocol as BetatronProtocol).InqurIandT(ref TandI, ref WJB);
        }
        /// <summary>
        /// 是否处于出束
        /// </summary>
        /// <returns></returns>
        public override bool IsRayOut()
        {
            return WJB == WorkingJob.WJ_OutBeam || WJB == WorkingJob.WJ_NewOutBeam;
        }
        /// <summary>
        /// 复位
        /// </summary>
        /// <returns></returns>
        public override bool Reset()
        {
           return (CurrentBetratronProrocol as BetatronProtocol).Reset();
        }
        /// <summary>
        /// 设置电流
        /// </summary>
        public override bool SetCurrentFlows(double CurrentFlows)
        {
            return false;
        }
        /// <summary>
        /// 设置高能脉冲个数
        /// </summary>
        public override bool SetHMC(double HLC)
        {
            return false;
        }
        /// <summary>
        /// 设置低能脉冲
        /// </summary>
        public override bool SetLMC(double LMC)
        {
            return false;
        }
        /// <summary>
        /// 设置高能脉冲个数
        /// </summary>
        public override bool SetHMCNum(double LMCNum)
        {
            return false;
        }
        /// <summary>
        /// 设置低能脉冲个数
        /// </summary>
        public override bool SetLMCNum(double LMCNum)
        {
            return false;
        }
        public bool ExecuteHandI(int H, int HMC, int L, int LMC)
        {
            return (base.CurrentBetratronProrocol as BetatronProtocol).ExecuteHandI(H, HMC, L, LMC);
        }
        public override void SwitchEngerHAndI(bool CheckOrNot, bool isShowMessageBox = true, bool isSetEnergy = true)
        {
            BoostingModel volModel = BoostingModelList.FirstOrDefault(q => q.Name == "InjectionCurrent");
            BoostingModel HMC = BoostingModelList.FirstOrDefault(q => q.Name == "HeightEnergyMC");
            BoostingModel H = BoostingModelList.FirstOrDefault(q => q.Name == "HeightEnergy");
            BoostingModel LMC = BoostingModelList.FirstOrDefault(q => q.Name == "LowerEnergyMC");
            BoostingModel L = BoostingModelList.FirstOrDefault(q => q.Name == "LowerEnergy");
            /*
            if (CheckOrNot)
            {
                H.Value = "75"; L.Value = "75";
                HMC.Value = "1"; LMC.Value = "1";
                volModel.Value = "97";
                if (isSetEnergy)
                {
                    ImageImportDll.SX_SetEnergy(ImageImportDll.intPtr, 0);
                }
            }
            else
            {
                H.Value = "75"; L.Value = "40";
                HMC.Value = "1"; LMC.Value = "1";
                volModel.Value = "97";
                if (isSetEnergy)
                {
                    ImageImportDll.SX_SetEnergy(ImageImportDll.intPtr, 1);
                }
            }
            */
            string HValue = H.IsUseDefalut.ToLower() == "true" ? H.ActureValue : H.Value;
            string LValue = L.IsUseDefalut.ToLower() == "true" ? L.ActureValue : L.Value;
            string HMCValue = HMC.IsUseDefalut.ToLower() == "true" ? HMC.ActureValue : HMC.Value;
            string LMCValue = LMC.IsUseDefalut.ToLower() == "true" ? LMC.ActureValue : LMC.Value;

            if (HValue == null || LValue == null)
            {
                return;
            }
            if (isSetEnergy) {
                if (HValue == LValue)
                {
                    ImageImportDll.SX_SetEnergy(ImageImportDll.intPtr, 0);
                } else
                {
                    ImageImportDll.SX_SetEnergy(ImageImportDll.intPtr, 1);
                }
            }
            
            (base.CurrentBetratronProrocol as BetatronProtocol).ExecuteHandI(Convert.ToInt32(HValue), Convert.ToInt32(HMCValue),
                  Convert.ToInt32(LValue), Convert.ToInt32(LMCValue));
            StopRay();
            if (CheckOrNot)
            {
                if(isShowMessageBox)
                {
                    CommonDeleget.MessageBoxActionAction("由于切换成被动模式，请确认加速器钥匙开关处于远程状态，并需要重新预热！");
                }
            }
            else
            {
                if (isShowMessageBox)
                {
                    CommonDeleget.MessageBoxActionAction("由于切换成主动模式，请确认加速器钥匙开关处于本地状态！");
                }
            }
            sbb.SaveConfigDataModel(BoostingModelList,controlVersion);
        }
        /// <summary>
        /// IGBT温度
        /// </summary>
        /// <returns></returns>
        public override string GetIGBTTemp()
        {
            return ((CurrentBetratronProrocol as BetatronProtocol).mTI.T[0] - 123).ToString();
        }
        /// <summary>
        /// 晶闸管温度
        /// </summary>
        /// <returns></returns>
        public override string GetThyristor()
        {
            return ((CurrentBetratronProrocol as BetatronProtocol).mTI.T[1] - 123).ToString();
        }
        /// <summary>
        /// 注入电流
        /// </summary>
        /// <returns></returns>
        public override string GetActureInject()
        {
            return ((CurrentBetratronProrocol as BetatronProtocol).mTI.DI.Inow).ToString();
        }
        /// <summary>
        /// 脉冲转换器温度
        /// </summary>
        /// <returns></returns>
        public override string GetPulseConverterTemperature()
        {
            return ((CurrentBetratronProrocol as BetatronProtocol).mTI.T[2] - 123).ToString();
        }
        /// <summary>
        /// 辐射器温度
        /// </summary>
        /// <returns></returns>
        public override string GetRadiatorTemperature()
        {
            return ((CurrentBetratronProrocol as BetatronProtocol).mTI.T[3] - 123).ToString();
        }
        /// <summary>
        /// 灯丝DAC温度
        /// </summary>
        /// <returns></returns>
        public override string GetDACValueOfFilament()
        {
            return string.Empty;
        }
        /// <summary>
        /// 注入DAC值
        /// </summary>
        /// <returns></returns>
        public override string GetInjectDACValue()
        {
            return string.Empty;
        }
        /// <summary>
        /// 约束器DAC值
        /// </summary>
        /// <returns></returns>
        public override string GetConstrainerDACValue()
        {
            return string.Empty;
        }
        /// <summary>
        /// 最大剂量率搜素进度
        /// </summary>
        /// <returns></returns>
        public override string MaximumDoseRateSearchRrogress()
        {
            return (CurrentBetratronProrocol as BetatronProtocol).mDR.CircuitMaxEnergy.ToString();
        }
        /// <summary>
        /// 剂量率
        /// </summary>
        /// <returns></returns>
        public override string DoseRate()
        {
            return ((CurrentBetratronProrocol as BetatronProtocol).mDR.Doserate / 100.0).ToString();
        }
        public override TandI GetTandI()
        {
            return (CurrentBetratronProrocol as BetatronProtocol).mTI;
        }
        public override WorkingJob GetWorkingJob()
        {
            if (CurrentBetratronProrocol == null) return WorkingJob.WJ_NULL;
            return (CurrentBetratronProrocol as BetatronProtocol).mWorkingJob;
        }
        public override bool IsReady()
        {
            if (CurrentBetratronProrocol == null) return false;
            if (WJB == WorkingJob.WJ_KeyNotOpen || WJB == WorkingJob.WJ_EmergencyPressOn)
            {
                return false;
            }
            if ((CurrentBetratronProrocol as BetatronProtocol).mTI.H != 0x00)
            {
                return false;
            }
            return true;
        }
        public override string GetCurrentEquipmentModel()
        {
            return "Russia_Betratron";
        }
        public override string GetCurrentEquipmentVersion()
        {
            return "1.0.0";
        }
    }
}