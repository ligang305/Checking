using BG_WorkFlow;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG_Services
{
    public class ParamaterServices : HttpService<ParamaterServices>, IWebServices
    {
        string RequestUrl = $@"{ConfigServices.GetInstance().localConfigModel.ParamaterServerPort}/api/ClientService/getDeviceConfig";
        public string GetWebServicesName()
        {
            return "ParamaterServices ";
        }

        public string UploadData(string Data)
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
