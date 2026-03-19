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
    public class BGV6000BSDal : RepositoryFactory<BG_BS2000>
    {
        public BGV6000BSDal()
        {

        }
        /// <summary>
        /// 查询BsView
        /// </summary>
        /// <returns></returns>
        public List<BG_BS2000> GetBg_BsFromDataBase()
        {
            try
            {
                string sqlStr = string.Format($@"select * from BG_BGV6000BS");
                return DataTableToObject<BG_BS2000>(BaseRepository().QueryDataTable(sqlStr));
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 插入背散IpAddress
        /// </summary>
        /// <param name="CarSpeed"></param>
        /// <returns></returns>
        public bool InsertBg_Bs(BG_BS2000 BsView)
        {
            string SqlStr = BaseRepository().InsertSpliteSqlByObject<BG_BS2000>(BsView);
            return BaseRepository().ExecuteNonQuerySQL(SqlStr) > 0;
        }
        /// <summary>
        /// 删除背散6000
        /// </summary>
        /// <param name="BsView"></param>
        /// <returns></returns>
        public bool DeleteBGV6000BS(BG_BS2000 BsView)
        {
            string SqlStr = $@"delete from BG_BGV6000BS where ID = '{BsView.ID}' ";
            return BaseRepository().ExecuteNonQuerySQL(SqlStr) > 0;
        }
        /// <summary>
        /// 更新背散
        /// </summary>
        /// <param name="CarSpeed"></param>
        /// <returns></returns>
        public bool UpdateBsView(BG_BS2000 BsView)
        {
            string sqlStr = BaseRepository().UpdateSqlByObject<BG_BS2000>(BsView);
            return BaseRepository().ExecuteNonQuerySQL(sqlStr) > 0;
        }
    }

}
