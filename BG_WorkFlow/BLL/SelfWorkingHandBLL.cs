using BG_Entities;
using BGModel;
using CMW.Common.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace BG_WorkFlow
{
    public class HandBll
    {
        public static List<CommonSettingModel> ConfigDataModelList;
        XmlBaseDataDAL xmlObject = new XmlBaseDataDAL();

        public HandBll(ControlVersion controlVersion)
        {
            ConfigDataModelList = GetHandConfigDataModel(GetPath(controlVersion));
        }

        public List<CommonSettingModel> GetHandConfigDataModel(string filePath)
        {
            //将xml路径先创立 创建节点
            return xmlObject.GetObjects<CommonSettingModel>(filePath, GlogbalBGModel.BG_CAR_BoostSettingsConfigs).ToList();
        }
        private string GetPath(ControlVersion controlVersion)
        {
            return SystemDirectoryConfig.GetInstance().GetHAND_CONFIG(controlVersion);
        }

        public bool SaveHandConfigDataModel(CommonSettingModel _TConfigDataModel, ControlVersion controlVersion = ControlVersion.Car)
        {
            return xmlObject.SaveSingleDataByCondition
                (GetPath(controlVersion), _TConfigDataModel, GlogbalBGModel.BG_CAR_BoostSettingsConfigs, "BG_COMMONSETTINGNAME", _TConfigDataModel.CommonSettingName, GlogbalBGModel.BG_CAR_BoostSettingsConfig);
        }
        public bool SaveConfigDataModel(IEnumerable<CommonSettingModel> _TConfigDataModels, ControlVersion controlVersion = ControlVersion.Car)
        {
            bool isSuccess = false;
            foreach (var item in _TConfigDataModels)
            {
                isSuccess = SaveHandConfigDataModel(item, controlVersion);
                if (!isSuccess) break;
            }
            return isSuccess;
        }
    }
    public class SelfWorkingComponentBll
    {
        public static List<CommonSettingModel> ConfigDataModelList;
        XmlBaseDataDAL xmlObject = new XmlBaseDataDAL();

        public SelfWorkingComponentBll(ControlVersion controlVersion)
        {
            ConfigDataModelList = GetSelfWorkingComponentDataModel(GetPath(controlVersion));
        }

        public List<CommonSettingModel> GetSelfWorkingComponentDataModel(string filePath)
        {
            //将xml路径先创立 创建节点
            return xmlObject.GetObjects<CommonSettingModel>(filePath, GlogbalBGModel.BG_CAR_BoostSettingsConfigs).ToList();
        }
        private string GetPath(ControlVersion controlVersion)
        {
            return SystemDirectoryConfig.GetInstance().GetComponentConfig(controlVersion);
        }

        public bool SaveSelfWorkingComponentDataModel(CommonSettingModel _TConfigDataModel, ControlVersion controlVersion = ControlVersion.Car)
        {
            return xmlObject.SaveSingleDataByCondition
                (GetPath(controlVersion), _TConfigDataModel, GlogbalBGModel.BG_CAR_BoostSettingsConfigs, "BG_COMMONSETTINGNAME", _TConfigDataModel.CommonSettingName, GlogbalBGModel.BG_CAR_BoostSettingsConfig);
        }
        public bool SaveConfigDataModel(IEnumerable<CommonSettingModel> _TConfigDataModels, ControlVersion controlVersion = ControlVersion.Car)
        {
            bool isSuccess = false;
            foreach (var item in _TConfigDataModels)
            {
                isSuccess = SaveSelfWorkingComponentDataModel(item, controlVersion);
                if (!isSuccess) break;
            }
            return isSuccess;
        }
    }
}
