using BG_Entities;
using BGModel;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BG_WorkFlow
{
    public class Car_BoostSettingBLL
    {
        public static List<CommonSettingModel> ConfigDataModelList;
        XmlBaseDataDAL xmlObject = new XmlBaseDataDAL();

        public Car_BoostSettingBLL(ControlVersion controlVersion)
        {
            ConfigDataModelList = GetCarBoostSettingConfigDataModel(GetPath(controlVersion));
        }
        public Car_BoostSettingBLL(string FilePath)
        {
            ConfigDataModelList = GetCarBoostSettingConfigDataModel(FilePath);
        }
        public List<CommonSettingModel> GetCarBoostSettingConfigDataModel(string filePath)
        {
            //将xml路径先创立 创建节点
            return xmlObject.GetObjects<CommonSettingModel>(filePath, GlogbalBGModel.BG_CAR_BoostSettingsConfigs).ToList();
        }
        private string GetPath(ControlVersion controlVersion)
        {
            return SystemDirectoryConfig.GetInstance().GetBOOSTCONFIG(controlVersion);
            //switch (controlVersion)
            //{
            //    case ControlVersion.Car:
            //        return SystemDirectoryConfig.GetInstance().GetCARBOOSTCONFIG();
            //    case ControlVersion.FastCheck:
            //        return SystemDirectoryConfig.GetInstance().GetFASTCHECKBOOSTCONFIG();
            //    case ControlVersion.CombinedMovement:
            //        return SystemDirectoryConfig.GetInstance().GetCOMBINEDMOVEMENTBOOSTCONFIG();
            //    default:
            //        return SystemDirectoryConfig.GetInstance().GetCARBOOSTCONFIG();
            //}
        }
        public bool SaveCarBoostSettingConfigDataModel(CommonSettingModel _TConfigDataModel,string Path)
        {
            return xmlObject.SaveSingleDataByCondition
                (Path, _TConfigDataModel, GlogbalBGModel.BG_CAR_BoostSettingsConfigs, "BG_COMMONSETTINGNAME", _TConfigDataModel.CommonSettingName, GlogbalBGModel.BG_CAR_BoostSettingsConfig);
        }
        public bool SaveCarBoostSettingConfigDataModel(CommonSettingModel _TConfigDataModel,ControlVersion controlVersion = ControlVersion.Car)
        {
            return xmlObject.SaveSingleDataByCondition
                (GetPath(controlVersion), _TConfigDataModel, GlogbalBGModel.BG_CAR_BoostSettingsConfigs, "BG_COMMONSETTINGNAME", _TConfigDataModel.CommonSettingName, GlogbalBGModel.BG_CAR_BoostSettingsConfig);
        }
        public bool SaveConfigDataModel(IEnumerable<CommonSettingModel> _TConfigDataModels,ControlVersion controlVersion = ControlVersion.Car)
        {
            bool isSuccess = false;
            foreach (var item in _TConfigDataModels)
            {
                isSuccess = SaveCarBoostSettingConfigDataModel(item, controlVersion);
                if (!isSuccess) break;
            }
            return isSuccess;
        }
        public bool SaveConfigDataModel(IEnumerable<CommonSettingModel> _TConfigDataModels,string Path, ControlVersion controlVersion = ControlVersion.Car)
        {
            bool isSuccess = false;
            foreach (var item in _TConfigDataModels)
            {
                isSuccess = SaveCarBoostSettingConfigDataModel(item, Path);
                if (!isSuccess) break;
            }
            return isSuccess;
        }
    }

    public class Self_BoostSettingBLL
    {
        public List<BoostingModel> BoostingModelList;
        XmlBaseDataDAL xmlObject = new XmlBaseDataDAL();

        public Self_BoostSettingBLL(ControlVersion controlVersion)
        {
            BoostingModelList = GetBoostingModelModel(GetPath(controlVersion));
        }

        public List<BoostingModel> GetBoostingModelList(ControlVersion controlVersion)
        {
            BoostingModelList = GetBoostingModelModel(GetPath(controlVersion));
            return BoostingModelList;
        }


        public List<BoostingModel> GetBoostingModelModel(string filePath)
        {
            //将xml路径先创立 创建节点
            return xmlObject.GetObjects<BoostingModel>(filePath, GlogbalBGModel.BG_SELFWORKAUTOBOOSTINGCONFIGS).ToList();
        }
        private string GetPath(ControlVersion controlVersion)
        {
            switch (controlVersion)
            {
                case ControlVersion.SelfWorking:
                    return SystemDirectoryConfig.GetInstance().GetBOOSTCONFIG(controlVersion);
                case ControlVersion.CombinedMovementBetatron:
                    return SystemDirectoryConfig.GetInstance().GetBOOSTCONFIG(controlVersion);
                //case ControlVersion.BGV5100Russia:
                //return SystemDirectoryConfig.GetInstance().GetBOOSTCONFIG(controlVersion);
                case ControlVersion.BGV5100FH:
                    return SystemDirectoryConfig.GetInstance().GetBOOSTCONFIG(controlVersion);
                case ControlVersion.BGV5100:
                    return SystemDirectoryConfig.GetInstance().GetBOOSTCONFIG(controlVersion);
                    
                default:
                    return string.Empty;
            }
        }

        public bool SaveBoostingModelModel(BoostingModel _TConfigDataModel, ControlVersion controlVersion = ControlVersion.SelfWorking)
        {
            return xmlObject.SaveSingleDataByCondition
                (GetPath(controlVersion), _TConfigDataModel, GlogbalBGModel.BG_SELFWORKAUTOBOOSTINGCONFIGS, "NAME", _TConfigDataModel.Name,
                GlogbalBGModel.BG_SELFWORKAUTOBOOSTINGCONFIG);
        }
        public bool SaveConfigDataModel(IEnumerable<BoostingModel> _TConfigDataModels, ControlVersion controlVersion = ControlVersion.SelfWorking)
        {
            bool isSuccess = false;
            foreach (var item in _TConfigDataModels)
            {
                isSuccess = SaveBoostingModelModel(item, controlVersion);
                if (!isSuccess) break;
            }
            return isSuccess;
        }
    }


    public class HitChModelBLL : BaseInstance<HitChModelBLL>
    {
        XmlBaseDataDAL xmlObject = new XmlBaseDataDAL();
        public HitChModelBLL()
        {
        }

        public List<StatusModel> GetHitchModelDataModel(string filePath)
        {
            //将xml路径先创立 创建节点
            return xmlObject.GetObjects<StatusModel>(filePath, GlogbalBGModel.BG_CAR_STATUS_CONFIGS).ToList();
        }

        public bool SaveHitModelConfigDataModel(StatusModel _TConfigDataModel)
        {
            return xmlObject.SaveSingleDataByCondition
                (SystemDirectoryConfig.GetInstance().GetHittingConfig(ControlVersion.Car), _TConfigDataModel, GlogbalBGModel.BG_CAR_STATUS_CONFIGS, "BG_STATUSNAME", _TConfigDataModel.StatusName, GlogbalBGModel.BG_CAR_STATUS_CONFIG);
        }
        public bool SaveConfigDataModel(IEnumerable<StatusModel> _TConfigDataModels)
        {
            bool isSuccess = false;
            foreach (var item in _TConfigDataModels)
            {
                isSuccess = SaveHitModelConfigDataModel(item);
                if (!isSuccess) break;
            }
            return isSuccess;
        }

        public Dictionary<string, ObservableCollection<Object>> GetHitModelDic(string name, ControlVersion controlVersion = ControlVersion.Car)
        {
            string FilePath = GetFilePath(controlVersion);
            Dictionary<string, ObservableCollection<Object>> hicModelDic = new Dictionary<string, ObservableCollection<Object>>();
            List<StatusModel> HitchModelList = GetHitchModelDataModel(FilePath).Where(q => q.StatusOwner.Contains(name)).ToList();

            foreach (var hitchItem in HitchModelList)
            {
                if (!hicModelDic.ContainsKey(hitchItem.StatusOwner))
                {
                    hicModelDic[hitchItem.StatusOwner] = new ObservableCollection<Object>();
                }
                hicModelDic[hitchItem.StatusOwner].Add(hitchItem);
            }
            return hicModelDic;
        }
        public ObservableCollection<StatusModel> GetHitModelList(ControlVersion controlVersion = ControlVersion.Car)
        {
            ObservableCollection<StatusModel> StatusModelList = new ObservableCollection<StatusModel>();
            string FilePath = GetFilePath(controlVersion);
            List<StatusModel> HitchModelList = GetHitchModelDataModel(FilePath);
            foreach (var hitchItem in HitchModelList)
            {
                StatusModelList.Add(hitchItem);
            }
            return StatusModelList;
        }

        public ObservableCollection<StatusModel> GetHitModelList(string FilePath,ControlVersion controlVersion = ControlVersion.Car)
        {
            ObservableCollection<StatusModel> StatusModelList = new ObservableCollection<StatusModel>();
            List<StatusModel> HitchModelList = GetHitchModelDataModel(FilePath);
            foreach (var hitchItem in HitchModelList)
            {
                StatusModelList.Add(hitchItem);
            }
            return StatusModelList;
        }

        private string GetFilePath(ControlVersion controlVersio) 
        {
           return SystemDirectoryConfig.GetInstance().GetHittingConfig(controlVersio);
        }


        public class CenterTreeBLL : BaseInstance<CenterTreeBLL>
        {
            XmlBaseDataDAL xmlObject = new XmlBaseDataDAL();
            public CenterTreeBLL()
            {
            }

            public List<StatusTreeModel> GetStatusTreeModelDataModel(string filePath)
            {
                //将xml路径先创立 创建节点
                return xmlObject.GetObjects<StatusTreeModel>(filePath, GlogbalBGModel.BG_TREE_CONFIGS).ToList();
            }

            public bool SaveStatusTreeModelDataModel(StatusTreeModel _TConfigDataModel)
            {
                return xmlObject.SaveSingleDataByCondition
                    (SystemDirectoryConfig.GetInstance().GetTreeConfig(ControlVersion.Car), _TConfigDataModel, GlogbalBGModel.BG_TREE_CONFIGS, "BG_STATUSNAME", _TConfigDataModel.Bg_TreeName, GlogbalBGModel.BG_TREE_CONFIG);
            }
            public bool SaveConfigDataModel(IEnumerable<StatusTreeModel> _TConfigDataModels)
            {
                bool isSuccess = false;
                foreach (var item in _TConfigDataModels)
                {
                    isSuccess = SaveStatusTreeModelDataModel(item);
                    if (!isSuccess) break;
                }
                return isSuccess;
            }

            public ObservableCollection<Object> GetStatusTreeModelDic(string name = "", ControlVersion controlVersion = ControlVersion.Car)
            {
                string FilePath = GetFilePath(controlVersion);
                Dictionary<string, ObservableCollection<Object>> hicModelDic = new Dictionary<string, ObservableCollection<Object>>();
                List<StatusTreeModel> StatusTreeList = GetStatusTreeModelDataModel(FilePath).Where(q => q.Bg_TreeName.Contains(name)).ToList();

                return new ObservableCollection<object>(StatusTreeList.ToArray());
            }


            private string GetFilePath(ControlVersion controlVersio)
            {
                return SystemDirectoryConfig.GetInstance().GetTreeConfig(controlVersio);
            }
        }



        public class BoostingStatusBLL : BaseInstance<BoostingStatusBLL>
        {
            XmlBaseDataDAL xmlObject = new XmlBaseDataDAL();
            public BoostingStatusBLL()
            {
            }

            public List<BoostingStatusModel> GetBoostingStatusModelModelDataModel(string filePath)
            {
                //将xml路径先创立 创建节点
                return xmlObject.GetObjects<BoostingStatusModel>(filePath, GlogbalBGModel.BG_BOOSTING_CONFIGS).ToList();
            }
         

            public ObservableCollection<Object> GetStatusTreeModelDic(string name = "", ControlVersion controlVersion = ControlVersion.SelfWorking)
            {
                string FilePath = GetFilePath(controlVersion);
                Dictionary<string, ObservableCollection<Object>> hicModelDic = new Dictionary<string, ObservableCollection<Object>>();
                List<BoostingStatusModel> StatusTreeList = GetBoostingStatusModelModelDataModel(FilePath).Where(q => q.Bg_BoostingName.Contains(name)).ToList();

                return new ObservableCollection<object>(StatusTreeList.ToArray());
            }


            private string GetFilePath(ControlVersion controlVersio)
            {
                return SystemDirectoryConfig.GetInstance().GetBetatronBoosting(controlVersio);
            }
        }
    }



}
