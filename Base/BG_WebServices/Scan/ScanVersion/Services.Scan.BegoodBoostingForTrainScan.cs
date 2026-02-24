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
    [CustomExportMetadata(1, "BegoodBoostingForTrainScan", "CGN. Begood", "zhuzhiwu", "1.0.0")]
    public class BegoodBoostingForTrainScan : BaseScan
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

                        ReceiveStartSignal(ReceiveStopSignal);
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
            CancelCallBack?.Invoke();
            logCallBack?.Invoke("Stop scanning successfully ", LogType.ScanStep, true);
            BuryingPoint($"{UpdateStatusNameAction("StopScaningSuccessfully")}");
            RequestTaskAndStop();
            InitScanCondition();
        }

        #region 被动模式给扫描站发送的指令
        /// <summary>
        /// 这是给扫描站发送开始扫描的指令
        /// </summary>
        /// <param name="_object"></param>
        [HandleProcessCorruptedStateExceptions]
        private void ReceiveStartSignal(CallBackAction _object)
        {
            BuryingPoint($"Start TrainScan");
            do
            {
                Thread.Sleep(5);
                //该函数正在执行，但是当点击了停止扫描之后就跳出
                if (IsStopScan())
                {
                    BuryingPoint($"Interrupt ReceiveStartSignal,Because IsScaning:{IsScaning} ,IsGotoConnnection:{IsGotoConnnection},IsConnection:{IsConnection}");
                    break;
                }
                //当发现状态为1且已经通知过扫描站 就不再进行通知扫描站了 ，然后跳出循环，直到判断停止扫图状态为1
                if ((!IsReadyStartScanImage) && PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.StartImage))
                {
                    #region 火车要在收到了开始扫描信号的时候去申请任务，且最好提前一秒给我信号
                    //最多等待500ms,没有那就没有
                    Task.Run(() => {
                        BuryingPoint($"{UpdateStatusNameAction("BrightCorrectionStart")}！");
                        RequestTaskAction();
                        BuryingPoint($"RequestTask  End ！");
                    }).Wait(500);
                    #endregion
                    SX_Start(intPtr);
                    BuryingPoint($"Get StartImage Signal in while");
                    IsReadyStartScanImage = true;
                    break;
                }
            }
            //循环判断 开始采图的状态是否为0 如果开始采图的指令为true 那么给扫描站发送一个指令开始扫图
            while (!IsReadyStartScanImage && CheckSystemConditionAction());

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
                int CarriageId = 1;
                bool CarriageSegmentationSingnal = false;
                do
                {
                    Thread.Sleep(5);
                    if (IsStopScan())
                    {
                        BuryingPoint($"Interrupt ReceiveStopSignal,Because IsScaning:{IsScaning} ,IsGotoConnnection:{IsGotoConnnection},IsConnection:{IsConnection}");
                        return;
                    }

                    //接收到停止扫描信号
                    if ((!IsReadSendDarkInfo) && PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.StopImageStr))
                    {
                        //这里给方海涛发送一个暗/亮矫正
                        IsReadSendDarkInfo = true;
                        SX_Stop(intPtr);
                        break;
                    }
                    if(PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.CarriageSegmentation))
                    {
                        if (!CarriageSegmentationSingnal)
                        {
                            CarriageSegmentationSingnal = true;
                            //TODO 调用方工车厢分隔函数 
                            //可能要获取车厢号用于图像编号
                            //调用分割函数 40/60现在无法获取 等从PLC获取到了再设置
                            float CarSpeed = Common.GlobalDoseStatus[0];
                            float CarLength = Common.GlobalDoseStatus[1];
                            DetecotrControllerManager.GetInstance().SX_SetFrequencyBySpeed(Common.GlobalDoseStatus[0]);
                            int CarNo = Common.GlobalDoseStatus[9];
                            SX_Segmentation(intPtr, CarriageId, CarLength, CarSpeed);
                            WriteLogAction($@"CarriageId :{CarriageId},CarNo:{CarNo},Carlength:{CarLength},CarSpeed:{CarSpeed}",LogType.NormalLog,false);
                            CarriageId++;
                        }
                    }
                    else
                    {
                        CarriageSegmentationSingnal = false;
                    }
                }
                //循环判断 是否满足扫描的安全条件具体见CheckSystemConditionAction()
                while (!IsReadSendDarkInfo && CheckSystemConditionAction());

                if (!(CheckSystemConditionAction() && IsScaning))
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


        #region 主动模式给扫描站发送的指令
        /// <summary>
        /// 这是给扫描站发送开始扫描的指令
        /// </summary>
        /// <param name="_object"></param>
        private void InitiaRecv(CallBackAction _object)
        {
            BuryingPoint(UpdateStatusNameAction("Initiacanning"));
            InitScanCondition();
            BuryingPoint(UpdateStatusNameAction("SendStartScanSignal") + "SX_Start");
            var result = PLCControllerManager.GetInstance().WritePositionValue(Common.CommandDic[Command.StartScan], true);
            BuryingPoint($"{UpdateStatusNameAction("SendStartScanSignalCompleted")}{Common.CommandDic[Command.StartScan]};result:{result}");
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
                if ((!IsReadyStartScanImage) && PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.StartImage))
                {
                    SX_Start(intPtr);
                    BuryingPoint($"Get StartImage Signal in while");
                    IsReadyStartScanImage = true;
                    break;
                }
            }
            //循环判断 开始采图的状态是否为0 如果开始采图的指令为true 那么给扫描站发送一个指令开始扫图
            while (!IsReadyStartScanImage && CheckSystemConditionAction());


            if (CheckSystemConditionAction() && IsScaning)
            {
                //如果连接了扫描站，就通知扫描站开始扫图
                if (DetecotrControllerManager.GetInstance().DetectorConnection != 3)
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
                if ((!IsReadSendDarkInfo) && PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.StopImageStr))
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
                        {
                            RequestTaskAndStop();
                        }
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
