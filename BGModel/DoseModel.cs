using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGModel
{
    public class DoseModel : BaseNotifyPropertyChanged
    {
        private string _BgDoseIndex = string.Empty;
        private string _BgDoseName = string.Empty;
        private string _BgIsShow = string.Empty;
        private string _BgUnit = string.Empty;
        private string _BS_ThorValue = string.Empty;
        private string _BG_multiple = "10";
        private string _BG_ValueType = "float";
        

        /// <summary>
        /// 故障名称
        /// </summary>
        [BaseAttribute(Name = "BG_DOSE_INDEX", Description = "查询的剂量索引")]
        public string BgDoseIndex
        {
            get { return _BgDoseIndex; }
            set
            {
                _BgDoseIndex = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("BgDoseIndex"));
            }
        }



        /// <summary>
        /// 故障名称
        /// </summary>
        [BaseAttribute(Name = "BG_DOSE_NAME", Description = "剂量名称")]
        public string BgDoseName
        {
            get { return _BgDoseName; }
            set
            {
                _BgDoseName = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("BgDoseName"));
            }
        }
        /// <summary>
        /// 故障名称
        /// </summary>
        [BaseAttribute(Name = "BG_IS_SHOW", Description = "是否显示")]
        public string BgIsShow
        {
            get { return _BgIsShow; }
            set
            {
                _BgIsShow = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("BgIsShow"));
            }
        }
        /// <summary>
        /// 单位
        /// </summary>
        [BaseAttribute(Name = "BG_UNIT", Description = "显示单位")]
        public string BgUnit
        {
            get { return _BgUnit; }
            set
            {
                _BgUnit = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("BgUnit"));
            }
        }

        /// <summary>
        /// 阈值，超出显示红色
        /// </summary>
        [BaseAttribute(Name = "BS_ThorValue", Description = "阈值")]
        public string BS_ThorValue
        {
            get { return _BS_ThorValue; }
            set
            {
                _BS_ThorValue = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("BgUnit"));
            }
        }

        /// <summary>
        /// PLC传过来的值需要做int - float 转换，具体倍数需要根据PLC的倍数来
        /// </summary>
        [BaseAttribute(Name = "BG_multiple", Description = "阈值")]
        public string BG_multiple
        {
            get { return _BG_multiple; }
            set
            {
                _BG_multiple = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("BG_multiple"));
            }
        }
        /// <summary>
        /// 判断应该用什么值类型接收
        /// </summary>
        [BaseAttribute(Name = "BG_ValueType", Description = "接收的值类型")]
        public string BG_ValueType
        {
            get { return _BG_ValueType; }
            set
            {
                _BG_ValueType = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("BG_ValueType"));
            }
        }
    }

    public class PLCDBStatus : BaseNotifyPropertyChanged
    {
        ObservableCollection<bool> _IPositions = new ObservableCollection<bool>();
        public ObservableCollection<bool> IPositions
        {
            get { return _IPositions; }
            set
            {
                _IPositions = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("IPositions"));
            }
        }

        ObservableCollection<bool> _OPositions = new ObservableCollection<bool>();
        public ObservableCollection<bool> OPositions
        {
            get { return _OPositions; }
            set
            {
                _OPositions = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("OPositions"));
            }
        }
        ObservableCollection<Int16> _IntArray = new ObservableCollection<Int16>();
        public ObservableCollection<Int16> IntArray
        {
            get { return _IntArray; }
            set
            {
                _IntArray = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("_IntArray"));
            }
        }
        ObservableCollection<int> _DIntArray = new ObservableCollection<int>();
        public ObservableCollection<int> DIntArray
        {
            get { return _DIntArray; }
            set
            {
                _DIntArray = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("DIntArray"));
            }
        }
        ObservableCollection<float> _FloatArray = new ObservableCollection<float>();
        public ObservableCollection<float> FloatArray
        {
            get { return _FloatArray; }
            set
            {
                _FloatArray = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("FloatArray"));
            }
        }
    }
}
