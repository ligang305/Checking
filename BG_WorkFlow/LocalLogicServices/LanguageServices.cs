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
    public class LanguageServices : BaseInstance<LanguageServices>
    {
        /// <summary>
        /// 语言的List
        /// </summary>
        public List<object> LanguageList = new List<object>();

        Dictionary<string, List<Language>> Languages = new Dictionary<string, List<Language>>();

        List<Language> CurrentLanguage = new List<Language>();

        public void  Start()
        {
            InitLanguageData();
            SwitchLanguage(ConfigServices.GetInstance().localConfigModel.LANGUAGE);
            ButtonInvoke.SwitchLanguageEvent += SwitchLanguage;
            CommonDeleget.UpdateStatusNameEvent -= CommonDeleget_UpdateStatusNameEvent;
            CommonDeleget.UpdateStatusNameEvent += CommonDeleget_UpdateStatusNameEvent;
        }

        public void Stop()
        {
            ButtonInvoke.SwitchLanguageEvent -= SwitchLanguage;
        }
        /// <summary>
        /// 获取语言包配置文件
        /// </summary>
        private void InitLanguageData()
        {
            LanguageList = new List<Object>(LanguageBLL.GetInstance().LanguageModelList);
            Languages = LanguageBLL.GetInstance().GetLangeusgeDic();
        }

        /// <summary>
        /// 全局通过委托来获取中英文
        /// </summary>
        /// <param name="Status"></param>
        /// <returns></returns>
        private string CommonDeleget_UpdateStatusNameEvent(string Status)
        {
            var Language = CurrentLanguage.Find(q => q.KEY == Status)?.Value;
            return Language?? Status;
        }


        protected void SwitchLanguage(string Language)
        {
            CurrentLanguage = Languages[Language];
        }
    }
}
