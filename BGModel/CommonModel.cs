using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace BGModel
{
    public class CommonModel
    {
        public static EquipmentParamaterModel DoubleMode = new EquipmentParamaterModel();
    }
    public class ListViewModel : INotifyPropertyChanged
    {

        private double _ListViewHeaderLength;

        public double ListViewHeaderLength
        {
            get
            {
                return _ListViewHeaderLength;
            }
            set
            {
                _ListViewHeaderLength = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("ListViewHeaderLength"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }
    }


    public class ListViewLogViewMode : INotifyPropertyChanged
    {
        private string _EquipmentTime;
        /// <summary>
        /// 设备时间
        /// </summary>
        public string EquipmentTime
        {
            get
            {
                return _EquipmentTime;
            }
            set
            {
                _EquipmentTime = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("EquipmentTime"));
            }
        }


        private string _EquipmentRunLog = string.Empty;
        public string EquipmentRunLog
        {
            get { return _EquipmentRunLog; }
            set
            {
                _EquipmentRunLog = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("EquipmentRunLog"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }
    }


    public class ParamaterLabel: INotifyPropertyChanged
    {
        private double _ParamaterNameFontSize = 16.0;
        /// <summary>
        /// 显示字体
        /// </summary>
        public double ParamaterNameFontSize
        {
            get
            {
                return _ParamaterNameFontSize;
            }
            set
            {
                _ParamaterNameFontSize = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("ParamaterNameFontSize"));
            }
        }
        private double _ParamaterFontSize = 14.0;
        /// <summary>
        /// 显示字体
        /// </summary>
        public double ParamaterFontSize
        {
            get
            {
                return _ParamaterFontSize;
            }
            set
            {
                _ParamaterFontSize = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("ParamaterFontSize"));
            }
        }


        private string _ParamaterName;
        /// <summary>
        /// 设备时间
        /// </summary>
        public string ParamaterName
        {
            get
            {
                return _ParamaterName;
            }
            set
            {
                _ParamaterName = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("ParamaterName"));
            }
        }


        private string _ParamaterValue = string.Empty;
        public string ParamaterValue
        {
            get { return _ParamaterValue; }
            set
            {
                _ParamaterValue = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("ParamaterValue"));
            }
        }
        private string _ParamaterForeColor = "#014087";
        public string ParamaterForeColor
        {
            get { return _ParamaterForeColor; }
            set
            {
                _ParamaterForeColor = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("ParamaterForeColor"));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

    }
                     

    public class EquipmentParamaterModel:BaseNotifyPropertyChanged
    {
        private string _StatusName;
        private SignalModel _VauleOne;
        private SignalModel _ValueTwo;
        private SignalModel _ValueThree;
        private SignalModel _ValueFour;
        private SignalModel _ValueFive;
        private SignalModel _ValueSix;
        private SignalModel _ValueSeven;
        private SignalModel _ValueEight;
        private SignalModel _ValueNine;
        private string _ForeColor;
        private string _BackColor;
        private string _LeftColor;
        private string _RightColor;

        private Visibility _ScanPreview;

        /// <summary>
        /// 电子围栏 右侧颜色
        /// </summary>
        public string RightColor
        {
            get { return _RightColor; }
            set
            {
                _RightColor = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("RightColor"));
            }
        }
        /// <summary>
        /// 电子围栏 左侧颜色
        /// </summary>
        public string LeftColor
        {
            get { return _LeftColor; }
            set
            {
                _LeftColor = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("LeftColor"));
            }
        }
        /// <summary>
        /// 电子围栏 前侧颜色
        /// </summary>
        public string ForeColor
        {
            get { return _ForeColor; }
            set
            {
                _ForeColor = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("ForeColor"));
            }
        }
        /// <summary>
        /// 电子围栏 后侧颜色
        /// </summary>
        public string BackColor
        {
            get { return _BackColor; }
            set
            {
                _BackColor = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("BackColor"));
            }
        }
        /// <summary>
        /// 状态名称
        /// </summary>
        public Visibility ScanPreview
        {
            get { return _ScanPreview; }
            set
            {
                _ScanPreview = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("ScanPreview"));
            }
        }

        /// <summary>
        /// 状态名称
        /// </summary>
        public string StatusName
        {
            get { return _StatusName; }
            set
            {
                _StatusName = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("StatusName"));
            }
        }
        /// <summary>
        /// 状态值1
        /// </summary>
        public SignalModel VauleOne
        {
            get { return _VauleOne; }
            set
            {
                _VauleOne = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("VauleOne"));
            }
        }
        /// <summary>
        /// 车速
        /// </summary>
        public SignalModel ValueTwo
        {
            get { return _ValueTwo; }
            set
            {
                _ValueTwo = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("ValueTwo"));
            }
        }
        /// <summary>
        /// 转台角度
        /// </summary>
        public SignalModel ValueThree
        {
            get { return _ValueThree; }
            set
            {
                _ValueThree = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("ValueThree"));
            }
        }
        /// <summary>
        /// 扫描模式
        /// </summary>
        public SignalModel ValueFour
        {
            get { return _ValueFour; }
            set
            {
                _ValueFour = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("ValueFour"));
            }
        }
        /// <summary>
        /// 频率 
        /// </summary>
        public SignalModel ValueFive
        {
            get { return _ValueFive; }
            set
            {
                _ValueFive = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("ValueFive"));
            }
        }
        /// <summary>
        /// 状态值2
        /// </summary>
        public SignalModel ValueSix
        {
            get { return _ValueSix; }
            set
            {
                _ValueSix = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("ValueSix"));
            }
        }
        /// <summary>
        /// 状态值2
        /// </summary>
        public SignalModel ValueSeven
        {
            get { return _ValueSeven; }
            set
            {
                _ValueSeven = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("ValueSeven"));
            }
        }
        /// <summary>
        /// 状态值2
        /// </summary>
        public SignalModel ValueEight
        {
            get { return _ValueEight; }
            set
            {
                _ValueEight = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("ValueEight"));
            }
        }
        /// <summary>
        /// 状态值2
        /// </summary>
        public SignalModel ValueNine
        {
            get { return _ValueNine; }
            set
            {
                _ValueNine = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("ValueNine"));
            }
        }
    }
}
