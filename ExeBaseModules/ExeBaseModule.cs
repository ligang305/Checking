using BG_Entities;
using BG_Services;
using BG_WorkFlow;
using BGCommunication;
using BGDAL;
using BGLogs;
using BGModel;
using BGUserControl;
using CMW.Common.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
/// <summary>
/// 所有窗体的基类
/// </summary>

namespace ExeBaseModules
{
    public abstract class ExeBaseModule : UserControl, IUserControlView
    {
        private CompositionContainer container = null;
        //你要导入的类型和元数据数组，所有继承IConditionView接口并导出的页面都在这个数组里  
        [ImportMany("PageModule", typeof(IConditionView), Source = ImportSource.Local, RequiredCreationPolicy = CreationPolicy.NonShared)]
        //保存插件的内存对象
        public Lazy<IConditionView, IMetaData>[] UserControlPlugins { get; set; }
        protected IConditionView CurrentModule;
        /// <summary>
        /// 记录前台操作的全局对象
        /// </summary>
        public UIList<ListViewLogViewMode> ListViewLogViewModeList = new UIList<ListViewLogViewMode>() { };
        protected IScan ScanEquipment;
        protected  bool? isVisible = false;
   
        public ExeBaseModule()
        {
           
        }


        public ExeBaseModule(ControlVersion cv, PLCProtocol pp)
        {
            InitProtocolAccrodDiffVersion(cv);
            InitResources();
            Loaded += ExeBaseModule_Loaded;
        }

        /// <summary>
        /// 通过不同的版本，初始化不同的协议内容
        /// </summary>
        /// <param name="cv"></param>
        public virtual void InitProtocolAccrodDiffVersion(ControlVersion cv)
        {
            if (cv == ControlVersion.Car)
            {
                InitArmPlcConnection(new ARMProtocol());
            }
        }


        private void ExeBaseModule_Loaded(object sender, RoutedEventArgs e)
        {
            ExcuteLoadWindow();
        }
     

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="pp"></param>
        public virtual void InitArmPlcConnection(ARMProtocol pp)
        {
            GlogbalArmProtocol = pp;
            GlogbalArmProtocol.WaitRecTick = 300;
            ConnectInterface client = new UDPClient(ConfigServices.GetInstance().localConfigModel.ArmLocalIpAddress,
                ConfigServices.GetInstance().localConfigModel.ArmIpAddress, ConfigServices.GetInstance().localConfigModel.ArmPort, ConfigServices.GetInstance().localConfigModel.ArmLocalPort,
                Convert.ToBoolean(ConfigServices.GetInstance().localConfigModel.KeepLive),
                Convert.ToBoolean(ConfigServices.GetInstance().localConfigModel.Heart));
            GlogbalArmProtocol.InitConnection(ref client);
        }

        private void InitResources()
        {
            this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://Application:,,,/RescourceDic;component/RescourceColor.xaml") });
            this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://Application:,,,/RescourceDic;component/RescourceControl.xaml") });
            this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://Application:,,,/RescourceDic;component/RescourceImage.xaml") });
        }

        /// <summary>
        /// ShowModuels
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="cv"></param>
        protected void ShowModules(string Name, ControlVersion cv)
        {
            bool isFind = FindModules(Name, cv);
            if(isFind) ShowModules();
        }
        /// <summary>
        /// 找到模块
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="cv"></param>
        /// <returns></returns>
        protected bool FindModules(string Name, ControlVersion cv)
        {
            if (UserControlPlugins.FirstOrDefault(q => Name.Contains(q.Metadata.Name)) == null)
            {
                BG_MESSAGEBOX.Show(UpdateStatusNameAction("Tip"), UpdateStatusNameAction("ModulesLost"));
                return false;
            }
            else
            {
                CurrentModule?.Close();
                CurrentModule = UserControlPlugins.First(q => Name == (q.Metadata.Name)).Value;
                CurrentModule.SetCarVersion(cv);
                CurrentModule.SetSelectTabName(string.Empty);// = string.Empty;
                return true;
            }
        }

        /// <summary>
        /// 显示模块
        /// </summary>
        protected void ShowModules()
        {
            CurrentModule.Show(new Window()
            {
                Owner = Application.Current?.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true
            });
        }
        protected bool StartBytePositionDefaultText(CommonSettingModel CommondTag, string SelectText)
        {
            bool isSendSuccess = false;

            ushort content = 0;
            switch (CommondTag.CommonSettingValueType)
            {
                case "ushort":
                    content = Convert.ToUInt16(SelectText);
                    if (CommondTag.CommonSettingName.Contains("FrequencySetting"))
                    {
                        if (!string.IsNullOrEmpty(CommondTag.Error)) return false;
                        isSendSuccess = ImageImportDll.SX_SetFrequency(ImageImportDll.intPtr, content) == 0 ? true : false;
                        ConfigServices.GetInstance().localConfigModel.Freeze = content.ToString();
                        UpdateConfigs("Frequency", ConfigServices.GetInstance().localConfigModel.Freeze, Section.SOFT);
                    }
                    else
                    {
                        if (CommondTag.CommonSettingPLCValue.Contains("M") ||
                            CommondTag.CommonSettingPLCValue.Contains("DB") ||
                            CommondTag.CommonSettingPLCValue.Contains("I") ||
                            CommondTag.CommonSettingPLCValue.Contains("O"))
                        {
                            isSendSuccess = PLCControllerManager.GetInstance().WritePositionValue(CommondTag.CommonSettingPLCValue, content);
                        }
                        else
                        {
                            byte StartPosition = Convert.ToByte(CommondTag.CommonSettingPLCValue);
                            isSendSuccess = PLCControllerManager.GetInstance().WritePositionValue(StartPosition, content);
                        }
                    }
                    break;
                case "float":
                    var floatContent = Convert.ToSingle(SelectText);
                    if (CommondTag.CommonSettingPLCValue.Contains("M") ||
                         CommondTag.CommonSettingPLCValue.Contains("DB") ||
                         CommondTag.CommonSettingPLCValue.Contains("I") ||
                         CommondTag.CommonSettingPLCValue.Contains("O"))
                    {
                        isSendSuccess = PLCControllerManager.GetInstance().WritePositionValue(CommondTag.CommonSettingPLCValue, floatContent);
                    }
                    else
                    {
                        byte _StartPosition = Convert.ToByte(CommondTag.CommonSettingPLCValue);
                        isSendSuccess = PLCControllerManager.GetInstance().WritePositionValue(_StartPosition, floatContent);
                    }
                    break;
            }
            return isSendSuccess;
        }
        /// <summary>
        /// 窗体加载时调用插件的方法
        /// </summary>
        private void ExcuteLoadWindow()
        {
            Log.GetDistance().WriteInfoLogs($"ExeBaseModule loading BGUserControl.dll");
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
                        WriteLogAction(ce.StackTrace, LogType.NormalLog);
                    }
                    UserControlPlugins.OrderBy(p => p.Metadata.Priority);
                }

                Log.GetDistance().WriteInfoLogs($"StartUgr");
                StartUgrApplication.Services.StartUgr();
                Log.GetDistance().WriteInfoLogs($"ExeBaseModule loading BGUserControl.dll ending");
            }
            catch (Exception ex)
            {
                Log.GetDistance().WriteInfoLogs($"Module File  Error:{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 调用web接口取消扫描
        /// </summary>
        protected void CancelScanWeb()
        {
            CommonDeleget.CancelTaskAction();
        }

        private string GetModulesPath()
        {
            return string.Empty;
        }

        public void SetCarVersion(ControlVersion cv)
        {

        }

        public virtual double GetHeight()
        {
            return 500;
        }

        public virtual double GetWidth()
        {
            return 500;
        }

        public virtual string GetName()
        {
            return "车载";
        }

        public virtual bool IsConnectionEquipment()
        {
            return true;
        }

        public virtual void Show(Window _OwnerWin)
        {

        }

        public void Close()
        {

        }

        public IEnumerable GetStatusBarStatus()
        {
            return default;
        }

        public virtual Dictionary<string, PLCPositionEnum> GetEnergyStopPanel()
        {
            return default;
        }

        public BitmapImage GetRayImage()
        {
            return default;
        }

        public List<Tuple<Point, Point>> GetRayPointList()
        {
            return default;
        }

        public BitmapImage GetMainImage()
        {
            return default;
        }

        public Visibility IsShowElctronicFence()
        {
            return Visibility.Collapsed;
        }
    }
}
