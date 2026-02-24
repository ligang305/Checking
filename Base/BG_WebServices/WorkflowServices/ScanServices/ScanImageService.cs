using CMW.Common.Utilities;
using BGModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
using BG_Entities;
using BG_WorkFlow;
using System.Threading;

namespace BG_Services
{
    public class ScanImageService : BaseInstance<ScanImageService>
    {
        public Action InterruptScanAction;
        public Action ClearImageAction;
        public Action CancelScanWebAction;
        public Action<bool> ClickAction;
        public Action<string> MessaageShowAction;
        public Action CompleteAction;

        IScan ScanEquipment;
        CompositionContainer container = null;
        ControlVersionsBLL _ControlVersionsBLL = new ControlVersionsBLL();
        //你要导入的类型和元数据数组，所有继承IConditionView接口并导出的页面都在这个数组里  
        [ImportMany("Scan", typeof(BaseScan), Source = ImportSource.Local, RequiredCreationPolicy = CreationPolicy.NonShared)]
        //保存插件的内存对象
        Lazy<BaseScan, IMetaData>[] ScanPlugins { get; set; }

        ScanHelper BsScanHelper = null;

        public ScanImageService()
        {
            IsUseBsScan();
            LoadScanByPlugins();
            LoadBSScanByPlugins();
            InitScanEnv();
        }

        /// <summary>
        /// 是否启用背散式扫描
        /// </summary>
        private void IsUseBsScan()
        {
            ///H986 添加背散为插件的时候才启用单独的背散扫描
            
            if (ConfigServices.GetInstance().localConfigModel.IsUserBS && controlVersion != ControlVersion.BS) //IsUserBS读的ini配置文件
            {
                if (BsScanHelper == null)
                {
                    BsScanHelper = new ScanHelper();
                }
            }
        }

        public void Start()
        {
            BsScanHelper?.Start();
            ScanHelper.GetInstance().Start();
            ScrollImageServices.Service.Start();
            //Test();
        }

        public void Stop(bool isneedCancel = false)
        {
            if(isneedCancel)
            {
                BsScanHelper?.Stop();
                ScanHelper.GetInstance().Stop();
            }
            ScrollImageServices.Service.Stop();
        }
      
        private void Test()
        {
            Task.Run(() =>
            {
                    BSPLCControllerManager.GetInstance().WritePositionValue("DB54.13.0", true);
                    Thread.Sleep(5000);
                    BSPLCControllerManager.GetInstance().WritePositionValue("DB54.13.0", false);
                    BSPLCControllerManager.GetInstance().WritePositionValue("DB54.13.1", true);
                    Thread.Sleep(2000);
                    BSPLCControllerManager.GetInstance().WritePositionValue("DB54.13.1", false);
            });
            Task.Run(() => {
                    PLCControllerManager.GetInstance().WritePositionValue("M6.5", true);
                    Thread.Sleep(8000);
                    PLCControllerManager.GetInstance().WritePositionValue("M6.5", false);
                    PLCControllerManager.GetInstance().WritePositionValue("M7.7", true);
                    Thread.Sleep(3000);
                    PLCControllerManager.GetInstance().WritePositionValue("M7.7", false);
                    PLCControllerManager.GetInstance().WritePositionValue("M7.6", true);
                    Thread.Sleep(3000);
                    PLCControllerManager.GetInstance().WritePositionValue("M7.6", false);
            });
        }


        /// <summary>
        /// 加载扫描插件
        /// </summary>
        private void LoadScanByPlugins()
        {
            try
            {
                List<Bg_ControlVersion> bg_ControlVersions =  _ControlVersionsBLL.GetControlVersionsBLLDataModel();//读取配置文件【CONFIG\CONTROLVERSION_MODULECONFIG.xml】
                Bg_ControlVersion Module = bg_ControlVersions.FirstOrDefault(q => q.ControlversionKey == ConfigServices.GetInstance().localConfigModel.CMW_Version); //读取是否为 10
                var dir = new DirectoryInfo(SystemDirectoryConfig.AppDir); // CMW_OUTPUT目录
                if (dir.Exists)
                {
                    //就是这里，读取所有符合条件的dll
                    var catalog = new DirectoryCatalog(SystemDirectoryConfig.AppDir, "*.dll");
                    container = new CompositionContainer(catalog);
                    try
                    {
                        container.ComposeParts(this);
                    }
                    catch (Exception ce)
                    {
                        CommonDeleget.WriteLogAction(ce.StackTrace, LogType.NormalLog, false);
                    }
                    ScanPlugins.OrderBy(p => p.Metadata.Priority);
                }
                var ScanPlugin = ScanPlugins?.First(q => q.Metadata.Name == Module.Scan)?.Value;
                ScanEquipment = ScanPlugin; //选出来一个插件：BG_Services.BegoodBSScan
                ScanHelper.GetInstance().SetScan(ScanPlugin);

            }
            catch (Exception e)     
            {
                WriteLogAction(e.InnerException.StackTrace, LogType.ApplicationError);
            }
            finally
            {
                ScanHelper.GetInstance().SetScan(ScanEquipment);
            }
        }

        /// <summary>
        /// 加载背散扫描插件
        /// </summary>
        private void LoadBSScanByPlugins()
        {
            try
            {
                if(BsScanHelper!=null)
                {
                    var BSScanPlugin = ScanPlugins?.First(q => q.Metadata.Name == "BSScan")?.Value;
                    BsScanHelper?.SetScan(BSScanPlugin);
                }
            }
            catch (Exception e)
            {
                WriteLogAction(e.InnerException.StackTrace, LogType.ApplicationError);
            }
            finally
            {
               //BsScanHelper?.SetScan(BSScanPlugin);
            }
        }
      
        /// <summary>
        /// 初始化扫描环境
        /// </summary>
        private void InitScanEnv()
        {
            ScanEquipment.SetControlVersion(controlVersion);
            ScanEquipment.SetLogActionCallBack(delegate (string logmess, string logType, bool isInsert) { WriteLogAction(logmess, LogType.ApplicationError);  });
            ScanEquipment.SetClearScanImageCallBack(delegate () { ClearImageAction?.Invoke(); });
            ScanEquipment.SetMessageBoxActionCallBack(delegate (string message) {
                MessaageShowAction?.Invoke(message);
            });
            ScanEquipment.SetCancelCallBack(delegate () { CancelScanWebAction?.Invoke(); });
            ScanEquipment.SetSuspendCallBack(delegate (bool isResult) { ClickAction?.Invoke(isResult); });
            ScanEquipment.SetScanCompleteCallBack(delegate () { CompleteAction?.Invoke(); });
        }

        public void SetBoostingModel(Dictionary<string, string> BoostingModelDic)
        {
            ScanEquipment.SetBoostingModel(BoostingModelDic);
        }
    }
}
