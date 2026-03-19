using BG_Entities;
using BGModel;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG_WorkFlow
{
    public class CarDoseBLL : BaseInstance<CarDoseBLL>
    {
        XmlBaseDataDAL xmlObject = new XmlBaseDataDAL();
        public CarDoseBLL()
        {
        }

        public List<DoseModel> GetDoseModelDataModel(string filePath)
        {
            //将xml路径先创立 创建节点
            return xmlObject.GetObjects<DoseModel>(filePath, GlogbalBGModel.BG_DOSE_CONFIGS).ToList();
        }

        public bool SaveDoseModelConfigDataModel(DoseModel _TConfigDataModel,ControlVersion cv)
        {
            return xmlObject.SaveSingleDataByCondition
                (SystemDirectoryConfig.GetInstance().GetDoseConfig(cv), _TConfigDataModel, GlogbalBGModel.BG_DOSE_CONFIGS, "BG_DOSE_NAME", _TConfigDataModel.BgDoseName, GlogbalBGModel.BG_DOSE_CONFIG);
        }
        public bool SaveDoseModel(IEnumerable<DoseModel> _TConfigDataModels,ControlVersion cv)
        {
            bool isSuccess = false;
            foreach (var item in _TConfigDataModels)
            {
                isSuccess = SaveDoseModelConfigDataModel(item, cv);
                if (!isSuccess) break;
            }
            return isSuccess;
        }

    }
}
