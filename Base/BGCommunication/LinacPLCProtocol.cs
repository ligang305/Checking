using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGCommunication
{
    public class LinacPLCProtocol: PLCProtocol
    {
        public LinacPLCProtocol()
        {
            ProtocolName = "控制站-Linac组合移动PLC协议";
            PLCConnectType = ConnectType.CT_DirectConnect;
        }

        protected override void InitSystemTypeParas()
        {
            ValidStatus.Clear();
            RetStatus.Clear();
            Flags.Clear();

            int[] status = new int[] { 3, 8, 9, 12, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
                            30,33,34,35,36,38,39,45,48,49,61,62,63,70,71 };
            ValidStatus = status.ToList();
            RetStatus.Capacity = 88;
            Flags = new byte[] { 0xAF, 0xB0, 0xB1, 0xB3, 0xB4, 0xB5, 0xB6, 0xBA, 0xBB, 0xBC, 0xBE, 0xC8, 0xDB, 0xDC, 0xDD, 0xDE, 0xDF }.ToList();

            ValidStatus.TrimExcess();
            for (int i = 0; i < RetStatus.Capacity; i++)
            {
                RetStatus.Add(false);
            }
            Flags.TrimExcess();
        }
    }
}
