using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGDAL
{
    public interface IBaseDal
    {
        string InsertSpliteSqlByObject<T>(T TObject);
        string UpdateSqlByObject<T>(T TObject);
        bool IsExistTabel<T>();
        bool CreateTabel<T>();
        string SelectAllSqlForPageIndex<T>(int PageIndex, int Current);
        bool IsExistIndex<T>(string IndexName);
        bool CreateIndex<T>(string ViewName, string TabelName, string ColumnName);
        string SelectAll<T>();
        string SelectCount<T>();
        DataTable QueryDataTable(string sql);
        int ExecuteNonQuerySQL(string sql);
        DbTransaction GetTransaction();
        DbCommand GetCommand();
        bool ExecuteNonQueryByTranscation(string sql);
        string SelectAllAccordSearchCondition<T>(T TObject);
        string SelectAllAccordSearchConditionForPageIndex<M>(int PageIndex, int Current, M TObject);
    }
}
