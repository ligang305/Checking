using BG_Entities;
using BG_WorkFlow;
using BGModel;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG_WorkFlow
{
    public class FontSizeServices : BaseInstance<FontSizeServices>
    {
        Dictionary<string, List<Fontsize>> Fontsizes = new Dictionary<string, List<Fontsize>>();
        List<Fontsize> CurrentFontsize = new List<Fontsize>();
        /// <summary>
        /// 字体的List
        /// </summary>
        public List<object> FontsizeList = new List<object>();
        public void Start()
        {
            InitFontSizeData();
            SwitchFontSize(ConfigServices.GetInstance().localConfigModel.FontSize);
            ButtonInvoke.SwitchFontSizeEvent += SwitchFontSize;
            CommonDeleget.UpdateFontSizeEvent -= CommonDeleget_UpdateFontSize;
            CommonDeleget.UpdateFontSizeEvent += CommonDeleget_UpdateFontSize;
        }

        public void Stop()
        {
         
            ButtonInvoke.SwitchFontSizeEvent -= SwitchFontSize;
        }
        public void InitFontSizeData()
        {
            FontsizeList = new List<Object>(FontSizeBLL.GetInstance().FontSizeModelList);
            Fontsizes = FontSizeBLL.GetInstance().GetFontsizeDic();
        }


        public double CommonDeleget_UpdateFontSize(string FontSizeKey)
        {
            return Convert.ToDouble(CurrentFontsize.Find(q => q.Key == FontSizeKey)?.size);
        }

        protected void SwitchFontSize(string FontSize)
        {
            CurrentFontsize = Fontsizes[FontSize];
        }
    }
}
