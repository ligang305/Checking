using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGCommunication
{
    /// <summary>
    /// 
    /// </summary>
    public class CIRProtocol : ICommonProtocol
    {

        private CommandFun mFun = CommandFun.CIR_GetTaskId;
        byte[] HeadBytes = Encoding.UTF8.GetBytes("#&&#");
        byte[] tail = Encoding.UTF8.GetBytes("&##&");
        List<byte> WaitByteArray = new List<byte>();
        List<byte> WaitServerSendByteArray = new List<byte>();
        public CIRProtocol()
        {
            ProtocolName = "Control Station -CIR BackGround Protocol";
            NeedFeedBack = false;
        }

        //发送命令前准备
        protected override void PrepareSend(byte[] SendBuf)
        {
            ConnIntf.SendedBuf = null;
            CorrectRet.Clear();
            NeedFeedBack = false;
            ExeResult = false;
            switch (CommandFunction)
            {
                case CommandFun.CIR_SendEquipment:
                    {
                        NeedFeedBack = true;
                    }
                    break;
            }
        }

        //设置命令类型
        protected CommandFun CommandFunction
        {
            get { return mFun; }
            set
            {
                mFun = value;
                switch (mFun)
                {
                    case CommandFun.CIR_GetTaskId:
                        {
                            FunctionName = "Cir GetTaskId";
                        }
                        break;
                    case CommandFun.CIR_SubmitImageInfo:
                        {
                            FunctionName = "CIr SubImageInfo";
                        }
                        break;
                }
                ConnIntf.DebugStepLog("begin" + FunctionName);
            }
        }

        //命令含义
        public override string CommandMeaning()
        {
            return FunctionName;
        }

        //解析发送命令后的回传信息
        internal override bool ResolveBackCommand(byte[] buffer, int size)
        {
            string Action = "【" + FunctionName + "】Return data";
            ConnIntf.DebugStepLog(FunctionName + "-Parsing received data failed.");
            switch (CommandFunction)
            {
                case CommandFun.CIR_SubmitImageInfo:
                    {
                        if (size != 7)
                        {
                            return false;
                        }
                        if (buffer[0] != 0x01 || buffer[2] != 0x03 || buffer[3] != 0x02)//0x0D
                        {
                            return false;
                        }
                        byte[] PreByteArray = buffer.Take(5).ToArray();
                        byte[] CrcByte = buffer.Skip(5).Take(2).ToArray();
                        ushort CrcResult = CRC16_Modbus(PreByteArray, 5);
                        ushort ValidEndByte = BitConverter.ToUInt16(CrcByte, 2);
                        if (CrcResult != ValidEndByte)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal status data check");
                        }
                        ExeResult = true;
                    }
                    break;
                case CommandFun.CIR_SendEquipment:
                    {
                        WaitByteArray.AddRange(buffer.Take(size).ToArray());
                        if (WaitByteArray.Count > HeadBytes.Length)
                        {
                            ////如果头文件
                            var reveiceHeadBytes = WaitByteArray.Take(4).ToArray();
                            for (int i = 0; i < HeadBytes.Length; i++)
                            {
                                if (reveiceHeadBytes[i] != HeadBytes[i])
                                {
                                    WaitByteArray.Clear();
                                    return false;
                                }
                            }
                        }
                        if (WaitByteArray.Count > (HeadBytes.Length + 8))
                        {
                            long DataLength = BitConverter.ToInt64(WaitByteArray.Skip(4).Take(8).Reverse().ToArray(), 0);
                            if (WaitByteArray.Count >= 12 + DataLength + 4)
                            {
                                string reve = Encoding.UTF8.GetString(WaitByteArray.Skip(12).Take(Convert.ToInt32(DataLength)).ToArray());
                                WaitByteArray.RemoveRange(0, 12 + Convert.ToInt32(DataLength) + 4);
                                WaitByteArray.TrimExcess();
                            }
                        }
                        ConnIntf.WriteLog("CIRProtocol:" + FunctionName, buffer, size);
                        ExeResult = true;
                    }
                    break;
            }
            NeedFeedBack = false;
            return true;
        }

        //解析服务端主动发送的命令
        protected override bool ResolveCommand(byte[] buffer, int size)
        {
            string Action = $"【CIR callback data return data】 size:{size} : ";

            WaitServerSendByteArray.AddRange(buffer.Take(size).ToArray());

            if (WaitServerSendByteArray.Count > HeadBytes.Length)
            {
                ////如果头文件
                var reveiceHeadBytes = WaitServerSendByteArray.Take(4).ToArray();
                for (int i = 0; i < HeadBytes.Length; i++)
                {
                    if (reveiceHeadBytes[i] != HeadBytes[i])
                    {
                        WaitServerSendByteArray.Clear();
                        return false;
                    }
                }
            }

            if(WaitServerSendByteArray.Count > (HeadBytes.Length + 8))
            {
                long DataLength = BitConverter.ToInt64(WaitServerSendByteArray.Skip(4).Take(8).Reverse().ToArray(),0);
                if(WaitServerSendByteArray.Count >= 12 + DataLength + 4)
                {
                    ReceiveServerData(WaitServerSendByteArray.Skip(12).Take(Convert.ToInt32(DataLength)).ToArray());
                    WaitServerSendByteArray.RemoveRange(0, 12 + Convert.ToInt32(DataLength) + 4);
                    WaitServerSendByteArray.TrimExcess();
                }
            }

            return true;
        }

        /// <summary>
        /// 将批量查询的系统命令转化为可以用的参数值
        /// </summary>
        private void ReceiveServerData(byte[] buffer)
        {
            ReceviceTaskEvent?.Invoke(buffer);
        }

        #region 给服务端 发送指令

        /// <summary>
        /// 发送设备信息
        /// </summary>
        /// <param name="ImageInfo"></param>
        /// <returns></returns>
        public bool SendEquipmentInfo(string EquipmentInfo)
        {
            LockSend();
            CommandFunction = CommandFun.CIR_SendEquipment;
            byte[] dataBytes = Encoding.UTF8.GetBytes(EquipmentInfo);
            List<byte> result = new List<byte>();
            byte[] length = BitConverter.GetBytes((long)dataBytes.Length).Reverse().ToArray();
            result.AddRange(HeadBytes);
            result.AddRange(length);
            result.AddRange(dataBytes);
            result.AddRange(tail);
            return SendCommand(result.ToArray());
        }

        /// <summary>
        /// 提交图片数据信息
        /// </summary>
        /// <returns></returns>
        public bool SendUploadImageInfo(string ImageInfo)
        {
            LockSend();
            CommandFunction = CommandFun.CIR_SubmitImageInfo;
            byte[] dataBytes = Encoding.UTF8.GetBytes(ImageInfo);
            List<byte> result = new List<byte>();
            byte[] length = BitConverter.GetBytes((long)dataBytes.Length).Reverse().ToArray();
            result.AddRange(HeadBytes);
            result.AddRange(length);
            result.AddRange(dataBytes);
            result.AddRange(tail);
            return SendCommand(result.ToArray());
        }
        /// <summary>
        /// 提交Rfid标签信息
        /// </summary>
        /// <returns></returns>
        public bool SendRfidInfo(string RfidInfo)
        {
            LockSend();
            CommandFunction = CommandFun.CIR_SubmitRfidInfo;
            byte[] dataBytes = Encoding.UTF8.GetBytes(RfidInfo);
            List<byte> result = new List<byte>();
            byte[] length = BitConverter.GetBytes((long)dataBytes.Length).Reverse().ToArray();
            result.AddRange(HeadBytes);
            result.AddRange(length);
            result.AddRange(dataBytes);
            result.AddRange(tail);
            return SendCommand(result.ToArray());
        }
        /// <summary>
        /// 提交报警信息
        /// </summary>
        /// <returns></returns>
        public bool SendAlarmInfo(string AlarmInfo)
        {
            LockSend();
            CommandFunction = CommandFun.CIR_UploadAlarm;
            byte[] dataBytes = Encoding.UTF8.GetBytes(AlarmInfo);
            List<byte> result = new List<byte>();
            byte[] length = BitConverter.GetBytes((long)dataBytes.Length).Reverse().ToArray();
            result.AddRange(HeadBytes);
            result.AddRange(length);
            result.AddRange(dataBytes);
            result.AddRange(tail);
            return SendCommand(result.ToArray());
        }
        /// <summary>
        /// 提交本地参数
        /// </summary>
        /// <returns></returns>
        public bool SendLocalParamater(string Paramaters)
        {
            LockSend();
            CommandFunction = CommandFun.CIR_UploadLocalParamater;
            byte[] dataBytes = Encoding.UTF8.GetBytes(Paramaters);
            List<byte> result = new List<byte>();
            byte[] length = BitConverter.GetBytes((long)dataBytes.Length).Reverse().ToArray();
            result.AddRange(HeadBytes);
            result.AddRange(length);
            result.AddRange(dataBytes);
            result.AddRange(tail);
            return SendCommand(result.ToArray());
        }
        /// <summary>
        /// 提交任务失败
        /// </summary>
        /// <returns></returns>
        public bool SendFaildScanTask(string FaildInfo)
        {
            LockSend();
            CommandFunction = CommandFun.CIR_SendFaildScanTask;
            byte[] dataBytes = Encoding.UTF8.GetBytes(FaildInfo);
            List<byte> result = new List<byte>();
            byte[] length = BitConverter.GetBytes((long)dataBytes.Length).Reverse().ToArray();
            result.AddRange(HeadBytes);
            result.AddRange(length);
            result.AddRange(dataBytes);
            result.AddRange(tail);
            return SendCommand(result.ToArray());
        }
        /// <summary>
        /// 发送背散主系统就绪过去
        /// </summary>
        /// <returns></returns>
        public bool SendTotalEquipmentSystemReady(string TotalEquipmentSystemReady)
        {
            LockSend();
            CommandFunction = CommandFun.TotalEquipmentSystemReady;
            NeedFeedBack = false;
            byte[] dataBytes = Encoding.UTF8.GetBytes(TotalEquipmentSystemReady);
            List<byte> result = new List<byte>();
            byte[] length = BitConverter.GetBytes((long)dataBytes.Length).Reverse().ToArray();
            result.AddRange(HeadBytes);
            result.AddRange(length);
            result.AddRange(dataBytes);
            result.AddRange(tail);
            return SendCommand(result.ToArray());
        }
        public bool SendCommand(byte[] Command)
        {
            bool bRet = base.SendCommand(Command);
            if (bRet)
            {
                ConnIntf.DebugLog(FunctionName, "Execution succeeded.");
            }
            else
            {
                ConnIntf.DebugLog(FunctionName, "Execution failed.");
            }
            FunctionName = "";
            UnlockSend();
            return bRet;
        }
        #endregion


        public delegate void ReceviceTaskId(byte[] TaskObject);
        public event ReceviceTaskId ReceviceTaskEvent;
    }
}
