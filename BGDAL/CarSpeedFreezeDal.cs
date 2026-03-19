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
    public class CarSpeedFreezeDal : RepositoryFactory<Bg_Carspeed>
    {
        //private static string DataBaseStr = SystemDirectoryConfig.GetInstance().GetSystemDataBase();
        public CarSpeedFreezeDal()
        {

        }
        public List<Bg_Carspeed> GetBg_CarspeedFromDataBase()
        {
            try
            {
                string sqlStr = string.Format($@"select * from BG_CARSPEED");
                return DataTableToObject<Bg_Carspeed>(BaseRepository().QueryDataTable(sqlStr));
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
        public bool InsertCarSpeed(Bg_Carspeed CarSpeed)
        {
            string SqlStr = BaseRepository().InsertSpliteSqlByObject<Bg_Carspeed>(CarSpeed);
            return BaseRepository().ExecuteNonQuerySQL(SqlStr) > 0;
        }
        /// <summary>
        /// 删除车速
        /// </summary>
        /// <param name="CarSpeed"></param>
        /// <returns></returns>
        public bool DeleteCarSpeed(Bg_Carspeed CarSpeed)
        {
            string SqlStr = $@"delete from BG_CARSPEED where No = '{CarSpeed.No}' ";
            return BaseRepository().ExecuteNonQuerySQL(SqlStr) > 0;
        }
        /// <summary>
        /// 更新指定的车速
        /// </summary>
        /// <param name="CarSpeed"></param>
        /// <returns></returns>
        public bool UpdateCarSpeed(Bg_Carspeed CarSpeed)
        {
            string sqlStr = BaseRepository().UpdateSqlByObject<Bg_Carspeed>(CarSpeed);
            //sqlStr += $@" where NO = '{CarSpeed.No}'";
            return BaseRepository().ExecuteNonQuerySQL(sqlStr) > 0;
        }
    }
}
