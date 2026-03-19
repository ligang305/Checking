using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BGCommunication
{
    /// <summary>
    /// 
    /// </summary>
    public class CarriageIdentificationRFIDProtocol : ICommonProtocol
    {
        byte[] HeadBytes = new byte[2] { 0XFA, 0X55 };
        byte[] tail = new byte[2] { 0XAA, 0XAF };
        List<byte> WaitServerSendByteArray = new List<byte>();
        ConcurrentQueue<byte> WaitServerSendByteArrayQueue = new ConcurrentQueue<byte>();
        bool isAlive = false;
        public CarriageIdentificationRFIDProtocol()
        {
            ProtocolName = "Control station -CarriageIdentification RFID Protocol";
            isAlive = true;
            Task.Run(() =>
            {
                ConsumptionQueue();
            });
        }
        ~CarriageIdentificationRFIDProtocol()
        {
            Console.WriteLine("Structure--" + ProtocolName);
            isAlive = false;
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
                case CommandRFIDun.CarriageIdentification_GET:
                    {
                        NeedFeedBack = true;
                    }
                    break;
            }
        }

        private CommandRFIDun mFun = CommandRFIDun.CarriageIdentification_GET;
        /// <summary>
        /// 识别字符串
        /// </summary>
        public UInt32 ReadStr = 0;
        public int CurrentIndex;
        //设置命令类型
        protected CommandRFIDun CommandFunction
        {
            get { return mFun; }
            set
            {
                mFun = value;
                switch (mFun)
                {
                    case CommandRFIDun.CarriageIdentification_GET:
                        {
                            FunctionName = "Control station -CarriageIdentification protocol";
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
                case CommandRFIDun.CarriageIdentification_GET:
                    {
                        Command = InqurGetCommand();
                    }
                    break;
            }
            return Command;
        }

        internal override void OnRecv(byte[] buffer, int size)
        {
            if (NeedFeedBack && size == 11)
            {
                NeedFeedBack_ResolveBackCommand(buffer, size);

            }
            else
            {
                UnNeedFeedBack_ResolveCommand(buffer, size);
            }
        }

        //解析服务端主动发送的命令
        internal override bool ResolveBackCommand(byte[] buffer, int size)
        {
            string Action = $"【CarriageIdentification callback data returns data】 size:{size} : ";

            

            return true;
        }
        protected override bool ResolveCommand(byte[] buffer, int size)
        {
            BGLogs.Log.GetDistance().WriteInfoLogs($@"buffer:{ICommonProtocol.ToHexString(buffer)}，size:{size}");

            string Action = $"【CarriageIdentificationRFID callback data return data】 size:{size} : ";

            byte[] tempBuffer = buffer.Take(size).ToArray();
            for (int i = 0; i < tempBuffer.Length; i++)
            {
                WaitServerSendByteArrayQueue.Enqueue(tempBuffer[i]);
            }

           

            return true;
        }

        /// <summary>
        /// 将批量查询的系统命令转化为可以用的参数值
        /// </summary>
        private void ReceiveServerData(byte[] buffer)
        {
            switch (buffer[1])
            {
                case (byte)RFID_CommandType.SyncTime:
                    {
                        if (buffer.Length == 5)
                        {
                            // 校验 
                            //取前2个字节进行CRC校验
                            byte[] ValidByte = buffer.Skip(0).Take(3).ToArray();
                            ushort CrcResult = CRC16_Modbus(ValidByte, 3);
                            ushort ValidEndByte = BitConverter.ToUInt16(buffer, 3);
                            if (CrcResult == ValidEndByte)
                            {
                                byte[] ResposeBytes = new byte[15];
                                Buffer.BlockCopy(HeadBytes, 0, ResposeBytes, 0, 2);
                                Buffer.BlockCopy(tail, 0, ResposeBytes, 13, 2);
                                ResposeBytes[2] = 0x08;
                                ResposeBytes[3] = 0x30;
                                byte[] TimeBytes = new byte[7];
                                TimeBytes[0] = (Byte)(DateTime.Now.Year - 2000);
                                TimeBytes[1] = (Byte)DateTime.Now.Month;
                                TimeBytes[2] = (Byte)DateTime.Now.Day;
                                TimeBytes[3] = (Byte)DateTime.Now.Hour;
                                TimeBytes[4] = (Byte)DateTime.Now.Minute;
                                TimeBytes[5] = (Byte)DateTime.Now.Second;
                                if(DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                                {
                                    TimeBytes[6] = 0x07;
                                }
                                else
                                {
                                    TimeBytes[6] = (byte)DateTime.Now.DayOfWeek;
                                }
                                Buffer.BlockCopy(TimeBytes, 0, ResposeBytes, 4, TimeBytes.Length);
                                byte[] crcValidData = ResposeBytes.Skip(2).Take(ResposeBytes.Length - 6).ToArray();
                                ushort crcValid = CRC16_Modbus(crcValidData, (ushort)crcValidData.Length);
                                Buffer.BlockCopy(BitConverter.GetBytes(crcValid), 0, ResposeBytes, ResposeBytes.Length - 4, 2);
                                SendCommand(ResposeBytes);
                                BGLogs.Log.GetDistance().WriteInfoLogs($@"SyncTime SendBuffer:{ICommonProtocol.ToHexString(ResposeBytes)}，size:{ResposeBytes.Length}");
                            }
                        }
                    }
                    break;
                case (byte)RFID_CommandType.RFID_Record:
                    {
                        // 校验 
                        //取前2个字节进行CRC校验
                        byte[] ValidByte = buffer.Skip(0).Take(buffer.Length - 2).ToArray();
                        ushort CrcResult = CRC16_Modbus(ValidByte, (ushort)ValidByte.Length);
                        ushort ValidEndByte = BitConverter.ToUInt16(buffer, buffer.Length - 2);
                        if (CrcResult == ValidEndByte)
                        {
                            string RFID_Info = Encoding.UTF8.GetString(buffer.Skip(3).Take(buffer.Length - 5).ToArray());
                            BGLogs.Log.GetDistance().WriteInfoLogs($@"RFID_Info:{RFID_Info}");
                            if (RFID_Info.Length>=20)
                            {
                                Carriage carriage = new Carriage();
                                carriage.CarriageNo = buffer[2];
                                carriage.TrainPropety = RFID_Info[0].ToString();
                                if (carriage.TrainPropety.ToUpper() == "J" || carriage.TrainPropety.ToUpper() == "K")
                                {
                                    carriage.TrainModel = RFID_Info.Substring(1, 3);
                                    carriage.TrainNumber = RFID_Info.Substring(4, 4);
                                    carriage.Locomotive_TrainAccessorySegment = RFID_Info.Substring(8, 4);
                                    carriage.Locomotive_upplement = RFID_Info.Substring(12, 1);
                                    carriage.Locomotive_PassengerFreight = RFID_Info.Substring(13, 1);
                                    carriage.Locomotive_TrainNumber = RFID_Info.Substring(14, 5);
                                }
                                else
                                {
                                    carriage.TraineType = RFID_Info[1].ToString();
                                    carriage.TrainModel = RFID_Info.Substring(2, 5);
                                    carriage.TrainNumber = RFID_Info.Substring(7, 7);
                                    carriage.TrainChangeLength = RFID_Info.Substring(14, 2);
                                    carriage.TrainManufacturer = RFID_Info[16].ToString();
                                    carriage.TrainManufacturerYear = RFID_Info.Substring(17, 2);
                                    carriage.TrainManufacturerMonth = RFID_Info[19].ToString();
                                }

                                //TODO 向外发送通知
                                Task.Run(() => {
                                    CarriageReachEvent?.Invoke(carriage);
                                });
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 握手报文
        /// </summary>
        /// <returns></returns>
        public byte[] InqurGetCommand()
        {
            byte[] command = null;
            command = new byte[7] { 0xfe, 0x31, 0x00, 0x38, 0x04, 0x11, 0x2b };
            return command;
        }

        protected new ushort CRC16_Modbus(byte[] p, ushort length)
        {
            ushort CRCV = 0xFFFF;
            ushort iB = 0, i = 0, j = 0;
            for (i = 0; i < length; i++)
            {
                CRCV ^= p[i];
                for (j = 0; j < 8; j++)
                {
                    iB = (byte)(CRCV & 0x0001);
                    CRCV = (ushort)(CRCV >> 1);
                    if (iB == 1)
                        CRCV = (ushort)(CRCV ^ 0xA001);
                }
            }
            return CRCV;
        }


        public delegate void CarriageReach(Carriage Carriage);
        public event CarriageReach CarriageReachEvent;

        private void ConsumptionQueue()
        {
            while (isAlive)
            {
                byte ConsumptionByte;
                while (WaitServerSendByteArrayQueue.TryDequeue(out ConsumptionByte))
                {
                    WaitServerSendByteArray.Add(ConsumptionByte);
                    if (WaitServerSendByteArray.Count > HeadBytes.Length)
                    {
                        ////如果头文件
                        var reveiceHeadBytes = WaitServerSendByteArray.Take(2).ToArray();
                        for (int i = 0; i < HeadBytes.Length; i++)
                        {
                            if (reveiceHeadBytes[i] != HeadBytes[i])
                            {
                                WaitServerSendByteArray.Clear();
                            }
                        }
                    }

                    if (WaitServerSendByteArray.Count > (HeadBytes.Length + 1))
                    {
                        byte DataLength = WaitServerSendByteArray[2];
                        if (WaitServerSendByteArray.Count >= 3 + DataLength + 2 + 2)//之所以不加起来汇总，怕后人看不懂，其实就是 总数据长度大于 2个头 + 1个数据长度 + 真实数据长度 + 2个CRC校验 + 2个尾部  
                        {
                            ReceiveServerData(WaitServerSendByteArray.Skip(2).Take(Convert.ToInt32(DataLength) + 3).ToArray());
                            WaitServerSendByteArray.RemoveRange(0, 3 + Convert.ToInt32(DataLength) + 2 + 2);
                            WaitServerSendByteArray.TrimExcess();
                            BGLogs.Log.GetDistance().WriteInfoLogs($@"WaitServerSendByteArray Less:{ICommonProtocol.ToHexString(WaitServerSendByteArray.ToArray())}，size:{WaitServerSendByteArray.Count()}");
                        }
                    }
                }
                Thread.Sleep(10);
            }
        }
    }

    public enum RFID_CommandType
    {
        SyncTime = 0x30,
        RFID_Record = 0x02
    }

    public struct Carriage
    {
        /// <summary>
        /// 车厢序号 1.2.3.4.5.....N
        /// </summary>
        public int CarriageNo;
        /// <summary>
        /// 属性码：T 货车  也可能是Q
        /// </summary>
        public string TrainPropety;
        /// <summary>
        /// 车种：C
        /// </summary>
        public string TraineType;
        /// <summary>
        /// 车型：70E
        /// </summary>
        public string TrainModel;
        /// <summary>
        /// 列车号 唯一
        /// </summary>
        public string TrainNumber;
        /// <summary>
        /// 换长 只有货车车厢有
        /// </summary>
        public string TrainChangeLength;
        /// <summary>
        /// 制造厂 只有货车车厢有
        /// </summary>
        public string TrainManufacturer;
        /// <summary>
        /// 制造年 只有货车车厢有
        /// </summary>
        public string TrainManufacturerYear;
        /// <summary>
        /// 制造月 只有货车车厢有
        /// </summary>
        public string TrainManufacturerMonth;
        /// <summary>
        /// 配属段  只有机车类型有
        /// </summary>
        public string Locomotive_TrainAccessorySegment;
        /// <summary>
        /// 本补  只有机车类型有
        /// </summary>
        public string Locomotive_upplement;
        /// <summary>
        /// 客货  只有机车类型有
        /// </summary>
        public string Locomotive_PassengerFreight;
        /// <summary>
        /// 车次  只有机车类型有
        /// </summary>
        public string Locomotive_TrainNumber; 
        /// <summary>
        /// 端码  只有机车类型有
        /// </summary>
        public string Locomotive_Endcode;
        public Carriage(int _CarriageNo = 0,string _TrainPropety = "",string _TraineType = "",string _TrainModel = "",string _TrainNumber = "",string _TrainChangeLength = "",
            string _TrainManufacturer="",string _TrainManufacturerYear = "",string _TrainManufacturerMonth = "")
        {
            CarriageNo = _CarriageNo;
            TrainPropety = _TrainPropety;
            TraineType = _TraineType;
            TrainModel = _TrainModel;
            TrainNumber = _TrainNumber;
            TrainChangeLength = _TrainChangeLength;
            TrainManufacturer = _TrainManufacturer;
            TrainManufacturerYear = _TrainManufacturerYear;
            TrainManufacturerMonth = _TrainManufacturerMonth;

            Locomotive_TrainAccessorySegment = string.Empty;
            Locomotive_upplement =string.Empty;
            Locomotive_PassengerFreight = string.Empty;
            Locomotive_TrainNumber = string.Empty;
            Locomotive_Endcode = string.Empty;
        }
    }
}
