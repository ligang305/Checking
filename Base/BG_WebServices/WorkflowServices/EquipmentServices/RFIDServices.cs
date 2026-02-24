using BG_Entities;
using BG_WorkFlow;
using BGCommunication;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static BGCommunication.CarriageIdentificationRFIDProtocol;
using static CMW.Common.Utilities.CommonDeleget;
namespace BG_Services
{
    public class RFIDServices : BaseInstance<RFIDServices>
    {
        BaseRFIDEquipment BaseRFIDEquipment;
        public event CarriageReach CarriageReachEvent;
        string RFIDAddress = string.Empty;
        int RFIDPort = 0;
        public Dictionary<int,Carriage> CarriagesList = new Dictionary< int, Carriage>();
        public RFIDServices()
        {
            BaseRFIDEquipment = new BegoodRFIDEquipment();
            BaseRFIDEquipment.CarriageReachEvent += BaseRFIDEquipment_CarriageReachEvent;
            RFIDAddress = ConfigServices.GetInstance().localConfigModel.RFIDAddress;
            RFIDPort = (int)ConfigServices.GetInstance().localConfigModel.RFIDPort;
            Task.Factory.StartNew(new Action(ConnectionRFID));
        }

        private void BaseRFIDEquipment_CarriageReachEvent(Carriage Carriage)
        {
            if(Carriage.CarriageNo == 1)
            {
                CarriagesList.Clear();
            }
            if (!CarriagesList.Keys.Contains(Carriage.CarriageNo))
            {
                CarriagesList[Carriage.CarriageNo] = Carriage;
            }
            Task.Run(() => {
                CarriageReachEvent?.Invoke(Carriage);
            });
        }

        public void Start()
        {
            BaseRFIDEquipment.Connect();
        }
        //服务终止
        public void Stop()
        {
            BaseRFIDEquipment.DisConnect();
        }

        public void ConnectionRFID()
        {
            while (Common.isExcuteTask)
            {
                try
                {
                    Thread.Sleep(100);
                    if (!CommonFunc.PingIp(RFIDAddress))
                    {
                        continue;
                    }
                    //如果未连接
                    if (!BaseRFIDEquipment.IsConnect())
                    {
                        BaseRFIDEquipment.Connect();
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    CommonDeleget.HandTaskException(ex);
                }
            }
        }

        public string GetCarriageInfo(ushort CarriageNo)
        {
            try
            {
                int _CarriageNo = Convert.ToInt32(CarriageNo);
                if(CarriagesList.Keys.Contains(_CarriageNo))
                {
                    return CarriagesList[_CarriageNo].TrainPropety;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}
