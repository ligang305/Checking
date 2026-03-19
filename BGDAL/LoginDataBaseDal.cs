using BGModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGDAL
{
    public class LoginDataBaseDal : RepositoryFactory<LoginModelObject>
    {
        public LoginDataBaseDal()
        {

        }
        public List<LoginModelObject> GetLoginUserFromDataBase()
        {
            try
            {
                string sqlStr = string.Format($@"{BaseRepository().SelectAll<LoginModelObject>()} Order by UpdateTime desc ");
                return DataTableToObject<LoginModelObject>(BaseRepository().QueryDataTable(sqlStr));
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }


        public bool GetLoginUserByLoginName(LoginModelObject loginObject)
        {
            try
            {
                string sqlStr = $@"{BaseRepository().SelectAll<LoginModelObject>()} where LOGIN_USERNAME = '{loginObject.LOGIN_USERNAME}' ";

                return BaseRepository().QueryDataTable(sqlStr).Rows.Count > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public LoginModelObject GetLoginUserByRememberStatus()
        {
            try
            {
                string sqlStr = $@"{BaseRepository().SelectAll<LoginModelObject>()} where LOGIN_STATUS = '{1}' ";
                var loginList = DataTableToObject<LoginModelObject>(BaseRepository().QueryDataTable(sqlStr));
                return loginList.Count>0? loginList[0]:null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool DeleteLoginUser(LoginModelObject loginObject)
        {
            try
            {
                string sqlStr = BaseRepository().InsertSpliteSqlByObject<LoginModelObject>(loginObject);
                return BaseRepository().ExecuteNonQuerySQL(sqlStr) > 0;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }


        public bool InsertOrUpdateLoginModelObject(LoginModelObject loginObject)
        {
            try
            {
                string OtherStr = $@"UPDATE BG_LOGINMODEL Set LOGIN_STATUS = '0'";
                BaseRepository().ExecuteNonQueryByTranscation(OtherStr);
                string sqlStr = string.Empty;
                if (GetLoginUserByLoginName(loginObject))
                {
                    sqlStr = BaseRepository().UpdateSqlByObject<LoginModelObject>(loginObject);
                }
                else
                {
                    sqlStr = BaseRepository().InsertSpliteSqlByObject<LoginModelObject>(loginObject);
                }
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
