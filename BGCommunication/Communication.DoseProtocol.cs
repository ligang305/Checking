using System;
using System.Linq;
using System.Text;

namespace BGCommunication
{
    /// <summary>
    /// 
    /// </summary>
    public class DoseProtocol : ICommonProtocol
    {
        public DoseProtocol()
        {
            ProtocolName = "Control station -Dose protocol";
        }
        ~DoseProtocol()
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
                case CommandDoseFun.DOSE_GET:
                    {
                        NeedFeedBack = true;
                    }
                    break;
            }
        }

        private CommandDoseFun mFun = CommandDoseFun.DOSE_GET;
        /// <summary>
        /// 识别字符串
        /// </summary>
        public UInt32 ReadStr=0;
        public int CurrentIndex;
        //设置命令类型
        protected CommandDoseFun CommandFunction
        {
            get { return mFun; }
            set
            {
                mFun = value;
                switch (mFun)
                {
                    case CommandDoseFun.DOSE_GET:
                        {
                            FunctionName = "Control station -Dose protocol";
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
                case CommandDoseFun.DOSE_GET:
                    {
                        Command = InqurGetCommand();
                    }
                    break;
            }
            return Command;
        }
        
        internal override void OnRecv(byte[] buffer, int size)
        {
            if (NeedFeedBack && size==11)
            {
                NeedFeedBack_ResolveBackCommand(buffer, size);
                
            }
            else
            {
                UnNeedFeedBack_ResolveCommand(buffer, size);
            }
        }

        //解析服务端主动发送的命令
        internal  override bool ResolveBackCommand(byte[] buffer, int size)
        {
            string Action = $"【Dose callback data returns data】 size:{size} : ";

            // 校验 
            //取前9个字节进行CRC校验
            byte[] ValidByte = buffer.Skip(0).Take(9).ToArray();
            ushort CrcResult = CRC16_Modbus(ValidByte,9);
            ushort ValidEndByte = BitConverter.ToUInt16(buffer,9);
            if (CrcResult != ValidEndByte)
            {
                throw new Exception(ProtocolName + "-" + Action + "-Illegal status data check");
            }
            byte[] tempStr = buffer.Skip(5).Take(4).ToArray();
            ReadStr = (UInt32)((tempStr[0] << 24) + (tempStr[1] << 16) + (tempStr[2] << 8) + tempStr[3]) * 10;
            ConnIntf.DebugLog(FunctionName, $"{Action}" +  ReadStr);
            return true;
        }
        
        /// <summary>
        /// 握手报文
        /// </summary>
        /// <returns></returns>
        public byte[] InqurGetCommand()
        {
            byte[] command = null;
            command = new byte[7] { 0xfe, 0x31, 0x00, 0x38, 0x04, 0x11, 0x2b};
            return command;
        }
        /// <summary>
        /// 握手通讯
        /// </summary>
        /// <param name="wj"></param>
        /// <returns></returns>
        public bool InqurDoseGetStatus(int Index)
        {
            LockSend();
            CurrentIndex = Index;
            CommandFunction = CommandDoseFun.DOSE_GET;
            byte[] command = BuildCommand();
            bool bRet = SendCommand(command);
            if (bRet)
            {
                //ConnIntf.DebugLog(a, "执行成功");
            }
            else
            {
                //ConnIntf.DebugLog(a, "执行失败");
            }
            FunctionName = "";
            UnlockSend();
            return bRet;
        }

    }
}
