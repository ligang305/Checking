using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGModel
{
    [BaseAttribute(Name = "BG_LICENSE")]
    public class License: BaseNotifyPropertyChanged
    {
        public string _License = string.Empty;
        public string _LicenseEffectTime = string.Empty;
        [BaseAttribute(Name = "LICENSE_CODE", Description = "授权码")]
        public string LICENSE_CODE
        {
            get
            {
                return _License;
            }
            set
            {
                _License = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("LICENSE_CODE"));
            }
        }
        [BaseAttribute(Name = "LICENSE_EFFECTTIME", Description = "授权码日期")]
        public string LICENSE_EFFECTTIME
        {
            get
            {
                return _LicenseEffectTime;
            }
            set
            {
                _LicenseEffectTime = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("LICENSE_EFFECTTIME"));
            }
        }
    }

    [BaseAttribute(Name = "BG_LOGINMODEL")]
    public class LoginModelObject: BaseNotifyPropertyChanged
    {
        public string _LoginUserName = string.Empty;
        public string _LoginPassWord = string.Empty;
        public string _LoginStatus = string.Empty;
        public string _UpdateTime = string.Empty;

        [BaseAttribute(Name = "LOGIN_USERNAME", Description = "用户名",IsUniqueKey =true)]
        public string LOGIN_USERNAME
        {
            get
            {
                return _LoginUserName;
            }
            set
            {
                _LoginUserName = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("LOGIN_USERNAME"));
            }
        }


        [BaseAttribute(Name = "LOGIN_PASSWORD", Description = "登录密码")]
        public string LOGIN_PASSWORD
        {
            get
            {
                return _LoginPassWord;
            }
            set
            {
                _LoginPassWord = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("LOGIN_PASSWORD"));
            }
        }

        [BaseAttribute(Name = "LOGIN_UPDATETIME", Description = "最后一次登陆时间")]
        public string LOGIN_UPDATETIME
        {
            get
            {
                return _UpdateTime;
            }
            set
            {
                _UpdateTime = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("LOGIN_UPDATETIME"));
            }
        }

        [BaseAttribute(Name = "LOGIN_STATUS", Description = "记录是否记录密码，IsRemember :1;UnRemember :0")]
        public string LOGIN_STATUS
        {
            get
            {
                return _LoginStatus;
            }
            set
            {
                _LoginStatus = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("LOGIN_STATUS"));
            }
        }
    }
}
