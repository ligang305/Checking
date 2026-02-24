using BG_Entities;
using BGCommunication;
using BGModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGPLC
{
    [Export("PLC", typeof(PLCProtocol))]
    [CustomExportMetadata(1, "CombinedMovementProtocol", "CGN. Begood", "zhuzhiwu", "1.0.0")]
    public class CombinedMovementProtocol : PLCSIEMENS_S300Protocol
    {
        public CombinedMovementProtocol()
        {
            PLCConnectType = ConnectType.CT_DirectConnect;
        }

        protected override void InitSystemTypeParas()
        {
            RetStatus.Clear();
            Flags.Clear();
            RetStatus.Capacity = 128;
            Flags = new byte[] {0xB6,  0xDF, 0xE0, 0xE1, 0xE2, 0xE3, 0xE4, 0xE5, 0xE6, 0xE8, 0xE9, 0xEA, 0xEB, 0xEC, 0xEE, 0xF9,  0xFB, 0xFC, 0x11, 0x0b,0x0c,0x0d,0x0e }.ToList();
            for (int i = 0; i < RetStatus.Capacity; i++)
            {
                RetStatus.Add(false);
            }
            Flags.TrimExcess();
        }
    }
}
