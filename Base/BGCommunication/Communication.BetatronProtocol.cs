using BGLogs;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BGCommunication
{
    /// <summary>
    /// 俄罗斯Betratron 电子回旋加速器协议
    /// </summary>
    public class BetatronProtocol : ICommonProtocol
    {
        private static Dictionary<WorkingJob, string> WJS = new Dictionary<WorkingJob, string>
        {
            { WorkingJob.WJ_StopBeam,"Stop beam" },
            { WorkingJob.WJ_WarmUp,"warm up" },
            { WorkingJob.WJ_OutBeam,"Out of beam" }
        };
        public static Dictionary<WorkingJob, string> WorkingJobStr { get { return WJS; } }

        public BetatronProtocol()
        {
            ProtocolName = "Control station -Betatron accelerator protocol";
            mTI = new TandI();
            mTI.T = new byte[4] { 0, 0, 0, 0 };
            mTI.DI = new DeviceI();
            mTI.DI.Iset = 0;
            mTI.DI.Inow = 0;
            mDI = new DeviceI();
            mHAndL = new HAndL();
            mDI.Iset = 0;
            mDI.Inow = 0;
            mDR = new DoseRate();
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
                case CommandFun.CF_InqurStatus:
                    {
                        mWorkingJob = WorkingJob.WJ_StopBeam;
                        NeedFeedBack = false;
                    }
                    break;
                case CommandFun.CF_InqurIandT:
                    {
                        //mTI.T = new byte[4] { 0, 0, 0, 0 };
                        //mTI.DI.Iset = 0;
                        //mTI.DI.Inow = 0;
                        NeedFeedBack = true;
                    }
                    break;
                case CommandFun.CF_InqurDose:
                    {
                        //mDR.InternalDose = 0;
                        //mDR.RemoteDose = 0;
                        //mDR.CircuitMaxEnergy = 0;
                        //mDR.Doserate = 0;
                        NeedFeedBack = true;
                    }
                    break;
                case CommandFun.CF_StopBeam:
                    {
                        NeedFeedBack = true;
                    }
                    break;
                case CommandFun.CF_WarmUp:
                    {
                        NeedFeedBack = true;
                    }
                    break;
                case CommandFun.CF_SetI:
                    {
                        mDI.Iset = 0;
                        mDI.Inow = 0;
                        NeedFeedBack = true;
                    }
                    break;
                case CommandFun.CF_OutputBeam:
                    {
                        NeedFeedBack = false;
                    }
                    break;
            }
        }

        private CommandFun mFun = CommandFun.CF_InqurStatus;
        public WorkingJob mWorkingJob = WorkingJob.WJ_StopBeam;
        public DeviceI mDI;
        public HAndL mHAndL;
        public TandI mTI;
        public DoseRate mDR;

        //设置命令类型
        protected CommandFun CommandFunction
        {
            get { return mFun; }
            set
            {
                mFun = value;
                switch (mFun)
                {
                    case CommandFun.CF_InqurStatus:
                        {
                            FunctionName = "Query status";
                        }
                        break;
                    case CommandFun.CF_InqurIandT:
                        {
                            FunctionName = "Query temperature and current";
                        }
                        break;
                    case CommandFun.CF_InqurDose:
                        {
                            FunctionName = "Query dose rate";
                        }
                        break;
                    case CommandFun.CF_StopBeam:
                        {
                            FunctionName = "Stop beam";
                        }
                        break;
                    case CommandFun.CF_WarmUp:
                        {
                            FunctionName = "warm up";
                        }
                        break;
                    case CommandFun.CF_SetI:
                        {
                            FunctionName = "Setting current";
                        }
                        break;
                    case CommandFun.CF_OutputBeam:
                        {
                            FunctionName = "Out of beam";
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
                case CommandFun.CF_InqurStatus:
                    {
                        Command = InqurStatusCommand();
                    }
                    break;
                case CommandFun.CF_InqurIandT:
                    {
                        Command = InqurIandTCommand();
                    }
                    break;
                case CommandFun.CF_InqurDose:
                    {
                        Command = InqurDoseCommand();
                    }
                    break;
                case CommandFun.CF_StopBeam:
                    {
                        Command = StopBeamCommand();
                    }
                    break;
                case CommandFun.CF_WarmUp:
                    {
                        Command = WarmUpCommand();
                    }
                    break;
                case CommandFun.CF_SetI:
                    {
                        Command = ExecuteCommand(mDI.Iset);
                    }
                    break;
                case CommandFun.CF_OutputBeam:
                    {
                        Command = new byte[4] { 0x01, 0x02, 0x00, 0x03 };
                    }
                    break;
                case CommandFun.CF_ReSet:
                    {
                        Command = new byte[4] { 0x01, 0x03, 0x00, 0x04 };
                    }
                    break;
                case CommandFun.CF_SetHAndL:
                    {
                        Command = ExecuteHAndLCommand(mHAndL.H, mHAndL.HMC, mHAndL.L, mHAndL.LMC);
                        ConnIntf.WriteError("ExecuteHAndLCommand Command :", ICommonProtocol.ToHexString(Command), true);
                    }
                    break;
            }
            Log.GetDistance().WriteErrorLogs($"Russial_SetWaitMachineStopMode WatiByteArray Send {FunctionName}:{ICommonProtocol.ToHexString(Command)}");
            return Command;
        }
        byte[] waitReceive = null;
        int hasReceive = 0;

        byte[] waitDoseReceive = null;
        int hasDoseReceive = 0;
        //接收回传数据处理函数
        internal override void OnRecv(byte[] buffer, int size)
        {
            //if (NeedFeedBack)
            //{
                NeedFeedBack_ResolveBackCommand(buffer, size);
            //}
            //else
            //{
            //    UnNeedFeedBack_ResolveCommand(buffer, size);
            //}
        }
        //解析发送命令后的回传信息
        internal override bool ResolveBackCommand(byte[] buffer, int size)
        {
            try
            {
                string Action = "【" + FunctionName + "】Return data";

                if (ConnIntf.SendedBuf == null)
                {
                    throw new Exception(ProtocolName + "-" + Action + "-Unknown error");
                }
                Log.GetDistance().WriteErrorLogs($"Russial_SetWaitMachineStopMode WatiByteArray repose :{ICommonProtocol.ToHexString(buffer)}");
                switch (CommandFunction)
                {
                    case CommandFun.CF_InqurStatus:
                        {
                            if (buffer.Length != 5)
                            {
                                if (waitDoseReceive == null)
                                {
                                    waitDoseReceive = new byte[5];
                                }
                                if (hasDoseReceive + buffer.Length > 5)
                                {
                                    var tempBytes = buffer.Take(5 - hasDoseReceive).ToArray();
                                    tempBytes.CopyTo(waitDoseReceive, hasDoseReceive);
                                    hasDoseReceive = 5;
                                }
                                else
                                {
                                    buffer.CopyTo(waitDoseReceive, hasDoseReceive);
                                    hasDoseReceive += buffer.Length;
                                }
                            }
                            else
                            {
                                hasDoseReceive = 5;
                                waitDoseReceive = buffer;
                            }
                            Log.GetDistance().WriteErrorLogs($"Russial_SetWaitMachineStopMode WatiByteArray repose CF_InqurStatus:{ICommonProtocol.ToHexString(waitDoseReceive)}");
                            if (hasDoseReceive == 5)
                            {
                                buffer = waitDoseReceive;
                            }
                            else
                            {
                                ExeResult = true;
                                NeedFeedBack = true;
                                return true;
                            }

                            buffer = waitDoseReceive;
                            waitDoseReceive = null;
                            hasDoseReceive = 0;

                            if (buffer.Length != 5)
                            {
                                return true;
                            }
                            ushort count = 0;
                            for (int i = 0; i < buffer.Length - 1; i++)
                            {
                                count += buffer[i];
                            }
                            byte temp = (byte)(count & 0xff);
                            if (buffer[4] == temp)
                            {
                                Log.GetDistance().WriteErrorLogs($"Russial_SetWaitMachineStopMode WatiByteArray repose CF_InqurStatus:{ICommonProtocol.ToHexString(buffer)}");
                                NeedFeedBack = false;
                                mWorkingJob = (WorkingJob)buffer[1];
                                ExeResult = true;
                            }
                            else
                            {
                                NeedFeedBack = false;
                                ExeResult = true;
                            }
                        }
                        break;
                    case CommandFun.CF_InqurIandT:
                        {
                            if (buffer.Length != 11)
                            {
                                if (waitReceive == null)
                                {
                                    waitReceive = new byte[11];
                                }
                                if (hasReceive + buffer.Length > 11)
                                {
                                    var tempBytes = buffer.Take(11 - hasReceive).ToArray();
                                    tempBytes.CopyTo(waitReceive, hasReceive);
                                    hasReceive = 11;
                                }
                                else
                                {
                                    buffer.CopyTo(waitReceive, hasReceive);
                                    hasReceive += buffer.Length;
                                }
                            }
                            else
                            {
                                hasReceive = 11;
                                waitReceive = buffer;
                            }
                            Log.GetDistance().WriteErrorLogs($"Russial_SetWaitMachineStopMode WatiByteArray repose CF_InqurIandT:{ICommonProtocol.ToHexString(waitReceive)}");
                            if (hasReceive == 11)
                            {
                                buffer = waitReceive;
                            }
                            else
                            {
                                ExeResult = true;
                                NeedFeedBack = true;
                                return true;
                            }

                            buffer = waitReceive;
                            waitReceive = null;
                            hasReceive = 0;

                            if (buffer.Length != 11)
                            {
                                return true;
                            }

                         
                            bool IsCheck = false;
                            var tempValue = 0x00;
                            for (int i = 0; i < buffer.Length - 2; i++)
                            {
                                tempValue += buffer[i];
                            }
                            IsCheck = (tempValue == buffer[9] * 256 + buffer[10]);
                            List<byte> byteList = new List<byte>() { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10 };
                            if (buffer[0] != 0x0E || !byteList.Contains(0x00))
                            {
                                return true;
                            }
                            mWorkingJob = (WorkingJob)buffer[1];
                            mTI.H = buffer[2];//代表有故障的编号
                            mTI.T[0] = buffer[3];
                            mTI.T[1] = buffer[4];
                            mTI.T[2] = buffer[5];
                            mTI.T[3] = buffer[6];
                            mTI.DI.Iset = buffer[7];
                            mTI.DI.Inow = buffer[8];
                            NeedFeedBack = false;
                            ExeResult = true;
                        }
                        break;
                    case CommandFun.CF_InqurDose:
                            {
                            
                            if (buffer.Length != 11)
                            {
                                if (waitDoseReceive == null)
                                {
                                    waitDoseReceive = new byte[11];
                                }
                                if (hasDoseReceive + buffer.Length > 11)
                                {
                                    var tempBytes = buffer.Take(11 - hasDoseReceive).ToArray();
                                    tempBytes.CopyTo(waitDoseReceive, hasDoseReceive);
                                    hasDoseReceive = 11;
                                }
                                else
                                {
                                    buffer.CopyTo(waitDoseReceive, hasDoseReceive);
                                    hasDoseReceive += buffer.Length;
                                }
                            }
                            else
                            {
                                hasDoseReceive = 11;
                                waitDoseReceive = buffer;
                            }
                            Log.GetDistance().WriteErrorLogs($"Russial_SetWaitMachineStopMode WatiByteArray repose CF_InqurDose:{ICommonProtocol.ToHexString(waitDoseReceive)}");
                            if (hasDoseReceive == 11)
                            {
                                buffer = waitDoseReceive;
                            }
                            else
                            {
                                ExeResult = true;
                                NeedFeedBack = true;
                                return true;
                            }

                            buffer = waitDoseReceive;
                            waitDoseReceive = null;
                            hasDoseReceive = 0;
                       
                            if (buffer.Length != 11)
                                {
                                    return true;
                                }

                                ushort count = 0;
                                for (int i = 0; i < buffer.Length - 2; i++)
                                {
                                    count += buffer[i];
                                }
                                int real = buffer[9] * 256 + buffer[10];

                                if (buffer[0] != 0x11 || real != count)
                                {
                                    return true;
                                }

                                mDR.InternalDose = (UInt16)((buffer[1] << 8) + buffer[2]);
                                mDR.RemoteDose = (UInt16)((buffer[3] << 8) + buffer[4]);
                                mDR.CircuitMaxEnergy = (UInt16)((buffer[5] << 8) + buffer[6]);
                                mDR.Doserate = (UInt16)((buffer[7] << 8) + buffer[8]);
                                ExeResult = true;
                                NeedFeedBack = false;
                            }
                            break;
                    case CommandFun.CF_WarmUp:
                        {
                            if (buffer.Length != 5)
                            {
                                if (waitDoseReceive == null)
                                {
                                    waitDoseReceive = new byte[5];
                                }
                                if (hasDoseReceive + buffer.Length > 5)
                                {
                                    var tempBytes = buffer.Take(5 - hasDoseReceive).ToArray();
                                    tempBytes.CopyTo(waitDoseReceive, hasDoseReceive);
                                    hasDoseReceive = 5;
                                }
                                else
                                {
                                    buffer.CopyTo(waitDoseReceive, hasDoseReceive);
                                    hasDoseReceive += buffer.Length;
                                }
                            }
                            else
                            {
                                hasDoseReceive = 5;
                                waitDoseReceive = buffer;
                            }
                            Log.GetDistance().WriteErrorLogs($"Russial_SetWaitMachineStopMode WatiByteArray repose CF_WarmUp:{ICommonProtocol.ToHexString(buffer)}");
                            if (hasDoseReceive == 5)
                            {
                                buffer = waitDoseReceive;
                            }
                            else
                            {
                                ExeResult = true;
                                NeedFeedBack = true;
                                return true;
                            }

                            buffer = waitDoseReceive;
                            waitDoseReceive = null;
                            hasDoseReceive = 0;

                            if (buffer.Length != 5)
                            {
                                return true;
                            }
                           
                            byte[] ret = new byte[5] { 0x0D, 0x00, 0x00, 0x00, 0x0D };
                            for (int i = 0; i < size; i++)
                            {
                                if (buffer[i] != ret[i])
                                {
                                   return false;
                                }
                            }
                            NeedFeedBack = false;
                            ExeResult = true;
                        }
                        break;
                    case CommandFun.CF_SetI:
                        {

                            if (buffer.Length != 11)
                            {
                                if (waitDoseReceive == null)
                                {
                                    waitDoseReceive = new byte[11];
                                }
                                if (hasDoseReceive + buffer.Length > 11)
                                {
                                    var tempBytes = buffer.Take(11 - hasDoseReceive).ToArray();
                                    tempBytes.CopyTo(waitDoseReceive, hasDoseReceive);
                                    hasDoseReceive = 11;
                                }
                                else
                                {
                                    buffer.CopyTo(waitDoseReceive, hasDoseReceive);
                                    hasDoseReceive += buffer.Length;
                                }
                            }
                            else
                            {
                                hasDoseReceive = 11;
                                waitDoseReceive = buffer;
                            }
                            Log.GetDistance().WriteErrorLogs($"Russial_SetWaitMachineStopMode WatiByteArray repose CF_SetI:{ICommonProtocol.ToHexString(waitDoseReceive)}");
                            if (hasDoseReceive == 11)
                            {
                                buffer = waitDoseReceive;
                            }
                            else
                            {
                                ExeResult = true;
                                NeedFeedBack = true;
                                return true;
                            }

                            buffer = waitDoseReceive;
                            waitDoseReceive = null;
                            hasDoseReceive = 0;

                            if (buffer.Length != 11)
                            {
                                return true;
                            }
                        
                            byte[] ret = new byte[7] { 0x0E, 0x00, 0x00, 0xAF, 0xA9, 0xB3, 0xB5 };
                            for (int i = 0; i < 7; i++)
                            {
                                if (buffer[i] != ret[i])
                                {
                                    return false;
                                }
                            }
                            if (buffer[9] != 0x03 || buffer[10] != 0x12)
                            {
                                return false;
                            }
                            mDI.Iset = buffer[7];
                            mDI.Inow = buffer[8];
                            ExeResult = true;
                            NeedFeedBack = false;
                        }
                        break;
                    case CommandFun.CF_OutputBeam:
                        {
                            if (buffer.Length != 5)
                            {
                                if (waitDoseReceive == null)
                                {
                                    waitDoseReceive = new byte[5];
                                }
                                if (hasDoseReceive + buffer.Length > 5)
                                {
                                    var tempBytes = buffer.Take(5 - hasDoseReceive).ToArray();
                                    tempBytes.CopyTo(waitDoseReceive, hasDoseReceive);
                                    hasDoseReceive = 5;
                                }
                                else
                                {
                                    buffer.CopyTo(waitDoseReceive, hasDoseReceive);
                                    hasDoseReceive += buffer.Length;
                                }
                            }
                            else
                            {
                                hasDoseReceive = 5;
                                waitDoseReceive = buffer;
                            }
                            if (hasDoseReceive == 5)
                            {
                                buffer = waitDoseReceive;
                            }
                            else
                            {
                                ExeResult = true;
                                NeedFeedBack = true;
                                return true;
                            }

                            buffer = waitDoseReceive;
                            waitDoseReceive = null;
                            hasDoseReceive = 0;

                            if (buffer.Length != 5)
                            {
                                return true;
                            }
                           
                            Log.GetDistance().WriteErrorLogs($"Russial_SetWaitMachineStopMode WatiByteArray repose CF_OutputBeam:{ICommonProtocol.ToHexString(buffer)}");
                            byte[] ret = new byte[5] { 0x0D, 0x80, 0x00, 0x00, 0x8D};
                            for (int i = 0; i < size; i++)
                            {
                                if (buffer[i] != ret[i])
                                {
                                   return false;
                                }
                            }
                            ExeResult = true;
                            NeedFeedBack = false;
                        }
                        break;
                    case CommandFun.CF_StopBeam:
                        {

                            if (buffer.Length != 11)
                            {
                                if (waitDoseReceive == null)
                                {
                                    waitDoseReceive = new byte[11];
                                }
                                if (hasDoseReceive + buffer.Length > 11)
                                {
                                    var tempBytes = buffer.Take(11 - hasDoseReceive).ToArray();
                                    tempBytes.CopyTo(waitDoseReceive, hasDoseReceive);
                                    hasDoseReceive = 11;
                                }
                                else
                                {
                                    buffer.CopyTo(waitDoseReceive, hasDoseReceive);
                                    hasDoseReceive += buffer.Length;
                                }
                            }
                            else
                            {
                                hasDoseReceive = 11;
                                waitDoseReceive = buffer;
                            }
                            if (hasDoseReceive == 11)
                            {
                                buffer = waitDoseReceive;
                            }
                            else
                            {
                                ExeResult = true;
                                NeedFeedBack = true;
                                return true;
                            }

                            buffer = waitDoseReceive;
                            waitDoseReceive = null;
                            hasDoseReceive = 0;

                            if (buffer.Length != 11)
                            {
                                return true;
                            }
                           
                            Log.GetDistance().WriteErrorLogs($"Russial_SetWaitMachineStopMode WatiByteArray repose CF_StopBeam:{ICommonProtocol.ToHexString(buffer)}");
                            byte[] ret = new byte[11] { 0x0E, 0x00, 0x00, 0xB0, 0xA9, 0xB2, 0xB5, 0x44, 0x00, 0x03, 0x13 };
                            for (int i = 0; i < size; i++)
                            {
                                if (buffer[i] != ret[i])
                                {
                                    NeedFeedBack = false;
                                }
                            }
                            ExeResult = true;
                            NeedFeedBack = false;
                        }
                        break;

                }

            }
            catch (Exception ex)
            {
                NeedFeedBack = false;
            }
            NeedFeedBack = false;
            return true;
        }
        /// <summary>
        /// 查询当前工作状态命令
        /// </summary>
        /// <returns></returns>
        public byte[] InqurStatusCommand()
        {
            byte[] command = null;
            command = new byte[3] { 0x0D, 0x00, 0x0D };
            return command;
        }
        /// <summary>
        /// 查询电流温度命令
        /// </summary>
        /// <returns></returns>
        public byte[] InqurIandTCommand()
        {
            byte[] command = null;
            command = new byte[3] { 0x0E, 0x00, 0x0E };
            return command;
        }
        /// <summary>
        /// 查询剂量率命令
        /// </summary>
        /// <returns></returns>
        public byte[] InqurDoseCommand()
        {
            byte[] command = null;
            command = new byte[3] { 0x11, 0x00, 0x11 };
            return command;
        }

        /// <summary>
        /// 停止出束命令
        /// </summary>
        /// <returns></returns>
        public byte[] StopBeamCommand()
        {
            byte[] command = null;
            command = new byte[4] { 0x01, 0x00, 0x00, 0x01 };
            return command;
        }
        /// <summary>
        /// 预热命令
        /// </summary>
        /// <returns></returns>
        public byte[] WarmUpCommand()
        {
            byte[] command = null;
            command = new byte[4] { 0x01, 0x01, 0x00, 0x02 };
            return command;
        }
        /// <summary>
        /// 出束命令
        /// </summary>
        /// <returns></returns>
        public byte[] OutputBeamCommand()
        {
            byte[] command = null;
            command = new byte[4] { 0x01, 0x02, 0x00, 0x03 };
            return command;
        }




        /// <summary>
        /// 查询当前工作状态
        /// </summary>
        /// <param name="wj"></param>
        /// <returns></returns>
        public bool InqurStatus(ref WorkingJob wj)
        {
            LockSend();
            CommandFunction = CommandFun.CF_InqurStatus;
            byte[] command = BuildCommand();
            string str = string.Empty;
            foreach (var item in command)
            {
                str += " " + item;
            }
            ConnIntf.DebugLog("BetratronProttocol InqurStatus:", str);
            bool bRet = SendCommand(command);
            if (bRet)
            {
                wj = mWorkingJob;
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
        /// 查询电流温度
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public bool InqurIandT(ref TandI ti, ref WorkingJob wjb)
        {
            LockSend();
            CommandFunction = CommandFun.CF_InqurIandT;
            byte[] command = BuildCommand();
            waitReceive = null;
            hasReceive = 0;
            bool bRet = SendCommand(command);
            if (bRet)
            {
                ti = mTI;
                wjb = mWorkingJob;
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
        /// 查询剂量率
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public bool InqurDose(ref DoseRate dr)
        {
            LockSend();
            CommandFunction = CommandFun.CF_InqurDose;
            byte[] command = BuildCommand();
            waitDoseReceive = null;
            hasDoseReceive = 0;
            bool bRet = SendCommand(command);
            if (bRet)
            {
                dr = mDR;
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
        /// 停止出束
        /// </summary>
        /// <returns></returns>
        public bool StopBeam()
        {
            LockSend();
            CommandFunction = CommandFun.CF_StopBeam;
            byte[] command = BuildCommand();
            Log.GetDistance().WriteErrorLogs($"Russial_SetWaitMachineStopMode WatiByteArray Send {FunctionName}:{ICommonProtocol.ToHexString(command)}");
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
        /// 复位
        /// </summary>
        /// <returns></returns>
        public bool Reset()
        {
            LockSend();
            CommandFunction = CommandFun.CF_ReSet;
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
        /// 出束
        /// </summary>
        /// <returns></returns>
        public bool OutputBeam()
        {
            LockSend();
            CommandFunction = CommandFun.CF_OutputBeam;
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
        /// 预热
        /// </summary>
        /// <returns></returns>
        public bool WarmUp()
        {
            Debug.WriteLine("-------------------------ganggang send command-----------------------data0");


            LockSend();
            Debug.WriteLine("-------------------------ganggang send command-----------------------data1");


            CommandFunction = CommandFun.CF_WarmUp;
            byte[] command = BuildCommand();
            Log.GetDistance().WriteErrorLogs($"Russial_SetWaitMachineStopMode WatiByteArray Send {FunctionName}:{ICommonProtocol.ToHexString(command)}");
            Debug.WriteLine("-------------------------ganggang send command-----------------------data--" + command[0] + command[1]);

            bool bRet = SendCommand(command);
            Debug.WriteLine("-------------------------ganggang send command-----------------------bRet" + bRet);

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
        /// 设置电流
        /// </summary>
        /// <param name="Iset"></param>
        /// <param name="di"></param>
        /// <returns></returns>
        public bool Execute(int Iset, ref DeviceI di)
        {
            LockSend();
            CommandFunction = CommandFun.CF_SetI;
            mDI.Iset = BitConverter.GetBytes(Iset)[0];
            mDI.Inow = 0;
            byte[] command = BuildCommand();
            bool bRet = SendCommand(command);
            if (bRet)
            {
                di = mDI;
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
        /// 设置电流
        /// </summary>
        /// <param name="Iset"></param>
        /// <param name="di"></param>
        /// <returns></returns>
        public bool ExecuteHandI(int H, int HMC, int L, int LMC)
        {
            LockSend();
            CommandFunction = CommandFun.CF_SetHAndL;
            mHAndL.H = H; mHAndL.HMC = HMC; mHAndL.L = L; mHAndL.LMC = LMC;
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

        //设置电流命令
        public byte[] ExecuteCommand(byte Iset)
        {
            byte[] Temp = new byte[4];
            byte by = Iset;
            byte[] command = new byte[2] { 0x04, by };
            byte[] bt = new byte[2];
            ushort count = 0;
            for (int i = 0; i < command.Length; i++)
            {
                count += command[i];
            }
            int count2 = count / 256 + count % 256 * 256;
            byte[] last = BitConverter.GetBytes(count2);
            command.CopyTo(Temp, 0);
            last.Take(2).ToArray().CopyTo(Temp, 2);
            command[1] = (byte)Iset;
            return Temp;
        }

        //设置电流命令
        /// <summary>
        /// 
        /// </summary>
        /// <param name="H"></param>
        /// <param name="HMC"></param>
        /// <param name="L"></param>
        /// <param name="LMC"></param>
        /// <returns></returns>
        public byte[] ExecuteHAndLCommand(int H, int HMC, int L, int LMC)
        {
            byte[] command = new byte[12];
            byte he = BitConverter.GetBytes(H)[0];
            byte LE = BitConverter.GetBytes(L)[0];
            byte HP = BitConverter.GetBytes(HMC)[0];
            byte LP = BitConverter.GetBytes(LMC)[0];
            byte[] TempCommand = new byte[10] { 0x03, 0x27, 0x0F, 0x27, 0x0F, he, LE, HP, LP, 0x00 };

            ushort count = 0;
            for (int i = 0; i < TempCommand.Length; i++)
            {
                count += TempCommand[i];
            }
            int count2 = count / 256 + count % 256 * 256;

            byte[] last = BitConverter.GetBytes(count2);

            TempCommand.CopyTo(command, 0);
            last.Take(2).ToArray().CopyTo(command, 10);

            return command;
        }

    }
}
