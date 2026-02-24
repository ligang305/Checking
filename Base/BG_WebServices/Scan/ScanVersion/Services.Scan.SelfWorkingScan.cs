using BG_Entities;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.ImageImportDll;

namespace BG_Services
{
    /// <summary>
    /// 适用于自行走扫描或同类扫描
    /// </summary>
    public class SelfWorkingScan:BaseScan
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
        #region 给扫描站发送扫描指令


        #region 被动模式给扫描站发送的指令
        /// <summary>
        /// 这是给扫描站发送开始扫描的指令
        /// </summary>
        /// <param name="_object"></param>
        private void recv(CallBackAction _object)
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
                _object();
            }
            else
            {
                BuryingPoint($"{UpdateStatusNameAction("StopScanning")}");
                CancelScan();
                return;
            }
        }

        /// <summary>
        /// 停止扫描之后，接收到PLC给过来的指令，然后通过采集空气状态给扫描站发送暗矫正指令 && (GlobalRetStatus[54]
        /// </summary>
        private void StartSendDarkImage()
        {
            BuryingPoint($"{UpdateStatusNameAction("GetStopScanSignaling")}!");
            do
            {
                Thread.Sleep(10);
                if (IsStopScan() || IsInative() || Common.SelfAutoMoveAlarm())
                {
                    BuryingPoint($"{UpdateStatusNameAction("StopScaningInDarkCorrection")}");
                    return;
                }

                //接收到开始采集暗矫正命令的时候
                if ((!IsReadSendDarkInfo) && !PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.StartImage))
                {
                    //这里给方海涛发送一个暗/亮矫正
                    IsReadSendDarkInfo = true;
                    break;
                }
            }
            //循环判断 开始采图的状态是否为0 如果开始采图的指令为true 那么给扫描站发送一个指令开始扫图
            while (!IsReadSendDarkInfo && CheckSystemConditionAction());
            BuryingPoint($"{UpdateStatusNameAction("StopSignalSuccessfully")}!");
            if (CheckSystemConditionAction() && IsScaning)
            {
                //如果连接了扫描站，就通知扫描站开始扫图
                if (IsScanCanScan())
                {
                    try
                    { SX_Stop(intPtr); }
                    catch { }
                }
                else
                {
                    messageBoxAction?.Invoke("UnConnectionWithScan");
                }
            }
            else
            {
                CancelScan();
                return;
            }

            BuryingPoint($"{UpdateStatusNameAction("ScanningEnd")}");
            Thread.Sleep(100);
            InitScanCondition();
            try
            { SX_Stop(intPtr); }
            catch { }
            //ClearSmallImage();
            Thread.Sleep(500);
        }
        #endregion

        #region 主动模式给扫描站发送的指令
        /// <summary>
        /// 这是给扫描站发送开始扫描的指令
        /// </summary>
        /// <param name="_object"></param>
        private void InitiaRecv(CallBackAction _object)
        {
            BuryingPoint($"{UpdateStatusNameAction("Initiacanning")}");
            BuryingPoint($"{UpdateStatusNameAction("QueryStartSignal")}");
            BuryingPoint($"{UpdateStatusNameAction("InitScanCondition")}");
            InitScanCondition();
            SetCommand(CommandDic[Command.StartScan], true);

            do
            {
                Thread.Sleep(10);
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
                    //SetCommand(CommandDic[Command.StationReady], false);
                    return;
                }
                //主动模式等待暗矫正型信号
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
                //一切正常，发送暗矫正指令
                if (IsScanCanScan())
                {
                    SX_Start(intPtr);

                    //SX_SetDark(intPtr);
                    //Thread.Sleep(2000);
                    //SX_SetLight(intPtr);
                }
                else
                {
                    messageBoxAction?.Invoke("UnConnectionWithScan");
                }
                //这里进行一个函数回调，去找是否停止扫图的命令
                _object();
            }
            else
            {
                CancelScan();
                return;
            }
        }

        /// <summary>
        /// 停止扫描之后，接收到PLC给过来的指令，然后通过采集空气状态给扫描站发送暗矫正指令
        /// </summary>
        private void InitiaStopImage()
        {
            BuryingPoint($"{UpdateStatusNameAction("GetStopScanSignaling")}!");
            do
            {
                Thread.Sleep(10);
                if (!IsInative() || IsStopScan()
                      || Common.SelfAutoMoveAlarm())
                {
                    BuryingPoint($"{UpdateStatusNameAction("StopScaningInAcquiring")}");
                    IsScaning = false;
                    Thread.Sleep(100);
                    Common.StopScan();
                    //出错了发送一个扫描站未就绪
                    SetCommand(CommandDic[Command.StationReady], false);
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
            BuryingPoint($"{UpdateStatusNameAction("StopSignalSuccessfully")}!");

            if (CheckSystemConditionAction() && IsScaning)
            {
                BuryingPoint($"{UpdateStatusNameAction("SendStopScanningSignal")}!");
                //如果连接了扫描站，就通知扫描站开始扫图
                if (IsScanCanScan())
                {
                    try
                    {
                        SX_Stop(intPtr);
                    }
                    catch
                    {
                    } 
                    BuryingPoint($"SX_Stop(intPtr) Row249!"); }
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
            BuryingPoint($"{UpdateStatusNameAction("ScanningEnd")}");
            IsScaning = false;
            InitScanCondition();
            SX_Stop(intPtr);
            messageBoxAction?.Invoke("ScanSuccess");
            ClearScanImageCallBack?.Invoke();
            ScanCompleteCallBack?.Invoke();
        }
        #endregion
        /// <summary>
        /// 出异常停止的方法
        /// </summary>
        private void CommonStopScan()
        {
            StopScan();
            CancelCallBack?.Invoke();
        }


        #endregion

        /// <summary>
        /// 系统出错停止扫描
        /// </summary>
        private void CancelScan()
        {
            IsScaning = false;
            logCallBack?.Invoke("停止扫描成功", LogType.ScanStep, true);
            BuryingPoint($"{UpdateStatusNameAction("StopScanSuccess")}");
            //出错了，或者停止扫描，发送开始扫描指令为false
            Common.StopScan();
            CancelCallBack?.Invoke();
        }
        public override void BD_Scan()
        {
            ImageImportDll.SX_SetMode(ImageImportDll.intPtr, 0);
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
                            BuryingPoint($"{UpdateStatusNameAction("StopScanning")}");
                            return;
                        }
                        InitScanCondition();
                        SetCommand(CommandDic[Command.StartScan], true);
                        logCallBack?.Invoke($"Command.StartScan {Command.StartScan} 发送完毕，值为：{true}", LogType.ScanStep, true);

                        recv(StartSendDarkImage);
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
