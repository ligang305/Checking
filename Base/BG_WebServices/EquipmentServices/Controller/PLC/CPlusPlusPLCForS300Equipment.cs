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
using static BGCommunication.PLCClientCallback;
using System.Runtime.InteropServices;
using System.Diagnostics;
using BG_Services.EquipmentServices.Controller;

namespace BG_Services
{
    public class CPlusPlusPLCForS300Equipment : CPlusCPlusEquipment
    {
        Dictionary<PLCPositionEnum, int> PlcPositionEnum = new Dictionary<PLCPositionEnum, int>();
        ObservableCollection<StatusModel> statusModels = new ObservableCollection<StatusModel>();
        public CPlusPlusPLCForS300Equipment()
        {
            protocolIndex=  PlcClientServices.SetPLCProtocolType(2);
            recvCallback = ReceiveCallback;
            PlcClientServices.Set_BufferRevCallback(protocolIndex, recvCallback);//1  callback 1 
            bool InitResult = PlcClientServices.InitCommonProtocol(protocolIndex, 0);

            CommonDeleget.CheckSystemConditionEvent += CheckSystemCondition;
            CommonDeleget.SetCommondEvent += WritePositionValue;
            CommonDeleget.SetDoseValueEvent += WritePositionValue;
            block = PLCTools.GetInstance().GetBlockTypeForCPlusPlus(ConfigServices.GetInstance().localConfigModel.InquireStartPosition);
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
        public override void Load(ControlVersion cv, PLCProtocol commonProtocol)
        {
            LoadPositionConfig(cv);
            base.Load(cv, commonProtocol);
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
                short RequestLength = 30;
                IntPtr plcBytePtr = Marshal.AllocHGlobal(RequestLength);
                Stopwatch stopwatch = Stopwatch.StartNew();
                Common.IsSearchStatusSuccess  = PlcClientServices.ReadBytes(protocolIndex, block.Item1, (byte)block.Item2, (byte)block.Item3, RequestLength, ref plcBytePtr);
                stopwatch.Stop();
                if(stopwatch.ElapsedMilliseconds>70)
                {
                    CommonDeleget.WriteLogAction($@"PLC InquirePlcPosition:{stopwatch.ElapsedMilliseconds}", LogType.NormalLog, false);
                }
                byte[] totalBytes = new byte[RequestLength];
                Marshal.Copy(plcBytePtr, totalBytes, 0, RequestLength);
                byte[] boolBytes = new byte[16];
                byte[] doseBytes = new byte[14];
                Array.Copy(totalBytes, 0, boolBytes, 0, 16);
                Array.Copy(totalBytes, 16, doseBytes, 0, 14);
                InquireBool(boolBytes);
                InquireDose(doseBytes);
                Marshal.FreeHGlobal(plcBytePtr);
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
        /// 查询剂量
        /// </summary>
        protected override void InquireDose(byte[] DoseBytes)
        {
            ushort[] doseShorts = new ushort[14 / 2];
            for (int i = 0; i < 14; i+=2)
            {
                ushort Dose = (ushort)((DoseBytes[i] << 8) + DoseBytes[i+1]);
                doseShorts[i / 2] = Dose;
            }
            Common.GlobalDoseStatus = doseShorts.ToList();
        }
        /// <summary>
        /// 查询点位
        /// </summary>
        protected override void InquireBool(byte[] BoolBytes)
        {
          
            bool[] batchBool = new bool[BoolBytes.Length * 8];
            if (BoolBytes.Length * 8 < 128) return;
            for (int i = 0; i < BoolBytes.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    byte n = (byte)Math.Pow(2, j);
                    int Ret = (BoolBytes[i] & n);
                    bool PositionBool = (Ret == 0 ? false : true);
                    batchBool[i*8 + j] = PositionBool;
                }
            }
            for (int index = 0; index < BoolBytes.Length * 8; index++)
            {
                if (GlobalRetStatus[index] != batchBool[index])
                {
                    //if(index == 114)
                    {
                        Log.GetDistance().WriteInfoLogs($"InquirePositionStatus: Index:{index},value:{ batchBool[index]},Postition:M{(index / 8 + 1)}.{index % 8}");
                    }
                    GlobalRetStatus[index] = batchBool[index];
                }
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
                return true;
            }
            catch (Exception ex)
            {
                Common.IsConnection = false; isSafeSerices = false;
                CommonDeleget.HandTaskException(ex);
                return false;
            }
        }

        public override string GetPlcBlockStr(string BlockPosition, ushort Strlength, InqureType _InqureType = InqureType.IString)
        {
            string plcValue = string.Empty;
            Tuple<byte, int, int> block = PLCTools.GetInstance().GetBlockTypeForCPlusPlus(BlockPosition);
            if (_InqureType == InqureType.IString)
            {
                plcValue = PlcClientServices.GetString(protocolIndex, block.Item1, (ushort)block.Item2, (ushort)block.Item3, (short)Strlength);
            }
            return plcValue;
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
    }
}
