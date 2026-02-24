using CMW.Common.Utilities;
using BGCommunication;
using BGModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
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
    [CustomExportMetadata(1, "FH_BetratronScan", "CGN. Begood", "zhuzhiwu", "1.0.0")]
    /// <summary>
    /// 适用于自行走扫描或同类扫描
    /// </summary>
    public class FHBetratronScan : BaseScan
    {
        public override void Scan_Start()
        {
            IsScaning = true;
            if (!PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.ScanMode))
            {
                BD_Scan();
            }
            else
            {
                Task.Run(() =>
                {
                    ZD_Scan();
                });
            }
        }

        public override void Scan_Stop()
        {
            Task.Run(() => { BoostingControllerManager.GetInstance().SendCommandWaitWorkStop(); });
            
            CancelScan();
        }
        #region 给扫描站发送扫描指令

        #region 被动模式给扫描站发送的指令
        /// <summary>
        /// 这是给扫描站发送开始扫描的指令
        /// </summary>
        /// <param name="_object"></param>
        private void recv(CallBackAction _object)
        {
            BuryingPoint(UpdateStatusNameAction("Passivescanning"));
            BuryingPoint(UpdateStatusNameAction("Startsampling"));
            do
            {
                //该函数正在执行，但是当点击了停止扫描之后就跳出
                //该函数正在执行，但是主被动扫描模式变化了之后就跳出
                if (IsInative() || IsStopScan())
                {
                    BuryingPoint(UpdateStatusNameAction("StopAcquisitionScanning"));
                    break;
                }
                //当发现状态为1且已经通知过扫描站 就不再进行通知扫描站了 ，然后跳出循环，直到判断停止扫图状态为1
                if ((!IsReadyStartScanImage) && PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.StartImage))
                {
                    IsReadyStartScanImage = true;

                    BoostingControllerManager.GetInstance().SendCommandRadiationOn();

                    SX_Start(intPtr);

                    break;
                }
            }
            //循环判断 开始采图的状态是否为0 如果开始采图的指令为true 那么给扫描站发送一个指令开始扫图
            while (!IsReadyStartScanImage && CheckSystemConditionAction());

            _object();
            BuryingPoint(UpdateStatusNameAction("StartSignalSuccess"));


            if (CheckSystemConditionAction() && IsScaning)
            {
                
            }
            else
            {
                BuryingPoint(UpdateStatusNameAction("StopScanning"));
                CancelScan();
                return;
            }
        }

        /// <summary>
        /// 当扫描站已经开始扫描了，这时候给扫描站发送停止扫描的操作/指令
        /// </summary>
        /// <param name="_object"></param>
        private void StartBrightSignleAndDarkStopImage()
        {
            try
            {
                BuryingPoint($"{UpdateStatusNameAction("BrightCorrectionStart")}！");
                RequestTaskAction();
                BuryingPoint($"RequestTask  End ！");
                do
                {
                    Thread.Sleep(100);
                    if (IsStopScan())
                    {
                        BuryingPoint($"{UpdateStatusNameAction("StopScaningInAcquiring")}");
                        return;
                    }
                    //当发现PLC给了我开始采图信号之后，我就给PLC发送亮矫正的信号
                    if ((!IsReadyStopScanImage) && PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.StartAir))
                    {
                        IsReadyStopScanImage = true;
                        BuryingPoint($"{UpdateStatusNameAction("BrightCorrectionStart")}");
                        SX_SetLight(intPtr);
                        Thread.Sleep(200);
                        BuryingPoint($"{UpdateStatusNameAction("BrightCorrectionEnd")}");
                        PLCControllerManager.GetInstance().WritePositionValue(Common.CommandDic[Command.LightCollectionEnd], true);
                        Thread.Sleep(100);
                        PLCControllerManager.GetInstance().WritePositionValue(Common.CommandDic[Command.LightCollectionEnd], false);
                      
                        break;
                    }
                }
                //循环判断 开始采图的状态是否为0 如果开始采图的指令为true 那么给扫描站发送一个指令开始扫图
                while (!IsReadyStopScanImage && CheckSystemConditionAction());
                BuryingPoint($"{UpdateStatusNameAction("BrightCorrectionSuccessfully")}");

                BuryingPoint($"Send 预热指令给FH_加速器使其停束");
                BoostingControllerManager.GetInstance().SendCommandWaitWorkStop();
                Thread.Sleep(2000);
                BuryingPoint($"{UpdateStatusNameAction("DarkCorrectionStart")}");
                SX_SetDark(intPtr);
                BuryingPoint($"{UpdateStatusNameAction("DarkCorrectionEnd")}");
                BuryingPoint($"{UpdateStatusNameAction("ScanningEnd")}!");
                Thread.Sleep(100);
                SX_Stop(intPtr);
                if (CheckSystemConditionAction() && IsScaning)
                {
                   
                }
                else
                {
                    BuryingPoint($"{UpdateStatusNameAction("ScanningZD")}");
                    Scan_Stop();
                    return;
                }
                InitScanCondition(); // 105*11 + 600 + 35 = 1155+635=1790
            }
            catch (Exception ex)
            {
                CommonDeleget.HandTaskException(ex);
            }
        }

       
        #endregion

        #region 主动模式给扫描站发送的指令
        /// <summary>
        /// 这是给扫描站发送开始扫描的指令
        /// </summary>
        /// <param name="_object"></param>
        private void InitiaRecv(CallBackAction _object)
        {
            RequestTaskAction();
            InitScanCondition();
            BuryingPoint(UpdateStatusNameAction("Initiacanning"));
            Ray();
            BuryingPoint(UpdateStatusNameAction("SendBrightSignal") + "SetLight");
            if (!IsScaning) return;
            BuryingPoint($@"{UpdateStatusNameAction("SendStartScanSignal")},{CommandDic[Command.StartScan]}");
            var result = SetCommand(CommandDic[Command.StartScan], true);
            BuryingPoint($"{UpdateStatusNameAction("SendStartScanSignalCompleted")}{CommandDic[Command.StartScan]};result:{result}");
            do
            {
                //该函数正在执行，但是当点击了停止扫描之后就跳出
                //当第一步来了之后，判断顺序有没有乱，就判断第二步有没有优于第一步先到，
                //后续添加防撞报警|| GlobalRetStatus[55] || GlobalRetStatus[46]
                if (!IsInative() || IsStopScan())
                {
                    BuryingPoint($"{UpdateStatusNameAction("StopAcquisitionScanning")}");
                    IsScaning = false;
                    Thread.Sleep(100);
                    Common.StopScan();
                    CancelCallBack?.Invoke();
                    SetCommand(CommandDic[Command.StartScan], false);
                    BoostingControllerManager.GetInstance().SendCommandWaitWorkStop();
                    return;
                }
                //主动模式等待亮矫正型信号
                if ((!IsReadyStartScanImage) && PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.StartAir))
                {
                    BuryingPoint($"{UpdateStatusNameAction("BrightCorrectionStart")}");
                    SX_SetLight(intPtr);
                    Thread.Sleep(200);
                    BuryingPoint($"{UpdateStatusNameAction("BrightCorrectionEnd")}");
                    PLCControllerManager.GetInstance().WritePositionValue(Common.CommandDic[Command.LightCollectionEnd], true);
                    Thread.Sleep(100);
                    PLCControllerManager.GetInstance().WritePositionValue(Common.CommandDic[Command.LightCollectionEnd], false);
                    IsReadyStartScanImage = true;
                    break;
                }
            }
            //循环判断 开始采图的状态是否为0 如果开始采图的指令为true 那么给扫描站发送一个指令开始扫图
            while (!IsReadyStartScanImage && CheckSystemConditionAction());


            if (CheckSystemConditionAction() && IsScaning)
            {
                //这里进行一个函数回调，去找是否停止扫图的命令
                _object();
            }
            else
            {
                CancelScan();
                BoostingControllerManager.GetInstance().StopRay();
                return;
            }
        }

        /// <summary>
        /// 停止扫描之后，接收到PLC给过来的指令，然后通过采集空气状态给扫描站发送暗矫正指令
        /// </summary>
        private void InitiaStopImage()
        {
            BuryingPoint(UpdateStatusNameAction("QueryStopSignal"));
            do
            {
                Thread.Sleep(10);
                if (!IsInative() || IsStopScan())
                {
                    BuryingPoint($"{UpdateStatusNameAction("StopScaningInDarkCorrection")}");
                    IsScaning = false;
                    Thread.Sleep(100);
                    Common.StopScan();
                    BoostingControllerManager.GetInstance().SendCommandWaitWorkStop();
                    return;
                }
                //等待停止扫图的信号`
                if ((!IsReadSendDarkInfo) && !PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.StartImage))
                {
                    SX_Stop(intPtr); 
                    BoostingControllerManager.GetInstance().SendCommandWaitWorkStop();
                    //这里给方海涛发送一个暗/亮矫正
                    IsReadSendDarkInfo = true;
                    break;
                }
            }
            //循环判断 开始采图的状态是否为0 如果开始采图的指令为true 那么给扫描站发送一个指令开始扫图
            while (!IsReadSendDarkInfo && CheckSystemConditionAction());
            BuryingPoint($"{UpdateStatusNameAction("StopSignalSuccessfully")}");
            BoostingControllerManager.GetInstance().SendCommandWaitWorkStop();
            if (!(CheckSystemConditionAction() && IsScaning))
            {
                ClearScanImageCallBack?.Invoke();
                CancelScan();
                return;
            }
            BuryingPoint($"{UpdateStatusNameAction("ScanningEnd")} ");
            IsScaning = false;
            InitScanCondition();
            messageBoxAction?.Invoke("ScanSuccess");
            ClearScanImageCallBack?.Invoke();
            ScanCompleteCallBack?.Invoke();
        }
        #endregion

        #endregion
        //判定是否预热
        static bool isPreHot = false;
        /// <summary>
        /// 出束
        /// </summary>
        private void Ray()
        {
            BuryingPoint($"{UpdateStatusNameAction("ActiveModeGettingAcceleratorParameters")}  ");
            DeviceI di = new DeviceI();
            HAndL mhl = new HAndL();
            {
                BuryingPoint($"{UpdateStatusNameAction("ActiveModeGettingAcceleratorToWarmUp")}");
                if (!IsScaning) return;
                Thread.Sleep(100);
                BoostingControllerManager.GetInstance().SendCommandWaitWorkStop();
                BuryingPoint($"{UpdateStatusNameAction("ActiveModeSendOutPut")}");
                Thread.Sleep(1000);
                if (!IsScaning) return;
                SX_Start(intPtr);
                BuryingPoint($"{UpdateStatusNameAction("DarkCorrectioning")}");
                SX_SetDark(intPtr);
                Thread.Sleep(1500);
                BuryingPoint($"{UpdateStatusNameAction("DarkCorrectioning")}over");
                BuryingPoint($"{UpdateStatusNameAction("ActiveModeAcceleratorBeam")}");
                BoostingControllerManager.GetInstance().SendCommandRadiationOn();
                Thread.Sleep(1000);
            }
            logCallBack?.Invoke($"{UpdateStatusNameAction("AccelertorBeamInWarmUp")}", LogType.ScanStep, true);
        }

        /// <summary>
        /// 系统出错停止扫描
        /// </summary>
        private void CancelScan()
        {
            IsScaning = false;
            logCallBack?.Invoke("Stop scanning successfully ", LogType.ScanStep, true);
            BuryingPoint($"{UpdateStatusNameAction("StopScaningSuccessfully")}");
            //出错了，或者停止扫描，发送开始扫描指令为false
            Common.StopScan();
            CancelCallBack?.Invoke();
        }
        public override void BD_Scan()
        {
            ImageImportDll.SX_SetMode(ImageImportDll.intPtr, 1);
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
            UnActiveTask = Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        if (token.IsCancellationRequested)
                        {
                            logCallBack?.Invoke("取消扫描成功", LogType.ScanStep, true);
                            return;
                        }
                        //如果被动扫描过程中，突然出现了主动/被动模式调整，那么就强行暂停
                        //并且把IsClicking置为false
                        if (IsInative() || IsStopScan() || !CheckSystemConditionAction())
                        {
                            CancelScan();
                            BuryingPoint($"{UpdateStatusNameAction("StopScanning")}");
                            return;
                        }
                        InitScanCondition();
                        SetCommand(CommandDic[Command.StartScan], true);
                        logCallBack?.Invoke($"Command.StartScan {Command.StartScan} 发送完毕，值为：{true}", LogType.ScanStep, true);

                        recv(StartBrightSignleAndDarkStopImage);
                    }
                    catch (Exception ex)
                    {
                        CommonDeleget.HandTaskException(ex);
                    }
                }
            });
        }

        public override void ZD_Scan()
        {
            try
            {
                ImageImportDll.SX_SetMode(ImageImportDll.intPtr, 0);
                //向前
                if (PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.ScanForWard))
                {
                    logCallBack?.Invoke("设置向前", LogType.ScanStep, true);
                    SX_SetDirection(intPtr, 0);
                }
                else
                {
                    logCallBack?.Invoke("设置向后", LogType.ScanStep, true);
                    SX_SetDirection(intPtr, 1);
                }
                InitiaRecv(InitiaStopImage);
            }
            catch (Exception ex)
            {
                CommonDeleget.HandTaskException(ex);
            }
        }
    }
}
