using BG_Entities;
using BG_WorkFlow;
using BGCommunication;
using BGModel;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonFunc;
using static CMW.Common.Utilities.CommonDeleget;
using BGLogs;
using System.IO;

namespace BG_Services
{
    public class CSharpPLCEquipment : BasePLCEquipment
    {
        CompositionContainer container = null;
        [ImportMany("PLC", typeof(PLCProtocol), AllowRecomposition = true)]
        //保存插件的内存对象
        public Lazy<PLCProtocol, IMetaData>[] Plugins { get; set; }
        Dictionary<PLCPositionEnum, int> PlcPositionEnum = new Dictionary<PLCPositionEnum, int>();
        public bool IsGotoConnnection = true;
        public bool IsLoad = false;
        protected PLCProtocol CurrentPLCProrocolControl;
        ObservableCollection<StatusModel> statusModels = new ObservableCollection<StatusModel>();
        ControlVersionsBLL _ControlVersionsBLL = new ControlVersionsBLL();
        string ParamaterAddress = "DB16.0";
        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        public override bool Connect(string PlcAddress,string Port)
        {
            while (IsGotoConnnection)
            {
                try
                {
                    if (!IsLoad)
                    {
                        break;
                    }
                    Thread.Sleep(500);
                    if (!CommonFunc.PingIp(PlcAddress))
                    {
                        IsConnection = CommonFunc.PingIp(PlcAddress);
                    }

                    //如果未连接
                    if (!IsConnection)
                    {
                        if (CurrentPLCProrocolControl == null)
                        {
                            continue;
                        }
                        InitPLC();
                    }
                }
                catch (Exception ex)
                {
                    CommonDeleget.HandTaskException(ex);
                }
            }
            return true;
        }
        /// <summary>
        /// 初始化PLC连接
        /// </summary>
        public void InitPLC()
        {
            Common.PlcIsInit = false;
            bool temp = CurrentPLCProrocolControl.Connect();
            if (temp)
            {
                Common.PlcIsInit = CurrentPLCProrocolControl.InitPLC();
                if (!Common.PlcIsInit)
                {
                    CurrentPLCProrocolControl.DisConnect();
                    return;
                }
                if (!Common.IsConnection)
                {
                    //TODO 如果检测到PLC断了就去重新赋默认值
                    CommonDeleget.InitParameterAction();
                }
                Common.IsConnection = true;
            }
            else
            { Common.IsConnection = false; isSafeSerices = false; }
        }
        /// <summary>
        /// 判断连接状态
        /// </summary>
        /// <returns></returns>
        public override bool IsConnect()
        {
            return CurrentPLCProrocolControl.IsConnect();
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        public override void DisConnect()
        {
            CurrentPLCProrocolControl.DisConnect();
        }
        /// <summary>
        /// 接收报文
        /// </summary>
        /// <returns></returns>
        public override bool ReceviceByte()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 处理报文逻辑给业务 
        /// </summary>
        /// <returns></returns>
        public override bool HandleByteToBussiness()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 查询点位状态
        /// </summary>
        /// <returns></returns>
        public override bool InquirePositionStatus()
        {
            try
            {
                if (!Common.IsConnection)
                {
                    return false;
                }

                if (!Common.PlcIsInit)
                {
                    return false;
                }
                if (CurrentPLCProrocolControl == null)
                {
                    return false;
                }

                List<bool> TempStatus = new List<bool>();
                List<ushort> TempDose = new List<ushort>();

                Common.IsSearchStatusSuccess = CurrentPLCProrocolControl.InqurMByte1(ref TempStatus, ref TempDose);

                if (Common.IsSearchStatusSuccess)
                {
                    int index = 0;
                    foreach (var ChangeItem in TempStatus)
                    {
                        if (GlobalRetStatus[index] != ChangeItem)
                        {
                            Log.GetDistance().WriteInfoLogs($"GlobalRetStatus: Index:{index},value:{ChangeItem},Postition:M{(index / 8 + 1)}.{index % 8}");
                            GlobalRetStatus[index] = ChangeItem;
                        }
                        index++;
                    }
                    //Common.GlobalRetStatus = TempStatus;
                    Common.GlobalDoseStatus = TempDose;
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.IsConnection = false; isSafeSerices = false;
                CommonDeleget.HandTaskException(ex);
                return false;
            }
        }


        /// <summary>
        /// 查询硬件参数
        /// </summary>
        /// <returns></returns>
        public override bool InquireHardwareStatus()
        {
            try
            {
                if (!Common.IsConnection)
                {
                    return false;
                }

                if (!Common.PlcIsInit)
                {
                    return false;
                }
                if (CurrentPLCProrocolControl == null)
                {
                    return false;
                }
                List<byte[]> ParamaterList = new List<byte[]>();
                Tuple<byte, int, int> block = PLCTools.GetInstance().GetBlockType(ParamaterAddress);
                CurrentPLCProrocolControl.InqurHardwareByte1(block.Item1, block.Item2, block.Item3, new ushort[] { 240 }, ref ParamaterList);
                ValidByteToStatusValue(CurrentPLCProrocolControl.StatusValues);
                HandwareParamaterCallback?.Invoke(PLCDBStatus);
                return true;
            }
            catch (Exception ex)
            {
                Common.IsConnection = false; isSafeSerices = false;
                CommonDeleget.HandTaskException(ex);
                return false;
            }
        }

        private void ValidByteToStatusValue(byte[] statusValue)
        {
            if (statusValue == null) return;
            if (PLCDBStatus == null) PLCDBStatus = new PLCDBStatus();
            if (statusValue.Length > 20)
            {
                PLCDBStatus.IPositions.Clear();
                byte[] IPosition = statusValue.Take(20).ToArray();
                for (int i = 0; i < IPosition.Length; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        byte n = (byte)Math.Pow(2, j);
                        int Ret = (IPosition[i] & n);
                        bool IPositionBool = (Ret == 0 ? false : true);
                        PLCDBStatus.IPositions.Add(IPositionBool);
                    }
                }
            }
            if (statusValue.Length > 40)
            {
                PLCDBStatus.OPositions.Clear();
                byte[] IPosition = statusValue.Skip(20).Take(20).ToArray();
                for (int i = 0; i < IPosition.Length; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        byte n = (byte)Math.Pow(2, j);
                        int Ret = (IPosition[i] & n);
                        bool OPositionBool = (Ret == 0 ? false : true);
                        PLCDBStatus.OPositions.Add(OPositionBool);
                    }
                }
            }
            if (statusValue.Length > 80)
            {
                PLCDBStatus.IntArray.Clear();
                byte[] IPosition = statusValue.Skip(40).Take(40).ToArray();
                for (int i = 0; i < IPosition.Length; i += 2)
                {
                    byte[] datas = new byte[2];
                    Array.Copy(IPosition, i, datas, 0, 2);
                    Int16 value = BitConverter.ToInt16(datas.Reverse().ToArray(), 0);
                    PLCDBStatus.IntArray.Add(value);
                }
            }
            if (statusValue.Length > 160)
            {
                PLCDBStatus.DIntArray.Clear();
                byte[] IPosition = statusValue.Skip(80).Take(80).ToArray();
                for (int i = 0; i < IPosition.Length; i += 4)
                {
                    byte[] datas = new byte[4];
                    Array.Copy(IPosition, i, datas, 0, 4);
                    int value = BitConverter.ToInt32(datas.Reverse().ToArray(), 0);
                    PLCDBStatus.DIntArray.Add(value);
                }
            }
            if (statusValue.Length >= 240)
            {
                PLCDBStatus.FloatArray.Clear();
                byte[] IPosition = statusValue.Skip(160).Take(80).ToArray();
                for (int i = 0; i < IPosition.Length; i += 4)
                {
                    byte[] datas = new byte[4];
                    Array.Copy(IPosition, i, datas, 0, 4);
                    float value = BitConverter.ToSingle(datas.Reverse().ToArray(), 0);
                    PLCDBStatus.FloatArray.Add(value);
                }
            }
        }
        /// <summary>
        /// 读取点位的状态-返回String读取
        /// </summary>
        /// <param name="Postion"></param>
        /// <param name="StartPositon"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public override string ReadPositionValue(string Postion, uint StartPositon, uint length)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 直接读取
        /// </summary>
        /// <returns></returns>
        public override bool ReadPositionValue(string positionItem)
        {
            return CurrentPLCProrocolControl.GetStatus(positionItem);
        }
        /// <summary>
        /// 将值写入到具体点位
        /// </summary>
        /// <param name="Position"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public override bool WritePositionValue(string Position, bool Value)
        {
            if (CurrentPLCProrocolControl == null) return false;
            Position = Position.TrimStart('M');
            byte StartPosition = Convert.ToByte(Position.Split('.')[0]);
            byte EndPosition = Convert.ToByte(Position.Split('.')[1]);
            return CurrentPLCProrocolControl.Execute(StartPosition, EndPosition, Value);
        }
        public override bool WritePositionValue(string Position, ushort Value)
        {
            if (CurrentPLCProrocolControl == null) return false;
            Position = Position.TrimStart('M');
            byte StartPosition = Convert.ToByte(Position.Split('.')[0]);
            byte EndPosition = Convert.ToByte(Position.Split('.')[1]);
            return CurrentPLCProrocolControl.SetValue(StartPosition, Value);
        }
        public override bool WritePositionValue(byte Position, ushort Value)
        {
            if (CurrentPLCProrocolControl == null) return false;
            return CurrentPLCProrocolControl.SetValue(Position, Value);
        }
        public override bool WritePositionValue(string Position, UInt32 Value)
        {
            if (CurrentPLCProrocolControl == null) return false;
            Position = Position.TrimStart('M');
            byte StartPosition = Convert.ToByte(Position.Split('.')[0]);
            byte EndPosition = Convert.ToByte(Position.Split('.')[1]);
            return CurrentPLCProrocolControl.SetValue(StartPosition, Value);
        }
        public override bool WritePositionValue(byte Position, UInt32 Value)
        {
            if (CurrentPLCProrocolControl == null) return false;
            return CurrentPLCProrocolControl.SetValue(Position, Value);
        }
        public override bool WritePositionValue(string Position, float Value)
        {
            if (CurrentPLCProrocolControl == null) return false;
            Position = Position.TrimStart('M');
            byte StartPosition = Convert.ToByte(Position.Split('.')[0]);
            byte EndPosition = Convert.ToByte(Position.Split('.')[1]);
            return CurrentPLCProrocolControl.SetValue(StartPosition, Value);
        }
        public override bool WritePositionValue(byte Position, float Value)
        {
            if (CurrentPLCProrocolControl == null) return false;
            return CurrentPLCProrocolControl.SetValue(Position, Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cv"></param>
        /// <param name="commonProtocol"></param>
        public override async void Load(ControlVersion cv, PLCProtocol commonProtocol)
        {
            await Task.Run(() => {
                IsLoad = true;
                CurrentPLCProrocolControl = commonProtocol;
                LoadPositionConfig(cv);
                LoadPLCPluginsConfig();
                LoadPlugins();
                LoadPLCModules();
                InitPlcConnection();
                CommonDeleget.CheckSystemConditionEvent += CheckSystemCondition;
                CommonDeleget.SetCommondEvent += WritePositionValue;
                CommonDeleget.SetDoseValueEvent += WritePositionValue;
                CurrentPLCProrocolControl.RegisterDisConnectedEvent(DisConnection);
                PlcConnectionCallback?.Invoke();
            });
        }

        /// <summary>
        /// 初始化协议
        /// </summary>
        /// <param name="pp"></param>
        void InitPlcConnection()
        {
            //CurrentPLCProrocolControll = pp;
            CurrentPLCProrocolControl.PLCConnectType = PLCProtocol.ConnectType.CT_DirectConnect;
            CurrentPLCProrocolControl.WaitRecTick = 300;
            //Log.GetDistance().WriteInfoLogs("正在调用PLC Init方法");
            ConnectInterface client = new TCPClient(ConfigServices.GetInstance().localConfigModel.IpAddress, Convert.ToInt32(ConfigServices.GetInstance().localConfigModel.Port),
                Convert.ToBoolean(ConfigServices.GetInstance().localConfigModel.KeepLive), Convert.ToBoolean(ConfigServices.GetInstance().localConfigModel.Heart));
            CurrentPLCProrocolControl.InitConnection(ref client);
            CurrentPLCProrocolControl.RegisterDisConnectedEvent(DisConnection);
        }
        /// <summary>
        /// 载入插件
        /// </summary>
        void LoadPlugins()
        {
            try
            {
                SystemDirectoryConfig.AppDir = AppDomain.CurrentDomain.BaseDirectory;
                //获取工作目录
                var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                if (dir.Exists)
                {
                    //就是这里，读取所有符合条件的dll
                    var catalog = new DirectoryCatalog(dir.FullName, "*.dll");
                    container = new CompositionContainer(catalog);
                    try
                    {
                        container.ComposeParts(this);
                    }
                    catch (Exception ce)
                    {
                        CommonDeleget.WriteLogAction(ce.Message, LogType.ApplicationError, true);
                    }
                    Plugins.OrderBy(p => p.Metadata.Priority);
                }
            }
            catch (Exception ex)
            {
                CommonDeleget.WriteLogAction($"Loading Plc Modules Exception : {ex.StackTrace}", LogType.ApplicationError, true);
            }
        }
        /// <summary>
        /// 加载适配的PLC型号
        /// </summary>
        void LoadPLCModules()
        {
            try
            {
                string ModuleNames = ControlVersionList.FirstOrDefault(q => q.ControlversionKey == ConfigServices.GetInstance().localConfigModel.CMW_Version)?.PlcEquipment;
                if (!string.IsNullOrEmpty(ModuleNames))
                {
                    CurrentPLCProrocolControl = Plugins.First((q => q.Metadata.Name == ModuleNames))?.Value;
                    if (CurrentPLCProrocolControl == null) { CommonDeleget.MessageBoxActionAction($"PLC Modules Lost,lost Modules Name {ModuleNames}"); return; }

                }
            }
            catch (Exception ex)
            {
                CommonDeleget.WriteLogAction(ex.StackTrace, LogType.ApplicationError, true);
            }
        }
        /// <summary>
        /// 获取PLC插件配置
        /// </summary>
        void LoadPLCPluginsConfig()
        {
            ControlVersionList = _ControlVersionsBLL.GetControlVersionsBLLDataModel();
        }
        /// <summary>
        /// 载入点位配置状态
        /// </summary>
        /// <param name="cv"></param>
        void LoadPositionConfig(ControlVersion cv)
        {
            statusModels = HitChModelBLL.GetInstance().GetHitModelList(cv);
            PlcPositionEnum = PLCTools.GetInstance().StatusModelConvertTo(statusModels.ToList());
        }
        void DisConnection(string status)
        {
            IsConnection = false;
            isSafeSerices = false;
        }
        public override void UnLoad()
        {
            IsLoad = false;
            IsGotoConnnection = false;
        }
        public override string GetPlcBlockStr(string BlockPosition, ushort Strlength, InqureType _InqureType = InqureType.IString)
        {
            if (CurrentPLCProrocolControl == null) return string.Empty;
            Tuple<byte, int, int> block = PLCTools.GetInstance().GetBlockType(BlockPosition);
            if (_InqureType == InqureType.IString)
            {
                CurrentPLCProrocolControl.InqurStr(block.Item1, block.Item2, block.Item3, new[] { Strlength });
            }
            else
            {
                CurrentPLCProrocolControl.InqurUInt(block.Item1, block.Item2, block.Item3, new[] { Strlength });
            }
            return CurrentPLCProrocolControl?.PLcStr.Replace("\0", "").Replace("?", "").Replace("\u0001", "").Replace("'", "");
        }
        ///通过枚举获取点位状态
        public override bool GetStatusByPositionEnum(PLCPositionEnum pLCPositionEnum)
        {
            if (PlcPositionEnum.ContainsKey(pLCPositionEnum))
            {
                return Common.GlobalRetStatus[PlcPositionEnum[pLCPositionEnum]];
            }
            else
            {
                return false;
            }
        }
        ///通过枚举获取点位状态
        public override bool GetStatusByPositionEnum(string plcPositionEnuStr)
        {
            PLCPositionEnum pLCPositionEnum = PLCPositionEnum.AcceleratorDoorLimit1;
            Enum.TryParse(plcPositionEnuStr, out pLCPositionEnum);
            if (PlcPositionEnum.ContainsKey(pLCPositionEnum))
            {
                return Common.GlobalRetStatus[PlcPositionEnum[pLCPositionEnum]];
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 检测系统条件是否就绪了
        /// </summary>
        /// <returns></returns>
        public override bool CheckSystemCondition()
        {
            if (Common.controlVersion == ControlVersion.FastCheck)
            {
                return GetStatusByPositionEnum(PLCPositionEnum.MainSystemReady) &&
                       GetStatusByPositionEnum(PLCPositionEnum.RadiationSourceOrAcceleratorReady) &&
                       (GetStatusByPositionEnum(PLCPositionEnum.SafetyInterlockReady) || isSafeSerices) &&
                       Common.IsScanCanScan();
                WriteLogAction($"PLCControllManager MainSystemReady:{GetStatusByPositionEnum(PLCPositionEnum.MainSystemReady)};RadiationSourceOrAcceleratorReady:{GetStatusByPositionEnum(PLCPositionEnum.RadiationSourceOrAcceleratorReady)};" +
                    $"SafetyInterlockReady:{GetStatusByPositionEnum(PLCPositionEnum.SafetyInterlockReady)};isSafeSerices:{isSafeSerices};Common.IsScanCanScan({Common.IsScanCanScan()}", LogType.ScanStep);
            }
            if (Common.controlVersion == ControlVersion.CombinedMovementBetatron)
            {
                return GetStatusByPositionEnum(PLCPositionEnum.MainSystemReady) &&
                       GetStatusByPositionEnum(PLCPositionEnum.DriveReady) &&
                       (GetStatusByPositionEnum(PLCPositionEnum.SafetyInterlockReady) &&
                       BoostingControllerManager.GetInstance().IsReady() || isSafeSerices) && Common.IsScanCanScan();
            }
            if (Common.controlVersion == ControlVersion.PassengerCar)
            {
                return GetStatusByPositionEnum(PLCPositionEnum.MainSystemReady) &&
                    GetStatusByPositionEnum(PLCPositionEnum.RadiationSourceOrAcceleratorReady) &&
                    GetStatusByPositionEnum(PLCPositionEnum.TransportUnit) &&
                    GetStatusByPositionEnum(PLCPositionEnum.StopReady) &&
                    (GetStatusByPositionEnum(PLCPositionEnum.SafetyInterlockReady) || isSafeSerices) && Common.IsScanCanScan();
            }
            return GetStatusByPositionEnum(PLCPositionEnum.MainSystemReady) &&
                    GetStatusByPositionEnum(PLCPositionEnum.RadiationSourceOrAcceleratorReady) &&
                    (GetStatusByPositionEnum(PLCPositionEnum.SafetyInterlockReady) || isSafeSerices) && Common.IsScanCanScan();
        }

        public override string GetCurrentEquipmentVersion()
        {
            return GetPlcBlockStr(ConfigServices.GetInstance().localConfigModel.EquipmentAddress, Convert.ToUInt16(ConfigServices.GetInstance().localConfigModel.EquipmentLength));
        }

        public override string GetCurrentEquipmentModel()
        {
            return CurrentPLCProrocolControl.GetProtocolName();
        }

        public string GetRunStatus()
        {
            if (GetStatusByPositionEnum(PLCPositionEnum.RunningState)) { return "RunningState"; }
            else if (GetStatusByPositionEnum(PLCPositionEnum.MaintenanceStatus)) { return "MaintenanceStatus"; }
            else { return "Null"; }
        }
    }
}
