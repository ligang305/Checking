using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BG.DataBase;

namespace BGDAL
{
    public class Repository
    {
        private IBaseDal Ibd;
        public Repository(IBaseDal DataBaseDal)
        {
            Ibd = DataBaseDal;
        }
        public bool CreateIndex<T>(string ViewName, string TabelName, string ColumnName)
        {
            return Ibd.CreateIndex<T>(ViewName, TabelName, ColumnName);
        }
        public bool CreateTabel<T>()
        {
            return Ibd.CreateTabel<T>();
        }
        public string InsertSpliteSqlByObject<T>(T TObject)
        {
            return Ibd.InsertSpliteSqlByObject<T>(TObject);
        }
        public bool IsExistIndex<T>(string IndexName)
        {
            return Ibd.IsExistIndex<T>(IndexName);
        }
        public bool IsExistTabel<T>()
        {
            return Ibd.IsExistTabel<T>();
        }
        public string SelectAllAccordSearchCondition<W>(W TObject)
        {
            return Ibd.SelectAllAccordSearchCondition<W>(TObject);
        }
        public string SelectAllAccordSearchConditionForPageIndex<M>(int PageIndex, int Current, M TObject)
        {
            return Ibd.SelectAllAccordSearchConditionForPageIndex<M>(PageIndex, Current, TObject);
        }
        public string SelectAllSqlForPageIndex<T>(int PageIndex, int Current)
        {
            return Ibd.SelectAllSqlForPageIndex<T>(PageIndex, Current);
        }
        public string SelectAll<T>()
        {
            return Ibd.SelectAll<T>();
        }
        public string SelectCount<T>()
        {
            return Ibd.SelectCount<T>();
        }
        public string UpdateSqlByObject<T>(T TObject)
        {
            return Ibd.UpdateSqlByObject<T>(TObject);
        }
        public DataTable QueryDataTable(string sql)
        {
            return Ibd.QueryDataTable(sql);
        }
        public int ExecuteNonQuerySQL(string sql)
        {
            return Ibd.ExecuteNonQuerySQL(sql);
        }
        public DbTransaction GetTransaction()
        {
            return Ibd.GetTransaction();
        }
        public DbCommand GetCommand()
        {
            return Ibd.GetCommand();
        }
        public bool ExecuteNonQueryByTranscation(string sql)
        {
            return Ibd.ExecuteNonQueryByTranscation(sql);
        }
    }
}
