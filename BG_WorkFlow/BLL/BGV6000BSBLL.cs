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

    public class BGV6000BSBLL : BaseInstance<BGV6000BSBLL>
    {
        private List<BG_BS2000> bsList = new List<BG_BS2000>();
        private BGV6000BSDal BGV6000BSDal;

        public BGV6000BSBLL()
        {
            BGV6000BSDal = new BGV6000BSDal();
            bsList = BGV6000BSDal.GetBg_BsFromDataBase();
        }

        /// <summary>
        /// 获取剂量列表
        /// </summary>
        /// <returns></returns>
        public List<BG_BS2000> GetBSModel()
        {
            bsList = BGV6000BSDal.GetBg_BsFromDataBase();
            return bsList;
        }
        /// <summary>
        /// 保存背散实体
        /// </summary>
        /// <param name="_TDoseModel"></param>
        /// <returns></returns>
        public bool SaveBSModel(BG_BS2000 _TBGV6000BSModel)
        {
            return BGV6000BSDal.UpdateBsView(_TBGV6000BSModel);
        }
        /// <summary>
        /// 插入新的
        /// </summary>
        /// <param name="_TDoseModel"></param>
        /// <returns></returns>
        public bool InsertBSModel(BG_BS2000 _TBGV6000BSModel)
        {
            return BGV6000BSDal.InsertBg_Bs(_TBGV6000BSModel);
        }
        /// <summary>
        /// 删除背散
        /// </summary>
        /// <param name="_TDoseModel"></param>
        /// <returns></returns>
        public bool DeleteBSModel(BG_BS2000 _TBGV6000BSModel)
        {
            //return BGV6000BSDal.DeleteBs2000(_TBGV6000BSModel);
            return BGV6000BSDal.DeleteBGV6000BS(_TBGV6000BSModel);
        }
    }

}
