using CMW.Common.Utilities;
using BGModel;
using BGUserControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using static CMW.Common.Utilities.Common;
using BG_Entities;

namespace BGUserControl
{
    public class CommonModules : BaseWindows, IUserControlView
    {

        protected void SwitchLanguage()
        {
            Dictionary<string, ResourceDictionary> LanguageRecdic = new Dictionary<string, ResourceDictionary>();
            if (dictionaryList.Count == 0)
            {
                foreach (ResourceDictionary dictionary in Resources.MergedDictionaries)
                {
                    dictionaryList.Add(dictionary);
                }
            }
            SwitchLanguage(Application.Current?.Resources.MergedDictionaries);
        }

        protected void SwitchLanguage(ICollection<ResourceDictionary> dicList)
        {
            Dictionary<string, ResourceDictionary> LanguageRecdic = new Dictionary<string, ResourceDictionary>();
            if (dictionaryList.Count == 0)
            {
                foreach (ResourceDictionary dictionary in dicList)
                {
                    dictionaryList.Add(dictionary);
                }
            }
        }

        public virtual string GetName()
        {
            return "模块";
        }
        public virtual double GetHeight()
        {
            return 400;
        }

        public virtual double GetWidth()
        {
            return 600;
        }

        public virtual bool IsConnectionEquipment()
        {
            return false;
        }


        public virtual void Show(Window _OwnerWin)
        {

        }
        public virtual void Close()
        {
            CurrentWindow?.Close();
        }

        public virtual void SetCarVersion(ControlVersion cv)
        {
            
        }

        public IEnumerable GetStatusBarStatus()
        {
            return default;
        }

        public Dictionary<string, PLCPositionEnum> GetEnergyStopPanel()
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
            return default;
        }
    }
}
