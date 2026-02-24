using BG_WorkFlow;
using BGCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BGCommunication.CarriageIdentificationRFIDProtocol;

namespace BG_Services
{
    public class BegoodRFIDEquipment: BaseRFIDEquipment
    {
        public override event CarriageReach CarriageReachEvent;
        CarriageIdentificationRFIDProtocol carriageIdentificationRFIDProtocol;
        public override bool Connect()
        {
            carriageIdentificationRFIDProtocol = new CarriageIdentificationRFIDProtocol();
            carriageIdentificationRFIDProtocol.WaitRecTick = 100;
            ConnectInterface client;
            client = new TCPClient(ConfigServices.GetInstance().localConfigModel.RFIDAddress, (int)ConfigServices.GetInstance().localConfigModel.RFIDPort, false, false);
            carriageIdentificationRFIDProtocol.CarriageReachEvent += CarriageIdentificationRFIDProtocol_CarriageReachEvent; ;
            carriageIdentificationRFIDProtocol.InitConnection(ref client);
            return carriageIdentificationRFIDProtocol.Connect();
        }

        private void CarriageIdentificationRFIDProtocol_CarriageReachEvent(Carriage Carriage)
        {
            CarriageReachEvent?.Invoke(Carriage);
        }

        public override void DisConnect()
        {
            carriageIdentificationRFIDProtocol?.DisConnect();
        }

        public override bool IsConnect()
        {
            if (carriageIdentificationRFIDProtocol == null) return false;
            return carriageIdentificationRFIDProtocol.IsConnect();
        }
    }
}
