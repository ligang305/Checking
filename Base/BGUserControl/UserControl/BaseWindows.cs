using CMW.Common.Utilities;
using BGModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static CMW.Common.Utilities.CommonDeleget;
using BG_Services;
using BG_WorkFlow;

namespace BGUserControl
{
    public class BaseWindows:UserControl
    {
        protected Window CurrentWindow = null;
        protected string UnConnectionWithPlc;// = "位于PLC进行连接，请检测连接状态!";
        protected string EquipmentResetSuccess;// = "设备复位成功!";
        protected string EquipmentResetFair;// = "设备复位失败!";
        protected string BoosttingSetting;//= (string);
        protected List<ResourceDictionary> dictionaryList = new List<ResourceDictionary>();
        public BaseWindows()
        {
            //this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://Application:,,,/RescourceDic;component/RescourceColor.xaml") });
            //this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://Application:,,,/RescourceDic;component/RescourceControl.xaml") });
            //this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://Application:,,,/RescourceDic;component/RescourceImage.xaml") });
            var colorDict = SafeResourceLoader.LoadResourceDictionary("RescourceDic", "RescourceColor.xaml");
            var controlDict = SafeResourceLoader.LoadResourceDictionary("RescourceDic", "RescourceControl.xaml");
            var imageDict = SafeResourceLoader.LoadResourceDictionary("RescourceDic", "RescourceImage.xaml");
            BoosttingSetting = "BoosttingSetting";
            EquipmentResetFair = UpdateStatusNameAction("EquipmentResetFair");
            EquipmentResetSuccess = UpdateStatusNameAction("EquipmentResetSuccess");
            UnConnectionWithPlc = UpdateStatusNameAction("UnConnectionWithPlc");
            Base_SwitchLanguage(ConfigServices.GetInstance().localConfigModel.LANGUAGE);
        }
        /// <summary>
        /// 切换中英文
        /// </summary>
        public virtual void Base_SwitchLanguage(string Language)
        {
            
        }
    }

    public class SafeResourceLoader
    {
        public static ResourceDictionary LoadResourceDictionary(string assemblyName, string componentPath)
        {
            try
            {
                // 方法1: 使用 Application.LoadComponent
                var uri = new Uri($"/{assemblyName};component/{componentPath}", UriKind.Relative);
                return Application.LoadComponent(uri) as ResourceDictionary;
            }
            catch
            {
                try
                {
                    // 方法2: 使用 ResourceDictionary 直接加载
                    var uri = new Uri($"pack://application:,,,/{assemblyName};component/{componentPath}");
                    return new ResourceDictionary { Source = uri };
                }
                catch (Exception ex)
                {
                    return new ResourceDictionary(); // 返回空字典
                }
            }
        }
    }
}
