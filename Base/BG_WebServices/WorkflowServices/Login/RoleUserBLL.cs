
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BGModel;
using BG_Entities;
using CMW.Common.Utilities;
using BG_WorkFlow;

namespace BG_Services
{
    public class RoleUserBLL
    {
        List<LoginUserConfig> LoginUserConfigs = new List<LoginUserConfig>();
        List<LoginRoleConfig> LoginRoleConfigs = new List<LoginRoleConfig>();
        XmlBaseDataDAL xmlObject = new XmlBaseDataDAL();
        LoginDal ld = new LoginDal();
        //角色用户配置类构造函数   
        public RoleUserBLL(ControlVersion cv)
        {
            LoginUserConfigs = xmlObject.GetObjects<LoginUserConfig>(SystemDirectoryConfig.GetInstance().
                GetUserConfig(cv), GlogbalBGModel.BG_USERS).ToList();
            LoginRoleConfigs = xmlObject.GetObjects<LoginRoleConfig>(SystemDirectoryConfig.GetInstance().
                GetRoleConfig(cv), GlogbalBGModel.BG_ROLES).ToList();
        }
        /// <summary>
        /// 异步获取通道号的方法
        /// </summary>
        /// <param name="ChannelId"></param>
        /// <returns></returns>
        public async Task<ChannelNoReceive> GetChannel(string ChannelId)
        {
            return await Task.Run(() => { return ld.GetChannel(ChannelId); });
        }

        public LoginStatus Login(LoginModel lm, ref List<string> Buttons)
        {
            ///Code: 1 成功， 2用户不存在，3密码错误， 99 异常
            var logReceive = ld.Login(lm);
            if (logReceive.Data == null) logReceive.Data = string.Empty;
            var loginObject = CommonFunc.JsonToObject<LoginRole>(CommonFunc.AesDecrypt(logReceive.Data?.ToString(), ConfigServices.GetInstance().localConfigModel.IsAES));
            if (logReceive ==null || logReceive.Code== "2")//如果用户不存在，返回用户不存在
                return LoginStatus.UnSuchUser;

            if (logReceive.Code == "3")//密码错误
                return LoginStatus.PasswordError;
            if (logReceive.Code == "90") //授权码过期
                return LoginStatus.AuthorizationCodeExpired;
            if (logReceive.Code == "99") //异常
                return LoginStatus.Faild;
            if (logReceive.Code == "4") //异常
                return LoginStatus.UnAuthorized;
            if (logReceive.Code == "7") //未授权
                return LoginStatus.InvalidAuthorizationCode;
            if (logReceive.Code == "1") //成功
            {
                loginObject.roleCode = loginObject.roleCode.Contains("jjAdmin") ? "jjAdmin" : loginObject.roleCode;
                var loginRole = LoginRoleConfigs.FirstOrDefault(q => q.Role == loginObject.roleCode);
                if (loginRole == null) //用户所属角色不存在，返回登录失败
                    return LoginStatus.Faild;


                ConfigServices.GetInstance().localConfigModel.Login.LoginMode = loginObject.realName;

                ConfigServices.GetInstance().localConfigModel.Login.LoginCode = loginObject.roleCode;

                ConfigServices.GetInstance().localConfigModel.Login.sccessToken = loginObject.accessToken;
                //获取可以操控的权限按钮列表
                Buttons = loginRole.UserAuthoritys;
                return LoginStatus.Success;
            }
            return LoginStatus.Faild;
        }

        public LoginStatus LoginResult(LoginModel lm,ref List<string> Buttons)
        {
            var loginModel = LoginUserConfigs.FirstOrDefault(q=>q.User == lm.UserName);
            if(loginModel == null) //如果用户不存在，返回用户不存在
                return LoginStatus.UnSuchUser;

            if (loginModel.Password != lm.Password) //密码错误，返回密码错误
                return LoginStatus.PasswordError;

            var loginRole = LoginRoleConfigs.FirstOrDefault(q => q.Role == loginModel.Role);
            if (loginRole == null) //用户所属角色不存在，返回登录失败
                return LoginStatus.Faild;


            ConfigServices.GetInstance().localConfigModel.Login.LoginMode = loginRole.Role.ToLower() == "administrator" ? "维护用户" : "一般用户";
            //获取可以操控的权限按钮列表
            Buttons = loginRole.UserAuthoritys;
            return LoginStatus.Success;
        }
    }
}
