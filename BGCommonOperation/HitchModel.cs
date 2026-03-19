using BGModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace BGCommonOperation
{
    /// <summary>
    /// 故障列表
    /// </summary>

    [BaseAttribute(Name = "BG_HITCH", Description = "车体与悬臂控制与检测")]
    public class HitchModel : BaseNotifyPropertyChanged
    {

        private string _HitchName = string.Empty;
        private string _HitchCode = string.Empty;
        private string _HitchOwner = string.Empty;
        private string _HitchIndex = string.Empty;
        /// <summary>
        /// 故障名称
        /// </summary>
        [BaseAttribute(Name = "BG_HITCHINDEX", Description = "故障名称")]
        public string HitchIndex
        {
            get { return _HitchIndex; }
            set
            {
                _HitchIndex = value;
                OnPropertyChanged(new PropertyChangedEventArgs("HitchIndex"));
            }
        }
        /// <summary>
        /// 故障名称
        /// </summary>
        [BaseAttribute(Name = "BG_HITCHNAME", Description = "故障名称")]
        public string HitchName
        {
            get { return _HitchName; }
            set {
                _HitchName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("HitchName"));
            }
        }
        /// <summary>
        /// 故障编码 对应PLC查询
        /// </summary>
        [BaseAttribute(Name = "BG_HITCHCODE", Description = "故障编码")]
        public string HitchCode
        {
            get { return _HitchCode; }
            set
            {
                _HitchCode = value;
                OnPropertyChanged(new PropertyChangedEventArgs("HitchCode"));
            }
        }
        /// <summary>
        /// 故障加速器来源 
        /// 达胜还是
        /// </summary>
        [BaseAttribute(Name = "BG_HITCHOWNER", Description = "故障来源")]
        public string HitchOwner
        {
            get { return _HitchOwner; }
            set
            {
                _HitchOwner = value;
                OnPropertyChanged(new PropertyChangedEventArgs("HitchOwner"));
            }
        }
    }
}
