
using BG_Entities;
using BGCommunication;
using System;
using System.Collections.Generic;
using System.Drawing;
//using log4net;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CMW.Common.Utilities
{
    public static class Common
    {
        public static Dictionary<HandStatus, BitmapImage> bitImgDic = new Dictionary<HandStatus, BitmapImage>();
        /// <summary>
        /// 命令字典
        /// </summary>
        public static Dictionary<Command, string> CommandDic = new Dictionary<Command, string>();
        /// <summary>
        /// 按钮权限的列表
        /// </summary>
        public static List<string> ButtonList = new List<string>();
        /// <summary>
        /// 当前的设备型号
        /// </summary>
        public static ControlVersion controlVersion = ControlVersion.Car;
        /// <summary>
        /// 用来存储各个角度的位置
        /// </summary>
        public static List<string> angleList = new List<string>()
        {
            "27.3","27.4","27.5","27.6"
        };

        /// <summary>
        /// 用来存储自行走加速器各个角度的位置
        /// </summary>
        public static List<string> BoostingList = new List<string>()
        {
            "28.2","28.3"
        };
        /// <summary>
        /// 用来存储小角度加速器各个角度的位置
        /// </summary>
        public static List<string> SelfAutoAngleList = new List<string>()
        {
            "28.4","28.5","28.6"
        };

        public static RecvMessage ReceiveMessage;
        public static int ReceiveMessageUsedTimes = 0;


        /// <summary>
        /// 全局查询Arm状态
        /// </summary>
        public static void InquireArmHand()
        {
            if (!Common.IsArmConnection)
            {
                return;
            }
            Common.IsSearchStatusSuccess = Common.GlogbalArmProtocol.InqurHandStatus();
           
        }

        #region LocalConfig

        public static FtpHelper _ftpHelper;
        /// <summary>
        /// 是否按下了复位
        /// </summary>
        public static bool IsReset = true;
        public static bool IsClickClose = false;
        public static bool IsModuleOpen = false;
        public static bool IsConnection = false;
        public static bool IsArmConnection = false;
        public static Dictionary<int, bool> IsDoseConnections = new Dictionary<int, bool>();
        //public static bool IsDoseConnection = false;
        public static bool _IsClick = false;
        public static bool _IsRaying = false;
        public static bool IsConnectionSocket = false;
        public static int IsConnectionScan = 0;

        public static bool isExcuteTask = true;
        //和Betatron进行通讯的Betatron加速器协议
        public static ARMProtocol GlogbalArmProtocol = null;

        public static bool IsGotoConnnection = true;
        public static bool IsArmGotoConnnection = true;

        /// <summary>
        /// 是否存在错误
        /// </summary>
        public static bool IsExistBoardError = false;
        /// <summary>
        /// 探测器块数
        /// </summary>
        public static int BoardNum = 0;
        /// <summary>
        /// 探测器板链数
        /// </summary>
        public static int BoardLine = 0;
        /// <summary>
        /// 所处探测器板链的块数
        /// </summary>
        public static int BoardLineIndex = 0;
        public static bool IsSearchStatusSuccess = false;
        public static bool IsSearchDoseSuccess = false;
        public static bool IsSearchDoseGetSuccess = false;
        public static bool IsReadyStartScanImage = false;
        public static bool IsReadyStopScanImage = false;
        public static bool IsReadSendDarkInfo = false;

        public static bool PlcIsInit = false;
        #region 剂量的顺序


        public static List<bool> _GlobalRetStatus = new List<bool>(128)
        {
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false
        };

       
        /// <summary>
        /// 初始化Arm连接
        /// </summary>
        public static void InitArm()
        {
            bool temp = Common.GlogbalArmProtocol.Connect();
            if (temp)
            {
                Common.IsArmConnection = true;
            }
            else
            { Common.IsArmConnection = false; }
        }

        /// <summary>
        /// 全局变量 用来本地存储128个状态值
        /// </summary>
        public static List<bool> GlobalRetStatus
        {
            get { return _GlobalRetStatus; }
            set
            {
                _GlobalRetStatus = value;
            }
        }

        public static List<ushort> GlobalDoseStatus = new List<ushort>(20)
        {
              0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
        };

        #endregion

        #endregion


        #region 查询各种状态按照写死的值
        /// <summary>
        /// 查询高能低能还是双能
        /// </summary>
        /// <returns></returns>
        public static string SetDoubleOrSingle()
        {
            if (Common.GlobalRetStatus[21]) { return "DoubleEnergy"; }

            else if (Common.GlobalRetStatus[22]) { return "HeightEnergy"; }

            else if (Common.GlobalRetStatus[23]) { return "LowerEnergy"; }
            else
            {
                return "Null";
            }
        }

        /// <summary>
        /// 查询驱动装置是否就绪
        /// </summary>
        /// <returns></returns>
        public static string SetCarHandStatus()
        {
            if (Common.GlobalRetStatus[10]) { return "Ready"; }
            else
            {
                return "UnReady";
            }
        }
        /// <summary>
        /// 分动器啮合
        /// </summary>
        /// <returns></returns>
        public static string Trans()
        {
            return GlobalRetStatus[89] ? "TransferCaseEngagementClose"
                           : "TransferCaseEngagementOpen";
        }

        /// <summary>
        /// 查询臂展状态
        /// </summary>
        /// <returns></returns>
        public static string SetHandStatus()
        {
            if (IsVercatilHandOpen())
            {
                return "VerticalOpenPosition";
            }
            //在判断横臂是否展开
            else if (IsHornalictalHandOpen())
            {
                return "CrossHandOpenPosition";
            }
            //在判断斜臂是否展开
            else if (IsItalyHandOpen())
            {
                return "OpliqueHandOpenArrivePosition";
            }
            //啥都没展开就是各个臂都关闭得状态
            else
            {
                return "HandClose";
            }
        }

        /// <summary>
        /// 用于检测电子围栏是否报警
        /// </summary>
        /// <returns></returns>
        public static bool CheckElectronAlarm()
        {
            return Common.GlobalRetStatus[117] ||
                Common.GlobalRetStatus[118] ||
                Common.GlobalRetStatus[119] ||
                Common.GlobalRetStatus[120];
        }

        /// <summary>
        /// 用于检测车辆行进过程中是否报警
        /// </summary>
        /// <returns></returns>
        public static bool MoveAlarm()
        {
            return Common.GlobalRetStatus[107] ||
                Common.GlobalRetStatus[108] ||
                Common.GlobalRetStatus[109] ||
                Common.GlobalRetStatus[110] ||
                Common.GlobalRetStatus[111] ||
                Common.GlobalRetStatus[112] ||
                Common.GlobalRetStatus[113] ||
                Common.GlobalRetStatus[114] ||
                Common.GlobalRetStatus[115] ||
                Common.GlobalRetStatus[116];
        }

        #region  自行走
        /// <summary>
        /// 用于自行走检测车辆行进过程中是否报警
        /// </summary>
        /// <returns></returns>
        public static bool SelfAutoMoveAlarm()
        {
            return Common.GlobalRetStatus[127];
        }

        #endregion

        public static bool IsScanConnection()
        {
            return Common.IsConnectionScan == 0;
        }
        public static bool IsScanConnectioning()
        {
            return Common.IsConnectionScan == 1;
        }
        public static bool IsScanConnectionWithRay()
        {
            return Common.IsConnectionScan == 2;
        }
        public static bool IsScanCanScan()
        {
            return Common.IsConnectionScan == 3;
        }

        /// <summary>
        /// 判断 开始采图的指令、停止采图的指令、开始采集空气的指令、开始采集本体的指令是否都是false，
        /// 说明此时才可以开始采图
        /// </summary>
        /// <returns></returns>
        public static bool StartScanCondition()
        {
            return StartCollectionAir() ||
                    StartCollectionEmpty() ||
                    StartColectionImage() ||
                    //IsConnectionScan == 3 是代表扫描站就绪
                    EndColectionImage() || IsConnectionScan != 3 ||
                    (IsReadSendDarkInfo || IsReadyStartScanImage || IsReadyStopScanImage);//这里判断是否还在扫描中
        }

        public static bool isSafeSerices = false;

 

        /// <summary>
        /// 查询是否开始采集图片 状态
        /// </summary>
        /// <returns></returns>
        public static bool StartColectionImage()
        {
            return Common.GlobalRetStatus[45];
        }
        /// <summary>
        /// 查询是否停止采图 状态
        /// </summary>
        /// <returns></returns>
        public static bool EndColectionImage()
        {
            return Common.GlobalRetStatus[46];
        }


        /// <summary>
        /// 开始采集空气状态
        /// </summary>
        /// <returns></returns>
        public static bool StartCollectionAir()
        {
            return Common.GlobalRetStatus[55];
        }

        /// <summary>
        /// 开始采集本体状态
        /// </summary>
        /// <returns></returns>
        public static bool StartCollectionEmpty()
        {
            return Common.GlobalRetStatus[54];
        }

        /// <summary>
        /// 停高压，停束，停车等危险操作
        /// </summary>
        public static void StopAction()
        {
            ImageImportDll.SX_Disconnect(ImageImportDll.intPtr);
            ImageImportDll.SX_Destroy(ImageImportDll.intPtr);
            if (CommandDic.Count == 0)
            {
                return;
            }
            if (controlVersion == ControlVersion.SelfWorking)
            {
                //停止出束
                SetCommand(CommandDic[Command.AutoMode], true);
                //停束
                Common.SetCommand(CommandDic[Command.StopRay], false);
                //停主系统启动
                Common.SetCommand(CommandDic[Command.SysetmStartEnd], false);
                //停21.5 开始扫描
                Common.SetCommand(CommandDic[Command.StartScan], false);
                //驱动装置停止
                Common.SetCommand(CommandDic[Command.CarStop], false);
                //给PLC发送加速器就绪为false的指令
                SetCommand(CommandDic[Command.BoostPreHot], false);
            }
            else
            {
                //停束
                Common.SetCommand(CommandDic[Command.StopRay], false);
                //停21.5 开始扫描
                Common.SetCommand(CommandDic[Command.StartScan], false);
                if (controlVersion != ControlVersion.CombinedMovementBetatron
                    && controlVersion != ControlVersion.CombinedMovement
                    && controlVersion != ControlVersion.PassengerCar)
                {
                    //驱动装置停止
                    Common.SetCommand(CommandDic[Command.CarStop], false);
                    //停高压
                    Common.SetCommand(CommandDic[Command.StartHighVoltage], false);
                }
                //停主系统启动
                Common.SetCommand(CommandDic[Command.SysetmStartEnd], false);
            }
          
        }

        /// <summary>
        /// 紧急停止系统
        /// </summary>
        /// <returns></returns>
        public static bool CarStopSystem()
        {
            bool isSuccess = false;
            if (CommandDic.Count() == 0)
            {
                return false;
            }
            //停止扫描 指令
            Common.SetCommand(CommandDic[Command.StartScan], false);


            //停止扫描 TODO 这里要给方海涛发一个指令 停止扫描
            if (ImageImportDll.intPtr != IntPtr.Zero)
            {
                ImageImportDll.SX_Stop(ImageImportDll.intPtr);
            }

            //电子枪使能 停束
            Common.SetCommand(CommandDic[Command.StopRay], false);
            //Common.GlobalPLCProtocol.Execute(21, 2, false);
            //高压按钮
            Common.SetCommand(CommandDic[Command.StartHighVoltage], false);
            //驱动装置停止
            Common.SetCommand(CommandDic[Command.CarStop], false);
            //Common.GlobalPLCProtocol.Execute(22, 5, false);

            //再发送主系统未就绪
            Common.SetCommand(CommandDic[Command.SysetmStartEnd], false);
            //Common.GlobalPLCProtocol.Execute(21, 0, false);
            //如果在扫描停止扫描
            StopScan();
            return isSuccess;
        }


        /// <summary>
        /// 查询转台角度
        /// </summary>
        /// <returns></returns>
        public static string SetAngle()
        {
            if (Common.GlobalRetStatus[66])
            {
                return $"0°";
            }

            if (Common.GlobalRetStatus[67])
            {
                return $"80°";
            }

            if (Common.GlobalRetStatus[68])
            {
                return $"90°";
            }

            if (Common.GlobalRetStatus[69])
            {
                return $"95°";
            }
            else
            {
                return "Null";
            }
        }

        /// <summary>
        /// 电子围栏前报警
        /// </summary>
        /// <returns></returns>
        public static string Color(ElectronDirection ed)
        {
            switch (ed)
            {
                case ElectronDirection.Fore:
                    return Common.GlobalRetStatus[117] ? "#FF0000" : "#00FF00";
                case ElectronDirection.Back:
                    return Common.GlobalRetStatus[118] ? "#FF0000" : "#00FF00";
                case ElectronDirection.Left:
                    return Common.GlobalRetStatus[119] ? "#FF0000" : "#00FF00";
                case ElectronDirection.Right:
                    return Common.GlobalRetStatus[120] ? "#FF0000" : "#00FF00";
            }
            return "#FF0000";
        }


        /// <summary>
        /// 查询扫描模式是主动还是被动 PassiveMode 主动模式 InitiativeMode 被动模式
        /// </summary>
        /// <returns></returns>
        public static string SearchScanMode()
        {
            return Common.GlobalRetStatus[53] ? "PassiveMode" : "InitiativeMode";
        }


        /// <summary>
        /// 是主动模式还是被动模式
        /// </summary>
        /// <returns></returns>
        public static bool IsInative()
        {
            return Common.GlobalRetStatus[53];
        }

        /// <summary>
        /// 斜臂
        /// </summary>
        /// <returns></returns>
        public static bool IsItalyHandOpen()
        {
            return GlobalRetStatus[60];
        }
        /// <summary>
        /// 横臂
        /// </summary>
        /// <returns></returns>
        public static bool IsHornalictalHandOpen()
        {
            return GlobalRetStatus[62];
        }
        /// <summary>
        /// 竖臂
        /// </summary>
        /// <returns></returns>
        public static bool IsVercatilHandOpen()
        {
            return GlobalRetStatus[64];
        }

        /// <summary>
        /// 初始化扫描条件
        /// </summary>
        public static void InitScanCondition()
        {
            try
            {
                //这一步相当于整个流程结束了 把初始值设置为false;等待下一个点击按钮
                Common.IsReadyStartScanImage = false;
                Common.IsReadSendDarkInfo = false;
                Common.IsReadyStopScanImage = false;
                Common.SetCommand(CommandDic[Command.StartScan], false);
                //CommonDeleget.BuryingPoint($"Command.StartScan {Command.StartScan} 发送完毕，值为：{false}");
                //Common.SetCommand(CommandDic[Command.LightCollectionEnd], true);
                //Common.SetCommand(CommandDic[Command.DarkCollectionEnd], true);
                Common.SetCommand(CommandDic[Command.DarkCollectionEnd], false);
                //CommonDeleget.BuryingPoint($"Command.DarkCollectionEnd {Command.DarkCollectionEnd} 发送完毕，值为：{true}");
                Common.SetCommand(CommandDic[Command.LightCollectionEnd], false);
                //CommonDeleget.BuryingPoint($"Command.LightCollectionEnd {Command.LightCollectionEnd} 发送完毕，值为：{true}");
#if Release  
            Common.SetCommand(Common.CommandDic[Command.LightCollectionEnd], false);
#endif
                //Common.GlobalPLCProtocol.Execute(24, 4, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 停止扫描
        /// </summary>
        public static void StopScan()
        {
            try
            {
                ImageImportDll.SX_Stop(ImageImportDll.intPtr);
                InitScanCondition();
            }
            catch (Exception ex)
            {
                CommonDeleget.HandTaskException(ex);
            }
        }

        #endregion

        #region 自行走状态查找
        /// <summary>
        /// 初始化自行走的一些状态值
        /// </summary>
        public static void InitSelfAutoWorking()
        {
            SetCommand(CommandDic[Command.BoostReay], false);
            SetCommand(CommandDic[Command.BoostPreHot], false);
            SetCommand(CommandDic[Command.BoostRaying], false);
            SetCommand(CommandDic[Command.StartScan], false);
            SetCommand(CommandDic[Command.SysetmStartEnd], false);
        }

        /// <summary>
        /// 自行走判断当前状态是前进还是后退
        /// </summary>
        /// <returns></returns>
        public static string SearchCurrentCarStatus()
        {
            if (Common.GlobalRetStatus[14])
            {
                return "Go";
            }
            else if (Common.GlobalRetStatus[15])
            {
                return "Back";
            }
            return "Stop";
        }
        /// <summary>
        /// 自行走判断抱闸是否打开
        /// </summary>
        /// <returns></returns>
        public static string SearchBandIsOpen()
        {
            if (Common.GlobalRetStatus[90])
            {
                return "Open";
            }
            else 
            {
                return "Close";
            }
        }

        #endregion


        #region 快检
        /// <summary>
        /// 组合移动
        /// </summary>
        /// <returns></returns>
        public static Tuple<bool, List<string>> CheckMomentCheckStop()
        {
            var HasReset = false;
            List<string> StopStr = new List<string>();

            if (Common.GlobalRetStatus[28])
            {
                StopStr.Add(CommonDeleget.UpdateStatusNameAction("DoseAlarm"));
                HasReset = true;
            }
            if (!Common.GlobalRetStatus[13])
            {
                StopStr.Add(CommonDeleget.UpdateStatusNameAction("ContronRoomStop"));
                HasReset = true;
            }
            if (Common.GlobalRetStatus[112])
            {
                StopStr.Add(CommonDeleget.UpdateStatusNameAction("ForwardLimitAlarm"));
                HasReset = true;
            }
            if (Common.GlobalRetStatus[113])
            {
                StopStr.Add(CommonDeleget.UpdateStatusNameAction("BackwardLimitAlarm"));
                HasReset = true;
            }
            Tuple<bool, List<string>> TupleItem = new Tuple<bool, List<string>>(HasReset, StopStr);
            return TupleItem;
        }
        #endregion

        #region 发送命令通用解析方法
        /// <summary>
        /// 解析M开头的地址，进行发送给协议
        /// </summary>
        /// <param name="Position"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetCommand(string Position, bool value)
        {
            try
            {
                return CommonDeleget.SetCommondAction(Position,value);
            }
            catch
            {
                return false;
            }
        }
  
        #endregion

        public static string RPMDataToWebData()
        {
            if (GlogbalArmProtocol == null)
            {
                return "0";
            }
            return GlogbalArmProtocol.pf.GetHashCode().ToString();
        }
    }
}
