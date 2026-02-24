using CMW.Common.Utilities;
using BGModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BG_Entities;
using BG_WorkFlow;

namespace BG_Services
{
    public class CirServices
    {
        private bool IsExcuteTask = true;
        public ObservableCollection<StatusModel> HitchList = new ObservableCollection<StatusModel>();
        /// <summary>
        /// 单实例服务
        /// </summary>
        public static CirServices Service { get; private set; }

        static CirServices()
        {
            Service = new CirServices();

        }

        public void Start()
        {
            InitHitchList();
            if(Common.controlVersion != ControlVersion.BS)
            {
                UploadEquipmentParamater();
            }
        }

        public void Stop()
        {
            IsExcuteTask = false;
        }

        private void UploadEquipmentParamater()
        {
            Task.Run(() =>
            {
                while (IsExcuteTask)
                {
                    Thread.Sleep(5000);
                    try
                    {
                        QueueAlarm.GetInstance().AddAlarm(new Alarm()
                        {
                            AlarmContext = TaskList.CMWConnectionAlarm,
                            AlarmCode = TaskCode.CMWConnectionAlarm,
                            DeviceNo = ConfigServices.GetInstance().localConfigModel.EquipmentNo
                        });
                        ////如果PLC未连接
                        QueueAlarm.GetInstance().AddAlarm(new Alarm()
                        {
                            AlarmContext = !PLCControllerManager.GetInstance().IsConnect() ? TaskAlarmStatus.Alarm : TaskAlarmStatus.Normal,
                            AlarmCode = TaskCode.PlcAlarm,
                            DeviceNo = ConfigServices.GetInstance().localConfigModel.EquipmentNo
                        });
                        QueueAlarm.GetInstance().AddAlarm(new Alarm()
                        {
                            AlarmContext = !PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.RadiationSourceOrAcceleratorReady) ? TaskAlarmStatus.Alarm : TaskAlarmStatus.Normal,//BoostingIsReady()
                            AlarmCode = TaskCode.BoostingAlarm,
                            DeviceNo = ConfigServices.GetInstance().localConfigModel.EquipmentNo
                        });

                        //掃描站未連接
                        QueueAlarm.GetInstance().AddAlarm(new Alarm()
                        {
                            AlarmContext = DetecotrControllerManager.GetInstance().DetectorConnection == 3 ? TaskAlarmStatus.Normal : TaskAlarmStatus.Alarm,
                            AlarmCode = TaskCode.ScanAlarm,
                            DeviceNo = ConfigServices.GetInstance().localConfigModel.EquipmentNo
                        });

                        
                        QueueAlarm.GetInstance().AddAlarm(new Alarm()
                        { 
                            AlarmContext = !PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.MainSystemReady) ? TaskAlarmStatus.Alarm : TaskAlarmStatus.Normal,
                            AlarmCode = TaskCode.StopAlarm,
                            DeviceNo = ConfigServices.GetInstance().localConfigModel.EquipmentNo
                        });

                        QueueAlarm.GetInstance().AddAlarm(new Alarm()
                        {
                            AlarmContext = PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.BoostingFire) ? TaskAlarmStatus.Alarm : TaskAlarmStatus.Normal,//IsBoostingFire() 
                            AlarmCode = TaskCode.BoostingFire,
                            DeviceNo = ConfigServices.GetInstance().localConfigModel.EquipmentNo
                        });
                        QueueAlarm.GetInstance().AddAlarm(new Alarm()
                        {

                            AlarmContext = (Convert.ToInt32(Common.GlobalDoseStatus[3]) / 10.00f).ToString("F2"),
                            AlarmCode = TaskCode.IndoorTemp,
                            DeviceNo = ConfigServices.GetInstance().localConfigModel.EquipmentNo
                        });
                        QueueAlarm.GetInstance().AddAlarm(new Alarm()
                        {
                            AlarmContext = (Convert.ToInt32(Common.GlobalDoseStatus[4]) / 10.00f).ToString("F2"),
                            AlarmCode = TaskCode.OuterDoorTemp,
                            DeviceNo = ConfigServices.GetInstance().localConfigModel.EquipmentNo
                        });
                        QueueAlarm.GetInstance().AddAlarm(new Alarm()
                        {
                            AlarmContext = (Convert.ToInt32(Common.GlobalDoseStatus[5]) / 10.00f).ToString("F2"),
                            AlarmCode = TaskCode.BoostingRoomTemp,
                            DeviceNo = ConfigServices.GetInstance().localConfigModel.EquipmentNo
                        });
                        QueueAlarm.GetInstance().AddAlarm(new Alarm()
                        {
                            AlarmContext = CommonFunc.MillToHour(PLCControllerManager.GetInstance().GetPlcBlockStr
                            (ConfigServices.GetInstance().localConfigModel.BoostingRayTimeAddress,
                            Convert.ToUInt16(ConfigServices.GetInstance().localConfigModel.BoostingRayTimeAddressLength), 
                            InqureType.IUInt32)),// "20000",
                            AlarmCode = TaskCode.BoostingTime,
                            DeviceNo = ConfigServices.GetInstance().localConfigModel.EquipmentNo
                        });
                        QueueAlarm.GetInstance().AddAlarm(new Alarm()
                        {
                            AlarmContext = CommonFunc.MillToHour(GetDetectorRunTime()),
                            AlarmCode = TaskCode.DectorTime,
                            DeviceNo = ConfigServices.GetInstance().localConfigModel.EquipmentNo
                        });
                        QueueAlarm.GetInstance().AddAlarm(new Alarm()
                        {
                            AlarmContext = PLCControllerManager.GetInstance().GetPlcBlockStr
                        (ConfigServices.GetInstance().localConfigModel.EquipmentOnTImeAddress, 
                        Convert.ToUInt16(ConfigServices.GetInstance().localConfigModel.EquipmentOnTImeAddressLength),
                        InqureType.IString),// "09",
                            AlarmCode = TaskCode.EquipmentOnTime,
                            DeviceNo = ConfigServices.GetInstance().localConfigModel.EquipmentNo
                        });
                        QueueAlarm.GetInstance().AddAlarm(new Alarm()
                        {
                            AlarmContext = PLCControllerManager.GetInstance().GetPlcBlockStr
                        (ConfigServices.GetInstance().localConfigModel.EquipmentOffTImeAddress,
                        Convert.ToUInt16(ConfigServices.GetInstance().localConfigModel.EquipmentOffTImeAddressLength), 
                        InqureType.IString),// "10",
                            AlarmCode = TaskCode.EquipmentOffTime,
                            DeviceNo = ConfigServices.GetInstance().localConfigModel.EquipmentNo
                        });
                        QueueAlarm.GetInstance().AddAlarm(new Alarm()
                        {
                            AlarmContext = PLCControllerManager.GetInstance().GetPlcBlockStr
                        (ConfigServices.GetInstance().localConfigModel.EquipmentTotalDayAddress,
                        Convert.ToUInt16(ConfigServices.GetInstance().localConfigModel.EquipmentTotalDayAddressLength), 
                        InqureType.IUInt32),// "11",
                            AlarmCode = TaskCode.EquipmentTotalDay,
                            DeviceNo = ConfigServices.GetInstance().localConfigModel.EquipmentNo
                        });
                        QueueAlarm.GetInstance().AddAlarm(new Alarm()
                        {
                            AlarmContext = CommonFunc.MillToHour(PLCControllerManager.GetInstance().GetPlcBlockStr
                      (ConfigServices.GetInstance().localConfigModel.EquipmentTotalTimeAddress,
                      Convert.ToUInt16(ConfigServices.GetInstance().localConfigModel.EquipmentTotalTimeAddressLength),
                      InqureType.IUInt32)),// "12",
                            AlarmCode = TaskCode.EquipmentTotalTime,
                            DeviceNo = ConfigServices.GetInstance().localConfigModel.EquipmentNo
                        });
                    }
                    catch (Exception ex)
                    {
                        CommonDeleget.WriteLogAction(ex.StackTrace, LogType.ApplicationError, true);
                    }
                    CheckBoostHitchCode();
                }
            });
        }

        private string GetDetectorRunTime()
        {
            int Runtime = 0;
            int Scantime = 0;
            ImageImportDll.SX_GetTimes(ImageImportDll.intPtr, out Runtime, out Scantime);
            return Runtime.ToString();
        }

        private void CheckBoostHitchCode()
        {
            try
            {
                if (!PLCControllerManager.GetInstance().IsConnect())
                {
                    foreach (var HitchItem in HitchList)
                    {
                        HitchItem.StatusCode = "0";
                    }
                }
                else
                {
                    foreach (var HitchItem in HitchList)
                    {
                        int ItemIndex = Convert.ToInt32(HitchItem.StatusIndex);
                        if (ItemIndex < Common.GlobalRetStatus.Count)
                        {
                            HitchItem.StatusCode = Common.GlobalRetStatus[ItemIndex] ? "0" : "2";
                            if (HitchItem.StatusCode == "0")
                            {
                                QueueAlarm.GetInstance().AddAlarm(new Alarm()
                                {
                                    AlarmContext = TaskList.BoostingAlarm,
                                    AlarmCode = TaskCode.BoostingAlarm,
                                    DeviceNo = ConfigServices.GetInstance().localConfigModel.EquipmentNo
                                });
                                break;
                            }
                        }
                    }
                    if (HitchList.Count(q => q.StatusCode == "0") == 0)
                    {
                        PollBoostResetServices.GetInstance().SetBoostingReset();
                    }
                }
            }
            catch (Exception ex)
            {
                CommonDeleget.WriteLogAction(ex.StackTrace, LogType.ApplicationError, true);
            }
        }

        private void InitHitchList()
        {
            HitchList.Clear();
            HitChModelBLL.GetInstance().GetHitchModelDataModel
              (SystemDirectoryConfig.GetInstance().GetHittingConfig(Common.controlVersion)).Where(q => q.StatusOwner.Contains("status_Hitch_")).ToList().ForEach(q => HitchList.Add(q));
        }
    }
}
