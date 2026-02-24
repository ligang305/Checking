using BG_WorkFlow;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG_Services
{
    public class RequestBoostingResetServices : HttpService<RequestBoostingResetServices>, IWebServices
    {
        string RequestUrl = $@"{ConfigServices.GetInstance().localConfigModel.Server}/CentralizedReview/reviewInterfaceController.do?MachineCheck";

        public string GetWebServicesName()
        {
            return "RequestBoostingResetServices ";
        }

        public string UploadData(string Data)
        {
            return GetDataFromWeb(Data);
        }
        public string GetDataFromWeb(string Data)
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
    }
}
