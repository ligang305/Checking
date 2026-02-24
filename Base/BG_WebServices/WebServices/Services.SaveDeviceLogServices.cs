using BG_WorkFlow;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BG_Services
{
    public class SaveDeviceLogServices : HttpService<SaveDeviceLogServices>, IWebServices
    {
        string RequestUrl = $@"{ConfigServices.GetInstance().localConfigModel.LogsServer}/api/reviewDeviceLog/saveDeviceLog";
        //{ConfigServices.GetInstance().localConfigModel.Server}

        public string GetWebServicesName()
        {
            return "SaveDeviceLog Services";
        }

        public string UploadData(string Data)
        {
            return base.Http(RequestUrl, Data, Encoding.UTF8);
        }
    }
}