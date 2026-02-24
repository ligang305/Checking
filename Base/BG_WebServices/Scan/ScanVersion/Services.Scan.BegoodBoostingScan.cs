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
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.ImageImportDll;
using BG_Entities;

namespace BG_Services
{
    [Export("Scan", typeof(BaseScan))]
    [CustomExportMetadata(1, "BegoodBoostingScan", "CGN. Begood", "zhuzhiwu", "1.0.0")]
    public class BegoodBoostingScan : BaseScan
    {
        public override void ZD_Scan()
        {
            try
            {
                ImageImportDll.SX_SetMode(ImageImportDll.intPtr, 0);
                SX_SetDirection(intPtr, PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.ScanForWard) ? 0 : 1);
                InitiaRecv(InitiaStopImage);
            }
            catch (Exception ex)
            {
                CommonDeleget.HandTaskException(ex);
            }
        }
        [HandleProcessCorruptedStateExceptions]
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
            logCallBack?.Invoke("Stop scanning successfully ", LogType.ScanStep, true);
            BuryingPoint($"{UpdateStatusNameAction("StopScaningSuccessfully")}");
            ImageImportDll.SX_Stop(ImageImportDll.intPtr);
            InitScanCondition();
            CancelCallBack?.Invoke();
        }

        #region 主动模式给扫描站发送的指令
        /// <summary>
        /// 这是给扫描站发送开始扫描的指令
        /// </summary>
        /// <param name="_object"></param>
        private void InitiaRecv(CallBackAction _object)
        {
            RequestTaskAction();
            BuryingPoint(UpdateStatusNameAction("Initiacanning"));
            InitScanCondition();
            BuryingPoint(UpdateStatusNameAction("SendStartScanSignal") + "SX_Start");
            if (!IsScaning) return;
            SX_Start(intPtr); //开始扫描
            if (!IsScaning) return;
            BuryingPoint($"{UpdateStatusNameAction("DarkCorrectionStart")}");
            SX_SetDark(intPtr); //做暗矫正
            BuryingPoint($"{UpdateStatusNameAction("DarkCorrectionEnd")}");
            Thread.Sleep(1000);
            var result = PLCControllerManager.GetInstance().WritePositionValue(Common.CommandDic[Command.StartScan], true);
            BuryingPoint($"{UpdateStatusNameAction("SendStartScanSignalCompleted")}{Common.CommandDic[Command.StartScan]};result:{result}");
            Thread.Sleep(10);
            BuryingPoint($"{UpdateStatusNameAction("WaitingBrightCorrectionSignal")} ！");
            do
            {
                //该函数正在执行，但是当点击了停止扫描之后就跳出
                if (IsStopScan())
                {
                    BuryingPoint($"{UpdateStatusNameAction("StopAcquisitionScanning")}");
                    IsScaning = false;
                    Thread.Sleep(100);
                    Common.StopScan();
                    CancelCallBack?.Invoke();
                    PLCControllerManager.GetInstance().WritePositionValue(Common.CommandDic[Command.StartScan], false);
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
                if (DetecotrControllerManager.GetInstance().DetectorConnection == 3)
                {
                    BuryingPoint($"{UpdateStatusNameAction("BrightCorrectionStart")} ");
                    SX_SetLight(intPtr);
                    Thread.Sleep(1000);
                    BuryingPoint($"{UpdateStatusNameAction("BrightCorrectionEnd")}");
                    PLCControllerManager.GetInstance().WritePositionValue(Common.CommandDic[Command.LightCollectionEnd], true);
                    Thread.Sleep(200);
                    PLCControllerManager.GetInstance().WritePositionValue(Common.CommandDic[Command.LightCollectionEnd], false);
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
                if (IsStopScan())
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
                if (DetecotrControllerManager.GetInstance().DetectorConnection == 3)
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
                Scan_Stop();
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

        #region 被动模式给扫描站发送的指令
        /// <summary>
        /// 这是给扫描站发送开始扫描的指令
        /// </summary>
        /// <param name="_object"></param>
        [HandleProcessCorruptedStateExceptions]
        private void recv(CallBackParamaterAction _object)
        {
            BuryingPoint($"{UpdateStatusNameAction("Passivescanning")}");
            BuryingPoint($"{UpdateStatusNameAction("GetStartScanSignaling")}");
            do
            {
                Thread.Sleep(10);
                //该函数正在执行，但是当点击了停止扫描之后就跳出
                if (IsStopScan())
                {
                    BuryingPoint($"{UpdateStatusNameAction("StopAcquisitionScanning")}");
                    break;
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

            BuryingPoint($"{UpdateStatusNameAction("StartSignalSuccess")}");
            _object(StartSendDarkImage);
        }

        /// <summary>
        /// 当扫描站已经开始扫描了，这时候给扫描站发送停止扫描的操作/指令
        /// </summary>
        /// <param name="_object"></param>
        [HandleProcessCorruptedStateExceptions]
        private void StartStopImage(CallBackAction _object)
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
                _object();
                if (CheckSystemConditionAction() && IsScaning)
                {
                    //如果连接了扫描站，就通知扫描站开始扫图
                    //if (DetecotrControllerManager.GetInstance().DetectorConnection)
                    //{
                    //    BuryingPoint($"{UpdateStatusNameAction("BrightCorrectionStart")}");
                    //    SX_SetLight(intPtr);
                    //    Thread.Sleep(1000);
                    //    BuryingPoint($"{UpdateStatusNameAction("BrightCorrectionEnd")}");
                    //    PLCControllerManager.GetInstance().WritePositionValue(Common.CommandDic[Command.LightCollectionEnd], true);
                    //    Thread.Sleep(500);
                    //    PLCControllerManager.GetInstance().WritePositionValue(Common.CommandDic[Command.LightCollectionEnd], false);
                    //}
                    //else
                    //{
                    //    messageBoxAction?.Invoke("UnConnectionWithScan");
                    //    return;
                    //}
                    //_object();
                }
                else
                {
                    BuryingPoint($"{UpdateStatusNameAction("ScanningZD")}");
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
        [HandleProcessCorruptedStateExceptions]
        private void StartSendDarkImage()
        {
            try
            {
                BuryingPoint($"{UpdateStatusNameAction("WaitingDarkCorrectionSignal")}!");
                do
                {
                    Thread.Sleep(100);
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
                        BuryingPoint($"{UpdateStatusNameAction("DarkCorrectionStart")}");
                        SX_SetDark(intPtr);
                        BuryingPoint($"{UpdateStatusNameAction("DarkCorrectionEnd")}");

                        Thread.Sleep(200);
                        PLCControllerManager.GetInstance().WritePositionValue(Common.CommandDic[Command.DarkCollectionEnd], true);
                        Thread.Sleep(200);
                        PLCControllerManager.GetInstance().WritePositionValue(Common.CommandDic[Command.DarkCollectionEnd], false);
                        break;
                    }
                }
                //循环判断 开始采图的状态是否为0 如果开始采图的指令为true 那么给扫描站发送一个指令开始扫图
                while (!IsReadSendDarkInfo && CheckSystemConditionAction());

                if (CheckSystemConditionAction() && IsScaning)
                {
                    ////如果连接了扫描站，就通知扫描站开始扫图
                    //if (DetecotrControllerManager.GetInstance().DetectorConnection)
                    //{
                    //    BuryingPoint($"{UpdateStatusNameAction("DarkCorrectionStart")}");
                    //    SX_SetDark(intPtr);
                    //    BuryingPoint($"{UpdateStatusNameAction("DarkCorrectionEnd")}");

                    //    Thread.Sleep(1000);
                    //    PLCControllerManager.GetInstance().WritePositionValue(Common.CommandDic[Command.DarkCollectionEnd], true);
                    //    Thread.Sleep(500);
                    //    PLCControllerManager.GetInstance().WritePositionValue(Common.CommandDic[Command.DarkCollectionEnd], false);
                    //}
                    //else
                    //{
                    //    messageBoxAction?.Invoke("UnConnectionWithScan");
                    //}
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
    }
}
