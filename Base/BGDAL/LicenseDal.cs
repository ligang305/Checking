using BGModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGDAL
{
    public class LicenseDal : RepositoryFactory<License>
    {
        public List<License> QueryLicense()
        {
            try
            {
                string sqlStr = BaseRepository().SelectAll<License>();
                return DataTableToObject<License>(BaseRepository().QueryDataTable(sqlStr));
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public bool InsertLicense(License loginObject)
        {
            try
            {
                string sqlStr =  BaseRepository().InsertSpliteSqlByObject<License>(loginObject);
                BaseRepository().ExecuteNonQuerySQL(sqlStr);
                return true;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateLicense(License loginObject)
        {
            try
            {
                string sqlStr = BaseRepository().UpdateSqlByObject<License>(loginObject);
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
