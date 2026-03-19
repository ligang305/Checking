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
    [CustomExportMetadata(1, "FastCheck", "CGN. Begood", "zhuzhiwu", "1.0.0")]
    public class KuaiJianPLCProtocol: PLCV400Protocol
    {
        public KuaiJianPLCProtocol()
        {
            ProtocolName = "S7-1200";
            PLCConnectType = ConnectType.CT_DirectConnect;
        }

        protected override void InitSystemTypeParas()
        {
            RetStatus.Clear();
            Flags.Clear();

            int[] status = new int[] { 3, 8, 9, 12, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
                            30,33,34,35,36,38,39,45,47,48,49,61,62,63,70,71 };
            RetStatus.Capacity = 128;
            Flags = new byte[] { 0xAF, 0xB0, 0xB1, 0xB3, 0xB4, 0xB5, 0xB6, 0xB7, 0xBA, 0xBB, 0xBC, 0xBE, 0xC7, 0xC8, 0xCB, 0xCC, 0xCE, 0xD3, 0xDB, 0xDC, 0xDD, 0xDE, 0xDF }.ToList();

            for (int i = 0; i < RetStatus.Capacity; i++)
            {
                RetStatus.Add(false);
            }
            Flags.TrimExcess();
        }
    }
}
