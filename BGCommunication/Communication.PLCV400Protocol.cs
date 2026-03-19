using BGLogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BGCommunication
{
   
    public abstract class PLCV400Protocol : PLCProtocol
    {
        public PLCV400Protocol()
        {
            ProtocolName = "Control station -PLCV400 protocol";
            PLCConnectType = ConnectType.CT_TouchScreen;
            InitSystemTypeParas();
        }
        ~PLCV400Protocol()
        {
            Console.WriteLine("Structure--" + ProtocolName);
        }
        //生成发送命令
        public override byte[] BuildCommand()
        {
            byte[] Command = null;
            switch (CommandType)
            {
                case SendInfoType.SIT_InqurMByte1:
                    {
                        Command = InqurStatusCommand();
                        //ConnIntf.WriteError("SIT_InqurMByte1 send :", ICommonProtocol.ToHexString(Command), true);
                    }
                    break;
                case SendInfoType.SIT_InqureStr:
                case SendInfoType.SIT_InqureUInt32:
                case SendInfoType.SIT_InqureParamater:
                    {
                        Command = BuildReadCommand(this.blockType,this.blockoffset,this.blockNum,this.length);
                        //BGLogs.Log.GetDistance().WriteInfoLogs(CommandType + ToHexString(Command));
                    }
                    break;
                case SendInfoType.SIT_Command:
                    {
                        Command = ExecuteCommand(FlagM, FlagB, FlagActive);
                    }
                    break;
                case SendInfoType.SIT_SetValue:
                    {
                        Command = SetValueCommand(MStartPos, DataValues);
                    }
                    break;
                case SendInfoType.SIT_Init:
                    {
                        Command = InitPLCCommand(InitIndex);
                    }
                    break;
            }

            return Command;
        }
        //解析回传命令
        internal override bool ResolveBackCommand(byte[] buffer, int size)
        {
            string Action = "【" + FunctionName + "】Return data";
            ConnIntf.DebugStepLog(FunctionName + "-Parsing return data");
       
            if (ConnIntf.SendedBuf == null)
            {
                throw new Exception(ProtocolName + "-" + Action + "-Unknown error");
            }

            if (PLCConnectType == ConnectType.CT_TouchScreen && ConnIntf.SendedBuf[1] != buffer[1])
            {
                throw new Exception(ProtocolName + "-" + Action + "-Command type identifier verification failed.");
            }
            switch (CommandType)
            {
                case SendInfoType.SIT_InqureStr:
                case SendInfoType.SIT_InqureUInt32:
                case SendInfoType.SIT_InqureParamater:
                    {
                    // 分析结果 -> Analysis results
                    int receiveCount = 0;
                        for (int i = 0; i < length.Length; i++)
                        {
                            receiveCount += length[i];
                        }

                        if (buffer.Length >= 21 && buffer[20] == length.Length)
                        {
                            byte[] bufferContent = new byte[receiveCount];
                            int kk = 0;
                            int ll = 0;
                            for (int ii = 21; ii < buffer.Length; ii++)
                            {
                                if ((ii + 1) < buffer.Length)
                                {
                                    if (buffer[ii] == 0xFF &&
                                        buffer[ii + 1] == 0x04)
                                    {
                                        Array.Copy(buffer, ii + 4, bufferContent, ll, length[kk]);
                                        ii += length[kk] + 3;
                                        ll += length[kk];
                                        kk++;
                                    }
                                }
                            }
                            if (CommandType == SendInfoType.SIT_InqureStr)
                            {
                                ResolveStr(bufferContent);
                            }
                            else if(CommandType == SendInfoType.SIT_InqureUInt32)
                            {
                                ResolveUInt(bufferContent);
                            }
                            else if(CommandType == SendInfoType.SIT_InqureParamater)
                            {
                                ResolveDWordArray(bufferContent, bufferContent.Length);
                            }
                            ExeResult = true;
                        }
                        else
                        {
                            ExeResult = false;
                            PLcStr = string.Empty;
                        }
                    }
                    break;
                case SendInfoType.SIT_InqurMByte1:
                    {
                    if (size != InqurMByteCnt+25)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Total data length is illegal.");
                        }
                        int len = ((buffer[2] << 8) + buffer[3]);
                        if (size != len)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal status data length resolution");
                        }
                        if (ConnIntf.SendedBuf[11] != buffer[11] || ConnIntf.SendedBuf[12] != buffer[12])
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Command serial number verification failed.");
                        }
                        int DataLen = (buffer[15] << 8) + buffer[16];
                        if (DataLen != InqurMByteCnt+4)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Data length is" + DataLen + " illegal！");
                        }
                        if (buffer[21] != 0xFF)
                        {
                            string str = "0x" + buffer[21].ToString("X2");
                            throw new Exception(ProtocolName + "-" + Action + "-Error in response code " + str + " != 0xFF");
                        }
                            
                        int length = RetStatus.Capacity;
                        for (int i = 0; i < length; i++)
                        {
                            ResolveStatusRet(i, buffer);
                        }


                        if (size == InqurMByteCnt + 25)
                        {
                            if (ResolveDose(buffer))
                            {
                                ExeResult = true;
                            }
                        }
                    //ConnIntf.WriteError("SIT_InqurMByte1 Parse complete :", ICommonProtocol.ToHexString(buffer), true);
                    ExeResult = true;
                    }
                    break;
                case SendInfoType.SIT_Command:
                    {
                        if (size != CorrectRet.Count)
                        {
                            //ConnIntf.WriteError("SIT_Command Parse complete :", ICommonProtocol.ToHexString(buffer), true);
                            throw new Exception(ProtocolName + "-" + Action + "-Command return data length is illegal.");
                        }
                        for (int i = 0; i < CorrectRet.Count; i++)
                        {
                            if (buffer[i] != CorrectRet[i])
                            {
                                //throw new Exception(ProtocolName + "-" + Action + "-Illegal data parsing");
                            }
                        }
                        //ConnIntf.WriteError("SIT_Command Parse complete :", ICommonProtocol.ToHexString(buffer), true);
                        ExeResult = true;
                    }
                    break;
                case SendInfoType.SIT_SetValue:
                    {
                        if (size != CorrectRet.Count)
                        {
                            ConnIntf.WriteError("SIT_SetValue Parse complete :", ICommonProtocol.ToHexString(buffer), true);
                            throw new Exception(ProtocolName + "-" + Action + "-Command return data length is illegal.");
                        }
                        for (int i = 0; i < CorrectRet.Count; i++)
                        {
                            if (buffer[i] != CorrectRet[i])
                            {
                                throw new Exception(ProtocolName + "-" + Action + "-Illegal data parsing");
                            }
                        }
                        ExeResult = true;
                    }
                    break;
                case SendInfoType.SIT_Init:
                    {
                        if (size != CorrectRet.Count)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-The length of data returned by setting parameters is illegal.");
                        }
                        ExeResult = true;
                    }
                    break;
            }
            NeedFeedBack = false;
            return true;
        }
        //发送命令前准备
        protected override void PrepareSend(byte[] SendBuf)
        {
            ConnIntf.SendedBuf = null;
            CorrectRet.Clear();

            if (connecttype == ConnectType.CT_DirectConnect)
            {
                switch (CommandType)
                {
                    case SendInfoType.SIT_InqurMByte1:
                        {
                            FunctionName = "Query partial byte 1";
                        }
                        break;
                    case SendInfoType.SIT_Command:
                        {
                            FunctionName = "Command flag";
                            CorrectRet = new byte[22] { 0x03, 0x00, 0x00, 0x16, 0x02, 0xF0, 0x80, 0x32, 0x03, 0x00, 0x00, 0x81, 0x24, 0x00, 0x02, 0x00, 0x01, 0x00, 0x00, 0x05, 0x01, 0xFF }.ToList();
                        }
                        break;
                    case SendInfoType.SIT_SetValue:
                        {
                            FunctionName = "Setting parameters";
                            CorrectRet = new byte[22] { 0x03, 0x00, 0x00, 0x16, 0x02, 0xF0, 0x80, 0x32, 0x03, 0x00, 0x00, 0x00, 0x01, 0x00, 0x02, 0x00, 0x01, 0x00, 0x00, 0x05, 0x01, 0xFF }.ToList();
                        }
                        break;
                    case SendInfoType.SIT_Init:
                        {
                            int size = SendBuf.Length;
                            if (size == 22)
                            {
                                FunctionName = "1 initialization";
                                CorrectRet = new byte[22] { 0x03, 0x00, 0x00, 0x16, 0x11, 0xD0, 0x00, 0x0E, 0x00, 0x13, 0x00, 0xC0,
                                0x01, 0x0A, 0xC1, 0x02, 0x01, 0x01, 0xC2, 0x02, 0x01, 0x01 }.ToList();
                              
                            }
                            else if (size == 25)
                            {
                                FunctionName = "2 initialization";
                                CorrectRet = new byte[27] { 0x03, 0x00, 0x00, 0x1B, 0x02, 0xF0, 0x80, 0x32, 0x03, 0x00, 0x00, 0xCC,
                                0xC1, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x00, 0x00, 0x01, 0x00, 0x01, 0x03, 0xC0 }.ToList();
                            }
                            else
                            {
                                throw new Exception("Command initialization error");
                            }
                        }
                        break;
                }
                NeedFeedBack = true;
                ExeResult = false;
            }
          

            ConnIntf.DebugStepLog("PrepareSend--" + FunctionName);
        }

        public override bool InqurMByte1(ref List<bool> StatusRet, ref List<ushort> dose)
        {
          
            return InqurStatus(ref StatusRet,ref dose);
            
        }

        public override bool InqurHardwareByte1(byte blockType, int blockNum, int blockOffset, ushort[] length,ref List<Byte[]> StatusRet)
        {
            return InqureParamater(blockType, blockNum, blockOffset, length);
        }
        public override bool InqurStr(byte blockType, int blockNum,int blockOffset,ushort[] length)
        {
            
            LockSend();
         
            CommandType = SendInfoType.SIT_InqureStr;
            this.blockType = blockType;
            this.blockNum = blockNum;
            this.blockoffset = blockOffset;
            this.length = length;
            byte[] Command = BuildCommand();
            bool bRet = SendCommand(Command);
          
            if (bRet)
            {
                ConnIntf.DebugLog(FunctionName, "Execution succeeded.");
            }
            else
            {
                ConnIntf.DebugLog(FunctionName, "Execution failed.");
            }
            UnlockSend();
          
            return bRet;
        }
        public override bool InqurUInt(byte blockType, int blockNum, int blockOffset, ushort[] length)
        {
            LockSend();
            CommandType = SendInfoType.SIT_InqureUInt32;
            this.blockType = blockType;
            this.blockNum = blockNum;
            this.blockoffset = blockOffset;
            this.length = length;
            byte[] Command = BuildCommand();
            bool bRet = SendCommand(Command);
            if (bRet)
            {
                ConnIntf.DebugLog(FunctionName, "Execution succeeded.");
            }
            else
            {
                ConnIntf.DebugLog(FunctionName, "Execution failed.");
            }
            UnlockSend();
            return bRet;
        }
        /// <summary>
        /// 查询函数
        /// </summary>
        /// <param name="StatusRet"></param>
        /// <returns></returns>
        public override bool InqureParamater(byte blockType, int blockNum, int blockOffset, ushort[] length)
        {
            LockSend();
            CommandType = SendInfoType.SIT_InqureParamater;
            this.blockType = blockType;
            this.blockNum = blockNum;
            this.blockoffset = blockOffset;
            this.length = length;
            byte[] Command = BuildCommand();
            bool bRet = SendCommand(Command);
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
        //查询状态数据
        public bool InqurStatus(ref List<bool> StatusRet, ref List<ushort> dose)
        {
            LockSend();
            DateTime startDt = DateTime.Now;
            CommandType = SendInfoType.SIT_InqurMByte1;
#if DEBUG
            InqurMByteCnt = 100;
#else
            InqurMByteCnt = 100;
#endif
            byte[] Command = BuildCommand();
            bool bRet = SendCommand(Command);
            if (bRet)
            {
                StatusRet = RetStatus;
                dose = Dose.ToList();
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
        //查询状态数据命令
        public byte[] InqurStatusCommand()
        {
            byte[] Command = null;
            if (PLCConnectType == ConnectType.CT_TouchScreen)
            {
                Command = new byte[12] { 0x00, 0x0A, 0x00, 0x00, 0x00, 0x06, 0x01, 0x01, 0x00, 0x28, 0x00, 0x58 };
            }
            else
            {
                Command = new byte[31] { 0x03, 0x00, 0x00, 0x1F, 0x02, 0xF0, 0x80, 0x32, 0x01, 0x00, 0x00, 0xFF, 0xF1, 0x00, 0x0E,
                                0x00, 0x00, 0x04, 0x01, 0x12, 0x0A, 0x10, 0x02, 0x00, InqurMByteCnt, 0x00, 0x00, 0x83 ,0x00, 0x00, 0x08 };
            }

            return Command;
        }
      
        //查询剂量数据命令
        public byte[] InqurDoseCommand()
        {
            byte[] Command = null;
            if (PLCConnectType == ConnectType.CT_TouchScreen)
            {
                Command = new byte[12] { 0x00, 0x0B, 0x00, 0x00, 0x00, 0x06, 0x01, 0x03, 0x08, 0x0C, 0x00, 0x0A };
            }
            else
            {
                Command = new byte[31] { 0x03, 0x00, 0x00, 0x1F, 0x02, 0xF0, 0x80, 0x32, 0x01, 0x00, 0x00, 0xFF, 0xF2,
                                0x00, 0x0E, 0x00, 0x00, 0x04, 0x01, 0x12, 0x0A, 0x10, 0x02, 0x00, 0x14, 0x00, 0x01, 0x84, 0x00,
                                0x01, 0x90 };
                int count = 10 * 2;
                Command[23] = (byte)((count & 0xFF00) >> 8);
                Command[24] = (byte)(count & 0xFF);
                int startbit = 50 * 8;
                Command[29] = (byte)(startbit >> 8);
                Command[30] = (byte)(startbit & 0xFF);
            }

            return Command;
        }
    }
}
