using BG_Entities;
using BG_Services;
using BG_WorkFlow;
using BGCommunication;
using BGLogs;
using BGModel;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonFunc;
using static CMW.Common.Utilities.CommonDeleget;
namespace BG_Services
{
    /// <summary>
    /// 这个主控PLC主要用于 H986的PLC控制
    /// </summary>
    public class PLCControllerManager :  BaseInstance<PLCControllerManager>,IPLCEquipment
    {
        public List<Bg_ControlVersion> ControlVersionList = new List<Bg_ControlVersion>();
        BasePLCEquipment pLCEquipment;
        public Action<PLCDBStatus> HandwareParamaterCallback = null;
        public Action PlcConnectionCallback = null;
        ControlVersionsBLL _ControlVersionsBLL = new ControlVersionsBLL();
        public PLCDBStatus PLCDBStatus = new PLCDBStatus();
        public PLCControllerManager()
        {
            int plcType = LoadPLCModules();
            switch (plcType)
            {
                case 1:
                    pLCEquipment = new CSharpPLCEquipment();
                    break;
                case 2:
                    pLCEquipment = new CPlusPlusPLCEquipment();
                    break;
                case 3:
                    pLCEquipment = new CPlusPlusPLCForS300Equipment();
                    break;
                default:
                    pLCEquipment = new CSharpPLCEquipment();
                    break;
            }
            pLCEquipment.PlcConnectionCallback += PlcConnection_Callback;
            pLCEquipment.HandwareParamaterCallback += HandwareParamater_Callback;
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        public bool Connect(string IpAddress, string Port)
        {
            return pLCEquipment.Connect(IpAddress, Port);
        }
     
        /// <summary>
        /// 判断连接状态
        /// </summary>
        /// <returns></returns>
        public bool IsConnect()
        {
            return pLCEquipment.IsConnect();
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        public void DisConnect()
        {
           pLCEquipment.DisConnect();
        }
        /// <summary>
        /// 接收报文
        /// </summary>
        /// <returns></returns>
        public bool ReceviceByte()
        {
            return pLCEquipment.ReceviceByte();
        }
        /// <summary>
        /// 处理报文逻辑给业务 
        /// </summary>
        /// <returns></returns>
        public bool HandleByteToBussiness()
        {
            return pLCEquipment.HandleByteToBussiness();
        }
        /// <summary>
        /// 查询点位状态
        /// </summary>
        /// <returns></returns>
        public bool InquirePositionStatus()
        {
            return pLCEquipment.InquirePositionStatus();
        }

        /// <summary>
        /// 查询硬件参数
        /// </summary>
        /// <returns></returns>
        public bool InquireHardwareStatus()
        {
            return pLCEquipment.InquireHardwareStatus();
        }
   
        /// <summary>
        /// 读取点位的状态-返回String读取
        /// </summary>
        /// <param name="Postion"></param>
        /// <param name="StartPositon"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public string ReadPositionValue(string Postion, uint StartPositon, uint length)
        {
            return pLCEquipment.ReadPositionValue(Postion, StartPositon, length);
        }

        /// <summary>
        /// 直接读取
        /// </summary>
        /// <returns></returns>
        public bool ReadPositionValue(string positionItem)
        {
            return pLCEquipment.ReadPositionValue(positionItem);
        }
        /// <summary>
        /// 将值写入到具体点位
        /// </summary>
        /// <param name="Position"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public bool WritePositionValue(string Position, bool Value)
        {
            return pLCEquipment.WritePositionValue(Position, Value);
        }
        public bool WritePositionValue(string Position, ushort Value)
        {
            return pLCEquipment.WritePositionValue(Position, Value);
        }
        public bool WritePositionValue(byte Position, ushort Value)
        {
            return pLCEquipment.WritePositionValue(Position, Value);
        }
        public bool WritePositionValue(string Position, UInt32 Value)
        {
            return pLCEquipment.WritePositionValue(Position, Value);
        }
        public bool WritePositionValue(byte Position, UInt32 Value)
        {
            return pLCEquipment.WritePositionValue(Position, Value);
        }
        public bool WritePositionValue(string Position, float Value)
        {
            return pLCEquipment.WritePositionValue(Position, Value);
        }
        public bool WritePositionValue(byte Position, float Value)
        {
            return pLCEquipment.WritePositionValue(Position, Value);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cv"></param>
        /// <param name="commonProtocol"></param>
        public void Load(ControlVersion cv, PLCProtocol commonProtocol)
        {
            pLCEquipment.Load(cv, commonProtocol);
        }

        public void UnLoad()
        {
            pLCEquipment.UnLoad();
        }
        /// <summary>
        /// 加载适配的PLC型号
        /// </summary>
        int LoadPLCModules()
        {
            try
            {
                ControlVersionList = _ControlVersionsBLL.GetControlVersionsBLLDataModel();
                string ModuleNames = ControlVersionList.FirstOrDefault(q => q.ControlversionKey == ConfigServices.GetInstance().localConfigModel.CMW_Version)?.PlcEquipment;
                return ModuleNames == "CPlusPlus" ? 2 : ModuleNames == "CPlusPlusS300"? 3: 1;
            }
            catch (Exception ex)
            {
                CommonDeleget.WriteLogAction(ex.StackTrace, LogType.ApplicationError, true);
            }
            return 1;
        }
        public string GetPlcBlockStr(string BlockPosition, ushort Strlength, InqureType _InqureType = InqureType.IString)
        {
            return pLCEquipment.GetPlcBlockStr(BlockPosition, Strlength, _InqureType);
        }
        ///通过枚举获取点位状态
        public bool GetStatusByPositionEnum(PLCPositionEnum pLCPositionEnum)
        {
            return pLCEquipment.GetStatusByPositionEnum(pLCPositionEnum);
        }
        ///通过枚举获取点位状态
        public bool GetStatusByPositionEnum(string plcPositionEnuStr)
        {
            return pLCEquipment.GetStatusByPositionEnum(plcPositionEnuStr);
        }
        public short GetStatusByDIntPositionEnum(PLCPositionEnum plcPositionEnu)
        {
            return pLCEquipment.GetStatusByDIntPositionEnum(plcPositionEnu);
        }
        public short GetStatusByDIntPositionEnum(string plcPositionEnuStr)
        {
            return pLCEquipment.GetStatusByDIntPositionEnum(plcPositionEnuStr);
        }

        public float GetStatusByFloatPositionEnum(PLCPositionEnum plcPositionEnu)
        {
            return pLCEquipment.GetStatusByFloatPositionEnum(plcPositionEnu);
        }
        public float GetStatusByFloatPositionEnum(string plcPositionEnuStr)
        {
            return pLCEquipment.GetStatusByFloatPositionEnum(plcPositionEnuStr);
        }

        public int GetStatusBUIntPositionEnum(PLCPositionEnum plcPositionEnu)
        {
            return pLCEquipment.GetStatusBUIntPositionEnum(plcPositionEnu);
        }
        public int GetStatusBUIntPositionEnum(string plcPositionEnuStr)
        {
            return pLCEquipment.GetStatusBUIntPositionEnum(plcPositionEnuStr);
        }
        /// <summary>
        /// 检测系统条件是否就绪了
        /// </summary>
        /// <returns></returns>
        public bool CheckSystemCondition()
        {
            return pLCEquipment.CheckSystemCondition();
        }

        public string GetCurrentEquipmentVersion()
        {
            return pLCEquipment.GetCurrentEquipmentVersion();
        }

        public string GetCurrentEquipmentModel()
        {
            return pLCEquipment.GetCurrentEquipmentModel();
        }

        public string GetRunStatus()
        {
            if (GetStatusByPositionEnum(PLCPositionEnum.RunningState)) { return "RunningState"; }
            else if (GetStatusByPositionEnum(PLCPositionEnum.MaintenanceStatus)) { return "MaintenanceStatus"; }
            else { return "Null"; }
        }

        public PLCDBStatus GetPLCDBStatus()
        {
            return pLCEquipment.PLCDBStatus;
        }

        public void PlcConnection_Callback()
        {
            PlcConnectionCallback?.Invoke();
        }
        public void HandwareParamater_Callback(PLCDBStatus _PLCDBStatus)
        {
            PLCDBStatus = _PLCDBStatus;
            HandwareParamaterCallback?.Invoke(_PLCDBStatus);
        }
    }
}
