using BG_Entities;
using BGModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;

namespace BG_Services
{
    public class ScanConditionService: BaseInstance<ScanConditionService>
    {
        /// <summary>
        /// 系统就绪的事件
        /// </summary>
        public Action<bool> SystemConditionEvent;

        /// <summary>
        /// 传达系统就绪状态
        /// </summary>
        /// <param name="IsSystemReady"></param>
        public void PassSystemCondition(bool IsSystemReady)
        {
            SystemConditionEvent?.Invoke(IsSystemReady);
        }

        /// <summary>
        /// 服务启动
        /// </summary>
        public void Start()
        {
           
        }
        /// <summary>
        /// 服务暂停
        /// </summary>
        public void Stop()
        {

        }
        public bool CheckSafeCondition()
        {
            if(controlVersion == ControlVersion.BS)
            {
                return BSSafeCondition();
            }
            else
            {
                return H986SafeCondition();
            }
        }

        private bool H986SafeCondition()
        {
            try
            {
                if (!PLCControllerManager.GetInstance().IsConnect())
                {
                    return false;
                }
                if (!IsScanCanScan())
                {
                    return false;
                }

                //判断扫描条件是否就绪
                if (!CheckSystemConditionAction())
                {
                    return false;
                }

                //判断 是否符合扫描状态
                if (StartScanCondition())
                {
                    //如果是软件重启，但是还是检测到这些状态，那么就发送主动扫描为false
                    if (!IsReadSendDarkInfo && !IsReadyStartScanImage && !IsReadyStopScanImage)
                    {
                        PLCControllerManager.GetInstance().WritePositionValue(CommandDic[Command.StartScan], false);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLogAction(ex.StackTrace, LogType.ApplicationError);
            }
            return true;
        }

        private bool BSSafeCondition()
        {
            if (!PLCControllerManager.GetInstance().IsConnect())
            {
                return false;
            }

            return PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.MainSystemReady);
        }
    }
}
