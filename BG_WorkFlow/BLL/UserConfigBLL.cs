using BG_Entities;
using BGModel;
using CMW.Common.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace BG_WorkFlow
{
    public class UserConfigBLL
    {
        XmlBaseDataDAL xmlObject = new XmlBaseDataDAL();
        List<UserConfig> UserConfigList = new List<UserConfig>();


        public UserConfigBLL()
        {
            UserConfigList = xmlObject.GetObjects<UserConfig>(SystemDirectoryConfig.GetInstance().GetUserConfig(), GlogbalBGModel.BG_USERS).ToList();
        }
        /// <summary>
        /// 更新UserConfig配置文件
        /// </summary>
        /// <param name="lm"></param>
        public void AddOrRemove(LoginModel lm)
        {
            var UserConfig = UserConfigList.FirstOrDefault(q=>q.UserName == lm.UserName);
            if (UserConfig != null)
            {
                UserConfig.PassWord = lm.Password;
            }
            else
            {
                UserConfigList.Add(new UserConfig() { UserName = lm.UserName,PassWord = lm.Password,IsDefault = false.ToString()});
            }
            foreach (var item in UserConfigList)
            {
                xmlObject.SaveSingleData<UserConfig>(SystemDirectoryConfig.GetInstance().GetUserConfig(),item, GlogbalBGModel.BG_USER, item.UserName);
            }
        }
    }
}
