using BGModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGDAL
{
    public class ModulesDal : RepositoryFactory<ModulesConfig>
    {
        public ModulesDal()
        {
            if (!base.BaseRepository().IsExistIndex<ModulesConfig>("modulesId"))
                base.BaseRepository().CreateIndex<ModulesConfig>("modulesId", "BG_Modules", "modulesId");
        }
        public List<ModulesConfig> GetModulesConfig(string Version)
        {
            try
            {
                string sqlStr = string.Format($@"{BaseRepository().SelectAll<ModulesConfig>()}");
                if(!string.IsNullOrEmpty(Version))
                {
                    sqlStr += $@" where modulesBelongs = '{Version}' ";
                }
                return DataTableToObject<ModulesConfig>(BaseRepository().QueryDataTable(sqlStr));
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
      

        public bool InsertModules(ModulesConfig bglogModel)
        {
            try
            {
                string sqlStr = BaseRepository().InsertSpliteSqlByObject<ModulesConfig>(bglogModel);
                return BaseRepository().ExecuteNonQuerySQL(sqlStr) > 0;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public bool DeleteModulesConfig(ModulesConfig bglogModel)
        {
            try
            {
                string SqlStr = $@"delete from BG_Modules ";
                if (bglogModel != null)
                {
                    SqlStr += $@" where modulesId = '{bglogModel.modulesId}' ";
                }
                return BaseRepository().ExecuteNonQuerySQL(SqlStr) > 0;
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
        public bool UpdateModulesConfig(ModulesConfig modulesConfig)
        {
            try
            {
                string sqlStr = BaseRepository().UpdateSqlByObject<ModulesConfig>(modulesConfig);
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
