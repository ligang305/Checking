
using BGModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BGCommonOperation
{
    [BaseAttribute(Name = "BG_COMMONSETTING", Description = "通用设置")]
    public class CommonSettingModel : BaseNotifyPropertyChanged
    {
        private string _CommonSettingDataSource;
        private string _CommonSettingName;
        private string _CommonSettingValue;
        private string _CommonSettingPLCCommandValue;
        private string _CommonSettingPLCValue;
        private string _CommonSettingType;

        [BaseAttribute(Name = "BG_COMMONSETTINGNAME", Description = "通用设置名称")]
        public string CommonSettingName
        {
            get { return _CommonSettingName; }
            set { _CommonSettingName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CommonSettingName"));
            }
        }
        [BaseAttribute(Name = "BG_COMMONSETTINGVALUE", Description = "通用设置当前值")]
        public string CommonSettingValue
        {
            get { return _CommonSettingValue; }
            set { _CommonSettingValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CommonSettingValue"));
            }
        }
        [BaseAttribute(Name = "BG_COMMONSETTINGPLCCOMMAND", Description = "发送给PLC的命令 例如AF、FF")]
        public string CommonSettingPLCValue
        {
            get { return _CommonSettingPLCValue; }
            set
            {
                _CommonSettingPLCValue = value; OnPropertyChanged(new PropertyChangedEventArgs("CommonSettingValue"));
            }
        }
        [BaseAttribute(Name = "BG_COMMONSETTINGPLCCOMMANDVALUE", Description = "发送给PLC的命令 例如00、FF")]
        public string CommonSettingPLCCommandValue
        {
            get { return _CommonSettingPLCCommandValue; }
            set
            {
                _CommonSettingPLCCommandValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CommonSettingPLCCommandValue"));
            }
        }
        [BaseAttribute(Name = "BG_COMMONSETTINGTYPE", Description = "通用设置类型 文本框还是 下拉框")]
        public string CommonSettingType
        {
            get { return _CommonSettingType; }
            set { _CommonSettingType = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CommonSettingType"));
            }
        }
        [BaseAttribute(Name = "BG_COMMONSETTINGDATASOURCE", Description = "节点名称")]
        public string CommonSettingDataSource
        {
            get { return _CommonSettingDataSource; }
            set
            {
                _CommonSettingDataSource = value;
                if (selectObject == null) selectObject = new List<SelectObject>();
                selectObject?.Clear();
                selectObject?.TrimExcess();
                if (!string.IsNullOrEmpty(_CommonSettingDataSource))
                {
                    List<string> DataList = _CommonSettingDataSource.Split(';').ToList();
                    foreach (var dataItem in DataList)
                    {
                        SelectObject _ObjectItem = new SelectObject();
                        List<string> _ObjectDataSouce = dataItem.Split(':').ToList();
                        _ObjectItem.SelectText = _ObjectDataSouce[1];
                        _ObjectItem.SelectValue = _ObjectDataSouce[0];
                        selectObject.Add(_ObjectItem);
                    }
                }
            }
        }

        public List<SelectObject> selectObject { get; set; }
    }

    [BaseAttribute(Name = "BG_CARCANTILEVER", Description = "车体与悬臂控制与检测")]
    public class CarCantileverModel : BaseNotifyPropertyChanged
    {
        private string _CarPropName;
        private string _CarPropStatus;
        private string _CarDisplayName;
        [BaseAttribute(Name = "BG_CARPROPNAME", Description = "车体与悬臂控制与检测属性名")]
        public string CarPropName
        {
            get { return _CarPropName; }
            set
            {
                _CarPropName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CarPropName"));
            }
        }
        [BaseAttribute(Name = "BG_CARPROPSTATUS", Description = "节点名称")]
        public string CarPropStatus
        {
            get { return _CarPropStatus; }
            set
            {
                _CarPropStatus = value;
                if (_CarPropStatus == "0")
                    CarDisPlayName = "向后";
                else CarDisPlayName = "向前";

                OnPropertyChanged(new PropertyChangedEventArgs("CarPropStatus"));
            }
        }
        [BaseAttribute(Name = "BG_DISPLAYNAME", Description = "状态显示值")]
        public string CarDisPlayName
        {
            get { return _CarDisplayName; }
            set
            {
                _CarDisplayName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CarDisPlayName"));
            }
        }
    }

    public class SelectObject
    {
        public string SelectText { get; set; }

        public string SelectValue { get; set; }
    }
}
