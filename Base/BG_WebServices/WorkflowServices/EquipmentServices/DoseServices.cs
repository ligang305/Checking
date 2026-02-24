using CMW.Common.Utilities;
using BGCommunication;
using BGModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BG_WorkFlow;

namespace BG_Services
{
    public class DoseServices
    {
        /// <summary>
        /// 单实例服务
        /// </summary>
        public static DoseServices Service { get; private set; }
        private List<Bg_Dose> DoseList = new List<Bg_Dose>();
        public Dictionary<int, bool> IsDoseConnections = new Dictionary<int, bool>();
        public Dictionary<int, DoseProtocol> GlogbalDoseProtocols = new Dictionary<int, DoseProtocol>();
        static DoseServices()
        {
            Service = new DoseServices();   
        }
        //服务启动
        public void Start()
        {
            InitDoseCondition();
            QuireDose();
        }
        //服务终止
        public void Stop()
        {
            foreach (var ProtocolItem in GlogbalDoseProtocols)
            {
                ProtocolItem.Value?.DisConnect();
            }
        }

        protected void InitDoseCondition()
        {
            DoseList = DoseBLL.GetInstance().GetDoseModel();
            foreach (var item in DoseList)
            {
                IsDoseConnections.Add(DoseList.IndexOf(item) + 1, false);
                InitDosePlcConnection(DoseList.IndexOf(item) + 1, new DoseProtocol(), item);
            }
        }
        /// <summary>
        /// 初始化Dose协议
        /// </summary>
        /// <param name="pp"></param>
        protected void InitDosePlcConnection(int DoseItemIndex, DoseProtocol pp, Bg_Dose DoseItem)
        {
            pp.WaitRecTick = 100;
            ConnectInterface client = new TCPClient(DoseItem.IPAddress, Convert.ToInt32(DoseItem.Port), false, false);
            pp.InitConnection(ref client);
            GlogbalDoseProtocols.Add(DoseItemIndex, pp);
        }
        protected void QuireDose()
        {
            int PropIndex = 1;
            foreach (var ProtocolItem in GlogbalDoseProtocols)
            {
                Task.Factory.StartNew(new Action<object>(ConnectionDose), PropIndex);
                PropIndex++;
            }
        }
        /// <summary>
        /// 检测辐射探测连接的线程
        /// </summary>
        protected void ConnectionDose(object PropIndex)
        {
            int PropIndexInt = Convert.ToInt32(PropIndex);
            Bg_Dose DoseItem = DoseList[PropIndexInt - 1];
            while (Common.isExcuteTask)
            {
                try
                {
                    Thread.Sleep(100);
                    if (!CommonFunc.PingIp(DoseItem.IPAddress))
                    {
                        IsDoseConnections[PropIndexInt] = CommonFunc.PingIp(DoseItem.IPAddress);
                        continue;
                    }     
                    //如果未连接
                    if (!IsDoseConnections[PropIndexInt] || !GlogbalDoseProtocols[PropIndexInt].IsConnect())
                    {
                        InitDose(PropIndexInt);
                        continue;
                    }
                    GlogbalDoseProtocols[PropIndexInt].InqurDoseGetStatus(PropIndexInt);
                    SetDoseToPlc(PropIndexInt);
                }
                catch (Exception ex)
                {
                    CommonDeleget.HandTaskException(ex);
                }
            }
        }
       

        /// <summary>
        /// 初始化Dose连接
        /// </summary>
        public void InitDose(int PropIndex)
        {
            bool IsCurrentIndexDoseConnection = GlogbalDoseProtocols[PropIndex].Connect();
            if (IsCurrentIndexDoseConnection)
            {
                IsDoseConnections[PropIndex] = true;
            }
            else
            { IsDoseConnections[PropIndex] = false; }
        }
        /// <summary>
        /// 获取到了Dose剂量更新至PLC
        /// </summary>
        /// <param name="DoseIndex"></param>
        /// <returns></returns>
        public bool SetDoseToPlc(int DoseIndex)
        {
            if (!ConfigServices.GetInstance().localConfigModel.IsSendDose) return false;
            bool result = false;
            switch (DoseIndex)
            {
                case 1:
                    UInt32 Value =   GlogbalDoseProtocols[DoseIndex].ReadStr;
                    return PLCControllerManager.GetInstance().WritePositionValue(ConfigServices.GetInstance().localConfigModel.Dose1Position, Value);
                case 2:
                    UInt32 Value2 =  GlogbalDoseProtocols[DoseIndex].ReadStr;
                    return PLCControllerManager.GetInstance().WritePositionValue(ConfigServices.GetInstance().localConfigModel.Dose2Position, Value2);
                case 3:
                    UInt32 Value3 = GlogbalDoseProtocols[DoseIndex].ReadStr;
                    return PLCControllerManager.GetInstance().WritePositionValue(ConfigServices.GetInstance().localConfigModel.Dose3Position, Value3);
            }

            return result;
        }
    }
}
