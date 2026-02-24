using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BG_WorkFlow;
using CMW.Common.Utilities;

namespace BG_Services
{
    public class UploadRFIDServices : HttpService<UploadRFIDServices>, IWebServices
    {
        string RequestUrl = $@"{ConfigServices.GetInstance().localConfigModel.Server}/CentralizedReview/reviewInterfaceController.do?CheckInOut";

        public string GetWebServicesName()
        {
            return "RFID";
        }

        public string UpdataRFIDServices(string Data)
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
            BGLogs.Log.GetDistance().WriteInfoLogs($"{GetWebServicesName()}:{Data}");
            return UpdataRFIDServices(Data);
        }
        public string GetDataFromWeb()
        {
            return string.Empty;
        }
    }
}
