using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BGCommunication
{
    public class UDPClient : ConnectInterface
    {
        public UDPClient(string LocalIp,string _ServerIP, int ServerPort,int LocalPort, bool keepconnect, bool heartbeat)
        {
            ServerIP = _ServerIP;
            localIP = LocalIp;
            LocalPORT = LocalPort;
            serverPORT = ServerPort;
            KeepConnect = keepconnect;
            HeartCheck = heartbeat;
            socket = null;
            IsSocketValid = false;
        }
        internal Thread RecvThread = null;
        private string serverIP;
        private string localIP;
        private int serverPORT;
        private int LocalPORT;
        //保持一直连接服务器，通信断开后主动重连
        private bool KEEP;
        //握手协议是否连接
        public bool isConnection = true;
        private bool Heartbeat;
        public string ServerIP
        {
            get { return this.serverIP; }
            set
            {
                if (socket == null)
                {
                    this.serverIP = value;
                }
            }
        }
        public string LocalIP
        {
            get { return this.localIP; }
            set
            {
                if (socket == null)
                {
                    this.localIP = value;
                }
            }
        }
        public int ServerPort { get { return serverPORT; } set { if (socket == null) { serverPORT = value; } } }
        public int LocalPort { get { return LocalPORT; } set { if (socket == null) { LocalPORT = value; } } }
        public bool KeepConnect { get { return KEEP; } set { if (socket == null) { KEEP = value; } } }
        public bool HeartCheck { get { return Heartbeat; } set { if (socket == null) { Heartbeat = value; } } }
        protected Socket socket = null;
        private bool IsSocketValid = false;
        public override bool Connect()
        {
            DebugStepLog("UDP Connect server");
            try
            {
                LastConnError = "";
                socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
                //.Parse(LocalIP)
                socket.Bind(new IPEndPoint(IPAddress.Any, LocalPort));

                RecvThread = new Thread(ReceiveMsg);
                RecvThread.IsBackground = true;
                RecvThread.Start(this);

                WriteLog("connect", ServerIP + ":" + LocalPort + "success");

                if (Heartbeat)
                {
                    Thread Heartthread = new Thread(HeartBeat);
                    Heartthread.IsBackground = true;
                    Heartthread.Start(this);
                }
                IsSocketValid = true;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                WriteError("Abnormal connection", ServerIP + ":" + LocalPort + "->" + ex.Message);
                CloseSocket();
                return false;
            }

            Thread ConnThread = new Thread(KeepConnectServer);
            ConnThread.IsBackground = true;
            ConnThread.Start(this);

            return true;
        }

        public override bool IsConnect()
        {
            if (IsSocketValid)
            {
                return true;
                //return isConnection;
            }
            return false;
        }

        public override void DisConnect()
        {
            if (IsSocketValid == true)
            {
                KEEP = false;
                LastConnError = "Abnormal connection";
                CloseSocket();
            }
        }

        public override int SendMsg(byte[] arrMsg)
        {
            try
            {
                EndPoint point = new IPEndPoint(IPAddress.Parse(ServerIP), serverPORT);
                int nRet = socket.SendTo(arrMsg, point);
                SendedBuf = arrMsg.ToList();
                SendedBuf.TrimExcess();
                OnSendMsg(arrMsg, nRet);
                return nRet;
            }
            catch (System.Exception ex)
            {
                LastConnError = ex.Message;
                Console.Write(ex.Message);
                string msg = ICommonProtocol.ToHexString(arrMsg);
                string Info = string.Format("{0}[{1}]={2}", cp.FunctionName, arrMsg.Length, msg);
                WriteError("Send failed.", Info + "=>" + ex.Message);
                OutputError(ex.Message);
                CloseSocket();
                return 0;
            }
        }

        protected void KeepConnectServer(Object obj)
        {
            UDPClient sc = obj as UDPClient;

            while (KEEP)
            {
                if (sc.socket == null)
                {
                    try
                    {
                        socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
                        socket.Bind(new IPEndPoint(IPAddress.Parse(LocalIP), LocalPort));

                        RecvThread = new Thread(ReceiveMsg);
                        RecvThread.IsBackground = true;
                        RecvThread.Start(sc);

                        if (Heartbeat)
                        {
                            Thread Heartthread = new Thread(HeartBeat);
                            Heartthread.IsBackground = true;
                            Heartthread.Start(sc);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.Message);
                        sc.CloseSocket();
                        break;
                    }
                }
            }

            Console.Write("TCP client connection service thread exited.\n");
        }

        protected void CloseSocket()
        {
            ParameterizedThreadStart pts = new ParameterizedThreadStart(UDPClient.ClearSocketFun);
            Thread thread = new Thread(pts);
            thread.Start(this);
        }
        protected static void ClearSocketFun(Object obj)
        {
            UDPClient client = obj as UDPClient;
            lock (client)
            {
                client.ClearSocket();
            }
        }
        protected void ClearSocket()
        {
            DebugLog("ClearSocket", "IsSocketValid=" + IsSocketValid);
            if (IsSocketValid == true)
            {
                socket.Close();
                socket.Dispose();
                socket = null;
                IsSocketValid = false;
                isConnection = false;
                WriteLog("断开", ServerIP + ":" + serverPORT);
                OnDisConnected();
            }
        }

        protected void ReceiveMsg(Object obj)
        {
            UDPClient sc = obj as UDPClient;
            while (sc.socket != null)
            {
                try
                {
                    EndPoint point = new IPEndPoint(IPAddress.Any, 0);//用来保存发送方的ip和端口号
                    byte[] rcvBuf = new byte[1024];
                    int length = sc.socket.ReceiveFrom(rcvBuf,ref point);
                    sc.OnRecvMsg(rcvBuf, length);
                }
                catch (Exception ex)
                {
                    LastConnError = ex.Message;
                    Console.Write(ex.Message);
                    WriteError("Abnormal reception", ex.Message);
                    OutputError(ex.Message);
                    sc.CloseSocket();
                    break;
                    //Thread.CurrentThread.Abort();
                }
            }
            Console.Write("TCP client thread receiving service information exits.\n");
        }

        protected void HeartBeat(Object obj)
        {
            UDPClient sc = obj as UDPClient;
            DateTime tick1 = DateTime.Now;
            double Waittick = 0;
            while (sc.IsSocketValid)
            {
                try
                {
                    if (sc.IsConnect())
                    {
                        if (Waittick > 10.6)
                        {
                            byte[] ch = new byte[1];
                            ch[0] = 0xFF;
                            sc.SendMsg(ch);
                            Waittick = 0;
                            tick1 = DateTime.Now;
                        }
                        else
                        {
                            DateTime tick2 = DateTime.Now;
                            Waittick = (tick2 - tick1).TotalSeconds;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    LastConnError = ex.Message;
                    Console.Write(ex.Message);
                    WriteError("Abnormal heartbeat", ex.Message);
                    sc.CloseSocket();
                }
            }
            Console.Write("Client heartbeat packet thread exited\n");
        }
    }
}
