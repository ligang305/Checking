using BGLogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BGCommunication
{
    public abstract class ConnectInterface
    {
        protected ICommonProtocol cp = null;
        internal ICommonProtocol CommonProtocol
        {
            set { cp = value; }
            get { return cp; }
        }

        //记录最后的连接异常信息
        protected string LastConnError = "";

        public void WriteError(string FunName, string Message,bool IsWriteLog = true)
        {
            if(IsWriteLog)
            {
                string Info = FunName + "-->" + Message;
                //Log.GetDistance().WriteErrorLogs(Info);
            }
        }

        public void WriteDebug(string FunName, string Message, bool IsWriteLog = true)
        {
            if (IsWriteLog)
            {
                string Info = FunName + "-->" + Message;
                //Log.GetDistance().WriteErrorLogs(Info);
            }
        }
        public void DebugStepLog(string Message)
        {
            Log.GetDistance().WriteErrorLogs(Message);
        }

        public void DebugLog(string FunName, string Message)
        {
            if (FunName == "控制站-Dose协议")
            {
                string Info = FunName + "-->" + Message;
                //Log.GetDistance().WriteInfoLogs(Info);
            }
        }

        public void WriteLog(string FunName, string Message)
        {
            string Info = FunName + "-->" + Message;
            //Log.GetDistance().WriteErrorLogs(Info);
            //ConnInterLog?.Info(Info);
        }

        public void WriteLog(string FunName, byte[] data, int length)
        {
            byte[] message = new byte[length];
            Array.Copy(data, message, length);
            string Message = ICommonProtocol.ToHexString(message);
            string Info = string.Format("{0}[{1}]=[{2}]", FunName, length, Message);
            Log.GetDistance().WriteInfoLogs(Info);
        }

        public void WriteLog(string FunName, string CommandName, byte[] data, int size)
        {
            byte[] message = new byte[size];
            Array.Copy(data, message, size);
            string Message = ICommonProtocol.ToHexString(message);
            string Info = string.Format("{0}-{1}[{2}]=[{3}]", FunName, CommandName, size, Message);
            //ConnInterLog?.Info(Info);
        }

        internal List<byte> SendedBuf = new List<byte>();
        public abstract bool Connect();
        public abstract bool IsConnect();
        public abstract void DisConnect();
        public virtual int SendMsgAsyc(byte[] arrMsg)
        {
            int nRet = 0;
            Thread thread = new Thread(() =>
            {
                nRet = SendMsg(arrMsg);
            });
            thread.IsBackground = true;
            thread.Start();
            thread.Join();
            return nRet;
        }
        public abstract int SendMsg(byte[] arrMsg);
        public virtual void OnSendMsg(byte[] buf, int bufsize)
        {
            WriteLog("send", cp.FunctionName, buf, bufsize);
            SendMsgEvent?.Invoke(buf, bufsize);
        }
        public virtual void OnRecvMsg(byte[] buf, int bufsize)
        {
            //string str = System.Text.Encoding.ASCII.GetString(buffer);
            //WriteLog("接收", str);
            //WriteLog("receive", buf, bufsize);
            RecvMsgEvent?.Invoke(buf, bufsize);
            cp?.OnRecv(buf, bufsize);
        }

        public virtual void OnDisConnected()
        {
            DebugStepLog("OnDisConnected");
            while (cp?.IsSending > 1)
            {
                string str = string.Format($"OnDisConnected Waiting IsSending={cp.IsSending}");
                DebugStepLog(str);
                Thread.Sleep(3);
            }
            if (LastConnError != "")
            {
                DebugStepLog("DisConnectCallback");
                DisConnectCallback?.Invoke(LastConnError);
            }
        }

        protected void OutputError(string error)
        {
            CommunicationError?.Invoke(error);
        }

        protected static void OntNotifyMsg(byte[] buf, int bufsize)
        {
            NotifyMsgEvent?.Invoke(buf, bufsize);
        }

        public delegate void MessageEvent(byte[] buf, int bufsize);
        public delegate void DisConnectedEvent(string ExMsg);

        public static event MessageEvent NotifyMsgEvent = null;
        public event MessageEvent RecvMsgEvent = null;
        public event MessageEvent SendMsgEvent = null;
        public delegate void LogError(string error);
        public event LogError CommunicationError = null;
        public event DisConnectedEvent DisConnectCallback = null;
    }
}
