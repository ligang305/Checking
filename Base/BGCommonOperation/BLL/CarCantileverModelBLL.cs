using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BGCommonOperation
{
    public class CarCantileverModelBLL : BaseInstance<CarCantileverModelBLL>
    {
        public static List<CarCantileverModel> ConfigDataModelList;

        public CarCantileverModelBLL()
        {
            ConfigDataModelList = GetCarCantileverModel(SystemDirectoryConfig.GetInstance().GetCarCantilever());
        }

        public List<CarCantileverModel> GetCarCantileverModel(string filePath)
        {
            //将xml路径先创立 创建节点
            return XmlBaseDataDAL.GetInstance().GetObjects<CarCantileverModel>(filePath, Common.CarCantileverModels).ToList();
        }

        public bool SaveCarCantileverModel(CarCantileverModel _TConfigDataModel)
        {
            return XmlBaseDataDAL.GetInstance().SaveSingleDataByCondition
                (SystemDirectoryConfig.GetInstance().GetCarCantilever(), _TConfigDataModel, Common.CarCantileverModels, "BG_CARPROPNAME", _TConfigDataModel.CarPropName);
        }
        public bool SaveCarCantileverModel(IEnumerable<CarCantileverModel> _TConfigDataModels)
        {
            bool isSuccess = false;
            foreach (var item in _TConfigDataModels)
            {
                isSuccess = SaveCarCantileverModel(item);
                if (!isSuccess) break;
            }
            return isSuccess;
        }

    }
}
