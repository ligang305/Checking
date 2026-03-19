using BGLogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGCommunication
{
    /// <summary>
    /// 泛华-Betratron 电子回旋加速器
    /// </summary>
    public class BetratronProtocolForFH : ICommonProtocol
    {
        private CommandFun mFun = CommandFun.CF_BoostingSystemWorkStatus;
        public FH_BetratronParamter FHParameter = new FH_BetratronParamter();
        protected List<UInt16> BatchSystemStat = new List<ushort>();
        public byte StartByte = 0x00;
        public byte EndByte = 0x00;
        public byte RegsLenL = 0x14;
        public byte RegsLenH = 0x00;
        public byte[] WatiByteArray = null;
        int HasRecevice = 0;

        public byte[] WatiSetCommandByteArray = null;
        int HasSetCommandRecevice = 0;
        public BetratronProtocolForFH()
        {
            ProtocolName = "Control Station -BetratronProtocol Accelerator Protocol";
            FHParameter.dr = new DoseRate();
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
                case CommandFun.CF_BoostingSystemWorkStatus:
                    {
                        NeedFeedBack = true;
                    }
                    break;
                case CommandFun.CF_DACOfContractor:
                    {
                        NeedFeedBack = true;
                    }
                    break;
                case CommandFun.CF_InqurDose:
                    {
                        //mDR.InternalDose = 0;
                        //mDR.RemoteDose = 0;
                        //mDR.CircuitMaxEnergy = 0;
                        //mDR.Doserate = 0;
                    }
                    break;
                case CommandFun.CF_StopBeam:
                    {
                        NeedFeedBack = true;
                    }
                    break;
                case CommandFun.CF_WarmUp:
                    {
                        NeedFeedBack = false;
                    }
                    break;
                case CommandFun.CF_SetI:
                    {
                        NeedFeedBack = false;
                    }
                    break;
                case CommandFun.CF_OutputBeam:
                    {
                        NeedFeedBack = false;
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
                    case CommandFun.CF_BoostingSystemWorkStatus:
                        {
                            FunctionName = "Working state of accelerator system";
                        }
                        break;
                    case CommandFun.CF_DACOfContractor:
                        {
                            FunctionName = "Constrainer DAC value";
                        }
                        break;
                    case CommandFun.CF_DACOfFilament:
                        {
                            FunctionName = "DAC value of filament";
                        }
                        break;
                    case CommandFun.CF_DACOfInjector:
                        {
                            FunctionName = "Injection DAC value";
                        }
                        break;
                    case CommandFun.CF_Radiator:
                        {
                            FunctionName = "Radiator temperature";
                        }
                        break;
                    case CommandFun.CF_PulseConverter:
                        {
                            FunctionName = "Pulse converter temperature";
                        }
                        break;
                    case CommandFun.CF_Thyristor:
                        {
                            FunctionName = "Temperature thyristor";
                        }
                        break;
                    case CommandFun.CF_IGBTTransistors:
                        {
                            FunctionName = "IGBT temperature";
                        }
                        break;
                    case CommandFun.CF_InjectionCurrent:
                        {
                            FunctionName = "Injection current";
                        }
                        break;
                    case CommandFun.CF_Dose_Internal:
                        {
                            FunctionName = "Internal current";
                        }
                        break;
                    case CommandFun.CF_ExposureTime:
                        {
                            FunctionName = "time of exposure";
                        }
                        break;
                    case CommandFun.CF_DoseRate:
                        {
                            FunctionName = "dosage rate";
                        }
                        break;
                    case CommandFun.CF_MainMarnetVol:
                        {
                            FunctionName = "Main magnetic voltage";
                        }
                        break;
                    case CommandFun.CF_SystemStatus:
                        {
                            FunctionName = "Read system status parameters";
                        }
                        break;
                }
               //ConnIntf.DebugStepLog("begin" + FunctionName);
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
                case CommandFun.CF_InqurStatus:
                case CommandFun.CF_DoseRate:
                case CommandFun.CF_ExposureTime:
                case CommandFun.CF_Dose_Internal:
                case CommandFun.CF_InjectionCurrent:
                case CommandFun.CF_IGBTTransistors:
                case CommandFun.CF_Thyristor:
                case CommandFun.CF_PulseConverter:
                case CommandFun.CF_Radiator:
                case CommandFun.CF_DACOfFilament:
                case CommandFun.CF_DACOfInjector:
                case CommandFun.CF_DACOfContractor:
                case CommandFun.CF_RSTProcess:
                case CommandFun.CF_BoostingSystemWorkStatus:
                    {
                        Command = InquireCommand(CommandFunction);
                    }
                    break;
                case CommandFun.CF_SystemStatus:
                    {
                        Command = InquireSystemCommand(StartByte,EndByte,RegsLenH,RegsLenL);
                        //ConnIntf.WriteDebug("CF_SystemStatus Request :", ICommonProtocol.ToHexString(Command), true);
                    }
                    break;
            }
            return Command;
        }

        //解析发送命令后的回传信息
        internal override bool ResolveBackCommand(byte[] buffer, int size)
        {
            string Action = "【" + FunctionName + "】Return data";
            Log.GetDistance().WriteErrorLogs($"Receiving serial port information successfully: the byte length is：{size}，Data:{ICommonProtocol.ToHexString(buffer)}");
            switch (CommandFunction)
            {
                //这里校验可能有问题
                case CommandFun.CF_DoseRate:
                    {
                        if (size != 7)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal data length");
                        }
                        if (buffer[0] != 0x01 || buffer[2] != 0x03 || buffer[3] != 0x02)//0x0D
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal data");
                        }
                        byte[] PreByteArray = buffer.Take(5).ToArray();
                        byte[] CrcByte = buffer.Skip(5).Take(2).ToArray();
                        ushort CrcResult = CRC16_Modbus(PreByteArray, 5);
                        ushort ValidEndByte = BitConverter.ToUInt16(CrcByte, 2);
                        if (CrcResult != ValidEndByte)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal status data check");
                        }
                        FHParameter.dr.Doserate =  BitConverter.ToUInt16(buffer.Skip(3).Take(2).ToArray(),0);
                        ExeResult = true;
                    }
                    break;
                case CommandFun.CF_ExposureTime:
                    {
                        if (size != 7)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal data length");
                        }
                        if (buffer[0] != 0x01 || buffer[2] != 0x03 || buffer[3] != 0x02)//0x0D
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal data");
                        }
                        byte[] PreByteArray = buffer.Take(5).ToArray();
                        byte[] CrcByte = buffer.Skip(5).Take(2).ToArray();
                        ushort CrcResult = CRC16_Modbus(PreByteArray, 5);
                        ushort ValidEndByte = BitConverter.ToUInt16(CrcByte, 2);
                        if (CrcResult != ValidEndByte)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal status data check");
                        }
                        FHParameter.ExposureTime = BitConverter.ToUInt16(buffer.Skip(3).Take(2).ToArray(), 0);
                        ExeResult = true;
                    }
                    break;
                case CommandFun.CF_Dose_Internal:
                    {
                        if (size != 7)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal data length");
                        }
                        if (buffer[0] != 0x01 || buffer[2] != 0x03 || buffer[3] != 0x02)//0x0D
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal data");
                        }
                        byte[] PreByteArray = buffer.Take(5).ToArray();
                        byte[] CrcByte = buffer.Skip(5).Take(2).ToArray();
                        ushort CrcResult = CRC16_Modbus(PreByteArray, 5);
                        ushort ValidEndByte = BitConverter.ToUInt16(CrcByte, 2);
                        if (CrcResult != ValidEndByte)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal status data check");
                        }
                        FHParameter.dr.InternalDose = BitConverter.ToUInt16(buffer.Skip(3).Take(2).ToArray(), 0);
                        ExeResult = true;
                    }
                    break;
                case CommandFun.CF_InjectionCurrent:
                    {
                        if (size != 7)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal data length");
                        }
                        if (buffer[0] != 0x01 || buffer[2] != 0x03 || buffer[3] != 0x02)//0x0D
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal data");
                        }
                        byte[] PreByteArray = buffer.Take(5).ToArray();
                        byte[] CrcByte = buffer.Skip(5).Take(2).ToArray();
                        ushort CrcResult = CRC16_Modbus(PreByteArray, 5);
                        ushort ValidEndByte = BitConverter.ToUInt16(CrcByte, 2);
                        if (CrcResult != ValidEndByte)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal status data check");
                        }
                        FHParameter.InjecttionCurrent = BitConverter.ToUInt16(buffer.Skip(3).Take(2).ToArray(), 0);
                        ExeResult = true;
                    }
                    break;
                case CommandFun.CF_IGBTTransistors:
                    {
                        if (size != 7)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal data length");
                        }
                        if (buffer[0] != 0x01 || buffer[2] != 0x03 || buffer[3] != 0x02)//0x0D
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal data");
                        }
                        byte[] PreByteArray = buffer.Take(5).ToArray();
                        byte[] CrcByte = buffer.Skip(5).Take(2).ToArray();
                        ushort CrcResult = CRC16_Modbus(PreByteArray, 5);
                        ushort ValidEndByte = BitConverter.ToUInt16(CrcByte, 2);
                        if (CrcResult != ValidEndByte)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal status data check");
                        }
                        FHParameter.IGBTTransistors = BitConverter.ToUInt16(buffer.Skip(3).Take(2).ToArray(), 0);
                        ExeResult = true;
                    }
                    break;
                case CommandFun.CF_Thyristor:
                    {
                        if (size != 7)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal data length");
                        }
                        if (buffer[0] != 0x01 || buffer[2] != 0x03 || buffer[3] != 0x02)//0x0D
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal data");
                        }
                        byte[] PreByteArray = buffer.Take(5).ToArray();
                        byte[] CrcByte = buffer.Skip(5).Take(2).ToArray();
                        ushort CrcResult = CRC16_Modbus(PreByteArray, 5);
                        ushort ValidEndByte = BitConverter.ToUInt16(CrcByte, 2);
                        if (CrcResult != ValidEndByte)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal status data check");
                        }
                        FHParameter.Thyristor = BitConverter.ToUInt16(buffer.Skip(3).Take(2).ToArray(), 0);
                        ExeResult = true;
                    }
                    break;
                case CommandFun.CF_PulseConverter:
                    {
                        if (size != 7)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal data length");
                        }
                        if (buffer[0] != 0x01 || buffer[2] != 0x03 || buffer[3] != 0x02)//0x0D
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal data");
                        }
                        byte[] PreByteArray = buffer.Take(5).ToArray();
                        byte[] CrcByte = buffer.Skip(5).Take(2).ToArray();
                        ushort CrcResult = CRC16_Modbus(PreByteArray, 5);
                        ushort ValidEndByte = BitConverter.ToUInt16(CrcByte, 2);
                        if (CrcResult != ValidEndByte)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal status data check");
                        }
                        FHParameter.PulseConverter = BitConverter.ToUInt16(buffer.Skip(3).Take(2).ToArray(), 0);
                        ExeResult = true;
                    }
                    break;
                case CommandFun.CF_Radiator:
                    {
                        if (size != 7)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal data length");
                        }
                        if (buffer[0] != 0x01 || buffer[2] != 0x03 || buffer[3] != 0x02)//0x0D
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal data");
                        }
                        byte[] PreByteArray = buffer.Take(5).ToArray();
                        byte[] CrcByte = buffer.Skip(5).Take(2).ToArray();
                        ushort CrcResult = CRC16_Modbus(PreByteArray, 5);
                        ushort ValidEndByte = BitConverter.ToUInt16(CrcByte, 2);
                        if (CrcResult != ValidEndByte)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal status data check");
                        }
                        FHParameter.Radiator = BitConverter.ToUInt16(buffer.Skip(3).Take(2).ToArray(), 0);
                        ExeResult = true;
                    }
                    break;
                case CommandFun.CF_DACOfFilament:
                    {
                        if (size != 7)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal data length");
                        }
                        if (buffer[0] != 0x01 || buffer[2] != 0x03 || buffer[3] != 0x02)//0x0D
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal data");
                        }
                        byte[] PreByteArray = buffer.Take(5).ToArray();
                        byte[] CrcByte = buffer.Skip(5).Take(2).ToArray();
                        ushort CrcResult = CRC16_Modbus(PreByteArray, 5);
                        ushort ValidEndByte = BitConverter.ToUInt16(CrcByte, 2);
                        if (CrcResult != ValidEndByte)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal status data check");
                        }
                        FHParameter.DACOfFilament = BitConverter.ToUInt16(buffer.Skip(3).Take(2).ToArray(), 0);
                        ExeResult = true;
                    }
                    break;
                case CommandFun.CF_DACOfInjector:
                    {
                        if (size != 7)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal data length");
                        }
                        if (buffer[0] != 0x01 || buffer[2] != 0x03 || buffer[3] != 0x02)//0x0D
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal data");
                        }
                        byte[] PreByteArray = buffer.Take(5).ToArray();
                        byte[] CrcByte = buffer.Skip(5).Take(2).ToArray();
                        ushort CrcResult = CRC16_Modbus(PreByteArray, 5);
                        ushort ValidEndByte = BitConverter.ToUInt16(CrcByte, 2);
                        if (CrcResult != ValidEndByte)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal status data check");
                        }
                        FHParameter.DACOfInjector = BitConverter.ToUInt16(buffer.Skip(3).Take(2).ToArray(), 0);
                        ExeResult = true;
                    }
                    break;
                case CommandFun.CF_DACOfContractor:
                    {
                        if (size != 7)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal data length");
                        }
                        if (buffer[0] != 0x01 || buffer[2] != 0x03 || buffer[3] != 0x02)//0x0D
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal data");
                        }
                        byte[] PreByteArray = buffer.Take(5).ToArray();
                        byte[] CrcByte = buffer.Skip(5).Take(2).ToArray();
                        ushort CrcResult = CRC16_Modbus(PreByteArray, 5);
                        ushort ValidEndByte = BitConverter.ToUInt16(CrcByte, 2);
                        if (CrcResult != ValidEndByte)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal status data check");
                        }
                        FHParameter.DACOfContractor = BitConverter.ToUInt16(buffer.Skip(3).Take(2).ToArray(), 0);
                        ExeResult = true;
                    }
                    break;
                case CommandFun.CF_RSTProcess:
                    {
                        if (size != 7)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal data length");
                        }
                        if (buffer[0] != 0x01 || buffer[2] != 0x03 || buffer[3] != 0x02)//0x0D
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal data");
                        }
                        byte[] PreByteArray = buffer.Take(5).ToArray();
                        byte[] CrcByte = buffer.Skip(5).Take(2).ToArray();
                        ushort CrcResult = CRC16_Modbus(PreByteArray, 5);
                        ushort ValidEndByte = BitConverter.ToUInt16(CrcByte, 2);
                        if (CrcResult != ValidEndByte)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal status data check");
                        }
                        FHParameter.RSTProcess = BitConverter.ToUInt16(buffer.Skip(3).Take(2).ToArray(), 0);
                        ExeResult = true;
                    }
                    break;
                case CommandFun.CF_BoostingSystemWorkStatus:
                    {
                        if (size != 7)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal data length");
                        }
                        if (buffer[0] != 0x01 || buffer[2] != 0x03 || buffer[3] != 0x02)//0x0D
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal data");
                        }
                        byte[] PreByteArray = buffer.Take(5).ToArray();
                        byte[] CrcByte = buffer.Skip(5).Take(2).ToArray();
                        ushort CrcResult = CRC16_Modbus(PreByteArray, 5);
                        ushort ValidEndByte = BitConverter.ToUInt16(CrcByte, 2);
                        if (CrcResult != ValidEndByte)
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal status data check");
                        }
                        FHParameter.BoostingSystemWorkStatus = (WorkingJob)BitConverter.ToUInt16(buffer.Skip(3).Take(2).ToArray(),0);
                        ExeResult = true;
                    }
                    break;
                //返回： 0x01,0x04, RegsLen*2,[SysStatPara1H, SysStatPara1L],…,[SysStatParanH, SysStatParanL],[CRC1, CRC2]
                case CommandFun.CF_SystemStatus:
                    {
                        ConnIntf.WriteLog($@"CF_SystemStatus repose : ",ICommonProtocol.ToHexString(buffer));
                        if (buffer.Length != 45)
                        {
                            if(WatiByteArray == null)
                            {
                                WatiByteArray = new byte[45];
                            }
                            if(HasRecevice + buffer.Length > 45)
                            {
                                var tempBytes = buffer.Take(45 - HasRecevice).ToArray();
                                tempBytes.CopyTo(WatiByteArray, HasRecevice);
                                HasRecevice = 45;
                            }
                            else
                            {
                                buffer.CopyTo(WatiByteArray, HasRecevice);
                                HasRecevice += buffer.Length;
                            }
                        }
                        else
                        {
                            HasRecevice = 45;
                            WatiByteArray = buffer;
                        }
                        //ConnIntf.WriteError("CF_SystemStatus WatiByteArray repose :", ICommonProtocol.ToHexString(WatiByteArray), true);
                        if (HasRecevice == 45)
                        {
                            buffer = WatiByteArray;
                        }
                        else
                        {
                            return true;
                        }

                        if (buffer[0] != 0x01 || buffer[1] != 0x04)//0x0D
                        {
                            HasRecevice = 0;
                            WatiByteArray = null;
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal data");
                        }
                        else
                        {
                            HasRecevice = 0;
                            WatiByteArray = null;
                        }
                        ushort RegsLen = buffer[2];
                        byte[] CrcValidByteArray = buffer.Take(buffer.Length - 2).ToArray();
                        byte[] SysStaParaArray = buffer.Skip(3).Take(buffer.Length - 5).ToArray();
                        byte[] CrcByte = buffer.Skip(buffer.Length - 2).Take(2).ToArray();
                        ushort CrcResult = CRC16_Modbus(CrcValidByteArray, (ushort)CrcValidByteArray.Length);
                        ushort ValidEndByte = BitConverter.ToUInt16(CrcByte, 0);
                        //if (CrcResult != ValidEndByte)
                        //{
                        //    //throw new Exception(ProtocolName + "-" + Action + "-状态数据校验非法");
                        //    //ConnIntf.WriteError("CF_SystemStatus CrcResult != ValidEndByte repose :", ICommonProtocol.ToHexString(buffer), true);
                        //    return false;
                        //}
                        //if (RegsLen != SysStaParaArray.Length)
                        //{
                        //    //throw new Exception(ProtocolName + "-" + Action + "-状态数据校验非法");
                        //    //ConnIntf.WriteError("CF_SystemStatus RegsLen != SysStaParaArray repose :", ICommonProtocol.ToHexString(buffer), true);
                        //    return false;
                        //}
                        BatchSystemStat.Clear();
                        for (int i = 0; i < SysStaParaArray.Length - 5; i = i+2)
                        {

                            BatchSystemStat.Add(BitConverter.ToUInt16(SysStaParaArray.Skip(i).Take(2).ToArray().Reverse().ToArray(),0));
                        }

                        SetParamaterValueTob();
                        ExeResult = true;
                        break;
                    }
                case CommandFun.CF_SetInjectionCurrent:
                case CommandFun.CF_SetMainMarnetWorkFre:
                case CommandFun.CF_SetMaxRayDoseFind:
                case CommandFun.CF_SetRayEndingWorkStatus:
                case CommandFun.CF_SetStartRaying:
                case CommandFun.CF_SetStopRaying:
                case CommandFun.CF_SetWaitMachineStopMode:
                case CommandFun.CF_SetDoseInternal:
                case CommandFun.CF_SetExposureTime: 
                case CommandFun.CF_SetHighEnergy:
                case CommandFun.CF_SetLowEnergy:
                case CommandFun.CF_SetLowPules:
                case CommandFun.CF_SetHighPulsees:
                case CommandFun.CF_SetEnergy:
                    {
                        if (buffer.Length != 8)
                        {
                            if (WatiSetCommandByteArray == null)
                            {
                                WatiSetCommandByteArray = new byte[8];
                            }
                            if (HasSetCommandRecevice + buffer.Length > 8)
                            {
                                var tempBytes = buffer.Take(8 - HasSetCommandRecevice).ToArray();
                                tempBytes.CopyTo(WatiSetCommandByteArray, HasSetCommandRecevice);
                                HasSetCommandRecevice = 8;
                            }
                            else
                            {
                                buffer.CopyTo(WatiSetCommandByteArray, HasSetCommandRecevice);
                                HasSetCommandRecevice += buffer.Length;
                            }
                        }
                        else
                        {
                            HasSetCommandRecevice = 8;
                            WatiSetCommandByteArray = buffer;
                        }
                        ConnIntf.WriteError("CF_SetWaitMachineStopMode WatiByteArray repose :", ICommonProtocol.ToHexString(WatiSetCommandByteArray), true);
                        if (HasSetCommandRecevice == 8)
                        {
                            buffer = WatiSetCommandByteArray;
                        }
                        else
                        {
                            return true;
                        }

                        if (buffer[0] != 0x01 || buffer[1] != 0x06)//0x0D
                        {
                            HasSetCommandRecevice = 0;
                            WatiSetCommandByteArray = null;
                            throw new Exception(ProtocolName + "-" + Action + "-Illegal data");
                        }
                        else
                        {
                            HasSetCommandRecevice = 0;
                            WatiSetCommandByteArray = null;
                        }

                        ////if (buffer.Length!= SetCommand.Length)
                        ////{
                        ////    throw new Exception(ProtocolName + "-" + Action + "-Illegal data");
                        ////}
                        ////for (int i = 0; i < buffer.Length; i++)
                        ////{
                        ////    if(buffer[i] != SetCommand[i])
                        ////    {
                        ////        throw new Exception(ProtocolName + "-" + Action + "-Illegal data");
                        ////    }
                        ////}
                        ExeResult = true;
                        break;
                    } 
            }

            NeedFeedBack = false;
            return true;
        }
        /// <summary>
        /// 通过查询类型构造不同的报文
        /// </summary>
        /// <param name="InquireType"></param>
        /// <returns></returns>
        public byte[] InquireCommand(CommandFun InquireType)
        {
            byte[] Command = new byte[8];
            byte[] CommandByte = new byte[6] { 0x01, 0x03, 0x00, 0x00, 0x00, 0x01 };
            switch (InquireType)
            {
                case CommandFun.CF_DoseRate:
                    CommandByte[3] = 0x00;
                    break;
                case CommandFun.CF_ExposureTime:
                    CommandByte[3] = 0x01;
                    break;
                case CommandFun.CF_Dose_Internal:
                    CommandByte[3] = 0x02;
                    break;
                case CommandFun.CF_InjectionCurrent:
                    CommandByte[3] = 0x03;
                    break;
                case CommandFun.CF_IGBTTransistors:
                    CommandByte[3] = 0x04;
                    break;
                case CommandFun.CF_Thyristor:
                    CommandByte[3] = 0x05;
                    break;
                case CommandFun.CF_PulseConverter:
                    CommandByte[3] = 0x06;
                    break;
                case CommandFun.CF_Radiator:
                    CommandByte[3] = 0x07;
                    break;
                case CommandFun.CF_DACOfFilament:
                    CommandByte[3] = 0x08;
                    break;
                case CommandFun.CF_DACOfInjector:
                    CommandByte[3] = 0x09;
                    break;
                case CommandFun.CF_DACOfContractor:
                    CommandByte[3] = 0x0a;
                    break;
                case CommandFun.CF_RSTProcess:
                    CommandByte[3] = 0x0b;
                    break;
                case CommandFun.CF_BoostingSystemWorkStatus:
                    CommandByte[3] = 0x0c;
                    break;
                case CommandFun.CF_MainMarnetVol:
                    CommandByte[3] = 0x0d;
                    break;
                default:
                    break;
            }
            byte[] CrcValid = BitConverter.GetBytes(CRC16_Modbus(CommandByte, 6));
            CommandByte.CopyTo(Command, 0);
            CrcValid.CopyTo(Command, CommandByte.Length);
            return CommandByte;
        }
        /// <summary>
        /// 批量查询泛华命令
        /// </summary>
        /// <returns></returns>
        public byte[] InquireSystemCommand(byte StartByte,byte EndByte,byte RegsLenH,byte RegsLenL)
        {
            byte[] Command = new byte[8];
            byte[] CommandByte = new byte[6] { 0x01, 0x04, StartByte, EndByte, RegsLenH, RegsLenL };
            byte[] CrcValid = BitConverter.GetBytes(CRC16_Modbus(CommandByte, 6));
            CommandByte.CopyTo(Command, 0);
            CrcValid.CopyTo(Command, CommandByte.Length);
            return Command;
        }
        /// <summary>
        /// 将批量查询的系统命令转化为可以用的参数值
        /// </summary>
        private void SetParamaterValueTob()
        {
            FHParameter.RSTProcess = BatchSystemStat[0];
            FHParameter.dr.Doserate = BatchSystemStat[1];
            FHParameter.ExposureTime = BatchSystemStat[2];
            FHParameter.dr.InternalDose = BatchSystemStat[3];
            FHParameter.InjecttionCurrent = BatchSystemStat[4];
            FHParameter.IGBTTransistors = BatchSystemStat[5];
            FHParameter.Thyristor = BatchSystemStat[6];
            FHParameter.PulseConverter = BatchSystemStat[7];
            FHParameter.Radiator = BatchSystemStat[8];
            FHParameter.DACOfFilament = BatchSystemStat[9];
            FHParameter.DACOfInjector = BatchSystemStat[10];
            FHParameter.DACOfContractor = BatchSystemStat[11];
            FHParameter.BoostingSystemWorkStatus = (WorkingJob)BatchSystemStat[12];
            FHParameter.FeedbackVal = BatchSystemStat[13];
            FHParameter.FeedbackValCoeff = BatchSystemStat[14];
        }

        #region 泛华Betratron查询和设置指令
 
        public bool InqureDoseRate(CommandFun commandFun)
        {
            LockSend();
            CommandFunction = commandFun;
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
            //UnlockSend();
            return bRet;
        }
        #endregion

        #region 泛华Betratron 发送指令
        byte[] SetCommand = new byte[8] { 0x01, 0x06, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00 };

        public byte[] BuildSetCommand(byte[] ValueByte)
        {
            byte[] CommandByt = ValueByte.Skip(0).Take(ValueByte.Length -2).ToArray();
            byte[] CrcValid = BitConverter.GetBytes(CRC16_Modbus(CommandByt, (ushort)CommandByt.Length));
            CrcValid.CopyTo(ValueByte, ValueByte.Length-2);
            return ValueByte;
        }

        /// <summary>
        /// 设置曝光时间(s)
        /// </summary>
        /// <returns></returns>
        public bool SendCommandExposureTime(ushort ExposureTime)
        {
            LockSend();
            CommandFunction = CommandFun.CF_SetExposureTime;
            SetCommand[3] = 0x01;
            byte[] setTime = BitConverter.GetBytes(ExposureTime).Reverse().ToArray();
            setTime.CopyTo(SetCommand,4);
            SetCommand = BuildSetCommand(SetCommand);
            return SendCommand(SetCommand);

        }
        /// <summary>
        /// 设置内部剂量(R)
        /// </summary>
        /// <returns></returns>
        public bool SendCommandDoseInternal(ushort dose)
        {
            LockSend();
            CommandFunction = CommandFun.CF_SetDoseInternal;
            SetCommand[3] = 0x02;
            byte[] setdose = BitConverter.GetBytes(dose).Reverse().ToArray();
            setdose.CopyTo(SetCommand, 4);
            SetCommand = BuildSetCommand(SetCommand);
            return SendCommand(SetCommand);
        }
        /// <summary>
        /// 设置注入电流
        /// </summary>
        /// <returns></returns>
        public bool SendCommandInjectionCurrent(ushort inject)
        {
            LockSend();
            CommandFunction = CommandFun.CF_SetInjectionCurrent;
            SetCommand[3] = 0x03;
            byte[] setinject = BitConverter.GetBytes(inject).Reverse().ToArray();
            setinject.CopyTo(SetCommand, 4);
            SetCommand = BuildSetCommand(SetCommand);
            return SendCommand(SetCommand);
        }
        /// <summary>
        /// 设置能量模式
        /// </summary>
        /// <returns></returns>
        public bool SendCommandEnergyMode(ushort EnergyMode)
        {
            LockSend();
            CommandFunction = CommandFun.CF_SetEnergy;
            SetCommand[3] = 0x04;
            byte[] setEnergyMode = BitConverter.GetBytes(EnergyMode).Reverse().ToArray();
            setEnergyMode.CopyTo(SetCommand, 4);
            SetCommand = BuildSetCommand(SetCommand);
            SetCommand[6] = 0x00;
            SetCommand[7] = 0x00;
            return SendCommand(SetCommand);
        }
        /// <summary>
        /// 设置高能量
        /// </summary>
        /// <returns></returns>
        public bool SendCommandHighEnergy(ushort Mev)
        {
            LockSend();
            CommandFunction = CommandFun.CF_SetHighEnergy;
            SetCommand[3] = 0x05;
            byte[] setPulses = BitConverter.GetBytes(Mev).Reverse().ToArray();
            setPulses.CopyTo(SetCommand, 4);
            SetCommand = BuildSetCommand(SetCommand);
            return SendCommand(SetCommand);
        }
        /// <summary>
        /// 设置高能脉冲个数
        /// </summary>
        /// <returns></returns>
        public bool SendCommandHighPulses(ushort pules)
        {
            LockSend();
            CommandFunction = CommandFun.CF_SetHighPulsees;
            SetCommand[3] = 0x06;
            byte[] setPulses = BitConverter.GetBytes(pules).Reverse().ToArray();
            setPulses.CopyTo(SetCommand, 4);
            SetCommand = BuildSetCommand(SetCommand);
            return SendCommand(SetCommand);
        }
        /// <summary>
        /// 设置低能
        /// </summary>
        /// <returns></returns>
        public bool SendCommandLowEnergy(ushort Energy)
        {
            LockSend();
            CommandFunction = CommandFun.CF_SetLowEnergy;
            SetCommand[3] = 0x07;
            byte[] setPulses = BitConverter.GetBytes(Energy).Reverse().ToArray();
            setPulses.CopyTo(SetCommand, 4);
            SetCommand = BuildSetCommand(SetCommand);
            return SendCommand(SetCommand);
        }
        /// <summary>
        /// 设置低能脉冲个数
        /// </summary>
        /// <returns></returns>
        public bool SendCommandLowPulsees(ushort Pulsees)
        {
            LockSend();
            CommandFunction = CommandFun.CF_SetLowPules;
            SetCommand[3] = 0x08;
            byte[] setPulsees = BitConverter.GetBytes(Pulsees).Reverse().ToArray();
            setPulsees.CopyTo(SetCommand, 4);
            SetCommand = BuildSetCommand(SetCommand);
            return SendCommand(SetCommand);
        }
        /// <summary>
        /// 启动辐射
        /// </summary>
        /// <returns></returns>
        public bool SendCommandRadiationOn()
        {
            LockSend();
            CommandFunction = CommandFun.CF_SetStartRaying;
            SetCommand[3] = 0x09;
            byte[] setRadiationOn = BitConverter.GetBytes(0).Reverse().ToArray();
            setRadiationOn.CopyTo(SetCommand, 4);
            SetCommand = BuildSetCommand(SetCommand);
            return SendCommand(SetCommand);
        }
        /// <summary>
        /// 停止辐射
        /// </summary>
        /// <returns></returns>
        public bool SendCommandRadiationOff()
        {
            LockSend();
            CommandFunction = CommandFun.CF_SetStopRaying;
            Debug.WriteLine("------------------------------------------------------------------gang修改0x0a----------------------------------------------------------");

            SetCommand[3] = 0x0a;
            //SetCommand[3] = 0x0c;
            byte[] setRadiationOff = BitConverter.GetBytes(0).Reverse().ToArray();
            setRadiationOff.CopyTo(SetCommand, 4);
            SetCommand = BuildSetCommand(SetCommand);
            return SendCommand(SetCommand);
        }
        /// <summary>
        /// 进行最大剂量率搜寻
        /// </summary>
        /// <returns></returns>
        public bool SendCommandSearchMaxDr()
        {
            LockSend();
            CommandFunction = CommandFun.CF_SetMaxRayDoseFind;
            SetCommand[3] = 0x0b;
            byte[] setSerchMaxDr = BitConverter.GetBytes(0).Reverse().ToArray();
            setSerchMaxDr.CopyTo(SetCommand, 4);
            SetCommand = BuildSetCommand(SetCommand);
            return SendCommand(SetCommand);
        }
        /// <summary>
        /// 进入待机预热暂停模式
        /// </summary>
        /// <returns></returns>
        public bool SendCommandWaitWorkStop()
        {
            LockSend();
            CommandFunction = CommandFun.CF_SetWaitMachineStopMode;
            SetCommand[3] = 0x0c;
            byte[] setWorkStop = BitConverter.GetBytes(0).Reverse().ToArray();
            setWorkStop.CopyTo(SetCommand, 4);
            SetCommand = BuildSetCommand(SetCommand);
            return SendCommand(SetCommand);
        }

        /// <summary>
        /// 设置辐射完成后工作状态
        /// </summary>
        /// <param name="StopMode">暂停模式 0:50hz 无辐射，1：0HZ 无辐射</param>
        /// <returns></returns>
        public bool SendCommandAfterRayWorkStatus(UInt16 StopMode)
        {
            LockSend();
            CommandFunction = CommandFun.CF_SetRayEndingWorkStatus;
            if (StopMode != 1 || StopMode != 0) return false;
            SetCommand[3] = 0x0d;
            byte[] SetStopMode = BitConverter.GetBytes(StopMode).Reverse().ToArray();
            SetStopMode.CopyTo(SetCommand,4);
            SetCommand = BuildSetCommand(SetCommand);
            return SendCommand(SetCommand);
        }
        /// <summary>
        /// 设置主磁工作频率模式(0为300Hz,1为200Hz,2为100Hz)
        /// </summary>
        /// <returns></returns>
        public bool SendCommandMainMagWorkFreMode(ushort WorkMode)
        {
            LockSend();
            CommandFunction = CommandFun.CF_SetMainMarnetWorkFre;
            if (WorkMode == 1 || WorkMode == 0 || WorkMode ==2) 
            {
                SetCommand[3] = 0x14;
                byte[] SetWorkMode = BitConverter.GetBytes(WorkMode).Reverse().ToArray();
                SetWorkMode.CopyTo(SetCommand, 4);
                SetCommand = BuildSetCommand(SetCommand);
                return SendCommand(SetCommand);
            }
            else
            {
                UnlockSend();
                return false;
            }
           
        }

        public bool SendCommand(byte[] Command)
        {
            bool bRet = base.SendCommand(Command);
            if (bRet)
            {
                Debug.WriteLine("------------------------------------------------------------------gang数据发送成功--------------------" + Command[0] + " " + Command[1] + " " + Command[2] + " " + Command[3] + " " + Command[4] + " " + Command[5] + " " + Command[6] + " " + Command[7] + " ");
                ConnIntf.DebugLog(FunctionName, "Execution succeeded.");
            }
            else
            {
                Debug.WriteLine("------------------------------------------------------------------gang数据发送失败--------------------" + Command[0] + " " + Command[1] + " " + Command[2] + " " + Command[3] + " " + Command[4] + " " + Command[5] + " " + Command[6] + " " + Command[7] + " ");
                ConnIntf.DebugLog(FunctionName, "Execution failed.");
            }
            FunctionName = "";
            UnlockSend();
            return bRet;
        }


        #endregion


    }
}
