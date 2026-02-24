using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static BGCommunication.BGSerialPortImportDll;

namespace BGCommunication
{
    public class SerialClient : ConnectInterface
    {
        public static SerialClient serialClient;
        public SerialClient(int port = 3, int boudRate = 9600,int parity = 1, int dataBit = 8, int stopBit = 1,  int timeout = 5000)
        {
            PortName = port;
            BoudRate = boudRate;
            PARITY = parity;
            DataBit = dataBit;
            StopBit = stopBit;
            Timeout = timeout;
            NotifyMsgEvent += SerialClient_NotifyMsgEvent;
        }

        private void SerialClient_NotifyMsgEvent(byte[] buf, int bufsize)
        {
            #if DEBUG
               //ConnInterLog?.Info($"接收 串口信息成功：字节长度为：{bufsize}");
            #endif
            OnRecvMsg(buf, bufsize);
        }

#region  字段/属性/委托 
        protected SerialPort sp = null;
        protected uint SerialId = 65535;
        protected bool IsConnectiont = false;
        public int PortName;
        public int BoudRate;
        public int DataBit;
        public int StopBit;
        public int PARITY;
        public int Timeout;
        internal Thread RecvThread = null;
        //先声明静态回调函数对象，否则该回调函数可能会被回收
        static SerialMsgCallback SerialMsgCallback_obj;//
#endregion

        public override bool Connect()
        {
            try
            {
                LastConnError = "";
               
                if (SerialId == 65535)
                {
                    SerialId = BGSerialPortImportDll.BGSPT_Create();
                }
                if (IsConnectiont == false)
                {
                    IsConnectiont = BGSerialPortImportDll.BGSPT_Open(SerialId, PortName, BoudRate, DataBit, PARITY, StopBit);
                    serialClient = this;
                    SerialMsgCallback_obj = new SerialMsgCallback(DataReceivedForNewDll);
                    BGSerialPortImportDll.BGSPT_SetSerialMsgEvent(SerialId, SerialMsgCallback_obj);
                    WriteLog("connect", PortName.ToString());

                    return true;
                }
            }
            catch (Exception e) 
            {
                LastConnError = e.Message;
                Console.Write(e.Message);
                WriteError("Abnormal connection: ", PortName + "->" + e.Message);
            }
            return IsConnectiont;
        }


        public override bool IsConnect()
        {
            if (SerialId != 65535 && IsConnectiont)
            {
                return true;
            }
            return false;
        }

        public override void DisConnect()
        {
            try
            {
                LastConnError = "Client actively disconnects.";
                IsConnectiont = false;
                if (IsConnect())
                {
                    BGSerialPortImportDll.BGSPT_Close(SerialId);
                }
                SerialId = 65535;
                WriteLog("disconnect", PortName.ToString());
            }
            catch (Exception e)
            {
                LastConnError = e.Message;
                Console.Write(e.Message);
                WriteError("Disconnect exception", PortName + "->" + e.Message);
            }
            OnDisConnected();
        }

        public override int SendMsg(byte[] arrMsg)
        {
            try
            {
                //sp.Encoding = Encoding.ASCII;
                string str = Encoding.ASCII.GetString(arrMsg);
                BGSerialPortImportDll.BGSPT_Write(SerialId, arrMsg, arrMsg.Length);

                SendedBuf = arrMsg.ToList();
                SendedBuf.TrimExcess();
                //OnSendMsg(arrMsg, str.Length);
                return str.Length;
            }
            catch (Exception e)
            {
                LastConnError = e.Message;
                Console.Write(e.Message);
                string msg = ICommonProtocol.ToHexString(arrMsg);
                string Info = string.Format("{0}[{1}]={2}", cp.FunctionName, arrMsg.Length, msg);
                WriteError("Send failed.", Info + "=>" + e.Message);
                OutputError(e.Message);
                DisConnect();
                return 0;
            }
        }


        public static void DataReceivedForNewDll(uint serial_id, IntPtr serial_info, uint length)
        {
            byte[] buff = new byte[length];
            Marshal.Copy(serial_info, buff, 0, (int)length);
            //OnRecvMsg(buff, buff.Length); 
            OntNotifyMsg(buff, buff.Length);
            //serialClient?.OnRecvMsg(buff, buff.Length); 
        }
        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] buffer = new byte[sp.BytesToRead]; 
            sp.Read(buffer, 0, buffer.Length);
            OnRecvMsg(buffer, buffer.Length); 
        }
    }
}
