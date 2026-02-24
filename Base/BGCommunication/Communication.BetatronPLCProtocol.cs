using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGCommunication
{
    public class BetatronPLCProtocol: PLCProtocol
    {
        public BetatronPLCProtocol()
        {
            ProtocolName = "Control station -Betatron combined mobile  protocol";
            PLCConnectType = ConnectType.CT_DirectConnect;
        }

        protected override void InitSystemTypeParas()
        {
            RetStatus.Clear();
            Flags.Clear();

            int[] status = new int[] { 2, 8, 10, 12, 18, 45, 47, 48, 49, 56, 57, 61, 62, 63, 65, 66, 68, 69, 70, 71 };
            RetStatus.Capacity = 88;
            Flags = new byte[] { 0xAF, 0xB1, 0xB3, 0xB4, 0xB5, 0xB6, 0xB7, 0xB8, 0xB9, 0xBA, 0xBB, 0xBC, 0xBE, 0xC8, 0xDB, 0xE1 }.ToList();

            for (int i = 0; i < RetStatus.Capacity; i++)
            {
                RetStatus.Add(false);
            }
            Flags.TrimExcess();
        }
    }
}
