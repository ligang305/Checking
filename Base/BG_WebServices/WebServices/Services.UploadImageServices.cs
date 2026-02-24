using BG_WorkFlow;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG_Services
{
    public class UploadImageServices : HttpService<UploadImageServices>
    {
        string RequestUrl = $@"{ConfigServices.GetInstance().localConfigModel.Server}/CentralizedReview/reviewInterfaceController.do?CheckInOut";
        public string GetTaskIdFromWeb(string Data)
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

        public string UploadImageToWeb(string Data)
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

        public string CancelImage(string EquipmentNo)
        {
            try
            {
                string Data = @"{""OrderType"":""Rescan"",""SC"":""0"", ""Data"":""{\""DeviceNo\"":\""" + EquipmentNo + @"\""}""}";
                //, EquipmentNo
                return base.Http(RequestUrl, Data, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
