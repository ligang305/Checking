using BG_Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace BGModel
{
    public class HardwareState:BaseNotifyPropertyChanged
    {
        public double _LblTextMaxWidth;
        public double LblTextMaxWidth { get { return _LblTextMaxWidth; } set { _LblTextMaxWidth = value; RaisePropertyChanged(new PropertyChangedEventArgs("LblTextMaxWidth")); } }

        public string _LabmForecolor;
        public string LabmForecolor { get { return _LabmForecolor; } set { _LabmForecolor = value; RaisePropertyChanged(new PropertyChangedEventArgs("LabmForecolor")); } }

        public string _LabelText;
        public string LabelText { get { return _LabelText; } set { _LabelText = value; RaisePropertyChanged(new PropertyChangedEventArgs("LabelText")); } }

        public string _ImageLogo;
        public string ImageLogo { get { return _ImageLogo; } set { _ImageLogo = value; RaisePropertyChanged(new PropertyChangedEventArgs("ImageLogo")); } }
        public string _ModulesName;
        public string ModulesName { get { return _ModulesName; } set { _ModulesName = value; RaisePropertyChanged(new PropertyChangedEventArgs("ModulesName")); } }

        public Visibility _IsShow = Visibility.Collapsed;
        public Visibility IsShow { get { return _IsShow; } set { _IsShow = value; RaisePropertyChanged(new PropertyChangedEventArgs("IsShow")); } }
    }

    public class CommonSettingModelHardwareState : BaseNotifyPropertyChanged
    {
        public double _LabelText;
        public double LabelText { get { return _LabelText; } set { _LabelText = value; RaisePropertyChanged(new PropertyChangedEventArgs("LabelText")); } }

        public string _LabmForecolor;
        public string LabmForecolor { get { return _LabmForecolor; } set { _LabmForecolor = value; RaisePropertyChanged(new PropertyChangedEventArgs("LabmForecolor")); } }

        public string _BackForeColor;
        public string BackForeColor { get { return _BackForeColor; } set { _BackForeColor = value; RaisePropertyChanged(new PropertyChangedEventArgs("BackForeColor")); } }

        public bool _StatusValue;
        public bool StatusValue { get { return _StatusValue; } set { _StatusValue = value; RaisePropertyChanged(new PropertyChangedEventArgs("StatusValue")); } }

        public DateTime _UpdateSource;
        public DateTime UpdateSource { get { return _UpdateSource; } set { _UpdateSource = value; RaisePropertyChanged(new PropertyChangedEventArgs("UpdateSource")); } }

        public PLCPositionEnum _PositionIndex;
        public PLCPositionEnum PositionIndex { get { return _PositionIndex; } set { _PositionIndex = value; RaisePropertyChanged(new PropertyChangedEventArgs("PositionIndex")); } }

        public int _StandStatusIndex;
        public int StandStatusIndex { get { return _StandStatusIndex; } set { _StandStatusIndex = value; RaisePropertyChanged(new PropertyChangedEventArgs("StandStatusIndex")); } }

        public int _StandStatusValue;
        public int StandStatusValue { get { return _StandStatusValue; } set { _StandStatusValue = value; RaisePropertyChanged(new PropertyChangedEventArgs("StaStandStatusValuendStatusIndex")); } }
    }

    public class CommonSettingModelWarmUpHardwareState : BaseNotifyPropertyChanged
    {
        public string _StandStatusIndex;
        public string StandStatusIndex { get { return _StandStatusIndex; } set { _StandStatusIndex = value; RaisePropertyChanged(new PropertyChangedEventArgs("StandStatusIndex")); } }

        public string _StandStatusValue;
        public string StandStatusValue { get { return _StandStatusValue; } set { _StandStatusValue = value; RaisePropertyChanged(new PropertyChangedEventArgs("StandStatusValue")); } }

        public int _WarmupTotalTime;
        public int WarmupTotalTime { get { return _WarmupTotalTime; } set { _WarmupTotalTime = value; RaisePropertyChanged(new PropertyChangedEventArgs("WarmupTotalTime")); } }

        public int _WarmupCurrentTime;
        public int WarmupCurrentTime { get { return _WarmupCurrentTime; } set { _WarmupCurrentTime = value; RaisePropertyChanged(new PropertyChangedEventArgs("WarmupCurrentTime")); } }

        public string _BtnForText;
        public string BtnForText { get { return _BtnForText; } set { _BtnForText = value; RaisePropertyChanged(new PropertyChangedEventArgs("BtnForText")); } }

        public int _FalutCode;
        public int FalutCode { get { return _FalutCode; } set { _FalutCode = value; RaisePropertyChanged(new PropertyChangedEventArgs("FalutCode")); } }
    }
}
