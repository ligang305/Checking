using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.ImageImportDll;
using CMW.Common.Utilities;
using BG_Entities;

namespace BG_Services
{
    /// <summary>
    /// 适用于快检扫描或同类扫描
    /// </summary>
    public class FastCheckScan : BaseScan
    {
        public override void Scan_Start()
        {
            IsScaning = true;
            BD_Scan();
        }

        public override void Scan_Stop()
        {
            CancelScan();
        }
        #region FastCheck快检的被动扫描图
        #region 被动模式给扫描站发送的指令
        /// <summary>
        /// 这是给扫描站发送开始扫描的指令
        /// </summary>
        /// <param name="_object"></param>
        private void recv(CallBackParamaterAction _object)
        {
            try
            {
                BuryingPoint($"Start BD Mode Scan ");
                do
                {
                    Thread.Sleep(10);
                    //该函数正在执行，但是当点击了停止扫描之后就跳出
                    //该函数正在执行，但是主被动扫描模式变化了之后就跳出
                    if (IsStopScan())
                    {
                        BuryingPoint($"Stop Scan at Get StartSignal");
                        IsScaning = false;
                        return;
                    }
                    //当发现状态为1且已经通知过扫描站 就不再进行通知扫描站了 ，然后跳出循环，直到判断停止扫图状态为1
                    if ((!IsReadyStartScanImage) && PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.StartImage))
                    {
                        SX_Start(intPtr);
                        BuryingPoint($"Get StartSignalSuccess in while");
                        IsReadyStartScanImage = true;
                        break;
                    }
                }
                //循环判断 开始采图的状态是否为0 如果开始采图的指令为true 那么给扫描站发送一个指令开始扫图
                while (!IsReadyStartScanImage && CheckSystemConditionAction());
                _object(StartSendDarkImage);
            }
            catch (Exception ex)
            {
                InitScanCondition();
                BGLogs.Log.GetDistance().WriteErrorLogs(ex.StackTrace);
            }
        }

        /// <summary>
        /// 当扫描站已经开始扫描了，这时候给扫描站发送停止扫描的操作/指令
        /// </summary>
        /// <param name="_object"></param>
        private void StartStopImage(CallBackAction _object)
        {
            try
            {
                BuryingPoint($"Waiting Bright Correction Signal ");
             
                do
                {
                    Thread.Sleep(100);

                    if (IsStopScan())
                    {
                        BuryingPoint($"Stop Scan at Waiting Bright Correction Signal ");
                        return;
                    }
                    //当发现PLC给了我开始采图信号之后，我就给PLC发送亮矫正的信号
                    if ((!IsReadyStopScanImage) && PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.StartAir))
                    {
                        BuryingPoint($"Send SX_SetLight Success Before");
                        SX_SetLight(intPtr);
                        Thread.Sleep(1000);
                        BuryingPoint($"Send SX_SetLight Success End");
                        PLCControllerManager.GetInstance().WritePositionValue(Common.CommandDic[Command.LightCollectionEnd], true);
                        Thread.Sleep(500);
                        PLCControllerManager.GetInstance().WritePositionValue(Common.CommandDic[Command.LightCollectionEnd], false);
                        IsReadyStopScanImage = true;
                        break;
                    }
                }
                //循环判断 开始采图的状态是否为0 如果开始采图的指令为true 那么给扫描站发送一个指令开始扫图
                while (!IsReadyStopScanImage && CheckSystemConditionAction());
                BuryingPoint($"Get Bright Correction Signal Success");
                _object();
                BuryingPoint($"CheckSystemConditionAction:{CheckSystemConditionAction()},IsScaning:{IsScaning}");
            }
            catch (Exception ex)
            {
                InitScanCondition();
                BGLogs.Log.GetDistance().WriteErrorLogs(ex.StackTrace);
            }
        }

        /// <summary>
        /// 停止扫描之后，接收到PLC给过来的指令，然后通过采集空气状态给扫描站发送暗矫正指令
        /// </summary>
        private void StartSendDarkImage()
        {
            try
            {
                BuryingPoint($"Wait to Get DarkCorrectionSignal");
                do
                {
                    Thread.Sleep(100);
                    if (IsStopScan())
                    {
                        BuryingPoint($"Stop Scan at GetDarkCorrectionSignal");
                        return;
                    }
                    //接收到开始采集暗矫正命令的时候
                    if ((!IsReadSendDarkInfo) && PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.StartDark))
                    {
                        BuryingPoint($"Send SX_SetDark Before");
                        SX_SetDark(intPtr);
                        BuryingPoint($"Send SX_SetDark End");
                        //这里给方海涛发送一个暗 / 亮矫正
                        IsReadSendDarkInfo = true;
                        Thread.Sleep(1000);
                        //本底才结束
                        PLCControllerManager.GetInstance().WritePositionValue(Common.CommandDic[Command.DarkCollectionEnd], true);
                        Thread.Sleep(500);
                        PLCControllerManager.GetInstance().WritePositionValue(Common.CommandDic[Command.DarkCollectionEnd], false);
                        BuryingPoint($"Get DarkCorrectionSignal Successfully");
                        break;
                    }
                }
                //循环判断 开始采图的状态是否为0 如果开始采图的指令为true 那么给扫描站发送一个指令开始扫图
                while (!IsReadSendDarkInfo && CheckSystemConditionAction());

                BuryingPoint($"Scan End");
                Thread.Sleep(100);
                RequestTaskAndStop();
                InitScanCondition();
                BuryingPoint($"InitScanCondition");
                Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                InitScanCondition();
                BGLogs.Log.GetDistance().WriteErrorLogs(ex.StackTrace);
            }
        }

        #endregion
        #endregion
        /// <summary>
        /// 系统出错停止扫描
        /// </summary>
        private void CancelScan()
        {
            IsScaning = false;
            Common.StopScan();
            CancelCallBack?.Invoke();
            RequestTaskAndStop();
        }
        public override void BD_Scan()
        {
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
            UnActiveTask = Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(1);
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                    //并且把IsClicking置为false
                    if (IsStopScan())
                    {
                        CancelScan();
                        ClearScanImageCallBack?.Invoke();
                        return;
                    }
                    InitScanCondition();
                    PLCControllerManager.GetInstance().WritePositionValue(Common.CommandDic[Command.StartScan], true);
                    recv(StartStopImage);
                }
            });
        }
        /// <summary>
        /// 调用ImageX之前请求一下任务
        /// </summary>
        private void RequestTaskAndStop()
        {
            BuryingPoint($"RequestTask  End ！");
            SX_Stop(intPtr);
            RequestTaskAction();
        }
    }
}
