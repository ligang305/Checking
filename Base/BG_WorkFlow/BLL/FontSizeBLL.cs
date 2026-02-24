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
    public class FontSizeBLL : BaseInstance<FontSizeBLL>
    {
        public List<FontSizeModel> FontSizeModelList;
        public Dictionary<string, Fontsize> LanguagesDic = new Dictionary<string, Fontsize>();
        XmlBaseDataDAL xmlObject = new XmlBaseDataDAL();

        public FontSizeBLL()
        {
            FontSizeModelList = GetFontSizeModelModel(GetPath());
        }

        public List<FontSizeModel> GetFontSizeModelModel(string filePath)
        {
            //将xml路径先创立 创建节点
            return xmlObject.GetObjects<FontSizeModel>(filePath, GlogbalBGModel.Fontsizes).ToList();
        }
        public List<Fontsize> GetFontsizes(string filePath)
        {
            //将xml路径先创立 创建节点
            return xmlObject.GetObjects<Fontsize>(filePath, GlogbalBGModel.Sizes).ToList();
        }
        private string GetPath()
        {
            return SystemDirectoryConfig.GetInstance().GetFontSizeConfig();
        }
        private string GetFontsizePath()
        {
            return SystemDirectoryConfig.GetInstance().GetFontsize();
        }
        public Dictionary<string, List<Fontsize>> GetFontsizeDic()
        {
            Dictionary<string, List<Fontsize>> TempDic = new Dictionary<string, List<Fontsize>>();
            foreach (var fontsizeModelItem in FontSizeModelList)
            {
                if (!TempDic.ContainsKey(fontsizeModelItem.FontSize))
                {
                    string FilePath = $"{GetFontsizePath()}{fontsizeModelItem.FontSize}.xml";
                    if (File.Exists(FilePath))
                    {
                        var Languages = GetFontsizes(FilePath);
                        TempDic.Add(fontsizeModelItem.FontSize, Languages);
                    }
                }
            }

            return TempDic;
        }

    }
}
