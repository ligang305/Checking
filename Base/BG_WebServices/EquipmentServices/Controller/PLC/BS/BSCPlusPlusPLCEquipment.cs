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
using System.Runtime.Remoting.Contexts;
using BG_Services.EquipmentServices.Controller;

namespace BG_Services
{
    /// <summary>
    /// 为了和主控PLC做区分，单独添加的BS PLC程序。。有一定的临时性，后期可能需要和主控PLC进行合并
    /// </summary>
    public class BSCPlusPlusPLCEquipment : CPlusCPlusEquipment 
    {

        Dictionary<PLCPositionEnum, int> PlcPositionEnum = new Dictionary<PLCPositionEnum, int>();
        Dictionary<PLCPositionEnum, int> PlcBoolPositionEnum = new Dictionary<PLCPositionEnum, int>();
        Dictionary<PLCPositionEnum, int> PlcShortPositionEnum = new Dictionary<PLCPositionEnum, int>();
        Dictionary<PLCPositionEnum, int> PlcIntPositionEnum = new Dictionary<PLCPositionEnum, int>();
        Dictionary<PLCPositionEnum, int> PlcFloatPositionEnum = new Dictionary<PLCPositionEnum, int>();

        ObservableCollection<StatusModel> statusModels = new ObservableCollection<StatusModel>();

        string BSPLCAddress = "DB54.0.0";
        public BSCPlusPlusPLCEquipment() : base()
        {
            BSPLCAddress = ConfigServices.GetInstance().localConfigModel.BSInquireStartPosition;
            block = PLCTools.GetInstance().GetBlockTypeForCPlusPlus(BSPLCAddress);
        }
        /// <summary>
        /// 载入点位配置状态
        /// </summary>
        /// <param name="cv"></param>
        void LoadPositionConfig(ControlVersion cv)
        {
            statusModels = HitChModelBLL.GetInstance().GetHitModelList(cv);
            PlcPositionEnum = PLCTools.GetInstance().StatusModelConvertTo(statusModels.ToList());

            var OPositionStatusModels = PositionBLL.GetInstance().GetPositionModel(cv, PositionConfigType.OPosition);
            PlcBoolPositionEnum = PLCTools.GetInstance().StatusModelConvertTo(OPositionStatusModels.ToList());

            var IntPositionStatusModels = PositionBLL.GetInstance().GetPositionModel(cv, PositionConfigType.IntPosition);
            PlcShortPositionEnum = PLCTools.GetInstance().StatusModelConvertTo(IntPositionStatusModels.ToList());

            var DIntPositionStatusModels = PositionBLL.GetInstance().GetPositionModel(cv, PositionConfigType.DIntPosition);
            PlcIntPositionEnum = PLCTools.GetInstance().StatusModelConvertTo(DIntPositionStatusModels.ToList());

            var FloatPositionStatusModels = PositionBLL.GetInstance().GetPositionModel(cv, PositionConfigType.FloatPosition);
            PlcFloatPositionEnum = PLCTools.GetInstance().StatusModelConvertTo(FloatPositionStatusModels.ToList());
        }
        public override void Load(ControlVersion cv, PLCProtocol commonProtocol)
        {
            LoadPositionConfig(cv);
            base.Load(cv, commonProtocol); 
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        public override bool Connect(string IpAddress, string Port)
        {
            while (true)
            {
                Thread.Sleep(ThreadSleepTime);
                if (!PlcClientServices.IsConnectedClient(protocolIndex))
                {
                    this.PlcIsInit = false;
                    this.IsConnection = false;
                    PlcClientServices.ConnectedClient(protocolIndex, IpAddress,
                        Convert.ToUInt16(Port));
                }
                if (!this.PlcIsInit)
                {
                    this.PlcIsInit = PlcClientServices.InitPlc(protocolIndex);
                    if (this.PlcIsInit)
                    {
                        //TODO 如果检测到PLC断了就去重新赋默认值
                        CommonDeleget.InitParameterAction();
                    }
                    this.IsConnection = true;
                }
            }
        }
        /// <summary>
        /// 查询点位状态
        /// </summary>
        /// <returns></returns>
        public override bool InquirePositionStatus()
        {
            try
            {
               
                if (!this.IsConnection)
                {
                    return false;
                }
                if (!this.PlcIsInit)
                {
                    return false;
                }
                short RequestLength = 66;
                IntPtr plcBytePtr = Marshal.AllocHGlobal(RequestLength);
                Stopwatch stopwatch = Stopwatch.StartNew();
                PlcClientServices.ReadBytes(protocolIndex, block.Item1, (byte)block.Item2, (byte)block.Item3, RequestLength, ref plcBytePtr);
                stopwatch.Stop();
                Console.WriteLine($@"PlcClientServices.ReadBytes:{stopwatch.ElapsedMilliseconds}");
                byte[] totalBytes = new byte[RequestLength];
                Marshal.Copy(plcBytePtr, totalBytes, 0, RequestLength);
                byte[] boolBytes = new byte[16];
                byte[] doseBytes = new byte[50];
                Array.Copy(totalBytes, 0, boolBytes, 0, 16);
                Array.Copy(totalBytes, 16, doseBytes, 0, 50);
                InquireBool(boolBytes);
                InquireDose(doseBytes);
                Marshal.FreeHGlobal(plcBytePtr);
                return true;
            }
            catch (Exception ex)
            {
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
                if (!this.IsConnection)
                {
                    return false;
                }
                if (!this.PlcIsInit)
                {
                    return false;
                }
                short RequestLength = 240;
                int muti = 3;
                short tempRequestLength = Convert.ToInt16(RequestLength / muti);
                Tuple<byte, int, int> block = PLCTools.GetInstance().GetBlockTypeForCPlusPlus(BSPLCAddress);
                IntPtr plcBytePtr = Marshal.AllocHGlobal(RequestLength);
                byte[] totalBytes = new byte[RequestLength];
                //为什么要这么写呢，是因为S7-1200只能 一次读不超过200个字节，S7-1500 一次可以读360个字节
                //PLC有时有病用1200 有时用1500 所以为了兼容只能分开读取
                for (int i = 0; i < muti; i++)
                {
                    PlcClientServices.ReadBytes(protocolIndex, block.Item1, (ushort)block.Item2, (ushort)((block.Item3 + 66 + tempRequestLength * i) * 8), tempRequestLength, ref plcBytePtr);
                    Marshal.Copy(plcBytePtr, totalBytes, tempRequestLength * i, tempRequestLength);
                }

                ValidByteToStatusValue(totalBytes);
                var a = 1;
                HandwareParamaterCallback?.Invoke(PLCDBStatus);
                Marshal.FreeHGlobal(plcBytePtr);
                return true;
            }
            catch (Exception ex)
            {
                this.IsConnection = false;
                CommonDeleget.HandTaskException(ex);
                return false;
            }
        }
        /// <summary>
        /// 查询剂量
        /// </summary>
        protected override void InquireDose(byte[] DoseBytes)
        {
            ushort[] doseShorts = new ushort[50 / 2];
            for (int i = 0; i < 50; i+=2)
            {
                ushort Dose = (ushort)((DoseBytes[i] << 8) + DoseBytes[i+1]);
                doseShorts[i / 2] = Dose;
            }
            if(doseShorts[9] != BSGlobalDoseStatus[9])
            {
                Log.GetDistance().WriteInfoLogs($"InquireDose: new value{doseShorts[9]},old Value{Common.GlobalDoseStatus[9]}");
            }
            BSGlobalDoseStatus = doseShorts.ToList();
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
                if (BSGlobalRetStatus[index] != batchBool[index])
                {
                    //if(index == 114)
                    {
                        Log.GetDistance().WriteInfoLogs($"InquirePositionStatus: Index:{index},value:{ batchBool[index]},Postition:M{(index / 8 + 1)}.{index % 8}");
                    }
                    BSGlobalRetStatus[index] = batchBool[index];
                }
            }
        }
        

        ///通过枚举获取点位状态
        public override bool GetStatusByPositionEnum(PLCPositionEnum pLCPositionEnum)
        {
            if (PlcPositionEnum.ContainsKey(pLCPositionEnum))
            {
                return BSGlobalRetStatus[PlcPositionEnum[pLCPositionEnum]];
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
                return BSGlobalRetStatus[PlcPositionEnum[pLCPositionEnum]];
            }
            else
            {
                return false;
            }
        }
        public override short GetStatusByDIntPositionEnum(PLCPositionEnum pLCPositionEnum)
        {
            if (PlcShortPositionEnum.ContainsKey(pLCPositionEnum))
            {
                //return PLCDBStatus.IntArray[PlcShortPositionEnum[pLCPositionEnum]];
                return 0;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 通过枚举获取点位状态
        /// </summary>
        /// <param name="plcPositionEnuStr"></param>
        /// <returns></returns>
        public override short GetStatusByDIntPositionEnum(string plcPositionEnuStr)
        {
            PLCPositionEnum pLCPositionEnum = PLCPositionEnum.AcceleratorDoorLimit1;
            Enum.TryParse(plcPositionEnuStr, out pLCPositionEnum);
            if (PlcShortPositionEnum.ContainsKey(pLCPositionEnum))
            {
                return PLCDBStatus.IntArray[PlcShortPositionEnum[pLCPositionEnum]];
            }
            else
            {
                return 0;
            }
        }

        public override float GetStatusByFloatPositionEnum(PLCPositionEnum pLCPositionEnum)
        {
            if (PlcFloatPositionEnum.ContainsKey(pLCPositionEnum))
            {
                return PLCDBStatus.FloatArray[PlcFloatPositionEnum[pLCPositionEnum]];
            }
            else
            {
                return 0.0f;
            }
        }
        ///通过枚举获取点位状态
        public override float GetStatusByFloatPositionEnum(string plcPositionEnuStr)
        {
            PLCPositionEnum pLCPositionEnum = PLCPositionEnum.AcceleratorDoorLimit1;
            Enum.TryParse(plcPositionEnuStr, out pLCPositionEnum);
            if (PlcFloatPositionEnum.ContainsKey(pLCPositionEnum))
            {
                return PLCDBStatus.FloatArray[PlcFloatPositionEnum[pLCPositionEnum]];
            }
            else
            {
                return 0f;
            }
        }

        public override int GetStatusBUIntPositionEnum(PLCPositionEnum pLCPositionEnum)
        {
            if (PlcIntPositionEnum.ContainsKey(pLCPositionEnum))
            {
                return PLCDBStatus.DIntArray[PlcIntPositionEnum[pLCPositionEnum]];
            }
            else
            {
                return 0;
            }
        }
        ///通过枚举获取点位状态
        public override int GetStatusBUIntPositionEnum(string plcPositionEnuStr) 
        {
            PLCPositionEnum pLCPositionEnum = PLCPositionEnum.AcceleratorDoorLimit1;
            Enum.TryParse(plcPositionEnuStr, out pLCPositionEnum);
            if (PlcIntPositionEnum.ContainsKey(pLCPositionEnum))
            {
                return PLCDBStatus.DIntArray[PlcIntPositionEnum[pLCPositionEnum]];
            }
            else
            {
                return 0;
            }
        }
    }
}
