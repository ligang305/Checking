using BG_Entities;
using BGCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG_Services
{
    public abstract class BoostingController<T>: BaseInstance<T>,IEquipment where T:class,new()
    {
        protected ICommonProtocol CurrentBetratronProrocol;
        //需要传出来的一个枚举值
        public static WorkingJob WJB = WorkingJob.WJ_NULL;
        //需要传出来的一个枚举值
        public static DoseRate Dose = new DoseRate();
        //电流和温度
        public static TandI TandI = new TandI();
        public virtual void Load(ICommonProtocol commonProtocol)
        {
            InitProcol(commonProtocol);
        }

        public virtual void Load()
        {
            InitBetratronProcol();
        }
        /// <summary>
        /// 初始化加速器协议
        /// </summary>
        /// <param name="commonProtocol"></param>
        public virtual void InitBetratronProcol( )
        {
        }
        /// <summary>
        /// 初始化加速器协议
        /// </summary>
        /// <param name="commonProtocol"></param>
        public virtual void InitProcol(ICommonProtocol commonProtocol)
        {
            CurrentBetratronProrocol = commonProtocol;
            //CurrentBetratronProrocol.WaitRecTick = 150;
            ////Log.GetDistance().WriteInfoLogs($@"正在调用{_cv.ToString()}Boosting Init方法");
            //ConnectInterface client;
            //if (controlVersion == ControlVersion.CombinedMovementBetatron)
            //{
            //    client = new CSharpSerialClient(localConfigModel.SerialPort, localConfigModel.BaudRate, localConfigModel.Parity, localConfigModel.ByteSize, localConfigModel.StopBit);
            //}
            //else
            //{
            //    client = new SerialClient(localConfigModel.SerialPort, localConfigModel.BaudRate, localConfigModel.Parity, localConfigModel.ByteSize, localConfigModel.StopBit);
            //}
            //CurrentBetratronProrocol.InitConnection(ref client);
            //CurrentBetratronProrocol.Connect();
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsConnection()
        {
            if (CurrentBetratronProrocol == null) return false;
            return CurrentBetratronProrocol.IsConnect();
        }

        public virtual void DisConnection()
        {
            CurrentBetratronProrocol?.DisConnect();
        }

        /// <summary>
        /// 判断是否处于出束状态
        /// </summary>
        /// <returns></returns>
        public virtual bool IsRayOut()
        {
            return false;
        }

        /// <summary>
        /// 出束 
        /// </summary>
        public virtual void Ray()
        {

        }
        /// <summary>
        /// 停束
        /// </summary>
        public virtual void StopRay()
        {

        }
        /// <summary>
        /// 预热
        /// </summary>
        public virtual bool WarmUp()
        {
            return false;
        }
        /// <summary>
        /// 预热毕
        /// </summary>
        public virtual bool WarmUpEnd()
        {
            return false;
        }
        public virtual bool Reset()
        {
            return false;
        }


        public virtual void Inquire()
        {
            if (CurrentBetratronProrocol == null) return;
            if (!CurrentBetratronProrocol.IsConnect())
            {
                CurrentBetratronProrocol.Connect();
                return ;
            }
        }

        /// <summary>
        /// 设置电流
        /// </summary>
        public virtual bool SetCurrentFlows(double CurrentFlows)
        {
            return false;
        }
        /// <summary>
        /// 设置高能脉冲个数
        /// </summary>
        public virtual bool SetHMC(double HLC)
        {
            return false;
        }

        /// <summary>
        /// 设置低能脉冲
        /// </summary>
        public virtual bool SetLMC(double LMC)
        {
            return false;
        }
        /// <summary>
        /// 设置高能脉冲个数
        /// </summary>
        public virtual bool SetHMCNum(double LMCNum)
        {
            return false;
        }
        /// <summary>
        /// 设置低能脉冲个数
        /// </summary>
        public virtual bool SetLMCNum(double LMCNum)
        {
            return false;
        }

        public virtual bool SetHighOrLowEnergy(bool SetOrCancel)
        {
            return false;
        }

        public virtual bool SetDoubleOrSigneEnergy(bool SetOrCancel)
        {
            return false;
        }

        public virtual bool SetHighAndLowPressure(bool SetHightOrLowPressure)
        {
            return false;
        }

        public virtual string GetRayAndPreviewHot()
        {
            return "UnPreheat";
        }
        public virtual string ReadDoubleOrSingle()
        {
            return "Null";
        }
        /// <summary>
        /// 读取能量
        /// </summary>
        /// <returns></returns>
        public virtual string ReadEnger()
        {
            return "Null";
        }
        public virtual string SearchScanMode()
        {
            return "Null";
        }

        public bool SendCommandMainMagWorkFreMode(ushort workMode)
        {
            return true;
        }

        public virtual bool SendCommandDoseInternal(ushort doseInternal)
        {
            return true;
        }

        public virtual bool ExecuteInjection(ushort inject)
        {
            return true;
        }

        public virtual bool SendCommandExposureTime(ushort ExposureTime)
        {
            return true;
        }
        public virtual void SwitchEngerHAndI(bool CheckOrNot, bool isShowMessageBox = true, bool isSetEnergy = true)
        {

        }

        public virtual string GetIGBTTemp()
        {
            return string.Empty;
        }

        public virtual string GetThyristor()
        {
            return string.Empty;
        }

        public virtual string GetActureInject()
        {
            return string.Empty;
        }

        public virtual string GetPulseConverterTemperature()
        {
            return string.Empty;
        }

        public virtual string GetRadiatorTemperature()
        {
            return string.Empty;
        }

        public virtual string GetDACValueOfFilament()
        {
            return string.Empty;
        }

        public virtual string GetInjectDACValue()
        {
            return string.Empty;
        }

        public virtual string GetConstrainerDACValue()
        {
            return string.Empty;
        }

        public virtual string MaximumDoseRateSearchRrogress()
        {
            return string.Empty;
        }

        public virtual string DoseRate()
        {
            return string.Empty;
        }
        public virtual TandI GetTandI()
        {
            return null;
        }
        public virtual WorkingJob GetWorkingJob()
        {
            return WorkingJob.WJ_NULL;
        }
        public virtual bool IsReady()
        {
            return false;
        }

        public virtual string GetCurrentEquipmentVersion()
        {
            return string.Empty;
        }

        public virtual string GetCurrentEquipmentModel()
        {
            return CurrentBetratronProrocol?.GetProtocolName();
        }

        public virtual bool SendCommandWaitWorkStop()
        {
            return false;
        }

        public virtual bool SendCommandRadiationOn()
        {
            return false;
        }
        public virtual bool SendCommandEnergyMode(ushort EnergyMode)
        {
            return false;
        }

        
        public virtual bool SendCommandAfterRayWorkStatus(ushort RayAfterStatus)
        {
            return false;
        }
    }
}
