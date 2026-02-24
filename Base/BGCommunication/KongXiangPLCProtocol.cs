using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGCommunication
{
    public class KongXiangPLCProtocol: PLCV400Protocol
    {
        public KongXiangPLCProtocol()
        {
            ProtocolName = "控制站-空箱PLC协议";
            PLCConnectType = ConnectType.CT_DirectConnect;
        }

        protected override void InitSystemTypeParas()
        {
            ValidStatus.Clear();
            RetStatus.Clear();
            Flags.Clear();

            int[] status = new int[] { 5, 8, 9, 12, 17, 18, 19, 56, 57, 59, 70, 71 };
            ValidStatus = status.ToList();
            RetStatus.Capacity = 88;
            Flags = new byte[] { 0xAF, 0xB1, 0xB5, 0xB6, 0xBE, 0xC8 }.ToList();

            ValidStatus.TrimExcess();
            for (int i = 0; i < RetStatus.Capacity; i++)
            {
                RetStatus.Add(false);
            }
            Flags.TrimExcess();
        }
    }
}
