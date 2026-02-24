using BG_Entities;
using BGCommunication;
using BGModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG_Services
{
    public abstract class BasePLCEquipment : IPLCEquipment
    {
        public List<Bg_ControlVersion> ControlVersionList = new List<Bg_ControlVersion>();
        public Action<PLCDBStatus> HandwareParamaterCallback;
        public Action PlcConnectionCallback;
        public PLCDBStatus PLCDBStatus = new PLCDBStatus();
        public List<bool> _BSGlobalRetStatus = new bool[128].ToList();
        public bool PlcIsInit = false;
        public bool IsConnection = false;
        /// <summary>
        /// 全局变量 用来本地存储128个状态值
        /// </summary>
        public List<bool> BSGlobalRetStatus
        {
            get { return _BSGlobalRetStatus; }
            set
            {
                _BSGlobalRetStatus = value;
            }
        }

        public List<ushort> BSGlobalDoseStatus = new List<ushort>(20)
        {
              0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
        };
        public virtual bool CheckSystemCondition()
        {
            return true;
        }
        public virtual bool Connect(string IpAddress, string Port)
        {
            throw new NotImplementedException();
        }

        public virtual void DisConnect()
        {
            throw new NotImplementedException();
        }

        public virtual string GetCurrentEquipmentModel()
        {
            throw new NotImplementedException();
        }

        public virtual string GetCurrentEquipmentVersion()
        {
            throw new NotImplementedException();
        }

        public virtual string GetPlcBlockStr(string BlockPosition, ushort Strlength, InqureType _InqureType = InqureType.IString)
        {
            throw new NotImplementedException();
        }

        public virtual bool GetStatusByPositionEnum(PLCPositionEnum pLCPositionEnum)
        {
            throw new NotImplementedException();
        }

        public virtual bool GetStatusByPositionEnum(string plcPositionEnuStr)
        {
            throw new NotImplementedException();
        }
        public virtual short GetStatusByDIntPositionEnum(PLCPositionEnum pLCPositionEnum)
        {
            throw new NotImplementedException();
        }
        public virtual short GetStatusByDIntPositionEnum(string plcPositionEnuStr)
        {
            throw new NotImplementedException();
        }
        public virtual float GetStatusByFloatPositionEnum(PLCPositionEnum pLCPositionEnum)
        {
            throw new NotImplementedException();
        }
        public virtual float GetStatusByFloatPositionEnum(string plcPositionEnuStr)
        {
            throw new NotImplementedException();
        }
        public virtual int GetStatusBUIntPositionEnum(PLCPositionEnum pLCPositionEnum)
        {
            throw new NotImplementedException();
        }
        public virtual int GetStatusBUIntPositionEnum(string plcPositionEnuStr)
        {
            throw new NotImplementedException();
        }

        
        public virtual bool HandleByteToBussiness()
        {
            throw new NotImplementedException();
        }

        public virtual bool InquireHardwareStatus()
        {
            throw new NotImplementedException();
        }

        public virtual bool InquirePositionStatus()
        {
            throw new NotImplementedException();
        }

        public virtual bool IsConnect()
        {
            throw new NotImplementedException();
        }

        public virtual void Load(ControlVersion cv, PLCProtocol commonProtocol)
        {
            throw new NotImplementedException();
        }

        public virtual string ReadPositionValue(string Postion, uint StartPositon, uint length)
        {
            throw new NotImplementedException();
        }

        public virtual bool ReadPositionValue(string positionItem)
        {
            throw new NotImplementedException();
        }

        public virtual bool ReceviceByte()
        {
            throw new NotImplementedException();
        }

        public virtual void UnLoad()
        {
            throw new NotImplementedException();
        }

        public virtual bool WritePositionValue(string Position, bool Value)
        {
            throw new NotImplementedException();
        }
        public virtual bool WritePositionValue(string Position, ushort Value)
        {
            throw new NotImplementedException();
        }
        public virtual bool WritePositionValue(byte Position, ushort Value)
        {
            throw new NotImplementedException();
        }
        public virtual bool WritePositionValue(string Position, uint Value)
        {
            throw new NotImplementedException();
        }
        public virtual bool WritePositionValue(byte Position, uint Value)
        {
            throw new NotImplementedException();
        }
        public virtual bool WritePositionValue(string Position, float Value)
        {
            throw new NotImplementedException();
        }
        public virtual bool WritePositionValue(byte Position, float Value)
        {
            throw new NotImplementedException();
        }
    }
}
