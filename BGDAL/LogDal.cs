using BGModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGDAL
{
    public class LogDal : RepositoryFactory<BG_Logs>
    {
        public LogDal()
        {
            if (!base.BaseRepository().IsExistIndex<BG_Logs>("LogName"))
                base.BaseRepository().CreateIndex<BG_Logs>("LogName", "BGLog", "LogName");
        }
        public List<BG_Logs> GetLog()
        {
            try
            {
                string sqlStr = string.Format($@"{BaseRepository().SelectAll<BG_Logs>()} Order by LogDataTime desc ");
                return DataTableToObject<BG_Logs>(BaseRepository().QueryDataTable(sqlStr));
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 获取日志信息
        /// </summary>
        /// <param name="LogConditionPageModel"></param>
        /// <param name="TotalCount"></param>
        /// <returns></returns>
        public List<BG_Logs> GetBGLogs(ParamaterModel<BG_Logs> LogConditionPageModel)
        {
            if (LogConditionPageModel == null || LogConditionPageModel.Model == null)
            {
                LogConditionPageModel = new ParamaterModel<BG_Logs>();
                LogConditionPageModel.num = 5;
                LogConditionPageModel.start = 0;
            }
            List<BG_Logs> BGLogsList = GetBGLogsFromDataBaseByParamaterModelCondition(LogConditionPageModel);
            return BGLogsList;
        }
        /// <summary>
        /// 通过条件获取系统日志信息
        /// </summary>
        /// <param name="count"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public List<BG_Logs> GetBGLogsFromDataBase(int count, int startIndex)
        {
            try
            {
                string sqlStr = $@"{BaseRepository().SelectAll<BG_Logs>()} Order by LogDataTime desc limit {count} offset {startIndex}";

                return DataTableToObject<BG_Logs>(BaseRepository().QueryDataTable(sqlStr));
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 通过条件获取系统日志信息
        /// </summary>
        /// <param name="count"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public List<BG_Logs> GetBGLogsFromDataBaseByParamaterModelCondition(ParamaterModel<BG_Logs> paramaterModelCondition)
        {
            try
            {
                string sqlStr = $@"{BaseRepository().SelectAllAccordSearchConditionForPageIndex<BG_Logs>(paramaterModelCondition.num,paramaterModelCondition.start, paramaterModelCondition.Model)}";

                return DataTableToObject<BG_Logs>(BaseRepository().QueryDataTable(sqlStr));
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public int GetAllConut(BG_Logs bglogs)
        {
            try
            {
                string sqlStr = BaseRepository().SelectAllAccordSearchCondition<BG_Logs>(bglogs);

                return Convert.ToInt32(BaseRepository().QueryDataTable(sqlStr).Rows[0][0]);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public bool InsertLog(BG_Logs bglogModel)
        {
            try
            {
                string sqlStr = BaseRepository().InsertSpliteSqlByObject<BG_Logs>(bglogModel);
                return BaseRepository().ExecuteNonQuerySQL(sqlStr) > 0;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public bool DeleteLog(BG_Logs bglogModel)
        {
            try
            {
                string SqlStr = $@"delete from BG_Log ";
                if (bglogModel != null)
                {
                    SqlStr += $@" where LogId = '{bglogModel.LogId}' ";
                }
                return BaseRepository().ExecuteNonQuerySQL(SqlStr) > 0;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}
