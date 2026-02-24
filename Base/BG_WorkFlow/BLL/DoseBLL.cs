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
    public class DoseBLL : BaseInstance<DoseBLL>
    {
        private List<Bg_Dose> doseList = new List<Bg_Dose>();
        private DoseDal DoseDal;

        public DoseBLL()
        {
            DoseDal = new DoseDal();
            doseList = DoseDal.GetBg_DoseFromDataBase();
        }

        /// <summary>
        /// 获取剂量列表
        /// </summary>
        /// <returns></returns>
        public List<Bg_Dose> GetDoseModel()
        {
            doseList = DoseDal.GetBg_DoseFromDataBase();
            return doseList;
        }
        /// <summary>
        /// 保存剂量实体
        /// </summary>
        /// <param name="_TDoseModel"></param>
        /// <returns></returns>
        public bool SaveDoseModel(Bg_Dose _TDoseModel)
        {
            return DoseDal.UpdateDose(_TDoseModel);
        }
    }
}
