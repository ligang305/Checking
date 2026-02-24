using BG_Entities;
using CMW.Common.Utilities;
using System.Collections.Generic;

namespace BG_Services
{
    /// <summary>
    /// 扫描函数类
    /// </summary>
    public class ScanHelper :BaseInstance<ScanHelper>,IScan
    {

        private IScan Scan;

        public void SetScan(IScan _Scan)
        {
            Scan = _Scan;
        }

        public void Stop()
        {
            Scan.Scan_Stop();
        }
        public void Start()
        {
            Scan.Scan_Start();
        }
        /// <summary>
        /// 开始扫描流程
        /// </summary>
        /// <param name="cv">车辆控制版本</param>
        public void Scan_Start()
        {
            Scan.Scan_Start();
        }
        /// <summary>
        /// 停止扫描流程
        /// </summary>
        public void Scan_Stop()
        {
            Scan.Scan_Stop();
        }

        public void BD_Scan()
        {
            Scan.BD_Scan();
        }

        public void ZD_Scan()
        {
            Scan.ZD_Scan();
        }

        public void SetControlVersion(ControlVersion _cv)
        {
            Scan.SetControlVersion(_cv);
        }

        public void SetLogActionCallBack(CommonDeleget.LogAction _logAction)
        {
            Scan.SetLogActionCallBack(_logAction);
        }

        public void SetMessageBoxActionCallBack(CommonDeleget.MessageBoxAction _messageBoxAction) 
        {
            Scan.SetMessageBoxActionCallBack(_messageBoxAction);
        }

        public void SetCancelCallBack(CommonDeleget.CallBackAction _CancelCallBack)
        {
            Scan.SetCancelCallBack(_CancelCallBack);
        }

        public void SetClearScanImageCallBack(CommonDeleget.CallBackAction _clearScanImageAction)
        {
            Scan.SetClearScanImageCallBack(_clearScanImageAction);
        }

        public void SetScanCompleteCallBack(CommonDeleget.CallBackAction __CompleteCallBack)
        {
            Scan.SetScanCompleteCallBack(__CompleteCallBack);
        }

        public void SetSuspendCallBack(CommonDeleget.NotifyBooleanCallBackAction __ScanSuspendCallBack)
        {
            Scan.SetSuspendCallBack(__ScanSuspendCallBack);
        }

        public void SetBoostingModel(Dictionary<string, string> BoostingModelDic)
        {

        }
    }
}
