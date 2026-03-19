using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGModel
{
    [BaseAttribute(Name = "BG_Modules", Description = "模块表")]
    public class ModulesConfig : BaseNotifyPropertyChanged
    {

        public string _modulesId = string.Empty;
        public string _modulesName = string.Empty;
        public string _modulesContentPluginsName = string.Empty;
        public string _modulesPluginsName = string.Empty;
        public string _modulesBelongs = string.Empty;
        public bool _IsCheck = false;
        public Visibsliy _IsModify = new Visibsliy();
        public string _Version = string.Empty;
        /// <summary>
        /// 配置文件的Key
        /// </summary>
        [BaseAttribute(Name = "modulesId", Description = "模块ID", IsUniqueKey = true)]
        public string modulesId { get { return _modulesId; } set { _modulesId = value; RaisePropertyChanged(new PropertyChangedEventArgs("modulesId")); } }

        /// <summary>
        /// 配置文件的Key
        /// </summary>
        [BaseAttribute(Name = "modulesName", Description = "模块功能名称")]
        public string modulesName { get { return _modulesName; } set { _modulesName = value; RaisePropertyChanged(new PropertyChangedEventArgs("modulesName")); } }
        /// <summary>
        /// 配置文件的Value
        /// </summary>
        [BaseAttribute(Name = "modulesContentPluginsName", Description = "模块插件名称")]
        public string modulesContentPluginsName { get { return _modulesContentPluginsName; } set { _modulesContentPluginsName = value; RaisePropertyChanged(new PropertyChangedEventArgs("modulesContentPluginsName")); } }
        /// <summary>
        /// 配置文件的Value
        /// </summary>
        [BaseAttribute(Name = "modulesPluginsName", Description = "插件dll名称")]
        public string modulesPluginsName { get { return _modulesPluginsName; } set { _modulesPluginsName = value; RaisePropertyChanged(new PropertyChangedEventArgs("modulesPluginsName")); } }
        /// <summary>
        /// 归属
        /// </summary>
        [BaseAttribute(Name = "modulesBelongs", Description = "是否已经属于当前版本")]
        public string modulesBelongs { get { return _modulesBelongs; } set { _modulesBelongs = value; RaisePropertyChanged(new PropertyChangedEventArgs("modulesPluginsName")); } }

        /// <summary>
        /// 归属
        /// </summary>
        [BaseAttribute(Name = "IsCheck", Description = "是否选中了",IsInsertDB =false)]
        public bool IsCheck { get { return _IsCheck; } set { _IsCheck = value; RaisePropertyChanged(new PropertyChangedEventArgs("IsCheck")); } }

        /// <summary>
        /// 归属
        /// </summary>
        [BaseAttribute(Name = "_IsModify", Description = "是否处于修改模式", IsInsertDB = false)]
        public Visibsliy IsModify { get { return _IsModify; } set { _IsModify = value; RaisePropertyChanged(new PropertyChangedEventArgs("IsModify")); } }

        /// <summary>
        /// 配置类型
        /// </summary>
        [BaseAttribute(Name = "Version", Description = "插件版本")]
        public string Version { get { return _Version; } set { _Version = value; RaisePropertyChanged(new PropertyChangedEventArgs("Version")); } }
    }
}
