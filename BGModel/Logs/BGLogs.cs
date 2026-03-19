using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGModel
{
    [BaseAttribute(Name = "BG_Log", Description = "日志表")]
    public class BG_Logs : BaseNotifyPropertyChanged
    {
        public double _LogFontsize;
        public string _LogType = string.Empty;
        public string _LogName = string.Empty;
        public string _LogContent = string.Empty;
        public string _LogDataTime = string.Empty;
        public string _LogEndDataTime = "29991231235959";
        public string _LogStartDataTime = "19700101000000";
        public string _LogId = string.Empty;

        [BaseAttribute(Name = "LogId", Description = "日志Id")]
        public string LogId { get => _LogId; set { _LogId = value; RaisePropertyChanged("LogId"); } }
        [BaseAttribute(Name = "LogName", Description = "日志名称")]
        [DataGridViewAttribute(ColumnBindingName = "LogName",ColumnDisplayName = "LogName", IsShow = true, Width = 150)]
        public string LogName { get => _LogName; set { _LogName = value; RaisePropertyChanged("LogName"); } }
        [BaseAttribute(Name = "LogContent", Description = "日志内容", IsSearch = true)]
        [DataGridViewAttribute(ColumnBindingName = "LogContent", ColumnDisplayName = "LogContent", IsShow = true, Width = 470)]
        public string LogContent { get => _LogContent; set { _LogContent = value; RaisePropertyChanged("LogContent"); } }
   
        [BaseAttribute(Name = "LogDataTime", Description = "日志时间")]
        [DataGridViewAttribute(ColumnBindingName = "LogDataTime", ColumnDisplayName = "LogDataTime", IsShow = true, Width = 100)]
        public string LogDataTime { get => _LogDataTime; set { _LogDataTime = value; RaisePropertyChanged("LogDataTime"); } }

        [BaseAttribute(Name = "LogDataTime", Description = "日志起始时间", IsInsertDB = false)]
        [TimeAttribute(IsStartTime = true)]
        public string LogStartDataTime { get => _LogStartDataTime; set {
                if (value != null)
                  _LogStartDataTime = value.Length != 14?Convert.ToDateTime(value).ToString("yyyyMMddHHmmss"): value;
                RaisePropertyChanged("LogStartDataTime"); } }
        [BaseAttribute(Name = "LogDataTime", Description = "日志结束时间", IsInsertDB = false)]
        [TimeAttribute(IsEndTime = true)]
        public string LogEndDataTime { get => _LogEndDataTime; set { if (value != null) _LogEndDataTime = value.Length != 14 ? Convert.ToDateTime(value).ToString("yyyyMMddHHmmss") : value; RaisePropertyChanged("LogEndDataTime"); } }

        [BaseAttribute(Name = "LogType", Description = "日志类型")]
        [DataGridViewAttribute(ColumnBindingName = "LogType", ColumnDisplayName = "LogType", IsShow = true,Width =100)]
        public string LogType { get => _LogType; set { _LogType = value; RaisePropertyChanged("LogType"); } }

        [BaseAttribute(Name = "LogFontSize", Description = "日志的字体大小", IsInsertDB = false)]
        public double LogFontSize { get => _LogFontsize; set { _LogFontsize = value; RaisePropertyChanged("LogFontSize"); } }
        
    }

}
