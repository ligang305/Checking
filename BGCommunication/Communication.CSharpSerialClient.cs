using BGLogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class CSharpSerialClient : ConnectInterface
    {
        public static CSharpSerialClient serialClient;
        SerialPort _serialPort = null;
        public CSharpSerialClient(int port = 3, int boudRate = 9600, int parity = 1, int dataBit = 8, int stopBit = 1, int timeout = 5000)
        {
            PortName = port;
            BoudRate = boudRate;
            PARITY = parity;
            DataBit = dataBit;
            StopBit = stopBit;
            Timeout = timeout;
            _serialPort = new SerialPort();
            NotifyMsgEvent += SerialClient_NotifyMsgEvent;
            //ConnInterLog?.Info($"CSharpSerialClient");
        }

        private void SerialClient_NotifyMsgEvent(byte[] buf, int bufsize)   
        {
#if DEBUG
           
            //ConnInterLog?.Info($"Receiving serial port information successfully: the byte length is：{bufsize}");
#endif
            //ConnInterLog?.Info($"Receiving serial port information successfully: the byte length is：{bufsize}");
            //this.WriteDebug("",$"Receiving serial port information successfully: the byte length is：{bufsize}");
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


        #region 打开串口资源

        /// <summary>

        /// 打开串口资源

        /// <returns>返回bool类型</returns>

        /// </summary>

        public bool openPort()
        {
            bool ok = false;
            //如果串口是打开的，先关闭
            if (_serialPort.IsOpen)
                _serialPort.Close();
            try
            {
                //打开串口
                _serialPort.Open();
                ok = true;
            }
            catch (Exception Ex)
            {
                Debug.WriteLine("------------------------------------------------------------------gang串口打开失败--------------------波特率(" + _serialPort.BaudRate + ")----串口号->" + _serialPort.PortName);
                throw Ex;
            }
            Debug.WriteLine("------------------------------------------------------------------gang串口打开成功--------------------波特率(" + _serialPort.BaudRate + ")----串口号->" + _serialPort.PortName);
            return ok;
        }
        /// <summary>

        /// 设置串口资源,还需重载多个设置串口的函数

        /// </summary>

        void setSerialPort()
        {
            if (_serialPort != null)
            {
                //设置触发DataReceived事件的字节数为1
                _serialPort.ReceivedBytesThreshold = 1;
                //接收到一个字节时，也会触发DataReceived事件
                _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
                //接收数据出错,触发事件
                _serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(_serialPort_ErrorReceived);
                //打开串口
                openPort();
            }
        }
        #endregion
        public override bool Connect()
        {
            try
            {
                LastConnError = "";
                WriteDebug("Connect",$"PortName:{PortName},BoudRate:{BoudRate},PARITY:{PARITY},StopBit:{StopBit}");
                _serialPort = new SerialPort($@"COM{PortName}", BoudRate, (Parity)Enum.ToObject(typeof(Parity), PARITY), 8, (StopBits)Enum.ToObject(typeof(StopBits), StopBit));
                _serialPort.RtsEnable = true;
                _serialPort.ReadTimeout = 2000;
                setSerialPort();
                return true;
            }
            catch (Exception e)
            {
                LastConnError = e.Message;
                Console.Write(e.Message);
                WriteError("Abnormal connection", PortName + "->" + e.Message);
            }
            return IsConnectiont;
        }


        public override bool IsConnect()
        {
            //if (SerialId != 65535 && IsConnectiont)
            //{
            //    return true;
            //}
            return _serialPort.IsOpen;
        }

        public override void DisConnect()
        {
            try
            {
                LastConnError = "Client actively disconnects.";
                IsConnectiont = false;
                if (_serialPort.IsOpen)
                    _serialPort.Close();
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
                if (_serialPort.IsOpen)
                {
                    _serialPort.Write(arrMsg, 0, arrMsg.Length);
                }
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
        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] buffer = new byte[_serialPort.BytesToRead];
            _serialPort.Read(buffer, 0, buffer.Length);
            //OnRecvMsg(buffer, buffer.Length);
            OntNotifyMsg(buffer, buffer.Length);
        }
        /// <summary>

        /// 接收数据出错事件

        /// </summary>

        /// <param name="sender"></param>

        /// <param name="e"></param>

        void _serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)

        {

        }
    }
}
