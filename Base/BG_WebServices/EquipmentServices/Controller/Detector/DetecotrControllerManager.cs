using BG_Entities;
using BG_WorkFlow;
using BGCommunication;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.ImageImportDll;
namespace BG_Services
{
    public class DetecotrControllerManager : BaseInstance<DetecotrControllerManager>, IDetectorEquipment
    {
        public Action ReflashImageAction;
        public Action<int> DetecotorConnectionAction;
        public int DetectorConnection;

        public Detector detectorEquipment;
        public bool Connection()
        {
            return detectorEquipment == null ? false: detectorEquipment.Connection();
        }
        public bool Connection(string IPAdress, short port, short ImagePort)
        {
            return detectorEquipment == null ? false : detectorEquipment.Connection(IPAdress, port, ImagePort);
        }
        public void DisConnection()
        {
            detectorEquipment?.DisConnection();
        }

        public bool IsConnection()
        {
            return detectorEquipment.IsConnection();
        }

        public void Load(DetectorEquipmenntEnum DetectorEquipmennt, string IpAddress, string Port, string CommandPort)
        {
            switch (DetectorEquipmennt)
            {
                case DetectorEquipmenntEnum.BegoodDetector:
                    detectorEquipment = new BegoodDetector();
                    detectorEquipment.ReflashImageAction -= ReflashImageEvent;
                    detectorEquipment.ReflashImageAction += ReflashImageEvent;
                    detectorEquipment.DetecotorConnectionAction -= DetecotorConnectionEvent;
                    detectorEquipment.DetecotorConnectionAction += DetecotorConnectionEvent;
                    Load(IpAddress, Port, CommandPort);
                    break;
                case DetectorEquipmenntEnum.BSBegoodDetector:
                    detectorEquipment = new BegoodBSDetector();
                    detectorEquipment.ReflashImageAction -= ReflashImageEvent;
                    detectorEquipment.ReflashImageAction += ReflashImageEvent;
                    detectorEquipment.DetecotorConnectionAction -= DetecotorConnectionEvent;
                    detectorEquipment.DetecotorConnectionAction += DetecotorConnectionEvent;
                    Load(IpAddress, Port, CommandPort);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 载入
        /// </summary>
        public void Load(string IpAddress, string Port, string CommandPort)
        {
            detectorEquipment?.Load(IpAddress, Port, CommandPort);
        }
        /// <summary>
        /// 设置单双能
        /// </summary>
        public void SX_SetEnergy(int energy)
        {
            DetecotorConnectionEvent(4);
            detectorEquipment?.SX_SetEnergy(energy);
        }
        /// <summary>
        /// 设置频率
        /// </summary>
        public void SX_SetFrequency(int Frequency)
        {
            detectorEquipment?.SX_SetFrequency(Frequency);
        }
        /// <summary>
        /// 刷新图像
        /// </summary>
        public void ReflashImageEvent()
        {
            ReflashImageAction?.Invoke();
        }

        /// <summary>
        /// 探测器连接回调
        /// </summary>
        public void DetecotorConnectionEvent(int Connection)
        {
            DetectorConnection = Connection;
            DetecotorConnectionAction?.Invoke(Connection);
        }
        public void SX_SetFrequencyBySpeed(ushort carspeed)
        {
            var TempFreez = CarSpeedBLL.GetInstance().GetFreezUseExpression(carspeed);
            CommonDeleget.WriteLogAction($"DyFrequency： 【Frequency：{TempFreez}，CarSpeed：{carspeed}】", LogType.NormalLog, true);
            ConfigServices.GetInstance().localConfigModel.Freeze = TempFreez.ToString();
            //TempFreez = PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.ScanMode) ? 400 : TempFreez;
            SX_SetFrequency(TempFreez);
            Task.Run(() =>
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                CommonDeleget.UpdateConfigs("Frequency", TempFreez.ToString(), Section.SOFT);
                stopWatch.Stop();
                CommonDeleget.WriteLogAction($"CommonDeleget.UpdateConfigs({stopWatch.ElapsedMilliseconds}", LogType.NormalLog, true);
            });
        }

        /// <summary>
        /// 开始扫描
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        public int SX_Start()
        {
            return detectorEquipment.SX_Start();
        }
        /// <summary>
        /// 停止扫描
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        public int SX_Stop()
        {
            return detectorEquipment.SX_Stop();
        }

        /**
        * @brief            获取探测器错误码
        * @param[in]        handle          扫描句柄
        * @return           错误码成功：0；失败-1
        */
        public  int SX_GetLastError()
        {
            return detectorEquipment.SX_GetLastError();
        }
        /**
         * @brief            获取探测器办卡号
         * @param[in]        handle          扫描句柄
         * @return           错误码
         */
        public  int SX_GetBoardNumber(out int number, int BoardLine)
        {
            return detectorEquipment.SX_GetBoardNumber(out number, BoardLine);
        }
        /// <summary>
        /// 设置车速
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="frequency"></param>
        /// <returns>0：成功 -1：失败</returns>
        public int SX_SetSpeed(int speed)
        {
            return detectorEquipment.SX_SetSpeed(speed);
        }
        /// <summary>
        /// 设置扫描模式
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        public int SX_SetMode(int mode)
        {
            return detectorEquipment.SX_SetMode(mode);
        }
        /// <summary>
        /// 设置扫描方向
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        public int SX_SetDirection(int Direction)
        {
            return detectorEquipment.SX_SetDirection(Direction);
        }
        /**
        * @brief            关闭文件句柄
        * @param[in]        handle          文件句柄
        * @return           成功：0；失败：-1
        */
        public int IX_Close()
        {
            return detectorEquipment.IX_Close();
        }

        /**
        * @brief            释放图像句柄
        * @param[in]        himage          图像句柄
        * @return           成功：0；失败：-1
        */
        public  int IX_Release(IntPtr ImagePtr)
        {
            return detectorEquipment.IX_Release(ImagePtr);
        }
        /**
        * @brief            生成高能物质识别文件（HEMD）
        * @param[in]        handle          文件句柄
        * @param[in]        path            高能物质识别文件路径
        * @return           成功：0；失败：-1
        */
        public int IX_Hemd(IntPtr handle, string path)
        {
            return detectorEquipment.IX_Hemd(handle, path);
        }


        /**
        * @brief            获取探测器板卡错误
        * @param[in]        handle          扫描句柄
        * @param[in]        number          板卡数量
        * @param[in]        board           出错板链序号
        * @param[in]        index           出错板卡序号
        * @return           成功：0；失败：-1
        */
        public int SX_GetBoardError(out int number, out int board, out int index)
        {
            return detectorEquipment.SX_GetBoardError(out number,out board,out index);
        }

        /**
        * @brief            初始化物质表颜色表
        * @param[in]        matex           物质表文件路径
        * @param[in]        color           颜色表文件路径
        * @return           成功：0；失败：-1
        */
        public int IX_Init(string matex, string color)
        {
            return detectorEquipment.IX_Init(matex, color);
        }
        /**
        * @brief            设置物质识别滤波参数
        * @param[in]        filter          滤波参数：0=中值5，1=均值5，2=均值11
        * @return           成功：0；失败：-1
        */
        public int IX_SetFilter(int matex)
        {
            return detectorEquipment.IX_SetFilter(matex);
        }
    }
}
