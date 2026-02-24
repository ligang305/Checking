using BG_Services;
using CMW.Common.Utilities;
using BGModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using BG_WorkFlow;

namespace BGUserControl
{
    public class BaseShowWindow : Window
    {

        protected List<ResourceDictionary> dictionaryList = new List<ResourceDictionary>();
        public BaseShowWindow()
        {
            this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://Application:,,,/RescourceDic;component/RescourceColor.xaml") });
            this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://Application:,,,/RescourceDic;component/RescourceControl.xaml") });
            this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://Application:,,,/RescourceDic;component/RescourceImage.xaml") });
            Base_SwitchLanguage(ConfigServices.GetInstance().localConfigModel.LANGUAGE);
        }

        /// <summary>
        /// 切换中英文
        /// </summary>
        public virtual void Base_SwitchLanguage(string Language)
        {
            
        }

    }
}
