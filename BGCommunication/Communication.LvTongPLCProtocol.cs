using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGCommunication
{
    public class LvTongPLCProtocol: PLCProtocol
    {
        public LvTongPLCProtocol()
        {
            ProtocolName = "控制站-绿通PLC协议";
            PLCConnectType = ConnectType.CT_DirectConnect;
        }

        protected override void InitSystemTypeParas()
        {
            RetStatus.Clear();
            Flags.Clear();
            
            int[] status = new int[] { 4, 8, 16, 56, 57, 59, 70, 71 };
            RetStatus.Capacity = 88;
            Flags = new byte[] { 0xAF, 0xB1, 0xB3, 0xB5, 0xB6, 0xB7, 0xB8, 0xB9, 0xBE, 0xC8 }.ToList();
            for (int i = 0; i < RetStatus.Capacity; i++)
            {
                RetStatus.Add(false);
            }
            Flags.TrimExcess();
        }
    }
}
