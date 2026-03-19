using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGCommunication
{
    [Description("Chezai服务接口")]
    [Export(nameof(CheZaiPLCProtocol), typeof(PLCProtocol))]
    public class CheZaiPLCProtocol : PLCV500Protocol
    {
        public CheZaiPLCProtocol()
        {
            ProtocolName = "Control station-vehicle PLC protocol";
            PLCConnectType = ConnectType.CT_DirectConnect;
        }
        
        protected override void InitSystemTypeParas()
        {
            RetStatus.Clear();
            Flags.Clear();

            int[] status = new int[] { 7, 8, 9,10,11, 12, 16, 17, 18, 19, 21, 22, 23, 24, 25, 26, 27, 28, 29,
                            30,31,32,33,34,35,36,37,38,39,40,45,46,52,53,54,55,58,59,60,61,62,63,64,65,66,67,68,69,70,71,
                            72,73,74,75,76,77,84,86,87,88,89,90,91,92,93,94,95,96,97,98,99,100,101,102,103,104,105,106,107,
                            108,109,110,111,112,113,114,115,116,117,118,119,120,121,122,123,124,125,126,127};
            RetStatus.Capacity = 128;
            byte[] flags = new byte[] { 0xAF,0xB0, 0xB1,0xB2,0xB3,0xB4, 0xB5, 0xB6,0xBA,0xBB,0xBC,0xBD,
                                0xBE, 0xC9,0xCA,0xCB,0xCC,0xCD,0xCF,0xD0,0xD1,0xDB,0xDC,0xDD,0xDE,0xDF,0xE0,0xE1,0xE2,0xE3,0xE4,0xE5 };
            Flags = flags.ToList();

            for (int i = 0; i < RetStatus.Capacity; i++)
            {
                RetStatus.Add(false);
            }
            Flags.TrimExcess();
        }

    }
}
