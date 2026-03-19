using BG_Entities;
using BGModel;
using CMW.Common.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace BG_WorkFlow
{
    public class ConfigDataBLL
    {
        public static List<CommonSettingModel> ConfigDataModelList;
        XmlBaseDataDAL xbdl = new XmlBaseDataDAL();
        public ConfigDataBLL()
        {
            ConfigDataModelList = GetConfigDataModel(SystemDirectoryConfig.GetInstance().GetDirection());
        }

        public List<CommonSettingModel> GetConfigDataModel(string filePath)
        {
            //将xml路径先创立 创建节点
            return xbdl.GetObjects<CommonSettingModel>(filePath, GlogbalBGModel.BGCommonSettingsConfigs).ToList();
        }

        public bool SaveConfigDataModel(CommonSettingModel _TConfigDataModel)
        {
            return xbdl.SaveSingleDataByCondition
                (SystemDirectoryConfig.GetInstance().GetDirection(), _TConfigDataModel, GlogbalBGModel.BGCommonSettingsConfigs, "BG_COMMONSETTINGNAME", _TConfigDataModel.CommonSettingName, GlogbalBGModel.BGCommonSettingConfig);
        }
        public bool SaveConfigDataModel(IEnumerable<CommonSettingModel> _TConfigDataModels)
        {
            bool isSuccess = false;
            foreach (var item in _TConfigDataModels)
            {
                isSuccess = SaveConfigDataModel(item);
                if (!isSuccess) break;
            }
            return isSuccess;
        }

        public bool SaveConfigDataModel(CommonSettingModel _TConfigDataModel,string FilePath)
        {
            return xbdl.SaveSingleDataByCondition
                (FilePath, _TConfigDataModel, GlogbalBGModel.BGCommonSettingsConfigs, "BG_COMMONSETTINGNAME", _TConfigDataModel.CommonSettingName, GlogbalBGModel.BGCommonSettingConfig);
        }
        public bool SaveConfigDataModel(IEnumerable<CommonSettingModel> _TConfigDataModels, string FilePath)
        {
            bool isSuccess = false;
            foreach (var item in _TConfigDataModels)
            {
                isSuccess = SaveConfigDataModel(item, FilePath);
                if (!isSuccess) break;
            }
            return isSuccess;
        }

    }

    public class BoosterConfigBLL
    {
        public static List<CommonSettingModel> ConfigDataModelList;
        XmlBaseDataDAL xbdl = new XmlBaseDataDAL();
        public BoosterConfigBLL()
        {
            ConfigDataModelList = GetConfigDataModel(SystemDirectoryConfig.GetInstance().GetBOOSTCONFIG(Common.controlVersion));//SystemDirectoryConfig.GetInstance().GetCARBOOSTCONFIG()
        }

        public List<CommonSettingModel> GetConfigDataModel(string filePath)
        {
            //将xml路径先创立 创建节点
            return xbdl.GetObjects<CommonSettingModel>(filePath, GlogbalBGModel.BGCommonSettingsConfigs).ToList();
        }

        public bool SaveConfigDataModel(CommonSettingModel _TConfigDataModel)
        {
            return xbdl.SaveSingleDataByCondition
                (SystemDirectoryConfig.GetInstance().GetBOOSTCONFIG(Common.controlVersion),
                _TConfigDataModel, GlogbalBGModel.BGCommonSettingsConfigs, "BG_COMMONSETTINGNAME", _TConfigDataModel.CommonSettingName, GlogbalBGModel.BGCommonSettingConfig);
        }
        public bool SaveConfigDataModel(IEnumerable<CommonSettingModel> _TConfigDataModels)
        {
            bool isSuccess = false;
            foreach (var item in _TConfigDataModels)
            {
                isSuccess = SaveConfigDataModel(item);
                if (!isSuccess) break;
            }
            return isSuccess;
        }
    }

}
