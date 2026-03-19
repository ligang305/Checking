using BGLogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static BGCommunication.ConnectInterface;

namespace BGCommunication
{
    public abstract class ICommonProtocol
    {
        #region CRC校验
        /// <summary>
        /// CRC高位校验码checkCRCHigh
        /// </summary>
        public static byte[] ArrayCRCHigh =
        {
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
        0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
        0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
        0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
        0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
        0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
        0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
        0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
        0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
        0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
        0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
        0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
        0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
        0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
        0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
        0x80, 0x41, 0x00, 0xC1, 0x81, 0x40
        };

        /// <summary>
        /// CRC地位校验码checkCRCLow
        /// </summary>
        public static byte[] checkCRCLow =
        {
        0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06,
        0x07, 0xC7, 0x05, 0xC5, 0xC4, 0x04, 0xCC, 0x0C, 0x0D, 0xCD,
        0x0F, 0xCF, 0xCE, 0x0E, 0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09,
        0x08, 0xC8, 0xD8, 0x18, 0x19, 0xD9, 0x1B, 0xDB, 0xDA, 0x1A,
        0x1E, 0xDE, 0xDF, 0x1F, 0xDD, 0x1D, 0x1C, 0xDC, 0x14, 0xD4,
        0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2, 0x12, 0x13, 0xD3,
        0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3,
        0xF2, 0x32, 0x36, 0xF6, 0xF7, 0x37, 0xF5, 0x35, 0x34, 0xF4,
        0x3C, 0xFC, 0xFD, 0x3D, 0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A,
        0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38, 0x28, 0xE8, 0xE9, 0x29,
        0xEB, 0x2B, 0x2A, 0xEA, 0xEE, 0x2E, 0x2F, 0xEF, 0x2D, 0xED,
        0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26,
        0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0, 0xA0, 0x60,
        0x61, 0xA1, 0x63, 0xA3, 0xA2, 0x62, 0x66, 0xA6, 0xA7, 0x67,
        0xA5, 0x65, 0x64, 0xA4, 0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F,
        0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB, 0x69, 0xA9, 0xA8, 0x68,
        0x78, 0xB8, 0xB9, 0x79, 0xBB, 0x7B, 0x7A, 0xBA, 0xBE, 0x7E,
        0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C, 0xB4, 0x74, 0x75, 0xB5,
        0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71,
        0x70, 0xB0, 0x50, 0x90, 0x91, 0x51, 0x93, 0x53, 0x52, 0x92,
        0x96, 0x56, 0x57, 0x97, 0x55, 0x95, 0x94, 0x54, 0x9C, 0x5C,
        0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E, 0x5A, 0x9A, 0x9B, 0x5B,
        0x99, 0x59, 0x58, 0x98, 0x88, 0x48, 0x49, 0x89, 0x4B, 0x8B,
        0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C,
        0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42,
        0x43, 0x83, 0x41, 0x81, 0x80, 0x40
        };
        /// <summary>
        /// CRC校验
        /// </summary>
        /// <param name="data">校验的字节数组</param>
        /// <param name="length">校验的数组长度</param>
        /// <returns>该字节数组的奇偶校验字节</returns>
        public static byte[] CRC16(byte[] data, int arrayLength)
        {
            byte CRCHigh = 0xFF;
            byte CRCLow = 0xFF;
            byte index;
            int i = 0;
            while (arrayLength-- > 0)
            {
                index = (System.Byte)(CRCHigh ^ data[i++]);
                CRCHigh = (System.Byte)(CRCLow ^ ArrayCRCHigh[index]);
                CRCLow = checkCRCLow[index];
            }

            byte[] crc = new byte[2] { CRCHigh, CRCLow };
            return crc;
        }
        #endregion

        #region CRC 双字节
        public static ushort[] CRC16Table = new ushort[256] {
        0x0000, 0xC0C1, 0xC181, 0x0140, 0xC301, 0x03C0, 0x0280, 0xC241,
        0xC601, 0x06C0, 0x0780, 0xC741, 0x0500, 0xC5C1, 0xC481, 0x0440,
        0xCC01, 0x0CC0, 0x0D80, 0xCD41, 0x0F00, 0xCFC1, 0xCE81, 0x0E40,
        0x0A00, 0xCAC1, 0xCB81, 0x0B40, 0xC901, 0x09C0, 0x0880, 0xC841,
        0xD801, 0x18C0, 0x1980, 0xD941, 0x1B00, 0xDBC1, 0xDA81, 0x1A40,
        0x1E00, 0xDEC1, 0xDF81, 0x1F40, 0xDD01, 0x1DC0, 0x1C80, 0xDC41,
        0x1400, 0xD4C1, 0xD581, 0x1540, 0xD701, 0x17C0, 0x1680, 0xD641,
        0xD201, 0x12C0, 0x1380, 0xD341, 0x1100, 0xD1C1, 0xD081, 0x1040,
        0xF001, 0x30C0, 0x3180, 0xF141, 0x3300, 0xF3C1, 0xF281, 0x3240,
        0x3600, 0xF6C1, 0xF781, 0x3740, 0xF501, 0x35C0, 0x3480, 0xF441,
        0x3C00, 0xFCC1, 0xFD81, 0x3D40, 0xFF01, 0x3FC0, 0x3E80, 0xFE41,
        0xFA01, 0x3AC0, 0x3B80, 0xFB41, 0x3900, 0xF9C1, 0xF881, 0x3840,
        0x2800, 0xE8C1, 0xE981, 0x2940, 0xEB01, 0x2BC0, 0x2A80, 0xEA41,
        0xEE01, 0x2EC0, 0x2F80, 0xEF41, 0x2D00, 0xEDC1, 0xEC81, 0x2C40,
        0xE401, 0x24C0, 0x2580, 0xE541, 0x2700, 0xE7C1, 0xE681, 0x2640,
        0x2200, 0xE2C1, 0xE381, 0x2340, 0xE101, 0x21C0, 0x2080, 0xE041,
        0xA001, 0x60C0, 0x6180, 0xA141, 0x6300, 0xA3C1, 0xA281, 0x6240,
        0x6600, 0xA6C1, 0xA781, 0x6740, 0xA501, 0x65C0, 0x6480, 0xA441,
        0x6C00, 0xACC1, 0xAD81, 0x6D40, 0xAF01, 0x6FC0, 0x6E80, 0xAE41,
        0xAA01, 0x6AC0, 0x6B80, 0xAB41, 0x6900, 0xA9C1, 0xA881, 0x6840,
        0x7800, 0xB8C1, 0xB981, 0x7940, 0xBB01, 0x7BC0, 0x7A80, 0xBA41,
        0xBE01, 0x7EC0, 0x7F80, 0xBF41, 0x7D00, 0xBDC1, 0xBC81, 0x7C40,
        0xB401, 0x74C0, 0x7580, 0xB541, 0x7700, 0xB7C1, 0xB681, 0x7640,
        0x7200, 0xB2C1, 0xB381, 0x7340, 0xB101, 0x71C0, 0x7080, 0xB041,
        0x5000, 0x90C1, 0x9181, 0x5140, 0x9301, 0x53C0, 0x5280, 0x9241,
        0x9601, 0x56C0, 0x5780, 0x9741, 0x5500, 0x95C1, 0x9481, 0x5440,
        0x9C01, 0x5CC0, 0x5D80, 0x9D41, 0x5F00, 0x9FC1, 0x9E81, 0x5E40,
        0x5A00, 0x9AC1, 0x9B81, 0x5B40, 0x9901, 0x59C0, 0x5880, 0x9841,
        0x8801, 0x48C0, 0x4980, 0x8941, 0x4B00, 0x8BC1, 0x8A81, 0x4A40,
        0x4E00, 0x8EC1, 0x8F81, 0x4F40, 0x8D01, 0x4DC0, 0x4C80, 0x8C41,
        0x4400, 0x84C1, 0x8581, 0x4540, 0x8701, 0x47C0, 0x4680, 0x8641,
        0x8201, 0x42C0, 0x4380, 0x8341, 0x4100, 0x81C1, 0x8081, 0x4040
    };

        /*****************************************************
        描述：  CRC16校验子程序，校验方式CRC-16/MODBUS
        入口参数：  指向数组指针，校验字节个数
        出口参数：  16位CRC校验码
        ******************************************************/
        protected ushort CRC16_Modbus(byte[] p, ushort length)
        {
            ushort checksum = 0xffff;
            int i = 0;
            for (; length > 0; length--)
            {
                ushort tmp = p[i++];
                checksum = (ushort)((checksum >> 8) ^ CRC16Table[(checksum & 0xFF) ^ tmp]);
            }
            return checksum;
        }

        /*****************************************************
        描述：  CRC16校验子程序，校验方式CRC-16/IBM
        入口参数：  指向数组指针，校验字节个数
        出口参数：  16位CRC校验码
        ******************************************************/
        protected ushort CRC16_IBM(byte[] p, ushort length)
        {
            ushort checksum = 0;
            int i = 0;
            for (; length > 0; length--)
            {
                ushort tmp = p[i++];
                checksum = (ushort)((checksum >> 8) ^ CRC16Table[(checksum & 0xFF) ^ tmp]);
            }
            return checksum;
        }
        #endregion

        #region 字节字符串转换
        //字节数组转16进制字符串
        public static string ToHexString(byte[] bytes) // 0xae00cf => "AE00CF "
        {
            string hexString = string.Empty;
            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    strB.Append(bytes[i].ToString("X2"));
                    strB.Append(" ");
                }
                hexString = strB.ToString();
            }
            return hexString;   
        }

        //16进制字符串转字节数组
        public static byte[] GetBytes(string hexString, out int discarded)
        {
            discarded = 0;
            string newString = "";
            char c;
            // remove all none A-F, 0-9, characters
            for (int i = 0; i < hexString.Length; i++)
            {
                c = hexString[i];
                if (Uri.IsHexDigit(c))
                    newString += c;
                else
                    discarded++;
            }
            // if odd number of characters, discard last character
            if (newString.Length % 2 != 0)
            {
                discarded++;
                newString = newString.Substring(0, newString.Length - 1);
            }

            int byteLength = newString.Length / 2;
            byte[] bytes = new byte[byteLength];
            string hex;
            int j = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                hex = new String(new Char[] { newString[j], newString[j + 1] });
                bytes[i] = Convert.ToByte(hex, 16);
                j = j + 2;
            }
            return bytes;
        }
        #endregion

        //通信接口
        protected ConnectInterface ConnIntf = null;

        //协议名称
        public string ProtocolName = "通信协议";

        public string FunctionName = "";
        
        //发送锁
        private Mutex SendMutex = new Mutex();
        //接收锁
        private Mutex RecvMutex = new Mutex();

        //发送命令是否需要反馈
        internal bool NeedFeedBack = false;

        //发送命令锁
        private Object sendlock = new Object();

        //服务器自动发来的数据
        protected string SendBackBuf = "";

        protected List<byte> CorrectRet = new List<byte>();

        //服务端执行命令成功标志
        protected bool ExeResult = false;
        
        //是否还在发送数据
        private int Sending = 0;
        public int IsSending { get { return Sending; } }

        //连续几次接收超时
        private byte TimeoutCnt = 0;
        //最大接受的连续回传接收超时次数
        private byte SendTimeoutFailCnt = 10;
        public void SetSendTimeoutFailCnt(byte cnt)
        {
            SendTimeoutFailCnt = cnt;
        }

        protected int WaitMS = 3000;
        //设置等待回传数据时间
        public int WaitRecTick
        {
            get { return WaitMS; }
            set { WaitMS = value; }
        }

        //上锁
        public void LockSend()
        {
            //if(this is PLCSIEMENS_S300Protocol)
            //{
            //    BGLogs.Log.GetDistance().WriteInfoLogs(GetCaller());
            //}
            Sending++;
            SendMutex.WaitOne();
        }
        //解锁
        public void UnlockSend()
        {
            SendMutex.ReleaseMutex();
            Sending--;
            //if (this is PLCSIEMENS_S300Protocol)
            //{
            //    BGLogs.Log.GetDistance().WriteInfoLogs(GetCaller());
            //}
        }
        //string str = string.Format("发送加锁开始 Sending={0}",Sending);
        //ConnIntf.DebugStepLog(str);
        public string GetCaller()
        {
            StackTrace st = new StackTrace(skipFrames: 1, fNeedFileInfo: true);
            StackFrame[] sfArray = st.GetFrames();
            return string.Join(" -> ",
                sfArray.Select(r =>
                  $"{r.GetMethod().Name} in {r.GetFileName()} line:{r.GetFileLineNumber()} column:{r.GetFileColumnNumber()}"));
        }
        //上锁
        public void LockRecv()
        {
            ConnIntf.DebugStepLog("接收加锁开始");
            RecvMutex.WaitOne();
            ConnIntf.DebugStepLog("接收加锁结束");
        }
        //解锁
        public void UnlockRecv()
        {
            ConnIntf.DebugStepLog("接收解锁开始");
            RecvMutex.ReleaseMutex();
            ConnIntf.DebugStepLog("接收解锁结束");
        }

        //获取协议名称
        public string GetProtocolName()
        {
            return ProtocolName;
        }

        //初始化通信接口
        public void InitConnection(ref ConnectInterface CI)
        {
            ConnIntf = CI;
            CI.CommonProtocol = this;
        }

        //连接通信
        public bool Connect()
        {
            if (ConnIntf == null) return false;
            return ConnIntf.Connect();
        }

        //通信是否连接
        public bool IsConnect()
        {
            if (ConnIntf != null)
            {
                return ConnIntf.IsConnect();
            }
            return false;
        }

        //断开通信连接
        public void DisConnect()
        {
            if (ConnIntf != null)
            {
                ConnIntf.DisConnect();
            }
        }

        //生成发送命令
        public virtual byte[] BuildCommand()
        {
            byte[] command = new byte[1];
            return command;
        }

        public virtual byte MBitAddrtoFlag(byte FlagM, byte FlagB)
        {
            if (FlagM < 11)
            {
                return 0x00;
            }
            if (FlagB > 7)
            {
                return 0x00;
            }

            byte start = 0xAF;
            byte tmp = (byte)((FlagM - 21) * 8 + FlagB);
            byte Flag = (byte)(start + tmp);
            if (Flag > 0xED)
            {
                return 0x00;
            }

            return Flag;
        }

        public virtual byte[] InitPLCCommand(int Index)
        {
            byte[] command = new byte[1];
            return command;
        }

        //生成发送命令
        public virtual string BuildSendBackCommand()
        {
            SendBackBuf = "";
            return SendBackBuf;
        }

        //命令含义
        public virtual string CommandMeaning()
        {
            string str = "";
            return str;
        }

        //接收回传数据处理函数
        internal virtual void OnRecv(byte[] buffer,int size)
        {
            if (NeedFeedBack)
            {
                NeedFeedBack_ResolveBackCommand(buffer, size);
            }
            else
            {
                UnNeedFeedBack_ResolveCommand(buffer, size);
            }
        }

        protected void UnNeedFeedBack_ResolveCommand(byte[] buffer, int size)
        {
            LockRecv();
            try
            {
                if (ResolveCommand(buffer, size))
                {
                    ConnIntf.DebugStepLog(FunctionName + "-解析接收数据成功");
                    RecvActionEvent?.Invoke();

                    if (SendBackBuf != "")
                    {
                        byte[] backbuf = Encoding.ASCII.GetBytes(SendBackBuf);
                        PrepareSendBack(backbuf);
                        int nRet = ConnIntf.SendMsg(backbuf);
                    }
                }
                else
                {
                    ConnIntf.DebugStepLog(FunctionName + "-解析接收数据失败");
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                ConnIntf.WriteError("接收异常", ex.Message);
                ResolveProtocolError?.Invoke(ex.Message);
            }

            UnlockRecv();
        }

        protected void NeedFeedBack_ResolveBackCommand(byte[] buffer, int size)
        {
            try
            {
                //ConnIntf.WriteDebug(ProtocolName + $"{size}",$"{ProtocolName} fh");
                if (ResolveBackCommand(buffer, size))
                {
                    //onnIntf.WriteError(FunctionName, "-Receive Callback");
                    RecvBackActionEvent?.Invoke();
                }
                else
                {
                    ConnIntf.DebugStepLog(FunctionName + "-Receive Callback Faild ");
                }
            }
            catch (System.Exception ex)
            {
                ConnIntf.WriteError("NeedFeedBack_ResolveBack Error", ex.Message);
                Console.WriteLine(ex.Message);
                NeedFeedBack = false;
                ExeResult = false;
                if (ResolveProtocolError != null)
                {
                    ResolveProtocolError?.Invoke(ex.Message);
                }
            }
        }

        //解析服务端主动发送的命令
        protected virtual bool ResolveCommand(byte[] buffer, int size)
        {
            SendBackBuf = "";
            return true;
        }

        //解析服务端反馈回传的命令
        internal virtual bool ResolveBackCommand(byte[] buffer, int size)
        {
            NeedFeedBack = false;
            ExeResult = true;
            return true;
        }

        //发送命令
        public virtual bool SendCommand(byte[] sendcommand,int waitRecvTick = 0)
        {
            string stmp = "SendCommand--" + FunctionName;
            //ConnIntf.DebugStepLog(stmp);
            lock (sendlock)
            {
               // ConnIntf.DebugStepLog("sendlock");
                if (!IsConnect())
                {
                    //Debug.WriteLine("-------------------------ganggang send command-----------------------连接已断开");

                    ConnIntf.WriteError(stmp, "Connection DisConnection");
                    return false;
                }
                PrepareSend(sendcommand);
                if (NeedFeedBack)
                {
                    int nRet = ConnIntf.SendMsg(sendcommand);
                    if (nRet > 0)
                    {
                        ConnIntf.DebugLog(stmp, "wait receive data");
                        bool bRecved = false;
                        waitRecvTick = waitRecvTick == 0 ? WaitRecTick : waitRecvTick;
                        DateTime tick1 = DateTime.Now;
                        double Waittick = 0;
                        while (Waittick < waitRecvTick && IsConnect())
                        {
                            if (NeedFeedBack == false)
                            {
                                bRecved = true;
                                TimeoutCnt = 0;
                                ConnIntf.DebugLog(ProtocolName, $"{ProtocolName}Receive back data");
                                break;
                            }
                            DateTime tick2 = DateTime.Now;
                            Waittick = (tick2 - tick1).TotalMilliseconds;
                        }
                        if (!bRecved)
                        {
                            string sSendCommand = ToHexString(sendcommand);
                            ConnIntf.DebugLog(FunctionName + "Receiving and returning", "-接收命令【"+sSendCommand+ "】回传信息超时");
                            ResolveProtocolError?.Invoke(sSendCommand + "-Receive back data time out");
                            TimeoutCnt++; //SendTimeoutFailCnt
                            if (TimeoutCnt > SendTimeoutFailCnt)
                            {
                                DisConnect();
                            }
                        }
                        NeedFeedBack = false;
                        ConnIntf.DebugLog(ProtocolName, $"{ProtocolName}Receive back data");
                        //ConnIntf.DebugLog(stmp, "执行完成");
                        return ExeResult;
                    }
                    else
                    {
                        ConnIntf.DebugLog(stmp,"Send byte length 0");
                    }
                }
                else
                {
                    ConnIntf.DebugLog(stmp, "not need receive callbacl data");
                    int nRet = ConnIntf.SendMsg(sendcommand);
                    ConnIntf.DebugLog(stmp, "Execution complete");
                    return (nRet > 0);
                }
                       
                ConnIntf.DebugLog(stmp, "Execution complete~");
                return false;
            }
        }

        //发送命令前准备
        protected virtual void PrepareSend(byte[] SendBuf)
        {
            ConnIntf.SendedBuf.Clear();
            NeedFeedBack = false;
            ExeResult = false;
        }

        //发送命令前准备
        protected virtual void PrepareSendBack(byte[] SendbackBuf)
        {

        }

        //注册接收服务发来数据的回调函数，一般用于显示服务器传来的数据
        public void RegisterRecvMsgEvent(MessageEvent RecvFun)
        {
            if (ConnIntf == null)
            {
                throw new Exception("Please initialize the communication connection first");
            }
            ConnIntf.RecvMsgEvent -= RecvFun;
            ConnIntf.RecvMsgEvent += RecvFun;
        }

        //注册发送至服务端的数据的回调函数，一般用于显示发送至服务端的数据
        public void RegisterSendMsgEvent(MessageEvent SendFun)
        {
            if (ConnIntf == null)
            {
                throw new Exception("Please initialize the communication connection first");
            }
            ConnIntf.SendMsgEvent += SendFun;
        }

        //注册通信错误处理显示函数
        public void RegisterCommErrorEvent(LogError ErrorFun)
        {
            if (ConnIntf == null)
            {
                throw new Exception("Please initialize the communication connection first");
            }
            ConnIntf.CommunicationError += ErrorFun;
        }

        public void RegisterDisConnectedEvent(DisConnectedEvent Fun)
        {
            ConnIntf.DisConnectCallback -= Fun;
            ConnIntf.DisConnectCallback += Fun;
        }
        
        public virtual bool Execute(byte flagm, byte flagb, bool active)
        {
            return true;
        }

        //设置电流
        public virtual bool Execute(float Iset, ref DeviceI di)
        {
            return true;
        }
        
        //设置电流命令
        public virtual byte[] ExecuteCommand(float Iset)
        {
            return new byte[2];
        }

        public virtual byte[] ExecuteCommand(byte flagm, byte flagb, bool active)
        {
            return new byte[2];
        }
        protected virtual bool SetValue(byte MAddr, List<byte> values)
        {
            return false;
        }
        public virtual byte[] SetValueCommand(byte MAddr, List<byte> values)
        {
            return new byte[2];
        }

        public delegate void ActionEvent();

        //接收服务端命令后回调处理函数
        public event ActionEvent RecvActionEvent = null;
        //接收服务端反馈信息回调处理函数
        public event ActionEvent RecvBackActionEvent = null;
        //协议解析错误回调处理函数
        public event LogError ResolveProtocolError = null;
    }
}
