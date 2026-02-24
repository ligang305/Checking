using BG_Entities;
using BG_WorkFlow;
using BGCommunication;
using BGModel;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG_Services
{
    public class EquipmentManager : BaseInstance<EquipmentManager>
    {
        public IPLCEquipment PlcManager; //这个只是给BS相关设备用的，主H986依然是用 PLCControllerManager
        /// <summary>
        /// 命令字典
        /// </summary>
        public Dictionary<Command, string> MouduleCommandDic = new Dictionary<Command, string>(); //同上
        public PLCDBStatus plcValusStatus
        {
            get
            {
                if (Common.controlVersion != ControlVersion.BS & ConfigServices.GetInstance().localConfigModel.IsUserBS)
                {
                    return BSPLCControllerManager.GetInstance().PLCDBStatus;
                }
                else
                {
                    return PLCControllerManager.GetInstance().PLCDBStatus;
                }
            }
        }

        public List<bool> BSGlobalRetStatus
        {
            get
            {
                if (Common.controlVersion != ControlVersion.BS & ConfigServices.GetInstance().localConfigModel.IsUserBS)
                {
                    return BSPLCControllerManager.GetInstance().BSGlobalRetStatus;
                }
                else
                {
                   return Common.GlobalRetStatus;
                }
            }
        }
        public List<ushort> BSGlobalDoseStatus
        {
            get
            {
                if (Common.controlVersion != ControlVersion.BS & ConfigServices.GetInstance().localConfigModel.IsUserBS)
                {
                    return BSPLCControllerManager.GetInstance().BSGlobalDoseStatus;
                }
                else
                {
                    return Common.GlobalDoseStatus;
                }
            }
        }

        public void Start()
        {
            AccelatorService.GetInstance().Start();
            DoseServices.Service.Start();
            PlcService.GetInstance().Start();
            if(Common.controlVersion != ControlVersion.BS)
            {
                DetectorService.GetInstance().Start();
            }
            PlcManager = PLCControllerManager.GetInstance();
            MouduleCommandDic = Common.CommandDic;
            if (Common.controlVersion != ControlVersion.BS & ConfigServices.GetInstance().localConfigModel.IsUserBS)
            {
                string BSIpaddredd = ConfigServices.GetInstance().localConfigModel.BSAddress;
                string BSPort = ConfigServices.GetInstance().localConfigModel.BSPort.ToString();

                BSPlcService.GetInstance().SetIpAddress(BSIpaddredd, BSPort);
                BSPlcService.GetInstance().Start();
                PlcManager = BSPLCControllerManager.GetInstance();
                MouduleCommandDic = BSPLCControllerManager.GetInstance().BSCommandDic;
            }
            BegoodServerController.GetInstance().Load(new CIRProtocol());
            ServerLogsServices.Service.Start();
            CirServices.Service.Start();
            Task.Run(() => {
                RFIDServices.GetInstance().Start();
                RFIDServices.GetInstance().CarriageReachEvent += EquipmentManager_CarriageReachEvent;
            });
        }

        private void EquipmentManager_CarriageReachEvent(Carriage Carriage)
        {
            RFIDData RfidData = new RFIDData()
            {
                OrderType = UploadImageOrderType.RFID001,
                Token = ConfigServices.GetInstance().localConfigModel.Login.sccessToken,
                Data = CommonFunc.ObjectToJson(Carriage)
            };
            string UploadRfid = CommonFunc.ObjectToJson(RfidData);
            BGLogs.Log.GetDistance().WriteInfoLogs($@"UploadRfid:{UploadRfid}");
            if(ConfigServices.GetInstance().localConfigModel.IsEnabledSocketToServer)
            {
                BegoodServerController.GetInstance().SendRFIDInfo(CommonFunc.ObjectToJson(Carriage));
            }
            else
            {
                UploadWebServiceControl.GetInstance().CreateWebServicesControl(UploadRFIDServices.GetInstance()).UploadData(UploadRfid);
            }
        }

        public void Stop()
        {
            AccelatorService.GetInstance().Stop();
            if (Common.controlVersion != ControlVersion.BS)
            {
                DetectorService.GetInstance().Stop();
            }
            DoseServices.Service.Stop();
            PlcService.GetInstance().Stop();
            if (Common.controlVersion != ControlVersion.BS && ConfigServices.GetInstance().localConfigModel.IsUserBS)
            {
                BSPlcService.GetInstance().Stop();
            }
            PLCControllerManager.GetInstance().UnLoad();
            ServerLogsServices.Service.Stop();
            RFIDServices.GetInstance().Stop();
        }
    }
}
