using BG_Entities;
using BG_Services;
using BG_WorkFlow;
using BGCommunication;
using BGModel;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CMW.Common.Utilities.CommonDeleget;
namespace BG_Services
{
    public class BoostingControllerManager : BaseInstance<BoostingControllerManager>, IEquipment
    {
        public IEquipment BoostingController;
        CompositionContainer container = null;

        [ImportMany("Boosting", typeof(IEquipment), AllowRecomposition = true)]
        //保存插件的内存对象
        public Lazy<IEquipment, IMetaData>[] Plugins { get; set; }

        public List<Bg_ControlVersion> ControlVersionList = new List<Bg_ControlVersion>();
        ControlVersionsBLL _ControlVersionsBLL = new ControlVersionsBLL();
        //需要传出来的一个枚举值
        public static DoseRate Dose = new DoseRate();
        //电流和温度
        public static TandI TandI = new TandI();

        public BoostingControllerManager()
        {
            WaitWorkStopEvent += SendCommandWaitWorkStop;
            RadiationOnEvent += SendCommandRadiationOn;
        }
        
        public async void Load()
        {
            LoadBoostingModules();
            await Task.Run(() => {
                if (BoostingController != null)
                BoostingController.Load();
            });
        }
        /// <summary>
        /// 载入加速器协议
        /// </summary>
        /// <param name="commonProtocol"></param>
        public void Load(ICommonProtocol commonProtocol)
        {
            LoadBoostingModules();
            if (BoostingController != null)
            {
                if (commonProtocol != null)
                {
                    BoostingController.Load(commonProtocol);
                }
                else
                {
                    BoostingController.Load();
                }
            }
        }
        /// <summary>
        /// 通过不同类型的
        /// </summary>
        /// <param name="cv"></param>
        /// <param name="commonProtocol"></param>
        public async void Load(ControlVersion cv)
        {
            await Task.Run(() => {
                LoadBoostingPluginsConfig();
                LoadPlugins();
                Load();
            });
        }
        /// <summary>
        /// 获取加速器插件配置
        /// </summary>
        private void LoadBoostingPluginsConfig()
        {
            ControlVersionList = _ControlVersionsBLL.GetControlVersionsBLLDataModel();
        }
        /// <summary>
        /// 载入插件
        /// </summary>
        private void LoadPlugins()
        {
            try
            {
                SystemDirectoryConfig.AppDir = AppDomain.CurrentDomain.BaseDirectory;
                //获取工作目录
                var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
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
                        CommonDeleget.WriteLogAction(ce.Message, LogType.ApplicationError,true);
                    }
                    Plugins.OrderBy(p => p.Metadata.Priority);
                }
            }
            catch (Exception ex)
            {
                CommonDeleget.WriteLogAction($"Loading Boosting Modules Exception{ex.StackTrace}", LogType.ApplicationError, true);
            }
        }
        /// <summary>
        /// 加载适配的加速器组件
        /// </summary>
        private void LoadBoostingModules()
        {
            try
            {
                string ModuleNames = ControlVersionList.FirstOrDefault(q => q.ControlversionKey ==ConfigServices.GetInstance().localConfigModel.CMW_Version)?.BoostingEquipment;
                if (!string.IsNullOrEmpty(ModuleNames))
                {
                    BoostingController = Plugins.First((q => q.Metadata.Name == ModuleNames))?.Value;
                    if (BoostingController == null) { CommonDeleget.MessageBoxActionAction($"Boostring Modules Lost,lost Modules Name {ModuleNames}"); return; }
                }
            }
            catch
            {
                throw;
            }
           
        }
        #region Function region
        public void DisConnection()
        {
            BoostingController?.DisConnection();
        }

        public void Inquire()
        {
            BoostingController?.Inquire();
        }

        public bool IsConnection()
        {
            if (BoostingController == null) return false;
            return BoostingController.IsConnection();
        }

        public bool IsRayOut()
        {
            if (BoostingController == null) return false;
            return BoostingController.IsRayOut();
        }
        /// <summary>
        /// 出束
        /// </summary>
        public void Ray()
        {
            if (BoostingController == null) return ;
            BoostingController.Ray();
        } 
        /// <summary>
        /// 重置
        /// </summary>
        /// <returns></returns>
        public bool Reset()
        {
            if (BoostingController == null) return false;
            return BoostingController.Reset();
        }
        /// <summary>
        /// 设置电流
        /// </summary>
        /// <param name="CurrentFlows"></param>
        /// <returns></returns>
        public bool SetCurrentFlows(double CurrentFlows) 
        {
            if (BoostingController == null) return false;
            return BoostingController.SetCurrentFlows(CurrentFlows);
        }
        /// <summary>
        /// 设置高能脉冲
        /// </summary>
        /// <param name="HLC"></param>
        /// <returns></returns>
        public bool SetHMC(double HLC)
        {
            if (BoostingController == null) return false;
            return BoostingController.SetHMC(HLC);
        }
        /// <summary>
        /// 设置高能脉冲个数
        /// </summary>
        /// <param name="LMCNum"></param>
        /// <returns></returns>
        public bool SetHMCNum(double LMCNum)
        {
            if (BoostingController == null) return false;
            return BoostingController.SetHMCNum(LMCNum);
        }
        /// <summary>
        /// 设置低能脉冲
        /// </summary>
        /// <param name="LMC"></param>
        /// <returns></returns>
        public bool SetLMC(double LMC)
        {
            if (BoostingController == null) return false;
            return BoostingController.SetLMC(LMC);
        }
        /// <summary>
        /// 设置低能脉冲个数
        /// </summary>
        /// <param name="LMCNum"></param>
        /// <returns></returns>
        public bool SetLMCNum(double LMCNum)
        {
            if (BoostingController == null) return false;
            return BoostingController.SetLMCNum(LMCNum);
        }
        /// <summary>
        /// 停止出束
        /// </summary>
        public void StopRay()
        {
            if (BoostingController == null) return ;
            BoostingController.StopRay();
        }
        /// <summary>
        /// 预热
        /// </summary>
        /// <returns></returns>
        public bool WarmUp()
        {
            if (BoostingController == null) return false;
            return BoostingController.WarmUp();
        }
        /// <summary>
        /// 查询预热状态
        /// </summary>
        /// <returns></returns>
        public string GetRayAndPreviewHot()
        {
            if (BoostingController == null) return string.Empty;
            return BoostingController.GetRayAndPreviewHot();
        }
        /// <summary>
        /// 读取单双能
        /// </summary>
        /// <returns></returns>
        public string ReadDoubleOrSingle()
        {
            if (BoostingController == null) return string.Empty;
            return BoostingController.ReadDoubleOrSingle();
        }
        /// <summary>
        /// 读取单双能
        /// </summary>
        /// <returns></returns>
        public string ReadEnger()
        {
            if (BoostingController == null) return string.Empty;
            return BoostingController.ReadEnger();
        }
        /// <summary>
        /// 读取扫描模式
        /// </summary>
        /// <returns></returns>
        public string SearchScanMode()
        {
            if (BoostingController == null) return string.Empty;
            return BoostingController.SearchScanMode();
        }
        /// <summary>
        /// 设置高低能
        /// </summary>
        /// <param name="SetOrCancel">true --双能；false--单能</param>
        /// <returns></returns>
        public bool SetHighOrLowEnergy(bool SetOrCancel)
        {
            if (BoostingController == null) return false;
            return BoostingController.SetHighOrLowEnergy(SetOrCancel);
        }
        /// <summary>
        /// 设置单双能
        /// </summary>
        /// <param name="SetHightOrLowPressure">true 为高压，false 为低压</param>
        /// <returns></returns>
        public bool SetDoubleOrSigneEnergy(bool SetOrCancel)
        {
            if (BoostingController == null) return false;
            return BoostingController.SetDoubleOrSigneEnergy(SetOrCancel);
        }
        /// <summary>
        /// 设置高低压
        /// </summary>
        /// <param name="SetHightOrLowPressure">true 为高压，false 为低压</param>
        /// <returns></returns>
        public bool SetHighAndLowPressure(bool SetHightOrLowPressure)
        {
            if (BoostingController == null) return false;
            return BoostingController.SetHighAndLowPressure(SetHightOrLowPressure);
        }
        /// <summary>
        /// 预热
        /// </summary>
        /// <returns></returns>
        public bool WarmUpEnd()
        {
            if (BoostingController == null) return false;
            return BoostingController.WarmUpEnd();
        }
        public bool SendCommandMainMagWorkFreMode(ushort workMode)
        {
            if (BoostingController == null) return false;
            return BoostingController.SendCommandMainMagWorkFreMode(workMode);
        }
        /// <summary>
        /// 设置内部剂量
        /// </summary>
        /// <param name="workMode"></param>
        /// <returns></returns>
        public bool SendCommandDoseInternal(ushort doseInternal)
        {
            if (BoostingController == null) return false;
            return BoostingController.SendCommandDoseInternal(doseInternal);
        }
        public  bool SendCommandWaitWorkStop()
        {
            if (BoostingController == null) return false;
            return BoostingController.SendCommandWaitWorkStop();
        }

        public  bool SendCommandRadiationOn()
        {
            if (BoostingController == null) return false;
            return BoostingController.SendCommandRadiationOn();
        }

        /// <summary>
        /// 设置出束之后的状态，是保持待机预热还是停束
        /// </summary>
        /// <param name="RayAfterStatus"></param>
        /// <returns></returns>
        public bool SendCommandAfterRayWorkStatus(ushort RayAfterStatus)
        {
            if (BoostingController == null) return false;
            return BoostingController.SendCommandAfterRayWorkStatus(RayAfterStatus);
        }
        /// <summary>
        /// 设置单双能
        /// </summary>
        /// <param name="EnergyMode"></param>
        /// <returns></returns>
        public bool SendCommandEnergyMode(ushort EnergyMode)
        {
            if (BoostingController == null) return false;
            return BoostingController.SendCommandEnergyMode(EnergyMode);
        }
      
        /// <summary>
        /// 设置注入电流
        /// </summary>
        /// <param name="inject"></param>
        /// <returns></returns>
        public bool ExecuteInjection(ushort inject)
        {
            if (BoostingController == null) return false;
            return BoostingController.ExecuteInjection(inject);
        }
        /// <summary>
        /// 设置曝光时间
        /// </summary>
        /// <param name="ExposureTime"></param>
        /// <returns></returns>
        public bool SendCommandExposureTime(ushort ExposureTime)
        {
            if (BoostingController == null) return false;
            return BoostingController.SendCommandExposureTime(ExposureTime);
        }
        /// <summary>
        /// 切换能量模式 重新设置单双能
        /// </summary>
        /// <param name="CheckOrNot"></param>
        /// <param name="isShowMessageBox"></param>
        public void SwitchEngerHAndI(bool CheckOrNot, bool isShowMessageBox = true, bool isSetEnergy = true)
        {
            if (BoostingController == null) return;
            BoostingController.SwitchEngerHAndI(CheckOrNot, isShowMessageBox, isSetEnergy);
        }

        public string GetIGBTTemp()
        {
            if (BoostingController == null) return string.Empty;
            return BoostingController.GetIGBTTemp();
        }

        public string GetThyristor()
        {
            if (BoostingController == null) return string.Empty;
            return BoostingController.GetThyristor();
        }

        public string GetActureInject()
        {
            if (BoostingController == null) return string.Empty;
            return BoostingController.GetActureInject();
        }

        public string GetPulseConverterTemperature()
        {
            if (BoostingController == null) return string.Empty;
            return BoostingController.GetPulseConverterTemperature();
        }

        public string GetRadiatorTemperature()
        {
            if (BoostingController == null) return string.Empty;
            return BoostingController.GetRadiatorTemperature();
        }

        public string GetDACValueOfFilament()
        {
            if (BoostingController == null) return string.Empty;
            return BoostingController.GetDACValueOfFilament();
        }

        public string GetInjectDACValue()
        {
            if (BoostingController == null) return string.Empty;
            return BoostingController.GetInjectDACValue();
        }

        public string GetConstrainerDACValue()
        {
            if (BoostingController == null) return string.Empty;
            return BoostingController.GetConstrainerDACValue();
        }

        public string MaximumDoseRateSearchRrogress()
        {
            if (BoostingController == null) return string.Empty;
            return BoostingController.MaximumDoseRateSearchRrogress();
        }

        public string DoseRate()
        {
            if (BoostingController == null) return string.Empty;
            return BoostingController.DoseRate();
        }

        public TandI GetTandI()
        {
            if (BoostingController == null) return null;
            return BoostingController.GetTandI();
        }
        public WorkingJob GetWorkingJob()
        {
            if (BoostingController == null) return WorkingJob.WJ_NULL;
            return BoostingController.GetWorkingJob();
        }

        public bool IsReady()
        {
            if (BoostingController == null) return false;
            return BoostingController.IsReady();
        }

        public string GetCurrentEquipmentVersion()
        {
            return BoostingController?.GetCurrentEquipmentVersion();
        }

        public string GetCurrentEquipmentModel()
        {
            return BoostingController?.GetCurrentEquipmentModel();
        }

        #endregion
    }
}
