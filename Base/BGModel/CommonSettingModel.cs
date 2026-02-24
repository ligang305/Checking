
using BGModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using CMW.Common.Utilities;
using static CMW.Common.Utilities.CommonDeleget;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace BGModel
{
    [BaseAttribute(Name = "BG_COMMONSETTING", Description = "通用设置")]
    public class CommonSettingModel : BaseNotifyPropertyChanged, IDataErrorInfo
    {
        private string _CommonSettingDataSource;
        private string _Warmup_TimeSource;
        private string _CommonSettingName;
        private string _CommonSettingDisplayName;
        private string _CommonSettingValue;
        private string _CommonSettingValueType;
        private string _CommonSettingPLCValue;
        private string _CommonSettingType;
        private string _CommonSettingSendType;
        private string _CommonSettingIndex;
        private string _CommonSettingVaildStr;

        [BaseAttribute(Name = "BG_COMMONSETTINGINDEX", Description = "通用设置的索引或位置用于查询状态")]
        public string CommonSettingIndex
        {
            get { return _CommonSettingIndex; }
            set
            {
                _CommonSettingIndex = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("CommonSettingIndex"));
            }
        }
        [BaseAttribute(Name = "BG_COMMONSETTING_DISPLAYNAME", Description = "通用设置显示名称")]
        public string CommonSettingDisplayName
        {
            get { return _CommonSettingDisplayName; }
            set
            {
                _CommonSettingDisplayName = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("CommonSettingDisplayName"));
            }
        }
        [BaseAttribute(Name = "BG_COMMONSETTINGNAME", Description = "通用设置名称")]
        public string CommonSettingName
        {
            get { return _CommonSettingName; }
            set { _CommonSettingName = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("CommonSettingName"));
            }
        }
        [BaseAttribute(Name = "BG_COMMONSETTINGVALUE", Description = "通用设置当前值")]
        public string CommonSettingValue
        {
            get { return _CommonSettingValue; }
            set { _CommonSettingValue = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("CommonSettingValue"));
            }
        }
        [BaseAttribute(Name = "BG_COMMONSETTINGPLCVALUE", Description = "发送给PLC的命令 例如AF、FF")]
        public string CommonSettingPLCValue
        {
            get { return _CommonSettingPLCValue; }
            set
            {
                _CommonSettingPLCValue = value; RaisePropertyChanged(new PropertyChangedEventArgs("CommonSettingValue"));
            }
        }
        [BaseAttribute(Name = "BG_COMMONSETTINGSENDTYPE", Description = "发送类型例如文本还是StartBytePositionDefaultText")]
        public string CommonSettingSendType
        {
            get { return _CommonSettingSendType; }
            set
            {
                _CommonSettingSendType = value; RaisePropertyChanged(new PropertyChangedEventArgs("CommonSettingSendType"));
            }
        }
        [BaseAttribute(Name = "BG_COMMONSETTINGVALUETYPE", Description = "判断值是bool还是int还是UShort")]
        public string CommonSettingValueType
        {
            get { return _CommonSettingValueType; }
            set
            {
                _CommonSettingValueType = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("CommonSettingValueType"));
            }
        }
        [BaseAttribute(Name = "BG_COMMONSETTINGTYPE", Description = "通用设置类型 文本框还是 下拉框")]
        public string CommonSettingType
        {
            get { return _CommonSettingType; }
            set { _CommonSettingType = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("CommonSettingType"));
            }
        }

        [BaseAttribute(Name = "BG_COMMONSETTINGVALID", Description = "对内容进行校验")]
        public string CommonSettingVaildStr
        {
            get { return _CommonSettingVaildStr; }
            set
            {
                _CommonSettingVaildStr = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("CommonSettingVaildStr"));
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
        [BaseAttribute(Name = "BG_COMMONSETTINGWARMTIME", Description = "节点名称")]
        public string Warmup_TimeSource
        {
            get { return _Warmup_TimeSource; }
            set
            {
                _Warmup_TimeSource = value;
            }
        }
        
        public List<SelectObject> selectObject { get; set; }
        public string this[string columnName]
        {
            get
            {
                string result = string.Empty;

                if (CommonSettingType == ControlType.TextBox)
                {
                    //如果需要校验的属性是 值
                    if (columnName == "CommonSettingValue")
                    {
                        //判断需要校验的值是否需要校验
                        //如果校验的值是空，那么就掠过，说明不需要校验
                        if (!string.IsNullOrEmpty(CommonSettingVaildStr))
                        {
                            if (string.IsNullOrWhiteSpace(CommonSettingValue))
                            {
                                result = UpdateStatusNameAction("ValueNotNull");
                            }
                            else
                            {
                                if (VaildDataList == null)
                                {
                                    VaildDataList = ValidationFactory.GetInstance().CreateVaildData(CommonSettingVaildStr);
                                }

                                foreach (var valiData in VaildDataList)
                                {
                                    result = valiData.Vaild(CommonSettingValue);
                                    if (!string.IsNullOrEmpty(result)) break;
                                }
                            }
                        }
                    }
                }

                _error = result;
                return result;
            }

        }
        public string Error
        {
            get { return _error; }
        }

        public string _error;

        public List<VaildData> VaildDataList { get; set; } = null;
    }

    [BaseAttribute(Name = "BG_CARSPEED", Description = "快检车速配置实体")]
    public class Bg_Carspeed : BaseNotifyPropertyChanged,IDataErrorInfo
    {
        private string _No;
        private string _SpeedMin;
        private string _SpeedMax;
        private string _Freez;
        private string _BayNumber;

        [BaseAttribute(Name = "NO", Description = "序号", IsUniqueKey=true)]
        public string No
        {
            get { return _No; }
            set
            {
                _No = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("No"));
            }
        }
        [BaseAttribute(Name = "SPEEDMIN", Description = "速度范围下值")]
        public string SpeedMin
        {
            get { return _SpeedMin; }
            set
            {
                _SpeedMin = value;
                var temp = SpeedMax;
                RaisePropertyChanged(new PropertyChangedEventArgs("SpeedMin"));
            }
        }
        [BaseAttribute(Name = "SPEEDMAX", Description = "速度范围上值")]
        public string SpeedMax
        {
            get { return _SpeedMax; }
            set
            {
                _SpeedMax = value;
                var temp = SpeedMin;
                RaisePropertyChanged(new PropertyChangedEventArgs("SpeedMax"));
            }
        }
        [BaseAttribute(Name = "FREEZ", Description = "频率值")]
        public string Freez
        {
            get { return _Freez; }
            set
            {
                _Freez = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("Freez"));
            }
        }
        [BaseAttribute(Name = "BAYNUMBER", Description = "对应的探测器排数")]
        public string BayNumber
        {
            get { return _BayNumber; }
            set
            {
                _BayNumber = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("BayNumber"));
            }
        }
        public string Error
        {
            get { return _error; }
        }

        public string _error;

        public string this[string columnName]
        {
            get
            {
                _error = string.Empty;
                DemicalValueValidation numValue = new DemicalValueValidation();

                //判断需要校验的值是否需要校验
                //如果校验的值是空，那么就掠过，说明不需要校验
                //if (string.IsNullOrEmpty(SpeedMin) || string.IsNullOrEmpty(SpeedMax)
                //    || string.IsNullOrEmpty(Freez))
                //{
                //    _error = UpdateStatusNameAction("ValueNotNull");
                //}
                //else
                //数值校验
                if (string.IsNullOrEmpty(SpeedMin) || string.IsNullOrEmpty(SpeedMax)
                    || string.IsNullOrEmpty(Freez))
                    {
                        _error = UpdateStatusNameAction("ValueNotNull");
                    }
                    else if (columnName == "SpeedMax" )
                    {
                        _error = numValue.Vaild(SpeedMin);
                        if (!string.IsNullOrEmpty(_error)) return _error;
                        _error = numValue.Vaild(SpeedMax);
                        if (!string.IsNullOrEmpty(_error)) return _error;
                        if (string.IsNullOrEmpty(SpeedMax)) return UpdateStatusNameAction("ValueNotNull");
                        if (Convert.ToDecimal(SpeedMin) > Convert.ToDecimal(SpeedMax))
                        {
                            _error = UpdateStatusNameAction("MinCannotLargeThanMax");
                            if (!string.IsNullOrEmpty(_error))
                                return _error;
                        }
                    }
                    else if(columnName == "SpeedMin")
                    {
                        _error = numValue.Vaild(SpeedMin);
                        if (!string.IsNullOrEmpty(_error))return _error;
                        _error = numValue.Vaild(SpeedMax);
                        if (!string.IsNullOrEmpty(_error)) return _error;
                        if (string.IsNullOrEmpty(SpeedMax)) return UpdateStatusNameAction("ValueNotNull");
                        if (Convert.ToDecimal(SpeedMin) > Convert.ToDecimal(SpeedMax))
                        {
                            _error = UpdateStatusNameAction("MinCannotLargeThanMax");
                            if (!string.IsNullOrEmpty(_error))
                                return _error;
                        }
                    }
                    else if(columnName == "Freez")
                    {
                        _error = numValue.Vaild(Freez);
                        if (!string.IsNullOrEmpty(_error)) return _error;
                        if (string.IsNullOrEmpty(SpeedMax)) return UpdateStatusNameAction("ValueNotNull");
                    }
                //}
                return _error;
            }
        }
    }

    [BaseAttribute(Name = "BG_DOSE", Description = "剂量探头阈值IP设置")]
    public class Bg_Dose : BaseNotifyPropertyChanged, IDataErrorInfo
    {
        private string _ID;
        private string _IPAddress;
        private string _Port;
        private string _Index;

        [BaseAttribute(Name = "ID", Description = "序号", IsUniqueKey = true)]
        public string ID
        {
            get { return _ID; }
            set
            {
                _ID = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("ID"));
            }
        }
        [BaseAttribute(Name = "IPAddress", Description = "IP地址")]
        public string IPAddress
        {
            get { return _IPAddress; }
            set
            {
                _IPAddress = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("IPAddress"));
            }
        }
        [BaseAttribute(Name = "Port", Description = "速度范围上值")]
        public string Port
        {
            get { return _Port; }
            set
            {
                _Port = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("Port"));
            }
        }
        [BaseAttribute(Name = "IndexNum", Description = "剂量序号")]
        public string IndexNum
        {
            get { return _Index; }
            set
            {
                _Index = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("Index"));
            }
        }
        public string Error
        {
            get { return _error; }
        }

        public string _error;

        public string this[string columnName]
        {
            get
            {
                _error = string.Empty;
                IPAddressValidation IPAddressValue = new IPAddressValidation();
                NumValueValidation numValidation = new NumValueValidation();
                //else
                //数值校验
                if (string.IsNullOrEmpty(IPAddress) || string.IsNullOrEmpty(Port))
                {
                    _error = UpdateStatusNameAction("ValueNotNull");
                }
                else if (columnName == "IPAddress")
                {
                    _error = IPAddressValue.Vaild(IPAddress);
                    if (!string.IsNullOrEmpty(_error)) return _error;
                  
                }
                else if (columnName == "Port")
                {
                    _error = numValidation.Vaild(Port);
                    if (!string.IsNullOrEmpty(_error)) return _error;
                }
                return _error;
            }
        }
    }

    [BaseAttribute(Name = "BG_BS2000", Description = "背散设备IP地址设置")]
    public class BG_BS2000 : BaseNotifyPropertyChanged, IDataErrorInfo
    {
        private string _ID;
        private string _IPAddress;
        private string _CommandPort;
        private string _DataPort;
        private string _Remark;
        private string _ViewCount = "1";

        [BaseAttribute(Name = "ID", Description = "序号", IsUniqueKey = true)]
        public string ID
        {
            get { return _ID; }
            set
            {
                _ID = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("ID"));
            }
        }
        [BaseAttribute(Name = "IPAddress", Description = "IP地址")]
        public string IPAddress
        {
            get { return _IPAddress; }
            set
            {
                _IPAddress = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("IPAddress"));
            }
        }
        [BaseAttribute(Name = "CommandPort", Description = "指令端口")]
        public string CommandPort
        {
            get { return _CommandPort; }
            set
            {
                _CommandPort = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("CommandPort"));
            }
        }
        [BaseAttribute(Name = "DataPort", Description = "数据端口")]
        public string DataPort
        {
            get { return _DataPort; }
            set
            {
                _DataPort = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("DataPort"));
            }
        }

        [BaseAttribute(Name = "Remark", Description = "备注")]
        public string Remark
        {
            get { return _Remark; }
            set
            {
                _Remark = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("Remark"));
            }
        }

        [BaseAttribute(Name = "ViewCount", Description = "视角")]
        public string ViewCount
        {
            get { return _ViewCount; }
            set
            {
                _ViewCount = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("ViewCount"));
            }
        }
        public string Error
        {
            get { return _error; }
        }

        public string _error;

        public string this[string columnName]
        {
            get
            {
                _error = string.Empty;
                IPAddressValidation IPAddressValue = new IPAddressValidation();
                NumValueValidation numValidation = new NumValueValidation();
                //else
                //数值校验
                if (string.IsNullOrEmpty(IPAddress) || string.IsNullOrEmpty(DataPort) || string.IsNullOrEmpty(CommandPort))
                {
                    _error = UpdateStatusNameAction("ValueNotNull");
                }
                else if (columnName == "IPAddress")
                {
                    _error = IPAddressValue.Vaild(IPAddress);
                    if (!string.IsNullOrEmpty(_error)) return _error;

                }
                else if (columnName == "DataPort" || columnName== "CommandPort")
                {
                    _error = numValidation.Vaild(DataPort);
                    if (!string.IsNullOrEmpty(_error)) return _error;
                }
                return _error;
            }
        }
    }


    [BaseAttribute(Name = "BG_BGV6000BS", Description = "背散设备IP地址设置")]
    public class BG_BGV6000BS : BaseNotifyPropertyChanged, IDataErrorInfo
    {
        private string _ID;
        private string _IPAddress; 
        private string _CommandPort;
        private string _DataPort;
        private string _Remark;
        private string _ViewCount = "1";

        [BaseAttribute(Name = "ID", Description = "序号", IsUniqueKey = true)]
        public string ID
        {
            get { return _ID; }
            set
            {
                _ID = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("ID"));
            }
        }
        [BaseAttribute(Name = "IPAddress", Description = "IP地址")]
        public string IPAddress
        {
            get { return _IPAddress; }
            set
            {
                _IPAddress = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("IPAddress"));
            }
        }
        [BaseAttribute(Name = "CommandPort", Description = "指令端口")]
        public string CommandPort
        {
            get { return _CommandPort; }
            set
            {
                _CommandPort = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("CommandPort"));
            }
        }
        [BaseAttribute(Name = "DataPort", Description = "数据端口")]
        public string DataPort
        {
            get { return _DataPort; }
            set
            {
                _DataPort = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("DataPort"));
            }
        }

        [BaseAttribute(Name = "Remark", Description = "备注")]
        public string Remark
        {
            get { return _Remark; }
            set
            {
                _Remark = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("Remark"));
            }
        }

        [BaseAttribute(Name = "ViewCount", Description = "视角")]
        public string ViewCount
        {
            get { return _ViewCount; }
            set
            {
                _ViewCount = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("ViewCount"));
            }
        }
        public string Error
        {
            get { return _error; }
        }

        public string _error;

        public string this[string columnName]
        {
            get
            {
                _error = string.Empty;
                IPAddressValidation IPAddressValue = new IPAddressValidation();
                NumValueValidation numValidation = new NumValueValidation();
                //else
                //数值校验
                if (string.IsNullOrEmpty(IPAddress) || string.IsNullOrEmpty(DataPort) || string.IsNullOrEmpty(CommandPort))
                {
                    _error = UpdateStatusNameAction("ValueNotNull");
                }
                else if (columnName == "IPAddress")
                {
                    _error = IPAddressValue.Vaild(IPAddress);
                    if (!string.IsNullOrEmpty(_error)) return _error;

                }
                else if (columnName == "DataPort" || columnName == "CommandPort")
                {
                    _error = numValidation.Vaild(DataPort);
                    if (!string.IsNullOrEmpty(_error)) return _error;
                }
                return _error;
            }
        }
    }

    public class ControlType
    {
        public static string Button { get; set; } = "Button";
        public static string ToggleButton { get; set; } = "ToggleButton";
        public static string TextBox { get; set; } = "TextBox";
        public static string DropDownList { get; set; } = "Button";
        public static string ReadPlc { get; set; } = "ReadPlc";
    }

    public class SelectObject : BaseNotifyPropertyChanged
    {
        private string _SelectText;
        public string SelectText 
        { get { return _SelectText; } set { _SelectText = value; SelectDisplayText = UpdateStatusNameAction(_SelectText); } }
        private string _SelectValue;
        public string SelectValue
        {
            get { return _SelectValue; }
            set
            {
                _SelectValue = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("SelectValue"));
            }
        }
        private string _SelectDisplayText;
        public string SelectDisplayText
        {
            get { return _SelectDisplayText; }
            set
            {
                _SelectDisplayText = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("SelectDisplayText"));
            }
        }
    }

    [BaseAttribute(Name = "BG_CARCANTILEVER", Description = "车体与悬臂控制与检测")]
    public class CarCantileverModel : BaseNotifyPropertyChanged
    {
        
        private string _CarStatusIndex;
        private string _CarPropName;
        private string _CarPropStatus;
        private string _CarDisplayName;
        private string _CarCommandPosition;



        [BaseAttribute(Name = "BG_CAR_COMMAND_POSITION", Description = "状态与命令对应的序号")]
        public string CarCommandPosition
        {
            get { return _CarCommandPosition; }
            set
            {
                _CarCommandPosition = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("CarCommandPosition"));
            }
        }

        [BaseAttribute(Name = "BG_STATUSINDEX", Description = "车体与悬臂控制与检测属性名")]
        public string CarPropSatusIndex
        {
            get { return _CarStatusIndex; }
            set
            {
                _CarStatusIndex = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("CarPropSatusIndex"));
            }
        }

        [BaseAttribute(Name = "BG_CARPROPNAME", Description = "车体与悬臂控制与检测属性名")]
        public string CarPropName
        {
            get { return _CarPropName; }
            set
            {
                _CarPropName = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("CarPropName"));
            }
        }
        [BaseAttribute(Name = "BG_CARPROPSTATUS", Description = "节点名称")]
        public string CarPropStatus
        {
            get { return _CarPropStatus; }
            set
            {
                _CarPropStatus = value;
                if (CarPropName == "扫描方向")
                {
                    if (_CarPropStatus.ToLower() == "false")
                        CarDisPlayName = "向后";
                    else CarDisPlayName = "向前";
                }
                RaisePropertyChanged(new PropertyChangedEventArgs("CarPropStatus"));
            }
        }
        [BaseAttribute(Name = "BG_DISPLAYNAME", Description = "状态显示值")]
        public string CarDisPlayName
        {
            get { return _CarDisplayName; }
            set
            {
                _CarDisplayName = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("CarDisPlayName"));
            }
        }
    }

    [BaseAttribute(Name = "BG_SELFWORKAUTOBOOSTINGCONFIG", Description = "自行走加速器设置实体")]
    public class BoostingModel : BaseNotifyPropertyChanged
    {
        private string _Name;
        private string _DisPlayName;
        private string _Value;
        private string _ActureValue;
        private string _IsUseDefaultValue;


        [BaseAttribute(Name = "NAME", Description = "名称")]
        public string Name
        {
            get { return _Name;  }
            set {
                _Name = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("Name"));
            }
        }
        [BaseAttribute(Name = "DISPLAYNAME", Description = "显示")]
        public string DisPlayName
        {
            get { return _DisPlayName; }
            set { _DisPlayName = value; RaisePropertyChanged(new PropertyChangedEventArgs("DisPlayName")); }
        }
        [BaseAttribute(Name = "VALUE", Description = "显示值")]
        public string Value
        {
            get { return _Value;  }
            set { _Value = value; RaisePropertyChanged(new PropertyChangedEventArgs("Value")); }
        }
        [BaseAttribute(Name = "ActureValue", Description = "实际值")]
        public string ActureValue
        {
            get { return _ActureValue; }
            set { _ActureValue = value; RaisePropertyChanged(new PropertyChangedEventArgs("ActureValue")); }
        }
        [BaseAttribute(Name = "IsUseDefalut", Description = "实际值")]
        public string IsUseDefalut
        {
            get { return _IsUseDefaultValue; }
            set { _IsUseDefaultValue = value; RaisePropertyChanged(new PropertyChangedEventArgs("IsUseDefalut")); }
        }

    }
    
}
