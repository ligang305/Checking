using System;
using System.Linq;
using System.Text;

namespace BGCommunication
{
    /// <summary>
    /// 
    /// </summary>
    public class ARMProtocol : ICommonProtocol
    {
        public ARMProtocol()
        {
            ProtocolName = "Control station -ARM protocol";
        }
        ~ARMProtocol()
        {
            Console.WriteLine("Structure--" + ProtocolName);
        }

        //发送命令前准备
        protected override void PrepareSend(byte[] SendBuf)
        {
            ConnIntf.SendedBuf = null;
            CorrectRet.Clear();
            NeedFeedBack = true;
            ExeResult = false;
            switch (CommandFunction)
            {
                case CommandARMFun.ARM_ENDSCAN:
                    {
                        NeedFeedBack = true;
                    }
                    break;
                case CommandARMFun.ARM_HAND:
                    {
                        NeedFeedBack = true;
                    }
                    break;
                case CommandARMFun.ARM_STARTSCAN:
                    {
                        NeedFeedBack = true;
                    }
                    break;

            }
        }

        private CommandARMFun mFun = CommandARMFun.ARM_HAND;
        /// <summary>
        /// 报警标志
        /// </summary>
        public PoliceFlag pf = PoliceFlag.NormalARM;
        /// <summary>
        /// 识别字符串
        /// </summary>
        public string ReadStr=string.Empty;

        //设置命令类型
        protected CommandARMFun CommandFunction
        {
            get { return mFun; }
            set
            {
                mFun = value;
                switch (mFun)
                {
                    case CommandARMFun.ARM_HAND:
                        {
                            FunctionName = "Handshake query";
                        }
                        break;
                    case CommandARMFun.ARM_ENDSCAN:
                        {
                            FunctionName = "End scan";
                        }
                        break;
                    case CommandARMFun.ARM_STARTSCAN:
                        {
                            FunctionName = "Start scanning";
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

        //创建发送命令
        public override byte[] BuildCommand()
        {
            byte[] Command = null;
            switch (CommandFunction)
            {
                case CommandARMFun.ARM_HAND:
                    {
                        Command = InqurHandCommand();
                    }
                    break;
                case CommandARMFun.ARM_ENDSCAN:
                    {
                        Command = InqurEndScanCommand();
                    }
                    break;
                case CommandARMFun.ARM_STARTSCAN:
                    {
                        Command = InqurStartScanCommand();
                    }
                    break;
            }
            return Command;
        }
        
        internal override void OnRecv(byte[] buffer, int size)
        {
            if (NeedFeedBack && size==15)
            {
                NeedFeedBack_ResolveBackCommand(buffer, size);
                
            }
            else
            {
                UnNeedFeedBack_ResolveCommand(buffer, size);
            }
        }

        //解析服务端主动发送的命令
        protected override bool ResolveCommand(byte[] buffer, int size)
        {
            string Action = $"【ARM callback data return data】 size:{size} : ";
            var Count = 0x00;
            if (size != 271)
            {
                throw new Exception(ProtocolName + "-" + Action + "-Illegal length of status data");
            }
            for (int i = 3; i <= 268; i++)
            {
                Count += buffer[i];
            }
            // 校验 1、第一个字节是不是0x56
            //2、第二个字节是不是0x57
            //3、第三个字节是不是0x41
            //4、第四个字节是不是0xD2
            //5、第五第六个字节由于长度已定，所以直接校验是不是0x01 和 0x07
            //6、第七个字节校验数据类型是不是0x01
            //由于基本上已经定了长度 所以校验倒数第二位的校验数据位长度是不是前面的一些之和
            if (buffer[0] != 0x56 && buffer[1] != 0x57 && buffer[2] != 0x41
                && buffer[3] != 0xD2 && buffer[4] != 0x01 && buffer[5] != 0x07 && buffer[6] != 0x01 
                && buffer[269] != Count)
            {
                throw new Exception(ProtocolName + "-" + Action + "-Illegal length of status data");
            }
            pf = (PoliceFlag)buffer[12];
            byte[] tempStr = buffer.Skip(13).Take(254).ToArray();
            ReadStr = Encoding.ASCII.GetString(tempStr).Trim('\0',new char()).Trim();// Trim();
            if (pf == PoliceFlag.None || pf == PoliceFlag.NormalARM)
            {
                if (!string.IsNullOrEmpty(ReadStr))
                {
                    pf = PoliceFlag.Gamma;
                }
            }

            ConnIntf.DebugLog(FunctionName, "Execution succeeded, ARM callback data：" + pf + " " +  ReadStr);
            return true;
        }
        //解析发送命令后的回传信息
        internal override bool ResolveBackCommand(byte[] buffer, int size)
        {
            string Action = "【" + FunctionName + "】Return data";

            if (ConnIntf.SendedBuf == null)
            {
                throw new Exception(ProtocolName + "-" + Action + "-Unknown error");
            }

            switch (CommandFunction)
            {
                case CommandARMFun.ARM_HAND:
                    {
                        var Count = 0x00;
                        if (size != 15)
                        {
                            //(ConnIntf as UDPClient).isConnection = false;
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal length of status data");
                        }
                        for (int i = 3; i <= 12; i++)
                        {
                            Count += buffer[i];
                        }

                        if (buffer[0] != 0x56 && buffer[1] != 0x57 && buffer[2] != 0x41
                            && buffer[3] != 0x00 && buffer[4] != 0x00 && buffer[5] != 0x07 && buffer[6] != 0x01 &&
                            buffer[11] != 0x00 && buffer[12] != 0x01 && buffer[13] != Count)
                        {
                            //(ConnIntf as UDPClient).isConnection = false;
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal length of status data");
                        }

                        string Recv = string.Empty;
                        for (int i = 0; i < size; i++)
                        {
                            Recv += " " + buffer[i];
                        }
                        ConnIntf.DebugLog(FunctionName, "Successful execution, handshake agreement：" + Recv);
                        //(ConnIntf as UDPClient).isConnection = true;
                        ExeResult = true;
                    }
                    break;
                case CommandARMFun.ARM_ENDSCAN:
                    {

                        var Count = 0x00;
                        if (size != 15)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal length of status data");
                        }
                        for (int i = 3; i <= 12; i++)
                        {
                            Count += buffer[i];
                        }

                        if (buffer[0] != 0x56 && buffer[1] != 0x57 && buffer[2] != 0x41
                            && buffer[3] != 0xD1&& buffer[4] != 0x00 && buffer[5] != 0x07 && buffer[6] != 0x01 &&
                            buffer[11] != 0x01 && buffer[12] != 0x00 && buffer[13] != Count)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal status data check");
                        }
                        string Recv = string.Empty;
                        for (int i = 0; i < buffer.Length; i++)
                        {
                            Recv += " " + buffer[i];
                        }
                        ConnIntf.DebugLog(FunctionName, "Successfully executed. Copy that. End of scan：" + Recv);
                        ExeResult = true;
                    }
                    break;
                case CommandARMFun.ARM_STARTSCAN:
                    {
                        var Count = 0x00;
                        if (size != 15)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal length of status data");
                        }
                        for (int i = 3; i <= 12; i++)
                        {
                            Count += buffer[i];
                        }

                        if (buffer[0] != 0x56 && buffer[1] != 0x57 && buffer[2] != 0x41
                            && buffer[3] != 0xD1 && buffer[4] != 0x00 && buffer[5] != 0x07 && buffer[6] != 0x01 &&
                            buffer[11] != 0x01 && buffer[12] != 0x01 && buffer[13] != Count)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal length of status data");
                        }
                        string Recv = string.Empty;
                        for (int i = 0; i < buffer.Length; i++)
                        {
                            Recv += " " + buffer[i];
                        }
                        ConnIntf.DebugLog(FunctionName, "Successfully executed. Copy that. Scan started.：" + Recv);
                        ExeResult = true;
                    }
                    break;
            }

            NeedFeedBack = false;
            return true;
        }
        /// <summary>
        /// 握手报文
        /// </summary>
        /// <returns></returns>
        public byte[] InqurHandCommand()
        {
            byte[] command = null;
            command = new byte[15] { 0x56, 0x57, 0x43, 0x00, 0x00, 0x07, 0x01, 0x31, 0x32, 0x33, 0x34, 0x00, 0x01, 0xD3, 0x5A };
            return command;
        }
        /// <summary>
        /// 发送开始扫描
        /// </summary>
        /// <returns></returns>
        public byte[] InqurStartScanCommand()
        {
            byte[] command = null;
            command = new byte[15] { 0x56, 0x57, 0x43, 0xD1, 0x00, 0x07, 0x01, 0x31, 0x32, 0x33, 0x34, 0x01, 0x01, 0xA5, 0x5A };
            return command;
        }
        /// <summary>
        /// 发送结束扫描
        /// </summary>
        /// <returns></returns>
        public byte[] InqurEndScanCommand()
        {
            byte[] command = null;
            command = new byte[15] { 0x56, 0x57, 0x43, 0xD1, 0x00, 0x07, 0x01, 0x31, 0x32, 0x33, 0x34, 0x01, 0x00, 0xA4, 0x5A };
            return command;
        }


        /// <summary>
        /// 握手通讯
        /// </summary>
        /// <param name="wj"></param>
        /// <returns></returns>
        public bool InqurHandStatus()
        {
            LockSend();
            CommandFunction = CommandARMFun.ARM_HAND;
            byte[] command = BuildCommand();
            bool bRet = SendCommand(command);
            if (bRet)
            {
                (ConnIntf as UDPClient).isConnection = true;
                ConnIntf.DebugLog(FunctionName, "Execution succeeded.");
            }
            else
            {
                (ConnIntf as UDPClient).isConnection = false;
                ConnIntf.DebugLog(FunctionName, "Execution failed.");
            }
            FunctionName = "";
            UnlockSend();
            return bRet;
        }

        /// <summary>
        /// 发送开始扫描指令
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public bool InqurStartScan()
        {
            LockSend();
            CommandFunction = CommandARMFun.ARM_STARTSCAN;
            byte[] command = BuildCommand();
            bool bRet = SendCommand(command);
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
        /// <summary>
        /// 发送结束扫描指令
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public bool InqurEndScan()
        {
            LockSend();
            CommandFunction = CommandARMFun.ARM_ENDSCAN;
            byte[] command = BuildCommand();
            bool bRet = SendCommand(command);
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

    }
}
