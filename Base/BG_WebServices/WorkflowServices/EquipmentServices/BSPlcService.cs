using BG_Entities;
using BG_WorkFlow;
using BGLogs;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BG_Services
{
    public class BSPlcService : BaseInstance<BSPlcService>
    {
        public delegate void ConnectionActionDelegate(bool Connection);
        public event EventHandler<bool> ConnectionAction;
        bool isAlive = true;
        string PlcIPAddress;
        string PlcPort;

        public void SetIpAddress(string IPAddress, string Port)
        {
            PlcIPAddress = IPAddress;
            PlcPort = Port;
        }

        /// <summary>
        /// 开始服务
        /// </summary>
        public void Start()
        {
            BSPLCControllerManager.GetInstance().PlcConnectionCallback += PlcConnectionCallback;
            BSPLCControllerManager.GetInstance().Load(ControlVersion.BS, null);
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            BSPLCControllerManager.GetInstance().PlcConnectionCallback -= PlcConnectionCallback;
            TaskPool.GetInstance().EndTask(TaskList.Inquire);
            TaskPool.GetInstance().EndTask(TaskList.InquireParamater);
            ConnectionAction?.Invoke(this,false);
            isAlive = false;
        }
        /// <summary>
        /// 查询PLC点位信息；M区字节位
        /// </summary>
        private void InquirePlcPosition()
        {
            TaskPool.GetInstance().AddAndStartTask(TaskList.BSInquire, (Action)delegate ()
            {
                try
                {
                    while (isAlive)
                    {
                        try
                        {
                         
                            Thread.Sleep(30);
                            if (!BSPLCControllerManager.GetInstance().pLCEquipment.IsConnection)
                            {
                                ConnectionAction?.Invoke(this, false);
                                BSPLCControllerManager.GetInstance().BSGlobalRetStatus = BSPLCControllerManager.GetInstance().BSGlobalRetStatus.Select(q => { q = false; return q; }).ToList();
                                continue;
                            }
                            BSPLCControllerManager.GetInstance().InquirePositionStatus();
                         
                            ConnectionAction?.Invoke(this, true);
                        }
                        catch (Exception ex)
                        {
                            Log.GetDistance().WriteErrorLogs(ex.StackTrace);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.GetDistance().WriteErrorLogs(ex.StackTrace);
                }
                finally
                {
                }
            });
        }

        /// <summary>
        /// 查询硬件状态信息-非Bool型，四字节或双字节数据
        /// </summary>
        private void InquireHandwareParamaters()
        {
            TaskPool.GetInstance().AddAndStartTask(TaskList.BSInquireParamater, (Action)delegate ()
            {
                try
                {
                    while (isAlive)
                    {
                        try
                        {
                            Thread.Sleep(100);
                            if (!BSPLCControllerManager.GetInstance().pLCEquipment.IsConnection)
                            {
                                continue;
                            }
                            BSPLCControllerManager.GetInstance().InquireHardwareStatus();
                        }
                        catch (Exception ex)
                        {
                            Log.GetDistance().WriteErrorLogs(ex.StackTrace);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.GetDistance().WriteErrorLogs(ex.StackTrace);
                }
                finally
                {
                    //Thread.EndThreadAffinity();
                }
            });
        }
        /// <summary>
        /// PLC连接
        /// </summary>
        private void PlcConnection()
        {
            TaskPool.GetInstance().AddAndStartTask(TaskList.BSPlcConnection, (Action)delegate () { BSPLCControllerManager.GetInstance().Connect(PlcIPAddress,PlcPort); });
        }

        private void PlcConnectionCallback()
        {
            InquirePlcPosition();
            InquireHandwareParamaters();
            PlcConnection();
        }
    }
}
