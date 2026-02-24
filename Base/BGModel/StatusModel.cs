using BGModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace BGModel
{

    /// <summary>
    /// 故障列表
    /// </summary>
    [BaseAttribute(Name = "BG_BOOSTING_CONFIGS", Description = "加速器实体状态")]
    public class BoostingStatusModel:BaseNotifyPropertyChanged
    {
        private string _BoostingStatus = string.Empty;
        private string _BoostingName = string.Empty;
        private string _BoostingDisplayName = string.Empty;
        private string _BoostingCode = string.Empty;
        private string _StatusCode = "0";

        /// <summary>
        /// 状态序号
        /// </summary>
        [BaseAttribute(Name = "BG_BOOSTING_INDEX", Description = "状态序号")]
        public string Bg_BoostingIndex
        {
            get { return _BoostingStatus; }
            set
            {
                _BoostingStatus = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("Bg_BoostingIndex"));
            }
        }

       
        public string Bg_StatusCode
        {
            get { return _StatusCode; }
            set
            {
                _StatusCode = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("Bg_StatusCode"));
            }
        }
        /// <summary>
        /// 状态序号
        /// </summary>
        [BaseAttribute(Name = "BG_BOOSTING_NAME", Description = "状态序号")]
        public string Bg_BoostingName
        {
            get { return _BoostingName; }
            set
            {
                _BoostingName = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("Bg_BoostingName"));
            }
        }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string BoostingDisplayName
        {
            get { return _BoostingDisplayName; }
            set
            {
                _BoostingDisplayName = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("BoostingDisplayName"));
            }
        }
        /// <summary>
        /// 加速器编码
        /// </summary>
        [BaseAttribute(Name = "BG_BOOSTING_CODE", Description = "加速器编码")]
        public string Bg_BoostingCode
        {
            get { return _BoostingCode; }
            set
            {
                _BoostingCode = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("Bg_BoostingCode"));
            }
        }
    }

    /// <summary>
    /// 故障列表
    /// </summary>

    [BaseAttribute(Name = "BG_STATUS", Description = "常规状态实体")]
    public class StatusModel : BaseNotifyPropertyChanged
    {

        private string _StatusName = string.Empty;
        private string _StatusDisplayName = string.Empty;
        private string _StatusCode = string.Empty;
        private string _StatusOwner = string.Empty;
        private string _StatusIndex = string.Empty;
        private string _BgCommandPosition = string.Empty;
        private string _Bg_Status_Position_Index = string.Empty;
        private string _BgOwn = string.Empty;
        private string _BgIsVisual = string.Empty;
        private string _BgIsClick = string.Empty;

        /// <summary>
        /// 是否跳转
        /// </summary>
        [BaseAttribute(Name = "BG_ISCLICK", Description = "是否允许跳转")]
        public string Bg_IsClick
        {
            get { return _BgIsClick; }
            set
            {
                _BgIsClick = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("Bg_IsClick"));
            }
        }

        /// <summary>
        /// 故障名称
        /// </summary>
        [BaseAttribute(Name = "BG_STATUS_OWN", Description = "归类")]
        public string Bg_Own
        {
            get { return _BgOwn; }
            set
            {
                _BgOwn = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("Bg_Own"));
            }
        }

        /// <summary>
        /// 什么值是正常的
        /// </summary>
        [BaseAttribute(Name = "BG_STATUS_ISVISUAL", Description = "默认值")]
        public string DefaultValue
        {
            get { return _BgIsVisual; }
            set
            {
                _BgIsVisual = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("DefaultValue"));
            }
        }

        /// <summary>
        /// 故障名称
        /// </summary>
        [BaseAttribute(Name = "BG_STATUS_POSITON_INDEX", Description = "查询命令需要发送的地址")]
        public string Bg_Status_Position_Index
        {
            get { return _Bg_Status_Position_Index; }
            set
            {
                _Bg_Status_Position_Index = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("Bg_Status_Position_Index"));
            }
        }

        /// <summary>
        /// 故障名称
        /// </summary>
        [BaseAttribute(Name = "BG_COMMAND_POSITION", Description = "查询命令需要发送的地址")]
        public string BgCommandPosition
        {
            get { return _BgCommandPosition; }
            set
            {
                _BgCommandPosition = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("BgCommandPosition"));
            }
        }

        /// <summary>
        /// 用来查询状态的序号
        /// </summary>
        [BaseAttribute(Name = "BG_STATUSINDEX", Description = "故障名称")]
        public string StatusIndex
        {
            get { return _StatusIndex; }
            set
            {
                _StatusIndex = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("StatusIndex"));
            }
        }
        /// <summary>
        /// 故障名称
        /// </summary>
        [BaseAttribute(Name = "BG_STATUSNAME", Description = "故障名称")]
        public string StatusName
        {
            get { return _StatusName; }
            set {
                _StatusName = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("StatusName"));
            }
        }

        /// <summary>
        /// 故障名称
        /// </summary>
        [BaseAttribute(Name = "BG_STATUSDSPLAYNAME", Description = "故障名称")]
        public string StatusDisplayName
        {
            get { return _StatusDisplayName; }
            set
            {
                _StatusDisplayName = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("StatusDisplayName"));
            }
        }
        /// <summary>
        /// 故障编码 对应PLC查询
        /// </summary>
        [BaseAttribute(Name = "BG_STATUSCODE", Description = "故障编码")]
        public string StatusCode
        {
            get { return _StatusCode; }
            set
            {
                _StatusCode = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("StatusCode"));
            }
        }
        /// <summary>
        /// 故障加速器来源 
        /// 达胜还是
        /// </summary>
        [BaseAttribute(Name = "BG_STATUSOWNER", Description = "故障来源")]
        public string StatusOwner
        {
            get { return _StatusOwner; }
            set
            {
                _StatusOwner = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("StatusOwner"));
            }
        }

        public bool Equals(StatusModel other)
         {  
        
             //Check whether the compared object is null.  
              if (Object.ReferenceEquals(other, null)) return false;  
        
              //Check whether the compared object references the same data.  
              if (Object.ReferenceEquals(this, other)) return true;  
        
              //Check whether the products' properties are equal.  
              return Bg_Own.Equals(other.Bg_Own) ;  
          }  
        
          // If Equals() returns true for a pair of objects   
          // then GetHashCode() must return the same value for these objects.  
        
          public override int GetHashCode()
          {  
        
              //Get hash code for the Name field if it is not null.  
              int _Bg_Own = Bg_Own == null ? 0 : Bg_Own.GetHashCode();  
       
        
              //Calculate the hash code for the product.  
              return _Bg_Own ;  
          }  


    }

    [BaseAttribute(Name = "BG_STATUS_TREE", Description = "树节点实体")]
    public class StatusTreeModel : BaseNotifyPropertyChanged
    {
        private string _ParentId = string.Empty;
        private string _Flow_Own = string.Empty;
        private string _StatusIndex = string.Empty;
        private string _TreeNodeName = string.Empty;
        private string _StatusCode = string.Empty;
        private string _StatusId = string.Empty;
        private string _isShow = false.ToString();
        private string _isDefalutBool = false.ToString();
        private string _bg_TYPE = string.Empty;


        /// <summary>
        /// 流程树类型
        /// </summary>
        [BaseAttribute(Name = "BG_TYPE", Description = "")]
        public string Type
        {
            get { return _bg_TYPE; }
            set { _bg_TYPE = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("BG_TYPE"));
            }
        }
        /// <summary>
        /// 判断默认值是true 正常还是false是正常
        /// </summary>
        [BaseAttribute(Name = "BG_VALUE_DEFLAUTVALUE", Description = "")]
        public string IsDefalutBool
        {
            get { return _isDefalutBool; }
            set
            {
                _isDefalutBool = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("IsDefalutBool"));
            }
        }

        /// <summary>
        /// 故障编码 对应PLC查询
        /// </summary>
        [BaseAttribute(Name = "BG_ISSHOW", Description = "")]
        public string IsShow
        {
            get { return _isShow; }
            set
            {
                _isShow = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("IsShow"));
            }
        }

        /// <summary>
        /// 故障编码 对应PLC查询
        /// </summary>
        [BaseAttribute(Name = "BG_STATUS_ID", Description = "")]
        public string StatusId
        {
            get { return _StatusId; }
            set
            {
                _StatusId = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("StatusId"));
            }
        }
        /// <summary>
        /// 故障编码 对应PLC查询
        /// </summary>
        [BaseAttribute(Name = "BG_STATUSCODE", Description = "")]
        public string StatusCode
        {
            get { return _StatusCode; }
            set
            {
                _StatusCode = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("StatusCode"));
            }
        }

        [BaseAttribute(Name = "BG_PARENT_ID", Description = "父节点ID")]
        public string Bg_Parent_Id
        {
            get { return _ParentId; }
            set { _ParentId = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("Bg_Parent_Id"));
            }
        }
        [BaseAttribute(Name = "BG_FLOW_OWN", Description = "所属流程名称")]
        public string Bg_Flow_Own
        {
            get { return _Flow_Own; }
            set
            {
                _Flow_Own = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("Bg_Flow_Own"));
            }
        }
        [BaseAttribute(Name = "BG_STATUSINDEX", Description = "状态查询序号")]
        public string Bg_StatusIndex
        {
            get { return _StatusIndex; }
            set
            {
                _StatusIndex = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("Bg_StatusIndex"));
            }
        }
        [BaseAttribute(Name = "BG_STATUSNAME", Description = "节点名称")]
        public string Bg_TreeName
        {
            get { return _TreeNodeName; }
            set
            {
                _TreeNodeName = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("Bg_TreeName"));
            }
        }
    }

}
