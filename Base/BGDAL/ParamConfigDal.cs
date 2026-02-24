using BGModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGDAL
{
    public class ParamConfigDal : RepositoryFactory<ParamConfig>
    {
        private static ParamConfigDal _Instance;
        //private static readonly object _lock = new object();
        public static ParamConfigDal GetInstance()
        {
            //lock (this)
            //{
            if (_Instance == null)
            {

                if (_Instance == null)
                {
                    _Instance = new ParamConfigDal();
                }
            }
            //}
            return _Instance;
        }
        /// <summary>
        /// 查询配置表
        /// </summary>
        /// <returns></returns>
        public List<ParamConfig> QueryParamConfig()
        {
            try
            {
                string sqlStr = BaseRepository().SelectAll<ParamConfig>();
                return DataTableToObject<ParamConfig>(BaseRepository().QueryDataTable(sqlStr));
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 查找当前key是否存在
        /// </summary>
        /// <returns></returns>
        public bool QueryParamConfig(ParamConfig paramConfig)
        {
            try
            {
                string sqlStr = $@"select * from BG_PARAMCONFIG where Key = '{paramConfig.Key}'";
                int rowResult = BaseRepository().QueryDataTable(sqlStr).Rows.Count;
                return rowResult > 0;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 插入配置表
        /// </summary>
        /// <param name="paramConfig"></param>
        /// <returns></returns>
        public bool InsertParamConfig(ParamConfig paramConfig)
        {
            try
            {
                string sqlStr = BaseRepository().InsertSpliteSqlByObject<ParamConfig>(paramConfig);
                BaseRepository().ExecuteNonQuerySQL(sqlStr);
                return true;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 更新配置表
        /// </summary>
        /// <returns></returns>
        public bool UpdateParamConfig(ParamConfig paramConfig)
        {
            try
            {
                string sqlStr = BaseRepository().UpdateSqlByObject<ParamConfig>(paramConfig);
                BaseRepository().ExecuteNonQuerySQL(sqlStr);
                return true;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}
