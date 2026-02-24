using BG_Entities;
using BG_WorkFlow;
using BGModel;
using CMW.Common.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BG_Services
{
    public class QueueAlarm : BaseInstance<QueueAlarm>
    {
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        public QueueAlarm()
        {
            TaskPool.GetInstance().AddTask(TaskList.Alarm, Excute);
            TaskPool.GetInstance().StartTask(TaskList.Alarm, false);
        }

        BlockingCollection<Alarm> AlarmQueue = new BlockingCollection<Alarm>();
        AlarmDeviceNoAndAlarmList AlarmList = new AlarmDeviceNoAndAlarmList();
        //发送锁
        private Mutex SendMutex = new Mutex();

        public void AddAlarm(Alarm AlarmModel)
        {
            var AlarmItem = AlarmQueue.FirstOrDefault(q => q.AlarmCode == AlarmModel.AlarmCode);
            if (AlarmItem != null)
            {
                AlarmItem.AlarmContext = AlarmModel.AlarmContext;
            }
            else
            {
                AlarmQueue.Add(AlarmModel);
            }
        }

        public void AddAlarmToList(Alarm alarmModel)
        {
            var AlarmItem = AlarmList.AlarmData.FirstOrDefault(q => q.AlarmCode == alarmModel.AlarmCode);
            if (AlarmItem != null)
            {
                AlarmItem.AlarmContext = alarmModel.AlarmContext;
            }
            else
            {
                AlarmList.AlarmData.Add(AlarmItem);
            }
        }


        /// <summary>  
        /// 执行上传报警信息
        /// </summary>
        public void Excute()
        {
            Debug.WriteLine("-------------------------ganggang_upload_img11-----------------------");
            Thread.Sleep(40000);
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    Thread.Sleep(10);
                    Alarm topAlarm = null;
                    AlarmQueue.TryTake(out topAlarm);
                    if (topAlarm == null) continue;
                    AlarmConvertToWithoutDeviceNo(topAlarm);
                    AlarmData ad = new AlarmData() { Data = CommonFunc.AesEncrypt(CommonFunc.ObjectToJson(topAlarm), ConfigServices.GetInstance().localConfigModel.IsAES, "begood-123456789"), OrderType = UploadImageOrderType.AddAlarm, Token = ConfigServices.GetInstance().localConfigModel.Login.sccessToken };
                    string AlarmObject = CommonFunc.ObjectToJson(ad);
                    if (!ConfigServices.GetInstance().localConfigModel.IsEnabledSocketToServer)
                    {
                        UploadWebServiceControl.GetInstance().CreateWebServicesControl(AlarmServices.GetInstance()).UploadData(AlarmObject);
                    }
                    else
                    {
                        CombineAlarmData();
                    }
                }
                catch (Exception e)
                {
                    CommonDeleget.WriteLogAction(e.StackTrace, LogType.ApplicationError, true);
                }
            }
        }

        public void AlarmConvertToWithoutDeviceNo(Alarm alarm)
        {
            try
            {
                if (alarm == null)
                    return;
                AlarmWithOutDeviceNo alarmWithOutDeviceNo = new AlarmWithOutDeviceNo()
                {
                    AlarmCode = alarm.AlarmCode,
                    AlarmContext = alarm.AlarmContext
                };
                if (ConfigServices.GetInstance().localConfigModel.IsEnabledSocketToServer)
                {
                    AlarmList.AlarmData.Add(alarmWithOutDeviceNo);
                }
            }
            catch (Exception ex)
            {
                CommonDeleget.WriteLogAction(ex.StackTrace, LogType.ApplicationError, true);
            }
        }


        /// <summary>
        /// 组装报警数据并清除
        /// </summary>
        public void CombineAlarmData()
        {
            try
            {
                ServerSocketBody serverSocketBody = new ServerSocketBody()
                {
                    SC = ConfigServices.GetInstance().localConfigModel.IsAES ? "1" : "0",
                    OrderType = UploadImageOrderType.AddAlarm,
                    Token = ConfigServices.GetInstance().localConfigModel.Login.sccessToken,
                    MessageCode = $"{UploadImageOrderType.DNT001}{DateTime.Now.ToString("yyyyMMddHHmmssfff")}",
                };
                AlarmList.DeviceNo = $@"{ConfigServices.GetInstance().localConfigModel.EquipmentNo}";
                serverSocketBody.Data = CommonFunc.ObjectToJson(AlarmList);
                string serverSocketBodyStr = CommonFunc.ObjectToJson(serverSocketBody);
                CommonDeleget.UploadAlarmInfoBySocket(serverSocketBodyStr);
            }
            catch (Exception ex)
            {
                CommonDeleget.WriteLogAction(ex.StackTrace, LogType.ApplicationError, true);
            }
            AlarmList.AlarmData.Clear();
        }
    }




    public class AlarmData : BaseRequestClientTypeWithToken
    {
        public string Data { get; set; }
    }
    public class RFIDData : BaseRequestClientTypeWithToken
    {
        public string Data { get; set; }
    }
    /// <summary>
    /// 报警的
    /// </summary>
    public class AlarmWithOutDeviceNo
    {
        public string AlarmCode { get; set; }
        public string AlarmContext { get; set; }
    }

    public class AlarmDeviceNoAndAlarmList
    {
        public string DeviceNo { get; set; }
        public List<AlarmWithOutDeviceNo> AlarmData { get; set; } = new List<AlarmWithOutDeviceNo>();
    }
    /// <summary>
    /// 报警的信息无设备编号
    /// </summary>
    public class Alarm : AlarmWithOutDeviceNo
    {
        public string DeviceNo { get; set; }
    }
}
