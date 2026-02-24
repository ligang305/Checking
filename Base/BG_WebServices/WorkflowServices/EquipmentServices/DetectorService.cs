using BG_Entities;
using BG_WorkFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BG_Services
{
    public class DetectorService : BaseInstance<DetectorService>
    {
        public Action ReflashPreviewImage;
        public delegate void ConnectionActionDelegate(int ConnectionStatus);
        public ConnectionActionDelegate ConnectionAction;
        public void Start()
        {
            DetecotrControllerManager.GetInstance().Load(DetectorEquipmenntEnum.BegoodDetector, ConfigServices.GetInstance().localConfigModel.ScanIpAddress, ConfigServices.GetInstance().localConfigModel.ScanPort,
            ConfigServices.GetInstance().localConfigModel.ScanImagePort);
            DetecotrControllerManager.GetInstance().ReflashImageAction += ReviewImageEvent;
            DetecotrControllerManager.GetInstance().Connection(ConfigServices.GetInstance().localConfigModel.ScanIpAddress, Convert.ToInt16(ConfigServices.GetInstance().localConfigModel.ScanPort),
            Convert.ToInt16(ConfigServices.GetInstance().localConfigModel.ScanImagePort));
            DetecotrControllerManager.GetInstance().DetecotorConnectionAction += DetectorConnection;
            //KeepDetecotrConnection();
        }
        public void Stop()
        {
            DetecotrControllerManager.GetInstance().ReflashImageAction -= ReviewImageEvent;
            ConnectionAction?.Invoke(0);
            DetecotrControllerManager.GetInstance().DisConnection();
        }

        public void SX_SetEnergy(int energy)
        {
            DetecotrControllerManager.GetInstance().SX_SetEnergy(energy);
        }

        private void DetectorConnection(int isConnection)  
        {
            if (DetecotrControllerManager.GetInstance().DetectorConnection == 0)
            {
                Task.Run(() =>
                {
                    Thread.Sleep(5000);
                    DetecotrControllerManager.GetInstance().
                    Connection(ConfigServices.GetInstance().localConfigModel.ScanIpAddress,
                    Convert.ToInt16(ConfigServices.GetInstance().localConfigModel.ScanPort),
                    Convert.ToInt16(ConfigServices.GetInstance().localConfigModel.ScanImagePort));
                });
            }
        }

        //private void KeepDetecotrConnection()
        //{
        //    Task.Run(() => {
        //        try
        //        {
        //            Thread.BeginThreadAffinity();
        //            ThreadService.SetThreadAffinityMask(ThreadService.GetCurrentThread(), new IntPtr(0x40));
        //            while (true)
        //            {
        //                if (DetecotrControllerManager.GetInstance().DetectorConnection == 0)
        //                {
        //                    DetecotrControllerManager.GetInstance().Connection();
        //                }
        //                Thread.Sleep(4848);
        //            }
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //        finally
        //        {
        //            Thread.EndThreadAffinity();
        //        }
        //    });
        //}

        /// <summary>
        /// 预览图像
        /// </summary>
        private void ReviewImageEvent()
        {
            ReflashPreviewImage?.Invoke();
        }
    }
}
