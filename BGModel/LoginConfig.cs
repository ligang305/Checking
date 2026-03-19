using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGModel
{


    public class UserConfig
    {
        [BaseAttribute(Name = "UserName", Description = "用户名")]
        public string UserName { get; set; }
        [BaseAttribute(Name = "PassWord", Description = "密码")]
        public string PassWord { get; set; }
        [BaseAttribute(Name = "IsDefault", Description = "是否默认账号")]
        public string IsDefault { get; set; }
    }

  



    /// <summary>
    /// 登录用户配置类
    /// </summary>
    public class LoginUserConfig
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [BaseAttribute(Name = "USERNAME", Description = "用户账号")]
        public string  User { get; set; }
        /// <summary>
        /// 用户名密码
        /// </summary>
        [BaseAttribute(Name = "PASSWORD", Description = "密码")]
        public string  Password { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        [BaseAttribute(Name = "ROLE", Description = "所属角色")]
        public string  Role { get; set; }
    }
    /// <summary>
    /// 登录用户角色配置类
    /// </summary>
    public class LoginRoleConfig
    {
        /// <summary>
        /// 角色和用户配置类角色相对应
        /// </summary>
        [BaseAttribute(Name = "ROLE", Description = "所属角色")]
        public string Role { get; set; }

        private string _UserAuthorityStr { get; set; }
        /// <summary>
        /// 用户角色的权限字符串
        /// </summary>
        [BaseAttribute(Name = "USERAUTHORITYSTR", Description = "所属角色")]
        public string UserAuthorityStr
        {
            get {
                return _UserAuthorityStr;
            }
            set {
                _UserAuthorityStr = value;
                UserAuthoritys =
                    _UserAuthorityStr.Split(',').ToList();
                UserAuthoritys.ForEach(q => { q.TrimEnd(); q.TrimStart(); });
            }
        }
        /// <summary>
        /// 权限列表
        /// </summary>
        public List<string> UserAuthoritys
        {
            get;set;
        } 
    }
}
