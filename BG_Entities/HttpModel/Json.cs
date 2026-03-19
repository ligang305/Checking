using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG_Entities
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
        public string SC { get; set; } =  "0";
        public string OrderType { get; set; }

        public string ClientType { get; set; } = "06";
    }
    public class BaseRequestClientTypeWithToken : BaseRequestClientType
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

    public class LoginServiceModelWithLicense : LoginServiceModel
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
        public string ViewNo { get; set; } 
        public string pictureNo { get; set; }
        public string fileType { get; set; }
        public string path { get; set; }

    }
    public class SubmitCheckInDataInfo
    {
        public string TaskId { get; set; }
        public string ImageInfo { get; set; }
         
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

    /// <summary>
    /// 将RPM上传到后台信息的实体对象
    /// </summary>
    public class RPMModel
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public string taskid { get; set; }
        /// <summary>
        /// RPM报警信息
        /// </summary>
        public string passState { get; set; }
    }


    public class ResposeModel : BaseClientType
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


    public class FTPResposeObject
    {
        public string FTP_USERNAME { get; set; }
        public string FTP_IP { get; set; }
        public string FTP_PASSWORD { get; set; }

        public string FTP_PORT { get; set; }
    }


    public class ServerSocketBodyData
    {
        /// <summary>
        /// 数据格式
        /// </summary>
        public string ClientCode { get; set; }
    }

    public class LocalParamaterServerSocketBodyData:ChannelNo
    {
        public string Parameters;
    }
    public class LocalParamaterServerHttpBodyData : ChannelNo
    {
        public string Config;
    }

    public class ServerSocketTaskInfo
    {
        public string Path { get; set; }

        public string TaskId { get; set; }
    }

    public class ServerSocketSettingTime
    {
        public string Time { get; set; }
    }


    public class ServerSocketBodyBaseWithoutToken
    {
        
        /// <summary>
        /// 协议编码
        /// </summary>
        public string OrderType { get; set; }
        /// <summary>
        /// 0未加密，1加密
        /// </summary>
        public string SC { get; set; }
        /// <summary>
        /// 0未加密，1加密
        /// </summary>
        public string MessageCode { get; set; }
        /// <summary>
        /// 消息体字符串
        /// </summary>
        public string Data { get; set; }
    }

    public class BarrierGateStatus
    {
        public string OpenOrClose;
    }

    public class ServerSocketBody: ServerSocketBodyBaseWithoutToken
    {
        /// <summary>
        /// 客户端登录时返回，验证接口有效性
        /// </summary>
        public string Token { get; set; }
    }

    public class FaildSocketBody
    {
        /// <summary>
        /// 失败的趟次任务Id
        /// </summary>
        public string TaskId { get; set; }
        /// <summary>
        /// 失败原因
        /// </summary>
        public string Reason { get; set; }
    }

    public class PathInfoBySocket
    {
        public string sequence { get; set; }
        public string fileType { get; set; }
        public string path { get; set; }

    }
    public class SubmitCheckInDataInfoBySocket
    {
        public string DeviceNo { get; set; }
        public string TaskId { get; set; }

        public string ImageInfo { get; set; }

        public int isLastCarriage { get; set; }

        public List<PathInfoBySocket> Files = new List<PathInfoBySocket>();
    }
    /// <summary>
    /// 带Data的消息
    /// </summary>
    public class ServerSocketBodyWithData: ServerSocketBodyData
    {
        /// <summary>
        /// 消息体字符串
        /// </summary>
        public string Data { get; set; }
    }
    public class NullClass
    {

    }
}
