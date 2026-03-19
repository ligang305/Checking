using BG_Services;
using CMW.Common.Utilities;
using BGModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BG_Entities;
using static CMW.Common.Utilities.Common;

namespace CMW.Boosting.BegoodBoostingController
{
    [Export("Boosting", typeof(IEquipment))]
    [CustomExportMetadata(1, "BegoodBoosting_Module", "CGN. Begood", "zhuzhiwu", "1.0.0")]
    public class BegoodBoosting : BoostingController<BegoodBoosting>
    {
        public BegoodBoosting()
        {

        }
        /// <summary>
        /// 出束 
        /// </summary>
        public override void Ray()
        {
            //SetCommand(CommandDic[Command.StartHighVoltage], true);
            SetCommand(CommandDic[Command.StopRay], true);
        }
        /// <summary>
        /// 停束
        /// </summary>
        public override void StopRay()
        {
            SetCommand(CommandDic[Command.StopRay], false);
        }
        /// <summary>
        /// 预热中
        /// </summary>
        public override bool WarmUp()
        {
            return false;
        }
        /// <summary>
        /// 预热毕
        /// </summary>
        public override bool WarmUpEnd()
        {
            return false;
        }
        /// <summary>
        /// 预热毕
        /// </summary>
        public override string GetRayAndPreviewHot()
        {
            if (PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.PreheatEnding))//Common.GlobalRetStatus[18]
            {
                return "PreheatEnding";
            }
            if (PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.Preheating))//Common.GlobalRetStatus[17]
            {
                return "Preheating";
            }
            return "UnPreheat";
        }
        /// <summary>
        /// 读取高低能
        /// </summary>
        /// <returns></returns>
        public override string ReadDoubleOrSingle()
        {

            if (PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.DualEnergyIndication)) { return "DoubleEnergy"; }//Common.GlobalRetStatus[37]
            else return "SingleEnergyIndication";
        }
        /// <summary>
        /// 读取能量
        /// </summary>
        /// <returns></returns>
        public override string ReadEnger()
        {
            if (PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.DualEnergyIndication)) { return "DoubleEnergy"; }//Common.GlobalRetStatus[37]
            else if (PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.HighEnergyIndication)) { return "HeightEnergy"; }//Common.GlobalRetStatus[36]
            else { return "LowerEnergy"; }
        }
        public override string SearchScanMode()
        {
            return PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.ScanMode) ? "PassiveMode" : "InitiativeMode";//Common.GlobalRetStatus[53] 
        }
        /// <summary>
        /// 是否正在出束
        /// </summary>
        /// <returns></returns>
        public override bool IsRayOut()
        {
            return PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.OutOfBeam);
        }
        /// <summary>
        /// 复位
        /// </summary>
        /// <returns></returns>
        public override bool Reset()
        {
            return false;
        }
        /// <summary>
        /// 设置高低能
        /// </summary>
        /// <param name="SetOrCancel">true--高能；false--低能</param>
        /// <returns></returns>
        public override bool SetHighOrLowEnergy(bool SetOrCancel)
        {
            return SetCommand(CommandDic[Command.SettingHighLow], SetOrCancel);
        }
        /// <summary>
        /// 设置单双能
        /// </summary>
        /// <param name="SetOrCancel">true--双能；false--单能</param>
        /// <returns></returns>
        public override bool SetDoubleOrSigneEnergy(bool SetOrCancel)
        {
            return SetCommand(CommandDic[Command.SettingSingleDouble], SetOrCancel);
        }
        /// <summary>
        /// 设置高低压
        /// </summary>
        /// <param name="SetHightOrLowPressure">true--高压；false--低压</param>
        /// <returns></returns>
        public override bool SetHighAndLowPressure(bool SetHightOrLowPressure)
        {
            return SetCommand(CommandDic[Command.StartHighVoltage], SetHightOrLowPressure);
        }

        public override string GetCurrentEquipmentModel()
        {
            return "BegoodBoosting";
        }
        public override string GetCurrentEquipmentVersion()
        {
            return "1.0.0";
        }
    }
}
