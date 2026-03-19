using BG_Entities;
using BG_Services;
using BG_WorkFlow;
using BGDAL;
using BGModel;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BGUserControl
{
    public class BSModulesConfigMvvm : BaseNotifyPropertyChanged
    {
        private CompositionContainer container = null;
        //你要导入的类型和元数据数组，所有继承IConditionView接口并导出的页面都在这个数组里  
        [ImportMany("BS_ContentPage", typeof(BaseModules), Source = ImportSource.Local, RequiredCreationPolicy = CreationPolicy.NonShared)]
        //保存插件的内存对象
        public Lazy<IConditionView, IMetaData>[] UserControlPlugins { get; set; }
        public ObservableCollection<ModulesConfig> ModulesConfigs = new ObservableCollection<ModulesConfig>();
        List<ModulesConfig> paramConfigs;
        ModulesDal modulesDal = new ModulesDal();
        public ICommand ModulePluginCheckCommand;
        public ICommand ParamConfigUpdateCommand;
        public ICommand ParamConfigCancelCommand;

        ControlVersion ControlVersion { get; set; }
        public BSModulesConfigMvvm(ControlVersion _ControlVersion)
        {
            ControlVersion = _ControlVersion;
            paramConfigs = modulesDal.GetModulesConfig(ControlVersion.ToString());
            ModulesConfigs = new ObservableCollection<ModulesConfig>();
            InitData();
        }
        private void InitData()
        {
            GetLocalPluginsModule();
            InitCommand();
        }

        public void GetLocalPluginsModule()
        {
            DateTime PlcStartTime = DateTime.Now;
            try
            {
                CommonDeleget.WriteLogAction($"Init Module File;Time {(DateTime.Now - PlcStartTime).TotalSeconds}", LogType.NormalLog);
                SystemDirectoryConfig.AppDir = AppDomain.CurrentDomain.BaseDirectory;
                var dir = new DirectoryInfo(SystemDirectoryConfig.AppDir);//SystemDirectoryConfig.GetInstance().GetPlugins()
                if (dir.Exists)
                {
                    //就是这里，读取所有符合条件的dll
                    var catalog = new DirectoryCatalog(dir.FullName, "BGUserControl.dll");
                    container = new CompositionContainer(catalog);
                    try
                    {
                        container.ComposeParts(this);
                    }
                    catch (Exception ce)
                    {
                        CommonDeleget.WriteLogAction(ce.StackTrace, LogType.NormalLog,false);
                    }
                    UserControlPlugins.OrderBy(p => p.Metadata.Priority);
                }
                if(UserControlPlugins!=null)
                {
                    foreach (var pluginsItem in UserControlPlugins)
                    {
                        ModulesConfig modulesConfig = new ModulesConfig()
                        {
                            modulesContentPluginsName = pluginsItem.Metadata.Description,
                            modulesPluginsName = pluginsItem.Metadata.Name,
                            Version = pluginsItem.Metadata.Version,
                        };
                        var ModulesItem = paramConfigs.FirstOrDefault(q => q.modulesPluginsName == modulesConfig.modulesPluginsName);
                        if (ModulesItem!=null)
                        {
                            modulesConfig.IsCheck = true;
                            modulesConfig.modulesId = ModulesItem.modulesId;
                        }
                        ModulesConfigs.Add(modulesConfig);
                    }
                }
                CommonDeleget.WriteLogAction($"Init Module File End;Time {(DateTime.Now - PlcStartTime).TotalSeconds}", LogType.NormalLog);
            }
            catch (Exception ex)
            {
                CommonDeleget.WriteLogAction($"Module File  Error:{ex.StackTrace}",LogType.ApplicationError,false);
            }
        }


        private void InitCommand()
        {
            ModulePluginCheckCommand = new ParamConfigCommand(CheckConfig);
            ParamConfigUpdateCommand = new ParamConfigCommand(UpdateConfig);
            ParamConfigCancelCommand = new ParamConfigCommand(InsertConfig);
        }
        public void CheckConfig(Object pc)
        {
            ModulesConfig modulesConfig = pc as ModulesConfig;
            modulesConfig.IsCheck = !modulesConfig.IsCheck;
            if(modulesConfig.IsCheck)
            {
                if (paramConfigs.Any(q => q.modulesContentPluginsName == modulesConfig.modulesContentPluginsName))
                {
                    UpdateConfig(modulesConfig);
                }
                else
                {
                    InsertConfig(modulesConfig);
                }
            }
            else
            {
                DeleteConfig(modulesConfig);
            }
         
            //}
        }
        public void InsertConfig(Object pc)
        {
            ModulesConfig paramConfig = pc as ModulesConfig;
            paramConfig.modulesBelongs = ControlVersion.ToString();
            paramConfig.modulesId = CommonFunc.GetGuid();
            modulesDal.InsertModules(paramConfig);
        }
        public void UpdateConfig(Object pc)
        {
            ModulesConfig paramConfig = pc as ModulesConfig;
            modulesDal.UpdateModulesConfig(paramConfig);
        }
        public void DeleteConfig(Object pc)
        {
            ModulesConfig paramConfig = pc as ModulesConfig;
            modulesDal.DeleteModulesConfig(paramConfig);
        }
    }
}
