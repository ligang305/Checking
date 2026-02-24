using BG_WorkFlow;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG_Services
{
    public class RPMServices : HttpService<RPMServices>, IWebServices
    {
        string RequestUrl = $@"{ConfigServices.GetInstance().localConfigModel.Server}/CentralizedReview/reviewInterfaceController.do?CheckInOut";
        public string GetWebServicesName()
        {
            return "RPM Services";
        }

        public string UploadData(string Data)
        {
            return base.Http(RequestUrl, Data, Encoding.UTF8);
        }
    }
}
