using BGLogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static BGCommunication.PLCClientCallback;

namespace BGCommunication
{
    public class PlcClientServices
    {
        #region Lazy Singleton

        private static PlcClientServices _instance = null;

        private static readonly object SyncRoot = new object();

        private PlcClientServices()
        {

        }

        public static PlcClientServices GetIns()
        {
            if (_instance == null)
            {
                lock (SyncRoot)
                {
                    if (_instance == null)
                    {
                        _instance = new PlcClientServices();
                    }
                }
            }
            return _instance;
        }
        #endregion

        #region DllImport Methods

        /// <summary>
        /// 设置协议类型
        /// </summary>
        /// <param name="ProtocolType">1：S7；2：S300</param>
        [DllImport("PLC_Client.dll", EntryPoint = "SetPLCProtocolType", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetPLCProtocolType(int ProtocolType);
        /// <summary>
        /// 初始化协议
        /// </summary>
        /// <param name="ProtocolType">传入协议ID</param>
        [DllImport("PLC_Client.dll", EntryPoint = "InitCommonProtocol", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool InitCommonProtocol(int ProtocolIndex, int CommunctionProtocolType);


        /// <summary>
        /// 初始化协议
        /// </summary>
        /// <param name="ProtocolType">传入协议ID</param>
        [DllImport("PLC_Client.dll", EntryPoint = "IsConnectedClient", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool IsConnectedClient(int ProtocolIndex);

        /// <summary>
        /// 初始化协议
        /// </summary>
        [DllImport("PLC_Client.dll", EntryPoint = "ConnectedClient", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool ConnectedClient(int ProtocolIndex, string Ip_Address, ushort Ip_Port);

        /// <summary>
        /// 获取PLC类型
        /// </summary>
        [DllImport("PLC_Client.dll", EntryPoint = "GetPLCProtocolType", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GetPLCProtocolType(int ProtocolIndex, ref IntPtr size);

        /// <summary>
        /// 初始化PLC
        /// </summary>
        [DllImport("PLC_Client.dll", EntryPoint = "InitPlc", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool InitPlc(int ProtocolIndex);

        /// <summary>
        /// 初始化PLC第一步
        /// </summary>
        [DllImport("PLC_Client.dll", EntryPoint = "InitFirstStep", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void InitFirstStep();

        /// <summary>
        /// 初始化PLC第二步
        /// </summary>
        [DllImport("PLC_Client.dll", EntryPoint = "InitSecondStep", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void InitSecondStep();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="BytePosition"></param>
        /// <param name="BitPosition"></param>
        /// <param name="DataBlock"></param>
        /// <param name="value"></param>
        [DllImport("PLC_Client.dll", EntryPoint = "WriteByteBool", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void WriteByteBool(int ProtocolIndex, ushort BytePosition, ushort BitPosition, byte DataBlock, bool value);

        /// <summary>
        /// 写入单字节
        /// </summary>
        /// <param name="BytePosition"></param>
        /// <param name="BitPosition"></param>
        /// <param name="DataBlock"></param>
        /// <param name="value"></param>
        [DllImport("PLC_Client.dll", EntryPoint = "WriteByteChar", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void WriteByteChar(int ProtocolIndex, ushort BytePosition, ushort BitPosition, byte DataBlock, string value);

        /// <summary>
        /// 写入Double
        /// </summary>
        /// <param name="BytePosition"></param>
        /// <param name="BitPosition"></param>
        /// <param name="DataBlock"></param>
        /// <param name="value"></param>
        [DllImport("PLC_Client.dll", EntryPoint = "WriteByteDouble", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void WriteByteDouble(int ProtocolIndex, ushort BytePosition, ushort BitPosition, byte DataBlock, double value);
        /// <summary>
        /// 写入Float
        /// </summary>
        /// <param name="BytePosition"></param>
        /// <param name="BitPosition"></param>
        /// <param name="DataBlock"></param>
        /// <param name="value"></param>
        [DllImport("PLC_Client.dll", EntryPoint = "WriteByteFloat", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void WriteByteFloat(int ProtocolIndex, ushort BytePosition, ushort BitPosition, byte DataBlock, Single value);
        /// <summary>
        /// 写入UINT32
        /// </summary>
        /// <param name="BytePosition"></param>
        /// <param name="BitPosition"></param>
        /// <param name="DataBlock"></param>
        /// <param name="value"></param>
        [DllImport("PLC_Client.dll", EntryPoint = "WriteByteUINT32", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void WriteByteUINT32(int ProtocolIndex, ushort BytePosition, ushort BitPosition, byte DataBlock, UInt32 value);
        /// <summary>
        /// 写入UINT32
        /// </summary>
        /// <param name="BytePosition"></param>
        /// <param name="BitPosition"></param>
        /// <param name="DataBlock"></param>
        /// <param name="value"></param>
        [DllImport("PLC_Client.dll", EntryPoint = "WriteByteUInt16", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void WriteByteUINT16(int ProtocolIndex, ushort BytePosition, ushort BitPosition, byte DataBlock, UInt16 value);
        /// <summary>
        /// 写入long
        /// </summary>
        /// <param name="BytePosition"></param>
        /// <param name="BitPosition"></param>
        /// <param name="DataBlock"></param>
        /// <param name="value"></param>
        [DllImport("PLC_Client.dll", EntryPoint = "WriteByteLong", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void WriteByteLong(int ProtocolIndex, ushort BytePosition, ushort BitPosition, byte DataBlock, long value);
        /// <summary>
        /// 读取字节
        /// </summary>
        /// <param name="BytePosition"></param>
        /// <param name="BitPosition"></param>
        /// <param name="DataBlock"></param>
        /// <param name="value"></param>
        [DllImport("PLC_Client.dll", EntryPoint = "ReadBytes", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool ReadBytes(int ProtocolIndex, byte DataBlock, ushort BytePosition, ushort BitPosition, Int16 value, ref IntPtr AnalyBytes);

        /// <summary>
        /// 读取字节
        /// </summary>
        /// <param name="BytePosition"></param>
        /// <param name="BitPosition"></param>
        /// <param name="DataBlock"></param>
        /// <param name="value"></param>
        [DllImport("PLC_Client.dll", EntryPoint = "ReadInt", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool ReadInt(int ProtocolIndex, byte DataBlock, ushort startBytePosition, ushort StartBitPosition, Int16 ByteLength, out int value);

        /// <summary>
        /// 读取字节
        /// </summary>
        /// <param name="BytePosition"></param>
        /// <param name="BitPosition"></param>
        /// <param name="DataBlock"></param>
        /// <param name="value"></param>
        [DllImport("PLC_Client.dll", EntryPoint = "ReadFloat", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool ReadFloat(int ProtocolIndex, byte DataBlock, ushort startBytePosition, ushort StartBitPosition, Int16 ByteLength, out float value);

        /// <summary>
        /// 读取字节
        /// </summary>
        /// <param name="BytePosition"></param>
        /// <param name="BitPosition"></param>
        /// <param name="DataBlock"></param>
        /// <param name="value"></param>
        [DllImport("PLC_Client.dll", EntryPoint = "ReadDouble", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool ReadDouble(int ProtocolIndex, byte DataBlock, ushort startBytePosition, ushort StartBitPosition, Int16 ByteLength, out double value);

        /// <summary>
        /// 读取字节
        /// </summary>
        /// <param name="BytePosition"></param>
        /// <param name="BitPosition"></param>
        /// <param name="DataBlock"></param>
        /// <param name="value"></param>
        [DllImport("PLC_Client.dll", EntryPoint = "ReadString", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool ReadString(int ProtocolIndex, byte DataBlock, ushort startBytePosition, ushort StartBitPosition, Int16 ByteLength, ref IntPtr value);

        /// <summary>
        /// 读取字节
        /// </summary>
        /// <param name="BytePosition"></param>
        /// <param name="BitPosition"></param>
        /// <param name="DataBlock"></param>
        /// <param name="value"></param>
        [DllImport("PLC_Client.dll", EntryPoint = "ReadBatchBool", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool ReadBatchBool(int ProtocolIndex, byte DataBlock, ushort startBytePosition, ushort StartBitPosition, Int16 ByteLength, ref IntPtr value, int arrayLength);
        #endregion

        /// <summary>
        /// 设置报文回调
        /// </summary>
        /// <param name="ProtocolIndex"></param>
        /// <param name="_RecvCallback"></param>
        [DllImport("PLC_Client.dll", EntryPoint = "Set_BufferRevCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Set_BufferRevCallback(int ProtocolIndex, RecvCallback _RecvCallback);

        public static string GetProtocolTypeName(int ProtocolIndex)
        {
            IntPtr protocolTypeName = Marshal.AllocHGlobal(1024);
            GetPLCProtocolType(ProtocolIndex, ref protocolTypeName);
            string result = Marshal.PtrToStringAnsi(protocolTypeName);
            Marshal.FreeHGlobal(protocolTypeName);
            return result;
        }
        [HandleProcessCorruptedStateExceptions]
        public static string GetString(int protocolIndex, byte BlockType, ushort DataPosition, ushort DataBitPosition, short Strlength)
        {
            try
            {
                byte[] stringValueBytes = new byte[Strlength];
                IntPtr stringValue = ArrToPtr(stringValueBytes);// Marshal.AllocHGlobal(Strlength);
                ReadString(protocolIndex, BlockType, DataPosition, DataBitPosition, Strlength, ref stringValue);
                string result = Marshal.PtrToStringAnsi(stringValue);
                result = result.Length > Strlength ? result.Substring(0, Strlength) : result;
                //Marshal.FreeHGlobal(stringValue);
                return result;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static string GetUInt32(int protocolIndex, byte BlockType, ushort DataPosition, ushort DataBitPosition, short Strlength)
        {
            int value = 0;
            ReadInt(protocolIndex, BlockType, DataPosition, DataBitPosition, Strlength, out value);
            return value.ToString();
        }
        private static IntPtr ArrToPtr(byte[] array)
        {
            return System.Runtime.InteropServices.Marshal.UnsafeAddrOfPinnedArrayElement(array, 0);
        }
    }
}
