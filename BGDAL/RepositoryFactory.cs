using BG_Entities;
using BGModel;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BGDAL
{
    public class RepositoryFactory<T>  where T : class, new()
    {
        public Repository BaseRepository()
        {
            IBaseDal database = null;
            string dbConnectionString = SystemDirectoryConfig.GetInstance().GetSystemDataBase();
            string dbType = DataBaseType.SqlLite;
            //TODO 这里需要配置数据库数据参数
            //TODO 和默认的数据库类型
            switch (dbType)
            {
                case "SqlLite": 
                    database = new SqlLiteDB<T>(dbConnectionString);
                    //CommonDeleget.WriteLogAction($@"database :{dbConnectionString}", LogType.NormalLog);
                    break;
                default:
                    throw new Exception("未找到数据库配置");
            }
            return new Repository(database);
        }
        public List<M> DataTableToObject<M>(DataTable WaitTrans) where M : class, new()
        {
            List<M> TList = new List<M>();    
            //获取需要转换的实体类的全部属性
            var Propties = typeof(M).GetProperties();
            M model = default(M);
            foreach (DataRow DataRow in WaitTrans.Rows)
            {
                model = Activator.CreateInstance<M>();
                //循环DataTable的列
                foreach (var singleColumn in WaitTrans.Columns)
                {
                    var currentColunm = singleColumn as DataColumn;
                    var ColumnName = currentColunm.ColumnName;
                    object drValue = DataRow[ColumnName];
                    var Props = model.GetType().GetProperties();

                    foreach (PropertyInfo property in Props)
                    {
                        var CustomAttribute = Attribute.GetCustomAttribute(property, typeof(BaseAttribute)) as BaseAttribute;
                        if (CustomAttribute == null) continue;
                        if(CustomAttribute.Name == ColumnName)
                        {
                            if (property != null && property.CanWrite && (drValue != null && !Convert.IsDBNull(drValue)))
                            {
                                property.SetValue(model, drValue, null);
                                continue;
                            }
                        }
                    }
                }
                TList.Add(model);
            }

            return TList;
        }
    }
}
