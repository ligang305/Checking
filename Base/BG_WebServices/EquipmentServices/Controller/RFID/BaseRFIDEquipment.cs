using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BGCommunication.CarriageIdentificationRFIDProtocol;

namespace BG_Services
{
    public abstract class BaseRFIDEquipment
    {
        public virtual event CarriageReach CarriageReachEvent;
        public virtual bool Connect()
        {
            throw new NotImplementedException();
        }

        public virtual void DisConnect()
        {
            throw new NotImplementedException();
        }

        public virtual bool IsConnect()
        {
            throw new NotImplementedException();
        }
    }
}
