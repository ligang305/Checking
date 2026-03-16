using System;

namespace BG_Entities
{
    #region 车载
    public enum Command
    {

        /// <summary>
        /// 系统启动/暂停
        /// M21.0
        /// </summary>
        SysetmStartEnd,
        /// <summary>
        /// 驱动装置就绪
        /// </summary>
        DriveReady,
        /// <summary>
        /// 高压按钮（断高压）
        /// </summary>
        StartHighVoltage,
        /// <summary>
        /// 停束/出束
        /// </summary>
        StopRay,
        /// <summary>
        /// 工作站就绪（出入道闸打开）
        /// </summary>
        StationReady,
        /// <summary>
        /// 急停
        /// </summary>
        SpeedStop,
        /// <summary>
        /// 审图结束
        /// </summary>
        ImageEnd,
        /// <summary>
        /// 开始扫描
        /// </summary>
        StartScan,
        /// <summary>
        /// 自动扫描
        /// </summary>
        AutoStartScan,
        /// <summary>
        /// 停止扫描
        /// </summary>
        StopScan,
        /// <summary>
        /// 自动模式/手动模式
        /// </summary>
        AutoMode,
        /// <summary>
        /// 维护模式
        /// </summary>
        Repair,
        /// <summary>
        /// 驱动装置前进
        /// </summary>
        CarGo,
        /// <summary>
        /// 驱动装置后退
        /// </summary>
        CarBack,
        /// <summary>
        /// 驱动装置停止
        /// </summary>
        CarStop,
        /// <summary>
        /// 系统关机
        /// </summary>
        SystemClose,
        /// <summary>
        /// 故障复位
        /// </summary>
        HitchReset,
        /// <summary>
        /// 出束装置故障复位
        /// </summary>
        RayReset,
        /// <summary>
        /// 拖车故障复位
        /// </summary>
        RrailerReset,
        /// <summary>
        /// 探测器故障复位
        /// </summary>
        DetectorRest,
        /// <summary>
        /// 急停故障复位
        /// </summary>
        CrashStopReset,
        /// <summary>
        /// 道闸拉杆故障复位
        /// </summary>
        BarrierReset,
        /// <summary>
        /// 地磅故障复位
        /// </summary>
        WeighBridge,
        /// <summary>
        /// 设置扫描方向:向前
        /// </summary>
        Preview,
        /// <summary>
        /// 设置扫描方向:向后
        /// </summary>
        Back,
        /// <summary>
        /// 加速器打火反馈
        /// </summary>
        BoostringFire,
        /// <summary>
        /// 本底采集结束
        /// </summary>
        DarkCollectionEnd,
        /// <summary>
        /// 空气采集结束
        /// </summary>
        LightCollectionEnd,
        /// <summary>
        /// 图像采集结束
        /// </summary>
        ImageCollectionEnd,
        /// <summary>
        /// AFC投入
        /// </summary>
        AFC,
        /// <summary>
        /// AFC手自动
        /// </summary>
        AFCAutoHand,
        /// <summary>
        /// 加速器远程上电
        /// </summary>
        SpeedAddElectron,
        /// <summary>
        /// 低压按钮
        /// </summary>
        LowVoltage,
        /// <summary>
        /// 设定单双能
        /// </summary>
        SettingSingleDouble,
        /// <summary>
        /// 设定高低能
        /// </summary>
        SettingHighLow,
        /// <summary>
        /// 外触发
        /// </summary>
        OutTouch,
        /// <summary>
        /// 磁触发
        /// </summary>
        MagneticTouch,
        /// <summary>
        /// 超低剂量
        /// </summary>
        SuperLow,
        /// <summary>
        /// 快检模式
        /// </summary>
        FastCheckMode,
        /// <summary>
        /// 转台角度选择0°
        /// </summary>
        ZeroAngle,
        /// <summary>
        /// 转台角度选择80°
        /// </summary>
        EightyAngle,
        /// <summary>
        /// 转台角度选择90°
        /// </summary>
        NinetyAngle,
        /// <summary>
        /// 转台角度选择0°
        /// </summary>
        NinetyFiveAngle,
        /// <summary>
        /// 设备防撞复位
        /// </summary>
        AntiCollisionReset,
        /// <summary>
        /// 手动开始矫正
        /// </summary>
        MTCorrect,
        /// <summary>
        /// 前转向左转
        /// </summary>
        ForwordTunLeft,
        /// <summary>
        /// 前转向回正
        /// </summary>
        ForwordGoStraght,
        /// <summary>
        /// 前转向右转
        /// </summary>
        ForwordTunRight,
        /// <summary>
        /// 后转向左转
        /// </summary>
        BackwordTunLeft,
        /// <summary>
        /// 后转向回正
        /// </summary>
        BackwordGoStraght,
        /// <summary>
        /// 后转向右转
        /// </summary>
        BackwordTunRight,
        /// <summary>
        /// 小角度一键偏转
        /// </summary>
        SmallAngleOneKeyDeflection,
        /// <summary>
        /// 小角度一键复位
        /// </summary>
        SmallAngleOneKeyReset,
        /// <summary>
        /// 加速器90度角旋转
        /// </summary>
        Accelerator90DegreeAngularRotation,
        /// <summary>
        /// 加速器75度角旋转
        /// </summary>
        Accelerator75DegreeAngularRotation,
        /// <summary>
        /// 臂架90度角旋转
        /// </summary>
        Arm90DegreeAngleRotation,
        /// <summary>
        /// 臂架75度角旋转
        /// </summary>
        Arm75DegreeAngleRotation,
        /// <summary>
        /// 臂架0度角旋转
        /// </summary>
        Arm0DegreeAngleRotation,
        /// <summary>
        /// 补光灯打开
        /// </summary>
        LightOpen,
        /// <summary>
        /// 百叶窗闭合
        /// </summary>
        ShutterOpenOrClose,
        /// <summary>
        /// 加速器就绪
        /// </summary>
        BoostReay,
        /// <summary>
        /// 加速器预热
        /// </summary>
        BoostPreHot,
        /// <summary>
        /// 加速器出束中
        /// </summary>
        BoostRaying,
        /// <summary>
        /// BetratronWaitPreHot
        /// </summary>
        BetratronWaitPreHot,
        /// <summary>
        /// BetratronStart
        /// </summary>
        BetratronStart,
        /// <summary>
        /// 登录模块
        /// </summary>
        LoginModules,
        /// <summary>
        /// 动态频率设置完成
        /// </summary>
        DynamicFSetting,
        /// <summary>
        /// 扫描高度
        /// </summary>
        ScanHeight,
        /// <summary>
        /// 是否启用电子围栏
        /// </summary>
        IsShowElctronicFence,
        /// <summary>
        /// 车厢读取复位
        /// </summary>
        CarriageSegmentationReset,
        /// <summary>
        /// 矫正模式
        /// </summary>
        CalibrationMode,

        /// <summary>
        /// 增强扫描开始像素
        /// </summary>
        EnhanceStartPosition,
        /// <summary>
        /// 增强扫描结束像素
        /// </summary>
        EnhanceEndPosition,
        /// <summary>
        /// 增强扫描总
        /// </summary>
        EnhanceTotalPosition,
        /// <summary>
        /// 增强扫描
        /// </summary>
        EnhanceScan,

        #region BS
        /// <summary>
        /// 顶舱预热
        /// </summary>
        TopCabinWarmUp,
        /// <summary>
        /// 进线舱预热
        /// </summary>
        InLetCabinWarmUp,
        /// <summary>
        /// 侧舱预热
        /// </summary>
        SideCabinWarmUp,

        InLetFlyingSaucer,
        TopCabinFlyingSaucer,
        SideCabinFlyingSaucer,
        /// <summary>
        /// 上海背散和盛世对接
        /// </summary>
        BarierGate,
        #endregion

        ///测试模式
        TestMode
    }
    #endregion

    public class RoleList
    {
        public static string jjAdmin = "jjAdmin";
        public static string jjczy = "jjczy";
    }

    public class Section
    {
        public static string SOFT = "SOFT";
        public static string SERVER = "SERVER";
        public static string SCAN = "SCAN";
        public static string Serial = "Serial";
        public static string Freeze = "Freeze";
        public static string Dose = "Dose";
        public static string PLC = "PLC";
        public static string RFID = "RFID";
        public static string BS = "BS";
    }

    public class TaskList
    {
        public static string Alarm = "Alarm";
        public static string UpdateCodeToWeb = "UpdateCodeToWeb";
        public static string CheckBoostHitchCode = "CheckBoostHitchCode";
        public static string CheckBetratron_FH = "CheckBetratron_FH";
        public static string PollBoostReset = "PollBoostReset";
        public static string Timer = "Timer";
        public static string Inquire = "Inquire";
        public static string BSInquire = "BSInquire";
        public static string BSInquireParamater = "BSInquireParamater";
        public static string AccelatorInquire = "AccelatorInquire";
        public static string InquireParamater = "InquireParamater";
        public static string BoostingFire = "BoostingFire";
        public static string DynamicGetFreez = "DynamicGetFreez";
        public static string PlcConnection = "PlcConnection";
        public static string BSPlcConnection = "BSPlcConnection";
        public static string loopUploadScanImage = "loopUploadScanImage";

        public static string CMWConnectionAlarm = "CMWConnectionAlarm";
        public static string PlcAlarm = "PlcAlarm";
        public static string BoostingAlarm = "BoostingAlarm";
        public static string ScanAlarm = "ScanAlarm";
        public static string StopAlarm = "StopAlarm";
        public static string WhatchBoosting = "WhatchBoosting";
        public static string RefalashDose = "RefalashDose";
        public static string RefalshStatus = "RefalshStatus";
        public static string MainSystemReady = "MainSystemReady";

        public static string UploadServerLogs = "UploadServerLogs";
        public static string UploadBsImage = "UploadBsImage";
    }

    public class TaskAlarmStatus
    {
        public static string Normal = "0";
        public static string Alarm = "1";
    }


    public class TaskCode
    {
        /// <summary>
        /// CMW连接
        /// </summary>
        public static string CMWConnectionAlarm = "8000";
        /// <summary>
        /// PLC状态
        /// </summary>
        public static string PlcAlarm = "20100";
        /// <summary>
        /// 加速器报警
        /// </summary>
        public static string BoostingAlarm = "20400";
        /// <summary>
        /// 扫描报警
        /// </summary>
        public static string ScanAlarm = "20300";
        /// <summary>
        /// 急停报警
        /// </summary>
        public static string StopAlarm = "20200";
        /// <summary>
        /// 加速器时间
        /// </summary>
        public static string BoostingTime = "05";
        /// <summary>
        /// 探测器时间
        /// </summary>
        public static string DectorTime = "06";
        /// <summary>
        /// 室内温度
        /// </summary>
        public static string IndoorTemp = "07";
        /// <summary>
        /// 室外温度
        /// </summary>
        public static string OuterDoorTemp = "08";
        /// <summary>
        /// 设备开机时间
        /// </summary>
        public static string EquipmentOnTime = "13";
        /// <summary>
        /// 设备关机时间
        /// </summary>
        public static string EquipmentOffTime = "10";
        /// <summary>
        /// 设备运行总时长
        /// </summary>
        public static string EquipmentTotalTime = "11";
        /// <summary>
        /// 设备运行总天数
        /// </summary>
        public static string EquipmentTotalDay = "12";
        /// <summary>
        /// 加速器舱温度
        /// </summary>
        public static string BoostingRoomTemp = "20401";
        /// <summary>
        /// 加速器打火
        /// </summary>
        public static string BoostingFire = "20402";
    }

    public class ForeColorKey
    {
        public static string GreenPoliceLight = "GreenPoliceLight";
        public static string RedPoliceLight = "RedPoliceLight";
        public static string YellowPoliceLight = "YellowPoliceLight";
    }


    public class Modules
    {
        /// <summary>
        /// 模块配置
        /// </summary>
        public const string ModulesConfigModule = "ModulesConfigModule";
        /// <summary>
        /// 背散模块配置
        /// </summary>
        public const string BSModulesConfigModule = "BSModulesConfigModule";
        /// <summary>
        /// 加速器模块
        /// </summary>
        public const string Car_BoostSettingModule = "Car_BoostSettingModule";
        /// <summary>
        /// 车体控制
        /// </summary>
        public const string CarCantileverModule = "CarCantileverModule";
        /// <summary>
        /// 自行走车体控制
        /// </summary>
        public const string SelfWorkingHandSettingModule = "SelfWorkingHandSettingModule";
        /// <summary>
        /// 自行走道闸控制
        /// </summary>
        public const string SelfWorkingCarComponentModule = "SelfWorkingCarComponentModule";

        /// <summary>
        /// 组合移动车体控制
        /// </summary>
        public const string CombinedMovement_HandSettingModule = "CombinedMovement_HandSettingModule";

        /// <summary>
        /// Betatron 加速器设置模块
        /// </summary>
        public const string SelfWorking_BoostingControlModule = "SelfWorking_BoostingControlModule";
        /// <summary>
        /// 快检 加速器设置模块
        /// </summary>
        public const string FastCheck_BoostSettingModule = "FastCheck_BoostSettingModule";

        /// <summary>
        /// 组合移动 加速器设置模块
        /// </summary>
        public const string CombinedMovement_BoostSetting = "CombinedMovement_BoostSetting";
        /// <summary>
        /// 加速器设置模块
        /// </summary>
        public const string BegoodBoostSettingModule = "BegoodBoostSettingModule";
        /// <summary>
        /// 行进方向、扫描模式设置
        /// </summary>
        public const string HandSettingModule = "HandSettingModule";
        /// <summary>
        /// 泛华加速器设置模块
        /// </summary>
        public const string FH_BetratronBoostingSetting = "FH_BetratronBoostingSetting";
        /// <summary>
        /// 俄罗斯加速器设置模块
        /// </summary>
        public const string RussiaBetratronAccelatorSetting = "RussiaBetratronAccelatorSetting";
        /// <summary>
        /// VJ 射线源加速器设置模块
        /// </summary>
        public const string VJ_BetratronBoostingSetting = "VJ_BetratronBoostingSetting";
        /// <summary>
        /// 通用设置模块
        /// </summary>
        public const string CommonSettingModule = "CommonSettingModule";
        /// <summary>
        /// 工程模式设置
        /// </summary>
        public const string RepirModule = "RepirModule";
        /// <summary>
        /// 软件详情设置模块
        /// </summary>
        public const string SoftDetailsModules = "SoftDetailsModules";

        /// <summary>
        /// 标准化信息显示监控面板
        /// </summary>
        public const string StandardStatusPanel = "StandardStatusPanel";

        /// <summary>
        /// 组合移动 Betratron加速器设置模块
        /// </summary>
        public const string CombinedMovement_FHBetratronBoostSetting = "CombinedMovement_BetratronBoostingSettingModule";
        /// <summary>
        /// 快检 流程控制
        /// </summary>
        public const string FastCheck_FlowSettingModule = "FastCheck_FlowSettingModule";
        /// <summary>
        /// 自行走 流程控制
        /// </summary>
        public const string SelfWorking_FlowSettingModule = "SelfWorking_FlowSetting";

        /// <summary>
        /// 探测器监控
        /// </summary>
        public const string DetectorBoardMonitorModule = "DetectorBoardMonitorModule";


        /// <summary>
        /// 探测器板监控
        /// </summary>
        public const string DetratronBoardModule = "DetectorBoardMonitorModule";

        /// <summary>
        /// 公共设置模块，包含车速阈值、剂量阈值
        /// </summary>
        public const string FlowSettingModule = "FlowSettingModule";

        /// <summary>
        /// 公共设置模块，包含车速阈值、剂量阈值 用于背散
        /// </summary>
        public const string BSFlowSettingModule = "BSFlowSettingModule";
        

        /// <summary>
        /// 工程设置模块
        /// </summary>
        public const string RepairSetting = "RepairSettingModule";
        

        /// <summary>
        /// 探测器板监控
        /// </summary>
        public const string FHBetratronStatusCheckModule = "FHBetratronStatusCheckModule";

        /// <summary>
        /// 控制频率
        /// </summary>
        public const string FastCheck_CarSpeedFreezeModule = "FastCheck_CarSpeedFreezeModule";
        /// <summary>
        /// 剂量仪器IP地址
        /// </summary>
        public const string DoseSettingModule = "DoseSettingModule";
        /// <summary>
        /// 背散设备IP地址连接
        /// </summary>
        public const string BS2000SettingModule = "BS2000SettingModule";
        /// <summary>
        /// 背散设备值类型监测
        /// </summary>
        public const string BSValueMonitor = "BSValueMonitorModule";
        
        /// <summary>
        /// 监测模块
        /// </summary>
        public const string ControlModule = "ControlModule";

        /// <summary>
        /// 背散监测模块
        /// </summary>
        public const string BSControlModule = "BSControlModule";
        /// <summary>
        /// 参数设置模块
        /// </summary>
        public const string ParamConfigModule = "ParamConfigModule";
        /// <summary>
        /// 插件模块
        /// </summary>
        public const string PluginsModule = "ModulesConfigModule";
        /// <summary>
        /// 审图像模块
        /// </summary>
        public const string TrialImageModules = "TrialImageModules";
        /// <summary>
        /// 日志模块
        /// </summary>
        public const string LogsModule = "LogsModule";

    }

    /// <summary>
    /// 全局通用的用户登录类
    /// </summary>
    public class LoginUserStatus
    {
        public string LoginUser { get; set; }
        public string LoginUserPwd { get; set; }
        public LoginStatus LoginStatus { get; set; }
        public string LoginTime { get; set; }
        /// <summary>
        /// 登录名称
        /// </summary>
        public string LoginMode { get; set; }
        /// <summary>
        /// Token
        /// </summary>
        public string sccessToken { get; set; }
        /// <summary>
        /// 登录角色v
        /// </summary>
        public string LoginCode { get; set; }
    }
    /// <summary>
    /// PLC单个查询类型
    /// </summary>
    public enum InqureType : int
    {
        IString = 0,
        IUInt32 = 1,
    }
    public class FtpUser
    {
        public string FtpUserName { get; set; }
        public string FtpPassword { get; set; }

        public string IpAddr { get; set; }
    }

    public class LocalConfigModel
    {
        public LoginUserStatus Login { get; set; } = new LoginUserStatus() { LoginStatus = LoginStatus.NoLogin };

        public FtpUser ftp_user { get; set; } = new FtpUser() { };
        public string SystemLock { get; set; } = string.Empty;
        public bool IsRemUser { get; set; } = false;
        public DateTime CorrentData = DateTime.MinValue;
        public string TrialImageMode { get; set; } = "";
        public string FreezeMode { get; set; } = "";
        public string LANGUAGE { get; set; } = string.Empty;
        public string FontSize { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string Port { get; set; } = string.Empty;
        public string ArmIpAddress { get; set; } = string.Empty;
        public int ArmPort { get; set; } = 23600;
        public string ArmLocalIpAddress { get; set; } = string.Empty;
        public int ArmLocalPort { get; set; } = 23605;
        public string KeepLive { get; set; } = string.Empty;
        public string Heart { get; set; } = string.Empty;

        public bool DefalutScanMode { get; set; } = true;
        public string ScanIpAddress { get; set; } = string.Empty;
        public string ScanPort { get; set; } = string.Empty;
        public string ScanImagePort { get; set; } = string.Empty;
        public bool IsPartition { get; set; } = false;

        public bool IsAES { get; set; } = true;
        public bool IsLogin { get; set; } = true;
        public int SerialPort { get; set; } = 3;
        public int BaudRate { get; set; } = 9600;
        public int ByteSize { get; set; } = 8;
        public int Parity { get; set; } = 1;
        public int StopBit { get; set; } = 0;

        public string Server { get; set; } = string.Empty;

        public string ParamaterServerPort { get; set; } = string.Empty;

        public string LogsServer { get; set; } = string.Empty;

        public string UgrServer { get; set; } = string.Empty;
        public string ServerSocket { get; set; } = string.Empty;
        public string EquipmentNo { get; set; } = string.Empty;
        public string ChannelNo { get; set; } = string.Empty;
        public string CMW_Version { get; set; } = string.Empty;
        public string CMW_FastCheckBayNumber { get; set; } = string.Empty;
        public string EquipmentAddress { get; set; } = string.Empty;
        public string EquipmentLength { get; set; } = string.Empty;
        public string BoostingVersionAddress { get; set; } = string.Empty;
        public string BoostingVersionLength { get; set; } = string.Empty;
        public string BoostingRunTimeAddress { get; set; } = string.Empty;
        public string BoostingRunTimeAddressLength { get; set; } = string.Empty;
        public string BoostingRayTimeAddress { get; set; } = string.Empty;
        public string BoostingRayTimeAddressLength { get; set; } = string.Empty;



        #region 新增海关接口
        public string EquipmentOnTImeAddress { get; set; } = string.Empty;
        public string EquipmentOnTImeAddressLength { get; set; } = string.Empty;

        public string EquipmentOffTImeAddress { get; set; } = string.Empty;
        public string EquipmentOffTImeAddressLength { get; set; } = string.Empty;

        public string EquipmentTotalDayAddress { get; set; } = string.Empty;
        public string EquipmentTotalDayAddressLength { get; set; } = string.Empty;

        public string EquipmentTotalTimeAddress { get; set; } = string.Empty;
        public string EquipmentTotalTimeAddressLength { get; set; } = string.Empty;
        #endregion

        public int VerticalSpatialResolution { get; set; } = 1;
        public int AdjustableParameters { get; set; } = 1;
        public string Freeze { get; set; }
        public bool IsSaveImage { get; set; }

        public bool IsDetectorTestMode { get; set; } = false;
        public bool IsShowDirection { get; set; }

        public bool IsShowDualDirection { get; set; }

        public bool IsShowElctronicFence { get; set; }
        
        public bool IsUserBS { get; set; }

        public bool IsEnabledSocketToServer { get; set; }

        public bool IsSendDose { get; set; } = true;
        public string Dose1Position { get; set; } = "60";
        public string Dose2Position { get; set; } = "64";
        public string Dose3Position { get; set; } = "68";

        public string InquireStartPosition { get; set; } = "DB0.0";
        public short CurrentPosition { get; set; } = 0;
        public DateTime LastCalibrationTime { get; set; } = DateTime.Now;
        public string RFIDAddress { get; set; } = "127.0.0.1";
        public uint RFIDPort { get; set; } = 1570;


        public string BSInquireStartPosition { get; set; } = "DB54.0.0";
        public string BSAddress { get; set; } = "127.0.0.1";
        public uint BSPort { get; set; } = 102;
    }


    public class UploadImageType
    {
        public static string scan { get; set; } = "scan";
        public static string scanJpg { get; set; } = "scanJpg";
        public static string scanbgp { get; set; } = "scanbgp";
        public static string scanHemd { get; set; } = "scanHemd";

        public static string scanBS { get; set; } = "scanBS";
        public static string scanJpgBS { get; set; } = "scanJpgBS";
        public static string scanbgpBS { get; set; } = "scanbgpBS";
        public static string scanHemdBS { get; set; } = "scanHemdBS";
    }


    public class UploadImageOrderType
    {
        public static string SubmitScanData { get; set; } = "SubmitScanData";

        public static string SubmitScanBSData { get; set; } = "SubmitScanBSData";
        
        public static string SaveScanExceptionMessage { get; set; } = "SaveScanExceptionMessage";
        public static string GetTaskIdByDevice { get; set; } = "GetTaskIdByDevice";
        public static string AddAlarm { get; set; } = "AddAlarm";
        public static string GetChannelInfoByDeviceId { get; set; } = "GetChannelInfoByDeviceId";
        public static string Login { get; set; } = "Login";
        public static string GetInstructions { get; set; } = "GetInstructions";
        public static string SetInstructions { get; set; } = "SetInstructions";
        public static string GetClientConfig { get; set; } = "GetClientConfig";

        public static string ModifyRadiationPassData { get; set; } = "ModifyRadiationPassData";
        /// <summary>
        /// socket 提交H986设备图像
        /// </summary>
        public static string UPK001 { get; set; } = "UPK001";
        /// <summary>
        /// socket 取消H986设备任务
        /// </summary>
        public static string UPK002 { get; set; } = "UPK002";
        /// <summary>
        /// socket 提交背散图像记录
        /// </summary>
        public static string UPK003 { get; set; } = "UPK003";
        public static string ClientBind { get; set; } = "ClientBind";
        /// <summary>
        /// 任务下发
        /// </summary>
        public static string DNT001 { get; set; } = "DNT001";
        /// <summary>
        /// 时间下发
        /// </summary>
        public static string DNT002 { get; set; } = "DNT002";
        /// <summary>
        /// 参数下发
        /// </summary>
        public static string DNJ003 { get; set; } = "DNJ003";
        /// <summary>
        /// 增强扫描数据下发
        /// </summary>
        public static string EnhanceScan004 { get; set; } = "EnhanceScan004";
        /// <summary>
        /// 获取设备配置
        /// </summary>
        public static string getDeviceConfig { get; set; } = "getDeviceConfig";
        /// <summary>
        /// 上传日志服务
        /// </summary>
        public static string saveDeviceLog { get; set; } = "saveDeviceLog";
        /// <summary>
        /// 获取设备配置
        /// </summary>
        public static string updateDeviceConfig { get; set; } = "updateDeviceConfig";
        /// <summary>
        /// 上传参数
        /// </summary>
        public static string UPT001 { get; set; } = "UPT001";
        /// <summary>
        /// 上传RFID数据
        /// </summary>
        public static string RFID001 { get; set; } = "UPTBN0100";
        /// <summary>
        /// 提交点位
        /// </summary>
        public static string TotalEquipmentSystemReady { get; set; } = "TotalEquipmentSystemReady";
        /// <summary>
        /// 收道闸开关信号
        /// </summary>
        public static string BarrierGateStatus { get; set; } = "BarrierGateStatus";
    }

    public class CMWFontSize
    {
        public static string Small = "Small";
        public static string Normal = "Normal";
        public static string NormalMiddle = "NormalMiddle";
        public static string Middle = "Middle";
        public static string Big = "Big";
        public static string SuperBig = "SuperBig";
    }


    public class ImageUploadStatus
    {

        /// <summary>
        /// 传输文件服务器失败
        /// </summary>
        public static string PutFTPFaild { get; set; } = "PutFTPFaild";
        /// <summary>
        /// 提交图片信息失败
        /// </summary>
        public static string CommitImageInfoFaild { get; set; } = "CommitImageInfoFaild";
        /// <summary>
        /// 提交失败
        /// </summary>
        public static string CommitFaild { get; set; } = "CommitFaild";
        /// <summary>
        /// 未提交
        /// </summary>
        public static string UnCommited { get; set; } = "UnCommited";

        /// <summary>
        /// 上传中
        /// </summary>
        public static string UpLoading { get; set; } = "UpLoading";
        /// <summary>
        /// 正在获取任务ID
        /// </summary>
        public static string GetTaskIDing { get; set; } = "GetTaskIDing";
        /// <summary>
        /// 正在等待推送文件给文件服务器
        /// </summary>
        public static string WaitingPutFTPing { get; set; } = "WaitingPutFTPing";
        /// <summary>
        /// 正在推送止FTP文件服务器
        /// </summary>
        public static string PutFTPing { get; set; } = "PutFTPing";
        /// <summary>
        /// 正在上传图片信息
        /// </summary>
        public static string UpSubmitImageInfoingLoading { get; set; } = "UpSubmitImageInfoingLoading";
        /// <summary>
        /// 上传完成
        /// </summary>
        public static string UploadComplete { get; set; } = "UploadComplete";
        /// <summary>
        /// 提交状态
        /// </summary>
        public static string SubmitStatus { get; set; } = "SubmitStatus";
    }

    public enum PlCSendType
    {
        /// <summary>
        /// 需要传字节位置和0/1，出现情况下拉框仅限2个值
        /// </summary>
        OneByteOneOrTwo,
        /// <summary>
        /// 需要传入字节位置默认一,出现情况，例如下拉框
        /// </summary>
        OneByteDefaultOne,
        /// <summary>
        /// 需要传入字节其实位置和文本值
        /// </summary>
        StartBytePositionDefaultText,
        /// <summary>
        /// 三个值三个地址
        /// </summary>
        OneByteThree,
        /// <summary>
        /// 一个值先发true 后发false
        /// </summary>
        OneByteTrueAndFalse,
    }

    /// <summary>
    /// 登陆状态枚举
    /// </summary>
    public enum LoginStatus
    {
        /// <summary>
        /// 登录中
        /// </summary>
        Logining,
        /// <summary>
        /// 登录成功
        /// </summary>
        Success,
        /// <summary>
        /// 登录失败
        /// </summary>
        Faild,
        /// <summary>
        /// 未授权
        /// </summary>
        UnAuthorized,
        /// <summary>
        /// 用户不存在
        /// </summary>
        UnSuchUser,
        /// <summary>
        /// 密码错误
        /// </summary>
        PasswordError,
        /// <summary>
        /// 登录取消
        /// </summary>
        Cancel,
        /// <summary>
        /// 未登录
        /// </summary>
        NoLogin,
        /// <summary>
        /// 连接超时
        /// </summary>
        TimeOut,
        /// <summary>
        /// 网络链接失败
        /// </summary>
        LinkServerFaild,
        /// <summary>
        /// FTP服务器连接失败
        /// </summary>
        FTPServerFaild,
        /// <summary>
        /// 授权码无效
        /// </summary>
        InvalidAuthorizationCode,
        /// <summary>
        /// 授权码过期
        /// </summary>
        AuthorizationCodeExpired,
        /// <summary>
        /// 请输入授权码
        /// </summary>
        AuthorizationCodeNull
    }

    public enum HandStatus
    {
        /// <summary>
        /// 斜臂展开
        /// </summary>
        LinarcOpen,
        /// <summary>
        /// 横臂展开
        /// </summary>
        HorniacalOpen,
        /// <summary>
        /// 竖臂展开
        /// </summary>
        Vercatical,
        /// <summary>
        /// 都不展开
        /// </summary>
        Close,
        /// <summary>
        /// 车载展开
        /// </summary>
        CarOpen,
        /// <summary>
        /// 车载关闭
        /// </summary>
        CarClose,
        /// <summary>
        /// 车进入闸道口---正常状态
        /// </summary>
        In,
        /// <summary>
        /// 车扫图结束-- 正常状态
        /// </summary>
        Out,
        /// <summary>
        /// 正在扫描
        /// </summary>
        Checking,
        /// <summary>
        /// 快检小图
        /// </summary>
        SmallImage,
        /// <summary>
        /// 准备急停
        /// </summary>
        StopPressOn,
        /// <summary>
        /// 准备急停[鼠标移入]
        /// </summary>
        StopPressOnMouseEnter,
        /// <summary>
        /// 按下急停
        /// </summary>
        StopPressOff,
        /// <summary>
        /// 按下急停[鼠标移入]
        /// </summary>
        StopPressOffMouseEnter,
        /// <summary>
        ///自行走大图
        /// </summary>
        SelfWorkBigImage,
        /// <summary>
        ///自行走小图
        /// </summary>
        SelfWordSmallImage,
    }

    public enum ControlVersion
    {
        /// <summary>
        /// 宇通新车载
        /// </summary>
        BGV7000,
        /// <summary>
        /// 7700自主加速器版本
        /// </summary>
        BGV7700,
        /// <summary>
        /// 8000火车版本
        /// </summary>
        BGV8000,
        //车载
        Car,
        //快检
        FastCheck,
        //自行走
        SelfWorking,
        //组合移动 - 自主加速器;PLC控制出束
        CombinedMovement,
        //组合移动 - Betatron加速器，自己控制出束，PLC控制电流
        CombinedMovementBetatron,
        //乘用车
        PassengerCar,
        /// <summary>
        /// 背散
        /// </summary>
        BS,
        //绿通
        BGV5100,
        //绿通（泛华加速器）
        //BGV5100Russia
        BGV5100FH,
        //广西项目
        BGV6000BS
    }
    /// <summary>
    /// 点位标准化类型
    /// </summary>
    public enum PositionConfigType
    {
        OPosition,
        IPosition,
        DIntPosition,
        IntPosition,
        FloatPosition //FloatPosition
    }
    public enum ElectronDirection
    {
        //  前侧
        Fore,
        /// <summary>
        /// 后侧
        /// </summary>
        Back,
        /// <summary>
        /// 左侧
        /// </summary>
        Left,
        /// <summary>
        /// 右侧
        /// </summary>
        Right,
    }

    public class DataBaseType
    {
        public static string SqlLite = "SqlLite"; 
    }

    #region 静态的属性描述
    public class GlogbalBGModel
    {
        public static string BGCommonSettingsConfigs = "BGCommonSettingsConfigs";
        public static string BGCommonSettingConfig = "BGCommonSettingConfig";

        public static string CarCantileverModels = "CarCantileverModels";
        public static string CarCantileverModel = "CarCantileverModel";

        public static string BG_CAR_BoostSettingsConfigs = "BG_CAR_BoostSettingsConfigs";
        public static string BG_CAR_BoostSettingsConfig = "BG_CAR_BoostSettingsConfig";

        public static string BG_SELFWORKAUTOBOOSTINGCONFIGS = "BG_SELFWORKAUTOBOOSTINGCONFIGS";
        public static string BG_SELFWORKAUTOBOOSTINGCONFIG = "BG_SELFWORKAUTOBOOSTINGCONFIG";

        public static string BG_CAR_STATUS_CONFIGS = "BG_CAR_STATUS_CONFIGS";
        public static string BG_CAR_STATUS_CONFIG = "BG_CAR_STATUS_CONFIG";

        public static string BG_TREE_CONFIGS = "BG_TREE_CONFIGS";
        public static string BG_TREE_CONFIG = "BG_TREE_CONFIG";

        public static string BG_BOOSTING_CONFIGS = "BG_BOOSTING_CONFIGS";
        public static string BG_BOOSTING_CONFIG = "BG_BOOSTING_CONFIG";

        public static string BG_DOSE_CONFIGS = "BG_DOSE_CONFIGS";
        public static string BG_DOSE_CONFIG = "BG_DOSE_CONFIG";

        public static string BG_CARSPEED_CONFIGS = "BG_CARSPEED_CONFIGS";
        public static string BG_CARSPEED_CONFIG = "BG_CARSPEED_CONFIG";

        public static string BG_COMMAND_CONFIGS = "BG_COMMAND_CONFIGS";
        public static string BG_COMMAND_CONFIG = "BG_COMMAND_CONFIG";

        public static string ControlVersions = "ControlVersions";
        public static string Bg_ControlVersion = "Bg_ControlVersion";

        public static string BG_USERS = "Users";
        public static string BG_USER = "User";

        public static string BG_ROLES = "Roles";
        public static string BG_ROLE = "Role";

        public static string Languages = "Languages";
        public static string Language = "Language";

        public static string Fontsizes = "FontSizes";
        public static string Fontsize = "FontSize";

        public static string Sizes = "Sizes";
        public static string Size = "Size";
    }

    #endregion

    #region 配置文件枚举
    public class ParamConfigEnum
    {
        public static string language = "language";
        public static string fontsize = "fontsize";
        public static string IpAddress = "IpAddress";
        public static string Port = "Port";
        public static string ArmIpAddress = "ArmIpAddress";
        public static string ArmPort = "ArmPort";
        public static string ArmLocalIpAddress = "ArmLocalIpAddress";
        public static string ArmLocalPort = "ArmLocalPort";
        public static string KeepLive = "KeepLive";

        public static string Heart = "Heart";
        public static string IsRemberber = "IsRemberber";
        public static string SystemLock = "SystemLock";
        public static string CorrentData = "CorrentData";
        public static string TrailImageMode = "TrailImageMode";
        public static string FreezeMode = "FreezeMode";
        public static string EquipmentNo = "EquipmentNo";
        public static string CMW_Version = "CMW_Version";
        public static string CMW_FastCheckBayNumber = "CMW_FastCheckBayNumber";
        public static string IsSaveImage = "IsSaveImage";
        public static string IsDetectorTestMode = "IsDetectorTestMode";
        public static string IsShowDirection = "IsShowDirection";
        public static string IsShowDualDirection = "IsShowDualDirection";
        public static string IsEnabledSocketToServer = "IsEnabledSocketToServer";
        public static string IsShowElctronicFence = "IsShowElctronicFence";
        
        public static string Type = "Type";
        public static string Frequency = "Frequency";
        public static string EquipmentAddress = "EquipmentAddress";
        public static string EquipmentLength = "EquipmentLength";
        public static string BoostingVersionAddress = "BoostingVersionAddress";
        public static string BoostingVersionLength = "BoostingVersionLength";
        public static string BoostingRunTimeAddress = "BoostingRunTimeAddress";
        public static string BoostingRunTimeAddressLength = "BoostingRunTimeAddressLength";
        public static string BoostingRayTimeAddress = "BoostingRayTimeAddress";
        public static string BoostingRayTimeAddressLength = "BoostingRayTimeAddressLength";

        public static string EquipmentOffTImeAddress = "EquipmentOffTImeAddress";
        public static string EquipmentOffTImeAddressLength = "EquipmentOffTImeAddressLength";
        public static string EquipmentOnTImeAddress = "EquipmentOnTImeAddress";
        public static string EquipmentOnTImeAddressLength = "EquipmentOnTImeAddressLength";
        public static string EquipmentTotalDayAddress = "EquipmentTotalDayAddress";
        public static string EquipmentTotalDayAddressLength = "EquipmentTotalDayAddressLength";
        public static string EquipmentTotalTimeAddress = "EquipmentTotalTimeAddress";
        public static string EquipmentTotalTimeAddressLength = "EquipmentTotalTimeAddressLength";


        public static string PassWord = "PassWord";
        public static string User = "User";
        public static string IsAES = "IsAES";
        public static string IsLogin = "IsLogin";
        public static string Server = "Server";
        public static string SerialPort = "SerialPort";
        public static string Parity = "Parity";
        public static string BaudRate = "BaudRate";

        public static string ScanImagePort = "ScanImagePort";
        public static string ScanIpAddress = "ScanIpAddress";
        public static string ScanPort = "ScanPort";


        public static string StopBit = "StopBit";
        public static string ByteSize = "ByteSize";
        public static string VerticalSpatialResolution = "VerticalSpatialResolution";
        public static string AdjustableParameters = "AdjustableParameters";

        public static string Dose1 = "Dose1";
        public static string Dose2 = "Dose2";
        public static string Dose3 = "Dose3";
    }


    public class LogType
    {
        public static string Alarm = "Alarm";

        public static string ImageImportDllError = "ImageImportDllError";

        public static string ApplicationError = "ApplicationError";

        public static string ScanStep = "ScanStep";

        public static string NormalLog = "NormalLog";

        public static string Services = "Services";

        public static string SocketServices = "SocketServices";

        public static string EquipmentFailure = "EquipmentFailure";

        public static string EquipmentStatus = "EquipmentStatus";

        public static string SystemDebug = "SystemDebug";
    }

    /// <summary>
    /// 系统时间结构
    /// </summary>
    public struct SYSTEMTIME
    {
        public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfWeek;
        public ushort wDay;
        public ushort wHour;
        public ushort wMinute;
        public ushort wSecond;
        public ushort wMilliseconds;

        /// <summary>
        /// 从System.DateTime转换。
        /// </summary>
        /// <param name="time">System.DateTime类型的时间。</param>
        public void FromDateTime(DateTime time)
        {
            wYear = (ushort)time.Year;
            wMonth = (ushort)time.Month;
            wDayOfWeek = (ushort)time.DayOfWeek;
            wDay = (ushort)time.Day;
            wHour = (ushort)time.Hour;
            wMinute = (ushort)time.Minute;
            wSecond = (ushort)time.Second;
            wMilliseconds = (ushort)time.Millisecond;
        }
        /// <summary>
        /// 转换为System.DateTime类型。
        /// </summary>
        /// <returns></returns>
        public DateTime ToDateTime()
        {
            return new DateTime(wYear, wMonth, wDay, wHour, wMinute, wSecond, wMilliseconds);
        }
        /// <summary>
        /// 静态方法。转换为System.DateTime类型。
        /// </summary>
        /// <param name="time">SYSTEMTIME类型的时间。</param>
        /// <returns></returns>
        public static DateTime ToDateTime(SYSTEMTIME time)
        {
            return time.ToDateTime();
        }
    }
    #endregion
}
