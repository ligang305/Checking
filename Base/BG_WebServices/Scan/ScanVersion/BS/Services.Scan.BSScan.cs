using CMW.Common.Utilities;
using BGModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.ImageImportDll;
using BG_Entities;

namespace BG_Services
{
    [Export("Scan", typeof(BaseScan))]
    [CustomExportMetadata(1, "BSScan", "CGN. Begood", "zhuzhiwu", "1.0.0")]
    public class BSScan : BaseScan
    {
        /// <summary>
        /// 开始扫描
        /// </summary>
        public override void Scan_Start()
        {
            IsScaning = true;
          
            BD_Scan();
            
        }
        /// <summary>
        /// 停止扫描
        /// </summary>
        public override void Scan_Stop()
        {
            Stop_Scan();
        }
        /// <summary>
        /// 系统出错停止扫描
        /// </summary>
        private void Stop_Scan()
        {
            IsScaning = false;
            logCallBack?.Invoke("Stop BS Scanning successfully ", LogType.ScanStep, true);
            BuryingPoint($"{UpdateStatusNameAction("StopBSScaningSuccessfully")}");
            CommonDeleget.ImageXStartAndStop(0);
            InitScanCondition();
            CancelCallBack?.Invoke();
        }
        /// <summary>
        /// 背散只有一种扫描模式
        /// </summary>
        public override void BD_Scan()
        {
            CommonDeleget.ImageXSetMode(0);
            CommonDeleget.ImageXSetDirection(BSPLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.ScanForWard) ? 0 : 1);
            Task.Run(() =>
            {
                while (true) 
                {
                    Thread.Sleep(10);
                    //并且把IsClicking置为false
                    if (IsStopScan() || !IsBSSystemCondition())
                    {
                        Scan_Stop();
                        BuryingPoint($"{UpdateStatusNameAction("EmergencySystemPause")}");
                        return;
                    }
                    ReceiveStartSignal(ReceiveStopSignal);
                }
            });
        }
        protected override void InitScanCondition()
        {
            try
            {
                //这一步相当于整个流程结束了 把初始值设置为false;等待下一个点击按钮
                IsReadyStartScanImage = false;
                IsReadSendDarkInfo = false;
                IsReadyStopScanImage = false;
                BSPLCControllerManager.GetInstance().WritePositionValue(BSPLCControllerManager.GetInstance().BSCommandDic[Command.StartScan], false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 判断由于 暂停扫描、网络断连引起的中止扫描的操作
        /// </summary>
        /// <returns></returns>
        protected override bool IsStopScan()
        {
            return !IsScaning  || !BSPLCControllerManager.GetInstance().IsConnect();
        }

        protected bool IsBSSystemCondition()
        {
            return BSPLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.MainSystemReady);
        }

        #region 被动模式给扫描站发送的指令
        /// <summary>
        /// 这是给扫描站发送开始扫描的指令
        /// </summary>
        /// <param name="_object"></param>
        [HandleProcessCorruptedStateExceptions]
        private void ReceiveStartSignal(CallBackAction _object)
        {
            BuryingPoint(UpdateStatusNameAction("Initiacanning"));
            InitScanCondition();
            BuryingPoint(UpdateStatusNameAction("SendStartScanSignal") + "SX_Start");
            var result = BSPLCControllerManager.GetInstance().WritePositionValue(BSPLCControllerManager.GetInstance().BSCommandDic[Command.StartScan], true);
            BuryingPoint($"{UpdateStatusNameAction("SendStartScanSignalCompleted")}{BSPLCControllerManager.GetInstance().BSCommandDic[Command.StartScan]};result:{result}");
            BuryingPoint($"Start BS");
            do
            {
                Thread.Sleep(5);
                //该函数正在执行，但是当点击了停止扫描之后就跳出
                if (IsStopScan())
                {
                    BuryingPoint($"BSEquipment Interrupt ReceiveStartSignal,Because IsScaning:{IsScaning},BSIsConnection:{BSPLCControllerManager.GetInstance().IsConnect()}");
                    break; // BuryingPoint
                }
                //当发现状态为1且已经通知过扫描站 就不再进行通知扫描站了 ，然后跳出循环，直到判断停止扫图状态为1
                if ((!IsReadyStartScanImage) && BSPLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.StartImage))
                {
                    CommonDeleget.ImageXStartAndStop(1);
                    BuryingPoint($"Get StartImage Signal in while");
                    IsReadyStartScanImage = true;
                    break;
                }
            }
            //循环判断 开始采图的状态是否为0 如果开始采图的指令为true 那么给扫描站发送一个指令开始扫图
            while (!IsReadyStartScanImage && IsBSSystemCondition());

            BuryingPoint($"Start Signal Success");
            _object();
        }

        /// <summary>
        /// 停止扫描之后，接收到PLC给过来的指令，然后通过采集空气状态给扫描站发送暗矫正指令
        /// </summary>
        [HandleProcessCorruptedStateExceptions]
        private void ReceiveStopSignal()
        {
            try
            {
                BuryingPoint($"Start Receive Stop Signal");
                do
                {
                    Thread.Sleep(5);
                    if (IsStopScan())
                    {
                        BuryingPoint($"BS Scan Interrupt ReceiveStopSignal,Because IsScaning:{IsScaning} ,IsGotoConnnection:{IsGotoConnnection},IsConnection:{IsConnection}");
                        return;
                    }

                    //接收到停止扫描信号
                    if ((!IsReadSendDarkInfo) && BSPLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.StopImageStr))
                    {
                        //这里给方海涛发送停止信号
                        IsReadSendDarkInfo = true;
                        CommonDeleget.ImageXStartAndStop(0);
                        break;
                    }
                }
                //循环判断 是否满足扫描的安全条件具体见CheckSystemConditionAction()
                while (!IsReadSendDarkInfo && IsBSSystemCondition()); 

                if (!(IsBSSystemCondition() && IsScaning))
                {
                    BuryingPoint($"{UpdateStatusNameAction("ScanningZD")}");
                    Scan_Stop();
                    logCallBack?.Invoke($"{UpdateStatusNameAction("ScanningZD")}", LogType.ScanStep, true);
                    return;
                }
                BuryingPoint($"{UpdateStatusNameAction("ScanningEnd")}!");
                InitScanCondition();
                Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                CommonDeleget.HandTaskException(ex);
            }
        }

        #endregion
    }
}
