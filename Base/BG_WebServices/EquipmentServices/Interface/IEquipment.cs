using BGCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG_Services
{
    public interface IEquipment
    {
      
        /// <summary>
        /// 加速器控制器加载
        /// </summary>
        /// <param name="commonProtocol"></param>
        void Load(ICommonProtocol commonProtocol);
        /// <summary>
        /// 应用于Begood自主加速器
        /// </summary>
        void Load();
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
        /// 是否出束
        /// </summary>
        /// <returns></returns>
        bool IsRayOut();
        /// <summary>
        /// 加速器是否就绪
        /// </summary>
        /// <returns></returns>
        bool IsReady();
        /// <summary>
        /// 出束
        /// </summary>
        void Ray();
        /// <summary>
        /// 停止出束
        /// </summary>
        void StopRay();
        /// <summary>
        /// 预热中
        /// </summary>
        bool WarmUp();
        /// <summary>
        /// 预热毕
        /// </summary>
        bool WarmUpEnd();
        /// <summary>
        /// 查询预热状态
        /// </summary>
        /// <returns></returns>
        string GetRayAndPreviewHot();
        /// <summary>
        /// 读取高低能
        /// </summary>
        /// <returns></returns>
        string ReadDoubleOrSingle();
        /// <summary>
        /// 读取能量
        /// </summary>
        /// <returns></returns>
        string ReadEnger();
        /// <summary>
        /// 读取扫描模式
        /// </summary>
        /// <returns></returns>
        string SearchScanMode();
        /// <summary>
        /// 复位
        /// </summary>
        /// <returns></returns>
        bool Reset();
        /// <summary>
        /// 查询状态
        /// </summary>
        void Inquire();
        /// <summary>
        /// 设置电流
        /// </summary>
        /// <param name="CurrentFlows"></param>
        /// <returns></returns>
        bool SetCurrentFlows(double CurrentFlows);
        /// <summary>
        /// 设置高能脉冲
        /// </summary>
        /// <param name="HLC"></param>
        /// <returns></returns>
        bool SetHMC(double HLC);
        /// <summary>
        /// 设置低能脉冲
        /// </summary>
        /// <param name="LMC"></param>
        /// <returns></returns>
        bool SetLMC(double LMC);
        /// <summary>
        /// 设置高能脉冲个数
        /// </summary>
        /// <param name="LMCNum"></param>
        /// <returns></returns>
        bool SetHMCNum(double LMCNum);
        /// <summary>
        /// 设置低能脉冲个数
        /// </summary>
        /// <param name="LMCNum"></param>
        /// <returns></returns>
        bool SetLMCNum(double LMCNum);
        /// <summary>
        /// 设置高低能
        /// </summary>
        /// <param name="SetOrCancel">true--高能；false--低能</param>
        /// <returns></returns>
        bool SetHighOrLowEnergy(bool SetOrCancel);
        /// <summary>
        /// 设置单双能
        /// </summary>
        /// <param name="SetOrCancel">true --双能；false--单能</param>
        /// <returns></returns>
        bool SetDoubleOrSigneEnergy(bool SetOrCancel);
        /// <summary>
        /// 设置高低压
        /// </summary>
        /// <param name="SetHightOrLowPressure">true 为高压，false 为低压</param>
        /// <returns></returns>
        bool SetHighAndLowPressure(bool SetHightOrLowPressure);

        /// <summary>
        /// 设置工作模式
        /// </summary>
        /// <param name="workMode"></param>
        /// <returns></returns>
        bool SendCommandMainMagWorkFreMode(ushort workMode);
        /// <summary>
        /// 设置内部剂量
        /// </summary>
        /// <param name="workMode"></param>
        /// <returns></returns>
        bool SendCommandDoseInternal(ushort doseInternal);

        bool SendCommandWaitWorkStop();

        bool SendCommandAfterRayWorkStatus(ushort RayAfterStatus);

        bool SendCommandRadiationOn();

        /// <summary>
        /// 设置单双能
        /// </summary>
        /// <returns></returns>
        bool SendCommandEnergyMode(ushort EnergyMode);
        /// <summary>
        /// 设置注入电流
        /// </summary>
        /// <param name="inject"></param>
        /// <returns></returns>
        bool ExecuteInjection(ushort inject);
        /// <summary>
        /// 设置曝光时间
        /// </summary>
        /// <param name="ExposureTime"></param>
        /// <returns></returns>
        bool SendCommandExposureTime(ushort ExposureTime);

        /// <summary>
        ///切换单双能并设置高低能脉冲
        /// </summary>
        /// <param name="CheckOrNot"></param>
        /// <param name="isShowMessageBox"></param>
        void SwitchEngerHAndI(bool CheckOrNot, bool isShowMessageBox = true, bool isSetEnergy = true);

        /// <summary>
        /// 获取IGBT温度
        /// </summary>
        /// <returns></returns>
        string GetIGBTTemp();
        /// <summary>
        /// 获取晶闸管温度
        /// </summary>
        /// <returns></returns>
        string GetThyristor();
        /// <summary>
        /// 获取电流
        /// </summary>
        /// <returns></returns>
        string GetActureInject();
        /// <summary>
        /// 获取脉冲转换器温度
        /// </summary>
        /// <returns></returns>
        string GetPulseConverterTemperature();
        /// <summary>
        /// 获取辐射器温度
        /// </summary>
        /// <returns></returns>
        string GetRadiatorTemperature();

        /// <summary>
        /// 获取灯丝DAC值
        /// </summary>
        /// <returns></returns>
        string GetDACValueOfFilament();
        /// <summary>
        /// 获取注入DAC值
        /// </summary>
        /// <returns></returns>
        string GetInjectDACValue();
        /// <summary>
        /// 获取约束器DAC值
        /// </summary>
        /// <returns></returns>
        string GetConstrainerDACValue();
        /// <summary>
        /// 最大剂量搜索进度
        /// </summary>
        /// <returns></returns>
        string MaximumDoseRateSearchRrogress();
        /// <summary>
        /// 剂量率
        /// </summary>
        /// <returns></returns>
        string DoseRate();
        TandI GetTandI();
        WorkingJob GetWorkingJob();
        string GetCurrentEquipmentVersion();
        string GetCurrentEquipmentModel();
    }
}
