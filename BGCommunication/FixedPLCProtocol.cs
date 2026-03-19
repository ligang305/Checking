using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGCommunication
{
    public class FixedPLCProtocol: PLCProtocol
    {
        public FixedPLCProtocol()
        {
            ProtocolName = "控制站-固定式PLC协议";
            PLCConnectType = ConnectType.CT_DirectConnect;
        }

        protected override void InitSystemTypeParas()
        {
            ValidStatus.Clear();
            RetStatus.Clear();
            Flags.Clear();

            int[] status = new int[] { 1, 8, 9, 10, 12, 15, 16, 17, 18, 19,21,22,23,24,25,26,27,28,29,30,33,34,35,36,
                            38,39,40,45,61,62,63,65,66,67,68,69,70,71,72,73,74,75,76,77,78,79,81,82,83,84,85,86,87 };
            ValidStatus = status.ToList();
            RetStatus.Capacity = 88;
            Flags = new byte[] { 0xAF,0xB0, 0xB1,0xB3, 0xB4, 0xB5, 0xB6, 0xBA, 0xBB, 0xBC, 0xBE, 0xC8,
                            0xDB,0xDC,0xDD,0xDE,0xDF,0xE0 }.ToList();

            ValidStatus.TrimExcess();
            for (int i = 0; i < RetStatus.Capacity; i++)
            {
                RetStatus.Add(false);
            }
            Flags.TrimExcess();
        }
    }
}
