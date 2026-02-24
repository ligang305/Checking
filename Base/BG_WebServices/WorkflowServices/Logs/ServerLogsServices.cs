using CMW.Common.Utilities;
using BGModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BG_Entities;
using BG_WorkFlow;

namespace BG_Services
{
    public class ServerLogsServices
    {
        /// <summary>
        /// 单实例服务
        /// </summary>
        public static ServerLogsServices Service { get; private set; }


        public Dictionary<string, string> ApplicationLogDic = new Dictionary<string, string>();


        static ServerLogsServices()
        {
            Service = new ServerLogsServices();
        }
        private Mutex SendMutex = new Mutex();

        Queue<ServerLogsRequestModel> ServerLogsQueue = new Queue<ServerLogsRequestModel>();

        public void Start()
        {
            TaskPool.GetInstance().AddAndStartTask(TaskList.UploadServerLogs, UploadLogs);
            InitLogDic();  
            CommonDeleget.WriteLogEvent += CommonDeleget_WriteLogEvent;
        }

        public void Stop()
        {
            TaskPool.GetInstance().EndTask(TaskList.UploadServerLogs);
        }

        public void InitLogDic()  
        { 
            ApplicationLogDic.Clear();
            ApplicationLogDic.Add(LogType.ApplicationError,"L99");  
            ApplicationLogDic.Add(LogType.ImageImportDllError, "L99");
            ApplicationLogDic.Add(LogType.ScanStep, "L03");
            ApplicationLogDic.Add(LogType.EquipmentFailure, "L02");
            ApplicationLogDic.Add(LogType.SystemDebug, "L04");
        }

        private void CommonDeleget_WriteLogEvent(string logMsg, string LogType, bool IsInsert = false)
        {
            try
            {
                if(ApplicationLogDic.ContainsKey(LogType))
                {
                    ServerLogs serverLogs = new ServerLogs()
                    {
                        ModulesNo = "001",
                        Equipment = "CMW",
                        LogNo = "0001",
                        Log = CommonFunc.AesEncrypt(logMsg, ConfigServices.GetInstance().localConfigModel.IsAES),
                        LogLevel = ApplicationLogDic[LogType],
                    };
                    AddLog(serverLogs);
                }
            }
            catch (Exception ex)
            {
                BGLogs.Log.GetDistance().WriteErrorLogs(ex.StackTrace);
            }
        }

        private void UploadLogs()
        {
            SendMutex.WaitOne();
            Thread.Sleep(4000);
            while (ServerLogsQueue.Count > 0)
            {
                try
                {
                    Thread.Sleep(10);
                    ServerLogsRequestModel topLogsModel = ServerLogsQueue.Dequeue();
                    if (topLogsModel != null)
                    {
                        topLogsModel.OrderType = UploadImageOrderType.saveDeviceLog;
                        topLogsModel.Token = ConfigServices.GetInstance().localConfigModel.Login.sccessToken;
                    }
                    string ServerLogsRequestModelObject = CommonFunc.ObjectToJson(topLogsModel);
                    string Repose =  UploadWebServiceControl.GetInstance().CreateWebServicesControl(SaveDeviceLogServices.GetInstance()).UploadData(ServerLogsRequestModelObject);
                }
                catch (Exception e)
                {
                    BGLogs.Log.GetDistance().WriteErrorLogs(e.StackTrace);
                }
            }
            SendMutex.ReleaseMutex();
        }

        public void AddLog(ServerLogs serverLogs)
        {
            ServerLogsRequestModel serverLogsRequestModel = new ServerLogsRequestModel();
            serverLogsRequestModel.Data = serverLogs.Log;
            serverLogsRequestModel.Time = CommonFunc.GetDateTime();
            serverLogsRequestModel.Code = serverLogs.LogCode();
            ServerLogsQueue.Enqueue(serverLogsRequestModel);
        }
    }
}
