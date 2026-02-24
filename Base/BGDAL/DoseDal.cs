using BGModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGDAL
{
    /// <summary>
    /// 用于车速频率的控制的Dal数据处理层
    /// </summary>
    public class DoseDal : RepositoryFactory<Bg_Dose>
    {
        //private static string DataBaseStr = SystemDirectoryConfig.GetInstance().GetSystemDataBase();
        public DoseDal()
        {

        }
        public List<Bg_Dose> GetBg_DoseFromDataBase()
        {
            try
            {
                string sqlStr = string.Format($@"select * from BG_DOSE");
                return DataTableToObject<Bg_Dose>(BaseRepository().QueryDataTable(sqlStr));
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 插入新的车速
        /// </summary>
        /// <param name="CarSpeed"></param>
        /// <returns></returns>
        public bool InsertBg_Dose(Bg_Dose Dose)
        {
            string SqlStr = BaseRepository().InsertSpliteSqlByObject<Bg_Dose>(Dose);
            return BaseRepository().ExecuteNonQuerySQL(SqlStr) > 0;
        }
        /// <summary>
        /// 删除车速
        /// </summary>
        /// <param name="CarSpeed"></param>
        /// <returns></returns>
        public bool DeleteDose(Bg_Dose Dose)
        {
            string SqlStr = $@"delete from BG_DOSE where ID = '{Dose.ID}' ";
            return BaseRepository().ExecuteNonQuerySQL(SqlStr) > 0;
        }
        /// <summary>
        /// 更新指定的车速
        /// </summary>
        /// <param name="CarSpeed"></param>
        /// <returns></returns>
        public bool UpdateDose(Bg_Dose Dose)
        {
            string sqlStr = BaseRepository().UpdateSqlByObject<Bg_Dose>(Dose);
            //sqlStr += $@" where ID = '{Dose.ID}'";
            return BaseRepository().ExecuteNonQuerySQL(sqlStr) > 0;
        }
    }
}
