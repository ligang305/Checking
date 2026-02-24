using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG_Entities
{
    public enum PLCPositionEnum
    {
        /// <summary>
        /// 无
        /// </summary>
        Null,
        /// <summary>
        /// 主系统就绪
        /// </summary>
        MainSystemReady,
        /// <summary>
        /// 射线源就绪
        /// </summary>
        RadiationSourceOrAcceleratorReady,
        /// <summary>
        /// 驱动装置就绪
        /// </summary>
        DriveReady,
        /// <summary>
        /// 探测器就绪
        /// </summary>
        DetectorReady,
        /// <summary>
        /// 安全联锁就绪
        /// </summary>
        SafetyInterlockReady,
        /// <summary>
        /// 高压就绪
        /// </summary>
        HighVoltageIndication,
        /// <summary>
        /// 预热中
        /// </summary>
        Preheating,
        /// <summary>
        /// 预热毕
        /// </summary>
        PreheatEnding,
        /// <summary>
        /// 雷达控制面板
        /// </summary>
        RadarControlPanel,
        /// <summary>
        /// 出束中
        /// </summary>
        OutOfBeam,
        /// <summary>
        /// 急停就绪
        /// </summary>
        StopReady,
        /// <summary>
        /// 设备前进
        /// </summary>
        EquipmentForward,
        /// <summary>
        /// 设备后退
        /// </summary>
        EquipmentBackward,
        /// <summary>
        /// 前进限位
        /// </summary>
        ForwardPosition,
        /// <summary>
        /// 舱体急停(入口)
        /// </summary>
        EnterCabinBodyStop,
        /// <summary>
        /// 舱体急停(出口)
        /// </summary>
        ExitCabinBodyStop,
        /// <summary>
        /// 竖臂急停(入口)
        /// </summary>
        EnterVertivalstop,
        /// <summary>
        /// 竖臂急停(出口)
        /// </summary>
        ExitVertivalstop,

        /// <summary>
        /// 控制柜远程
        /// </summary>
        RemoteControlCabinet,
        /// <summary>
        /// 后退限位
        /// </summary>
        BackPosition,
        /// <summary>
        /// 前进限位1
        /// </summary>
        ForwardLimit1,
        /// <summary>
        /// 前进限位2
        /// </summary>
        ForwardLimit2,
        /// <summary>
        /// 后退限位1
        /// </summary>
        BackwardLimit1,
        /// <summary>
        /// 后退限位2
        /// </summary>
        BackwardLimit2,
        /// <summary>
        /// 扫描方向
        /// </summary>
        ScanDirection,
        /// <summary>
        /// 雷达进入信号
        /// </summary>
        RadarEntrySignal,
        /// <summary>
        /// 车头急停
        /// </summary>
        EmergencyStopOfMainCabinHead,
        /// <summary>
        /// 车尾防撞
        /// </summary>
        EmergencyStopAtTheRearOfMainCabin,

        HeadCollisionAvoidance2,

        RearCollisionAvoidance1,

        RearCollisionAvoidance2,

        ChannelCollisionAvoidance1,

        ChannelCollisionAvoidance2,
        ChannelCollisionAvoidance3,
        ChannelCollisionAvoidance4,
        TravelingFrequencyNO1ConverterFaulty,
        TravelingFrequencyNO2ConverterFaulty,
        TravelingFrequencyNO3ConverterFaulty,
        TravelingFrequencyNO4ConverterFaulty,
        SteeringNO1InverterMalfunction,
        SteeringNO2InverterMalfunction,
        /// <summary>
        /// 故障复位
        /// </summary>
        HitchReset,
        /// <summary>
        /// 车头急停
        /// </summary>
        HeadCollisionAvoidance1,
        /// <summary>
        /// 
        /// </summary>
        EmergencyStopAtRearOfAuxiliaryCabin,
        /// <summary>
        /// 设备车尾急停
        /// </summary>
        EmergencyStopOfEquipmentCabin,
        /// <summary>
        /// 加速器门限位
        /// </summary>
        AcceleratorDoorLimit1,
        /// <summary>
        /// 加速器门限位2
        /// </summary>
        AcceleratorDoorLimit2,
        /// <summary>
        /// 调制器1
        /// </summary>
        Transducer1,
        /// <summary>
        /// 调制器2
        /// </summary>
        Transducer2,
        /// <summary>
        /// 调制器3
        /// </summary>
        Transducer3,
        /// <summary>
        /// 入口光栅
        /// </summary>
        EnterLight,
        /// <summary>
        /// 出口光栅
        /// </summary>
        ExitLight,
        /// <summary>
        /// 
        /// </summary>
        LocalOrFaraway,
        /// <summary>
        /// 雷达出口信号
        /// </summary>
        RadarExitSignal,
        /// <summary>
        /// 出口急停
        /// </summary>
        EmergencyStopOfAuxiliaryCabinHead,
        /// <summary>
        /// 拉绳急停
        /// </summary>
        RopeEmergencyStop,
        /// <summary>
        /// 控制室急停
        /// </summary>
        ContronRoomStop,
        /// <summary>
        /// 单双能
        /// </summary>
        DualEnergyIndication,
        /// <summary>
        /// 高能
        /// </summary>
        HighEnergyIndication,
        /// <summary>
        /// 低能
        /// </summary>
        LowEnergyIndication,
        /// <summary>
        /// 单能
        /// </summary>
        SingleEnergyIndication,
        /// <summary>
        /// 剂量报警
        /// </summary>
        DoseAlarm,
        /// <summary>
        /// 控制室剂量报警
        /// </summary>
        Dose1,
        /// <summary>
        /// 控制室剂量报警
        /// </summary>
        Dose2,
        /// <summary>
        /// 激光雷达故障
        /// </summary>
        LidarFault,
        /// <summary>
        /// 车辆反向行驶
        /// </summary>
        VehicleRunningInReverse,
        /// <summary>
        /// 通道内停车
        /// </summary>
        ParkingInPassageway,
        /// <summary>
        /// 加速器舱门
        /// </summary>
        BoostingDoor,
        /// <summary>
        /// 车速过快
        /// </summary>
        CarSpeedFast,
        /// <summary>
        /// 车速过慢
        /// </summary>
        CarSpeedSlow,
        /// <summary>
        /// 加速器总故障
        /// </summary>
        BoosttingHitch,
        /// <summary>
        /// 加速器打火
        /// </summary>
        BoostingFire,
        /// <summary>
        /// 水流量故障
        /// </summary>
        WaterFlowFailure,
        /// <summary>
        /// PLC和加速器
        /// </summary>
        PLCBoosting,
        /// <summary>
        /// 水流量故障
        /// </summary>
        LowAirPressureFault,
        /// <summary>
        /// X机头门故障
        /// </summary>
        XHeadDoorFailure,
        /// <summary>
        /// 太泵欠压故障
        /// </summary>
        TitaniumPumpUnderVoltagefault,
        /// <summary>
        /// 太泵25μ安故障
        /// </summary>
        TitaniumPump25μaOvercurrentFault,
        /// <summary>
        /// 激光联锁
        /// </summary>
        LaserInterlocking,
        /// <summary>
        /// 磁放电过流
        /// </summary>
        MagneticDischargeOvercurrent,
        /// <summary>
        /// 无触发保护
        /// </summary>
        NoTriggerProtection,
        /// <summary>
        /// 高压接触器
        /// </summary>
        HighVoltageContactorFault,
        /// <summary>
        /// 调制器门故障
        /// </summary>
        ModulatorDoorFailure,
        /// <summary>
        /// 高压断路器故障
        /// </summary>
        HighVoltageVircuitbreakerFailure,
        /// <summary>
        /// 加速器高压
        /// </summary>
        HighVoltageAccelerator,
        /// <summary>
        /// 中控钥匙联锁
        /// </summary>
        CentralKeyInterlock,
        /// <summary>
        /// 大厅门故障
        /// </summary>
        HallDoorFailure,
        /// <summary>
        /// 电源故障
        /// </summary>
        PowerFailure,
        /// <summary>
        /// 开关过温故障
        /// </summary>
        SwitchOverTemperatureFault,
        /// <summary>
        /// 变频器故障
        /// </summary>
        Transducer,
        /// <summary>
        /// 开始采图
        /// </summary>
        StartImage,
        /// <summary>
        /// 停止采图
        /// </summary>
        StopImageStr,
        /// <summary>
        /// 火车车厢分割
        /// </summary>
        CarriageSegmentation,
        /// <summary>
        /// 控制模式
        /// </summary>
        ControlMode,
        /// <summary>
        /// 扫描模式
        /// </summary>
        ScanMode,
        /// <summary>
        /// 抱闸打开
        /// </summary>
        BandTypeBrakeOpen,
        /// <summary>
        /// 开始暗矫正
        /// </summary>
        StartDark,
        /// <summary>
        /// 开始亮矫正
        /// </summary>
        StartAir,
        /// <summary>
        /// 扫描方向
        /// </summary>
        ScanForWard,
        /// <summary>
        /// 臂架状态
        /// </summary>
        HandStatus,
        /// <summary>
        /// 斜臂展开
        /// </summary>
        OpliqueHandOpenArrivePosition,
        /// <summary>
        /// 斜臂回收
        /// </summary>
        OpliqueHandCloseArrivePosition,
        /// <summary>
        /// 横臂展开
        /// </summary>
        CrossHandOpenPosition,
        /// <summary>
        /// 横臂回收
        /// </summary>
        CrossHandClosePosition,
        /// <summary>
        /// 斜臂展开
        /// </summary>
        VerticalOpenPosition,
        /// <summary>
        /// 斜臂回收
        /// </summary>
        VerticalClosePosition,
        /// <summary>
        /// 转台0度回收到位
        /// </summary>
        TurntableZeroDegreeRecoveryInPlace,
        /// <summary>
        /// 转台80度回收到位
        /// </summary>
        Turntable_80_DegreeRecoveryInPlace,
        /// <summary>
        /// 转台90度回收到位
        /// </summary>
        Turntable_90_DegreeRecoveryInPlace,
        /// <summary>
        /// 转台95度回收到位
        /// </summary>
        Turntable_95_DegreeRecoveryInPlace,
        /// <summary>
        /// 转台插销落到位
        /// </summary>
        TurntableLatchInPlace,
        /// <summary>
        /// 系统压力阀状态
        /// </summary>
        SystemPressureValveStatu,
        /// <summary>
        /// 回油滤芯堵塞
        /// </summary>
        ReturnFilterElementBlocked,
        /// <summary>
        /// 油温低
        /// </summary>
        OILTEMPERATURELOW,
        /// <summary>
        /// 油温高
        /// </summary>
        OILTEMPERATUREHeight,
        /// <summary>
        /// 液位高
        /// </summary>
        HIGHLEVEL,
        /// <summary>
        /// 液位低
        /// </summary>
        LOWLEVEL,
        /// <summary>
        /// 高压滤芯堵塞
        /// </summary>
        HighPressureFilterElementBlocked,
        /// <summary>
        /// 通讯正常
        /// </summary>
        NormalCommunication,
        /// <summary>
        /// PLC与软件通讯
        /// </summary>
        PLCWithSoftCommunication,
        /// <summary>
        /// PLC与加速器通讯
        /// </summary>
        PLCWithBoostringCommunication,
        /// <summary>
        /// 探测器与软件通讯
        /// </summary>
        ScanWithSoft,
        /// <summary>
        /// 分动器状态
        /// </summary>
        TransferCaseEngagementStatus,
        /// <summary>
        /// 分动器已啮合
        /// </summary>
        ChassisHandbrakeInspection,
        /// <summary>
        /// 分动器未啮合
        /// </summary>
        FrontTrafficLightStatus,
        /// <summary>
        /// 底盘车手刹检测
        /// </summary>
        BackTrafficLightStatus,
        /// <summary>
        /// 抱闸状态
        /// </summary>
        LightScreenSignal,
        /// <summary>
        /// 主束光电开关NO
        /// </summary>
        MainBeamPhotoelectricSwitch,
        /// <summary>
        /// 竖臂侧地感白线
        /// </summary>
        VerticalArmSideGroundSenseWhiteLine,
        /// <summary>
        /// 车体侧地感白线
        /// </summary>
        WhiteLineAtVehicleBodySide,
        /// <summary>
        /// 2.5m处漫反射光电
        /// </summary>
        DiffusePhotoelectricity,
        /// <summary>
        /// 出口处漫反射光电
        /// </summary>
        OutSideDiffusePhotoelectricity,
        /// <summary>
        /// 控制舱内操作面板急停
        /// </summary>
        OperationPanelInControlCabin,
        /// <summary>
        /// 发电机舱急停
        /// </summary>
        EmergencyStopOfEngineRoom,
        /// <summary>
        /// 车头急停
        /// </summary>
        EmergencyStop,
        /// <summary>
        /// 手控盒急停
        /// </summary>
        ControlBoxEmergencyStop,
        /// <summary>
        /// 车体右侧急停
        /// </summary>
        CarBodyRigthStop,
        /// <summary>
        /// 车体左侧急停
        /// </summary>
        CarBodyLeftStop,
        /// <summary>
        /// 竖臂急停
        /// </summary>
        Vertivalstop,
        /// <summary>
        /// 软件控制急停
        /// </summary>
        EquipmentStop,
        /// <summary>
        /// 车头超声波防撞
        /// </summary>
        EquipmentStopSX,
        /// <summary>
        /// 车尾超声波防撞
        /// </summary>
        UltrasonicAnticollisionAtThRear,
        /// <summary>
        /// 车头急停
        /// </summary>
        CarHeadStop,
        /// <summary>
        /// 车尾右侧通道防撞
        /// </summary>
        RearRightSideChannelCollisionAvoidance,
        /// <summary>
        /// 横臂左侧限高光电
        /// </summary>
        HorHandLeftHightLight,
        /// <summary>
        /// 横臂右侧限高光电
        /// </summary>
        HorHandRightHeightLight,
        /// <summary>
        /// 竖臂前侧通道防撞
        /// </summary>
        VerForwardStopHitch,
        /// <summary>
        /// 竖臂后侧通道防撞
        /// </summary>
        VerBackWardStopHitch,
        /// <summary>
        /// 竖臂前侧底部防撞
        /// </summary>
        BottomSoundStopHitch,
        /// <summary>
        /// 竖臂后侧底部防撞
        /// </summary>
        VerBottomSoundStopHitch2,
        /// <summary>
        /// 电子围栏前侧
        /// </summary>
        BeforeProtectionZonecrashes,
        /// <summary>
        /// 电子围栏后侧
        /// </summary>
        AfterProtectionZonecrashes,
        /// <summary>
        /// 电子围栏左侧
        /// </summary>
        LeftProtectionZonecrashes,
        /// <summary>
        /// 电子围栏右侧
        /// </summary>
        RightProtectionZonecrashes,
        /// <summary>
        /// 配电屏门
        /// </summary>
        LimitOfElectricScreenDoor,
        /// <summary>
        /// 控制屏门限位
        /// </summary>
        ControlPanelDoorLimit,
        /// <summary>
        /// 控制舱门限位
        /// </summary>
        ControlHatchLimit,
        /// <summary>
        /// 发电机舱门限位
        /// </summary>
        GeneratorDoorLimit,
        /// <summary>
        /// 设备舱门限位
        /// </summary>
        EquipmentDoorLimit,
        /// <summary>
        /// 机头门限位
        /// </summary>
        HeadThreshold,
        /// <summary>
        /// 柜门急停
        /// </summary>
        EleDoorStopSX,
        /// <summary>
        /// 加速器舱左侧急停
        /// </summary>
        BoostingRoomLeftStopSX,
        /// <summary>
        /// 加速器舱右侧急停
        /// </summary>
        BoostingRoomRightStopSX,
        /// <summary>
        /// 加速器舱外急停
        /// </summary>
        BoostingOutRoomStopSX,
        /// <summary>
        /// 探测器臂左侧急停
        /// </summary>
        VerHandLeftStopSX,
        /// <summary>
        /// 探测器臂右侧急停
        /// </summary>
        VerHandRightStopSX,
        /// <summary>
        /// 控制室急停
        /// </summary>
        ContronRoomStopSX,
        /// <summary>
        /// 加速器急停
        /// </summary>
        BoostingStopSX,
        /// <summary>
        /// 入口左侧急停
        /// </summary>
        EntranceLeftSX,
        /// <summary>
        /// 入口右侧急停
        /// </summary>
        EntranceRightSX,
        /// <summary>
        /// 出口左侧急停
        /// </summary>
        ExitLeftSX,
        /// <summary>
        /// 出口右侧急停
        /// </summary>
        ExitRightSX,
        /// <summary>
        /// 
        /// </summary>
        HeadUltrasonicCollisionAvoidance,
        /// <summary>
        /// 
        /// </summary>
        UltrasonicAntiCollisionAtThRear,
        /// <summary>
        /// 传送装置就绪
        /// </summary>
        TransportUnit,
        /// <summary>
        /// 运行状态
        /// </summary>
        RunningState,
        /// <summary>
        /// 维护状态
        /// </summary>
        MaintenanceStatus,
        /// <summary>
        /// ER1抱闸打开
        /// </summary>
        ER1Brake,
        /// <summary>
        /// ER2抱闸打开
        /// </summary>
        ER2Brake,
        /// <summary>
        /// ER1上电
        /// </summary>
        ER1PowerOn,
        /// <summary>
        /// ER2上电
        /// </summary>
        ER2PowerOn,
        /// <summary>
        /// 入口辊筒上电
        /// </summary>
        InletRoller,
        /// <summary>
        /// 出口辊筒上电
        /// </summary>
        ExportRoller,
        /// <summary>
        /// 浪涌正常
        /// </summary>
        SurgeNormal,
        /// <summary>
        /// 加速器上电
        /// </summary>
        AcceleratorPowerOn,
        /// <summary>
        /// 安全继电器
        /// </summary>
        SafetyRelay,
        /// <summary>
        /// PLC柜急停
        /// </summary>
        PLCEmergencyStop,
        /// <summary>
        /// 门限位1
        /// </summary>
        DoorThreshold1,
        /// <summary>
        /// 门限位2
        /// </summary>
        DoorThreshold2,
        /// <summary>
        /// 区扫正常
        /// </summary>
        SweepNormalArea,
        /// <summary>
        /// 入口区域
        /// </summary>
        EntranceArea,
        /// <summary>
        /// 出口区域
        /// </summary>
        ExportArea,
        /// <summary>
        /// 主束区域
        /// </summary>
        MainBeamArea,
        /// <summary>
        /// 防撞报警
        /// </summary>
        HitchAlarmNormal,
        /// <summary>
        /// 遥控急停
        /// </summary>
        RemotelyControl,
        /// <summary>
        /// 车头防撞
        /// </summary>
        CarHeadAvoidance,
        /// <summary>
        /// 车尾防撞
        /// </summary>
        CarEndAvoidance,
        /// <summary>
        /// 车头通道内测防撞
        /// </summary>
        CarHeadFrontPassageAvoidance,
        /// <summary>
        /// 车尾通道内测防撞
        /// </summary>
        CarEndFrontPassageAvoidance,

        /// <summary>
        /// 车头通道外侧防撞
        /// </summary>
        CarHeadOutsidePassageAvoidance,
        /// <summary>
        /// 车尾通道外侧防撞
        /// </summary>
        CarEndOutsidePassageAvoidance,
        /// <summary>
        /// 竖臂左侧防撞
        /// </summary>
        VerticalALeftACollision,
        /// <summary>
        /// 竖臂右侧防撞
        /// </summary>
        VerticalARightACollision,
        /// <summary>
        /// 行走变频器故障
        /// </summary>
        WalkingFrequencyConverter,
        /// <summary>
        /// 转向变频器故障
        /// </summary>
        SteeringFrequencyConverter,
        /// <summary>
        /// 编码器1
        /// </summary>
        Encoder1,
        /// <summary>
        /// 编码器2
        /// </summary>
        Encoder2,
        /// <summary>
        /// 编码器3
        /// </summary>
        Encoder3,
        /// <summary>
        /// 编码器4
        /// </summary>
        Encoder4,

        #region BGV7000
        /// <summary>
        /// 急停总故障
        /// </summary>
        EmergryStopHitch,
        /// <summary>
        /// 门限位故障
        /// </summary>
        DoorHitch,
        /// <summary>
        /// 
        /// </summary>
        CollisionAvoidanceAlarm,
        /// <summary>
        /// 
        /// </summary>
        HydraulicPressureSystemReady,
        /// <summary>
        /// 车头防撞
        /// </summary>
        AutomobileHeadStop,
        /// <summary>
        /// 车尾防撞
        /// </summary>
        RearEmergencyStop,
        /// <summary>
        /// 底舱前门限位
        /// </summary>
        BottomDoorForeLimit,
        /// <summary>
        /// 底舱左门限位
        /// </summary>
        BottomDoorLeftLimit,
        /// <summary>
        /// 底舱右门限位
        /// </summary>
        BottomDoorRightLimit,
        /// <summary>
        /// 竖臂左侧急停
        /// </summary>
        VerticalALeftAStop,
        /// <summary>
        /// 竖臂右侧急停
        /// </summary>
        VerticalARightAStop,
        /// <summary>
        /// 空气左悬挂报警
        /// </summary>
        HangingLeftRanging,
        /// <summary>
        /// 空气右悬挂报警
        /// </summary>
        HangingRightRanging,
        /// <summary>
        /// 动态调频
        /// </summary>
        DYNAMIC,
        /// <summary>
        /// 加速器通讯
        /// </summary>
        AcceleratorCommunication,
        /// <summary>
        /// 远程IP通讯
        /// </summary>
        RemoteIOStationCommunication,
        /// <summary>
        /// 变频器通讯
        /// </summary>
        FrequencyConverterCommunication,
        /// <summary>
        /// 雷达通讯
        /// </summary>
        LaserRadarCommunication,
        /// <summary>
        /// 车头避让控制板通讯
        /// </summary>
        FrontControlPanelCommunication,
        /// <summary>
        /// 保压
        /// </summary>
        Pressurize,
        /// <summary>
        /// 斜壁压力传感器
        /// </summary>
        InclinedArmPressureSensor,
        /// <summary>
        /// 竖臂压力传感器
        /// </summary>
        CrossArmPressureSensor,
        /// <summary>
        /// 斜壁压力传感器
        /// </summary>
        VerticalArmPressureSensor,
        #region 7700新增
        HeadCollisionAvoidanceOfMainCabinHead1,
        HeadCollisionAvoidanceOfMainCabinHead2,
        HeadCollisionAvoidanceOfMainCabinHead3,
        HeadCollisionAvoidanceOfAuxiliaryCabin1,
        HeadCollisionAvoidanceOfAuxiliaryCabin2,
        HeadCollisionAvoidanceOfAuxiliaryCabin3,
        RearCollisionAvoidanceOfMainCabinHead1,
        RearCollisionAvoidanceOfMainCabinHead2,
        RearCollisionAvoidanceOfMainCabinHead3,
        RearCollisionAvoidanceOfAuxiliaryCabin1,
        RearCollisionAvoidanceOfAuxiliaryCabin2,
        RearCollisionAvoidanceOfAuxiliaryCabin3,
        WalkingMotorOfMainCabin1,
        WalkingMotorOfMainCabin2,
        SteeringMotorOfMainCabin1,
        SteeringMotorOfMainCabin2,
        WalkingMotorOfAuxiliaryCabin1,
        WalkingMotorOfAuxiliaryCabin2,
        SteeringMotorOfAuxiliaryCabin1,
        SteeringMotorOfAuxiliaryCabin2,
        DeflectionMotor,
        AcceleratorForeDoorLimit2,
        highHanded,
        ViewMode,
        DeflectionMode,
        #endregion
        #endregion

        #region BS2000
        /// <summary>
        /// 工作模式
        /// </summary>
        WorkMode,
        /// <summary>
        /// 进线舱急停1
        /// </summary>
        InLineEmergyStop1,
        /// <summary>
        /// 进线舱急停2
        /// </summary>
        InLineEmergyStop2,
        /// <summary>
        /// 进线舱急停就绪
        /// </summary>
        InLineEmergyStopReady,
        /// <summary>
        /// 顶舱急停就绪
        /// </summary>
        TopEmergyStopReady,
        /// <summary>
        /// 顶舱急停1
        /// </summary>
        TopEmergyStop1,
        /// <summary>
        /// 顶舱急停2
        /// </summary>
        TopEmergyStop2,
        /// <summary>
        /// 侧舱急停1
        /// </summary>
        SideCabinEmergyStop1,
        /// <summary>
        /// 侧舱急停2
        /// </summary>
        SideCabinEmergyStop2,
        /// <summary>
        /// 侧舱急停就绪
        /// </summary>
        SideCabinEmergyStopReady,
        /// <summary>
        /// 进线舱门限位
        /// </summary>
        InLineCabinDoor,
        /// <summary>
        /// 侧舱急停就绪
        /// </summary>
        SideCabinDoor,
        /// <summary>
        /// 顶舱门限位
        /// </summary>
        TopCabinDoor,
        /// <summary>
        /// 侧舱水冷就绪
        /// </summary>
        SideCabinWaterCoolingReady,
        /// <summary>
        /// 进线舱水冷就绪
        /// </summary>
        InletCabinWaterCoolingReady,
        /// <summary>
        /// 顶舱水冷就绪
        /// </summary>
        TopCabinWaterCoolingReady,
        /// <summary>
        /// 侧舱射线源就绪
        /// </summary>
        SideCabinXrayReady,
        /// <summary>
        /// 顶舱射线源就绪
        /// </summary>
        TopCabinXrayReady,
        /// <summary>
        /// 进线舱射线源就绪
        /// </summary>
        InLetCabinXrayReady,
        /// <summary>
        /// 侧舱电机就绪
        /// </summary>
        SideCabinElectricalMachineryReady,
        /// <summary>
        /// 顶舱电机就绪
        /// </summary>
        TopCabinElectricalMachineryReady,
        /// <summary>
        /// 进线舱电机就绪
        /// </summary>
        InLetCabinElectricalMachineryReady,
        /// <summary>
        /// 进线舱安全联锁就绪
        /// </summary>
        InLetCabinSafeLinkReady,
        /// <summary>
        /// 侧舱安全联锁就绪
        /// </summary>
        SideCabinSafeLinkReady,
        /// <summary>
        /// 顶舱安全联锁就绪
        /// </summary>
        TopCabinSafeLinkReady,
        /// <summary>
        /// 进线舱系统就绪
        /// </summary>
        InLetCabinSystemReady,
        /// <summary>
        /// 侧舱系统就绪
        /// </summary>
        SideCabinSystemReady,
        /// <summary>
        /// 顶舱系统就绪
        /// </summary>
        TopCabinSystemReady,
        /// <summary>
        /// 电机就绪
        /// </summary>
        ElectricalMachineryReady,
        /// <summary>
        /// 进线舱出束
        /// </summary>
        InLetCabinRaying,
        /// <summary>
        /// 侧舱出束
        /// </summary>
        SideCabinRaying,
        /// <summary>
        /// 顶舱出束
        /// </summary>
        TopCabinRaying,

        /// <summary>
        /// 进线舱错误码
        /// </summary>
        InLetCabinFalut,
        /// <summary>
        /// 顶舱错误码
        /// </summary>
        TopCabinFalut,
        /// <summary>
        /// 侧舱错误码
        /// </summary>
        SideCabinFalut,

        TopCabinRayStatus,
        InLetCabinRayStatus,
        SideCabinRayStatus,
        TopCabinRayTotalWarmUpTime,
        TopCabinRayCurrentWarmUpTime,
        InLetCabinRayTotalWarmUpTime,
        InLetCabinRayCurrentWarmUpTime,
        SideCabinRayTotalWarmUpTime,
        SideCabinRayCurrentWarmUpTime,
        TopAnodeTube,
        TopOilTemp,
        TopInverter,
        TopVol,
        TopCurrent,
        InLetAnodeTube,
        InLetOilTemp,
        InLetInverter,
        InLetVol,
        InLetCurrent,
        SideAnodeTube,
        SideOilTemp,
        SideInverter,
        SideVol,
        SideCurrent,
        InLetStartingElectricalMachinery,
        TopStartingElectricalMachinery,
        SideStartingElectricalMachinery,
        /// <summary>
        /// 总系统就绪-背散转速到了之后的标志位
        /// </summary>
        TotalEquipmentSystemReady,
        #endregion

        TestMode,
    }
}
