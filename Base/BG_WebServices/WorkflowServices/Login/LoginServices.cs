using BG_Entities;
using BG_WorkFlow;
using BGModel;
using CMW.Common.Utilities;
using System.Diagnostics;

namespace BG_Services
{
    public class LoginDal : BaseInstance<LoginDal>
    {
        public ResposeModel Login(LoginModel lm)
        {
            LoginHttpModel sri = new LoginHttpModel();
            sri.OrderType = "Login";
            sri.Data = CommonFunc.AesEncrypt(CommonFunc.ObjectToJson(new LoginServiceModelWithLicense() { UserName = lm?.UserName, UserPassword = lm?.Password, License = lm?.License }), ConfigServices.GetInstance().localConfigModel.IsAES, "begood-123456789");//, License = lm?.License

            ResposeModel rm = LoginToServices(sri);
            if (rm.Code == "1")
            {
                CommonDeleget.WriteLogAction("Login Success",LogType.NormalLog);
            }
            return rm;
        }


        public ChannelNoReceive GetChannel(string ChannelId)
        {
            RequestModel chm = new RequestModel();
            chm.OrderType = UploadImageOrderType.GetChannelInfoByDeviceId;
            chm.Token = ConfigServices.GetInstance().localConfigModel.Login.sccessToken;
            chm.Data = CommonFunc.AesEncrypt(CommonFunc.ObjectToJson (new ChannelNo() { DeviceNo = ChannelId }), ConfigServices.GetInstance().localConfigModel.IsAES);
            ChannelNoReceive cnr = GetChannelNoAccrodChannelId(chm);
            if (cnr.Code == "1")
            {
                CommonDeleget.WriteLogAction($"GetChannelNoAccrodChannelId Success！",LogType.NormalLog);
            }
            else
            {
                CommonDeleget.WriteLogAction($"GetChannelNoAccrodChannelId Error！", LogType.NormalLog);
            }
            return cnr;
        }

        /// <summary>
        /// 获取ID之后把图片上传
        /// </summary>
        /// <param name="ihp"></param>
        /// <returns></returns>
        private ResposeModel LoginToServices(LoginHttpModel ihp)
        {
            Debug.WriteLine("-------------------------ganggang_upload_img29-----------------------");
            string PostData = CommonFunc.ObjectToJson<LoginHttpModel>(ihp);
            CommonDeleget.WriteLogAction($"LoginToServices : {PostData} \n", LogType.NormalLog);
            var ResposeStr =
                UploadWebServiceControl.GetInstance().CreateWebServicesControl(LoginService.GetInstance()).UploadData(PostData);
            //LoginService.GetInstance().LoginSystem(PostData);

            ResposeModel rm = CommonFunc.JsonToObject<ResposeModel>(ResposeStr);
            return rm;
        }

        /// <summary>
        /// 传入通道ID 获取通道号
        /// </summary>
        /// <param name="ihp"></param>
        /// <returns></returns>
        private ChannelNoReceive GetChannelNoAccrodChannelId(RequestModel chm)
        {
            string PostData = CommonFunc.ObjectToJson<RequestModel>(chm);
            CommonDeleget.WriteLogAction($"GetChannelNoAccrodChannelId : {PostData} \n", LogType.NormalLog);
            var ResposeStr = UploadWebServiceControl.GetInstance().CreateWebServicesControl(GetChannelServices.GetInstance()).UploadData(PostData);
            // GetChannelServices.GetInstance().GetChannelNo(PostData);
            ChannelNoReceive rm = CommonFunc.JsonToObject<ChannelNoReceive>(ResposeStr);
            var NormalObject = CommonFunc.JsonToObject<ChannelModel>(CommonFunc.AesDecrypt(rm.Data, ConfigServices.GetInstance().localConfigModel.IsAES));
            rm.cm = NormalObject;
            return rm;
        }

        private LoginModel GetRembermerLastUserName()
        {
            LoginModel lm = new LoginModel();



            return lm;
        }
    }
}