using BG_Entities;
using BGCommunication;
using BGModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace BG_Services
{
    /// <summary>
    /// PLC设备
    /// </summary>
    public interface IPLCEquipment
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="cv"></param>
        /// <param name="commonProtocol"></param>
        void Load(ControlVersion cv, PLCProtocol commonProtocol);
        /// <summary>
        /// 卸载
        /// </summary>
        void UnLoad();
        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        bool Connect(string IpAddress, string Port);
        /// <summary>
        /// 判断是否在线
        /// </summary>
        /// <returns></returns>
        bool IsConnect();
        /// <summary>
        /// 断开连接
        /// </summary>
        void DisConnect();
        /// <summary>
        /// 批量查询状态
        /// </summary>
        /// <returns></returns>
        bool InquirePositionStatus();
        /// <summary>
        /// 查询硬件状态
        /// </summary>
        /// <returns></returns>
        bool InquireHardwareStatus();
        /// <summary>
        ///  设置点位的值
        /// </summary>
        /// <returns></returns>
        bool WritePositionValue(string Position,bool Value);


        /// <summary>
        ///  设置点位的值
        /// </summary>
        /// <returns></returns>
        bool WritePositionValue(byte Position, ushort Value);

        /// <summary>
        ///  设置点位的值
        /// </summary>
        /// <returns></returns>
        bool WritePositionValue(string Position, ushort Value);
        /// <summary>
        ///  设置点位的值
        /// </summary>
        /// <returns></returns>
        bool WritePositionValue(byte Position, UInt32 Value);
        /// <summary>
        ///  设置点位的值
        /// </summary>
        /// <returns></returns>
        bool WritePositionValue(string Position, UInt32 Value);
        /// <summary>
        ///  设置点位的值
        /// </summary>
        /// <returns></returns>
        bool WritePositionValue(byte Position, float Value);
        /// <summary>
        ///  设置点位的值
        /// </summary>
        /// <returns></returns>
        bool WritePositionValue(string Position, float Value);
        /// <summary>
        /// 读取点位的值
        /// </summary>
        /// <param name="Postion">位子 M区、DB区等</param>
        /// <param name="StartPositon">起始位</param>
        /// <param name="length">长度位</param>
        /// <returns></returns>
        string ReadPositionValue(string Postion,uint StartPositon,uint length);
        /// <summary>
        /// 传入M26.2 
        /// </summary>
        /// <param name="positionItem"></param>
        /// <returns></returns>
        bool ReadPositionValue(string positionItem);
        /// <summary>
        /// 这个方法只管处理数据
        /// </summary>
        /// <returns></returns>
        bool HandleByteToBussiness();
        /// <summary>
        /// 这个方法只管接收数据
        /// </summary>
        /// <returns></returns>
        bool ReceviceByte();
        /// <summary>
        /// 获取PLC版本号
        /// </summary>
        /// <returns></returns>
        string GetCurrentEquipmentVersion();
        /// <summary>
        /// 获取PLC的型号
        /// </summary>
        /// <returns></returns>
        string GetCurrentEquipmentModel();
        /// <summary>
        /// 获取DB 区的string值
        /// </summary>
        /// <param name="BlockPosition"></param>
        /// <param name="Strlength"></param>
        /// <param name="_InqureType"></param>
        /// <returns></returns>
        string GetPlcBlockStr(string BlockPosition, ushort Strlength, InqureType _InqureType = InqureType.IString);
        /// <summary>
        /// 通过点位获取状态点位
        /// </summary>
        /// <param name="pLCPositionEnum"></param>
        /// <returns></returns>
        bool GetStatusByPositionEnum(PLCPositionEnum pLCPositionEnum);
        /// <summary>
        /// 通过点位非枚举值获取状态点位
        /// </summary>
        /// <param name="plcPositionEnuStr"></param>
        /// <returns></returns>
        bool GetStatusByPositionEnum(string plcPositionEnuStr);

        short GetStatusByDIntPositionEnum(PLCPositionEnum pLCPositionEnum);
        ///通过枚举获取点位状态
        short GetStatusByDIntPositionEnum(string plcPositionEnuStr);

        float GetStatusByFloatPositionEnum(PLCPositionEnum pLCPositionEnum);

        ///通过枚举获取点位状态
        float GetStatusByFloatPositionEnum(string plcPositionEnuStr);

        int GetStatusBUIntPositionEnum(PLCPositionEnum pLCPositionEnum);
        ///通过枚举获取点位状态
        int GetStatusBUIntPositionEnum(string plcPositionEnuStr);
        
        bool CheckSystemCondition();
    }
}
