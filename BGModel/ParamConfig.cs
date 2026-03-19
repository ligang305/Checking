using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGModel
{
    [BaseAttribute(Name = "BG_PARAMCONFIG", Description = "参数表")]
    public class ParamConfig : BaseNotifyPropertyChanged
    {

        public string _key = string.Empty;
        public string _Value = string.Empty;
        public string _Type = string.Empty;
        public string _IsShow = "true";
        public string _IsAsync = "false";
        public string _Version = string.Empty;
        public Visibsliy _isModifing = new Visibsliy();

        /// <summary>
        /// 配置文件的Key
        /// </summary>
        [BaseAttribute(Name = "Key", Description = "键", IsUniqueKey = true)]
        public string Key { get { return _key; } set { _key = value; RaisePropertyChanged(new PropertyChangedEventArgs("Key")); } }
        /// <summary>
        /// 配置文件的Value
        /// </summary>
        [BaseAttribute(Name = "Value", Description = "值")]
        public string Value { get { return _Value; } set { _Value = value; RaisePropertyChanged(new PropertyChangedEventArgs("Value")); } }
        /// <summary>
        /// 配置类型
        /// </summary>
        [BaseAttribute(Name = "Type", Description = "类型")]
        public string Type { get { return _Type; } set { _Type = value; RaisePropertyChanged(new PropertyChangedEventArgs("Type")); } }
        /// <summary>
        /// 配置类型
        /// </summary>
        [BaseAttribute(Name = "Version", Description = "类型")]
        public string Version { get { return _Version; } set { _Version = value; RaisePropertyChanged(new PropertyChangedEventArgs("Version")); } }
        /// <summary>
        /// 配置类型
        /// </summary>
        [BaseAttribute(Name = "IsShow", Description = "是否前台展现")]
        public string IsShow { get { return _IsShow; } set { _IsShow = value; RaisePropertyChanged(new PropertyChangedEventArgs("IsShow")); } }
        /// <summary>
        /// 配置类型
        /// </summary>
        [BaseAttribute(Name = "IsAsync", Description = "是否已经同步")]
        public string IsAsync { get { return _IsAsync; } set { _IsAsync = value; RaisePropertyChanged(new PropertyChangedEventArgs("IsAsync")); } }
        /// <summary>
        /// 判断是否处于修改状态
        /// </summary>
        [BaseAttribute(Name = "ismodifing", Description = "操作状态",IsInsertDB = false)]
        public Visibsliy ismodifing { get { return _isModifing; } set { _isModifing = value; RaisePropertyChanged(new PropertyChangedEventArgs("ismodifing")); } }
    }

    /// <summary>
    /// 参数类
    /// </summary>
    public class Parameter
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
    }
}
