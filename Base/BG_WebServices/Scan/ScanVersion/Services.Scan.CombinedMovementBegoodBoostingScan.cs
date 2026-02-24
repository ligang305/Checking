using CMW.Common.Utilities;
using BGCommunication;
using BGModel;
using System;
using System.Collections.Generic;
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
    /// <summary>
    /// 适用于自行走扫描或同类扫描
    /// </summary>
    public class CombinedMovementBegoodBoostingScan : BaseScan
    {
        public override void Scan_Start()
        {
            IsScaning = true;
            if (PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.ScanMode))
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
            CancelScan();
        }
        #region 给扫描站发送扫描指令


        #region 主动模式给扫描站发送的指令
        /// <summary>
        /// 这是给扫描站发送开始扫描的指令
        /// </summary>
        /// <param name="_object"></param>
        private void InitiaRecv(CallBackAction _object)
        {
            InitScanCondition();
            BuryingPoint(UpdateStatusNameAction("Initiacanning"));
            BuryingPoint(UpdateStatusNameAction("SendStartScanSignal") + "SX_Start");
            if (!IsScaning) return;
            SX_Start(intPtr); //开始扫描
            Thread.Sleep(100);
            if (!IsScaning) return;
            BuryingPoint($@"{UpdateStatusNameAction("SendStartScanSignal")},{CommandDic[Command.StartScan]}");
            BuryingPoint($"{UpdateStatusNameAction("DarkCorrectionStart")}");
            SX_SetDark(intPtr); //做暗矫正
            BuryingPoint($"{UpdateStatusNameAction("DarkCorrectionEnd")}");
            Thread.Sleep(1000);
            var result = SetCommand(CommandDic[Command.StartScan], true);
            BuryingPoint($"{UpdateStatusNameAction("SendStartScanSignalCompleted")}{CommandDic[Command.StartScan]};result:{result}");
            Thread.Sleep(10);
            BuryingPoint($"{UpdateStatusNameAction("WaitingBrightCorrectionSignal")} ！");
            do
            {
                //该函数正在执行，但是当点击了停止扫描之后就跳出
                //当第一步来了之后，判断顺序有没有乱，就判断第二步有没有优于第一步先到，
                //后续添加防撞报警|| GlobalRetStatus[55] || GlobalRetStatus[46]
                if (!IsInative() || IsStopScan() || Common.SelfAutoMoveAlarm())
                {
                    BuryingPoint($"{UpdateStatusNameAction("StopAcquisitionScanning")}");
                    IsScaning = false;
                    Thread.Sleep(100);
                    Common.StopScan();
                    CancelCallBack?.Invoke();
                    SetCommand(CommandDic[Command.StartScan], false);
                    return;
                }
                //主动模式等待暗矫正型信号
                if ((!IsReadyStartScanImage) && PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.StartAir))
                {
                    BuryingPoint($"{UpdateStatusNameAction("BrightCorrectionEnd")}");
                    IsReadyStartScanImage = true;
                    break;
                }
            }
            //循环判断 开始采图的状态是否为0 如果开始采图的指令为true 那么给扫描站发送一个指令开始扫图
            while (!IsReadyStartScanImage && CheckSystemConditionAction());


            if (CheckSystemConditionAction() && IsScaning)
            {
                //如果连接了扫描站，就通知扫描站开始扫图
                if (IsScanCanScan())
                {
                    BuryingPoint($"{UpdateStatusNameAction("BrightCorrectionStart")} ");
                    SX_SetLight(intPtr);
                    Thread.Sleep(1000);
                    BuryingPoint($"{UpdateStatusNameAction("BrightCorrectionEnd")}");
                    SetCommand(CommandDic[Command.LightCollectionEnd], true);
                    Thread.Sleep(200);
                    SetCommand(CommandDic[Command.LightCollectionEnd], false);
                }
                else
                {
                    messageBoxAction?.Invoke("UnConnectionWithScan");
                }
            }
            else
            {
                BuryingPoint($"{UpdateStatusNameAction("ScanningZD")}");
                CancelScan();
                logCallBack?.Invoke($"出错，获取暗矫正数据阶段停止扫图", LogType.ScanStep, true);
                return;
            }
            //这里进行一个函数回调，去找是否停止扫图的命令
            _object();
        }

        /// <summary>
        /// 接收到亮矫正信号之后等待停止扫描信号
        /// </summary>
        private void InitiaStopImage()
        {
            BuryingPoint(UpdateStatusNameAction("QueryStopSignal"));
            do
            {
                Thread.Sleep(10);
                if (!IsInative() || IsStopScan()
                      || Common.SelfAutoMoveAlarm())
                {
                    BuryingPoint($"{UpdateStatusNameAction("StopScaningInDarkCorrection")}");
                    IsScaning = false;
                    Thread.Sleep(100);
                    Common.StopScan();
                    return;
                }
                //等待停止扫图的信号
                if ((!IsReadSendDarkInfo) && !PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.StartImage))
                {
                    //这里给方海涛发送一个暗/亮矫正
                    IsReadSendDarkInfo = true;
                    break;
                }
            }
            //循环判断 开始采图的状态是否为0 如果开始采图的指令为true 那么给扫描站发送一个指令开始扫图
            while (!IsReadSendDarkInfo && CheckSystemConditionAction());
            BuryingPoint($"{UpdateStatusNameAction("StopSignalSuccessfully")}");
            if (CheckSystemConditionAction() && IsScaning)
            {
                BuryingPoint($"{UpdateStatusNameAction("SendStopScanningSignal")}");
                //如果连接了扫描站，就通知扫描站开始扫图
                if (IsScanCanScan())
                {
                    Task.Run(() =>
                    {
                        try
                        { SX_Stop(intPtr); }
                        catch { }
                    });
                }
                else
                {
                    messageBoxAction?.Invoke("UnConnectionWithScan");
                }
            }
            else
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
        #region 被动模式给扫描站发送的指令
        /// <summary>
        /// 这是给扫描站发送开始扫描的指令
        /// </summary>
        /// <param name="_object"></param>
        private void recv(CallBackParamaterAction _object)
        {
            BuryingPoint($"{UpdateStatusNameAction("Passivescanning")}");
            BuryingPoint($"{UpdateStatusNameAction("GetStartScanSignaling")}");
            do
            {
                Thread.Sleep(10);
                //该函数正在执行，但是当点击了停止扫描之后就跳出
                //该函数正在执行，但是主被动扫描模式变化了之后就跳出
                if (IsInative() || IsStopScan() || Common.SelfAutoMoveAlarm())
                {
                    BuryingPoint($"{UpdateStatusNameAction("StopAcquisitionScanning")}");
                    break;
                }
                //当发现状态为1且已经通知过扫描站 就不再进行通知扫描站了 ，然后跳出循环，直到判断停止扫图状态为1
                if ((!IsReadyStartScanImage) && PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.StartImage))
                {
                    IsReadyStartScanImage = true;
                    break;
                }
            }
            //循环判断 开始采图的状态是否为0 如果开始采图的指令为true 那么给扫描站发送一个指令开始扫图
            while (!IsReadyStartScanImage && CheckSystemConditionAction());

            BuryingPoint($"{UpdateStatusNameAction("StartSignalSuccess")}");


            if (CheckSystemConditionAction() && IsScaning)
            {
                //如果连接了扫描站，就通知扫描站开始扫图
                if (IsScanCanScan()) { SX_Start(intPtr); }
                else
                {
                    messageBoxAction?.Invoke("UnConnectionWithScan");
                }
                _object(StartSendDarkImage);
            }
            else
            {
                BuryingPoint($"{UpdateStatusNameAction("StopScanning")}");
                CancelScan();
                return;
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
                BuryingPoint($"{UpdateStatusNameAction("BrightCorrectionStart")}！");
                do
                {
                    if (IsStopScan())
                    {
                        BuryingPoint($"{UpdateStatusNameAction("StopScaningInAcquiring")}");
                        return;
                    }
                    //当发现PLC给了我开始采图信号之后，我就给PLC发送亮矫正的信号
                    if ((!IsReadyStopScanImage) && PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.StartAir))
                    {
                        IsReadyStopScanImage = true;
                        break;
                    }
                }
                //循环判断 开始采图的状态是否为0 如果开始采图的指令为true 那么给扫描站发送一个指令开始扫图
                while (!IsReadyStopScanImage && CheckSystemConditionAction());
                BuryingPoint($"{UpdateStatusNameAction("BrightCorrectionSuccessfully")}");
                if (CheckSystemConditionAction() && IsScaning)
                {
                    //如果连接了扫描站，就通知扫描站开始扫图
                    if (IsScanCanScan())
                    {
                        BuryingPoint($"{UpdateStatusNameAction("BrightCorrectionStart")}");
                        SX_SetLight(intPtr);
                        Thread.Sleep(1000);
                        BuryingPoint($"{UpdateStatusNameAction("BrightCorrectionEnd")}");
                        SetCommand(CommandDic[Command.LightCollectionEnd], true);
                        Thread.Sleep(500);
                        SetCommand(CommandDic[Command.LightCollectionEnd], false);
                    }
                    else
                    {
                        messageBoxAction?.Invoke("UnConnectionWithScan");
                        return;
                    }
                    _object();
                }
                else
                {
                    BuryingPoint($"{UpdateStatusNameAction("ScanningZD")}");
                    CancelScan();
                    return;
                }
            }
            catch (Exception ex)
            {
                CommonDeleget.HandTaskException(ex);
            }
        }

        /// <summary>
        /// 停止扫描之后，接收到PLC给过来的指令，然后通过采集空气状态给扫描站发送暗矫正指令
        /// </summary>
        private void StartSendDarkImage()
        {
            try
            {
                BuryingPoint($"{UpdateStatusNameAction("WaitingDarkCorrectionSignal")}!");

                do
                {
                    Thread.Sleep(1);
                    if (IsStopScan())
                    {
                        BuryingPoint($"{UpdateStatusNameAction("StopScaningInDarkCorrection")}");
                        return;
                    }

                    //接收到开始采集暗矫正命令的时候
                    if ((!IsReadSendDarkInfo) && PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.StartDark))
                    {
                        //这里给方海涛发送一个暗/亮矫正
                        IsReadSendDarkInfo = true;
                        BuryingPoint($"{UpdateStatusNameAction("DarkCorrectionSuccessfully")}!");
                        break;
                    }
                }
                //循环判断 开始采图的状态是否为0 如果开始采图的指令为true 那么给扫描站发送一个指令开始扫图
                while (!IsReadSendDarkInfo && CheckSystemConditionAction());

                if (CheckSystemConditionAction() && IsScaning)
                {
                    //如果连接了扫描站，就通知扫描站开始扫图
                    if (IsScanCanScan())
                    {
                        BuryingPoint($"{UpdateStatusNameAction("DarkCorrectionStart")}");
                        SX_SetDark(intPtr);
                        BuryingPoint($"{UpdateStatusNameAction("DarkCorrectionEnd")}");

                        Thread.Sleep(1000);
                        //本底才结束
                        SetCommand(CommandDic[Command.DarkCollectionEnd], true);
                        Thread.Sleep(500);
                        SetCommand(CommandDic[Command.DarkCollectionEnd], false);
                    }
                    else
                    {
                        messageBoxAction?.Invoke("UnConnectionWithScan");
                    }
                }
                else
                {
                    BuryingPoint($"{UpdateStatusNameAction("ScanningZD")}");
                    CancelScan();
                    logCallBack?.Invoke($"{UpdateStatusNameAction("ScanningZD")}", LogType.ScanStep, true);
                    return;
                }
                BuryingPoint($"{UpdateStatusNameAction("ScanningEnd")}!");
                Thread.Sleep(100);
                SX_Stop(intPtr);
                InitScanCondition();
                Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                CommonDeleget.HandTaskException(ex);
            }
        }

        #endregion

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
                //SX_SetSpeed(ImageImportDll.intPtr, 500);
                InitiaRecv(InitiaStopImage);
            }
            catch (Exception ex)
            {
                CommonDeleget.HandTaskException(ex);
            }
        }

        public override void BD_Scan()
        {
            ImageImportDll.SX_SetMode(ImageImportDll.intPtr, 1);
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
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
                        if (IsInative() || IsStopScan() || !CheckSystemConditionAction()
                            || Common.SelfAutoMoveAlarm())
                        {
                            CancelScan();
                            BuryingPoint($"{UpdateStatusNameAction("EmergencySystemPause")}");
                            return;
                        }
                        InitScanCondition();
                        SetCommand(CommandDic[Command.StartScan], true);
                        logCallBack?.Invoke($"Command.StartScan {Command.StartScan} 发送完毕，值为：{true}", LogType.ScanStep, true);

                        recv(StartStopImage);
                    }
                    catch (Exception ex)
                    {
                        CommonDeleget.HandTaskException(ex);
                    }
                }
            });
        }
    }
}
