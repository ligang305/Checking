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
    public class TCPClient : ConnectInterface
    {
        public TCPClient(string IP, int port, bool keepconnect, bool heartbeat, ProtocolType _pt = ProtocolType.IP)
        {
            pt = _pt;
            ServerIP = IP;
            Port = port;
            KeepConnect = keepconnect;
            HeartCheck = heartbeat;
            socket = null;
            IsSocketValid = false;
        }
        internal Thread RecvThread = null;
        private string IP;
        private int PORT;
        //保持一直连接服务器，通信断开后主动重连
        private bool KEEP;
        private bool Heartbeat;
        private ProtocolType pt;
        public string ServerIP
        {
            get { return this.IP; }
            set
            {
                if (socket == null)
                {
                    this.IP = value;
                }
            }
        }
        public int Port { get { return PORT; } set { if (socket == null) { PORT = value; } } }
        public bool KeepConnect { get { return KEEP; } set { if (socket == null) { KEEP = value; } } }
        public bool HeartCheck { get { return Heartbeat; } set { if (socket == null) { Heartbeat = value; } } }



        protected Socket socket { get; set; } = null;

        private bool IsSocketValid { get; set; } = false;
        public override bool Connect()
        {
            DebugStepLog("Connect server");   
            try
            {
                LastConnError = "";
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, pt);
                IPAddress ip = IPAddress.Parse(ServerIP);
                IPEndPoint endPoint = new IPEndPoint(ip, Port);
                socket.Connect(endPoint);

                RecvThread = new Thread(ReceiveMsg);
                RecvThread.IsBackground = true;
                RecvThread.Start(this);

                WriteLog("connect", ServerIP + ":" + Port + "success");

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
                WriteError("Abnormal connection", ServerIP + ":" + Port + "->" + ex.Message);
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
            try
            {
                if (IsSocketValid && socket != null)
                {
                    return socket.Connected;
                }
            }
            catch (Exception ex)
            {
                WriteError("Abnormal connection", ServerIP + ":" + Port + "->" + ex.Message);
            }
            return false;
        }

        public override void DisConnect()
        {
            if (IsSocketValid == true)
            {
                KEEP = false;
                LastConnError = "Client actively disconnects.";
                CloseSocket();
            }
        }

        public override int SendMsg(byte[] arrMsg)
        {
            try
            {
                if (socket == null) return 0;
                int nRet = socket.Send(arrMsg);
                SendedBuf = arrMsg.ToList();
                SendedBuf.TrimExcess();
                OnSendMsg(arrMsg, nRet);
                return nRet;
            }
            catch (System.Exception ex)
            {
                LastConnError = ex.StackTrace;
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
            TCPClient sc = obj as TCPClient;

            while (KEEP)
            {
                Thread.Sleep(100);
                if (sc.socket == null)
                {
                    try
                    {
                        sc.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                        IPAddress ip = IPAddress.Parse(sc.ServerIP);
                        IPEndPoint endPoint = new IPEndPoint(ip, sc.Port);
                        sc.socket.Connect(endPoint);

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
            //ParameterizedThreadStart pts = new ParameterizedThreadStart(TCPClient.ClearSocketFun);
            //Thread thread = new Thread(pts);
            //thread.Start(this);
            try
            {
                ClearSocketFun(this);
            }
            catch (Exception ex)
            {
                WriteLog("CloseSocket Error", ex.Message);
            }
        }
        protected static void ClearSocketFun(Object obj)
        {
            TCPClient client = obj as TCPClient;
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
                try
                {
                    socket?.Close();
                    socket?.Dispose();
                    socket = null;
                    IsSocketValid = false;

                    WriteLog("disconnect", ServerIP + ":" + Port);
                    OnDisConnected();
                }
                catch (Exception ex)
                {
                    WriteLog("abnormal", ex.Message);
                }
          
            }
        }

        protected void ReceiveMsg(Object obj)
        {
            TCPClient sc = obj as TCPClient;
            while (sc.socket != null)
            {
                try
                {
                    if (sc.socket == null) return;
                    byte[] rcvBuf = new byte[1024];
                    int length = sc.socket != null ? sc.socket.Receive(rcvBuf):0;
                    sc.OnRecvMsg(rcvBuf, length);
                    //if (sc.socket.Poll(100,SelectMode.SelectRead))
                    //{
                    //    if (sc.IsSocketValid)
                    //    {
                    //        byte[] rcvBuf = new byte[1024];
                    //        int length = sc.socket.Receive(rcvBuf);
                    //        sc.OnRecvMsg(rcvBuf, length);
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    LastConnError = ex.StackTrace;
                    Console.Write(ex.Message);
                    WriteError("Abnormal reception", ex.Message); 
                    OutputError(ex.Message);
                    sc.CloseSocket();
                    break;
                    //Thread.CurrentThread.Abort();
                }
            }
            Console.Write("Abnormal reception\n");
        }

        protected void HeartBeat(Object obj)
        {
            TCPClient sc = obj as TCPClient;
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
                    LastConnError = ex.StackTrace;
                    Console.Write(ex.Message);
                    WriteError("Abnormal heartbeat", ex.Message);
                    sc.CloseSocket();
                }
            }
            Console.Write("Client heartbeat packet thread exited\n");
        }
    }
}
