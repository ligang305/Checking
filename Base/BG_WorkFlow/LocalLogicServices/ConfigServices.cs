using BG_Entities;
using BG_WorkFlow;
using BGDAL;
using BGModel;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using static CMW.Common.Utilities.CommonDeleget;


namespace BG_WorkFlow
{
    public class ConfigServices : BaseInstance<ConfigServices>
    {
        public LocalConfigModel localConfigModel = new LocalConfigModel();

        List<ParamConfig> paramConfigs = new List<ParamConfig>();
        Dictionary<string, string> paramConfigDic = new Dictionary<string, string>();

        public void Start()
        {
            InitCommonPara();
            UpdateConfigAction -= UpdateAppConfig;
            UpdateConfigAction += UpdateAppConfig;

            SettingAddressActionEvent -= SettingAddressAction;
            SettingAddressActionEvent += SettingAddressAction;
        }

        public void Stop()
        {
            UpdateConfigAction -= UpdateAppConfig;
            SettingAddressActionEvent -= SettingAddressAction;
        }


        private void InitCommonPara()
        {
            paramConfigs = ParamConfigDal.GetInstance().QueryParamConfig();
            if (paramConfigs.Count == 0 || paramConfigs.Count!= GetLocalBackUpParamConfig().Count)
            {
                paramConfigs = GetLocalBackUpParamConfig();
            }
            ParaConfigToDic();
            /*常规配置*/
            localConfigModel.LANGUAGE = paramConfigDic[ParamConfigEnum.language];
            localConfigModel.FontSize = paramConfigDic[ParamConfigEnum.fontsize];
            localConfigModel.IpAddress = paramConfigDic[ParamConfigEnum.IpAddress];
            localConfigModel.Port = paramConfigDic[ParamConfigEnum.Port];
            localConfigModel.ArmIpAddress = paramConfigDic[ParamConfigEnum.ArmIpAddress];
            localConfigModel.ArmLocalIpAddress = paramConfigDic[ParamConfigEnum.ArmLocalIpAddress];
            localConfigModel.ArmLocalPort = Convert.ToInt32(paramConfigDic[ParamConfigEnum.ArmLocalPort]);
            localConfigModel.KeepLive = paramConfigDic[ParamConfigEnum.KeepLive];
            localConfigModel.Heart = paramConfigDic[ParamConfigEnum.Heart];
            localConfigModel.IsRemUser = Convert.ToBoolean(paramConfigDic[ParamConfigEnum.IsRemberber]);
            localConfigModel.SystemLock = paramConfigDic[ParamConfigEnum.SystemLock];
            localConfigModel.ArmPort = Convert.ToInt32(paramConfigDic[ParamConfigEnum.ArmPort]);
            string Date = paramConfigDic[ParamConfigEnum.CorrentData];
            localConfigModel.CorrentData = Date.Equals(string.Empty) ? DateTime.MinValue : Convert.ToDateTime(Date);
            localConfigModel.TrialImageMode = paramConfigDic[ParamConfigEnum.TrailImageMode];
            localConfigModel.FreezeMode = paramConfigDic[ParamConfigEnum.FreezeMode];
            localConfigModel.CMW_FastCheckBayNumber = paramConfigDic[ParamConfigEnum.CMW_FastCheckBayNumber];
            localConfigModel.EquipmentNo = IniDll.Read(Section.SOFT, "EquipmentNo"); //EquipmentNo
            localConfigModel.CMW_Version = IniDll.Read(Section.SOFT, "CMW_Version");
            localConfigModel.Type = paramConfigDic[ParamConfigEnum.Type];//IniDll.Read(Section.SOFT, "Type");
            localConfigModel.Freeze = IniDll.Read(Section.SOFT, "Frequency");
            localConfigModel.EquipmentAddress = paramConfigDic[ParamConfigEnum.EquipmentAddress];// IniDll.Read(Section.SOFT, "EquipmentAddress");
            localConfigModel.EquipmentLength = paramConfigDic[ParamConfigEnum.EquipmentLength]; //IniDll.Read(Section.SOFT, "EquipmentLength");
            localConfigModel.BoostingVersionAddress = paramConfigDic[ParamConfigEnum.BoostingVersionAddress];// IniDll.Read(Section.SOFT, "BoostingVersionAddress");
            localConfigModel.BoostingVersionLength = paramConfigDic[ParamConfigEnum.BoostingVersionLength]; //IniDll.Read(Section.SOFT, "BoostingVersionLength");
            localConfigModel.BoostingRunTimeAddress = paramConfigDic[ParamConfigEnum.BoostingRunTimeAddress]; //IniDll.Read(Section.SOFT, "BoostingRunTimeAddress");
            localConfigModel.BoostingRunTimeAddressLength = paramConfigDic[ParamConfigEnum.BoostingRunTimeAddressLength]; //IniDll.Read(Section.SOFT, "BoostingRunTimeAddressLength");
            localConfigModel.BoostingRayTimeAddress = paramConfigDic[ParamConfigEnum.BoostingRayTimeAddress]; //IniDll.Read(Section.SOFT, "BoostingRayTimeAddress");
            localConfigModel.BoostingRayTimeAddressLength = paramConfigDic[ParamConfigEnum.BoostingRayTimeAddressLength]; //IniDll.Read(Section.SOFT, "BoostingRayTimeAddressLength");

            localConfigModel.EquipmentOffTImeAddress = paramConfigDic[ParamConfigEnum.EquipmentOffTImeAddress];
            localConfigModel.EquipmentOffTImeAddressLength = paramConfigDic[ParamConfigEnum.EquipmentOffTImeAddressLength];

            localConfigModel.EquipmentOnTImeAddress = paramConfigDic[ParamConfigEnum.EquipmentOnTImeAddress];
            localConfigModel.EquipmentOnTImeAddressLength = paramConfigDic[ParamConfigEnum.EquipmentOnTImeAddressLength];

            localConfigModel.EquipmentTotalDayAddress = paramConfigDic[ParamConfigEnum.EquipmentTotalDayAddress];
            localConfigModel.EquipmentTotalDayAddressLength = paramConfigDic[ParamConfigEnum.EquipmentTotalDayAddressLength];

            localConfigModel.EquipmentTotalTimeAddress = paramConfigDic[ParamConfigEnum.EquipmentTotalTimeAddress];
            localConfigModel.EquipmentTotalTimeAddressLength = paramConfigDic[ParamConfigEnum.EquipmentTotalTimeAddressLength];
            localConfigModel.IsUserBS = string.IsNullOrEmpty(IniDll.Read(Section.SOFT, "IsUserBS")) ?false: Convert.ToBoolean(IniDll.Read(Section.SOFT, "IsUserBS"));


            localConfigModel.IsSaveImage = Convert.ToBoolean(IniDll.Read(Section.SOFT, ParamConfigEnum.IsSaveImage)); //Convert.ToBoolean(paramConfigDic[ParamConfigEnum.IsSaveImage]); 
           
            localConfigModel.IsDetectorTestMode = 
            String.IsNullOrEmpty(IniDll.Read(Section.SOFT, ParamConfigEnum.IsDetectorTestMode)) ?
            localConfigModel.IsDetectorTestMode : 
            Convert.ToBoolean(IniDll.Read(Section.SOFT, ParamConfigEnum.IsDetectorTestMode));


            /*常规配置*/
            localConfigModel.Server = IniDll.Read(Section.SERVER, "Server");//paramConfigDic[ParamConfigEnum.Server];
            localConfigModel.ParamaterServerPort = IniDll.Read(Section.SERVER, "ParamaterServerPort");//paramConfigDic[ParamConfigEnum.Server];
            localConfigModel.LogsServer = IniDll.Read(Section.SERVER, "LogsServer");
            localConfigModel.IsShowDirection = Convert.ToBoolean(IniDll.Read(Section.SOFT, ParamConfigEnum.IsShowDirection)); 
            localConfigModel.IsShowDualDirection = Convert.ToBoolean(IniDll.Read(Section.SOFT, ParamConfigEnum.IsShowDualDirection));
            localConfigModel.IsShowElctronicFence = string.IsNullOrEmpty(IniDll.Read(Section.SOFT, "IsShowElctronicFence")) ? false : Convert.ToBoolean(IniDll.Read(Section.SOFT, ParamConfigEnum.IsShowElctronicFence));


            localConfigModel.UgrServer = IniDll.Read(Section.SERVER, "UgrServer");//paramConfigDic[ParamConfigEnum.Server];
            localConfigModel.IsAES = Convert.ToBoolean(paramConfigDic[ParamConfigEnum.IsAES]);// Convert.ToBoolean(IniDll.Read(Section.SERVER, "IsAES"));

            CommonDeleget.WriteLogAction($@"localConfigModel.IsAES:{localConfigModel.IsAES}", LogType.NormalLog);

            localConfigModel.IsLogin = Convert.ToBoolean(paramConfigDic[ParamConfigEnum.IsLogin]); //Convert.ToBoolean(IniDll.Read(Section.SERVER, "IsLogin"));
            localConfigModel.ServerSocket = localConfigModel.Server.Trim(@"http:/".ToCharArray()).Split(':')[0];
            localConfigModel.IsEnabledSocketToServer = Convert.ToBoolean(IniDll.Read(Section.SOFT, ParamConfigEnum.IsEnabledSocketToServer));

            /*扫描站配置*/
            localConfigModel.DefalutScanMode = string.IsNullOrEmpty(IniDll.Read(Section.SCAN, "DefalutScanMode")) ? localConfigModel.DefalutScanMode :Convert.ToBoolean(IniDll.Read(Section.SCAN, "DefalutScanMode"));
            localConfigModel.ScanImagePort = paramConfigDic[ParamConfigEnum.ScanImagePort]; IniDll.Read(Section.SCAN, "ScanImagePort");
            localConfigModel.ScanIpAddress = paramConfigDic[ParamConfigEnum.ScanIpAddress]; IniDll.Read(Section.SCAN, "ScanIpAddress");
            localConfigModel.ScanPort = paramConfigDic[ParamConfigEnum.ScanPort]; IniDll.Read(Section.SCAN, "ScanPort");
            localConfigModel.IsPartition = string.IsNullOrEmpty(IniDll.Read(Section.SOFT, "IsPartition")) ? localConfigModel.IsPartition : Convert.ToBoolean(IniDll.Read(Section.SOFT, "IsPartition"));

            /*扫描站配置*/
            localConfigModel.SerialPort = Convert.ToInt32(paramConfigDic[ParamConfigEnum.SerialPort]);// Convert.ToInt32(IniDll.Read(Section.Serial, "SerialPort"));
            localConfigModel.Parity = Convert.ToInt32(paramConfigDic[ParamConfigEnum.Parity]);// Convert.ToInt32(IniDll.Read(Section.Serial, "Parity"));
            localConfigModel.BaudRate = Convert.ToInt32(paramConfigDic[ParamConfigEnum.BaudRate]);// Convert.ToInt32(IniDll.Read(Section.Serial, "BaudRate"));
            localConfigModel.StopBit = Convert.ToInt32(paramConfigDic[ParamConfigEnum.StopBit]); //Convert.ToInt32(IniDll.Read(Section.Serial, "StopBit"));
            localConfigModel.ByteSize = Convert.ToInt32(paramConfigDic[ParamConfigEnum.ByteSize]); //Convert.ToInt32(IniDll.Read(Section.Serial, "ByteSize"));
            /*动态频率配置表*/
            localConfigModel.VerticalSpatialResolution = Convert.ToInt32(paramConfigDic[ParamConfigEnum.VerticalSpatialResolution]); //Convert.ToInt32(IniDll.Read(Section.Freeze, "VerticalSpatialResolution"));
            localConfigModel.AdjustableParameters = Convert.ToInt32(paramConfigDic[ParamConfigEnum.AdjustableParameters]);// Convert.ToInt32(IniDll.Read(Section.Freeze, "AdjustableParameters"));

            //Dose剂量反馈给PLC的地址
            localConfigModel.IsSendDose = string.IsNullOrEmpty(IniDll.Read(Section.Dose, "IsSendDose")) ? false : Convert.ToBoolean(IniDll.Read(Section.Dose, "IsSendDose"));
            localConfigModel.Dose1Position = string.IsNullOrEmpty(IniDll.Read(Section.Dose, "Dose1"))? localConfigModel.Dose1Position : IniDll.Read(Section.Dose, "Dose1");
            localConfigModel.Dose2Position = string.IsNullOrEmpty(IniDll.Read(Section.Dose, "Dose2")) ? localConfigModel.Dose2Position : IniDll.Read(Section.Dose, "Dose2");
            localConfigModel.Dose3Position = string.IsNullOrEmpty(IniDll.Read(Section.Dose, "Dose3")) ? localConfigModel.Dose3Position : IniDll.Read(Section.Dose, "Dose3");

            //PLC其实读取位置
            localConfigModel.InquireStartPosition = string.IsNullOrEmpty(IniDll.Read(Section.PLC, "InquireStartPosition")) ? localConfigModel.InquireStartPosition : IniDll.Read(Section.PLC, "InquireStartPosition");
            localConfigModel.CurrentPosition = string.IsNullOrEmpty(IniDll.Read(Section.PLC, "CurrentPosition")) ? localConfigModel.CurrentPosition :Convert.ToInt16(IniDll.Read(Section.PLC, "CurrentPosition"));
            localConfigModel.LastCalibrationTime = string.IsNullOrEmpty(IniDll.Read(Section.PLC, "LastCalibrationTime")) ? localConfigModel.LastCalibrationTime : Convert.ToDateTime(IniDll.Read(Section.PLC, "LastCalibrationTime"));
           
            
            //RFID的IP
            localConfigModel.RFIDAddress = string.IsNullOrEmpty(IniDll.Read(Section.RFID, "RFIDAddress")) ? localConfigModel.RFIDAddress : IniDll.Read(Section.RFID, "RFIDAddress");
            localConfigModel.RFIDPort = string.IsNullOrEmpty(IniDll.Read(Section.RFID, "RFIDPort")) ? localConfigModel.RFIDPort : Convert.ToUInt32(IniDll.Read(Section.RFID, "RFIDPort"));

            //背散的IP点位
            localConfigModel.BSPort = string.IsNullOrEmpty(IniDll.Read(Section.BS, "BSPort")) ? localConfigModel.BSPort : Convert.ToUInt32(IniDll.Read(Section.BS, "BSPort"));
            localConfigModel.BSAddress = string.IsNullOrEmpty(IniDll.Read(Section.BS, "BSAddress")) ? localConfigModel.BSAddress : IniDll.Read(Section.BS, "BSAddress");
            localConfigModel.BSInquireStartPosition = string.IsNullOrEmpty(IniDll.Read(Section.BS, "BSInquireStartPosition")) ? localConfigModel.BSInquireStartPosition : IniDll.Read(Section.BS, "BSInquireStartPosition");
        }
        private void ParaConfigToDic()
        {
            foreach (var pcitem in paramConfigs)
            {
                paramConfigDic[pcitem.Key] = pcitem.Value;
            }
        }
        private void SettingAddressAction(string IpAddress, string Port, string Flag)
        {
            string IPKey = Flag == "Plc" ? "IpAddress" : "ScanIpAddress";
            string SectionValue = Flag == "Plc" ? Section.SOFT : Section.SCAN;
            string IPValue = IpAddress;
            UpdateAppConfig(IPKey, IPValue, SectionValue, "false");

            string PortKey = Flag == "Plc" ? "Port" : "ScanPort";
            string SectionPortValue = Flag == "Plc" ? Section.SOFT : Section.SCAN;
            string PortValue = Port;
            UpdateAppConfig(PortKey, PortValue, SectionPortValue, "false");
            if (Flag == "Plc")
            {
                localConfigModel.IpAddress = IniDll.Read(Section.SOFT, "IpAddress");// ConfigurationSettings.AppSettings["IpAddress"].ToString();
                localConfigModel.Port = IniDll.Read(Section.SOFT, "Port");//ConfigurationSettings.AppSettings["Port"].ToString();
            }
            else
            {
                localConfigModel.ScanIpAddress = IniDll.Read(Section.SCAN, "ScanIpAddress"); //ConfigurationSettings.AppSettings["ScanIpAddress"].ToString();
                localConfigModel.ScanPort = IniDll.Read(Section.SCAN, "ScanPort"); //ConfigurationSettings.AppSettings["ScanPort"].ToString();
            }
        }
        /// <summary>
        /// 更新配置文件
        /// </summary>
        /// <param name="newKey"></param>
        /// <param name="newValue"></param>
        public void UpdateAppConfig(string newKey, string newValue, string Section, string isAsync = "true", string isShow = "false")
        {
            if (!string.IsNullOrEmpty(Section))
            {
                IniDll.Write(Section, newKey, newValue);
            }
            ParamConfigDal.GetInstance().UpdateParamConfig(new ParamConfig() { Key = newKey, Value = newValue, IsShow = isShow, IsAsync = isAsync });
            //HardwareParamaterServices.Service.Start();
            InitCommonPara();
        }
        public List<ParamConfig> GetLocalBackUpParamConfig()
        {
            List<ParamConfig> backupParamConfigs = new List<ParamConfig>()
            {
                    new ParamConfig(){ Key ="language",Value = "en-us",IsShow = "false"},new ParamConfig(){ Key ="fontsize",Value = "Small",IsShow = "false"},
                    new ParamConfig(){ Key ="IpAddress",Value = "192.168.0.11",IsShow = "false"},new ParamConfig(){ Key ="Port",Value = "102",IsShow = "false"},
                    new ParamConfig(){ Key ="ArmIpAddress",Value = "172.16.213.230",IsShow = "false"},new ParamConfig(){ Key ="ArmPort",Value = "23600",IsShow = "false"},
                    new ParamConfig(){ Key ="ArmLocalIpAddress",Value = "172.16.213.220",IsShow = "false"},new ParamConfig(){ Key ="ArmLocalPort",Value = "23605",IsShow = "false"},
                    new ParamConfig(){ Key ="KeepLive",Value = "false",IsShow = "false"},new ParamConfig(){ Key ="Heart",Value = "false",IsShow = "false"},
                    new ParamConfig(){ Key ="IsRemberber",Value = "True",IsShow = "false"},new ParamConfig(){ Key ="SystemLock",Value = "123456",IsShow = "false"},
                    new ParamConfig(){ Key ="CorrentData",Value = "0001/1/1 0:00:00",IsShow = "false"},new ParamConfig(){ Key ="TrailImageMode",Value = "net",IsShow = "false"},
                    new ParamConfig(){ Key ="FreezeMode",Value = "DYNAMIC",IsShow = "false"},new ParamConfig(){ Key ="EquipmentNo",Value = @">",IsShow = "false"},
                    new ParamConfig(){ Key ="CMW_Version",Value = "2",IsShow = "false"},new ParamConfig(){ Key ="CMW_FastCheckBayNumber",Value = "4"},
                    new ParamConfig(){ Key ="Type",Value = "BGV6000",IsShow = "false"},new ParamConfig(){ Key ="Frequency",Value = "200",IsShow = "false"},
                    new ParamConfig(){ Key ="EquipmentAddress",Value = "DB15.2"},new ParamConfig(){ Key ="EquipmentLength",Value = "50"},
                    new ParamConfig(){ Key ="BoostingVersionAddress",Value = "DB8.6"},new ParamConfig(){ Key ="BoostingVersionLength",Value = "50"},
                    new ParamConfig(){ Key ="BoostingRunTimeAddress",Value = "DB17.0"},new ParamConfig(){ Key ="BoostingRunTimeAddressLength",Value = "4"},
                    new ParamConfig(){ Key ="BoostingRayTimeAddress",Value = "DB17.4"},new ParamConfig(){ Key ="BoostingRayTimeAddressLength",Value = "4"},
                    new ParamConfig(){ Key ="EquipmentOffTImeAddress",Value = "DB17.0"},new ParamConfig(){ Key ="EquipmentOffTImeAddressLength",Value = "4"},
                    new ParamConfig(){ Key ="EquipmentOnTImeAddress",Value = "DB17.4"},new ParamConfig(){ Key ="EquipmentOnTImeAddressLength",Value = "4"},
                    new ParamConfig(){ Key ="EquipmentTotalDayAddress",Value = "DB17.0"},new ParamConfig(){ Key ="EquipmentTotalDayAddressLength",Value = "4"},
                    new ParamConfig(){ Key ="EquipmentTotalTimeAddress",Value = "DB17.4"},new ParamConfig(){ Key ="EquipmentTotalTimeAddressLength",Value = "4"},
                    new ParamConfig(){ Key ="PassWord",Value = "123456",IsShow = "false"},new ParamConfig(){ Key ="User",Value = "CMW",IsShow = "false"},
                    new ParamConfig(){ Key ="IsAES",Value = "false",IsShow = "false"},new ParamConfig(){ Key ="IsLogin",Value = "true",IsShow = "false"},
                    new ParamConfig(){ Key ="Server",Value = "http://172.16.212.16:8080"},new ParamConfig(){ Key ="SerialPort",Value = "4"},
                    new ParamConfig(){ Key ="Parity",Value = "1"},new ParamConfig(){ Key ="BaudRate",Value = "9600"},
                    new ParamConfig(){ Key ="StopBit",Value = "1"},new ParamConfig(){ Key ="ByteSize",Value = "8"},
                    new ParamConfig(){ Key ="VerticalSpatialResolution",Value = "3"},new ParamConfig(){ Key ="AdjustableParameters",Value = "0"},
                    new ParamConfig(){ Key ="ScanImagePort",Value = "4001",IsShow = "false"},new ParamConfig(){ Key ="ScanIpAddress",Value = "127.0.0.1",IsShow = "false"},
                    new ParamConfig(){ Key ="ScanPort",Value = "3000",IsShow = "false"},new ParamConfig(){ Key = "IsSaveImage",Value = "false",IsShow = "false"},
                    new ParamConfig(){ Key = "TestMode",Value = "false",IsShow = "true"}
            };
            CommonDeleget.WriteLogAction($@"backupParamConfigs End:{localConfigModel.IsAES}", LogType.NormalLog);
            foreach (var item in backupParamConfigs)
            {
                if(!ParamConfigDal.GetInstance().QueryParamConfig(item))
                {
                    ParamConfigDal.GetInstance().InsertParamConfig(item);
                }
            }
            return backupParamConfigs;
        }



    }
}
