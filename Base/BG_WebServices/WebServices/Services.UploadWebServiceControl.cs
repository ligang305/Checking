using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BG_Entities;

namespace BG_Services
{
    public class UploadWebServiceControl :BaseInstance<UploadWebServiceControl>,IWebServices
    {
        private IWebServices IUploadService;

        /// <summary>
        /// 上传接口控制类
        /// </summary>
        /// <param name="UploadService"></param>
        public UploadWebServiceControl()
        {
            
        }

        public UploadWebServiceControl CreateWebServicesControl(IWebServices TWebServices)
        {
            IUploadService = TWebServices;
            return this;
        }

        /// <summary>
        /// 获取当前调用的接口名
        /// </summary>
        /// <returns></returns>
        public string GetWebServicesName()
        {
           return IUploadService.GetWebServicesName();
        }
        /// <summary>
        /// 调用接口
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public string UploadData(string Data)
        {
            //CommonDeleget.WriteLogAction($"User Interface{GetWebServicesName()}:{Data}");
            return IUploadService.UploadData(Data);
        }
    }
}
