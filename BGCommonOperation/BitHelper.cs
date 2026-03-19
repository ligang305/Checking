using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMW.Common.Utilities
{
    public class BitHelper
    {
        public static string ToHexString(byte[] bytes) // 0xae00cf => "AE00CF "
        {
            string hexString = string.Empty;
            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    strB.Append(bytes[i].ToString("X2"));
                    strB.Append(" ");
                }
                hexString = strB.ToString();
            }
            return hexString;
        }
        /// <summary>
        /// 获取字节中的指定Bit的值
        /// </summary>
        /// <param name="this">字节</param>
        /// <param name="index">Bit的索引值(0-7)</param>
        /// <returns></returns>
        public static int GetBit(byte data, short index)
        {
            byte x = 1;
            switch (index)
            {
                case 0: { x = 0x01; } break;
                case 1: { x = 0x02; } break;
                case 2: { x = 0x04; } break;
                case 3: { x = 0x08; } break;
                case 4: { x = 0x10; } break;
                case 5: { x = 0x20; } break;
                case 6: { x = 0x40; } break;
                case 7: { x = 0x80; } break;
                default: { return 0; }
            }
            return (data & x) == x ? 1 : 0;
        }

        /// <summary>
        /// 设置字节中的指定Bit的值
        /// </summary>
        /// <param name="this">字节</param>
        /// <param name="index">Bit的索引值(0-7)</param>
        /// <returns></returns>
        public static bool SetBit(ref byte data, int index,bool bitValue)
        {
            index = index + 1;
            if (index > 8 || index < 1)
            {
                throw new ArgumentOutOfRangeException();
            }
            int v = index < 2 ? index : (2 << (index - 2));
            if(bitValue)
            {
                data = (byte)(data | v);
            }
            else
            {
                data = (byte)(data & ~v);
            }
            return true;
        }
    }
}
