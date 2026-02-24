
using BG_Entities;
using BGModel;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CMW.Common.Utilities.Common;

namespace BG_WorkFlow
{
    public class ControlVersionsBLL
    {
        public static List<Bg_ControlVersion> Bg_ControlVersionList;
        XmlBaseDataDAL xmlObject = new XmlBaseDataDAL();

        public ControlVersionsBLL()
        {
            Bg_ControlVersionList = GetControlVersionsBLLDataModel();
        }
        public List<Bg_ControlVersion> GetControlVersionsBLLDataModel()
        {
            //将xml路径先创立 创建节点
            return xmlObject.GetObjects<Bg_ControlVersion>(GetPath(), GlogbalBGModel.ControlVersions).ToList();
        }
        private string GetPath()
        {
           return SystemDirectoryConfig.GetInstance().GetControlVersionsConfig();
        }

        public bool SaveControlVersionsDataModel(Bg_ControlVersion _TBg_ControlVersionModel)
        {
            return xmlObject.SaveSingleDataByCondition
                (GetPath(), _TBg_ControlVersionModel, GlogbalBGModel.ControlVersions, "Bg_ControlVersion", _TBg_ControlVersionModel.ControlVersionName, GlogbalBGModel.Bg_ControlVersion);
        }
        public bool SaveControlVersionsDataModel(IEnumerable<Bg_ControlVersion> _TCommandPlcs)
        {
            bool isSuccess = false;
            foreach (var item in _TCommandPlcs)
            {
                isSuccess = SaveControlVersionsDataModel(item);
                if (!isSuccess) break;
            }
            return isSuccess;
        }
    }
}
