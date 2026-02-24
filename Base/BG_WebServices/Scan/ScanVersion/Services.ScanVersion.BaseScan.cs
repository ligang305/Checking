using CMW.Common.Utilities;
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
    public abstract class BaseScan : IScan
    {

        protected  Dictionary<string, string> BoostringModelDic = new Dictionary<string, string>();

        //一些用于异步线程的多线程任务
        protected static Task UnActiveTask;
        protected static CancellationTokenSource tokenSource;
        protected static CancellationToken token;
        protected  bool IsReadyStartScanImage = false;
        protected  bool IsReadyStopScanImage = false;
        protected  bool IsReadSendDarkInfo = false;
        /// <summary>
        /// 日志回调函数
        /// </summary>
        protected  LogAction logCallBack;
        /// <summary>
        /// 弹框信息回调函数
        /// </summary>
        protected  MessageBoxAction messageBoxAction;
        /// <summary>
        /// 取消扫描的回调函数
        /// </summary>
        protected  CallBackAction CancelCallBack;
        /// <summary>
        /// 清空扫描图片的一些状态
        /// </summary>
        protected  CallBackAction ClearScanImageCallBack;
        /// <summary>
        /// 扫描完成
        /// </summary>
        protected  CallBackAction ScanCompleteCallBack;
        /// <summary>
        /// 扫描中断
        /// </summary>
        protected  NotifyBooleanCallBackAction ScanSuspendCallBack;
        /// <summary>
        /// 当前车载版本
        /// </summary>
        protected  ControlVersion CurrentCv;
        public  bool _IsScaning = true;
        /// <summary>
        /// 判断是否在扫描
        /// </summary>
        protected  bool IsScaning
        {
            get { return _IsScaning; }
            set
            {
                _IsScaning = value;
                ScanSuspendCallBack?.Invoke(_IsScaning);
            }
        }
        #region 设置

        /// <summary>
        /// 设置当前车辆版本
        /// </summary>
        public void SetControlVersion(ControlVersion _cv)
        {
            CurrentCv = _cv;
        }
        /// <summary>
        /// 设置日志回调函数
        /// </summary>
        /// <param name="_logAction"></param>
        public void SetLogActionCallBack(LogAction _logAction)
        {
            logCallBack = _logAction;
        }
        /// <summary>
        /// 设置弹框回调函数
        /// </summary>
        /// <param name="_messageBoxAction"></param>
        public void SetMessageBoxActionCallBack(MessageBoxAction _messageBoxAction)
        {
            messageBoxAction = _messageBoxAction;
        }
        /// <summary>
        /// 设置取消扫描回调函数
        /// </summary>
        /// <param name="_CancelCallBack"></param>
        public void SetCancelCallBack(CallBackAction _CancelCallBack)
        {
            CancelCallBack = _CancelCallBack;
        }
        /// <summary>
        /// 设置清空图片数据回调
        /// </summary>
        /// <param name="_clearScanImageAction"></param>
        public void SetClearScanImageCallBack(CallBackAction _clearScanImageAction)
        {
            ClearScanImageCallBack = _clearScanImageAction;
        }
        /// <summary>
        /// 设置扫描完成回调函数
        /// </summary>
        /// <param name="__CompleteCallBack"></param>
        public void SetScanCompleteCallBack(CallBackAction __CompleteCallBack)
        {
            ScanCompleteCallBack = __CompleteCallBack;
        }
        /// <summary>
        /// 设置扫描中断回调函数
        /// </summary>
        /// <param name="__CompleteCallBack"></param>
        public void SetSuspendCallBack(NotifyBooleanCallBackAction __ScanSuspendCallBack)
        {
            ScanSuspendCallBack = __ScanSuspendCallBack;
        }

        #endregion
        /// <summary>
        /// 判断由于 暂停扫描、网络断连引起的中止扫描的操作
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsStopScan()
        {
            return !IsScaning || !IsGotoConnnection || !IsConnection;
        }
        /// <summary>
        /// 
        /// </summary>
        protected void StopScanCurrent()
        {
            IsScaning = false;
            BuryingPoint($"{UpdateStatusNameAction("StopScaningSuccessfully")}");
            //出错了，或者停止扫描，发送开始扫描指令为false
            SetCommand(CommandDic[Command.StartScan], false);
            //发送工作站状态为未就绪
            SetCommand(CommandDic[Command.StationReady], false);
            Common.StopScan();
            CancelCallBack?.Invoke();
        }

        /// <summary>
        /// 停止扫描
        /// </summary>
        public void ScanStop()
        {
            IsScaning = false;
            BuryingPoint($"{UpdateStatusNameAction("StopScaningSuccessfully")}");
            logCallBack.Invoke($"停止扫描成功", LogType.ScanStep);
            //发送工作站状态为未就绪
            SetCommand(CommandDic[Command.StationReady], false);
            Common.StopScan();
            CancelCallBack?.Invoke();
        }

        public virtual void BD_Scan()
        {
            
        }

        public virtual void Scan_Start()
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

        public virtual void Scan_Stop()
        {
           
        }

        public virtual void ZD_Scan()
        {
          
        }

        public void SetBoostingModel(Dictionary<string,string> _BoostringModelDic)
        {
            BoostringModelDic = _BoostringModelDic;
        }

        protected virtual void InitScanCondition()
        {
            try
            {
                //这一步相当于整个流程结束了 把初始值设置为false;等待下一个点击按钮
                IsReadyStartScanImage = false;
                IsReadSendDarkInfo = false;
                IsReadyStopScanImage = false;
                PLCControllerManager.GetInstance().WritePositionValue(CommandDic[Command.StartScan], false);
                PLCControllerManager.GetInstance().WritePositionValue(CommandDic[Command.DarkCollectionEnd], false);
                PLCControllerManager.GetInstance().WritePositionValue(CommandDic[Command.LightCollectionEnd], false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
