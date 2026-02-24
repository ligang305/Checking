using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BGCommunication
{
    public static class BGSerialPortImportDll
    {
        /*回调函数函数体*/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void SerialMsgCallback(uint serial_id, IntPtr serial_info, uint length);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void SerialErrorCallback(uint serial_id, uint error_code);
//#if DEBUG
//        [DllImport("BGSerialPort.dll", EntryPoint = "BGSPT_Create", ExactSpelling = false, CharSet = CharSet.Ansi,
//            CallingConvention = CallingConvention.Cdecl)]
//        public static extern uint BGSPT_Create();

//        [DllImport("BGSerialPort.dll", EntryPoint = "BGSPT_Open", ExactSpelling = false, CharSet = CharSet.Ansi,
//            CallingConvention = CallingConvention.Cdecl)]
//        public static extern bool BGSPT_Open(uint id, int Port, int BaudRate, int ByteSize, int Parity, int StopBit);

//        [DllImport("BGSerialPort.dll", EntryPoint = "BGSPT_Close", ExactSpelling = false, CharSet = CharSet.Ansi,
//            CallingConvention = CallingConvention.Cdecl)]
//        public static extern bool BGSPT_Close(uint id);

//        [DllImport("BGSerialPort.dll", EntryPoint = "BGSPT_Write", ExactSpelling = false, CharSet = CharSet.Ansi,
//            CallingConvention = CallingConvention.Cdecl)]
//        public static extern bool BGSPT_Write(uint id, byte[] sendchar, int sendsize);

//        [DllImport("BGSerialPort.dll", EntryPoint = "BGSPT_SetSerialMsgEvent", ExactSpelling = false, CharSet = CharSet.Ansi,
//            CallingConvention = CallingConvention.Cdecl)]
//        public static extern bool BGSPT_SetSerialMsgEvent(uint id, SerialMsgCallback callback);
//#else
        [DllImport("BGSerialPort.dll", EntryPoint = "BGSPT_Create", CharSet = CharSet.Ansi,
           CallingConvention = CallingConvention.Cdecl)]
        public static extern uint BGSPT_Create();

        [DllImport("BGSerialPort.dll", EntryPoint = "BGSPT_Open", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern bool BGSPT_Open(uint id, int Port, int BaudRate, int ByteSize, int Parity, int StopBit);

        [DllImport("BGSerialPort.dll", EntryPoint = "BGSPT_Close", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern bool BGSPT_Close(uint id);
         
        [DllImport("BGSerialPort.dll", EntryPoint = "BGSPT_Write", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern bool BGSPT_Write(uint id, byte[] sendchar, int sendsize);

        [DllImport("BGSerialPort.dll", EntryPoint = "BGSPT_SetSerialMsgEvent", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern bool BGSPT_SetSerialMsgEvent(uint id, SerialMsgCallback callback);
//#endif


        /**
        * \brief	打开串口
        * \param	id：索引id
        * \param	Port：端口号
        * \param	BaudRate：波特率
        * \param	ByteSize：数据位
        * \param	Parity:检验位
        * \param	StopBits：停止位
        * \return  true/false
*/
    }
}
