using BGCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BGModel;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.CommonFunc;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.ImageImportDll;
using BG_WorkFlow;
using CMW.Common.Utilities;
using BG_Entities;
using System.IO;

namespace BG_Services
{
    public class BegoodBSDetector : Detector
    {
        //先声明静态回调函数对象，否则该回调函数可能会被回收
        CALLBACK_STATUS CALLBACK_STATUS_obj = null;
        CALLBACK_UPDATE CALLBACK_UPDATE_obj = null;// new CALLBACK_UPDATE(ReflashImage_Callback);
        CALLBACK_SCAN CALLBACK_SCAN_obj = null; //new CALLBACK_SCAN(ScanImagePath_Callback);
     
        public BegoodBSDetector()
        {
            //先声明静态回调函数对象，否则该回调函数可能会被回收
             CALLBACK_STATUS_obj = new CALLBACK_STATUS(ConnectionStatus_Callback);
             CALLBACK_UPDATE_obj = new CALLBACK_UPDATE(ReflashImage_Callback);
             CALLBACK_SCAN_obj = new CALLBACK_SCAN(ScanImagePath_Callback);
        }

        public override bool Connection(string IPAdress, short port, short ImagePort)
        {
            return SX_Connect(IPAdress, port, ImagePort) == 0;
        }

        public override void DisConnection()
        {
            SX_Disconnect();
        }

        public override bool IsConnection()
        {
            return SingleDetectorConnection == 3;
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="commonProtocol"></param>
        public override void Load(string IPAdress, string port, string ImagePort)
        {
            if (DetectorIntPtr == IntPtr.Zero)
            {
                SX_Create();
                WriteLogAction($@"SX_Create IP:{IPAdress}_{port}_{ImagePort},DetectorIntPtr:{DetectorIntPtr.ToString()}", LogType.NormalLog);
            }
            string ImageFile = SystemDirectoryConfig.GetInstance().GetBSScanImageFile();
            string LogPath = $@"{SystemDirectoryConfig.AppDir}\Logs\";

            if(!Directory.Exists(ImageFile))
            {
                Directory.CreateDirectory(ImageFile);
            }
            if(!Directory.Exists($@"{ImageFile}{IPAdress}_{port}_{ImagePort}"))
            {
                Directory.CreateDirectory($@"{ImageFile}{IPAdress}_{port}_{ImagePort}");
            }
            if (!Directory.Exists(LogPath))
            {
                Directory.CreateDirectory(LogPath);
            }
    
            //设置配置文件
            SX_SetLog($@"{LogPath}{IPAdress}_{port}_{ImagePort}_{DateTime.Now.ToString("yyyy-MM-dd")}.xml", Convert.ToByte('V'));
            SX_SetPath($@"{ImageFile}{IPAdress}_{port}_{ImagePort}");

            SX_SetConfigFile(SystemDirectoryConfig.GetInstance().GetScanConfigFile($@"{IPAdress}_{port}"));//
            IX_Init(SystemDirectoryConfig.GetInstance().GetMatexLineFile(), 
                SystemDirectoryConfig.GetInstance().GetMatexColorFile());

            IX_SetFilter(0);
            //设置回调函数
            SX_SetStatusCallback(CALLBACK_STATUS_obj);
            //设置回调函数
            SX_SetScanCallback(CALLBACK_SCAN_obj);
            //设置回调函数
            SX_SetUpdateCallback(CALLBACK_UPDATE_obj);

            ///背散只有快检模式
            SX_SetMode(0);
            //向前
            if (PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.ScanForWard))
            {
                SX_SetDirection(0);
            }
            else
            {
                SX_SetDirection(1);
            }
        }
        public override int SX_SetSpeed(int Speed)
        {
            return ImageImportDll.SX_SetSpeed(DetectorIntPtr, Speed);
        }
        public override void SX_SetEnergy(int energy)
        {
            ImageImportDll.SX_SetEnergy(DetectorIntPtr, energy); 
        }
        public override void SX_SetFrequency(int Frequency)
        {
            ImageImportDll.SX_SetFrequency(DetectorIntPtr, Frequency);
        }
        #region 扫描站回调函数
        public void ConnectionStatus_Callback(int status, int error)
        {
            SingleDetectorConnection = status;
            if (error == -1)
            {
                WriteLogAction($"发生错误了：扫描站回调错误信息 error：{error}",LogType.NormalLog);
            }
            DetecotorConnectionAction?.Invoke(status);
        }
        //TODO 这里要加一个判断当前图片是否是当前扫描流程的第几张图，和是否是最后一张图
        /// <summary>
        /// 扫描状态回调
        /// </summary>
        /// <param name="status">是不是停止扫描回传过来的</param>
        /// <param name="carrid">当前扫描的车厢号</param>
        /// <param name="path">当前车厢扫描出来的路径</param>
        public void ScanImagePath_Callback(int status,int carrid, string path)
        {
            ScanImagePathAction?.Invoke(status, carrid, path);
        }
        /// <summary>
        /// 回调更新主界面的图片
        /// </summary>
        public void ReflashImage_Callback()
        {
            ReflashImageAction?.Invoke();
        }
        #endregion
    }
}
