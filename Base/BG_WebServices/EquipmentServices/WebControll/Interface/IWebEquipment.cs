using BGCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG_Services
{
    public interface IWebEquipment
    {
        /// <summary>
        /// 加载Web,Socket的通讯类
        /// </summary>
        /// <param name="commonProtocol"></param>
        void Load(CIRProtocol commonProtocol);
        
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
        /// 发送上传图片信息
        /// </summary>
        /// <returns></returns>
        bool SendUploadImageInfo(string ImageInfo);
        /// <summary>
        /// 接收服务端信息
        /// </summary>
        void ReceiveServerData(byte[] ServerData);
        /// <summary>
        /// 发送设备状态信息
        /// </summary>
        void SendEquipmentInfo();
    }
}
