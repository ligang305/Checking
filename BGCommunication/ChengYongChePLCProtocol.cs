using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGCommunication
{
    public class ChengYongChePLCProtocol : PLCProtocol
    {
        public ChengYongChePLCProtocol()
        {
            ProtocolName = "控制站-乘用车PLC协议";
            PLCConnectType = ConnectType.CT_DirectConnect;
        }

        protected override void InitSystemTypeParas()
        {
            ValidStatus.Clear();
            RetStatus.Clear();
            Flags.Clear();

            int[] status = new int[] { 0, 8, 12, 44, 45, 46, 56, 57, 70, 71 };
            ValidStatus = status.ToList();
            RetStatus.Capacity = 88;
            Flags = new byte[] { 0xAF,0xB3, 0xB4, 0xB5, 0xB6, 0xB7, 0xB8, 0xB9, 0xBE,0xC5, 0xC6, 0xC7, 0xC8,
                            0xD1,0xD2,0xD3,0xD4,0xD5,0xD6,0xD7,0xD8,0xD9,0xDA}.ToList();

            ValidStatus.TrimExcess();
            for (int i = 0; i < RetStatus.Capacity; i++)
            {
                RetStatus.Add(false);
            }
            Flags.TrimExcess();
        }
    }
}
