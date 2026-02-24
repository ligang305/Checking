using BG_Entities;
using BGModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG_Services
{
    public class PLCTools : BaseInstance<PLCTools>
    {
        /// <summary>
        /// 将Status转为美剧对象
        /// </summary>
        /// <param name="statusModels"></param>
        public Dictionary<PLCPositionEnum, int> StatusModelConvertTo(List<StatusModel> statusModels)
        {
            Dictionary<PLCPositionEnum, int> PlcPositionEnum = new Dictionary<PLCPositionEnum, int>();
            foreach (var item in statusModels)
            {
                if (string.IsNullOrEmpty(item.StatusName)) continue;
                try
                {
                    PLCPositionEnum pe = (PLCPositionEnum)Enum.Parse(typeof(PLCPositionEnum), item.StatusName.Replace(" ", ""), false);
                    PlcPositionEnum[pe] = Convert.ToInt32(item.StatusIndex);
                }
                catch (Exception ex)
                {
                    
                }
            }
            return PlcPositionEnum;
        }

        public Tuple<byte, int, int> GetBlockType(string address)
        {
            Tuple<byte, int, int> result;//= new Tuple<byte, int, ushort>(0x00,0,0);
            byte Item1 = 0x00;
            int Position = 0;
            int Offset = 0;
            if (address[0] == 'I')
            {
                Item1 = 0x81;
                Position = CalculateAddressStarted(address.Substring(1));
            }
            else if (address[0] == 'Q')
            {
                Item1 = 0x82;
                Position = CalculateAddressStarted(address.Substring(1));
            }
            else if (address[0] == 'M')
            {
                Item1 = 0x83;
                Position = CalculateAddressStarted(address.Substring(1));
            }
            else if (address[0] == 'D' || address.Substring(0, 2) == "DB")
            {
                Item1 = 0x84;
                string[] adds = address.Split('.');
                if (address[1] == 'B')
                {
                    Offset = Convert.ToUInt16(adds[0].Substring(2));
                }
                else
                {
                    Offset = Convert.ToUInt16(adds[0].Substring(1));
                }

                Position = CalculateAddressStarted(address.Substring(address.IndexOf('.') + 1));
            }
            else if (address[0] == 'T')
            {
                Item1 = 0x1D;
                Position = CalculateAddressStarted(address.Substring(1));
            }
            else if (address[0] == 'C')
            {
                Item1 = 0x1C;
                Position = CalculateAddressStarted(address.Substring(1));
            }
            else if (address[0] == 'V')
            {
                Item1 = 0x84;
                Offset = 1;
                Position = CalculateAddressStarted(address.Substring(1));
            }
            else
            {
                Item1 = 0;
                Position = 0;
                Offset = 0;
            }
            result = new Tuple<byte, int, int>(Item1, Offset, Position);
            return result;
        }
        /// <summary>
        /// 计算特殊的地址信息 -> Calculate Special Address information
        /// </summary>
        /// <param name="address">字符串地址 -> String address</param>
        /// <returns>实际值 -> Actual value</returns>
        public int CalculateAddressStarted(string address)
        {
            if (address.IndexOf('.') < 0)
            {
                return Convert.ToInt32(address) * 8;
            }
            else
            {
                string[] temp = address.Split('.');
                return Convert.ToInt32(temp[0]) * 8 + Convert.ToInt32(temp[1]);
            }
        }

        public Tuple<byte, int, int> GetBlockTypeForCPlusPlus(string address)
        {
            Tuple<byte, int, int> result;
            byte Item1 = 0x00;
            int Position = 0;// address.IndexOf('.') < 0 ? Convert.ToInt32(address.Substring(1)) : Convert.ToInt32(address.Substring(1).Split('.')[0]);
            int Offset = 0;// address.IndexOf('.') < 0 ? 0 : Convert.ToInt32(address.Substring(1).Split('.')[1]);
            if (address[0] == 'I')
            {
                Item1 = 0x81;
                Position = address.IndexOf('.') < 0 ? Convert.ToInt32(address.Substring(1)) : Convert.ToInt32(address.Substring(1).Split('.')[0]);
                Offset = address.IndexOf('.') < 0 ? 0 : Convert.ToInt32(address.Substring(1).Split('.')[1]);
            }
            else if (address[0] == 'Q')
            {
                Item1 = 0x82;
                Position = address.IndexOf('.') < 0 ? Convert.ToInt32(address.Substring(1)) : Convert.ToInt32(address.Substring(1).Split('.')[0]);
                Offset = address.IndexOf('.') < 0 ? 0 : Convert.ToInt32(address.Substring(1).Split('.')[1]);
            }
            else if (address[0] == 'M')
            {
                Item1 = 0x83;
                Position = address.IndexOf('.') < 0 ? Convert.ToInt32(address.Substring(1)) : Convert.ToInt32(address.Substring(1).Split('.')[0]);
                Offset = address.IndexOf('.') < 0 ? 0 : Convert.ToInt32(address.Substring(1).Split('.')[1]);
            }
            else if (address[0] == 'D' || address.Substring(0, 2) == "DB")
            {
                Item1 = 0x84;
                string[] adds = address.Split('.');
                if (address[1] == 'B')
                {
                    Position = address.IndexOf('.') < 0 ? Convert.ToInt32(address.Substring(2)) : Convert.ToInt32(address.Substring(2).Split('.')[0]);
                    Offset = address.IndexOf('.') < 0 ? 0 : Convert.ToInt32(address.Substring(2).Split('.')[1]) * 8;
                    if(adds.Length == 3)
                    {
                        Offset += Convert.ToInt32(address.Substring(2).Split('.')[2]);
                    }
                }
                else
                {
                    Position = address.IndexOf('.') < 0 ? Convert.ToInt32(address.Substring(2)) : Convert.ToInt32(address.Substring(2).Split('.')[0]);
                    Offset = address.IndexOf('.') < 0 ? 0 : Convert.ToInt32(address.Substring(2).Split('.')[1]) * 8;
                    if (adds.Length == 3)
                    {
                        Offset += Convert.ToInt32(address.Substring(2).Split('.')[2]);
                    }
                }
            }
            else if (address[0] == 'T')
            {
                Item1 = 0x1D;
                Position = address.IndexOf('.') < 0 ? Convert.ToInt32(address.Substring(2)) : Convert.ToInt32(address.Substring(2).Split('.')[0]);
                Offset = address.IndexOf('.') < 0 ? 0 : Convert.ToInt32(address.Substring(2).Split('.')[1]);
            }
            else if (address[0] == 'C')
            {
                Item1 = 0x1C;
                Position = address.IndexOf('.') < 0 ? Convert.ToInt32(address.Substring(2)) : Convert.ToInt32(address.Substring(2).Split('.')[0]);
                Offset = address.IndexOf('.') < 0 ? 0 : Convert.ToInt32(address.Substring(2).Split('.')[1]);
            }
            else if (address[0] == 'V')
            {
                Item1 = 0x84;
                Offset = 1;
                Position = address.IndexOf('.') < 0 ? Convert.ToInt32(address.Substring(2)) : Convert.ToInt32(address.Substring(2).Split('.')[0]);
            }
            else
            {
                Item1 = 0x83;
                Position = address.IndexOf('.') < 0 ? Convert.ToInt32(address) : Convert.ToInt32(address.Split('.')[0]);
                Offset = address.IndexOf('.') < 0 ? 0 : Convert.ToInt32(address.Split('.')[1]);
            }
            result = new Tuple<byte, int, int>(Item1, Position, Offset);
            return result;
        }

    }
}
