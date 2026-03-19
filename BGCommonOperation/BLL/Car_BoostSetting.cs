using BGModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace BGCommonOperation
{
    public class Car_BoostSettingBLL : BaseInstance<Car_BoostSettingBLL>
    {
        public static List<CommonSettingModel> ConfigDataModelList;

        public Car_BoostSettingBLL()
        {
            ConfigDataModelList = GetCarBoostSettingConfigDataModel(SystemDirectoryConfig.GetInstance().GetCARBOOSTCONFIG());
        }

        public List<CommonSettingModel> GetCarBoostSettingConfigDataModel(string filePath)
        {
            //将xml路径先创立 创建节点
            return XmlBaseDataDAL.GetInstance().GetObjects<CommonSettingModel>(filePath, Common.BG_CAR_BoostSettingsConfigs).ToList();
        }

        public bool SaveCarBoostSettingConfigDataModel(CommonSettingModel _TConfigDataModel)
        {
            return XmlBaseDataDAL.GetInstance().SaveSingleDataByCondition
                (SystemDirectoryConfig.GetInstance().GetDirection(), _TConfigDataModel, Common.BG_CAR_BoostSettingsConfigs, "BG_COMMONSETTINGNAME", _TConfigDataModel.CommonSettingName);
        }
        public bool SaveConfigDataModel(IEnumerable<CommonSettingModel> _TConfigDataModels)
        {
            bool isSuccess = false;
            foreach (var item in _TConfigDataModels)
            {
                isSuccess = SaveCarBoostSettingConfigDataModel(item);
                if (!isSuccess) break;
            }
            return isSuccess;
        }
    }

    public class HitChModelBLL : BaseInstance<HitChModelBLL>
    {

        public HitChModelBLL()
        {
        }

        public List<HitchModel> GetHitchModelDataModel(string filePath)
        {
            //将xml路径先创立 创建节点
            return XmlBaseDataDAL.GetInstance().GetObjects<HitchModel>(filePath, Common.BG_CAR_HITCH_CONFIGS).ToList();
        }

        public bool SaveHitModelConfigDataModel(HitchModel _TConfigDataModel)
        {
            return XmlBaseDataDAL.GetInstance().SaveSingleDataByCondition
                (SystemDirectoryConfig.GetInstance().GetHitchConfig(), _TConfigDataModel, Common.BG_CAR_HITCH_CONFIGS, "BG_HITCHNAME", _TConfigDataModel.HitchName);
        }
        public bool SaveConfigDataModel(IEnumerable<HitchModel> _TConfigDataModels)
        {
            bool isSuccess = false;
            foreach (var item in _TConfigDataModels)
            {
                isSuccess = SaveHitModelConfigDataModel(item);
                if (!isSuccess) break;
            }
            return isSuccess;
        }
    }

}
