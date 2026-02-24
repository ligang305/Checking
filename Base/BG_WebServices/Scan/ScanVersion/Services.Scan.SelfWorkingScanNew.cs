using CMW.Common.Utilities;
using BGCommunication;
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
    public class SelfWorkingScanNew : BaseScan
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
        private void recv(CallBackParamaterAction _object)
        {
            BuryingPoint($"Start Bd Scan");
            do
            {
                Thread.Sleep(10);
                //该函数正在执行，但是主被动扫描模式变化了之后就跳出
                if (IsInative() || IsStopScan() || Common.SelfAutoMoveAlarm())
                {
                    BuryingPoint($"Stop Scan in IsInative():{IsInative()};IsStopScan:{IsStopScan()};SelfAutoMoveAlarm：{SelfAutoMoveAlarm()}");
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
                        SetCommand(CommandDic[Command.LightCollectionEnd], true);
                        Thread.Sleep(500);
                        SetCommand(CommandDic[Command.LightCollectionEnd], false);
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
                        SetCommand(CommandDic[Command.DarkCollectionEnd],   true);    
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

        #region 主动模式给扫描站发送的指令
        //判定是否预热
        static bool isPreHot = false;
        /// <summary>
        /// 出束
        /// </summary>
        //private void Ray()
        //{
        //    BuryingPoint($"{UpdateStatusNameAction("ActiveModeGettingAcceleratorParameters")} ");
        //    string volModel = BoostringModelDic["InjectionCurrent"];
        //    string HMC = BoostringModelDic["HeightEnergyMC"];
        //    string H = BoostringModelDic["HeightEnergy"];
        //    string LMC = BoostringModelDic["LowerEnergyMC"];
        //    string L = BoostringModelDic["LowerEnergy"];
        //    if (volModel.Equals(string.Empty))
        //    {
        //        messageBoxAction?.Invoke("电流不能为空！");
        //        return;
        //    }
        //    if (HMC.Equals(string.Empty))
        //    {
        //        messageBoxAction?.Invoke("高能脉冲不能为空！");
        //        //BG_MESSAGEBOX.Show(Application.Current?.MainWindow, "Tip", "高能脉冲不能为空！");
        //        return;
        //    }
        //    if (H.Equals(string.Empty))
        //    {
        //        messageBoxAction?.Invoke("高能不能为空！");
        //        //BG_MESSAGEBOX.Show(Application.Current?.MainWindow, "Tip", "高能不能为空！");
        //        return;
        //    }
        //    if (LMC.Equals(string.Empty))
        //    {
        //        messageBoxAction?.Invoke("低能脉冲不能为空！");
        //        //BG_MESSAGEBOX.Show(Application.Current?.MainWindow, "Tip", "低能脉冲不能为空！");
        //        return;
        //    }
        //    if (L.Equals(string.Empty))
        //    {
        //        messageBoxAction?.Invoke("低能不能为空！");
        //        //BG_MESSAGEBOX.Show(Application.Current?.MainWindow, "Tip", "低能不能为空！");
        //        return;
        //    }
        //    isPreHot = true;
        //    //float vol = (float)Convert.ToDouble(txtvol.Text);
        //    //Task.Factory.StartNew(() =>
        //    //{
        //    Thread.Sleep(60);//这里停51毫秒的目的是让停止预热的线程跳出来
        //    if (!isPreHot)
        //    {
        //        return;
        //    }
        //    DeviceI di = new DeviceI();
        //    HAndL mhl = new HAndL();

        //    //while (WJB != WorkingJob.WJ_WarmUp && isPreHot)
        //    {
        //        //string RealValue = volModel.IsUseDefalut.ToLower() == "true" ? volModel.ActureValue : volModel.Value;
        //        BuryingPoint($"{UpdateStatusNameAction("ActiveModeGettingAcceleratorToWarmUp")}WarmUp ");
        //        if (!IsScaning) return;

        //        //BoostingControllerManager.GetInstance().WarmUp();
        //        (GlobalBetatronProtocol as BetatronProtocol).WarmUp();
        //        Thread.Sleep(170);
        //        BuryingPoint($"{UpdateStatusNameAction("ActiveModeSendOutPut")} Execute");
        //        if (!IsScaning) return;
        //        (GlobalBetatronProtocol as BetatronProtocol).Execute(Convert.ToInt32(volModel), ref di);
        //    }
        //    Thread.Sleep(3000);
        //    string HValue = H;//.IsUseDefalut.ToLower() == "true" ? H.ActureValue : H.Value;
        //    string LValue = L;//.IsUseDefalut.ToLower() == "true" ? L.ActureValue : L.Value;
        //    string HMCValue = HMC;// HMC.IsUseDefalut.ToLower() == "true" ? HMC.ActureValue : HMC.Value;
        //    string LMCValue = LMC;//.IsUseDefalut.ToLower() == "true" ? LMC.ActureValue : LMC.Value;
        //    BuryingPoint($"{UpdateStatusNameAction("ActiveModeAcceleratorBeam")} ExecuteHandI");
        //    (GlobalBetatronProtocol as BetatronProtocol).ExecuteHandI(Convert.ToInt32(HValue), Convert.ToInt32(HMCValue),
        //        Convert.ToInt32(LValue), Convert.ToInt32(LMCValue));
        //    Thread.Sleep(3000);
        //    //while (WJB != WorkingJob.WJ_ReadyWarm && isPreHot)
        //    {
        //        BuryingPoint($"{UpdateStatusNameAction("ActiveModeSendOutPut")} OutputBeam");
        //        Thread.Sleep(100);
        //        if (!IsScaning) return;
        //        (GlobalBetatronProtocol as BetatronProtocol).OutputBeam();
        //    }

        //    logCallBack?.Invoke($"加速器已经进入预热状态", LogType.ScanStep, true);
        //    //});
        //}

        /// <summary>
        /// 停止预热
        /// </summary>
        //private void StopPreHot()
        //{
        //    isPreHot = false;
        //    //Task.Factory.StartNew(() =>
        //    {
        //        Thread.Sleep(101);//这里停101毫秒的目的是让预热的线程跳出来
        //        while (WJB != WorkingJob.WJ_StopBeam)
        //        {
        //            if (isPreHot)
        //            {
        //                return;
        //            }
        //            Thread.Sleep(50);
        //            (GlobalBetatronProtocol as BetatronProtocol).StopBeam();
        //        }
        //    }
        //    //);
        //}

        /// <summary>
        /// 这是给扫描站发送开始扫描的指令
        /// </summary>
        /// <param name="_object"></param>
        private void InitiaRecv(CallBackAction _object)
        {
            InitScanCondition();
            BuryingPoint($"{UpdateStatusNameAction("Passivescanning")}");
            SX_Start(intPtr);
            BuryingPoint($"{UpdateStatusNameAction("BrightCorrectionStart")}");
            SX_SetDark(intPtr);
            SetCommand(CommandDic[Command.DarkCollectionEnd], true);
            Thread.Sleep(1000);
            SetCommand(CommandDic[Command.DarkCollectionEnd], false);
            BuryingPoint($"{UpdateStatusNameAction("DarkCorrectionEnd")}");
            BuryingPoint($"{UpdateStatusNameAction("ActiveModeSendOutPut")} ");
            //Ray();
            RayAction();
            Thread.Sleep(10000);
            BuryingPoint($"{UpdateStatusNameAction("BrightCorrectionStart")} SX_SetLight");
            if (!IsScaning) return;
            SX_SetLight(intPtr);
            Thread.Sleep(1000);
            if (!IsScaning) return;
            BuryingPoint($"{UpdateStatusNameAction("SendStartScanSignal")} ,{CommandDic[Command.StartScan]}");
            var result = SetCommand(CommandDic[Command.StartScan], true);
            BuryingPoint($"{UpdateStatusNameAction("StartScan")} End{CommandDic[Command.StartScan]};result:{result}");

            do
            {
                Thread.Sleep(10);
                //该函数正在执行，但是当点击了停止扫描之后就跳出
                //当第一步来了之后，判断顺序有没有乱，就判断第二步有没有优于第一步先到，
                //后续添加防撞报警|| GlobalRetStatus[55] || GlobalRetStatus[46]
                if (!IsInative() || !IsScaning || Common.SelfAutoMoveAlarm())
                {
                    BuryingPoint($"!IsInative() = {!IsInative()}; IsStopScan() = { IsStopScan()};{!IsScaning};{!IsGotoConnnection};{!IsConnection};Common.SelfAutoMoveAlarm() = {Common.SelfAutoMoveAlarm()}");
                    BuryingPoint($"{UpdateStatusNameAction("StopAcquisitionScanning")}");
                    IsScaning = false;
                    Thread.Sleep(100);
                    Common.StopScan();
                    CancelCallBack?.Invoke();
                    SetCommand(CommandDic[Command.StartScan], false);
                    //SetCommand(CommandDic[Command.StationReady], false);
                    StopPrehotAction();
                    //StopPreHot();
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
                CancelScan();
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
                if (!IsInative() || !IsScaning
                      || Common.SelfAutoMoveAlarm())
                {
                    BuryingPoint($"!IsInative() = {!IsInative()}; IsStopScan() = { IsStopScan()};{!IsScaning};{!IsGotoConnnection};{!IsConnection};Common.SelfAutoMoveAlarm() = {Common.SelfAutoMoveAlarm()}");
                    BuryingPoint($"{UpdateStatusNameAction("StopScaningInAcquiring")}");
                    IsScaning = false;
                    Thread.Sleep(100);
                    //SX_Stop(intPtr);
                    Common.StopScan();
                    //出错了发送一个扫描站未就绪
                    //SetCommand(CommandDic[Command.StationReady], false);
                    StopPrehotAction();
                    //StopPreHot();
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
                //如果连接了扫描站，就通知扫描站开始扫图
                //if (IsScanCanScan()) {
                //    try
                //    { SX_Stop(intPtr); }
                //    catch { }
                //    BuryingPoint($"SX_Stop(intPtr) Row249!"); }
                //else
                //{
                //    messageBoxAction?.Invoke("UnConnectionWithScan");
                //}
            }
            else
            {
                ClearScanImageCallBack?.Invoke();
                CancelScan();
                return;
            }
            BuryingPoint($"{UpdateStatusNameAction("ScanningEnd")}");
            StopPrehotAction();
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
            logCallBack?.Invoke("停止扫描成功", LogType.ScanStep, true);
            BuryingPoint($"{UpdateStatusNameAction("StopScanSuccess")}");
            //出错了，或者停止扫描，发送开始扫描指令为false
            Common.StopScan();
            StopPrehotAction();
            //StopPreHot();
            CancelCallBack?.Invoke();
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
