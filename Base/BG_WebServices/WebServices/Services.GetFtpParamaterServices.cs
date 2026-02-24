using BG_WorkFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG_Services
{
    public class GetFtpParamaterServices : HttpService<GetFtpParamaterServices>, IWebServices
    {
        string RequestUrl = $@"{ConfigServices.GetInstance().localConfigModel.Server}/CentralizedReview/reviewInterfaceController.do?review";

        public string GetFTPMsgFromWeb(string Data)
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

        public string GetWebServicesName()
        {
            return "获取FTP数据";
        }

        public string UploadData(string Data)
        {
            return GetFTPMsgFromWeb(Data);
        }
    }
}
