using BGCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG_Services
{
    public interface IDetectorEquipment
    {
        /// <summary>
        /// 探测器
        /// </summary>
        /// <param name="commonProtocol"></param>
        void Load(string IpAddress, string Port, string CommandPort);
        /// <summary>
        /// 探测器
        /// </summary>
        /// <param name="commonProtocol"></param>
        void Load(DetectorEquipmenntEnum DetectorEquipmennt,string IpAddress,string Port,string CommandPort);

        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        bool Connection();

        /// <summary>
        /// 判断是否链接
        /// </summary>
        /// <returns></returns>
        bool IsConnection();
        /// <summary>
        /// 要断开链接
        /// </summary>
        void DisConnection();
        /// <summary>
        /// 设置单双能
        /// </summary>
        void SX_SetEnergy(int energy);
    }
}
