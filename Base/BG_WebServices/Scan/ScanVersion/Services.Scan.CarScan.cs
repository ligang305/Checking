using CMW.Common.Utilities;
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
    [CustomExportMetadata(1, "CarScan", "CGN. Begood", "zhuzhiwu", "1.0.0")]
    /// <summary>
    /// 适用于车载扫描
    /// </summary>
    public class CarScan: BaseScan
    {
        public override void Scan_Start()
        {
            IsScaning = true;
            if (SearchScanMode() == "InitiativeMode")
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

        /// <summary>
        /// 判断扫描条件
        /// </summary>
        /// <returns></returns>
        private bool JudgeScanCondition()
        {
            return IsInative() || IsStopScan() || !CheckSystemConditionAction()
                   || Common.MoveAlarm();
        }

        #region Car车载的被动扫描图
        /// <summary>
        /// 这是给扫描站发送开始扫描的指令
        /// </summary>
        /// <param name="_object"></param>
        private void recv(CommonDeleget.CallBackParamaterAction _object)
        {
            BuryingPoint(UpdateStatusNameAction($"Passivescanning"));
            BuryingPoint(UpdateStatusNameAction($"Startsampling"));
            do
            {
                Thread.Sleep(1);
                //该函数正在执行，但是当点击了停止扫描之后就跳出
                //该函数正在执行，但是主被动扫描模式变化了之后就跳出|| GlobalRetStatus[55] || GlobalRetStatus[54]
                if (IsInative() || IsStopScan() || Common.MoveAlarm())
                {
                    BuryingPoint(UpdateStatusNameAction("StopAcquisitionScanning"));
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

            BuryingPoint(UpdateStatusNameAction("StartSignalSuccess"));


            if (CheckSystemConditionAction() && IsScaning)
            {
                //如果连接了扫描站，就通知扫描站开始扫图
                if (IsScanCanScan()) { SX_Start(intPtr); }
                else
                {
                    messageBoxAction.Invoke("UnConnectionWithScan");
                }
                _object(StartSendDarkImage);
            }
            else
            {
                BuryingPoint(UpdateStatusNameAction("StopScanning"));
                CancelScan();
                return;
            }
        }
        /// <summary>
        /// 当扫描站已经开始扫描了，这时候给扫描站发送停止扫描的操作/指令) || GlobalRetStatus[54]
        /// </summary>
        /// <param name="_object"></param>
        private void StartStopImage(CommonDeleget.CallBackAction _object)
        {
            BuryingPoint(UpdateStatusNameAction("WaitingBrightCorrectionSignal"));
            do
            {
                Thread.Sleep(1);
                if (IsStopScan() || IsInative() || Common.MoveAlarm())
                {
                    BuryingPoint(UpdateStatusNameAction("StopScaningInAcquiring"));
                    break;
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
            BuryingPoint(UpdateStatusNameAction("BrightCorrectionSuccessfully"));
            if (CheckSystemConditionAction() && IsScaning)
            {
                //如果连接了扫描站，就通知扫描站开始扫图
                if (IsScanCanScan())
                {
                    BuryingPoint(UpdateStatusNameAction("BrightCorrectionStart"));
                    if (CurrentCv == ControlVersion.Car)
                    {
                        StartARMScan();
                    }
                    SX_SetLight(intPtr); Thread.Sleep(100);
                    SetCommand(CommandDic[Command.LightCollectionEnd], true);
                }
                else
                {
                    messageBoxAction.Invoke("UnConnectionWithScan");
                    return;
                }
                BuryingPoint(UpdateStatusNameAction("BrightCorrectionEnd"));
                //这里进行一个函数回调，去找是否停止扫图的命令
                _object();
            }
            else
            {
                BuryingPoint(UpdateStatusNameAction("EmergencySystemPause"));
                CancelScan();
                return;
            }

        }
        /// <summary>
        /// 停止扫描之后，接收到PLC给过来的指令，然后通过采集空气状态给扫描站发送暗矫正指令 && (GlobalRetStatus[54]
        /// </summary>
        private void StartSendDarkImage()
        {
            BuryingPoint(UpdateStatusNameAction("DarkCorrectionStart"));
            do
            {
                Thread.Sleep(1);
                if (IsStopScan() || IsInative() || Common.MoveAlarm())
                {
                    BuryingPoint(UpdateStatusNameAction("StopScaningInDarkCorrection"));
                    break;
                }

                //接收到开始采集暗矫正命令的时候
                if ((!IsReadSendDarkInfo) && PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.StartDark))
                {
                    //这里给方海涛发送一个暗/亮矫正
                    IsReadSendDarkInfo = true;
                    break;
                }
            }
            //循环判断 开始采图的状态是否为0 如果开始采图的指令为true 那么给扫描站发送一个指令开始扫图
            while (!IsReadSendDarkInfo && CheckSystemConditionAction());
            BuryingPoint(UpdateStatusNameAction("DarkCorrectionSuccessfully"));
            if (CheckSystemConditionAction() && IsScaning)
            {
                //如果连接了扫描站，就通知扫描站开始扫图
                if (IsScanCanScan())
                {
                    SX_SetDark(intPtr); Thread.Sleep(100);
                    SetCommand(CommandDic[Command.DarkCollectionEnd], true);
                }
                else
                {
                    messageBoxAction.Invoke("UnConnectionWithScan");
                }
            }
            else
            {
                BuryingPoint(UpdateStatusNameAction("StopScanning"));
                CancelScan();
                logCallBack.Invoke(UpdateStatusNameAction("StopScanning"), LogType.ScanStep, true);
                return;
            }

            BuryingPoint(UpdateStatusNameAction("ScanningEnd"));
            Thread.Sleep(100);
            InitScanCondition();
            StartARMEnd();
            SX_Stop(intPtr);
            ClearScanImageCallBack?.Invoke();
            Thread.Sleep(500);
        }
        #endregion

        #region 主动模式给扫描站发送的指令
        /// <summary>
        /// 这是给扫描站发送开始扫描的指令
        /// </summary>
        /// <param name="_object"></param>
        private void InitiaRecv(CommonDeleget.CallBackParamaterAction _object)
        {
            BuryingPoint(UpdateStatusNameAction("Initiacanning"));
            BuryingPoint(UpdateStatusNameAction("QueryStartSignal"));
            BuryingPoint(UpdateStatusNameAction("InitScanCondition"));
            InitScanCondition();
            SetCommand(CommandDic[Command.StartScan], true);
            SX_Start(intPtr);
            do
            {
                //该函数正在执行，但是当点击了停止扫描之后就跳出
                //当第一步来了之后，判断顺序有没有乱，就判断第二步有没有优于第一步先到，
                //后续添加防撞报警
                if (JudgeScanStatus())
                {
                    BuryingPoint(UpdateStatusNameAction("StopAcquisitionScanning"));
                    IsScaning = false;
                    Thread.Sleep(100);
                    Common.StopScan();
                    CancelCallBack?.Invoke();
                    SetCommand(CommandDic[Command.StartScan], false);
                    SetCommand(CommandDic[Command.StationReady], false);
                    return;
                }
                //主动模式等待暗矫正型信号
                if ((!IsReadyStartScanImage) && PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.StartDark))
                {
                    IsReadyStartScanImage = true;
                    break;
                }
            }
            //循环判断 开始采图的状态是否为0 如果开始采图的指令为true 那么给扫描站发送一个指令开始扫图
            while (!IsReadyStartScanImage && CheckSystemConditionAction());

            BuryingPoint(UpdateStatusNameAction("StartSignalSuccess"));



            if (CheckSystemConditionAction() && IsScaning)
            {
                //一切正常，发送暗矫正指令
                if (IsScanCanScan())
                {
                    BuryingPoint(UpdateStatusNameAction("DarkCorrectioning"));
                    SX_SetDark(intPtr); Thread.Sleep(100);
                    SetCommand(CommandDic[Command.DarkCollectionEnd], true);
                }
                else
                {
                    messageBoxAction.Invoke("UnConnectionWithScan");
                }
                //这里进行一个函数回调，去找是否停止扫图的命令
                _object(InitiaStopImage);
            }
            else
            {
                BuryingPoint(UpdateStatusNameAction("StopScanning"));
                CancelScan();
                logCallBack?.Invoke(UpdateStatusNameAction("StopScanning"), LogType.ScanStep, true);
                return;
            }
        }
        /// <summary>
        /// 获取开始彩图信号第一步
        /// </summary>
        /// <returns></returns>
        private bool JudgeScanStatus()
        {
            return !IsInative() || IsStopScan() ||
                PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.StartAir)||
                 PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.StopImageStr)
                //GlobalRetStatus[55] || GlobalRetStatus[46]
                   || Common.MoveAlarm();
        }
        /// <summary>
        /// 当扫描站已经开始扫描了，这时候给扫描站发送停止扫描的操作/指令
        /// </summary>
        /// <param name="_object"></param>
        private void InitiaLightImage(CommonDeleget.CallBackAction _object)
        {
            BuryingPoint(UpdateStatusNameAction("WaitingBrightCorrectionSignal"));
            do
            {
                if (!IsInative() || IsStopScan() || PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.StopImageStr)
                      || Common.MoveAlarm())
                {
                    BuryingPoint(UpdateStatusNameAction("StopAcquisitionScanning"));
                    Common.StopScan();
                    Thread.Sleep(100);
                    SetCommand(CommandDic[Command.StationReady], false);
                    return;
                }
                if ((!IsReadyStopScanImage) && PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.StartAir))
                {
                    IsReadyStopScanImage = true;
                    break;
                }
            }
            //循环判断 开始采图的状态是否为0 如果开始采图的指令为true 那么给扫描站发送一个指令开始扫图
            while (!IsReadyStopScanImage && CheckSystemConditionAction());
            BuryingPoint(UpdateStatusNameAction("StartSignalSuccess"));

            if (CheckSystemConditionAction() && IsScaning)
            {
                //如果连接了扫描站，就通知扫描站开始扫图
                if (IsScanCanScan())
                {
                    SX_SetLight(intPtr);
                    Thread.Sleep(100);
                    StartARMScan();
                    SetCommand(CommandDic[Command.LightCollectionEnd], true);
                }
                else
                {
                    messageBoxAction.Invoke("UnConnectionWithScan");
                }
                _object();
            }
            else
            {
                BuryingPoint(UpdateStatusNameAction("StopScanning"));
                IsScaning = false;
                CancelScan();
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
                if (!IsInative() || IsStopScan()
                      || Common.MoveAlarm())
                {
                    BuryingPoint(UpdateStatusNameAction("StopScaningInAcquiring"));
                    IsScaning = false;
                    Thread.Sleep(100);
                    Common.StopScan();
                    //出错了发送一个扫描站未就绪
                    SetCommand(CommandDic[Command.StationReady], false);
                    return;
                }
                //等待停止扫图的信号
                if ((!IsReadSendDarkInfo) && PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.StopImageStr))
                {
                    //这里给方海涛发送一个暗/亮矫正
                    IsReadSendDarkInfo = true;
                    break;
                }
            }
            //循环判断 开始采图的状态是否为0 如果开始采图的指令为true 那么给扫描站发送一个指令开始扫图
            while (!IsReadSendDarkInfo && CheckSystemConditionAction());
            BuryingPoint(UpdateStatusNameAction("StopSignalSuccessfully"));

            if (CheckSystemConditionAction() && IsScaning)
            {
                BuryingPoint(UpdateStatusNameAction("SendStopSignaltoScanning"));
                //如果连接了扫描站，就通知扫描站开始扫图
                if (IsScanCanScan()) { SX_Stop(intPtr); }
                else
                {
                    messageBoxAction.Invoke("UnConnectionWithScan");
                }
            }
            else
            {
                BuryingPoint(UpdateStatusNameAction("StopScanning"));
                //出错了取消扫描
                CancelCallBack?.Invoke();
                ClearScanImageCallBack?.Invoke();
                CancelScan();
                return;
            }
            BuryingPoint(UpdateStatusNameAction("ScanningEnd"));
            IsScaning = false;
            StartARMEnd();
            InitScanCondition();
            SX_Stop(intPtr);
            messageBoxAction.Invoke("ScanSuccess");
            ClearScanImageCallBack?.Invoke();
            ScanCompleteCallBack?.Invoke();
            Thread.Sleep(500);
        }
        #endregion


        /// <summary>
        /// 系统出错停止扫描
        /// </summary>
        private void CancelScan()
        {
            IsScaning = false;
            Common.StopScan();
            CancelCallBack?.Invoke();
        }
        /// <summary>
        /// 给Arm发送开始扫描的信号
        /// </summary>
        private void StartARMScan()
        {
            Task.Factory.StartNew(() =>
            {
                int SendCount = 0;
                bool IsSuccess = false;
                while (!IsSuccess && SendCount <= 3)
                {
                    IsSuccess = GlogbalArmProtocol.InqurStartScan();
                    if (!IsSuccess) SendCount++;
                }
            });
        }
        /// <summary>
        /// 给Arm发送开始扫描的信号
        /// </summary>
        private void StartARMEnd()
        {
            Task.Factory.StartNew(() =>
            {
                int SendCount = 0;
                bool IsSuccess = false;
                while (!IsSuccess && SendCount <= 3)
                {
                    IsSuccess = GlogbalArmProtocol.InqurEndScan();
                    if (!IsSuccess) SendCount++;
                }
            });
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
                            logCallBack.Invoke("Cancel scan successfully ", LogType.ScanStep, true);
                            return;
                        }
                        //如果被动扫描过程中，突然出现了主动/被动模式调整，那么就强行暂停
                        //并且把IsClicking置为false
                        if (JudgeScanCondition())
                        {
                            StopScanCurrent();
                            BuryingPoint(UpdateStatusNameAction("StopScanning"));
                            return;
                        }
                        InitScanCondition();
                        SetCommand(CommandDic[Command.StartScan], true);


                        logCallBack.Invoke($"Command.StartScan {Command.StartScan} 发送完毕，值为：{true}", LogType.ScanStep, true);

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
                //向前
                if (PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.ScanForWard))
                {
                    //Log.GetDistance().WriteInfoLogs("设置向前");
                    SX_SetDirection(intPtr, 0);
                }
                else
                {
                    //Log.GetDistance().WriteInfoLogs("设置向后");
                    SX_SetDirection(intPtr, 1);
                }
                InitiaRecv(InitiaLightImage);
            }
            catch (Exception ex)
            {
                CommonDeleget.HandTaskException(ex);
            }
        }
    }
}
