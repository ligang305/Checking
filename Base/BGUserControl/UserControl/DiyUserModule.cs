using BG_Entities;
using BG_Services;
using BG_WorkFlow;
using BGModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static CMW.Common.Utilities.Common;

namespace BGUserControl
{
    public class DiyUserModule : UserControl, IConditionView
    {        protected Window CurrentWindow;
        protected string TitleName = string.Empty;
        protected List<ResourceDictionary> dictionaryList = new List<ResourceDictionary>();
        public DiyUserModule()
        {
            //foreach (var item in Common.LanguageList)
            //{
            //    var LanguageItem = item as LanguageModel;
            //    string RescourceDicPath = $"pack://Application:,,,/RescourceDic;component/{LanguageItem.LanguageFile}";
            //    this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(RescourceDicPath) });
            //}
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
            //Dictionary<string, ResourceDictionary> LanguageRecdic = new Dictionary<string, ResourceDictionary>();
            //if (dictionaryList.Count == 0)
            //{
            //    foreach (ResourceDictionary dictionary in Resources.MergedDictionaries)
            //    {
            //        dictionaryList.Add(dictionary);
            //    }
            //}
            //foreach (var item in Common.LanguageList)
            //{
            //    var LanguageItem = item as LanguageModel;
            //    string ResourceDictionXaml = $"RescourceDic;component/{LanguageItem.LanguageFile}";
            //    var RecItem = dictionaryList.FirstOrDefault(d => d.Source.OriginalString.Contains(ResourceDictionXaml));
            //    LanguageRecdic.Add(LanguageItem.LanguageKey, RecItem);
            //}
            //foreach (var item in LanguageRecdic)
            //{
            //    if (item.Key != ConfigServices.GetInstance().localConfigModel.LANGUAGE)
            //    {
            //        if (item.Value != null)
            //            Resources.MergedDictionaries.Remove(item.Value);
            //    }
            //}
            //ResourceDictionary tempRec = LanguageRecdic[ConfigServices.GetInstance().localConfigModel.LANGUAGE];
            //if (tempRec != null)
            //{
            //    Resources.MergedDictionaries.Remove(tempRec);
            //    Resources.MergedDictionaries.Add(tempRec);
            //}
        }
        public void Close()
        {
            CurrentWindow?.Close();
        }

        public double GetHeight()
        {
            return 400;
        }

        public virtual string GetName()
        {
            return TitleName;
        }

        public double GetWidth()
        {
            return 980;
        }

        public bool IsConnectionEquipment()
        {
            return IsConnection;
        }

        public void SetCarVersion(ControlVersion cv)
        {
          
        }

        public virtual void Show(Window _OwnerWin)
        {
           
        }

        public void SetSelectTabName(string TabName)
        {
            
        }
    }
}
