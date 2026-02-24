using BG_Entities;
using BG_WorkFlow;
using BGCommunication;
using BGModel;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.CommonFunc;

namespace BG_Services
{
    public class BegoodServerController : BaseInstance<BegoodServerController>, IWebEquipment
    {
        public bool IsLoad = false;
        /// <summary>
        /// 任务队列
        /// </summary>
        public Stack<RecvMessage> TaskIdQueue = new Stack<RecvMessage>();

        public Action<RecvMessage> TaskArriveAction = null;

        /// <summary>
        /// 增强扫描车辆数据列表
        /// </summary>
        public List<string> EnhanceScanList = new List<string>();

        public Action<List<string>> EnhanceScan = null;

        protected CIRProtocol CurrenCirProrocol;
        public void DisConnection()
        {
            CurrenCirProrocol?.DisConnect();
        }
        public bool IsConnection()
        {
            return CurrenCirProrocol.IsConnect();
        }
        public async void Load(CIRProtocol commonProtocol)
        {
            IsLoad = true;
            await Task.Run(() =>
            {
                CurrenCirProrocol = commonProtocol;
                CurrenCirProrocol.WaitRecTick = 10000;
                ConnectInterface client;
                client = new TCPClient(ConfigServices.GetInstance().localConfigModel.ServerSocket, Convert.ToInt32("2021"), false, false);
                CurrenCirProrocol.ReceviceTaskEvent += ReceiveServerData;
                CurrenCirProrocol.InitConnection(ref client);
                CurrenCirProrocol.Connect();
                Thread.Sleep(1000);
                SendEquipmentInfo();
                InquireCIRProtocol();
                HardwareParamaterServices.Service.Start();
            });
            CommonDeleget.UploadImageInfoToWebBySocketEvent += CommonDeleget_UploadImageInfoToWebBySocketEvent;
            CommonDeleget.UploadAlarmInfoToWebBySocketEvent += CommonDeleget_UploadAlarmInfoToWebBySocketEvent;
            
            //TimeAsync("20220307144200000");
        }
        private bool CommonDeleget_UploadAlarmInfoToWebBySocketEvent(string Data)
        {
            return SubmitAlarmData(Data);
        }
        /// <summary>
        /// 将图像信息通过Socket上传至后台服务器
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public void InquireCIRProtocol()
        {
            Debug.WriteLine("-------------------------ganggang_upload_img13-----------------------");
            Thread th = new Thread(delegate ()
            {
                while (true)
                {
                    if (IsLoad == false)
                    {
                        break;
                    }
                    Thread.Sleep(300);
                    if (!CurrenCirProrocol.IsConnect())
                    {
                        CurrenCirProrocol.Connect();
                        Thread.Sleep(100);
                        SendEquipmentInfo();
                        continue;
                    }
                }
            });
            th.Start();
            th.Priority = ThreadPriority.Normal;
        }
        /// <summary>
        /// 接收服务端回调的数据
        /// </summary>
        public void ReceiveServerData(byte[] bytes)
        {
            try
            {
                string data = Encoding.UTF8.GetString(bytes);
                ServerSocketBodyBaseWithoutToken serverSocketBodyBaseWithoutToken = CommonFunc.JsonToObject<ServerSocketBodyBaseWithoutToken>(data);
                if (serverSocketBodyBaseWithoutToken.OrderType.Equals(UploadImageOrderType.DNT001))
                {
                    CommonDeleget.WriteLogAction($"ReceiveServerData-TaskId:bytes {BitConverter.ToString(bytes)}", LogType.SocketServices, true);
                    CommonDeleget.WriteLogAction($"ReceiveServerData-TaskId:string {data}", LogType.SocketServices, true);
                    RecvMessage recvMessage = ConvertServerSocketBodyToRecvMessage(serverSocketBodyBaseWithoutToken);
                    TaskIdQueue.Push(recvMessage);
                    TaskArriveAction?.Invoke(recvMessage);
                }
                else if (serverSocketBodyBaseWithoutToken.OrderType.Equals(UploadImageOrderType.DNT002))
                {
                    string ServerTime = ConvertServerSocketBodyToTime(serverSocketBodyBaseWithoutToken);
                    TimeAsync(ServerTime);
                }
                else if(serverSocketBodyBaseWithoutToken.OrderType.Equals(UploadImageOrderType.DNJ003))
                {
                    BGLogs.Log.GetDistance().WriteInfoLogs($@"DNJ003:{data}");
                    HardwareParamaterServices.Service.Start();
                    BGLogs.Log.GetDistance().WriteInfoLogs($@"服务端主动下发参数同步，同步完成");
                }
                else if (serverSocketBodyBaseWithoutToken.OrderType.Equals(UploadImageOrderType.BarrierGateStatus))
                {
                    BGLogs.Log.GetDistance().WriteInfoLogs($@"DNJ003:{data}");

                    if(!string.IsNullOrEmpty(serverSocketBodyBaseWithoutToken.Data))
                    {
                        BarrierGateStatus barrierGateStatus = CommonFunc.JsonToObject<BarrierGateStatus>(serverSocketBodyBaseWithoutToken.Data);
                        if(barrierGateStatus!=null)
                        {
                            ///道闸开
                            if(barrierGateStatus.OpenOrClose == "1")
                            {
                                PLCControllerManager.GetInstance().WritePositionValue(CommandDic[Command.BarierGate], true);
                            }
                            //道闸关
                            else
                            {
                                PLCControllerManager.GetInstance().WritePositionValue(CommandDic[Command.BarierGate], false);
                            }    
                        }
                    }
                    

                    BGLogs.Log.GetDistance().WriteInfoLogs($@"收到服务端道闸信息");
                }
                else if (serverSocketBodyBaseWithoutToken.OrderType.Equals(UploadImageOrderType.EnhanceScan004))
                {
                    BGLogs.Log.GetDistance().WriteInfoLogs($@"EnhanceScan004 接收到增强扫描数据:{data},");
                    EnhanceScanList.Clear();
                    //这里把接到的需要增强扫描的数据向外更新
                    EnhanceScanList.Add(serverSocketBodyBaseWithoutToken.Data);
                    EnhanceScan?.Invoke(EnhanceScanList);
                }
            }
            catch (Exception ex)
            {
                CommonDeleget.WriteLogAction(ex.StackTrace, LogType.ApplicationError, true);
            }
        }
        /// <summary>
        /// 发送设备信息
        /// </summary>
        public void SendEquipmentInfo()
        {
            ServerSocketBody serverSocketBody = new ServerSocketBody()
            {
                Token = ConfigServices.GetInstance().localConfigModel.Login.sccessToken,
                SC = ConfigServices.GetInstance().localConfigModel.IsAES ? "1" : "0",
                MessageCode = $"{UploadImageOrderType.ClientBind}{DateTime.Now.ToString("yyyyMMddHHmmssfff")}",
                OrderType = UploadImageOrderType.ClientBind
            };
            ServerSocketBodyData serverSocketBodyData = new ServerSocketBodyData()
            {
                ClientCode = $@"{ConfigServices.GetInstance().localConfigModel.EquipmentNo}-Socket"
            };
            serverSocketBody.Data = CommonFunc.ObjectToJson<ServerSocketBodyData>(serverSocketBodyData);
            string serverSocketBodyStr = $@"{CommonFunc.ObjectToJson<ServerSocketBody>(serverSocketBody)}";
            CurrenCirProrocol.SendEquipmentInfo(serverSocketBodyStr);
        }
        /// <summary>
        /// 发送上传图片信息
        /// </summary>
        /// <param name="ImageInfo"></param>
        /// <returns></returns>
        public bool SendUploadImageInfo(string ImageInfo)
        {
            Debug.WriteLine("-------------------------ganggang_upload_img12-----------------------");
            return CurrenCirProrocol.SendUploadImageInfo(ImageInfo);
        }
        /// <summary>
        /// 发送RFID的信息
        /// </summary>
        /// <param name="ImageInfo"></param>
        /// <returns></returns>
        public bool SendRFIDInfo(string RfidIndo)
        {
            ServerSocketBody serverSocketBody = new ServerSocketBody()
            {
                Token = ConfigServices.GetInstance().localConfigModel.Login.sccessToken,
                SC = ConfigServices.GetInstance().localConfigModel.IsAES ? "1" : "0",
                MessageCode = $"{UploadImageOrderType.RFID001}{DateTime.Now.ToString("yyyyMMddHHmmssfff")}",
                OrderType = UploadImageOrderType.RFID001
            };
            serverSocketBody.Data = RfidIndo;
            string serverSocketBodyStr = $@"{CommonFunc.ObjectToJson<ServerSocketBody>(serverSocketBody)}";
            BGLogs.Log.GetDistance().WriteInfoLogs($@"SendRfidInfo Socket:{serverSocketBodyStr}");
            return CurrenCirProrocol.SendRfidInfo(serverSocketBodyStr);
        }
        public void SendScanFaildTask(string TaskId,string ScanStep)
        {
            ServerSocketBody serverSocketBody = new ServerSocketBody()
            {
                Token = ConfigServices.GetInstance().localConfigModel.Login.sccessToken,
                SC = ConfigServices.GetInstance().localConfigModel.IsAES ? "1" : "0",
                MessageCode = $"{UploadImageOrderType.UPK002}{DateTime.Now.ToString("yyyyMMddHHmmssfff")}",
                OrderType = UploadImageOrderType.UPK002
            };
            FaildSocketBody faildSocketBody = new FaildSocketBody()
            {
                TaskId = TaskId,
                Reason = ScanStep
            };
            serverSocketBody.Data = CommonFunc.ObjectToJson<FaildSocketBody>(faildSocketBody);
            string serverSocketBodyStr = $@"{CommonFunc.ObjectToJson<ServerSocketBody>(serverSocketBody)}";
            CurrenCirProrocol.SendFaildScanTask(serverSocketBodyStr);
        }

        public void SendTotalEquipmentSystemReady(List<bool> isSystemSystemReady)
        {
            ServerSocketBody serverSocketBody = new ServerSocketBody()
            {
                Token = ConfigServices.GetInstance().localConfigModel.Login.sccessToken,
                SC = ConfigServices.GetInstance().localConfigModel.IsAES ? "1" : "0",
                MessageCode = $"{UploadImageOrderType.ClientBind}{DateTime.Now.ToString("yyyyMMddHHmmssfff")}",
                OrderType = UploadImageOrderType.TotalEquipmentSystemReady
            };
            ServerSocketBodyWithData serverSocketBodyData = new ServerSocketBodyWithData()
            {
                ClientCode = $@"{ConfigServices.GetInstance().localConfigModel.EquipmentNo}-Socket",
                Data = CommonFunc.ObjectToJson<List<bool>>(isSystemSystemReady)
            };
            serverSocketBody.Data = CommonFunc.ObjectToJson<ServerSocketBodyData>(serverSocketBodyData);
            string serverSocketBodyStr = $@"{CommonFunc.ObjectToJson<ServerSocketBody>(serverSocketBody)}";
            CurrenCirProrocol.SendTotalEquipmentSystemReady(serverSocketBodyStr);
        }

        /// <summary>
        /// 将服务器SocketBody转为之前的RecvMessage
        /// </summary>
        /// <param name="serverSocketBodyBaseWithoutToken"></param>
        private RecvMessage ConvertServerSocketBodyToRecvMessage(ServerSocketBodyBaseWithoutToken serverSocketBodyBaseWithoutToken)
        {
            try
            {
                CommonDeleget.WriteLogAction($"string {serverSocketBodyBaseWithoutToken.Data}", LogType.Services, true);
                ServerSocketTaskInfo serverSocketTaskInfo = CommonFunc.JsonToObject<ServerSocketTaskInfo>(serverSocketBodyBaseWithoutToken.Data);
                CommonDeleget.WriteLogAction($"GetTaskId string {serverSocketTaskInfo.TaskId}", LogType.Services, true);
                RecvMessage recvMessage = new RecvMessage()
                {
                    Code = "1",
                    Data = CommonFunc.ObjectToJson<TaskInfo>(new TaskInfo() { FileInfos = serverSocketTaskInfo.Path, TaskId = serverSocketTaskInfo.TaskId }),
                };
                return recvMessage;
            }
            catch (Exception ex)
            {
                CommonDeleget.WriteLogAction(ex.StackTrace, LogType.ApplicationError, true);
                return null;
            }
        }

        /// <summary>
        /// 将Socket消息体转化为设置时间格式的对象
        /// </summary>
        /// <param name="serverSocketBodyBaseWithoutToken"></param>
        private string ConvertServerSocketBodyToTime(ServerSocketBodyBaseWithoutToken serverSocketBodyBaseWithoutToken)
        {
            try
            {
                CommonDeleget.WriteLogAction($"ConvertServerSocketBodyToTime-ServerSocketSettingTime-data:string {serverSocketBodyBaseWithoutToken.Data}", LogType.Services, true);
                ServerSocketSettingTime serverSocketTaskInfo = CommonFunc.JsonToObject<ServerSocketSettingTime>(serverSocketBodyBaseWithoutToken.Data);
                CommonDeleget.WriteLogAction($"ConvertServerSocketBodyToTime-ServerSocketSettingTime-data:Time string {serverSocketTaskInfo.Time}", LogType.Services, true);
                return serverSocketTaskInfo?.Time;
            }
            catch (Exception ex)
            {
                CommonDeleget.WriteLogAction(ex.StackTrace, LogType.ApplicationError, true);
                return null;
            }
        }
        private ServerSocketBody ConvertRequestModelToRequestModelBySocket(RequestModel requestModel)
        {
            SubmitCheckInDataInfoBySocket submitCheckInDataInfoBySocket = new SubmitCheckInDataInfoBySocket();
            SubmitCheckInDataInfo submitCheckInDataInfo = CommonFunc.JsonToObject<SubmitCheckInDataInfo>(CommonFunc.AesDecrypt(requestModel.Data, ConfigServices.GetInstance().localConfigModel.IsAES));
            submitCheckInDataInfoBySocket.DeviceNo = ConfigServices.GetInstance().localConfigModel.EquipmentNo;
            submitCheckInDataInfoBySocket.TaskId = submitCheckInDataInfo.TaskId;
            submitCheckInDataInfoBySocket.ImageInfo = submitCheckInDataInfo.ImageInfo;

            foreach (var pathInfoItem in submitCheckInDataInfo.Files)
            {
                PathInfoBySocket pathInfoBySocket = new PathInfoBySocket() { fileType = pathInfoItem.fileType, path = pathInfoItem.path, sequence = pathInfoItem.pictureNo };
                submitCheckInDataInfoBySocket.Files.Add(pathInfoBySocket);
            }
            ServerSocketBody serverSocketBody = new ServerSocketBody()
            {
                Token = ConfigServices.GetInstance().localConfigModel.Login.sccessToken,
                SC = ConfigServices.GetInstance().localConfigModel.IsAES ? "1" : "0",//ConfigServices.GetInstance().localConfigModel.IsAES ? "1" :
                OrderType = requestModel.OrderType == UploadImageOrderType.SubmitScanData?UploadImageOrderType.UPK001: UploadImageOrderType.UPK003,
                Data = CommonFunc.ObjectToJson<SubmitCheckInDataInfoBySocket>(submitCheckInDataInfoBySocket),
            };
            serverSocketBody.MessageCode = $"{serverSocketBody.OrderType}{DateTime.Now.ToString("yyyyMMddHHmmssfff")}";
            return serverSocketBody;
        }
        /// <summary>
        /// 将图片信息上传至服务器后台
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        private bool CommonDeleget_UploadImageInfoToWebBySocketEvent(string Data)
        {
            try
            {
                Debug.WriteLine("-------------------------ganggang_upload_img14-----------------------");
                RequestModel requestModel = CommonFunc.JsonToObject<RequestModel>(Data);
                ServerSocketBody serverSocketBody = ConvertRequestModelToRequestModelBySocket(requestModel);
                string SendBody = CommonFunc.ObjectToJson<ServerSocketBody>(serverSocketBody);
                CommonDeleget.WriteLogAction($"UploadImageInfoToWebBySocketEvent-Body: {SendBody}", LogType.SocketServices, true);
                return SendUploadImageInfo(SendBody);
            }
            catch (Exception ex)
            {
                CommonDeleget.WriteLogAction(ex.StackTrace, LogType.ApplicationError, true);
                return false;
            }
        }
        /// <summary>
        /// 提交报警信息
        /// </summary>
        /// <param name="serverSocketBody"></param>
        /// <returns></returns>
        private bool SubmitAlarmData(string serverSocketBody)
        {
            return CurrenCirProrocol.SendAlarmInfo(serverSocketBody);
        }

        /// <summary>
        /// 提交本地参数
        /// </summary>
        /// <param name="serverSocketBody"></param>
        /// <returns></returns>
        public bool SendLocalParamater(string serverSocketBody)
        {
            if (CurrenCirProrocol == null) return false;
            ServerSocketBody LocalParamaterServerSocketBody = new ServerSocketBody()
            {
                Token = ConfigServices.GetInstance().localConfigModel.Login.sccessToken,
                SC = ConfigServices.GetInstance().localConfigModel.IsAES ? "1" : "0",
                MessageCode = $"{UploadImageOrderType.UPT001}{DateTime.Now.ToString("yyyyMMddHHmmssfff")}",
                OrderType = UploadImageOrderType.UPT001
            };
            LocalParamaterServerSocketBodyData serverSocketBodyData = new LocalParamaterServerSocketBodyData()
            {
                DeviceNo = $@"{ConfigServices.GetInstance().localConfigModel.EquipmentNo}"
            };
            serverSocketBodyData.Parameters = serverSocketBody;
            LocalParamaterServerSocketBody.Data = CommonFunc.ObjectToJson(serverSocketBodyData);
            return CurrenCirProrocol.SendLocalParamater(CommonFunc.ObjectToJson(LocalParamaterServerSocketBody));
        }
        /// <summary>
        /// 时间同步
        /// </summary>
        /// <returns></returns>
        private bool TimeAsync(string sysTime)
        {
            if (sysTime.Length != 17) return false;
            SYSTEMTIME sYSTEMTIME = ConvertSystimeStrToDateTime(sysTime);
            return SystemTimeSetting.SetLocalTime(ref sYSTEMTIME);
        }
        /// <summary>
        /// 将字符串转化为Datetime
        /// </summary>
        /// <param name="sysTime"></param>
        /// <returns></returns>
        private SYSTEMTIME ConvertSystimeStrToDateTime(string sysTimestr)
        {
            SYSTEMTIME sYSTEMTIME = new SYSTEMTIME();
            ushort Year = Convert.ToUInt16(sysTimestr.Substring(0, 4));
            ushort Month = Convert.ToUInt16(sysTimestr.Substring(4, 2));
            ushort Day = Convert.ToUInt16(sysTimestr.Substring(6, 2));
            ushort Hour = Convert.ToUInt16(sysTimestr.Substring(8, 2));
            ushort Min = Convert.ToUInt16(sysTimestr.Substring(10, 2));
            ushort Second = Convert.ToUInt16(sysTimestr.Substring(12, 2));
            ushort millSecond = Convert.ToUInt16(sysTimestr.Substring(14, 3));
            sYSTEMTIME.wYear = Year;
            sYSTEMTIME.wMonth = Month;
            sYSTEMTIME.wDay = Day;
            sYSTEMTIME.wHour = Hour;
            sYSTEMTIME.wMinute = Min;
            sYSTEMTIME.wSecond = Second;
            sYSTEMTIME.wMilliseconds = millSecond;
            return sYSTEMTIME;
        }
    }
}