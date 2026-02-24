using BG_Entities;
using BG_WorkFlow;
using BGCommunication;
using BGModel;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonFunc;
using static CMW.Common.Utilities.CommonDeleget;
using BGLogs;
using System.IO;
using static BGCommunication.PLCClientCallback;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Runtime.Remoting.Contexts;
using BG_Services.EquipmentServices.Controller;

namespace BG_Services
{
    public class CPlusPlusPLCEquipment : CPlusCPlusEquipment
    {
        Dictionary<PLCPositionEnum, int> PlcPositionEnum = new Dictionary<PLCPositionEnum, int>();
        Dictionary<PLCPositionEnum, int> PlcBoolPositionEnum = new Dictionary<PLCPositionEnum, int>();
        Dictionary<PLCPositionEnum, int> PlcShortPositionEnum = new Dictionary<PLCPositionEnum, int>();
        Dictionary<PLCPositionEnum, int> PlcIntPositionEnum = new Dictionary<PLCPositionEnum, int>();
        Dictionary<PLCPositionEnum, int> PlcFloatPositionEnum = new Dictionary<PLCPositionEnum, int>();

        ObservableCollection<StatusModel> statusModels = new ObservableCollection<StatusModel>();
        public CPlusPlusPLCEquipment() : base()
        {
            CommonDeleget.CheckSystemConditionEvent += CheckSystemCondition;
            CommonDeleget.SetCommondEvent += WritePositionValue;
            CommonDeleget.SetDoseValueEvent += WritePositionValue;
        }
        /// <summary>
        /// 载入点位配置状态
        /// </summary>
        /// <param name="cv"></param>
        void LoadPositionConfig(ControlVersion cv)
        {
            statusModels = HitChModelBLL.GetInstance().GetHitModelList(cv);
            PlcPositionEnum = PLCTools.GetInstance().StatusModelConvertTo(statusModels.ToList());

            var OPositionStatusModels = PositionBLL.GetInstance().GetPositionModel(cv,PositionConfigType.OPosition);
            PlcBoolPositionEnum =  PLCTools.GetInstance().StatusModelConvertTo(OPositionStatusModels.ToList());

            var IntPositionStatusModels = PositionBLL.GetInstance().GetPositionModel(cv, PositionConfigType.IntPosition);
            PlcShortPositionEnum = PLCTools.GetInstance().StatusModelConvertTo(IntPositionStatusModels.ToList());

            var DIntPositionStatusModels = PositionBLL.GetInstance().GetPositionModel(cv, PositionConfigType.DIntPosition);
            PlcIntPositionEnum = PLCTools.GetInstance().StatusModelConvertTo(DIntPositionStatusModels.ToList());

            var FloatPositionStatusModels = PositionBLL.GetInstance().GetPositionModel(cv, PositionConfigType.FloatPosition);
            PlcFloatPositionEnum = PLCTools.GetInstance().StatusModelConvertTo(FloatPositionStatusModels.ToList());
        }
        public override void Load(ControlVersion cv, PLCProtocol commonProtocol)
        {
            LoadPositionConfig(cv);
            base.Load(cv, commonProtocol); 
        }

        /// <summary>
        /// 查询剂量
        /// </summary>
        protected override void InquireDose(byte[] DoseBytes)
        {
            ushort[] doseShorts = new ushort[50 / 2];
            for (int i = 0; i < 50; i+=2)
            {
                ushort Dose = (ushort)((DoseBytes[i] << 8) + DoseBytes[i+1]);
                doseShorts[i / 2] = Dose;
            }
            if(doseShorts[9] != Common.GlobalDoseStatus[9])
            {
                Log.GetDistance().WriteInfoLogs($"InquireDose: new value{doseShorts[9]},old Value{Common.GlobalDoseStatus[9]}");
            }
            Common.GlobalDoseStatus = doseShorts.ToList();
        }
        /// <summary>
        /// 查询点位
        /// </summary>
        protected override void InquireBool(byte[] BoolBytes)
        {
          
            bool[] batchBool = new bool[BoolBytes.Length * 8];
            if (BoolBytes.Length * 8 < 128) return;
            for (int i = 0; i < BoolBytes.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    byte n = (byte)Math.Pow(2, j);
                    int Ret = (BoolBytes[i] & n);
                    bool PositionBool = (Ret == 0 ? false : true);
                    batchBool[i*8 + j] = PositionBool;
                }
            }
            for (int index = 0; index < BoolBytes.Length * 8; index++)
            {
                if (GlobalRetStatus[index] != batchBool[index])
                {
                    //if(index == 114)
                    {
                        Log.GetDistance().WriteInfoLogs($"InquirePositionStatus: Index:{index},value:{ batchBool[index]},Postition:M{(index / 8 + 1)}.{index % 8}");
                    }
                    GlobalRetStatus[index] = batchBool[index];
                }
            }

           
        }

        /// <summary>
        /// 检测系统条件是否就绪了
        /// </summary>
        /// <returns></returns>
        public override bool CheckSystemCondition()
        {
            if (Common.controlVersion == ControlVersion.FastCheck)
            {
                return GetStatusByPositionEnum(PLCPositionEnum.MainSystemReady) &&
                       GetStatusByPositionEnum(PLCPositionEnum.RadiationSourceOrAcceleratorReady) &&
                       (GetStatusByPositionEnum(PLCPositionEnum.SafetyInterlockReady) || isSafeSerices) &&
                       Common.IsScanCanScan();
                WriteLogAction($"PLCControllManager MainSystemReady:{GetStatusByPositionEnum(PLCPositionEnum.MainSystemReady)};RadiationSourceOrAcceleratorReady:{GetStatusByPositionEnum(PLCPositionEnum.RadiationSourceOrAcceleratorReady)};" +
                    $"SafetyInterlockReady:{GetStatusByPositionEnum(PLCPositionEnum.SafetyInterlockReady)};isSafeSerices:{isSafeSerices};Common.IsScanCanScan({Common.IsScanCanScan()}", LogType.ScanStep);
            }
            if (Common.controlVersion == ControlVersion.CombinedMovementBetatron)
            {
                return GetStatusByPositionEnum(PLCPositionEnum.MainSystemReady) &&
                       GetStatusByPositionEnum(PLCPositionEnum.DriveReady) &&
                       (GetStatusByPositionEnum(PLCPositionEnum.SafetyInterlockReady) &&
                       BoostingControllerManager.GetInstance().IsReady() || isSafeSerices) && Common.IsScanCanScan();
            }
            if (Common.controlVersion == ControlVersion.PassengerCar)
            {
                return GetStatusByPositionEnum(PLCPositionEnum.MainSystemReady) &&
                    GetStatusByPositionEnum(PLCPositionEnum.RadiationSourceOrAcceleratorReady) &&
                    GetStatusByPositionEnum(PLCPositionEnum.TransportUnit) &&
                    GetStatusByPositionEnum(PLCPositionEnum.StopReady) &&
                    (GetStatusByPositionEnum(PLCPositionEnum.SafetyInterlockReady) || isSafeSerices) && Common.IsScanCanScan();
            }
            return GetStatusByPositionEnum(PLCPositionEnum.MainSystemReady) &&
                    GetStatusByPositionEnum(PLCPositionEnum.RadiationSourceOrAcceleratorReady) &&
                    (GetStatusByPositionEnum(PLCPositionEnum.SafetyInterlockReady) || isSafeSerices) && Common.IsScanCanScan();
        }

        ///通过枚举获取点位状态
        public override bool GetStatusByPositionEnum(PLCPositionEnum pLCPositionEnum)
        {
            if (PlcPositionEnum.ContainsKey(pLCPositionEnum))
            {
                return Common.GlobalRetStatus[PlcPositionEnum[pLCPositionEnum]];
            }
            else
            {
                return false;
            }
        }
        ///通过枚举获取点位状态
        public override bool GetStatusByPositionEnum(string plcPositionEnuStr)
        {
            PLCPositionEnum pLCPositionEnum = PLCPositionEnum.AcceleratorDoorLimit1;
            Enum.TryParse(plcPositionEnuStr, out pLCPositionEnum);
            if (PlcPositionEnum.ContainsKey(pLCPositionEnum))
            {
                return Common.GlobalRetStatus[PlcPositionEnum[pLCPositionEnum]];
            }
            else
            {
                return false;
            }
        }
        public override short GetStatusByDIntPositionEnum(PLCPositionEnum pLCPositionEnum)
        {
            if (PlcShortPositionEnum.ContainsKey(pLCPositionEnum))
            {
                if(PLCDBStatus.IntArray.Count > PlcShortPositionEnum[pLCPositionEnum])
                {
                    return PLCDBStatus.IntArray[PlcShortPositionEnum[pLCPositionEnum]];
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
        ///通过枚举获取点位状态
        public override short GetStatusByDIntPositionEnum(string plcPositionEnuStr)
        {
            PLCPositionEnum pLCPositionEnum = PLCPositionEnum.AcceleratorDoorLimit1;
            Enum.TryParse(plcPositionEnuStr, out pLCPositionEnum);
            if (PlcShortPositionEnum.ContainsKey(pLCPositionEnum))
            {
                if (PLCDBStatus.IntArray.Count > PlcShortPositionEnum[pLCPositionEnum])
                {
                    return PLCDBStatus.IntArray[PlcShortPositionEnum[pLCPositionEnum]];
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        public override float GetStatusByFloatPositionEnum(PLCPositionEnum pLCPositionEnum)
        {
            if (PlcFloatPositionEnum.ContainsKey(pLCPositionEnum))
            {
                if (PLCDBStatus.FloatArray.Count > PlcFloatPositionEnum[pLCPositionEnum])
                {
                    return PLCDBStatus.FloatArray[PlcFloatPositionEnum[pLCPositionEnum]];
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0.0f;
            }
        }
        ///通过枚举获取点位状态
        public override float GetStatusByFloatPositionEnum(string plcPositionEnuStr)
        {
            PLCPositionEnum pLCPositionEnum = PLCPositionEnum.AcceleratorDoorLimit1;
            Enum.TryParse(plcPositionEnuStr, out pLCPositionEnum);
            if (PlcFloatPositionEnum.ContainsKey(pLCPositionEnum))
            {
                if (PLCDBStatus.FloatArray.Count > PlcFloatPositionEnum[pLCPositionEnum])
                {
                    return PLCDBStatus.FloatArray[PlcFloatPositionEnum[pLCPositionEnum]];
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0f;
            }
        }

        public override int GetStatusBUIntPositionEnum(PLCPositionEnum pLCPositionEnum)
        {
            if (PlcIntPositionEnum.ContainsKey(pLCPositionEnum))
            {
                if (PLCDBStatus.DIntArray.Count > PlcIntPositionEnum[pLCPositionEnum])
                {
                    return PLCDBStatus.DIntArray[PlcIntPositionEnum[pLCPositionEnum]];
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
        ///通过枚举获取点位状态
        public override int GetStatusBUIntPositionEnum(string plcPositionEnuStr)
        {
            PLCPositionEnum pLCPositionEnum = PLCPositionEnum.AcceleratorDoorLimit1;
            Enum.TryParse(plcPositionEnuStr, out pLCPositionEnum);
            if (PlcIntPositionEnum.ContainsKey(pLCPositionEnum))
            {
                if (PLCDBStatus.DIntArray.Count > PlcIntPositionEnum[pLCPositionEnum])
                {
                    return PLCDBStatus.DIntArray[PlcIntPositionEnum[pLCPositionEnum]];
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
    }
}
