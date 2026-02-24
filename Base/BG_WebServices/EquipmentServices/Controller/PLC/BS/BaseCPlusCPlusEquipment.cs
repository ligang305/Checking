using BG_Entities;
using BG_WorkFlow;
using BGCommunication;
using BGLogs;
using BGModel;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static BGCommunication.PLCClientCallback;

namespace BG_Services.EquipmentServices.Controller
{
    public class CPlusCPlusEquipment : BasePLCEquipment
    {
        protected static RecvCallback recvCallback;
        protected string ParamaterAddress = "DB2.0";
        protected int ThreadSleepTime = 50;
        protected int protocolIndex = 0;
        protected Tuple<byte, int, int> block;
  
        public CPlusCPlusEquipment()
        {
            protocolIndex = PlcClientServices.SetPLCProtocolType(0);//1
            recvCallback = ReceiveCallback;
            PlcClientServices.Set_BufferRevCallback(protocolIndex, recvCallback);//1  callback 1 
            bool InitResult = PlcClientServices.InitCommonProtocol(protocolIndex, 0);

            CommonDeleget.SetCommondEvent += WritePositionValue;
            CommonDeleget.SetDoseValueEvent += WritePositionValue;
            block = PLCTools.GetInstance().GetBlockTypeForCPlusPlus(ConfigServices.GetInstance().localConfigModel.InquireStartPosition);
        }

        public override void Load(ControlVersion cv, PLCProtocol commonProtocol)
        {
            PlcConnectionCallback?.Invoke();
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
                if (!CommonFunc.PingIp(IpAddress))
                {
                    IsConnection = CommonFunc.PingIp(IpAddress);
                }
                if (!PlcClientServices.IsConnectedClient(protocolIndex))
                {
                    Common.PlcIsInit = false;
                    Common.IsConnection = false;
                    Common.IsConnection = PlcClientServices.ConnectedClient(protocolIndex, IpAddress,
                        Convert.ToUInt16(Port));
                }
                if (!Common.PlcIsInit)
                {
                    Common.PlcIsInit = PlcClientServices.InitPlc(protocolIndex);
                    if (Common.PlcIsInit)
                    {
                        //TODO 如果检测到PLC断了就去重新赋默认值
                        CommonDeleget.InitParameterAction();
                    }
                }
            }
        }

        /// <summary>
        /// 判断连接状态
        /// </summary>
        /// <returns></returns>
        public override bool IsConnect()
        {
            return PlcClientServices.IsConnectedClient(protocolIndex);
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        public override void DisConnect()
        {
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
                short RequestLength = Convert.ToInt16(66 + ConfigServices.GetInstance().localConfigModel.CurrentPosition);
                IntPtr plcBytePtr = Marshal.AllocHGlobal(RequestLength);
                Stopwatch stopwatch = Stopwatch.StartNew();
                Common.IsSearchStatusSuccess = PlcClientServices.ReadBytes(protocolIndex, block.Item1, (byte)block.Item2, (byte)block.Item3, RequestLength, ref plcBytePtr);
          
                byte[] totalBytes = new byte[RequestLength];
                Marshal.Copy(plcBytePtr, totalBytes, 0, RequestLength);
                byte[] boolBytes = new byte[16];
                byte[] doseBytes = new byte[50];
                Array.Copy(totalBytes, 0, boolBytes, 0, 16);
                Array.Copy(totalBytes, 16 + ConfigServices.GetInstance().localConfigModel.CurrentPosition, doseBytes, 0, 50);
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
        /// 查询剂量
        /// </summary>
        protected virtual void InquireDose(byte[] DoseBytes)
        {
           
        }
        /// <summary>
        /// 查询点位
        /// </summary>
        protected virtual void InquireBool(byte[] BoolBytes)
        {
           
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
                short RequestLength = 240;
                int muti = 3;
                short tempRequestLength = Convert.ToInt16(RequestLength / muti);
                Tuple<byte, int, int> block = PLCTools.GetInstance().GetBlockTypeForCPlusPlus(ConfigServices.GetInstance().localConfigModel.InquireStartPosition);
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
                HandwareParamaterCallback?.Invoke(PLCDBStatus);
                Marshal.FreeHGlobal(plcBytePtr);
                return true;
            }
            catch (Exception ex)
            {
                Common.IsConnection = false; 
                CommonDeleget.HandTaskException(ex);
                return false;
            }
        }

        protected void ValidByteToStatusValue(byte[] statusValue)
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
            Tuple<byte, int, int> block = PLCTools.GetInstance().GetBlockTypeForCPlusPlus(Postion);
            string plcValue = PlcClientServices.GetString(protocolIndex, block.Item1, (ushort)block.Item2, (ushort)block.Item3, (short)length);
            return plcValue;
        }

        /// <summary>
        /// 直接读取
        /// </summary>
        /// <returns></returns>
        public override bool ReadPositionValue(string positionItem)
        {
            return false;
        }
        /// <summary>
        /// 将值写入到具体点位
        /// </summary>
        /// <param name="Position">例如M21.1之类的</param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public override bool WritePositionValue(string Position, bool Value)
        {
            // Position M21.3
            Tuple<byte, int, int> block = PLCTools.GetInstance().GetBlockTypeForCPlusPlus(Position);
            PlcClientServices.WriteByteBool(protocolIndex, (ushort)block.Item2, (ushort)block.Item3, block.Item1, Value);
            return true;
        }

        public override bool WritePositionValue(byte Position, ushort Value)
        {
            ushort BytePosition = (byte)(Position / 8);
            ushort BitPosition = (byte)(Position % 8);
            PlcClientServices.WriteByteUINT16(protocolIndex, BytePosition, BitPosition, 0x83, Value);
            return true;
        }
        public override bool WritePositionValue(string Position, UInt32 Value)
        {
            // Position DB21.3
            Tuple<byte, int, int> block = PLCTools.GetInstance().GetBlockTypeForCPlusPlus(Position);
            PlcClientServices.WriteByteUINT32(protocolIndex, (byte)block.Item2, (byte)block.Item3, block.Item1, Value);
            return true;
        }
        public override bool WritePositionValue(byte Position, UInt32 Value)
        {
            ushort BytePosition = (ushort)(Position / 8);
            ushort BitPosition = (ushort)(Position % 8);
            PlcClientServices.WriteByteUINT32(protocolIndex, BytePosition, BitPosition, 0x83, Value);
            return true;
        }
        public override bool WritePositionValue(string Position, float Value)
        {
            // Position DB21.3
            Tuple<byte, int, int> block = PLCTools.GetInstance().GetBlockTypeForCPlusPlus(Position);
            PlcClientServices.WriteByteFloat(protocolIndex, (ushort)block.Item2, (ushort)block.Item3, block.Item1, Value);
            return true;
        }
        public override bool WritePositionValue(byte Position, float Value)
        {
            ushort BytePosition = (ushort)(Position / 8);
            ushort BitPosition = (ushort)(Position % 8);
            PlcClientServices.WriteByteFloat(protocolIndex, BytePosition, BitPosition, 0x83, Value);
            return true;
        }


        public override void UnLoad()
        {

        }
        public override string GetPlcBlockStr(string BlockPosition, ushort Strlength, InqureType _InqureType = InqureType.IString)
        {
            string plcValue = string.Empty;
            Tuple<byte, int, int> block = PLCTools.GetInstance().GetBlockTypeForCPlusPlus(BlockPosition);
            if (_InqureType == InqureType.IString)
            {
                plcValue = PlcClientServices.GetString(protocolIndex, block.Item1, (ushort)block.Item2, (ushort)block.Item3, (short)Strlength);
            }
            else if(_InqureType == InqureType.IUInt32)
            {
                plcValue = PlcClientServices.GetUInt32(protocolIndex, block.Item1, (ushort)block.Item2, (ushort)block.Item3, (short)Strlength);
            }
            return plcValue;
        }

        public override string GetCurrentEquipmentVersion()
        {
            return GetPlcBlockStr(ConfigServices.GetInstance().localConfigModel.EquipmentAddress, Convert.ToUInt16(ConfigServices.GetInstance().localConfigModel.EquipmentLength));
        }

        public override string GetCurrentEquipmentModel()
        {
            return PlcClientServices.GetProtocolTypeName(protocolIndex);
        }

        public static int ReceiveCallback(int protocolIndex, IntPtr buffer, int length)
        {
            byte[] szbuf = new byte[length];
            Marshal.Copy(buffer, szbuf, 0, length);
            return 0;
        }
    }
}
