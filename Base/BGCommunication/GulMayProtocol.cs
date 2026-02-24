using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGCommunication
{
    public class GulMayProtocol : ICommonProtocol
    {
        //发送命令类型
        public enum GM_Command
        {
            GMC_InqurError = 0,
            GMC_InqurBeamStatus = 1,
            GMC_InqurWorkStatus = 2,
            GMC_InqurWarmUpMode = 3,
            GMC_InqurV = 4,
            GMC_InqurI = 5,
            GMC_SetWarmUpMode = 6,
            GMC_SetV = 7,
            GMC_SetI = 8,
            GMC_OutputBeam = 9,
            GMC_StopBeam = 10,
        }

        //出束状态
        public enum BeamStatus
        {
            BS_OutputBeam = 0,
            BS_StopBeam = 1,

        }

        //工作状态
        public enum WorkStatus
        {
            WS_Ready = 0,
            WS_WaitWarmUp = 1,
        }

        //预热模式
        public enum WarmUpMode
        {
            WUM_NotNeed = 0,
            WUM_Short = 1,
            WUM_Long = 2,
        }
        
        //当前命令
        private GM_Command CurrentCommand = GM_Command.GMC_InqurError;
        protected GM_Command CommandType
        {
            get { return CurrentCommand; }
            set
            {
                CurrentCommand = value;
                switch (CurrentCommand)
                {
                    case GM_Command.GMC_InqurError:
                        {
                            FunctionName = "查询错误码";
                        }
                        break;
                    case GM_Command.GMC_InqurBeamStatus:
                        {
                            FunctionName = "查询出束状态";
                        }
                        break;
                    case GM_Command.GMC_InqurWorkStatus:
                        {
                            FunctionName = "查询工作状态";
                        }
                        break;
                    case GM_Command.GMC_InqurWarmUpMode:
                        {
                            FunctionName = "查询预热模式";
                        }
                        break;
                    case GM_Command.GMC_InqurV:
                        {
                            FunctionName = "查询电压";
                        }
                        break;
                    case GM_Command.GMC_InqurI:
                        {
                            FunctionName = "查询电流";
                        }
                        break;
                    case GM_Command.GMC_SetWarmUpMode:
                        {
                            FunctionName = "设置预热模式";
                        }
                        break;
                    case GM_Command.GMC_SetV:
                        {
                            FunctionName = "设置电压";
                        }
                        break;
                    case GM_Command.GMC_SetI:
                        {
                            FunctionName = "设置电流";
                        }
                        break;
                    case GM_Command.GMC_OutputBeam:
                        {
                            FunctionName = "出束";
                        }
                        break;
                    case GM_Command.GMC_StopBeam:
                        {
                            FunctionName = "停束";
                        }
                        break;
                }
                ConnIntf.DebugStepLog("开始" + FunctionName);
            }
        }

        string ErrorCode = "";
        BeamStatus mBeamStatus = BeamStatus.BS_StopBeam;
        WorkStatus mWorkStatus = WorkStatus.WS_Ready;
        WarmUpMode mWarmUpMode = WarmUpMode.WUM_NotNeed;
        int mVoltage = 0;
        float mIcurent = 0.0f;

        public GulMayProtocol()
        {
            ProtocolName = "控制站-高美射线源协议";
        }

        //发送命令前准备
        protected override void PrepareSend(byte[] SendBuf)
        {
            ConnIntf.SendedBuf = null;
            CorrectRet.Clear();

            switch (CommandType)
            {
                case GM_Command.GMC_InqurError:
                    {
                        ErrorCode = "";
                        NeedFeedBack = true;
                        ExeResult = false;
                    }
                    break;
                case GM_Command.GMC_InqurBeamStatus:
                    {
                        mBeamStatus = BeamStatus.BS_StopBeam;
                        NeedFeedBack = true;
                        ExeResult = false;
                    }
                    break;
                case GM_Command.GMC_InqurWorkStatus:
                    {
                        mWorkStatus = WorkStatus.WS_Ready;
                        NeedFeedBack = true;
                        ExeResult = false;
                    }
                    break;
                case GM_Command.GMC_InqurWarmUpMode:
                    {
                        mWarmUpMode = WarmUpMode.WUM_NotNeed;
                        NeedFeedBack = true;
                        ExeResult = false;
                    }
                    break;
                case GM_Command.GMC_InqurV:
                    {
                        mVoltage = 0;
                        NeedFeedBack = true;
                        ExeResult = false;
                    }
                    break;
                case GM_Command.GMC_InqurI:
                    {
                        mIcurent = 0;
                        NeedFeedBack = true;
                        ExeResult = false;
                    }
                    break;
                case GM_Command.GMC_SetWarmUpMode:
                    {
                        NeedFeedBack = false;
                        ExeResult = false;
                    }
                    break;
                case GM_Command.GMC_SetV:
                    {
                        NeedFeedBack = false;
                        ExeResult = false;
                    }
                    break;
                case GM_Command.GMC_SetI:
                    {
                        NeedFeedBack = false;
                        ExeResult = false;
                    }
                    break;
                case GM_Command.GMC_OutputBeam:
                    {
                        NeedFeedBack = false;
                        ExeResult = false;
                    }
                    break;
                case GM_Command.GMC_StopBeam:
                    {
                        NeedFeedBack = false;
                        ExeResult = false;
                    }
                    break;
            }
        }

        //创建发送命令
        public override byte[] BuildCommand()
        {
            byte[] Command = null;
            string sCommand = "";
            switch (CommandType)
            {
                case GM_Command.GMC_InqurError:
                    {
                        sCommand = InqurErrorCommand();
                    }
                    break;
                case GM_Command.GMC_InqurBeamStatus:
                    {
                        sCommand = InqurBeamStatusCommand();
                    }
                    break;
                case GM_Command.GMC_InqurWorkStatus:
                    {
                        sCommand = InqurWorkStatusCommand();
                    }
                    break;
                case GM_Command.GMC_InqurWarmUpMode:
                    {
                        sCommand = InqurWarmUpModeCommand();
                    }
                    break;
                case GM_Command.GMC_InqurV:
                    {
                        sCommand = InqurVoltageCommand();
                    }
                    break;
                case GM_Command.GMC_InqurI:
                    {
                        sCommand = InqurICurrentCommand();
                    }
                    break;
                case GM_Command.GMC_SetWarmUpMode:
                    {
                        sCommand = SetWarmUpModeCommand(mWarmUpMode);
                    }
                    break;
                case GM_Command.GMC_SetV:
                    {
                        sCommand = SetVoltageCommand(mVoltage);
                    }
                    break;
                case GM_Command.GMC_SetI:
                    {
                        sCommand = SetICurrentCommand(mIcurent);
                    }
                    break;
                case GM_Command.GMC_OutputBeam:
                    {
                        sCommand = OutputBeamCommand();
                    }
                    break;
                case GM_Command.GMC_StopBeam:
                    {
                        sCommand = StopBeamCommand();
                    }
                    break;
            }
            Command = Encoding.ASCII.GetBytes(sCommand);
            return Command;
        }

        //解析发送命令后的回传信息
        internal override bool ResolveBackCommand(byte[] buffer, int size)
        {
            string Action = "【" + FunctionName + "】回传数据";

            if (ConnIntf.SendedBuf == null)
            {
                throw new Exception(ProtocolName + "-" + Action + "-未知错误");
            }

            string Result = Encoding.ASCII.GetString(buffer);
            int nLen = Result.Length;
            if (Result.Substring(nLen-1) != "\r")
            {
                throw new Exception(ProtocolName + "-" + Action + "-结束符错误");
            }

            switch (CommandType)
            {
                case GM_Command.GMC_InqurError:
                    {
                        if (Result == "?E\r")
                        {
                            ErrorCode = "";
                        }
                        else if(nLen == 6)
                        {
                            if(Result.Substring(0,2) != "?E")
                            {
                                throw new Exception(ProtocolName + "-" + Action + "-数据非法");
                            }
                            ErrorCode = Result.Substring(2, 3);
                        }
                        else
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-数据长度非法");
                        }
                    }
                    break;
                case GM_Command.GMC_InqurBeamStatus:
                    {
                        if (nLen == 6)
                        {
                            if (Result.Substring(0, 2) != "?E")
                            {
                                throw new Exception(ProtocolName + "-" + Action + "-数据非法");
                            }
                            string Code = Result.Substring(2, 3);
                            if (Code == "000")
                            {
                                mBeamStatus = BeamStatus.BS_StopBeam;
                            }
                            else if(Code == "004")
                            {
                                mBeamStatus = BeamStatus.BS_OutputBeam;
                            }
                            else
                            {
                                throw new Exception(ProtocolName + "-" + Action + "-出束状态代码非法");
                            }
                        }
                        else
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-数据长度非法");
                        }
                    }
                    break;
                case GM_Command.GMC_InqurWorkStatus:
                    {
                        if (nLen == 6)
                        {
                            if (Result.Substring(0, 2) != "?E")
                            {
                                throw new Exception(ProtocolName + "-" + Action + "-数据非法");
                            }
                            string Code = Result.Substring(2, 3);
                            if (Code == "000" || Code == "002")
                            {
                                mWorkStatus = WorkStatus.WS_Ready;
                            }
                            else if (Code == "100" || Code == "101")
                            {
                                mWorkStatus = WorkStatus.WS_WaitWarmUp;
                            }
                            else
                            {
                                throw new Exception(ProtocolName + "-" + Action + "-工作状态代码非法");
                            }
                        }
                        else
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-数据长度非法");
                        }
                    }
                    break;
                case GM_Command.GMC_InqurWarmUpMode:
                    {
                        if (nLen == 6)
                        {
                            if (Result.Substring(0, 2) != "?E")
                            {
                                throw new Exception(ProtocolName + "-" + Action + "-数据非法");
                            }
                            string Code = Result.Substring(2, 3);
                            if (Code == "000")
                            {
                                mWarmUpMode = WarmUpMode.WUM_NotNeed;
                            }
                            else if (Code == "100")
                            {
                                mWarmUpMode = WarmUpMode.WUM_Short;
                            }
                            else if (Code == "101")
                            {
                                mWarmUpMode = WarmUpMode.WUM_Long;
                            }
                            else
                            {
                                throw new Exception(ProtocolName + "-" + Action + "-预热状态代码非法");
                            }
                        }
                        else
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-数据长度非法");
                        }
                    }
                    break;
                case GM_Command.GMC_InqurV:
                    {
                        if (nLen == 6)
                        {
                            if (Result.Substring(0, 2) != "?E")
                            {
                                throw new Exception(ProtocolName + "-" + Action + "-数据非法");
                            }
                            string Code = Result.Substring(2, 3);
                            mVoltage = Convert.ToInt32(Code);
                        }
                        else
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-数据长度非法");
                        }
                    }
                    break;
                case GM_Command.GMC_InqurI:
                    {
                        if (nLen == 6)
                        {
                            if (Result.Substring(0, 2) != "?E")
                            {
                                throw new Exception(ProtocolName + "-" + Action + "-数据非法");
                            }
                            string Code = Result.Substring(2, 3);
                            int I = Convert.ToInt32(Code);
                            mIcurent = I / 10.0f;
                        }
                        else
                        {
                            throw new Exception(ProtocolName + "-" + Action + "-数据长度非法");
                        }
                    }
                    break;
                default:
                    {
                        throw new Exception(ProtocolName + "-" + Action + "-未知错误");
                    }
            }

            NeedFeedBack = false;
            return true;
        }

        //查询错误码
        public bool InqurError(ref string sErrorCode)
        {
            LockSend();
            CommandType = GM_Command.GMC_InqurError;
            byte[] command = BuildCommand();
            bool bRet = SendCommand(command);
            if (bRet)
            {
                sErrorCode = ErrorCode;
                ConnIntf.DebugLog(FunctionName, "执行成功");
            }
            else
            {
                ConnIntf.DebugLog(FunctionName, "执行失败");
            }
            FunctionName = "";
            UnlockSend();
            return bRet;
        }
        //查询错误码命令
        public string InqurErrorCommand()
        {
            string sCommand = "";
            sCommand = "?E\r";
            return sCommand;
        }

        //查询出束状态
        public bool InqurBeamStatus(ref BeamStatus status)
        {
            LockSend();
            CommandType = GM_Command.GMC_InqurBeamStatus;
            byte[] command = BuildCommand();
            bool bRet = SendCommand(command);
            if (bRet)
            {
                status = mBeamStatus;
                ConnIntf.DebugLog(FunctionName, "执行成功");
            }
            else
            {
                ConnIntf.DebugLog(FunctionName, "执行失败");
            }
            FunctionName = "";
            UnlockSend();
            return bRet;
        }
        //查询出束状态命令
        public string InqurBeamStatusCommand()
        {
            string sCommand = "";
            sCommand = "?M\r";
            return sCommand;
        }

        //查询工作状态
        public bool InqurWorkStatus(ref WorkStatus status)
        {
            LockSend();
            CommandType = GM_Command.GMC_InqurWorkStatus;
            byte[] command = BuildCommand();
            bool bRet = SendCommand(command);
            if (bRet)
            {
                status = mWorkStatus;
                ConnIntf.DebugLog(FunctionName, "执行成功");
            }
            else
            {
                ConnIntf.DebugLog(FunctionName, "执行失败");
            }
            FunctionName = "";
            UnlockSend();
            return bRet;
        }
        //查询工作状态命令
        public string InqurWorkStatusCommand()
        {
            string sCommand = "";
            sCommand = "?W\r";
            return sCommand;
        }

        //查询预热模式
        public bool InqurWarmUpMode(ref WarmUpMode mode)
        {
            LockSend();
            CommandType = GM_Command.GMC_InqurWarmUpMode;
            byte[] command = BuildCommand();
            bool bRet = SendCommand(command);
            if (bRet)
            {
                mode = mWarmUpMode;
                ConnIntf.DebugLog(FunctionName, "执行成功");
            }
            else
            {
                ConnIntf.DebugLog(FunctionName, "执行失败");
            }
            FunctionName = "";
            UnlockSend();
            return bRet;
        }
        //查询预热模式命令
        public string InqurWarmUpModeCommand()
        {
            string sCommand = "";
            sCommand = "?P\r";
            return sCommand;
        }

        //查询电压
        public bool InqurVoltage(ref int voltage)
        {
            LockSend();
            CommandType = GM_Command.GMC_InqurV;
            byte[] command = BuildCommand();
            bool bRet = SendCommand(command);
            if (bRet)
            {
                voltage = mVoltage;
                ConnIntf.DebugLog(FunctionName, "执行成功");
            }
            else
            {
                ConnIntf.DebugLog(FunctionName, "执行失败");
            }
            FunctionName = "";
            UnlockSend();
            return bRet;
        }
        //查询电压命令
        public string InqurVoltageCommand()
        {
            string sCommand = "";
            sCommand = "?V\r";
            return sCommand;
        }

        //查询电流
        public bool InqurICurrent(ref float current)
        {
            LockSend();
            CommandType = GM_Command.GMC_InqurI;
            byte[] command = BuildCommand();
            bool bRet = SendCommand(command);
            if (bRet)
            {
                current = mIcurent;
                ConnIntf.DebugLog(FunctionName, "执行成功");
            }
            else
            {
                ConnIntf.DebugLog(FunctionName, "执行失败");
            }
            FunctionName = "";
            UnlockSend();
            return bRet;
        }
        //查询电流命令
        public string InqurICurrentCommand()
        {
            string sCommand = "";
            sCommand = "?I\r";
            return sCommand;
        }

        //设置预热模式
        public bool SetWarmUpMode(WarmUpMode mode)
        {
            LockSend();
            CommandType = GM_Command.GMC_SetWarmUpMode;
            mWarmUpMode = mode;
            byte[] command = BuildCommand();
            bool bRet = SendCommand(command);
            if (bRet)
            {
                ConnIntf.DebugLog(FunctionName, "执行成功");
            }
            else
            {
                ConnIntf.DebugLog(FunctionName, "执行失败");
            }
            FunctionName = "";
            UnlockSend();
            return bRet;
        }
        //设置预热模式命令
        public string SetWarmUpModeCommand(WarmUpMode mode)
        {
            if (mode == WarmUpMode.WUM_NotNeed)
            {
                throw new Exception("只能设置短预热模式或长预热模式");
            }
            string sCommand = "";
            string tmp = "";
            if (mode == WarmUpMode.WUM_Short)
            {
                tmp = "100";
            }
            else if (mode == WarmUpMode.WUM_Long)
            {
                tmp = "101";
            }
            sCommand = "!P" + tmp + "\r";
            return sCommand;
        }

        //设置电压
        public bool SetVoltage(int Voltage)
        {
            LockSend();
            byte[] command = BuildCommand();
            bool bRet = SendCommand(command);
            if (bRet)
            {
                ConnIntf.DebugLog(FunctionName, "执行成功");
            }
            else
            {
                ConnIntf.DebugLog(FunctionName, "执行失败");
            }
            FunctionName = "";
            UnlockSend();
            return bRet;
        }
        //设置电压命令
        public string SetVoltageCommand(int Voltage)
        {
            string sCommand = "";
            string tmp = Convert.ToString(Voltage).PadLeft(3, '0');
            sCommand = "!V" + tmp + "\r";
            return sCommand;
        }

        //设置电流
        public bool SetICurrent(float current)
        {
            LockSend();
            CommandType = GM_Command.GMC_SetI;
            mIcurent = current;
            byte[] command = BuildCommand();
            bool bRet = SendCommand(command);
            if (bRet)
            {
                ConnIntf.DebugLog(FunctionName, "执行成功");
            }
            else
            {
                ConnIntf.DebugLog(FunctionName, "执行失败");
            }
            FunctionName = "";
            UnlockSend();
            return bRet;
        }
        //设置电流命令
        public string SetICurrentCommand(float current)
        {
            string sCommand = "";
            string tmp = Convert.ToString(current * 10).PadLeft(3, '0');
            sCommand = "!I" + tmp + "\r";
            return sCommand;
        }

        //出束
        public bool OutputBeam()
        {
            LockSend();
            CommandType = GM_Command.GMC_OutputBeam;
            byte[] command = BuildCommand();
            bool bRet = SendCommand(command);
            if (bRet)
            {
                ConnIntf.DebugLog(FunctionName, "执行成功");
            }
            else
            {
                ConnIntf.DebugLog(FunctionName, "执行失败");
            }
            FunctionName = "";
            UnlockSend();
            return bRet;
        }
        //出束命令
        public string OutputBeamCommand()
        {
            string sCommand = "";
            sCommand = "!X\r";
            return sCommand;
        }

        //停止出束
        public bool StopBeam()
        {
            LockSend();
            byte[] command = BuildCommand();
            bool bRet = SendCommand(command);
            if (bRet)
            {
                ConnIntf.DebugLog(FunctionName, "执行成功");
            }
            else
            {
                ConnIntf.DebugLog(FunctionName, "执行失败");
            }
            FunctionName = "";
            UnlockSend();
            return bRet;
        }
        //停止出束命令
        public string StopBeamCommand()
        {
            string sCommand = "";
            sCommand = "!O\r";
            return sCommand;
        }
    }
}
