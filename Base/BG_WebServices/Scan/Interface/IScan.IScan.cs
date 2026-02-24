using BG_Entities;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG_Services
{
    public interface IScan
    {
  
        void Scan_Start();

        void Scan_Stop();

        void BD_Scan();

        void ZD_Scan();

        void SetControlVersion(ControlVersion _cv);

        void SetBoostingModel(Dictionary<string,string> BoostingModelDic);

        void SetLogActionCallBack(CommonDeleget.LogAction _logAction);

        void SetMessageBoxActionCallBack(CommonDeleget.MessageBoxAction _messageBoxAction);

        void SetCancelCallBack(CommonDeleget.CallBackAction _CancelCallBack);

        void SetClearScanImageCallBack(CommonDeleget.CallBackAction _clearScanImageAction);

        void SetScanCompleteCallBack(CommonDeleget.CallBackAction __CompleteCallBack);

        void SetSuspendCallBack(CommonDeleget.NotifyBooleanCallBackAction __ScanSuspendCallBack);
    }
}
