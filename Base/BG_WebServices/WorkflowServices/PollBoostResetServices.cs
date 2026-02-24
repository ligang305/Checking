using BG_Entities;
using BG_WorkFlow;
using BGModel;
using CMW.Common.Utilities;

namespace BG_Services
{
    /// <summary>
    /// 轮询复位的功能
    /// </summary>
    public class PollBoostResetServices:BaseInstance<PollBoostResetServices>
    {
        private RequestModel bsm = null;
        public PollBoostResetServices():base()
        {
            bsm = new RequestModel()
            {
                OrderType = UploadImageOrderType.GetInstructions,
                Token = ConfigServices.GetInstance().localConfigModel.Login.sccessToken,
                Data = CommonFunc.AesEncrypt(CommonFunc.ObjectToJson(new Instruction() { deviceCode = ConfigServices.GetInstance().localConfigModel.EquipmentNo }), ConfigServices.GetInstance().localConfigModel.IsAES,"begood-123456789")
            };
        }
        /// <summary>
        /// 轮询加速器是否复位
        /// </summary>
        public void GetBoostingReset()
        {
            bsm.OrderType = UploadImageOrderType.GetInstructions;
            string RequestData = CommonFunc.ObjectToJson(bsm);
            var Result = UploadWebServiceControl.GetInstance().CreateWebServicesControl(RequestBoostingResetServices.GetInstance())
                .UploadData(RequestData);
            BoostResetResetReposeModel brrm = CommonFunc.JsonToObject<BoostResetResetReposeModel>(Result);
            var CodeStr = CommonFunc.AesDecrypt(brrm?.data, ConfigServices.GetInstance().localConfigModel.IsAES);
            //var CodeArray = CommonFunc.JsonToObject<List<string>>(CodeStr); 
            if (brrm.code == "1" && CodeStr.Contains("04"))    
            {
                //Common.SetCommand(Common.CommandDic[Command.HitchReset],true);
            }
        }
         
        /// <summary>
        /// 反馈给服务端说明无故障，不需要给我故障代码
        /// </summary>
        public void SetBoostingReset()
        {
            bsm.OrderType = UploadImageOrderType.SetInstructions;
            string RequestSetData = CommonFunc.ObjectToJson(bsm);
            var SetResult = UploadWebServiceControl.GetInstance().CreateWebServicesControl(RequestBoostingResetServices.GetInstance())
                .UploadData(RequestSetData);
            ResposeModel brrm = CommonFunc.JsonToObject<ResposeModel>(SetResult);
            if(brrm.Code != "1")
            {
                SetBoostingReset();
            }
        }
    }
}
