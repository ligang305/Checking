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
   
    public abstract class PLCProtocol : ICommonProtocol
    {
        public PLCProtocol()
        {
            ProtocolName = "Control station -PLC protocol";
            PLCConnectType = ConnectType.CT_TouchScreen;
            InitSystemTypeParas();
        }
        ~PLCProtocol()
        {
            Console.WriteLine("Structure--" + ProtocolName);
        }

        public enum ConnectType
        {
            CT_TouchScreen = 0,
            CT_DirectConnect = 1,
        }

        //发送命令类型
        public enum SendInfoType
        {
            //查询全部字节
            SIT_InqurMByte1 = 0,
            //命令标志
            SIT_Command = 1,
            //设置数据
            SIT_SetValue = 2,
            //初始化
            SIT_Init = 3,
            //查询部分字节
            SIT_InqurMByte2 = 4,
            //查询PLC字符串
            SIT_InqureStr =8,
            //查询PLCUint32
            SIT_InqureUInt32 = 32,
            //查询硬件参数
            SIT_InqurHandWare = 6,
            //批量查询参数
            SIT_InqureParamater = 64,
        }
        /// <summary>
        /// 应用于多字节数据的解析或是生成格式
        /// </summary>
        public enum DataFormat
        {
            /// <summary>
            /// 按照顺序排序
            /// </summary>
            ABCD = 0,
            /// <summary>
            /// 按照单字反转
            /// </summary>
            BADC = 1,
            /// <summary>
            /// 按照双字反转
            /// </summary>
            CDAB = 2,
            /// <summary>
            /// 按照倒序排序
            /// </summary>
            DCBA = 3,
        }
        #region 块
        /// <summary>
        /// 块的类型，DB,M,I,Q分别对应不同的type
        /// </summary>
        protected byte blockType;
        /// <summary>
        /// 快的编号
        /// </summary>
        protected int blockNum;
        /// <summary>
        /// 快的偏移量
        /// </summary>
        protected int blockoffset;
        /// <summary>
        /// 从块开始查询的长度
        /// </summary>
        protected ushort[] length;
        #endregion
        protected byte InqurMByteCnt = 70;

        public byte StartPos = 50;

        protected int port = 8000;
        public ConnectType connecttype = ConnectType.CT_TouchScreen;

        public int Port
        {
            get { return port; }
        }
        public ConnectType PLCConnectType
        {
            get { return connecttype; }
            set
            {
                connecttype = value;
                if (connecttype == ConnectType.CT_TouchScreen)
                {
                    port = 8000;
                }
                else
                {
                    port = 102;
                }
            }
        }

        #region 查询状态

        //当前加速器类型有效的状态数据
        protected List<bool> RetStatus = new List<bool>();
        //一些字符串
        public string PLcStr = string.Empty;

        public byte[] StatusValues;

        public bool GetStatus(byte M, byte b)
        {
            int nPos = M * 8 + b;
            if (nPos > RetStatus.Count || nPos < 0)
            {
                throw new Exception("查询状态超出范围");
            }
            return RetStatus[nPos];
        }
        /// <summary>
        /// 传类似于25.3这种的
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool GetStatus(string position)
        {
            
            if (position.Contains("."))
            {
                int M =Convert.ToInt32(position.Split('.')[0]) - 1;
                int b = Convert.ToInt32(position.Split('.')[1]);
                int nPos = M * 8 + b;
                if (nPos > RetStatus.Count || nPos < 0)
                {
                    throw new Exception("查询状态超出范围");
                }
                return RetStatus[nPos];
            }
            return false;
            
        }
        //解析回传状态数据
        protected virtual void ResolveStatusRet(int StatusPos, byte[] RecvBuff)
        {
            int DataPos = (PLCConnectType == ConnectType.CT_TouchScreen ? 9 : 25);
            int Status = StatusPos;
            int BytePos = Status / 8;
            int BitPos = Status % 8;
            byte StatusByte = RecvBuff[DataPos + BytePos];
            byte n = (byte)Math.Pow(2, BitPos);
            int Ret = (StatusByte & n);
            RetStatus[StatusPos] = (Ret == 0 ? false : true);
        }
        protected virtual void ResolveStr(byte[] RecvBuff)
        {
            PLcStr = TransString(RecvBuff,0, RecvBuff.Length,Encoding.ASCII);
        }
        protected virtual void ResolveUInt(byte[] RecvBuff)
        {
            PLcStr = TransUInt(RecvBuff, 0, 1, Encoding.ASCII);
        }
        protected virtual void ResolveByteArray(byte[] RecvBuff)
        {
            StatusValues = RecvBuff;
        }
        protected virtual void ResolveDWordArray(byte[] RecvBuff,int length)
        {
           
        }
        /// <summary>
        /// 从缓存中提取string结果，使用指定的编码
        /// </summary>
        /// <param name="buffer">缓存对象</param>
        /// <param name="index">索引位置</param>
        /// <param name="length">byte数组长度</param>
        /// <param name="encoding">字符串的编码</param>
        /// <returns>string对象</returns>
        public virtual string TransString(byte[] buffer, int index, int length, Encoding encoding)
        {
            byte[] tmp = TransByte(buffer, index, length);
            return encoding.GetString(tmp);
        }
        /// <summary>
        /// 从缓存中提取string结果，使用指定的编码
        /// </summary>
        /// <param name="buffer">缓存对象</param>
        /// <param name="index">索引位置</param>
        /// <param name="length">byte数组长度</param>
        /// <param name="encoding">字符串的编码</param>
        /// <returns>string对象</returns>
        public virtual string TransUInt(byte[] buffer, int index, int length, Encoding encoding)
        {
            uint[] tmp = new uint[length];
            for (int i = 0; i < length; i++)
            {
                tmp[i] = TransUInt32(buffer, index + 4 * i);
            }
            return tmp[0].ToString();
        }
        /// <summary>
        /// 从缓存中提取DWord结果，使用指定的编码
        /// </summary>
        /// <param name="buffer">缓存对象</param>
        /// <param name="index">索引位置</param>
        /// <param name="length">byte数组长度</param>
        /// <param name="encoding">字符串的编码</param>
        /// <returns>string对象</returns>
        public virtual byte[] TransUIntArray(byte[] buffer, int index, int length, Encoding encoding)
        {
            byte[] tmp = TransByte(buffer, index, length);
            return tmp;
        }
        /// <summary>
        /// 从缓存中提取byte数组结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <param name="length">读取的数组长度</param>
        /// <returns>byte数组对象</returns>
        public virtual byte[] TransByte(byte[] buffer, int index, int length)
        {
            byte[] tmp = new byte[length];
            Array.Copy(buffer, index, tmp, 0, length);
            return tmp;
        }
        /// <summary>
        /// 从缓存中提取uint结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>uint对象</returns>
        public virtual uint TransUInt32(byte[] buffer, int index)
        {
            byte[] tmp = new byte[4];
            tmp[0] = buffer[3 + index];
            tmp[1] = buffer[2 + index];
            tmp[2] = buffer[1 + index];
            tmp[3] = buffer[0 + index];
            return BitConverter.ToUInt32(ByteTransDataFormat4(tmp, index), 0);
        }
        /// <summary>
        /// 反转多字节的数据信息
        /// </summary>
        /// <param name="value">数据字节</param>
        /// <param name="index">起始索引，默认值为0</param>
        /// <returns>实际字节信息</returns>
        protected byte[] ByteTransDataFormat4(byte[] value, int index = 0)
        {
            byte[] buffer = new byte[4];
            //switch (DataFormat)
            //{
            //    case DataFormat.ABCD:
            //        {
            //            buffer[0] = value[index + 3];
            //            buffer[1] = value[index + 2];
            //            buffer[2] = value[index + 1];
            //            buffer[3] = value[index + 0];
            //            break;
            //        }
            //    case DataFormat.BADC:
            //        {
            //            buffer[0] = value[index + 2];
            //            buffer[1] = value[index + 3];
            //            buffer[2] = value[index + 0];
            //            buffer[3] = value[index + 1];
            //            break;
            //        }

            //    case DataFormat.CDAB:
            //        {
            //            buffer[0] = value[index + 1];
            //            buffer[1] = value[index + 0];
            //            buffer[2] = value[index + 3];
            //            buffer[3] = value[index + 2];
            //            break;
            //        }
            //    case DataFormat.DCBA:
            //        {
                        buffer[0] = value[index + 0];
                        buffer[1] = value[index + 1];
                        buffer[2] = value[index + 2];
                        buffer[3] = value[index + 3];
                        //break;
                    //}
            //}
            return buffer;
        }
        #endregion

        #region 命令标志

        //命令标志字节所在位置
        protected byte FlagM = 11;
        //命令标志位所在位置
        protected byte FlagB = 0;
        //是否有效
        protected bool FlagActive = true;
        //当前乘用车类型有效的命令标志表
        protected List<byte> Flags = new List<byte>();
        public override byte MBitAddrtoFlag(byte FlagM,byte FlagB)
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
            byte tmp = (byte)((FlagM - 21) * 8 + FlagB);
            byte Flag = (byte)(start + tmp);
            if (Flag > 0xED)
            {
                return 0x00;
            }

            return Flag;
        }
        //设置命令标志及是否激活
        protected virtual void SetCommandflag(byte flagm, byte flagb, bool Active)
        {
            if (CommandType != SendInfoType.SIT_Command)
            {
                throw new Exception("必须先设置发送命令模式才能设置命令标志");
            }

            byte flag = MBitAddrtoFlag(flagm, flagb);
            bool legal = false;
            foreach (var f in Flags)
            {
                if (flag == f)
                {
                    legal = true;
                    break;
                }
            }
            if (!legal)
            {
                //throw new Exception("命令标志非法");
            }

            FlagM = flagm;
            FlagB = flagb;
            FlagActive = Active;
        }
        #endregion
        
        #region 查询剂量
        //剂量数组
        protected ushort[] Dose = new ushort[26];
        
        //public ushort[] GetDose() { return Dose; }
        
        //解析剂量及文本数据
        //这里考虑温度负数 所以不能用ushort
        protected virtual bool ResolveDose(byte[] RecvBuff)
        {
            if (PLCConnectType == ConnectType.CT_TouchScreen)
            {
                for (int i = 0; i < 26; i++)
                {
                    Dose[i] = (ushort)((RecvBuff[9 + i * 2] << 8) + RecvBuff[9 + i * 2 + 1]);
                }
            }
            else
            {
                int yus = StartPos % 2;
                int start = (StartPos - 50) / 2;
                start = start + yus;
                Dose = new ushort[26];
                for (int i = 0; i < 26; i++)
                {
                    int p1 = 74 + i * 2 + yus;
                    int p2 = 74 + i * 2 + 1 + yus;
                    short b1 = (short)(RecvBuff[p1] << 8);
                    short b2 = RecvBuff[p2];
                    Dose[start+i] = (ushort)(b1 + b2);
                }
            }
            return true;
        }
        #endregion

        #region 设置数据
        protected byte MStartPos = 0;
        protected List<byte> DataValues = new List<byte>();

        protected virtual void SetValuePara(byte MPos, List<byte> values)
        {
            MStartPos = MPos;
            DataValues.Clear();
            foreach (byte val in values)
            {
                DataValues.Add(val);
            }
        }
        #endregion

        #region 初始化
        protected int InitIndex = 0;
        public virtual void SetInitIndex(int Index)
        {
            if (CommandType != SendInfoType.SIT_Init)
            {
                throw new Exception("必须先设置初始化模式才能设置初始次数标志");
            }
            if (connecttype != ConnectType.CT_DirectConnect)
            {
                throw new Exception("直连模式才能设置初始次数标志");
            }
            InitIndex = Index;
            if(InitIndex == 1)
            {
                FunctionName = "初始化2";
                ConnIntf.DebugStepLog("开始" + FunctionName);
            }
        }
        #endregion

        //发送命令类型
        private SendInfoType commmandType = SendInfoType.SIT_InqurMByte1;

        //设置命令类型
        protected SendInfoType CommandType
        {
            get { return commmandType; }
            set
            {
                commmandType = value;
                switch (commmandType)
                {
                    case SendInfoType.SIT_InqurMByte1:
                        {
                            FunctionName = "Query partial byte 1";
                        }
                        break;
                    case SendInfoType.SIT_Command:
                        {
                            FunctionName = "Command flag";
                        }
                        break;
                    case SendInfoType.SIT_SetValue:
                        {
                            FunctionName = "Setting parameters";
                        }
                        break;
                    case SendInfoType.SIT_Init:
                        {
                            FunctionName = "1 initialization";
                        }
                        break;
     
                }
                //ConnIntf.DebugStepLog("begin" + FunctionName);
            }
        }

        protected abstract void InitSystemTypeParas();

        //命令含义
        public override string CommandMeaning()
        {
            return FunctionName;
        }

        //生成发送命令
        public override byte[] BuildCommand()
        {
            byte[] Command = null;
            switch (CommandType)
            {
                case SendInfoType.SIT_InqurMByte1:
                    {
                        Command = InqurMByte1Command();
                    }
                    break;
                case SendInfoType.SIT_InqureParamater:
                case SendInfoType.SIT_InqureStr:
                case SendInfoType.SIT_InqureUInt32:
                    {
                        Command = BuildReadCommand(this.blockType, this.blockoffset, this.blockNum, this.length);
                    }
                    break;
                case SendInfoType.SIT_Command:
                    {
                        Command = ExecuteCommand(FlagM, FlagB,FlagActive);
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

            if (PLCConnectType == ConnectType.CT_TouchScreen)
            {
                switch(CommandType)
                {
                    case SendInfoType.SIT_InqurMByte1:
                        {
                            if (size != 20)
                            {
                                throw new Exception(ProtocolName + "-" + Action + "-Illegal length of status data");
                            }
                            int len = RetStatus.Capacity;
                            for (int i = 0; i < len; i++)
                            {
                                ResolveStatusRet(i, buffer);
                            }
                            ExeResult = true;
                        }
                        break;
                    case SendInfoType.SIT_Command:
                    case SendInfoType.SIT_SetValue:
                    case SendInfoType.SIT_Init:
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Touch screen mode return data processing error");
                        }
                }
            }
            else
            {
                switch (CommandType)
                {
                    case SendInfoType.SIT_InqureStr:
                    case SendInfoType.SIT_InqureParamater:
                    case SendInfoType.SIT_InqureUInt32:
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
                                    ResolveByteArray(bufferContent);
                                }
                                ExeResult = true;
                                //return OperateResult.CreateSuccessResult(buffer);
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
                            if (size != InqurMByteCnt + 25)
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
                            if (DataLen != InqurMByteCnt + 4)
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
                            ExeResult = true;
                        }
                        break;
                    case SendInfoType.SIT_Command:
                        {
                            if (size != CorrectRet.Count)
                            {
                                ConnIntf.WriteError("SIT_Command Parse complete :", ICommonProtocol.ToHexString(buffer), true);
                                throw new Exception(ProtocolName + "-" + Action + "-Command return data length is illegal.");
                            }
                            for (int i = 0; i < CorrectRet.Count; i++)
                            {
                                if (buffer[i] != CorrectRet[i])
                                {
                                    ConnIntf.WriteError("SIT_Command Illegal :", ICommonProtocol.ToHexString(buffer), true);
                                    ConnIntf.WriteError("SIT_Command Illegal :", ICommonProtocol.ToHexString(CorrectRet.ToArray()), true);
                                    throw new Exception(ProtocolName + "-" + Action + "-Illegal data parsing");
                                }
                            }
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
                                    ConnIntf.WriteError("SIT_SetValue Illegal :", ICommonProtocol.ToHexString(buffer), true);
                                    ConnIntf.WriteError("SIT_SetValue Illegal :", ICommonProtocol.ToHexString(CorrectRet.ToArray()), true);
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
                switch(CommandType)
                {
                    case SendInfoType.SIT_InqurMByte1:
                        {
                            FunctionName = "Query partial byte 1";
                            //for (int i = 0; i < RetStatus.Capacity; i++)
                            //{
                            //    RetStatus[i] = false;
                            //}
                            Dose = Dose.ToList().Select(q => { q = 0; return q; }).ToArray();
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
            else
            {
                switch(CommandType)
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

            ConnIntf.DebugStepLog("PrepareSend--" + FunctionName);
        }

        public virtual bool InqurMByte1(ref List<bool> StatusRet, ref List<ushort> dose)
        {
            return false;
        }
        public virtual bool InqurHardwareByte1(byte blockType, int blockNum, int blockOffset, ushort[] length, ref List<Byte[]> StatusRet)
        {
            return false;
        }
        //查询状态数据命令
        public virtual byte[] InqurMByte1Command()
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

        public virtual bool InqurStr(byte blockType, int blockNum, int blockOffset, ushort[] length)
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
        public virtual bool InqurUInt(byte blockType, int blockNum, int blockOffset, ushort[] length)
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
        public virtual bool InqureParamater(byte blockType, int blockNum, int blockOffset, ushort[] length)
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
            UnlockSend();
            return bRet;
        }
        //查询状态数据命令
        public virtual byte[] InqurMByte2Command()
        {
            byte[] Command = null;

            return Command;
        }

       /// <summary>
       /// 发送报文
       /// </summary>
       /// <param name="flagm">点位M 13 14 15</param>
       /// <param name="flagb">点位的值：13.1中的1</param>
       /// <param name="active"></param>
       /// <returns></returns>
        public override bool Execute(byte flagm, byte flagb, bool active)
        {
            LockSend();
            bool bRet = false;
            try
            {
                CommandType = SendInfoType.SIT_Command;
                SetCommandflag(flagm, flagb, active);
                byte[] Command = BuildCommand();
                bRet = SendCommand(Command);
                if (bRet)
                {
                    ConnIntf.DebugLog(FunctionName, "Execution succeeded.");
                }
                else
                {
                    ConnIntf.DebugLog(FunctionName, "Execution failed.");
                }
                FunctionName = "";
            }
            catch (Exception ex)
            {
                UnlockSend();
                throw ex;
            }
            UnlockSend();
            return bRet;
        }
        /// <summary>
        /// 发送PLC报文
        /// </summary>
        /// <param name="flagm">M区字节序号</param>
        /// <param name="flagb">所属区字节位号</param>
        /// <param name="active">点位是True,还是False</param>
        /// <returns></returns>
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
                byte[] datas = new byte[1] { (byte)(active ? 0x01 : 0x00) };
                Command = BuildWriteByteCommand(flagm, datas, flagb, 0x01);
            }
            return Command;
        }

        /// <summary>
        /// 生成一个读取字数据指令头的通用方法 ->
        /// A general method for generating a command header to read a Word data
        /// </summary>
        /// <param name="address">起始地址，例如M100，I0，Q0，DB2.100 ->
        /// Start address, such as M100,I0,Q0,DB2.100</param>
        /// <param name="length">读取数据长度 -> Read Data length</param>
        /// <returns>包含结果对象的报文 -> Message containing the result object</returns>
        protected byte[]  BuildReadCommand(byte blockType, int OffsetPosition, int blockNum, ushort[] length)
        {
            if (length == null) throw new NullReferenceException("count");
            if (length.Length > 19) throw new Exception("SiemensReadLengthCannotLargerThan19");

            int readCount = length.Length;
            byte[] _PLCCommand = new byte[19 + readCount * 12];
            // ======================================================================================
            _PLCCommand[0] = 0x03;                                                // 报文头 -> Head
            _PLCCommand[1] = 0x00;
            _PLCCommand[2] = (byte)(_PLCCommand.Length / 256);                    // 长度 -> Length
            _PLCCommand[3] = (byte)(_PLCCommand.Length % 256);
            _PLCCommand[4] = 0x02;                                                // 固定 -> Fixed
            _PLCCommand[5] = 0xF0;
            _PLCCommand[6] = 0x80;
            _PLCCommand[7] = 0x32;                                                // 协议标识 -> Protocol identification
            _PLCCommand[8] = 0x01;                                                // 命令：发 -> Command: Send
            _PLCCommand[9] = 0x00;                                                // redundancy identification (reserved): 0x0000;
            _PLCCommand[10] = 0x00;                                               // protocol data unit reference; it’s increased by request event;
            _PLCCommand[11] = 0x00;
            _PLCCommand[12] = 0x01;                                               // 参数命令数据总长度 -> Parameter command Data total length
            _PLCCommand[13] = (byte)((_PLCCommand.Length - 17) / 256);
            _PLCCommand[14] = (byte)((_PLCCommand.Length - 17) % 256);
            _PLCCommand[15] = 0x00;                                               // 读取内部数据时为00，读取CPU型号为Data数据长度 -> Read internal data is 00, read CPU model is data length
            _PLCCommand[16] = 0x00;
            // =====================================================================================
            _PLCCommand[17] = 0x04;                                               // 读写指令，04读，05写 -> Read-write instruction, 04 read, 05 Write
            _PLCCommand[18] = (byte)readCount;                                    // 读取数据块个数 -> Number of data blocks read

            for (int ii = 0; ii < readCount; ii++)
            {
                //===========================================================================================
                // 指定有效值类型 -> Specify a valid value type
                _PLCCommand[19 + ii * 12] = 0x12;
                // 接下来本次地址访问长度 -> The next time the address access length
                _PLCCommand[20 + ii * 12] = 0x0A;
                // 语法标记，ANY -> Syntax tag, any
                _PLCCommand[21 + ii * 12] = 0x10;
                // 按字为单位 -> by word
                _PLCCommand[22 + ii * 12] = 0x02;
                // 访问数据的个数 -> Number of Access data
                _PLCCommand[23 + ii * 12] = (byte)(length[ii] / 256);
                _PLCCommand[24 + ii * 12] = (byte)(length[ii] % 256);
                // DB块编号，如果访问的是DB块的话 -> DB block number, if you are accessing a DB block
                _PLCCommand[25 + ii * 12] = (byte)(blockNum / 256);
                _PLCCommand[26 + ii * 12] = (byte)(blockNum % 256);
                // 访问数据类型 -> Accessing data types
                _PLCCommand[27 + ii * 12] = blockType;
                // 偏移位置 -> Offset position
                _PLCCommand[28 + ii * 12] = (byte)(OffsetPosition / 256 / 256 % 256);
                _PLCCommand[29 + ii * 12] = (byte)(OffsetPosition / 256 % 256);
                _PLCCommand[30 + ii * 12] = (byte)(OffsetPosition % 256);
            }

            string CommandStr = string.Empty;
            for (int i = 0; i < _PLCCommand.Length; i++)
            {
                CommandStr += _PLCCommand[i] + " ";
            }
            return _PLCCommand;
        }
        public bool SetValue(byte MAddr, ushort Val)
        {
            LockSend();
            List<byte> Values = new List<byte>();
            byte tmp1 = (byte)((Val >> 8) & 0xFF);
            byte tmp2 = (byte)(Val & 0xFF);
            Values.Add(tmp1);
            Values.Add(tmp2);
            bool bRet = SetValue(MAddr, Values);
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
        public bool SetValue(byte MAddr, float Val)
        {
            LockSend();
            List<byte> Values = new List<byte>();
            byte[] datas = BitConverter.GetBytes(Val);
            Values.AddRange(datas.Reverse().ToList());
            bool bRet = SetValue(MAddr, Values);
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
        /// 给PLC地址写入值的方法
        /// </summary>
        /// <param name="MAddr">起始地址，默认M区，如需要其他区，请参照报文扩充</param>
        /// <param name="Val">写入地址的值</param>
        /// <returns></returns>
        public bool SetValue(byte MAddr, UInt32 Val)
        {
            LockSend();
            List<byte> Values = new List<byte>();
            byte tmp1 = (byte)((Val >> 24) & 0xFF);
            byte tmp2 = (byte)((Val >> 16) & 0xFF);
            byte tmp3 = (byte)((Val >> 8) & 0xFF);
            byte tmp4 = (byte)(Val & 0xFF);
            Values.Add(tmp1);
            Values.Add(tmp2);
            Values.Add(tmp3);
            Values.Add(tmp4);
            bool bRet = SetValue(MAddr, Values);
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
        /// 给PLC地址写入值的方法
        /// </summary>
        /// <param name="MAddr">起始地址，默认M区，如需要其他区，请参照报文扩充</param>
        /// <param name="Val"></param>
        /// <returns></returns>
        protected override bool SetValue(byte MAddr, List<byte> values)
        {
            CommandType = SendInfoType.SIT_SetValue;
            SetValuePara(MAddr, values);
            byte[] Command = BuildCommand();
            bool bRet = SendCommand(Command);
            return bRet;
        }
        public override byte[] SetValueCommand(byte MAddr, List<byte> values)
        {
            byte[] Command = null;
            Command = BuildWriteWordCommand(MAddr, values.ToArray());
            return Command;
        }

        /// <summary>
        /// 生成一个写入字节数据的指令 -> Generate an instruction to write byte data
        /// </summary>
        /// <param name="analysis">起始地址，示例M100,I100,Q100,DB1.100 -> Start Address, example M100,I100,Q100,DB1.100</param>
        /// <param name="data">原始的字节数据 -> Raw byte data</param>
        /// <returns>包含结果对象的报文 -> Message containing the result object</returns>
        protected byte[] BuildWriteWordCommand(byte MAddr, byte[] data, byte flagb = 0, byte WordOrBit = 0x02, DataBlock dataBlock = DataBlock.M_Block)
        {
            int addr = MAddr * 8 + flagb;
            byte[] _PLCCommand = new byte[35 + data.Length];
            _PLCCommand[0] = 0x03;
            _PLCCommand[1] = 0x00;
            // 长度 -> Length
            _PLCCommand[2] = (byte)((35 + data.Length) / 256);
            _PLCCommand[3] = (byte)((35 + data.Length) % 256);
            // 固定 -> Fixed
            _PLCCommand[4] = 0x02;
            _PLCCommand[5] = 0xF0;
            _PLCCommand[6] = 0x80;
            _PLCCommand[7] = 0x32;
            // 命令 发 -> command to send
            _PLCCommand[8] = 0x01;
            // 标识序列号 -> Identification serial Number
            _PLCCommand[9] = 0x00;
            _PLCCommand[10] = 0x00;
            _PLCCommand[11] = 0x00;
            _PLCCommand[12] = 0x01;
            // 固定 -> Fixed
            _PLCCommand[13] = 0x00;
            _PLCCommand[14] = 0x0E;
            // 写入长度+4 -> Write Length +4
            _PLCCommand[15] = (byte)((4 + data.Length) / 256);
            _PLCCommand[16] = (byte)((4 + data.Length) % 256);
            // 读写指令 -> Read and write instructions
            _PLCCommand[17] = 0x05;
            // 写入数据块个数 -> Number of data blocks written
            _PLCCommand[18] = 0x01;
            // 固定，返回数据长度 -> Fixed, return data length
            _PLCCommand[19] = 0x12;
            _PLCCommand[20] = 0x0A;
            _PLCCommand[21] = 0x10;
            // 写入方式，1是按位，2是按字 -> Write mode, 1 is bitwise, 2 is by word
            _PLCCommand[22] = WordOrBit;
            // 写入数据的个数 -> Number of Write Data
            _PLCCommand[23] = (byte)(data.Length / 256);
            _PLCCommand[24] = (byte)(data.Length % 256);
            // DB块编号，如果访问的是DB块的话 -> DB block number, if you are accessing a DB block
            _PLCCommand[25] = (byte)(0);
            _PLCCommand[26] = (byte)(0);
            // 写入数据的类型 -> Types of writing data
            //0x83 代表M区 ；
            _PLCCommand[27] = (byte)dataBlock;
            // 偏移位置 -> Offset position
            _PLCCommand[28] = (byte)(0);
            _PLCCommand[29] = (byte)((addr >> 8) & 0xFF);
            _PLCCommand[30] = (byte)(addr & 0xFF);
            // 按字写入 -> Write by Word
            _PLCCommand[31] = 0x00;
            _PLCCommand[32] = 0x04;
            // 按位计算的长度 -> The length of the bitwise calculation
            _PLCCommand[33] = (byte)(data.Length * 8 / 256);
            _PLCCommand[34] = (byte)(data.Length * 8  % 256);

            data.CopyTo(_PLCCommand, 35);
            var plc = ToHexString(_PLCCommand);
            return _PLCCommand;
        }

        /// <summary>
        /// 生成一个写入字节数据的指令 -> Generate an instruction to write byte data
        /// </summary>
        /// <param name="analysis">起始地址，示例M100,I100,Q100,DB1.100 -> Start Address, example M100,I100,Q100,DB1.100</param>
        /// <param name="data">原始的字节数据 -> Raw byte data</param>
        /// <returns>包含结果对象的报文 -> Message containing the result object</returns>
        protected byte[] BuildWriteByteCommand(byte MAddr, byte[] data, byte flagb = 0, byte WordOrBit = 0x02,DataBlock dataBlock = DataBlock.M_Block)
        {
            int addr = MAddr * 8 + flagb;
            byte[] _PLCCommand = new byte[35 + data.Length];
            _PLCCommand[0] = 0x03;
            _PLCCommand[1] = 0x00;
            // 长度 -> Length
            _PLCCommand[2] = (byte)((35 + data.Length) / 256);
            _PLCCommand[3] = (byte)((35 + data.Length) % 256);
            // 固定 -> Fixed
            _PLCCommand[4] = 0x02;
            _PLCCommand[5] = 0xF0;
            _PLCCommand[6] = 0x80;
            _PLCCommand[7] = 0x32;
            // 命令 发 -> command to send
            _PLCCommand[8] = 0x01;
            // 标识序列号 -> Identification serial Number
            _PLCCommand[9] = 0x00;
            _PLCCommand[10] = 0x00;
            _PLCCommand[11] = 0x00;
            _PLCCommand[12] = 0x01;
            // 固定 -> Fixed
            _PLCCommand[13] = 0x00;
            _PLCCommand[14] = 0x0E;
            // 写入长度+4 -> Write Length +4
            _PLCCommand[15] = (byte)((4 + data.Length) / 256);
            _PLCCommand[16] = (byte)((4 + data.Length) % 256);
            // 读写指令 -> Read and write instructions
            _PLCCommand[17] = 0x05;
            // 写入数据块个数 -> Number of data blocks written
            _PLCCommand[18] = 0x01;
            // 固定，返回数据长度 -> Fixed, return data length
            _PLCCommand[19] = 0x12;
            _PLCCommand[20] = 0x0A;
            _PLCCommand[21] = 0x10;
            // 写入方式，1是按位，2是按字 -> Write mode, 1 is bitwise, 2 is by word
            _PLCCommand[22] = WordOrBit;
            // 写入数据的个数 -> Number of Write Data
            _PLCCommand[23] = (byte)(data.Length / 256);
            _PLCCommand[24] = (byte)(data.Length % 256);
            // DB块编号，如果访问的是DB块的话 -> DB block number, if you are accessing a DB block
            _PLCCommand[25] = (byte)(0);
            _PLCCommand[26] = (byte)(0);
            // 写入数据的类型 -> Types of writing data
            //0x83 代表M区 ；
            _PLCCommand[27] = (byte)dataBlock;
            // 偏移位置 -> Offset position
            _PLCCommand[28] = (byte)(0);
            _PLCCommand[29] = (byte)((addr >> 8) & 0xFF);
            _PLCCommand[30] = (byte)(addr & 0xFF);
            // 按字写入 -> Write by Word
            _PLCCommand[31] = 0x00;
            _PLCCommand[32] = 0x03;
            // 按位计算的长度 -> The length of the bitwise calculation
            _PLCCommand[33] = (byte)(data.Length / 256);
            _PLCCommand[34] = (byte)(data.Length % 256);

            data.CopyTo(_PLCCommand, 35);
            var plc = ToHexString(_PLCCommand);
            return _PLCCommand;
        }

        //直连模式下需要先调用此函数初始化PLC
        public bool InitPLC()
        {
            LockSend();
            bool bRet = true;
            byte[] Command = null;
            CommandType = SendInfoType.SIT_Init;
            if (PLCConnectType == ConnectType.CT_DirectConnect)
            {
                SetInitIndex(0);
                Command = BuildCommand();
                bRet = SendCommand(Command);
                if (bRet)
                {
                    FunctionName = "";
                    ConnIntf.DebugLog(FunctionName, "Execution succeeded.");
                }
                else
                {
                    FunctionName = "";
                    ConnIntf.DebugLog(FunctionName, "Execution failed.");
                    UnlockSend();
                    return bRet;
                }

                Thread.Sleep(200);

                SetInitIndex(1);
                Command = BuildCommand();
                bRet = SendCommand(Command);
                if (bRet)
                {
                    FunctionName = "";
                    ConnIntf.DebugLog(FunctionName, "Execution succeeded.");
                }
                else
                {
                    FunctionName = "";
                    ConnIntf.DebugLog(FunctionName, "Execution failed.");
                }
            }
            UnlockSend();
            return bRet;
        }
        public bool InitPLC(int Index)
        {
            LockSend();
            bool bRet = true;
            byte[] Command = null;
            CommandType = SendInfoType.SIT_Init;
            SetInitIndex(Index);
            if (PLCConnectType == ConnectType.CT_DirectConnect)
            {
                Command = BuildCommand();
                bRet = SendCommand(Command);
                if (bRet)
                {
                    FunctionName = "";
                    ConnIntf.DebugLog(FunctionName, "Execution succeeded.");
                }
                else
                {
                    FunctionName = "";
                    ConnIntf.DebugLog(FunctionName, "Execution failed."); 
                }
            }
            UnlockSend();
            return bRet;
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
                    Command = new byte[22] { 0x03, 0x00, 0x00, 0x16, 0x11, 0xE0, 0x00, 0x00, 0x00, 0x0E, 0x00,
                                    0xC1, 0x02, 0x01, 0x01, 0xC2, 0x02, 0x01, 0x01, 0xC0, 0x01, 0x0A };
                }
                else if (Index == 1)
                {
                    Command = new byte[25] { 0x03, 0x00, 0x00, 0x19, 0x02, 0xF0, 0x80, 0x32, 0x01, 0x00, 0x00,
                                    0xCC, 0xC1, 0x00, 0x08, 0x00, 0x00, 0xF0, 0x00, 0x00, 0x01, 0x00, 0x01, 0x03, 0xC0 };
                }
                else
                {
                    throw new Exception("Incorrect initialization times");
                }
            }
            return Command;
        }
    }
}
