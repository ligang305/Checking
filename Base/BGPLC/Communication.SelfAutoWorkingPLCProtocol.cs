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
    [CustomExportMetadata(1, "SelfAutoWorking", "CGN. Begood", "zhuzhiwu", "1.0.0")]
    public class SelfAutoWorkingPLCProtocol : PLCV400Protocol
    {
        public SelfAutoWorkingPLCProtocol()
        {
            ProtocolName = "S7-1200";
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
            byte[] flags = new byte[] {  0xA8, 0xAA,0xAD,  0xAE,0xAF,0xB0,0xB1,0xB2,0xB3,
                                0xB4, 0xB5,0xB7,0xBE,0xBF,0xC0,0xC2,0xC3,
                                    0xC4,0xC5,0xC6,0xC7,0xC8,0xC9,0xCA
            ,0xCB ,0xCC ,0xCD ,0xCE ,0xCF ,0xD0,0xD2,0xD3,0xD4,0xD5,0xD6,0xD7,0xDA,0xE0,0xE1,0xE2,0xE3,0xE4,0xE5,0xE6};
           
            Flags = flags.ToList().Select(q => { q += 7; return q; }).ToList();
            for (int i = 0; i < RetStatus.Capacity; i++)
            {
                RetStatus.Add(false);
            }
            Flags.TrimExcess();
        }

    }
}
