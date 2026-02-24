using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BG_WorkFlow;
using CMW.Common.Utilities;

namespace BG_Services
{
    public class GetChannelServices: HttpService<GetChannelServices>, IWebServices
    {
        string RequestUrl = $@"{ConfigServices.GetInstance().localConfigModel.Server}/CentralizedReview/reviewInterfaceController.do?CheckInOut";
        public string GetChannelNo(string Data)
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
            return "GetChannel";
        }

        public string UploadData(string Data)
        {
            return GetChannelNo(Data);
        }
        public string GetDataFromWeb()
        {
            return string.Empty;
        }
    }
}
