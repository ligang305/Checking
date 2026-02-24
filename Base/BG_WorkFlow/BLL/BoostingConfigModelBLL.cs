using BG_Entities;
using BGModel;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG_WorkFlow
{
    public class BoostingConfigModelBLL
    {
        public static List<CommonSettingModel> ConfigDataModelList;
        XmlBaseDataDAL xmlObject = new XmlBaseDataDAL();

        public BoostingConfigModelBLL(ControlVersion controlVersion)
        {
            ConfigDataModelList = GetCarBoostSettingConfigDataModel(GetPath(controlVersion));
        }

        public List<CommonSettingModel> GetCarBoostSettingConfigDataModel(string filePath)
        {
            //将xml路径先创立 创建节点
            return xmlObject.GetObjects<CommonSettingModel>(filePath, GlogbalBGModel.BG_CAR_BoostSettingsConfigs).ToList();
        }
        private string GetPath(ControlVersion controlVersion)
        {
            return SystemDirectoryConfig.GetInstance().GetBetatronBoosting(controlVersion);
        }

        public bool SaveCarBoostSettingConfigDataModel(CommonSettingModel _TConfigDataModel, ControlVersion controlVersion = ControlVersion.Car)
        {
            return xmlObject.SaveSingleDataByCondition
                (GetPath(controlVersion), _TConfigDataModel, GlogbalBGModel.BG_CAR_BoostSettingsConfigs, "BG_COMMONSETTINGNAME", _TConfigDataModel.CommonSettingName, GlogbalBGModel.BG_CAR_BoostSettingsConfig);
        }
        public bool SaveConfigDataModel(IEnumerable<CommonSettingModel> _TConfigDataModels, ControlVersion controlVersion = ControlVersion.Car)
        {
            bool isSuccess = false;
            foreach (var item in _TConfigDataModels)
            {
                isSuccess = SaveCarBoostSettingConfigDataModel(item, controlVersion);
                if (!isSuccess) break;
            }
            return isSuccess;
        }
    }
}
