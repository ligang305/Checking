using BG_Entities;
using BGModel;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BG_WorkFlow
{
    public class CarCantileverModelBLL
    {
        public static List<CarCantileverModel> ConfigDataModelList;
        XmlBaseDataDAL xmlObject = new XmlBaseDataDAL();
        public CarCantileverModelBLL(ControlVersion cv = ControlVersion.Car)
        {
            ConfigDataModelList = GetCarCantileverModel(SystemDirectoryConfig.GetInstance().GetCarCantilever(cv));
        }

        public List<CarCantileverModel> GetCarCantileverModel(string filePath)
        {
            //将xml路径先创立 创建节点
            return xmlObject.GetObjects<CarCantileverModel>(filePath, GlogbalBGModel.CarCantileverModels).ToList();
        }

        public bool SaveCarCantileverModel(CarCantileverModel _TConfigDataModel,ControlVersion cv = ControlVersion.Car)
        {
            return xmlObject.SaveSingleDataByCondition
                (SystemDirectoryConfig.GetInstance().GetCarCantilever(cv), _TConfigDataModel, GlogbalBGModel.CarCantileverModels, "BG_CARPROPNAME", _TConfigDataModel.CarPropName, GlogbalBGModel.CarCantileverModel);
        }

        public bool SaveCarCantileverModel(CarCantileverModel _TConfigDataModel,string FilePath)
        {
            return xmlObject.SaveSingleDataByCondition
                (FilePath, _TConfigDataModel, GlogbalBGModel.CarCantileverModels, "BG_CARPROPNAME", _TConfigDataModel.CarPropName, GlogbalBGModel.CarCantileverModel);
        }
        public bool SaveCarCantileverModel(IEnumerable<CarCantileverModel> _TConfigDataModels,string FilePath)
        {
            bool isSuccess = false;
            foreach (var item in _TConfigDataModels)
            {
                isSuccess = SaveCarCantileverModel(item, FilePath);
                if (!isSuccess) break;
            }
            return isSuccess;
        }
        public bool SaveCarCantileverModel(IEnumerable<CarCantileverModel> _TConfigDataModels)
        {
            bool isSuccess = false;
            foreach (var item in _TConfigDataModels)
            {
                isSuccess = SaveCarCantileverModel(item,ControlVersion.Car);
                if (!isSuccess) break;
            }
            return isSuccess;
        }

    }




}
