using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BGCommunication
{
    //发送命令类型
    public enum CommandFun
    {
        CF_InqurStatus = 0,
        CF_InqurIandT = 1,
        CF_InqurDose = 2,
        CF_WarmUp = 3,
        CF_SetI = 4,
        CF_SetHAndL = 8,
        CF_OutputBeam = 5,
        CF_StopBeam = 6,
        CF_ReSet = 7,
        /// <summary>
        /// 剂量率
        /// </summary>
        CF_DoseRate = 9,
        /// <summary>
        /// 曝光时间
        /// </summary>
        CF_ExposureTime=10,
        /// <summary>
        /// 内部剂量
        /// </summary>
        CF_Dose_Internal=11,
        /// <summary>
        /// 注入电流
        /// </summary>
        CF_InjectionCurrent=12,
        /// <summary>
        /// IGBT温度
        /// </summary>
        CF_IGBTTransistors=13,
        /// <summary>
        /// 晶体管温度
        /// </summary>
        CF_Thyristor=14,
        /// <summary>
        /// 脉冲转换器温度
        /// </summary>
        CF_PulseConverter=15,
        /// <summary>
        /// 辐射器温度
        /// </summary>
        CF_Radiator=16,
        /// <summary>
        /// 灯丝DAC值
        /// </summary>
        CF_DACOfFilament=17,
        /// <summary>
        /// 注入DAC值
        /// </summary>
        CF_DACOfInjector=18,
        /// <summary>
        /// 约束器DAC值
        /// </summary>
        CF_DACOfContractor=19,
        /// <summary>
        /// 最大剂量率搜索进度
        /// </summary>
        CF_RSTProcess=20,
        /// <summary>
        /// 加速器系统工作状态
        /// </summary>
        CF_BoostingSystemWorkStatus=21,
        /// <summary>
        /// 读取主磁电压
        /// </summary>
        CF_MainMarnetVol = 22,
        /// <summary>
        /// 读取系统状态参数
        /// </summary>
        CF_SystemStatus=23,
        /// <summary>
        /// 设置主磁工作频率模式
        /// </summary>
        CF_SetMainMarnetWorkFre= 24,
        /// <summary>
        /// 设置辐射完成后工作状态
        /// </summary>
        CF_SetRayEndingWorkStatus = 25,
        /// <summary>
        /// 设置待机预热暂停模式
        /// </summary>
        CF_SetWaitMachineStopMode = 26,
        /// <summary>
        /// 进行最大剂量率搜寻
        /// </summary>
        CF_SetMaxRayDoseFind = 27,
        /// <summary>
        /// 停止辐射
        /// </summary>
        CF_SetStopRaying = 28,
        /// <summary>
        /// 开始辐射
        /// </summary>
        CF_SetStartRaying = 29,
        /// <summary>
        /// 设置高能脉冲个数
        /// </summary>
        CF_SetHighPulsees = 30,
        /// <summary>
        /// 设置低能能脉冲
        /// </summary>
        CF_SetLowPules = 32,
        /// <summary>
        /// 设置低能
        /// </summary>
        CF_SetLowEnergy = 31,
        /// <summary>
        /// 设置高能
        /// </summary>
        CF_SetHighEnergy = 33,
        /// <summary>
        /// 设置能量模式
        /// </summary>
        CF_SetEnergy = 42,
        /// <summary>
        /// 设置注入电流
        /// </summary>
        CF_SetInjectionCurrent = 34,
        /// <summary>
        /// 设置内部剂量
        /// </summary>
        CF_SetDoseInternal = 35,
        /// <summary>
        /// 设置曝光时间
        /// </summary>
        CF_SetExposureTime = 36,
        /// <summary>
        /// 任务下发获取任务编号
        /// </summary>
        CIR_GetTaskId = 37,
        /// <summary>
        /// 提交图片信息
        /// </summary>
        CIR_SubmitImageInfo = 38,
        /// <summary>
        /// 提交Rfid信息
        /// </summary>
        CIR_SubmitRfidInfo = 42,
        /// <summary>
        /// 发送设备
        /// </summary>
        CIR_SendEquipment = 39,
        /// <summary>
        /// 上传报警信息
        /// </summary>
        CIR_UploadAlarm = 40,
        /// <summary>
        /// 提交本地参数
        /// </summary>
        CIR_UploadLocalParamater = 40,
        /// <summary>
        /// 发送扫描失败的任务
        /// </summary>
        CIR_SendFaildScanTask = 41,
        /// <summary>
        /// 发送点位
        /// </summary>
        TotalEquipmentSystemReady = 42
    }

    public enum DataBlock
    {
        M_Block = 0x83,
        I_Block = 0x81,
        Q_Block = 0x82,
        DB_Block = 0x84,
        T_Block = 0x1D,
        C_Block = 0x1c,
        V_Block = 0x84
    }

    //和ARM进行通讯
    public enum CommandARMFun
    {
        /// <summary>
        /// 握手
        /// </summary>
        ARM_HAND = 0,
        /// <summary>
        /// 开始扫描
        /// </summary>
        ARM_STARTSCAN = 1,
        /// <summary>
        /// 结束扫描
        /// </summary>
        ARM_ENDSCAN = 2,
        /// <summary>
        /// 收到结果
        /// </summary>
        ARM_RECVRESULT = 2
    }
    //和ARM进行通讯
    public enum CommandDoseFun
    {
        /// <summary>
        /// GetDose
        /// </summary>
        DOSE_GET = 0,
    }
    //和ARM进行通讯
    public enum CommandRFIDun
    {
        /// <summary>
        /// GetCarriageIdentification
        /// </summary>
        CarriageIdentification_GET = 0,
        /// <summary>
        /// GetCarriageIdentification
        /// </summary>
        CarriageIdentification_SyncTime = 0,
    }

    //当前工作状态
    public enum WorkingJob
    {
        /// <summary>
        /// 状态空
        /// </summary>
        WJ_NULL = -1,
        /// <summary>
        /// 状态停束
        /// </summary>
        WJ_StopBeam = 0x00,
        /// <summary>
        /// 预热状态
        /// </summary>
        WJ_WarmUp = 0x80,
        /// <summary>
        /// 搜索最打剂量
        /// </summary>
        WJ_SearchMaxDoseRate = 0xC1,
        /// <summary>
        /// 出束状态
        /// </summary>
        WJ_OutBeam = 0xC4,

        /// <summary>
        /// 出束状态
        /// </summary>
        WJ_NewOutBeam = 0xC7,

        /// <summary>
        /// 就绪状态
        /// </summary>
        WJ_ReadyWarm = 0xC5,
        /// <summary>
        /// 钥匙未开
        /// </summary>
        WJ_KeyNotOpen = 0x04,
        /// <summary>
        /// 急停按钮按下
        /// </summary>
        WJ_EmergencyPressOn = 0x05,
        /// <summary>
        /// 主磁过流
        /// </summary>
        FH_MainMagneticOvercurrent = 0x8000,
        /// <summary>
        /// 主磁过压
        /// </summary>
        FH_MainMagneticOvervoltage = 0x4000,
        /// <summary>
        /// 涡流电场过流
        /// </summary>
        FH_EddyCurrentElectricFieldOvercurrent = 0x2000,
        /// <summary>
        /// 系统准备完毕——加速器无故障且准备好进入工作
        /// </summary>
        FH_SystemReady = 0x1000,
        /// <summary>
        /// 辐射开始按钮按下后进入 10S 延时程序
        /// </summary>
        FH_10SecondReady = 0x0100,
        /// <summary>
        /// 辐射中——加速器已开启且产生辐射能量，剂量率为最大值
        /// </summary>
        FH_Radiationing = 0x0001,
        /// <summary>
        ///  辐射开启，寻求最大值——加速器已开启且产生辐射能量，但处理器正寻找最大值
        /// </summary>
        FH_RadiationOpenFindMax = 0x0002,
        /// <summary>
        /// 进入待机预热暂停模式（50Hz、无辐射）
        /// </summary>
        FH_WaitWramup50 = 0x0004,
        /// <summary>
        /// 辐射关闭，暂停模式——加速器在暂停模式中运行
        /// </summary>
        FH_RadiationCloseOfDose = 0x0010,
        /// <summary>
        /// 因时间到辐射关闭——预置时间到后，终止工作
        /// </summary>
        FH_TimeOutRayClose = 0x0020,
        /// <summary>
        /// 因剂量到辐射关闭——当到达预置的剂量后终止工作
        /// </summary>
        FH_StopOfDoseMax = 0x0040,
        /// <summary>
        /// 系统不工作，钥匙关闭——操作面板上的钥匙开关处于关闭状态
        /// </summary>
        FH_KeyOff = 0x0080,
        /// <summary>
        /// 最大剂量量搜索完成,辐射关闭
        /// </summary>
        FH_StopOfMaxDoseFindCompleted = 0x0200,
        /// <summary>
        /// 因剂量到辐射关闭——预置辐射剂量到后，关闭正常工作进入待机模式
        /// </summary>
        FH_StopOfPresetRadiationDose = 0x0044,
        /// <summary>
        /// 因时间到辐射关闭——预置时间到后，终止工作
        /// </summary>
        FH_StopWorkOfPresetTime = 0x0050,
        /// <summary>
        /// 因时间到辐射关闭——预置时间到后，关闭正常工作进入待机模式
        /// </summary>
        FH_StopInWaitModeOfPresetTime = 0x0024,
        /// <summary>
        /// 4,最大剂量率搜索完成，进入待机模式
        /// </summary>
        FH_StopOfMaxDoseFind = 0x0204,
        /// <summary>
        /// 最大剂量率搜索完成，进入关闭模式
        /// </summary>
        FH_CloseModeOfMaxDoseFind = 0x0210,
        /// <summary>
        /// 待机状态下进入 10S 预延时
        /// </summary>
        FH_WaitModeOf10Second = 0x0104,
    }

   

    //电流数据
    public class DeviceI
    {
        public byte Iset;
        public byte Inow;
    }
    //高能低能
    public class HAndL
    {
        public int H;
        public int HMC;
        public int L;
        public int LMC;
    }
    //报警标志
    public enum PoliceFlag
    {
        None = 0x09,
        NormalARM = 0x00,
        Gamma = 0x01,
        Neutrom = 0x02,
        GammaAndNeutrom = 0x03,
    }

    // 电流和温度
    public class TandI
    {
        /// <summary>
        /// 温度
        /// </summary>
        public byte[] T = new byte[4] { 0, 0, 0, 0 };
        /// <summary>
        /// 故障
        /// </summary>
        public byte H;
        /// <summary>
        /// 电流
        /// </summary>
        public DeviceI DI;
    }

    //剂量率数据
    public class DoseRate
    {
        public UInt16 InternalDose;
        public UInt16 RemoteDose;
        public UInt16 CircuitMaxEnergy;
        public UInt16 Doserate;
    }

    /// <summary>
    /// 泛华betratron加速器参数
    /// </summary>
    public class FH_BetratronParamter
    {
        /// <summary>
        /// 剂量率
        /// </summary>
        public DoseRate dr = new DoseRate();
        /// <summary>
        /// 曝光时间
        /// </summary>
        public UInt16 ExposureTime = 0;
        /// <summary>
        /// 注入电流
        /// </summary>
        public UInt16 InjecttionCurrent = 0;
        /// <summary>
        /// IGBT温度，单位℃
        /// </summary>
        public UInt16 IGBTTransistors = 0;
        /// <summary>
        /// 晶闸管温度
        /// </summary>
        public UInt16 Thyristor = 0;
        /// <summary>
        /// 脉冲转换器温度
        /// </summary>
        public UInt16 PulseConverter = 0;
        /// <summary>
        /// 辐射器温度
        /// </summary>
        public UInt16 Radiator = 0;
        /// <summary>
        /// 灯丝DAC温度
        /// </summary>
        public UInt16 DACOfFilament = 0;
        /// <summary>
        /// 注入DAC值
        /// </summary>
        public UInt16 DACOfInjector = 0;
        /// <summary>
        /// 约束器 DAC 值
        /// </summary>
        public UInt16 DACOfContractor = 0;
        /// <summary>
        /// 最大剂量率搜索进度
        /// </summary>
        public UInt16 RSTProcess = 0;
        /// <summary>
        /// 加速器系统工作状态
        /// </summary>
        public WorkingJob BoostingSystemWorkStatus = 0;
        /// <summary>
        /// 反馈电压
        /// </summary>
        public UInt16 FeedbackVal = 0;
        /// <summary>
        /// 乘以该系数将 FeedbackVal 转换为单位为 V 的电压
        /// </summary>
        public UInt16 FeedbackValCoeff = 0;
        
    }


    #region FH-Betratron Enum
    public enum InquireType
    {
        /// <summary>
        /// 剂量率
        /// </summary>
        DoseRate,
        /// <summary>
        /// 曝光时间
        /// </summary>
        ExposureTime,
        /// <summary>
        /// 内部剂量
        /// </summary>
        Dose_Internal,
        /// <summary>
        /// 注入电流
        /// </summary>
        InjectionCurrent,
        /// <summary>
        /// IGBT温度
        /// </summary>
        IGBTTransistors,
        /// <summary>
        /// 晶体管温度
        /// </summary>
        Thyristor,
        /// <summary>
        /// 脉冲转换器温度
        /// </summary>
        PulseConverter,
        /// <summary>
        /// 辐射器温度
        /// </summary>
        Radiator,
        /// <summary>
        /// 灯丝DAC值
        /// </summary>
        DACOfFilament,
        /// <summary>
        /// 注入DAC值
        /// </summary>
        DACOfInjector,
        /// <summary>
        /// 约束器DAC值
        /// </summary>
        DACOfContractor,
        /// <summary>
        /// 最大剂量率搜索进度
        /// </summary>
        RSTProcess,
        /// <summary>
        /// 加速器系统工作状态
        /// </summary>
        BoostingSystemWorkStatus
    }
    #endregion

}
