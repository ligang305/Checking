using BG.DataBase;
using BGModel;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BGDAL
{
    public class SqlLiteDB<T> : Sqlite, IBaseDal where T : class, new()
    {
        public SqlLiteDB(string sqlLiteString)
            : base(sqlLiteString)
        {
            if (!IsExistTabel<T>())
            {
                CreateTabel<T>();
            }
        }
        /// <summary>
        /// 获取事务对象
        /// </summary>
        /// <returns></returns>
        public DbTransaction GetTransaction()
        {
            return base.GetSQLiteTransaction();
        }
        /// <summary>
        /// 获取事务对象命令
        /// </summary>
        /// <returns></returns>
        public DbCommand GetCommand()
        {
            return base.GetSQLiteCommand();
        }
        public bool ExecuteNonQueryByTranscation(string sql)
        {
            try
            {
                var SqlLiteTransaction = GetTransaction();
                var SQLiteCommand = GetCommand();
                SQLiteCommand.Transaction = SqlLiteTransaction;
                base.ExecuteNonQueryByTranscation(SQLiteCommand as SQLiteCommand, sql);
                SqlLiteTransaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string SelectAll<T>()
        {
            var TType = typeof(T);
            var Props = TType.GetProperties();
            string ClassAttributeName = (TType.GetCustomAttributes(typeof(BaseAttribute), true)[0] as BaseAttribute).Name;
            string Sql = string.Format($@"select * from {ClassAttributeName}");
            return Sql;
        }
        public string SelectCount<T>()
        {
            var TType = typeof(T);
            var Props = TType.GetProperties();
            string ClassAttributeName = (TType.GetCustomAttributes(typeof(BaseAttribute), true)[0] as BaseAttribute).Name;
            string Sql = string.Format($@"select Count(1) from {ClassAttributeName}");
            return Sql;
        }
        public string InsertSpliteSqlByObject<M>(M TObject)
        {
            var TType = typeof(M);
            var Props = TType.GetProperties();
            string ClassAttributeName = (TType.GetCustomAttributes(typeof(BaseAttribute), true)[0] as BaseAttribute).Name;
            string Sql = string.Format("insert into {0} (", ClassAttributeName);

            foreach (PropertyInfo property in Props)
            {
                var CustomAttribute = Attribute.GetCustomAttribute(property, typeof(BaseAttribute)) as BaseAttribute;
                if (CustomAttribute == null) continue;
                string ColumnName = CustomAttribute.Name;
                if (string.IsNullOrEmpty(CustomAttribute.Name)) ColumnName = property.Name;
                if (CustomAttribute.IsInsertDB)
                {
                    Sql += ColumnName + ",";
                }
            }
            Sql = Sql.Substring(0, Sql.Length - 1) + ") Values (";
            foreach (var property in Props)
            {
                var CustomAttribute = Attribute.GetCustomAttribute(property, typeof(BaseAttribute)) as BaseAttribute;
                if (CustomAttribute == null) continue;
                string ColumnName = CustomAttribute.Name;
                if (string.IsNullOrEmpty(CustomAttribute.Name)) ColumnName = property.Name;
                if (CustomAttribute.IsInsertDB)
                {
                    string ItemValue = property.GetValue(TObject, null) == null ? string.Empty : property.GetValue(TObject, null).ToString();
                    Sql += "'" + property.GetValue(TObject, null) + "',";
                }
            }
            Sql = Sql.Substring(0, Sql.Length - 1) + ")";
            return Sql;
        }
        public string UpdateSqlByObject<M>(M TObject)
        {
            var TType = typeof(M);
            var Props = TType.GetProperties();
            string ClassAttributeName = (TType.GetCustomAttributes(typeof(BaseAttribute), true)[0] as BaseAttribute).Name;
            string Sql = string.Format("update {0} set ", ClassAttributeName);
            BaseAttribute Conditionba = null;
            PropertyInfo Conditionpi = null;
            foreach (var property in Props)
            {
                var CustomAttribute = Attribute.GetCustomAttribute(property, typeof(BaseAttribute)) as BaseAttribute;
                if (CustomAttribute == null) continue;
                string ColumnName = CustomAttribute.Name;
                if (string.IsNullOrEmpty(CustomAttribute.Name)) ColumnName = property.Name;
                if (CustomAttribute.IsInsertDB)
                {
                    string ItemValue = property.GetValue(TObject, null) == null ? "" : property.GetValue(TObject, null).ToString();
                    Sql += string.Format("{0} = '{1}',", ColumnName, ItemValue);
                }
                if (CustomAttribute.IsUniqueKey)
                {
                    Conditionpi = property;
                    Conditionba = CustomAttribute;
                } 
            }
            Sql = Sql.Substring(0, Sql.Length - 1);
            if (Conditionpi != null)
            {
                string ItemValue = Conditionpi.GetValue(TObject, null) == null ? "" : Conditionpi.GetValue(TObject, null).ToString();
                if (!string.IsNullOrEmpty(ItemValue))
                {
                    Sql += $@" where {Conditionpi.Name} = '{ItemValue}' ";
                }
            }
            return Sql;
        }
        public bool IsExistTabel<M>()
        {
            var TType = typeof(M);
            var Props = TType.GetProperties();
            string ClassAttributeName = (TType.GetCustomAttributes(typeof(BaseAttribute), true)[0] as BaseAttribute)?.Name;
            string IsExistTabel = string.Format("select * from sqlite_master where [type] = 'table' and [name] = '{0}' ", ClassAttributeName);
            System.Data.DataTable DataTable = base.QueryDataTable(IsExistTabel);
            return DataTable.Rows.Count > 0;
        }
        public bool CreateTabel<M>()
        {
            var TType = typeof(M);
            var Props = TType.GetProperties();
            string ClassAttributeName = (TType.GetCustomAttributes(typeof(BaseAttribute), true)[0] as BaseAttribute)?.Name;
            string CreateTabelSql = string.Format("Create Table {0}( ", ClassAttributeName);
            foreach (PropertyInfo property in Props)
            {
                var CustomAttribute = Attribute.GetCustomAttribute(property, typeof(BaseAttribute)) as BaseAttribute;
                if (CustomAttribute == null) continue;
                if (CustomAttribute.IsInsertDB)
                {
                    string ColumnName = CustomAttribute.Name;
                    if (string.IsNullOrEmpty(CustomAttribute.Name)) ColumnName = property.Name;
                    CreateTabelSql += CustomAttribute.Name + " Text ";
                    if (CustomAttribute.IsPrimaryKey)
                    {
                        CreateTabelSql += "PRIMARY KEY";
                    }
                    else
                    {
                        CreateTabelSql += ",";
                    }
                }
            }
            CreateTabelSql = CreateTabelSql.Substring(0, CreateTabelSql.Length - 1) + ");";
            return base.ExecuteNonQuerySQL(CreateTabelSql) > 0;
        }
        public string SelectAllSqlForPageIndex<M>(int PageIndex, int Current)
        {
            var TType = typeof(M);
            var Props = TType.GetProperties();
            string ClassAttributeName = (TType.GetCustomAttributes(typeof(BaseAttribute), true)[0] as BaseAttribute)?.Name;
            string sql = string.Format("select * from {0} limit {1} offset {1}*{2}", ClassAttributeName, PageIndex, Current);
            return sql;
        }
        public bool IsExistIndex<M>(string IndexName)
        {
            string SqlText = string.Format("SELECT count(*) as Total FROM sqlite_master WHERE type = 'index' AND name = '{0}' ", IndexName);
            System.Data.DataTable DataTable = base.QueryDataTable(SqlText);
            if (DataTable.Rows[0].ToString() == "0")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool CreateIndex<M>(string ViewName, string TabelName, string ColumnName)
        {
            string SqlText = string.Format(@"CREATE INDEX '{0}' ON '{1}' ('{2}')", ViewName, TabelName, ColumnName);
            return base.ExecuteNonQuerySQL(SqlText) > 0;
        }
        public new int ExecuteNonQuerySQL(string sql)
        {
            return base.ExecuteNonQuerySQL(sql);
        }
        public string SelectAllAccordSearchCondition<W>(W TObject)
        {
            var TType = typeof(T);
            var Props = TType.GetProperties();
            string ClassAttributeName = (TType.GetCustomAttributes(typeof(BaseAttribute), true)[0] as BaseAttribute).Name;
            string Sql = string.Format("select count(1) from {0}", ClassAttributeName);
            string ConditionStr = string.Empty;
            string DataTimeStr = string.Empty;
            foreach (var property in Props)
            {
                string ItemValue = property.GetValue(TObject, null) == null ? "" : property.GetValue(TObject, null).ToString();
                var CustomAttribute = Attribute.GetCustomAttribute(property, typeof(BaseAttribute)) as BaseAttribute;
                var DataAttribute = Attribute.GetCustomAttribute(property, typeof(TimeAttribute)) as TimeAttribute;
                if (CustomAttribute == null) continue;
                if (DataAttribute != null)
                {
                    if (DataAttribute.IsStartTime)
                    {
                        DataTimeStr = $@" {CustomAttribute.Name} between '{ItemValue}' and ";
                    }
                    if (DataAttribute.IsEndTime)
                    {
                        DataTimeStr = $@" {DataTimeStr} '{ItemValue}' ";
                    }
                }

                if (CustomAttribute.IsSearch && !string.IsNullOrEmpty(ItemValue))
                {
                    ConditionStr += $"{CustomAttribute.Name} like '%{ItemValue}%' and ";
                }
            }
            Sql = Sql + (!string.IsNullOrEmpty(ConditionStr) ? $@"where {ConditionStr} {DataTimeStr} " : " ");
            return Sql;
        }
        public string SelectAllAccordSearchConditionForPageIndex<M>(int PageIndex, int Current,M TObject)
        {
            var TType = typeof(T);
            var Props = TType.GetProperties();
            string ClassAttributeName = (TType.GetCustomAttributes(typeof(BaseAttribute), true)[0] as BaseAttribute).Name;
            string Sql = string.Format("select * from {0}", ClassAttributeName);
            string ConditionStr = string.Empty;
            string DataTimeStr = string.Empty;
            foreach (var property in Props)
            {
                string ItemValue = property.GetValue(TObject, null) == null ? "" : property.GetValue(TObject, null).ToString();
                var CustomAttribute = Attribute.GetCustomAttribute(property, typeof(BaseAttribute)) as BaseAttribute;
                var DataAttribute = Attribute.GetCustomAttribute(property, typeof(TimeAttribute)) as TimeAttribute;
                if (CustomAttribute == null) continue;
                if (DataAttribute != null)
                {
                    if (DataAttribute.IsStartTime)
                    {
                        DataTimeStr = $@" {CustomAttribute.Name} between '{ItemValue}' and ";
                    }
                    if(DataAttribute.IsEndTime)
                    {
                        DataTimeStr = $@" {DataTimeStr} '{ItemValue}' ";
                    }
                }
                if (CustomAttribute.IsSearch && !string.IsNullOrEmpty(ItemValue))
                {
                    ConditionStr += $"{CustomAttribute.Name} like '%{ItemValue}%' and ";
                }
            }
            Sql = $@"{Sql} where {ConditionStr} {DataTimeStr} limit {PageIndex} offset {PageIndex}*{Current}";
            return Sql;
        }
    }
}
