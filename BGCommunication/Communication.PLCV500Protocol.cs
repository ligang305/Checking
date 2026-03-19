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
   
    public abstract class PLCV500Protocol : PLCProtocol
    {
        public PLCV500Protocol()
        {
            ProtocolName = "Control station -PLCV500 protocol";
            PLCConnectType = ConnectType.CT_TouchScreen;
            InitSystemTypeParas();
        }
        ~PLCV500Protocol()
        {
            Console.WriteLine("Structure--" + ProtocolName);
        }

        public override bool InqurMByte1(ref List<bool> StatusRet, ref List<ushort> dose)
        {
            LockSend();
            CommandType = SendInfoType.SIT_InqurMByte1;
            #if DEBUG
                InqurMByteCnt =  100;
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
        public override bool InqurStr(byte blockType, int blockNum, int blockOffset, ushort[] length)
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
        public override bool InqurHardwareByte1(byte blockType, int blockNum, int blockOffset, ushort[] length, ref List<Byte[]> StatusRet)
        {
            return InqureParamater(blockType, blockNum, blockOffset, length);
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
        //生成发送命令
    }
}
