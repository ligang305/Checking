using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGCommunication
{
    public abstract class PLCSIEMENS_S300Protocol: PLCProtocol
    {
        public PLCSIEMENS_S300Protocol()
        {
            ProtocolName = "S300 Smart";
            PLCConnectType = ConnectType.CT_TouchScreen;
            StartPos = 28;
            InitSystemTypeParas();
        }
        ~PLCSIEMENS_S300Protocol()
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
                        //ConnIntf.WriteError("SIT_InqurMByte1  Request:", ICommonProtocol.ToHexString(Command), true);
                    }
                    break;
                case SendInfoType.SIT_InqureStr:
                case SendInfoType.SIT_InqureUInt32:
                    {
                        Command = BuildReadCommand(this.blockType, this.blockoffset, this.blockNum, this.length);
                    }
                    break;
                case SendInfoType.SIT_Command:
                    {
                        Command = ExecuteCommand(FlagM, FlagB, FlagActive);
                        //ConnIntf.WriteError("SIT_Command  Request:", ICommonProtocol.ToHexString(Command), true);
                    }
                    break;
                case SendInfoType.SIT_SetValue:
                    {
                        Command = SetValueCommand(MStartPos, DataValues);
                        //ConnIntf.WriteError("SIT_SetValue  Request:", ICommonProtocol.ToHexString(Command), true);
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
            string Action = "【" + FunctionName + "】Return data" + ByteToHexString(buffer, ' ');
            //ConnIntf.DebugStepLog(Action + "-Parsing return data");
            switch (CommandType)
            {
                case SendInfoType.SIT_InqureStr:
                case SendInfoType.SIT_InqureUInt32:
                case SendInfoType.SIT_Command:
                    {
                        // 分析结果 -> Analysis results
                        int receiveCount = 0;
                        if (length == null) return false;
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
                            else if (CommandType == SendInfoType.SIT_InqureUInt32)
                            {
                                ResolveUInt(bufferContent);
                            }
                            ExeResult = true;
                            //return OperateResult.CreateSuccessResult(buffer);
                        }
                        else
                        {
                            if (CommandType != SendInfoType.SIT_Command)
                            {
                                ExeResult = false;
                                PLcStr = string.Empty;
                            }
                            else
                            {
                                ExeResult = true;
                            }
                        }
                    }
                    break;
                case SendInfoType.SIT_InqurMByte1:
                    {
                        //ConnIntf.WriteError("SIT_InqurMByte1  Respose:", ICommonProtocol.ToHexString(buffer), true);
                        if (size != InqurMByteCnt + 25)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Total data length is illegal.");
                        }
                        int len = ((buffer[2] << 8) + buffer[3]);
                        if (size != len)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal status data length resolution");
                        }
                        int DataLen = (buffer[15] << 8) + buffer[16];
                        if (DataLen != InqurMByteCnt + 4)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Data length is" + DataLen + " illegal！");
                        }
                        if (buffer[21] != 0xFF)
                        {
                            string str = "0x" + buffer[21].ToString("X2");
                            throw new Exception(ProtocolName + "-" + Action + "-Error in response code " + str + " != 0xFF");
                        }

                        string BackData = ByteToHexString(buffer, ' ');
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
                        ExeResult = true;
                    }
                    break;
                //case SendInfoType.SIT_Command:
                //    {
                //        //ConnIntf.WriteError("SIT_Command  Respose:", ICommonProtocol.ToHexString(buffer), true);
                //        if (size != CorrectRet.Count)
                //        {
                //            throw new Exception(ProtocolName + "-" + Action + "-Command return data length is illegal.");
                //        }
                //        for (int i = 0; i < CorrectRet.Count; i++)
                //        {
                //            if (buffer[i] != CorrectRet[i])
                //            {
                //                throw new Exception(ProtocolName + "-" + Action + "-Illegal data parsing");
                //            }
                //        }
                //        ExeResult = true;
                //    }
                //    break;
                case SendInfoType.SIT_SetValue:
                    {
                        ////ConnIntf.WriteError("SIT_SetValue Respose :", ICommonProtocol.ToHexString(buffer), true);
                        //if (size != CorrectRet.Count)
                        //{
                        //    throw new Exception(ProtocolName + "-" + Action + "-Command return data length is illegal.");
                        //}
                        //for (int i = 0; i < CorrectRet.Count; i++)
                        //{
                        //    if (buffer[i] != CorrectRet[i])
                        //    {
                        //        //throw new Exception(ProtocolName + "-" + Action + "-Illegal data parsing");
                        //    }
                        //}
                        ExeResult = true;
                    }
                    break;
                case SendInfoType.SIT_Init:
                    {
                        if (size != CorrectRet.Count)
                        {
                            //throw new Exception(ProtocolName + "-" + Action + "-设置参数回传数据长度非法");
                        }

                        //for (int i = 0; i < CorrectRet.Count; i++)
                        //{
                        //    if (buffer[i] != CorrectRet[i])
                        //    {Setting parameters
                        //        throw new Exception(ProtocolName + "-" + Action + "-数据解析非法");
                        //    }
                        //}
                        ExeResult = true;
                    }
                    break;
            }
            NeedFeedBack = false;
            return true;
        }
        protected override bool ResolveDose(byte[] RecvBuff)
        {
            int yus = StartPos % 2;
            int start = (StartPos - 28) / 2;
            start = start + yus;
            Dose = new ushort[26];
            for (int i = 0; i < 26; i++)
            {
                int p1 = 53 + i * 2 + yus;
                int p2 = 53 + i * 2 + 1 + yus;
                short b1 = (short)(RecvBuff[p1] << 8);
                short b2 = RecvBuff[p2];
                Dose[start + i] = (ushort)(b1 + b2);
            }
            return true;
        }
        //发送命令前准备
        protected override void PrepareSend(byte[] SendBuf)
        {
            ConnIntf.SendedBuf = null;
            CorrectRet.Clear();
            NeedFeedBack = false;
            if (connecttype == ConnectType.CT_DirectConnect)
            {
                switch (CommandType)
                {
                    case SendInfoType.SIT_InqurMByte1:
                        {
                            FunctionName = "查询部分字节1";
                            NeedFeedBack = true;
                        }
                        break;
                    case SendInfoType.SIT_Command:
                        {
                               FunctionName = "命令标志";
                                CorrectRet = new byte[22] { 0x03, 0x00, 0x00, 0x16, 0x02, 0xF0, 0x80, 0x32, 0x03, 0x00, 0x00, 0x00, 0x01, 0x00, 0x02, 0x00, 0x01, 0x00, 0x00, 0x05, 0x01, 0xFF }.ToList();
                        }
                        break;
                    case SendInfoType.SIT_SetValue:
                        {
                            FunctionName = "Setting parameters";
                            CorrectRet = new byte[22] { 0x03, 0x00, 0x00, 0x16, 0x02, 0xF0, 0x80, 0x32, 0x03, 0x00, 0x00, 0x00, 0x01, 0x00, 0x02, 0x00, 0x01, 0x00, 0x00, 0x05, 0x01,0xFF }.ToList();
                            NeedFeedBack = true;
                        }
                        break;
                    case SendInfoType.SIT_Init:
                        {
                            int size = SendBuf.Length;
                            if (size == 22)
                            {
                                FunctionName = "1 initialization";
                                CorrectRet = new byte[22] {  0x03,0x00,0x00,0x16,0x11,0xE0,0x00,0x00,0x00,0x01,0x00,0xC1,0x02,0x10,0x00,0xC2,0x02,0x03,0x00,0xC0,0x01,0x0A }.ToList();
                            }
                            else if (size == 25)
                            {
                                FunctionName = "2 initialization";
                                CorrectRet = new byte[27] { 0x03,0x00,0x00,0x1B,0x02,0xF0,0x80,0x32,0x03,0x00,0x00,0xCC,0xC1,0x00,0x08,0x00,0x00,0x00,0x00,0xF0,0x00,0x00,0x01 ,0x00,0x01,0x00,0xF0 }.ToList();
                            }
                            else
                            {
                                throw new Exception("Command initialization error");
                            }
                        }
                        break;
                }
                ExeResult = false;
            }
            else
            {
                switch (CommandType)
                {
                    case SendInfoType.SIT_InqurMByte1:
                        {
                            //for (int i = 0; i < RetStatus.Capacity; i++)
                            //{
                            //    RetStatus[i] = false;
                            //}
                            Dose = Dose.ToList().Select(q => { q = 0; return q; }).ToArray();
                            NeedFeedBack = true;
                            ExeResult = false;
                        }
                        break;
                    case SendInfoType.SIT_Command:
                        {
                            NeedFeedBack = false;
                            ExeResult = false;
                        }
                        break;
                    case SendInfoType.SIT_SetValue:
                        {
                            NeedFeedBack = false;
                            ExeResult = false;
                        }
                        break;
                    case SendInfoType.SIT_Init:
                        {
                            NeedFeedBack = false;
                            ExeResult = false;
                        }
                        break;
                }
            }

            //ConnIntf.DebugStepLog("PrepareSend--" + FunctionName);
        }
        //发送命令标志命令
        public override byte[] ExecuteCommand(byte flagm, byte flagb, bool active)
        {
            byte[] Command = null;
            if (PLCConnectType == ConnectType.CT_TouchScreen)
            {
                Command = new byte[12] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x06, 0x01, 0x05, 0x03, 0xAF, 0xFF, 0x00 };
                Command[9] = MBitAddrtoFlag(flagm, flagb);
                Command[10] = (byte)(active ? 0xFF : 0x00); ;
            }
            else
            {
                Command = new byte[36] { 0x03, 0x00, 0x00, 0x24, 0x02, 0xf0, 0x80, 0x32, 0x01, 0x00, 0x00, 0x00, 0x01,
                    0x00, 0x0e, 0x00, 0x05, 0x05, 0x01, 0x12, 0x0a, 0x10,0x01,0x00, 0x01, 0x00, 0x00, 0x83, 0x00, 0x00,
                    0x7a, 0x00, 0x03, 0x00, 0x01, 0x01};

                int addr = flagm * 8 + flagb;
                Command[29] = (byte)((addr >> 8) & 0xFF);
                Command[30] = (byte)(addr & 0xFF);
                Command[35] = (byte)(active ? 0x01 : 0x00);
                //byte[] datas = new byte[1] { (byte)(active ? 0x01 : 0x00) };
                //Command = BuildWriteByteCommand(flagm, datas, flagb, 0x01);
            }

            return Command;
        }


        public override byte[] InitPLCCommand(int Index)
        {
            byte[] Command = null;
            if (PLCConnectType == ConnectType.CT_TouchScreen)
            {
            }
            else
            {
                if (Index == 0)
                {
                    Command = new byte[22] { 0x03,0x00,0x00,0x16,0x11,0xE0,0x00,0x00,0x00,0x01,0x00,0xC1,0x02,0x10,0x00,0xC2,
                            0x02,0x03,0x00,0xC0,0x01,0x0A  };
                }
                else if (Index == 1)
                {
                    Command = new byte[25] {0x03,0x00,0x00,0x19,0x02,0xF0,0x80,0x32,0x01,0x00,0x00,0xCC,0xC1,0x00,0x08,0x00,
                            0x00,0xF0,0x00,0x00,0x01,0x00,0x01,0x03,0xC0 };
                }
                else
                {
                    throw new Exception("Incorrect initialization times");
                }
            }
            return Command;
        }


        public override bool InqurMByte1(ref List<bool> StatusRet, ref List<ushort> dose)
        {
            return InqurStatus(ref StatusRet, ref dose);
        }

        //查询状态数据
        public bool InqurStatus(ref List<bool> StatusRet, ref List<ushort> dose)
        {
            LockSend();
            CommandType = SendInfoType.SIT_InqurMByte1;
#if DEBUG
            InqurMByteCnt = 30;
#else
            InqurMByteCnt = 30;
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
                //Command = BuildBitReadCommand(InqurMByteCnt);
                Command = new byte[31] { 0x03, 0x00, 0x00, 0x1F, 0x02, 0xF0, 0x80, 0x32, 0x01, 0x00, 0x00, 0xFF, 0xF1, 0x00, 0x0E,
                                0x00, 0x00, 0x04, 0x01, 0x12, 0x0A, 0x10, 0x02, 0x00, InqurMByteCnt, 0x00, 0x00, 0x83 ,0x00, 0x00, 0x08 };

               var temp =  ICommonProtocol.ToHexString(Command);
            }

            return Command;
        }
        public override byte MBitAddrtoFlag(byte FlagM, byte FlagB)
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
            byte tmp = (byte)((FlagM - 111) * 8 + FlagB);
            byte Flag = (byte)(start + tmp);
            if (Flag > 0xFC)
            {
                return 0x00;
            }

            return Flag;
        }

        /// <summary>
        /// 生成一个位读取数据指令头的通用方法 ->
        /// A general method for generating a bit-read-Data instruction header
        /// </summary>
        /// <param name="address">起始地址，例如M100.0，I0.1，Q0.1，DB2.100.2 ->
        /// Start address, such as M100.0,I0.1,Q0.1,DB2.100.2
        /// </param>
        /// <returns>包含结果对象的报文 -> Message containing the result object</returns>
        public static byte[] BuildBitReadCommand(byte ReadCount)
        {
            byte[] _PLCCommand = new byte[31];
            _PLCCommand[0] = 0x03;
            _PLCCommand[1] = 0x00;
            // 长度 -> Length
            _PLCCommand[2] = (byte)(_PLCCommand.Length / 256);
            _PLCCommand[3] = (byte)(_PLCCommand.Length % 256);
            // 固定 -> Fixed
            _PLCCommand[4] = 0x02;
            _PLCCommand[5] = 0xF0;
            _PLCCommand[6] = 0x80;
            _PLCCommand[7] = 0x32;
            // 命令：发 -> command to send
            _PLCCommand[8] = 0x01;
            // 标识序列号
            _PLCCommand[9] = 0x00;
            _PLCCommand[10] = 0x00;
            _PLCCommand[11] = 0x00;
            _PLCCommand[12] = 0x01;
            // 命令数据总长度 -> Identification serial Number
            _PLCCommand[13] = (byte)((_PLCCommand.Length - 17) / 256);
            _PLCCommand[14] = (byte)((_PLCCommand.Length - 17) % 256);

            _PLCCommand[15] = 0x00;
            _PLCCommand[16] = 0x00;

            // 命令起始符 -> Command start character
            _PLCCommand[17] = 0x04;
            // 读取数据块个数 -> Number of data blocks read
            _PLCCommand[18] = 0x01;

            //===========================================================================================
            // 读取地址的前缀 -> Read the prefix of the address
            _PLCCommand[19] = 0x12;
            _PLCCommand[20] = 0x0A;
            _PLCCommand[21] = 0x10;
            // 读取的数据时位 -> Data read-time bit
            _PLCCommand[22] = 0x02;
            // 访问数据的个数 -> Number of Access data
            _PLCCommand[23] = 0x00;
            _PLCCommand[24] = ReadCount;
            // DB块编号，如果访问的是DB块的话 -> DB block number, if you are accessing a DB block
            _PLCCommand[25] = (byte)(0 / 256);
            _PLCCommand[26] = (byte)(0 % 256);
            // 访问数据类型 -> Types of reading data
            // M区 0x83 I区0x81 Q区0x82  DB区0x84
            _PLCCommand[27] = 0x83;
            // 偏移位置 -> Offset position
            _PLCCommand[28] = (byte)(808 / 256 / 256 % 256);
            _PLCCommand[29] = (byte)(808 / 256 % 256);
            _PLCCommand[30] = (byte)(808 % 256);

            string TempPLC = ByteToHexString(_PLCCommand,' ');
            return _PLCCommand;
        }


        /// <summary>
        /// 字节数据转化成16进制表示的字符串 ->
        /// Byte data into a string of 16 binary representations
        /// </summary>
        /// <param name="InBytes">字节数组</param>
        /// <param name="segment">分割符</param>
        /// <returns>返回的字符串</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="ByteToHexStringExample2" title="ByteToHexString示例" />
        /// </example>
        public static string ByteToHexString(byte[] InBytes, char segment)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte InByte in InBytes)
            {
                if (segment == 0) sb.Append(string.Format("{0:X2}", InByte));
                else sb.Append(string.Format("{0:X2}{1}", InByte, segment));
            }

            if (segment != 0 && sb.Length > 1 && sb[sb.Length - 1] == segment)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
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


