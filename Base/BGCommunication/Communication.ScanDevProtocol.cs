using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BGCommunication
{
    public enum ScanDeviceType
    {
        SDT_0PLC = 0x00, //PLC
        SDT_1RaysDev = 0x01, //射线装置
        SDT_2TrailCar = 0x02, //拖车
        SDT_3Detector = 0x03, //探测器
        SDT_4PlateRcgn = 0x04, //车牌识别
        SDT_5ChassisScan = 0x05, //底盘扫描
        SDT_6LED = 0x06, //LED大屏
        SDT_7BarricadeLever = 0x07, //道闸拉杆
        SDT_8FloorScale = 0x08, //地磅
        SDT_100Control = 0x64, //控制指令
    }

    public enum ScanDevOpcode
    {
        SDO_0Status = 0x00,
        SDO_1Error = 0x01,
        SDO_2Custom = 0x02,
        SDO_3Custom = 0x03,
        SDO_4Custom = 0x04,
        SDO_10Custom = 0x10,
        SDO_11Custom = 0x11,
        SDO_12Custom = 0x12,
    }

    public enum DeviceStatus
    {
        //未连接
        DS_Unconnect = 0,
        //连接中
        DS_Connecting = 1,
        //已连接
        DS_Connected = 2,
        //已就绪
        DS_Ready = 3
    }
   
    public class ScanDevice
    {
        internal ScanDevProtocol sdp = null;
        public ScanDevice(ScanDeviceType type,ScanDevProtocol protocol)
        {
            sdp = protocol;
            sdt = type;
        }

        //设置操作码和命令内容
        public virtual void RecieveCommand(byte opcode, byte[] content)
        {
            sdp.LockSend();
            SetOpcode(opcode);
            SetCommandData(content);
            sdp.UnlockSend();
        }

        //设置操作码
        internal virtual void SetOpcode(byte opcode)
        {
            OpcodeMeaning(opcode);
            sdp.Opcode = opcode;
        }

        //设置命令内容
        internal virtual void SetCommandData(byte[] content)
        {
            CommandDataMeaning(content);
            sdp.CommandData = content;
        }

        //操作码含义
        internal virtual string OpcodeMeaning(byte opcode)
        {
            string sMeaning = "";
            ScanDevOpcode sdo = (ScanDevOpcode)opcode;
            switch (sdo)
            {
                case ScanDevOpcode.SDO_0Status:
                    {
                        sMeaning = "device status";
                    }
                    break;
                case ScanDevOpcode.SDO_1Error:
                    {
                        sMeaning = "breakdown";
                    }
                    break;
                default:
                    {
                        throw new Exception("The device type does not have this opcode.");
                    }
            }
            return sMeaning;
        }

        //命令内容含义
        internal virtual string CommandDataMeaning(byte[] content)
        {
            string Meaning = "";
            switch (sdt)
            {
                case ScanDeviceType.SDT_0PLC:
                case ScanDeviceType.SDT_1RaysDev:
                case ScanDeviceType.SDT_4PlateRcgn:
                case ScanDeviceType.SDT_5ChassisScan:
                case ScanDeviceType.SDT_6LED:
                case ScanDeviceType.SDT_7BarricadeLever:
                case ScanDeviceType.SDT_2TrailCar:
                case ScanDeviceType.SDT_3Detector:
                case ScanDeviceType.SDT_8FloorScale:
                    {
                        if (sdp.Opcode == 0x00)
                        {
                            if (content.Length > 1)
                            {
                                throw new Exception("Wrong command data length");
                            }

                            if (content[0] == 0x00)
                            {
                                Meaning = "Not connected";
                            }
                            else if (content[0] == 0x01)
                            {
                                Meaning = "linking";
                            }
                            else if (content[0] == 0x02)
                            {
                                Meaning = "Connected";
                            }
                            else if (content[0] == 0x03)
                            {
                                Meaning = "Ready";
                            }
                            else
                            {
                                throw new Exception("Error in command data content");
                            }
                        }
                        else if (sdp.Opcode == 0x01)
                        {
                            byte n = content[0];
                            if (content.Length != n + 1)
                            {
                                throw new Exception("Wrong command data length");
                            }

                            Meaning = string.Format("breakdown code: ");
                            for(int i = 1;i < content[0]+1;i++)
                            {
                                string str = string.Format("{0:X2}", content[i]);
                                Meaning = Meaning + str + " ";
                            }
                        }
                        else
                        {
                            throw new Exception("Error in command data content");
                        }
                    }
                    break;
                case ScanDeviceType.SDT_100Control:
                    {
                        if (content != null)
                        {
                            throw new Exception("Wrong command data length");
                        }
                    }
                    break;
            }
            return Meaning;
        }

        //命令含义
        internal virtual string CommandMeaning()
        {
            string sMeaning = "";

            switch(sdt)
            {
                case ScanDeviceType.SDT_0PLC:
                    {
                        sMeaning += "PLC";
                    }
                    break;
                case ScanDeviceType.SDT_1RaysDev:
                    {
                        sMeaning += "Ray device";
                    }
                    break;
                case ScanDeviceType.SDT_4PlateRcgn:
                    {
                        sMeaning += "License plate recognition";
                    }
                    break;
                case ScanDeviceType.SDT_5ChassisScan:
                    {
                        sMeaning += "Chassis scanning";
                    }
                    break;
                case ScanDeviceType.SDT_6LED:
                    {
                        sMeaning += "Led large screen";
                    }
                    break;
                case ScanDeviceType.SDT_7BarricadeLever:
                    {
                        sMeaning += "Brake pull rod";
                    }
                    break;
                case ScanDeviceType.SDT_2TrailCar:
                    {
                        sMeaning += "trailer";
                    }
                    break;
                case ScanDeviceType.SDT_3Detector:
                    {
                        sMeaning += "explorer";
                    }
                    break;
                case ScanDeviceType.SDT_8FloorScale:
                    {
                        sMeaning += "wagon balance";
                    }
                    break;
                case ScanDeviceType.SDT_100Control:
                    {
                        sMeaning += "control command";
                    }
                    break;
            }

            string sOpcodeMeaning = OpcodeMeaning(sdp.Opcode);
            string sCommandDataMeaning = CommandDataMeaning(sdp.CommandData);

            sMeaning = sMeaning + sOpcodeMeaning + "-" + sCommandDataMeaning;

            return sMeaning;
        }

        //扫描设备类型
        internal ScanDeviceType sdt = ScanDeviceType.SDT_0PLC;

        //接收命令执行动作
        internal virtual void ExecuteAction()
        {

        }
    }

    internal class ScanDevice0PLC : ScanDevice
    {
        public ScanDevice0PLC(ScanDevProtocol protocol)
            :base(ScanDeviceType.SDT_0PLC,protocol)
        {
        }
        internal override string OpcodeMeaning(byte opcode)
        {
            string sMeaning = "";
            ScanDevOpcode sdo = (ScanDevOpcode)opcode;
            switch (sdo)
            {
                case ScanDevOpcode.SDO_0Status:
                case ScanDevOpcode.SDO_1Error:
                    {
                        sMeaning = base.OpcodeMeaning(opcode);
                    }
                    break;
                case ScanDevOpcode.SDO_2Custom:
                    {
                        sMeaning = "Safety interlocking state";
                    }
                    break;
                default:
                    {
                        throw new Exception("The device type does not have this opcode.");
                    }
            }
            return sMeaning;
        }
        internal override string CommandDataMeaning(byte[] content)
        {
            string sMeaning = "";
            if (sdp.Opcode == 0x00 || sdp.Opcode == 0x01)
            {
                sMeaning = base.CommandDataMeaning(content);
            }
            else if (sdp.Opcode == 0x02)
            {
                if (content.Length > 1)
                {
                    throw new Exception("Wrong command data length");
                }

                if (content[0] == 0x00)
                {
                    sMeaning = "Not ready";
                }
                else if(content[0] == 0x01)
                {
                    sMeaning = "Ready";
                }
                else
                {
                    throw new Exception("Error in command data content");
                }
            }

            return sMeaning;
        }
    }

    internal class ScanDevice1:ScanDevice
    {
        public ScanDevice1(ScanDevProtocol protocol)
            :base(ScanDeviceType.SDT_1RaysDev,protocol)
        {
        }
        internal override string OpcodeMeaning(byte opcode)
        {
            string sMeaning = "";
            ScanDevOpcode sdo = (ScanDevOpcode)opcode;
            switch (sdo)
            {
                case ScanDevOpcode.SDO_0Status:
                case ScanDevOpcode.SDO_1Error:
                    {
                        sMeaning = base.OpcodeMeaning(opcode);
                    }
                    break;
                case ScanDevOpcode.SDO_2Custom:
                    {
                        sMeaning = "Electron beam state";
                    }
                    break;
                default:
                    {
                        throw new Exception("Electron beam state");
                    }
            }
            return sMeaning;
        }
        internal override string CommandDataMeaning(byte[] content)
        {
            string sMeaning = "";
            if (sdp.Opcode == 0x00 || sdp.Opcode == 0x01)
            {
                sMeaning = base.CommandDataMeaning(content);
            }
            else if (sdp.Opcode == 0x02)
            {
                if (content.Length > 1)
                {
                    throw new Exception("Wrong command data length");
                }

                switch(content[0])
                {
                    case 0x00:
                        {
                            sMeaning = "Finish preheating";
                        }
                        break;
                    case 0x01:
                        {
                            sMeaning = "To be preheated";
                        }
                        break;
                    case 0x02:
                        {
                            sMeaning = "Medium preheating";
                        }
                        break;
                    case 0x03:
                        {
                            sMeaning = "Out of the beam";
                        }
                        break;
                    default:
                        throw new Exception("Error in command data content");
                }
            }

            return sMeaning;
        }
    }

    internal class ScanDevice4 : ScanDevice
    {
        public ScanDevice4(ScanDevProtocol protocol)
            : base(ScanDeviceType.SDT_4PlateRcgn, protocol)
        {
        }
        internal override string OpcodeMeaning(byte opcode)
        {
            string sMeaning = "";
            ScanDevOpcode sdo = (ScanDevOpcode)opcode;
            switch (sdo)
            {
                case ScanDevOpcode.SDO_0Status:
                case ScanDevOpcode.SDO_1Error:
                    {
                        sMeaning = base.OpcodeMeaning(opcode);
                    }
                    break;
                case ScanDevOpcode.SDO_2Custom:
                    {
                        sMeaning = "vehicle license plate number";
                    }
                    break;
                default:
                    {
                        throw new Exception("The device type does not have this opcode.");
                    }
            }
            return sMeaning;
        }
        internal override string CommandDataMeaning(byte[] content)
        {
            string sMeaning = "";
            if (sdp.Opcode == 0x00 || sdp.Opcode == 0x01)
            {
                sMeaning = base.CommandDataMeaning(content);
            }
            else if (sdp.Opcode == 0x02)
            {
                byte n = content[0];
                if (content.Length != n + 1)
                {
                    throw new Exception("Wrong command data length");
                }

                sMeaning = string.Format("vehicle license plate number: ");
                for (int i = 1; i < content[0] + 1; i++)
                {
                    string str = string.Format("{0:d}", content[i]);
                    sMeaning = sMeaning + str;
                }
            }
            else
            {
                throw new Exception("Error in command data content");
            }
            return sMeaning;
        }
    }

    internal class ScanDevice5 : ScanDevice
    {
        public ScanDevice5(ScanDevProtocol protocol)
            : base(ScanDeviceType.SDT_5ChassisScan, protocol)
        {
        }
        internal override string OpcodeMeaning(byte opcode)
        {
            string sMeaning = "";
            ScanDevOpcode sdo = (ScanDevOpcode)opcode;
            switch (sdo)
            {
                case ScanDevOpcode.SDO_0Status:
                case ScanDevOpcode.SDO_1Error:
                    {
                        sMeaning = base.OpcodeMeaning(opcode);
                    }
                    break;
                case ScanDevOpcode.SDO_2Custom:
                    {
                        sMeaning = "scanned picture";
                    }
                    break;
                default:
                    {
                        throw new Exception("The device type does not have this opcode.");
                    }
            }
            return sMeaning;
        }
        internal override string CommandDataMeaning(byte[] content)
        {
            string sMeaning = "";
            if (sdp.Opcode == 0x00 || sdp.Opcode == 0x01)
            {
                sMeaning = base.CommandDataMeaning(content);
            }
            else if (sdp.Opcode == 0x02)
            {
                byte n = content[0];
                if (content.Length != n + 1)
                {
                    throw new Exception("Wrong command data length");
                }

                sMeaning = string.Format("Image path: ");
                for (int i = 1; i < content[0] + 1; i++)
                {
                    sMeaning += content[i];
                }
            }
            return sMeaning;
        }
    }

    internal class ScanDevice6 : ScanDevice
    {
        public ScanDevice6(ScanDevProtocol protocol)
            : base(ScanDeviceType.SDT_6LED, protocol)
        {
        }
        internal override string OpcodeMeaning(byte opcode)
        {
            string sMeaning = "";
            ScanDevOpcode sdo = (ScanDevOpcode)opcode;
            switch (sdo)
            {
                case ScanDevOpcode.SDO_0Status:
                case ScanDevOpcode.SDO_1Error:
                    {
                        sMeaning = base.OpcodeMeaning(opcode);
                    }
                    break;
                case ScanDevOpcode.SDO_2Custom:
                    {
                        sMeaning = "Display text";
                    }
                    break;
                default:
                    {
                        throw new Exception("The device type does not have this opcode.");
                    }
            }
            return sMeaning;
        }
        internal override string CommandDataMeaning(byte[] content)
        {
            string sMeaning = "";
            if (sdp.Opcode == 0x00 || sdp.Opcode == 0x01)
            {
                sMeaning = base.CommandDataMeaning(content);
            }
            else if (sdp.Opcode == 0x02)
            {
                byte n = content[0];
                if (content.Length != n + 1)
                {
                    throw new Exception("Wrong command data length");
                }

                sMeaning = string.Format("Text content: ");
                for (int i = 1; i < content[0] + 1; i++)
                {
                    sMeaning += content[i];
                }
            }
            return sMeaning;
        }
    }

    internal class ScanDevice7 : ScanDevice
    {
        public ScanDevice7(ScanDevProtocol protocol)
            : base(ScanDeviceType.SDT_7BarricadeLever, protocol)
        {
        }
        internal override string OpcodeMeaning(byte opcode)
        {
            string sMeaning = "";
            ScanDevOpcode sdo = (ScanDevOpcode)opcode;
            switch (sdo)
            {
                case ScanDevOpcode.SDO_0Status:
                case ScanDevOpcode.SDO_1Error:
                    {
                        sMeaning = base.OpcodeMeaning(opcode);
                    }
                    break;
                case ScanDevOpcode.SDO_2Custom:
                    {
                        sMeaning = "control command";
                    }
                    break;
                default:
                    {
                        throw new Exception("The device type does not have this opcode.");
                    }
            }
            return sMeaning;
        }
        internal override string CommandDataMeaning(byte[] content)
        {
            string sMeaning = "";
            if (sdp.Opcode == 0x00 || sdp.Opcode == 0x01)
            {
                sMeaning = base.CommandDataMeaning(content);
            }
            else if (sdp.Opcode == 0x02)
            {
                if (content.Length > 1)
                {
                    throw new Exception("Wrong command data length");
                }

                if (content[0] == 0x00)
                {
                    sMeaning = "Lower the pull rod";
                }
                else if(content[0] == 0x01)
                {
                    sMeaning = "Lift the pull rod";
                }
                else
                {
                    throw new Exception("Error in command data content");
                }
            }

            return sMeaning;
        }

        internal override void ExecuteAction()
        {
            if (sdp.Opcode == (byte)ScanDevOpcode.SDO_2Custom)
            {
                if (sdp.CommandData.Length == 1)
                {
                    if (sdp.CommandData[0] == 0)
                    {
                        sdp.BarricadeLeverDownCallBack();
                    }
                    else
                    {
                        sdp.BarricadeLeverUpCallBack();
                    }
                    return;
                }
            }
            throw new Exception("Error in executing command of brake lever");
        }
    }

    internal class ScanDevice100: ScanDevice
    {
        public ScanDevice100(ScanDevProtocol protocol)
            :base(ScanDeviceType.SDT_100Control,protocol)
        {
        }

        internal override string OpcodeMeaning(byte opcode)
        {
            string sMeaning = "";
            ScanDevOpcode sdo = (ScanDevOpcode)opcode;
            switch (sdo)
            {
                case ScanDevOpcode.SDO_0Status:
                    {
                        sMeaning = "Start scanning";
                    }
                    break;
                case ScanDevOpcode.SDO_1Error:
                    {
                        sMeaning = "Stop scanning";
                    }
                    break;
                case ScanDevOpcode.SDO_2Custom:
                    {
                        sMeaning = "reset";
                    }
                    break;
                case ScanDevOpcode.SDO_3Custom:
                    {
                        sMeaning = "set up";
                    }
                    break;
                case ScanDevOpcode.SDO_4Custom:
                    {
                        sMeaning = "quit";
                    }
                    break;
                case ScanDevOpcode.SDO_10Custom:
                    {
                        sMeaning = "Start dark correction";
                    }
                    break;
                case ScanDevOpcode.SDO_11Custom:
                    {
                        sMeaning = "Start drawing";
                    }
                    break;
                case ScanDevOpcode.SDO_12Custom:
                    {
                        sMeaning = "Stop drawing";
                    }
                    break;
                default:
                    {
                        throw new Exception("The device type does not have this opcode.");
                    }
            }
            return sMeaning;
        }
        
        internal override void ExecuteAction()
        {
            if (sdp.CommandData != null)
            {
                throw new Exception("Error in control instruction execution command");
            }
            switch(sdp.Opcode)
            {
                case 0:
                    {
                        sdp.StartScanCallBack();
                    }
                    break;
                case 1:
                    {
                        sdp.StopScanCallBack();
                    }
                    break;
                case 2:
                    {
                        sdp.ResetCallBack();
                    }
                    break;
                case 3:
                    {
                        sdp.SetCallBack();
                    }
                    break;
                case 4:
                    {
                        sdp.AbortCallBack();
                    }
                    break;
                default:
                    throw new Exception("Error in control instruction execution command");
            }
        }
    }

    [Description("ScanDevice")]
    [Export(nameof(ScanDevProtocol), typeof(BaseScanProtocol))]
    public class ScanDevProtocol : BaseScanProtocol
    {
        public ScanDevice sd = null;
        internal byte Opcode = 0x00;
        internal byte[] CommandData = null;

        public ScanDevProtocol()
        {
            ProtocolName = "Control station-scanning station protocol";
            sd = new ScanDevice0PLC(this);
            RecvActionEvent += RecvAction;
        }

        //扫描设备类型
        public ScanDeviceType Device
        {
            get
            {
                return sd.sdt;
            }
            set
            {
                switch(value)
                {
                    case ScanDeviceType.SDT_0PLC:
                        {
                            sd = new ScanDevice0PLC(this);
                        }
                        break;
                    case ScanDeviceType.SDT_1RaysDev:
                        {
                            sd = new ScanDevice1(this);
                        }
                        break;
                    case ScanDeviceType.SDT_4PlateRcgn:
                        {
                            sd = new ScanDevice4(this);
                        }
                        break;
                    case ScanDeviceType.SDT_5ChassisScan:
                        {
                            sd = new ScanDevice5(this);
                        }
                        break;
                    case ScanDeviceType.SDT_6LED:
                        {
                            sd = new ScanDevice6(this);
                        }
                        break;
                    case ScanDeviceType.SDT_7BarricadeLever:
                        {
                            sd = new ScanDevice7(this);
                        }
                        break;
                    case ScanDeviceType.SDT_2TrailCar:
                    case ScanDeviceType.SDT_3Detector:
                    case ScanDeviceType.SDT_8FloorScale:
                        {
                            sd = new ScanDevice(value, this);
                        }
                        break;
                    case ScanDeviceType.SDT_100Control:
                        {
                            sd = new ScanDevice100(this);
                        }
                        break;
                    default:
                        throw new Exception("Wrong communication device type");
                }
            }
        }

        //获取扫描设备
        public ScanDevice GetScanDevice()
        {
            return sd;
        }

        //获取操作码
        public byte GetOpcode
        {
            get { return Opcode; }
        }

        //获取数据内容
        public byte[] GetCommandData
        {
            get { return CommandData; }
        }

        //生成发送命令
        public override byte[] BuildCommand()
        {
            sd.OpcodeMeaning(Opcode);
            sd.CommandDataMeaning(CommandData);

            String sCommand = "";
            sCommand += sd.sdt;
            sCommand += Opcode;

            if (CommandData != null)
            {
                string str = Encoding.ASCII.GetString(CommandData);
                sCommand += str;
            }

            byte[] command = Encoding.ASCII.GetBytes(sCommand);
            int len = sCommand.Length;
            byte[] crc = CRC16(command,len);
            sCommand += crc[0];
            sCommand += crc[1];
            sCommand += "\r\n";
            byte[] Command = Encoding.ASCII.GetBytes(sCommand);

            return Command;
        }

        //命令含义
        public override string CommandMeaning()
        {
            return sd.CommandMeaning();
        }

        //解析服务端主动发来的信息含义
        protected override bool ResolveCommand(byte[] buffer, int size)
        {
            SendBackBuf = "";
            string str = "";
            string Action = "receive data";

            str += buffer[size - 2];
            str += buffer[size - 1];
            if (str != "\r\n")
            {
                throw new Exception(ProtocolName + "-" + Action + "-End character error");
            }

            if (size == 7)
            {
                byte[] data = new byte[3];
                data[0] = buffer[0];
                data[1] = buffer[1];
                data[2] = buffer[2];
                byte[] CRC = CRC16(data, 3);
                if (CRC[0] != buffer[3] || CRC[1] != buffer[4])
                {
                    throw new Exception(ProtocolName + "-" + Action + "-CRC check error");
                }

                if (buffer[0] != 0x07)
                {
                    throw new Exception(ProtocolName + "-" + Action + "-Parsing device parameters is illegal.");
                }

                try
                {
                    byte[] content = new byte[1];
                    content[0] = buffer[2];
                    Device = ScanDeviceType.SDT_7BarricadeLever;
                    sd.RecieveCommand(buffer[1],content);
                }
                catch (System.Exception ex)
                {
                    UnlockSend();
                    throw new Exception(ProtocolName + "-" + Action + "-" + ex.Message);
                }
            }
            else if (size == 6)
            {
                byte[] data = new byte[2];
                data[0] = buffer[0];
                data[1] = buffer[1];
                byte[] CRC = CRC16(data, 2);
                if (CRC[0] != buffer[2] || CRC[1] != buffer[3])
                {
                    throw new Exception(ProtocolName + "-" + Action + "-CRC check error");
                }

                if (buffer[0] != 0x64)
                {
                    throw new Exception(ProtocolName + "-" + Action + "-Parsing device parameters is illegal.");
                }
                
                try
                {
                    LockSend();
                    Device = ScanDeviceType.SDT_100Control;
                    sd.RecieveCommand(buffer[1],null);
                    UnlockSend();
                }
                catch (System.Exception ex)
                {
                    UnlockSend();
                    throw new Exception(ProtocolName + "-" + Action + "-" + ex.Message);
                }
            }

            return true;
        }
        
        //发送命令前准备
        protected override void PrepareSend(byte[] SendBuf)
        {
            NeedFeedBack = false;
            ExeResult = true;
        }

        //发送PLC状态
        public bool SendPLCStatus(DeviceStatus ds)
        {
            byte[] command = PLCStatusCommand((byte)ds);
            bool bRet = SendCommand(command);
            return bRet;
        }
        //PLC状态命令
        public byte[] PLCStatusCommand(byte ds)
        {
            Device = ScanDeviceType.SDT_0PLC;
            Opcode = 0;
            CommandData = new byte[] { ds };
            byte[] command = BuildCommand();
            return command;
        }

        //发送PLC故障命令
        public bool SendPLCError(byte[] ErrorCode)
        {
            byte[] command = PLCErrorCommand(ErrorCode);
            bool bRet = SendCommand(command);
            return bRet;
        }
        //PLC故障命令
        public byte[] PLCErrorCommand(byte[] ErrorCode)
        {
            byte Len = (byte)ErrorCode.Length;
            Opcode = 1;
            CommandData = new byte[Len+1];
            CommandData[0] = Len;
            Array.Copy(ErrorCode, 0, CommandData, 1, Len);
            byte[] command = BuildCommand();
            return command;
        }

        //发送PLC安全联锁状态
        public bool SendPLCSafeLockStatus(bool Ready)
        {
            byte[] command = PLCSafeLockStatusCommand(Ready);
            bool bRet = SendCommand(command);
            return bRet;
        }
        //PLC安全联锁状态命令
        public byte[] PLCSafeLockStatusCommand(bool Ready)
        {
            Device = ScanDeviceType.SDT_0PLC;
            Opcode = 2;
            CommandData = new byte[1];
            CommandData[0] = (byte)(Ready ? 0x01 : 0x00);
            byte[] command = BuildCommand();
            return command;
        }

        //发送射线源状态
        public bool SendRaysDevStatus(DeviceStatus ds)
        {
            byte[] command = RaysDevStatusCommand((byte)ds);
            bool bRet = SendCommand(command);
            return bRet;
        }
        //射线源状态命令
        public byte[] RaysDevStatusCommand(byte ds)
        {
            Device = ScanDeviceType.SDT_1RaysDev;
            Opcode = 0;
            CommandData = new byte[] { ds };
            byte[] command = BuildCommand();
            return command;
        }


        //发送射线源故障状态
        public bool SendRaysDevError(byte[] ErrorCode)
        {
            byte[] command = RaysDevErrorCommand(ErrorCode);
            bool bRet = SendCommand(command);
            return bRet;
        }
        //射线源故障命令
        public byte[] RaysDevErrorCommand(byte[] ErrorCode)
        {
            Device = ScanDeviceType.SDT_1RaysDev;
            Opcode = 1;
            byte Len = (byte)ErrorCode.Length;
            CommandData = new byte[Len + 1];
            CommandData[0] = Len;
            Array.Copy(ErrorCode, 0, CommandData, 1, Len);
            byte[] command = BuildCommand();
            return command;
        }
        
        //发送射线源电子束状态
        public bool SendRaysDevElectronBeam(byte ds)
        {
            byte[] command = RaysDevElectronBeamCommand(ds);
            bool bRet = SendCommand(command);
            return bRet;
        }
        //射线源电子束命令
        public byte[] RaysDevElectronBeamCommand(byte ds)
        {
            Device = ScanDeviceType.SDT_1RaysDev;
            Opcode = 2;
            CommandData = new byte[] { ds };
            byte[] command = BuildCommand();
            return command;
        }
        
        //发送拖车状态
        public bool SendTrailCarStatus(byte ds)
        {
            byte[] command = TrailCarStatusCommand(ds);
            bool bRet = SendCommand(command);
            return bRet;
        }
        //拖车状态命令
        public byte[] TrailCarStatusCommand(byte ds)
        {
            Device = ScanDeviceType.SDT_2TrailCar;
            Opcode = 0;
            CommandData = new byte[] { ds };
            byte[] command = BuildCommand();
            return command;
        }

        //发送拖车状态
        public bool SendTrailCarError(byte[] ErrorCode)
        {
            byte[] command = TrailCarErrorCommand(ErrorCode);
            bool bRet = SendCommand(command);
            return bRet;
        }
        //拖车状态命令
        public byte[] TrailCarErrorCommand(byte[] ErrorCode)
        {
            Device = ScanDeviceType.SDT_2TrailCar;
            Opcode = 1;
            byte Len = (byte)ErrorCode.Length;
            CommandData = new byte[Len + 1];
            CommandData[0] = Len;
            Array.Copy(ErrorCode, 0, CommandData, 1, Len);
            byte[] command = BuildCommand();
            return command;
        }
        
        //发送探测器状态
        public bool SendDetectorStatus(byte ds)
        {
            byte[] command = DetectorStatusCommand(ds);
            bool bRet = SendCommand(command);
            return bRet;
        }
        //探测器状态命令
        public byte[] DetectorStatusCommand(byte ds)
        {
            Device = ScanDeviceType.SDT_3Detector;
            Opcode = 0;
            CommandData = new byte[] { ds };
            byte[] command = BuildCommand();
            return command;
        }

        //发送探测器状态
        public bool SendDetectorCarError(byte[] ErrorCode)
        {
            byte[] command = DetectorErrorCommand(ErrorCode);
            bool bRet = SendCommand(command);
            return bRet;
        }
        //探测器状态命令
        public byte[] DetectorErrorCommand(byte[] ErrorCode)
        {
            Device = ScanDeviceType.SDT_3Detector;
            Opcode = 1;
            byte Len = (byte)ErrorCode.Length;
            CommandData = new byte[Len + 1];
            CommandData[0] = Len;
            Array.Copy(ErrorCode, 0, CommandData, 1, Len);
            byte[] command = BuildCommand();
            return command;
        }

        //发送车牌识别状态
        public bool SendPlateRcgnStatus(byte ds)
        {
            byte[] command = PlateRcgnStatusCommand(ds);
            bool bRet = SendCommand(command);
            return bRet;
        }
        //车牌识别状态命令
        public byte[] PlateRcgnStatusCommand(byte ds)
        {
            Device = ScanDeviceType.SDT_4PlateRcgn;
            Opcode = 0;
            CommandData = new byte[] { ds };
            byte[] command = BuildCommand();
            return command;
        }

        //发送车牌识别错误码
        public bool SendPlateRcgnCarError(byte[] ErrorCode)
        {
            byte[] command = PlateRcgnErrorCommand(ErrorCode);
            bool bRet = SendCommand(command);
            return bRet;
        }
        //车牌识别错误码命令
        public byte[] PlateRcgnErrorCommand(byte[] ErrorCode)
        {
            Device = ScanDeviceType.SDT_4PlateRcgn;
            Opcode = 1;
            byte Len = (byte)ErrorCode.Length;
            CommandData = new byte[Len + 1];
            CommandData[0] = Len;
            Array.Copy(ErrorCode, 0, CommandData, 1, Len);
            byte[] command = BuildCommand();
            return command;
        }

        //发送车牌识别车牌号
        public bool SendPlateRcgn(byte[] CarCode)
        {
            byte[] command = PlateRcgnCommand(CarCode);
            bool bRet = SendCommand(command);
            return bRet;
        }
        //车牌识别车牌号命令
        public byte[] PlateRcgnCommand(byte[] CarCode)
        {
            Device = ScanDeviceType.SDT_4PlateRcgn;
            Opcode = 2;
            byte Len = (byte)CarCode.Length;
            CommandData = new byte[Len + 1];
            CommandData[0] = Len;
            Array.Copy(CarCode, 0, CommandData, 1, Len);
            byte[] command = BuildCommand();
            return command;
        }

        //发送底盘扫描状态
        public bool SendChassisScanStatus(byte ds)
        {
            byte[] command = ChassisScanStatusCommand(ds);
            bool bRet = SendCommand(command);
            return bRet;
        }
        //底盘扫描状态命令
        public byte[] ChassisScanStatusCommand(byte ds)
        {
            Device = ScanDeviceType.SDT_5ChassisScan;
            Opcode = 0;
            CommandData = new byte[] { ds };
            byte[] command = BuildCommand();
            return command;
        }

        //发送底盘扫描错误码
        public bool SendChassisScanCarError(byte[] ErrorCode)
        {
            byte[] command = ChassisScanErrorCommand(ErrorCode);
            bool bRet = SendCommand(command);
            return bRet;
        }
        //底盘扫描错误码命令
        public byte[] ChassisScanErrorCommand(byte[] ErrorCode)
        {
            Device = ScanDeviceType.SDT_5ChassisScan;
            Opcode = 1;
            byte Len = (byte)ErrorCode.Length;
            CommandData = new byte[Len + 1];
            CommandData[0] = Len;
            Array.Copy(ErrorCode, 0, CommandData, 1, Len);
            byte[] command = BuildCommand();
            return command;
        }

        //发送底盘扫描图片路径
        public bool SendChassisScanImg(byte[] ImgPath)
        {
            byte[] command = ChassisScanImgCommand(ImgPath);
            bool bRet = SendCommand(command);
            return bRet;
        }
        //底盘扫描图片路径命令
        public byte[] ChassisScanImgCommand(byte[] ImgPath)
        {
            Device = ScanDeviceType.SDT_5ChassisScan;
            Opcode = 2;
            byte Len = (byte)ImgPath.Length;
            CommandData = new byte[Len + 1];
            CommandData[0] = Len;
            Array.Copy(ImgPath, 0, CommandData, 1, Len);
            byte[] command = BuildCommand();
            return command;
        }

        //发送LED大屏状态
        public bool SendLEDStatus(byte ds)
        {
            byte[] command = LEDStatusCommand(ds);
            bool bRet = SendCommand(command);
            return bRet;
        }
        //LED大屏状态命令
        public byte[] LEDStatusCommand(byte ds)
        {
            Device = ScanDeviceType.SDT_6LED;
            Opcode = 0;
            CommandData = new byte[] { ds };
            byte[] command = BuildCommand();
            return command;
        }

        //发送LED大屏错误码
        public bool SendLEDError(byte[] ErrorCode)
        {
            byte[] command = LEDErrorCommand(ErrorCode);
            bool bRet = SendCommand(command);
            return bRet;
        }
        //LED大屏错误码命令
        public byte[] LEDErrorCommand(byte[] ErrorCode)
        {
            Device = ScanDeviceType.SDT_6LED;
            Opcode = 1;
            byte Len = (byte)ErrorCode.Length;
            CommandData = new byte[Len + 1];
            CommandData[0] = Len;
            Array.Copy(ErrorCode, 0, CommandData, 1, Len);
            byte[] command = BuildCommand();
            return command;
        }

        //发送LED大屏显示文本
        public bool SendLEDText(byte[] txt)
        {
            byte[] command = LEDTextCommand(txt);
            bool bRet = SendCommand(command);
            return bRet;
        }
        //LED大屏显示文本命令
        public byte[] LEDTextCommand(byte[] txt)
        {
            Device = ScanDeviceType.SDT_6LED;
            Opcode = 2;
            byte Len = (byte)txt.Length;
            CommandData = new byte[Len + 1];
            CommandData[0] = Len;
            Array.Copy(txt, 0, CommandData, 1, Len);
            byte[] command = BuildCommand();
            return command;
        }

        //发送道闸拉杆状态
        public bool SendBarricadeLeverStatus(byte ds)
        {
            byte[] command = BarricadeLeverStatusCommand(ds);
            bool bRet = SendCommand(command);
            return bRet;
        }
        //道闸拉杆状态命令
        public byte[] BarricadeLeverStatusCommand(byte ds)
        {
            Device = ScanDeviceType.SDT_7BarricadeLever;
            Opcode = 0;
            CommandData = new byte[] { ds };
            byte[] command = BuildCommand();
            return command;
        }

        //发送道闸拉杆错误码
        public bool SendBarricadeLeverError(byte[] ErrorCode)
        {
            byte[] command = BarricadeLeverErrorCommand(ErrorCode);
            bool bRet = SendCommand(command);
            return bRet;
        }
        //道闸拉杆错误码命令
        public byte[] BarricadeLeverErrorCommand(byte[] ErrorCode)
        {
            Device = ScanDeviceType.SDT_7BarricadeLever;
            Opcode = 1;
            byte Len = (byte)ErrorCode.Length;
            CommandData = new byte[Len + 1];
            CommandData[0] = Len;
            Array.Copy(ErrorCode, 0, CommandData, 1, Len);
            byte[] command = BuildCommand();
            return command;
        }

        //发送地磅状态
        public bool SendFloorScaleStatus(byte ds)
        {
            byte[] command = FloorScaleStatusCommand(ds);
            bool bRet = SendCommand(command);
            return bRet;
        }
        //地磅状态命令
        public byte[] FloorScaleStatusCommand(byte ds)
        {
            Device = ScanDeviceType.SDT_8FloorScale;
            Opcode = 0;
            CommandData = new byte[] { ds };
            byte[] command = BuildCommand();
            return command;
        }

        //发送地磅错误码
        public bool SendFloorScaleError(byte[] ErrorCode)
        {
            byte[] command = FloorScaleErrorCommand(ErrorCode);
            bool bRet = SendCommand(command);
            return bRet;
        }
        //地磅错误码命令
        public byte[] FloorScaleErrorCommand(byte[] ErrorCode)
        {
            Device = ScanDeviceType.SDT_8FloorScale;
            Opcode = 1;
            byte Len = (byte)ErrorCode.Length;
            CommandData = new byte[Len + 1];
            CommandData[0] = Len;
            Array.Copy(ErrorCode, 0, CommandData, 1, Len);
            byte[] command = BuildCommand();
            return command;
        }

        //开始暗校正
        public bool StartDarkCorrection()
        {
            byte[] command = StartDarkCorrectionCommand();
            bool bRet = SendCommand(command);
            return bRet;
        }
        //开始暗校正命令
        public byte[] StartDarkCorrectionCommand()
        {
            Device = ScanDeviceType.SDT_100Control;
            Opcode = 10;
            CommandData = null;
            byte[] command = BuildCommand();
            return command;
        }

        //开始采图
        public bool StartCaptureImg()
        {
            byte[] command = StartCaptureImgCommand();
            bool bRet = SendCommand(command);
            return bRet;
        }
        //开始采图命令
        public byte[] StartCaptureImgCommand()
        {
            Device = ScanDeviceType.SDT_100Control;
            Opcode = 11;
            CommandData = null;
            byte[] command = BuildCommand();
            return command;
        }

        //停止采图
        public bool StopCaptureImg()
        {
            byte[] command = StopCaptureImgCommand();
            bool bRet = SendCommand(command);
            return bRet;
        }
        //停止采图命令
        public byte[] StopCaptureImgCommand()
        {
            Device = ScanDeviceType.SDT_100Control;
            Opcode = 12;
            CommandData = null;
            byte[] command = BuildCommand();
            return command;
        }


        public byte[] ExecuteCommand(ScanDeviceType devicetype, ScanDevOpcode opcode,byte[] content)
        {
            Device = devicetype;
            Opcode = (byte)opcode;
            byte Len = (byte)content.Length;
            if (Len == 0)
            {
                CommandData = null;
            }
            else if (Len == 1)
            {
                CommandData = new byte[1] { content[0] };
            }
            else
            {
                CommandData = new byte[Len + 1];
                CommandData[0] = Len;
                Array.Copy(content, 0, CommandData, 1, Len);
            }
            byte[] command = BuildCommand();
            return command;
        }

        public byte[] ExecuteCommand(byte devicetype, byte opcode, byte[] content)
        {
            return ExecuteCommand((ScanDeviceType)devicetype, (ScanDevOpcode)opcode,content);
        }

        public bool Execute(byte devicetype, byte opcode, byte[] content)
        {
            byte[] command = ExecuteCommand((ScanDeviceType)devicetype, (ScanDevOpcode)opcode, content);
            bool bRet = SendCommand(command);
            return bRet;
        }

        protected void RecvAction()
        {
            sd.ExecuteAction();
        }

        internal void BarricadeLeverDownCallBack()
        {
            BarricadeLeverDown?.Invoke();
        }

        internal void BarricadeLeverUpCallBack()
        {
            BarricadeLeverUp?.Invoke();
        }

        internal void StartScanCallBack()
        {
            StartScan?.Invoke();
        }

        internal void StopScanCallBack()
        {
            StopScan?.Invoke();
        }

        internal void ResetCallBack()
        {
            Reset?.Invoke();
        }

        internal void SetCallBack()
        {
            Set?.Invoke();
        }

        internal void AbortCallBack()
        {
            Abort?.Invoke();
        }

        public delegate void ActionAfterCommand();
        //降下道闸拉杆
        public event ActionAfterCommand BarricadeLeverDown;
        //升起道闸拉杆
        public event ActionAfterCommand BarricadeLeverUp;
        //开始扫描
        public event ActionAfterCommand StartScan;
        //停止扫描
        public event ActionAfterCommand StopScan;
        //复位
        public event ActionAfterCommand Reset;
        //设置
        public event ActionAfterCommand Set;
        //退出
        public event ActionAfterCommand Abort;
    }
}
