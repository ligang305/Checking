using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGModel
{
    /// <summary>
    /// 探测器板卡
    /// </summary>
    public class DetectorBoard : BaseNotifyPropertyChanged
    {
        private string _DetectorBoardTotalIndex = string.Empty;
        private string _DetectorBoardLineIndex = string.Empty;
        private string _DetectorBoardLinePosition = string.Empty;
        private string _DetectorBoardStatus = string.Empty;
        /// <summary>
        /// 探测器板卡总序号
        /// </summary>
        [BaseAttribute(Name = "DetectorBoardTotalIndex", Description = "探测器板卡总序号")]
        public string DetectorBoardTotalIndex
        {
            get { return _DetectorBoardTotalIndex; }
            set
            {
                _DetectorBoardTotalIndex = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("DetectorBoardTotalIndex"));
            }
        }
        /// <summary>
        /// 探测器板卡所在链路序号
        /// </summary>
        [BaseAttribute(Name = "DetectorBoardLineIndex", Description = "探测器板卡所在链路序号")]
        public string DetectorBoardLineIndex
        {
            get { return _DetectorBoardLineIndex; }
            set
            {
                _DetectorBoardLineIndex = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("DetectorBoardLineIndex"));
            }
        }
        /// <summary>
        /// 探测器板卡所在链路的块内部序号
        /// </summary>
        [BaseAttribute(Name = "DetectorBoardLinePosition", Description = "探测器板卡所在链路的块内部序号")]
        public string DetectorBoardLinePosition
        {
            get { return _DetectorBoardLinePosition; }
            set
            {
                _DetectorBoardLinePosition = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("DetectorBoardLinePosition"));
            }
        }
        /// <summary>
        /// 探测器板卡状态
        /// </summary>
        [BaseAttribute(Name = "DetectorBoardStatus", Description = "探测器板卡状态")]
        public string DetectorBoardStatus
        {
            get { return _DetectorBoardStatus; }
            set
            {
                _DetectorBoardStatus = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("DetectorBoardStatus"));
            }
        }
    }
}
