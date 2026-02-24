using BG_Entities;
using BG_WorkFlow;
using BGDAL;
using BGModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.CommonFunc;

namespace BG_Services
{
    public class HardwareParamaterServices
    {
        ParamConfigDal paramConfigDal = new ParamConfigDal();
        List<ParamConfig> ParamConfigs = new List<ParamConfig>();
        List<Parameter> ServerParameters = new List<Parameter>();
        /// <summary>
        /// 单实例服务
        /// </summary>
        public static HardwareParamaterServices Service { get; private set; }
        /// <summary>
        /// 硬件参数服务类/管理类
        /// </summary>
        static HardwareParamaterServices()
        {
            Service = new HardwareParamaterServices();
        }

        public async void Start()
        {
            await Task.Run(() =>
            {
                LoadLocalParamater();
            });

            await Task.Run(() =>
            {
                SyncParamaterFromClient();
            });

            await Task.Run(() =>
            {
                SyncParamaterFromServer();  
            });
        }

        public void Stop()
        {

        }


        /// <summary>
        /// 从服务器同步参数
        /// </summary>
        private void SyncParamaterFromServer()
        {
            RequestModel requestModel = new RequestModel();
            requestModel.OrderType = UploadImageOrderType.getDeviceConfig;
            requestModel.Token = ConfigServices.GetInstance().localConfigModel.Login.sccessToken;
            requestModel.Data = AesEncrypt(ObjectToJson(new ChannelNo() { DeviceNo = ConfigServices.GetInstance().localConfigModel.EquipmentNo }), ConfigServices.GetInstance().localConfigModel.IsAES);
            try
            {
                BGLogs.Log.GetDistance().WriteInfoLogs("HardwareParamaterServices" + ObjectToJson(requestModel));
                string Respose = UploadWebServiceControl.GetInstance().CreateWebServicesControl
                            (ParamaterServices.GetInstance()).UploadData(ObjectToJson(requestModel));
                HandleParamaterCallback(Respose);
            }
            catch (Exception ex)
            {
                BGLogs.Log.GetDistance().WriteInfoLogs(ex.StackTrace);
            }
        }
        /// <summary>
        /// 处理请求回来的报文
        /// </summary>
        /// <param name="Respose"></param>
        private void HandleParamaterCallback(string Respose)
        {
            try
            {
                ResposeModel resposeModel = JsonToObject<ResposeModel>(Respose);
                if (resposeModel.Code == "0")
                {
                    ServerParameters = JsonToObject<List<Parameter>>(AesDecrypt(resposeModel.Data.ToString(), ConfigServices.GetInstance().localConfigModel.IsAES));
                    foreach (var ServerParametersItem in ServerParameters)
                    {
                        ParamConfig paramConfig = ParamConfigs.FirstOrDefault(q => q.Key == ServerParametersItem.Key);
                        if (paramConfig != null)
                        {
                            paramConfig.Value = ServerParametersItem.Value;
                            paramConfig.IsAsync = "true";
                            paramConfigDal.UpdateParamConfig(paramConfig);
                        }
                    }
                }
                else
                {
                    BGLogs.Log.GetDistance().WriteInfoLogs($@"resposeModel 错误：{resposeModel.Message}");
                }
            }
            catch (Exception ex)
            {
                BGLogs.Log.GetDistance().WriteInfoLogs(ex.InnerException);
            }
        }

        /// <summary>
        /// 从客户端上传参数
        /// </summary>
        private void SyncParamaterFromClient()
        {
            try
            {
                Debug.WriteLine("-------------------------ganggang_upload_img30-----------------------");
                List<Parameter> Parameters = new List<Parameter>();
                ParamConfigs.ForEach(ConfigItem =>
                {
                    if(ConfigItem.IsAsync == "false")
                    {
                        Parameter parameter = new Parameter()
                        {
                            Key = ConfigItem.Key,
                            Value = ConfigItem.Value
                        };
                        Parameters.Add(parameter);
                    }
                });
                if(Parameters.Count!=0)
                {
                    if (ConfigServices.GetInstance().localConfigModel.IsEnabledSocketToServer)
                    {
                        BegoodServerController.GetInstance().SendLocalParamater(ObjectToJson(Parameters));
                        return;
                    }
                    else
                    {
                        UpdateParamaterByHttp(Parameters);
                    }
                }
                //ParamConfigs.ForEach(q => 
                //{ 
                //    q.IsAsync = "true";
                //    paramConfigDal.UpdateParamConfig(q);
                //});
            }
            catch (Exception ex)
            {
                BGLogs.Log.GetDistance().WriteErrorLogs(ex.StackTrace);
            }
        }
        /// <summary>
        /// 从客户端上传参数
        /// </summary>
        public void SubmitParamater(ref ParamConfig paramConfig)
        {
            try
            {
                Debug.WriteLine("-------------------------ganggang_upload_img31-----------------------");
                List<Parameter> Parameters = new List<Parameter>();
                Parameter parameter = new Parameter() { Key = paramConfig.Key, Value = paramConfig.Value };
                Parameters.Add(parameter);
                if (ConfigServices.GetInstance().localConfigModel.IsEnabledSocketToServer)
                {
                    BegoodServerController.GetInstance().SendLocalParamater(ObjectToJson(Parameters));
                    return;
                }
                else
                {
                    UpdateParamaterByHttp(Parameters);
                }
                paramConfig.IsAsync = "true";
            }
            catch (Exception ex)
            {
                paramConfig.IsAsync = "false";
                BGLogs.Log.GetDistance().WriteErrorLogs(ex.StackTrace);
            }
        }

        /// <summary>
        /// 通过Http上传
        /// </summary>
        private static void UpdateParamaterByHttp(List<Parameter> Parameters)
        {
            try
            {
                Debug.WriteLine("-------------------------ganggang_upload_img32-----------------------");
                RequestModel requestModel = new RequestModel();
                requestModel.OrderType = UploadImageOrderType.updateDeviceConfig;
                requestModel.Token = ConfigServices.GetInstance().localConfigModel.Login.sccessToken;
                LocalParamaterServerHttpBodyData serverHttpBodyData = new LocalParamaterServerHttpBodyData()
                {
                    DeviceNo = $@"{ConfigServices.GetInstance().localConfigModel.EquipmentNo}"
                };
                serverHttpBodyData.Config = ObjectToJson(Parameters);
                requestModel.Data = AesEncrypt(ObjectToJson(serverHttpBodyData), ConfigServices.GetInstance().localConfigModel.IsAES);
                UploadWebServiceControl.GetInstance().CreateWebServicesControl(ParamaterUpdateServices.GetInstance()).UploadData(ObjectToJson(requestModel));

            }
            catch (Exception ex)
            {
                BGLogs.Log.GetDistance().WriteErrorLogs(ex.StackTrace);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void LoadLocalParamater()
        {
            ParamConfigs = paramConfigDal.QueryParamConfig();
        }
    }
}
