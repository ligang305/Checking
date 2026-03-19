using BG_Entities;
using BGDAL;
using BGModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG_WorkFlow
{
    public class BS2000BLL : BaseInstance<BS2000BLL>
    {
        private List<BG_BS2000> bsList = new List<BG_BS2000>();
        private BS2000Dal BS2000Dal;

        public BS2000BLL()
        {
            BS2000Dal = new BS2000Dal();
            bsList = BS2000Dal.GetBg_BsFromDataBase();
        }

        /// <summary>
        /// 获取剂量列表
        /// </summary>
        /// <returns></returns>
        public List<BG_BS2000> GetBSModel()
        {
            bsList = BS2000Dal.GetBg_BsFromDataBase();
            return bsList;
        }
        /// <summary>
        /// 保存背散实体
        /// </summary>
        /// <param name="_TDoseModel"></param>
        /// <returns></returns>
        public bool SaveBSModel(BG_BS2000 _TBS2000Model)
        {
            return BS2000Dal.UpdateBsView(_TBS2000Model);
        }
        /// <summary>
        /// 插入新的
        /// </summary>
        /// <param name="_TDoseModel"></param>
        /// <returns></returns>
        public bool InsertBSModel(BG_BS2000 _TBS2000Model)
        {
            return BS2000Dal.InsertBg_Bs(_TBS2000Model);
        }
        /// <summary>
        /// 删除背散
        /// </summary>
        /// <param name="_TDoseModel"></param>
        /// <returns></returns>
        public bool DeleteBSModel(BG_BS2000 _TBS2000Model)
        {
            return BS2000Dal.DeleteBs2000(_TBS2000Model);
        }
    }
}
