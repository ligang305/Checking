using BG_Entities;
using BGModel;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG_WorkFlow
{
    public class LanguageBLL : BaseInstance<LanguageBLL>
    {
        public List<LanguageModel> LanguageModelList;
        public Dictionary<string,Language> LanguagesDic = new Dictionary<string, Language>();
        XmlBaseDataDAL xmlObject = new XmlBaseDataDAL();

        public LanguageBLL()
        {
            LanguageModelList = GetLanguageModelModel(GetPath());
        }

        public List<LanguageModel> GetLanguageModelModel(string filePath)
        {
            //将xml路径先创立 创建节点
            return xmlObject.GetObjects<LanguageModel>(filePath, GlogbalBGModel.Languages).ToList();
        }
        public List<Language> GetLanguages(string filePath)
        {
            //将xml路径先创立 创建节点
            return xmlObject.GetObjects<Language>(filePath, GlogbalBGModel.Languages).ToList();
        }
        private string GetPath()
        {
            return SystemDirectoryConfig.GetInstance().GetLanguageConfig();
        }
        private string GetLanguagePath()
        {
            return SystemDirectoryConfig.GetInstance().GetLanguage();
        }
        public Dictionary<string, List<Language>> GetLangeusgeDic()
        {
            Dictionary<string, List<Language>> TempDic = new Dictionary<string, List<Language>>();
            foreach (var languageModelItem in LanguageModelList)
            {
                if (!TempDic.ContainsKey(languageModelItem.LanguageKey))
                {
                    string FilePath = $"{GetLanguagePath()}{languageModelItem.LanguageKey}.xml";
                    if (File.Exists(FilePath))
                    {
                        var Languages = GetLanguages(FilePath);
                        TempDic.Add(languageModelItem.LanguageKey, Languages);
                    }
                }
            }

            return TempDic;
        }

    }
}
