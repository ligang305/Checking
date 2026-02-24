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
    [CustomExportMetadata(1, "RussiaBetratronScan", "CGN. Begood", "zhuzhiwu", "1.0.0")]
    /// <summary>
    /// 适用于自行走扫描或同类扫描
    /// </summary>
    public class RussiaBetratronScan : BaseScan
    {
        public override void Scan_Stop()
        {
            Stop_Scan();
        }
        /// <summary>
        /// 遇错停止扫描
        /// </summary>
        private void Stop_Scan()
        {
            IsScaning = false;
            logCallBack?.Invoke("停止扫描成功", LogType.ScanStep, true);
            BuryingPoint($"{UpdateStatusNameAction("StopScanSuccess")}");
            Task.Run(() => {
                //出错了，或者停止扫描，发送开始扫描指令为false
                ImageImportDll.SX_Stop(ImageImportDll.intPtr);
                InitScanCondition();
                BoostingControllerManager.GetInstance().StopRay();
                CancelCallBack?.Invoke();
            });
        }


        #region 被动模式给扫描站发送的指令
        /// <summary>
        /// 这是给扫描站发送开始扫描的指令
        /// </summary>
        /// <param name="_object"></param>
        private void recv(CallBackParamaterAction _object)
        {
            BuryingPoint($"Start Bd Scan");
            do
            {
                Thread.Sleep(10);
                //该函数正在执行，但是主被动扫描模式变化了之后就跳出
                if (IsStopScan())
                {
                    BuryingPoint($"Stop Scan in IsStopScan:{IsStopScan()}");
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
            BuryingPoint($"Get Start Single Successful");

            if (CheckSystemConditionAction() && IsScaning)
            {
                //如果连接了扫描站，就通知扫描站开始扫图
                if (IsScanCanScan()) { BuryingPoint($"SX_Start Before"); SX_Start(intPtr); BuryingPoint($"SX_Start End"); }
                else
                {
                    BuryingPoint($"SX_Start Failed");
                }
                _object(StartSendDarkImage);
            }
            else
            {
                BuryingPoint($"SX_Start Failed in Else");
                Scan_Stop();
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
                RequestTaskAction();
                BuryingPoint($"RequestTask  End ！");
                do
                {
                    if (IsStopScan())
                    {
                        BuryingPoint($"StopScaningInAcquiring");
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
                BuryingPoint($"Get BrightCorrectionSuccessfully");
                if (CheckSystemConditionAction() && IsScaning)
                {
                    //如果连接了扫描站，就通知扫描站开始扫图
                    if (IsScanCanScan())
                    {
                        BuryingPoint($" SX_SetLight Before");
                        SX_SetLight(intPtr);
                        Thread.Sleep(1000);
                        BuryingPoint($" SX_SetLight End");
                        PLCControllerManager.GetInstance().WritePositionValue(Common.CommandDic[Command.LightCollectionEnd], true);
                        Thread.Sleep(500);
                        PLCControllerManager.GetInstance().WritePositionValue(Common.CommandDic[Command.LightCollectionEnd], false);
                    }
                    else
                    {
                        BuryingPoint($"Stop Scan UnConnectionWithScan");
                        return;
                    }
                    _object();
                }
                else
                {
                    BuryingPoint($"Stop Scan In BrightCorrection");
                    Scan_Stop();
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
                BuryingPoint($"WaitingDarkCorrectionSignal");
                do
                {
                    Thread.Sleep(1);
                    if (IsStopScan())
                    {
                        BuryingPoint($"StopScaning In Get DarkCorrection");
                        return;
                    }

                    //接收到开始采集暗矫正命令的时候
                    if ((!IsReadSendDarkInfo) && PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.StartDark))
                    {
                        //这里给方海涛发送一个暗/亮矫正
                        IsReadSendDarkInfo = true;
                        BuryingPoint($"Get DarkCorrectionSuccessfully");
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
                        BuryingPoint($"SX_SetDark Before");
                        SX_SetDark(intPtr);
                        BuryingPoint($"SX_SetDark End");
                        Thread.Sleep(1000);
                        //本底才结束
                        PLCControllerManager.GetInstance().WritePositionValue(Common.CommandDic[Command.DarkCollectionEnd], true);
                        Thread.Sleep(500);
                        PLCControllerManager.GetInstance().WritePositionValue(Common.CommandDic[Command.DarkCollectionEnd], false);
                    }
                    else
                    {
                        messageBoxAction?.Invoke("UnConnectionWithScan");
                    }
                }
                else
                {
                    BuryingPoint($"{UpdateStatusNameAction("ScanningZD")}");
                    Scan_Stop();
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

        #region 主动模式给扫描站发送的指令
        /// <summary>
        /// 这是给扫描站发送开始扫描的指令
        /// </summary>
        /// <param name="_object"></param>
        private void InitiaRecv(CallBackAction _object)
        {
            BuryingPoint($"{UpdateStatusNameAction("BrightCorrectionStart")}！");
            RequestTaskAction();
            BuryingPoint($"RequestTask  End ！");
            InitScanCondition();
            BuryingPoint($"{UpdateStatusNameAction("Initiacanning")}");
            SX_Start(intPtr);
            BuryingPoint($"{UpdateStatusNameAction("SX_SetDark")}");
            SX_SetDark(intPtr);
            PLCControllerManager.GetInstance().WritePositionValue(Common.CommandDic[Command.DarkCollectionEnd], true);
            Thread.Sleep(1000);
            PLCControllerManager.GetInstance().WritePositionValue(Common.CommandDic[Command.DarkCollectionEnd], false);
            BuryingPoint($"{UpdateStatusNameAction("DarkCorrectionEnd")}");
            BuryingPoint($"{UpdateStatusNameAction("ActiveModeSendOutPut")} ");
            BoostingControllerManager.GetInstance().Ray();
            Thread.Sleep(15000);
            BuryingPoint($"{UpdateStatusNameAction("BrightCorrectionStart")} SX_SetLight");
            if (!IsScaning) return;
            SX_SetLight(intPtr);
            Thread.Sleep(1000);
            if (!IsScaning) return;
            BuryingPoint($"{UpdateStatusNameAction("SendStartScanSignal")} ,{CommandDic[Command.StartScan]}");
            PLCControllerManager.GetInstance().WritePositionValue(Common.CommandDic[Command.StartScan], true);

            do
            {
                Thread.Sleep(10);
                //该函数正在执行，但是当点击了停止扫描之后就跳出
                //当第一步来了之后，判断顺序有没有乱，就判断第二步有没有优于第一步先到，
                if (IsStopScan())
                {
                    BuryingPoint($"{UpdateStatusNameAction("StopAcquisitionScanning")}");
                    IsScaning = false;
                    Thread.Sleep(100);
                    Common.StopScan();
                    CancelCallBack?.Invoke();
                    PLCControllerManager.GetInstance().WritePositionValue(Common.CommandDic[Command.StartScan], false);
                    BoostingControllerManager.GetInstance().StopRay();
                    return;
                }
                //主动模式等待开始采图信号
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
                _object();
            }
            else
            {
                Scan_Stop();
                return;
            }
        }

        /// <summary>
        /// 停止扫描之后，接收到PLC给过来的指令，然后通过采集空气状态给扫描站发送暗矫正指令
        /// </summary>
        private void InitiaStopImage()
        {
            BuryingPoint($"{UpdateStatusNameAction("QueryStopSignal")}!");
            do
            {
                Thread.Sleep(10);
                if (IsStopScan())
                {
                    BuryingPoint($"IsStopScan() = { IsStopScan()};{!IsScaning};{!IsGotoConnnection};{!IsConnection};");
                    BuryingPoint($"{UpdateStatusNameAction("StopScaningInAcquiring")}");
                    IsScaning = false;
                    Thread.Sleep(100);
                    Scan_Stop();
                    BoostingControllerManager.GetInstance().StopRay();
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
            BuryingPoint($"{UpdateStatusNameAction("StopScanSuccess")}!");
            if (CheckSystemConditionAction() && IsScaning)
            {
                BuryingPoint($"{UpdateStatusNameAction("SendStopScanningSignal")}!");

                SX_Stop(intPtr);
            }
            else
            {
                ClearScanImageCallBack?.Invoke();
                Scan_Stop();
                return;
            }
            BuryingPoint($"{UpdateStatusNameAction("ScanningEnd")}");
            BoostingControllerManager.GetInstance().StopRay();
            Scan_Stop();
            messageBoxAction?.Invoke("ScanSuccess");
            ClearScanImageCallBack?.Invoke();
            ScanCompleteCallBack?.Invoke();
        }
        #endregion


        public override void BD_Scan()
        {
            ImageImportDll.SX_SetMode(ImageImportDll.intPtr, 1);
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
            SX_SetDirection(intPtr, PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.ScanForWard) ? 0 : 1);
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
                        //并且把IsClicking置为false
                        if (IsStopScan() || !CheckSystemConditionAction())
                        {
                            Scan_Stop();
                            BuryingPoint($"{UpdateStatusNameAction("EmergencySystemPause")}");
                            return;
                        }
                        InitScanCondition();
                        PLCControllerManager.GetInstance().WritePositionValue(Common.CommandDic[Command.StartScan], true);
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

        public override void ZD_Scan()
        {
            try
            {
                ImageImportDll.SX_SetMode(ImageImportDll.intPtr, 0);
                ImageImportDll.SX_SetSpeed(ImageImportDll.intPtr,144);
                SX_SetDirection(intPtr, PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.ScanForWard) ? 0 : 1);
                InitiaRecv(InitiaStopImage);
            }
            catch (Exception ex)
            {
                CommonDeleget.HandTaskException(ex);
            }
        }
    }
}
