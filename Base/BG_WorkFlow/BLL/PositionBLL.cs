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
    public class PositionBLL: BaseInstance<PositionBLL>
    {
        public static List<StatusModel> ConfigDataModelList;
        XmlBaseDataDAL xmlObject = new XmlBaseDataDAL();

        public PositionBLL()
        {
        }
        public List<StatusModel> GetPositionModel(string filePath)
        {
            //将xml路径先创立 创建节点
            return xmlObject.GetObjects<StatusModel>(filePath, GlogbalBGModel.BG_CAR_STATUS_CONFIGS).ToList();
        }


        public List<StatusModel> GetPositionModel(ControlVersion controlVersion, PositionConfigType positionConfigType)
        {
            //将xml路径先创立 创建节点
            return xmlObject.GetObjects<StatusModel>(GetPath(controlVersion, positionConfigType), GlogbalBGModel.BG_CAR_STATUS_CONFIGS).ToList();
        }
        private string GetPath(ControlVersion controlVersion, PositionConfigType positionConfigType)
        {
            switch (positionConfigType)
            {
                case PositionConfigType.OPosition:
                    return SystemDirectoryConfig.GetInstance().GetOPosition(controlVersion);
                case PositionConfigType.IPosition:
                    return SystemDirectoryConfig.GetInstance().GetIPosition(controlVersion);
                case PositionConfigType.DIntPosition:
                    return SystemDirectoryConfig.GetInstance().GetDIntPosition(controlVersion);
                case PositionConfigType.IntPosition:
                    return SystemDirectoryConfig.GetInstance().GetIntPosition(controlVersion);
                case PositionConfigType.FloatPosition:
                    return SystemDirectoryConfig.GetInstance().GetFloatPosition(controlVersion);
                default:
                    break;
            }
            return string.Empty;
        }
    }
}
