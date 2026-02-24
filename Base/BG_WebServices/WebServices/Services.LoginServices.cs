using BG_Entities;
using BG_WorkFlow;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG_Services
{
    public class LoginService : HttpService<LoginService>, IWebServices
    {
        string RequestUrl = $@"{ConfigServices.GetInstance().localConfigModel.Server}/CentralizedReview/reviewInterfaceController.do?review";

        public string GetWebServicesName()
        {
            return "Login";
        }

        public string LoginSystem(string Data)
        {
            try
            {
                return base.Http(RequestUrl, Data, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string UploadData(string Data)
        {
            CommonDeleget.WriteLogAction($"User Interface{GetWebServicesName()}:{Data}", LogType.Services, true);
            return LoginSystem(Data);
        }
        public string GetDataFromWeb()
        {
            return string.Empty;
        }
    }
}
