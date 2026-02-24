using BG_Services;
using BG_WorkFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG_WebServices
{

    public class TaskInfo
    {
        public string TaskId { get; set; }
        public string FileInfos { get; set; }
    }


    public class RecvMessage
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
    }




    public class LoginRole
    {
        public string realName { get; set; }

        public string roleCode { get; set; }
        public string accessToken { get; set; }
        public string id { get; set; }
        public string userkey { get; set; }
        public string key { get; set; }
        public string userName { get; set; }
    }

    public class BaseClientType
    {
        public string Code { get; set; }
        public string Message { get; set; }
    }
    public class BaseRequestClientType
    {
        public string SC { get; set; } = ConfigServices.GetInstance().localConfigModel.IsAES?"1":"0";
        public string OrderType { get; set; }

        public string ClientType { get; set; } = "06";
    }
    public class BaseRequestClientTypeWithToken: BaseRequestClientType
    {
        public string Token { get; set; }
    }
    /// <summary>
    /// 获取通道号
    /// </summary>
    public class ChannelNoReceive : BaseClientType
    {
        public string Data { get; set; }
        public ChannelModel cm { get; set; }
    }
    public class ChannelModel
    {
        public string CheckAreaId { get; set; }

        public string ChannelName { get; set; }

        public string DeviceId { get; set; }

        public string CheckAreaName { get; set; }

        public string ChannelId { get; set; }

        public string DeviceName { get; set; }
    }


    public class LoginHttpModel : BaseRequestClientType
    {
        public string Data { get; set; }
    }

    public class LoginServiceModel
    {
        public string UserName { get; set; }

        public string UserPassword { get; set; }

    }

    public class LoginServiceModelWithLicense:LoginServiceModel
    {
        public string License { get; set; }
    }

    public class ChannelNo
    {
        public string DeviceNo { get; set; }
    }

 
    public class Instruction
    {
        public string deviceCode { get; set; }
    }


    public class BoostResetResetReposeModel
    {
        public string message { get; set; }
        public string data { get; set; }
        public string code { get; set; }
    }


    public class PathInfo
    {
        public string pictureNo { get; set; }
        public string fileType { get; set; }
        public string path { get; set; }

    }
    public class SubmitCheckInDataInfo
    {
        public string TaskId { get; set; }
        public List<PathInfo> Files = new List<PathInfo>();
    }

    public class RequestModel : BaseRequestClientTypeWithToken
    {

        public string Data { get; set; }
    }
    /// <summary>
    /// 上传到Web的异常模型
    /// </summary>
    public class WebExceptionModel
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public string TaskId { get; set; }
        /// <summary>
        /// 异常信息
        /// </summary>
        public string Message { get; set; }
    }

    //public class BoostResetHttpModel : BaseRequestClientTypeWithToken
    //{
    //    public string Data { get; set; }
    //}

    //public class SubmitReviewInfo : BaseRequestClientTypeWithToken
    //{
    //    public string Data { get; set; }
    //}
    ///// <summary>
    ///// 请求FTP参数的实体
    ///// </summary>
    //public class FTPHTTPData : BaseRequestClientTypeWithToken
    //{
    //    public string Data { get; set; }
    //}


    public class ResposeModel:BaseClientType
    {
        public Object Data { get; set; }
    }
    //public class RecvMessageDatanull : BaseClientType
    //{
    //    public object Data { get; set; }
    //}
    //public class LoginReceive : BaseClientType
    //{
    //    public string Data { get; set; }
    //}


    public class  FTPResposeObject
    {
        public string FTP_USERNAME { get; set; }
        public string FTP_IP { get; set; }
        public string FTP_PASSWORD { get; set; }

        public string FTP_PORT { get; set; }
    }


    public class NullClass
    {

    }
}
