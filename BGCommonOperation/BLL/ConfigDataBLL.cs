using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace BGCommonOperation
{
    public class ConfigDataBLL : BaseInstance<ConfigDataBLL>
    {
        public static List<CommonSettingModel> ConfigDataModelList;

        public ConfigDataBLL()
        {
            ConfigDataModelList = GetConfigDataModel(SystemDirectoryConfig.GetInstance().GetDirection());
        }

        public List<CommonSettingModel> GetConfigDataModel(string filePath)
        {
            //将xml路径先创立 创建节点
            return XmlBaseDataDAL.GetInstance().GetObjects<CommonSettingModel>(filePath, Common.BGCommonSettingsConfigs).ToList();
        }

        public bool SaveConfigDataModel(CommonSettingModel _TConfigDataModel)
        {
            return XmlBaseDataDAL.GetInstance().SaveSingleDataByCondition
                (SystemDirectoryConfig.GetInstance().GetDirection(), _TConfigDataModel, Common.BGCommonSettingsConfigs, "BG_COMMONSETTINGNAME", _TConfigDataModel.CommonSettingName);
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
