using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BG_Entities;
using BG_Services;
using BG_WorkFlow;
using BGDAL;
using BGLogs;
using BGModel;
using BGUserControl;
using CMW.Common.Utilities;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace CMW
{
    public class SettingPageViewModel : PageViewModelBase
    {
        ModulesDal modulesDal = new ModulesDal();
        private CompositionContainer container = null;
        //你要导入的类型和元数据数组，所有继承IConditionView接口并导出的页面都在这个数组里  
        [ImportMany("ContentPage", typeof(BaseModules), Source = ImportSource.Local, RequiredCreationPolicy = CreationPolicy.NonShared)]
        //保存插件的内存对象
        public Lazy<BaseModules, IMetaData>[] ContentPlugins { get; set; }
        #region Command
        public RelayCommand LoadCommand { get; set; }
        #endregion
        public SettingPageViewModel()
        {
            CommonDeleget.WriteLogAction("USMS.UI.SMU.ViewModel.Pages.SystemMenu.SystemSecondMenuPageViewModel", LogType.NormalLog);

            CreateCommand();

            CommonDeleget.WriteLogAction("USMS.UI.SMU.ViewModel.Pages.SystemMenu.SystemSecondMenuPageViewModel", LogType.NormalLog);
        }

        private void CreateCommand()
        {
            LoadCommand = new RelayCommand(Loaded);
        }

        /// <summary>
        /// 页面载入
        /// </summary>
        private void Loaded()
        {
            try
            {
                SystemDirectoryConfig.AppDir = AppDomain.CurrentDomain.BaseDirectory;
                var dir = new DirectoryInfo(SystemDirectoryConfig.AppDir + GetModulesPath());
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
                        Log.GetDistance().WriteErrorLogs(ce.StackTrace);
                    }
                    ContentPlugins.OrderBy(p => p.Metadata.Priority);
                }
                StartUgrApplication.Services.StartUgr();
            }
            catch (Exception ex)
            {
                Log.GetDistance().WriteInfoLogs($"Module File  Error:{ex.StackTrace}");
            }
        }
        private string GetModulesPath()
        {
            return string.Empty;
        }


        public List<ModulesConfig> SettingModules()
        {
            return modulesDal.GetModulesConfig(ConfigServices.GetInstance().localConfigModel.CMW_Version);
        }

        /// <summary>
        /// 窗口关闭事件：在窗口关闭，清理最后一个页面占用的资源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosed()
        {

        }
        public override void OnKeyDown(KeyEventArgs args)
        {

        }

        public override void Cleanup()
        {
            OnClosed();
            MessengerInstance.Unregister(this);
            base.Cleanup();
        }
    }
}
