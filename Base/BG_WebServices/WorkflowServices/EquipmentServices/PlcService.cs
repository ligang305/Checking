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
    public class PlcService : BaseInstance<PlcService>
    {
        public delegate void ConnectionActionDelegate(bool Connection);
        public event EventHandler<bool> ConnectionAction;
        bool isAlive = true;
        string PlcIPAddress;
        string PlcPort;
        /// <summary>
        /// 开始服务
        /// </summary>
        public void Start()
        {
            PlcIPAddress = ConfigServices.GetInstance().localConfigModel.IpAddress;
            PlcPort = ConfigServices.GetInstance().localConfigModel.Port;
            PLCControllerManager.GetInstance().PlcConnectionCallback += PlcConnectionCallback;
            PLCControllerManager.GetInstance().Load(Common.controlVersion, null);
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            PLCControllerManager.GetInstance().DisConnect();
            PLCControllerManager.GetInstance().PlcConnectionCallback -= PlcConnectionCallback;
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
            TaskPool.GetInstance().AddAndStartTask(TaskList.Inquire, (Action)delegate ()
            {
                try
                {
                    //Thread.BeginThreadAffinity();
                    //ThreadService.SetThreadAffinityMask(ThreadService.GetCurrentThread(), new IntPtr(0x40));
                    while (isAlive)
                    {
                        try
                        {
                            Thread.Sleep(30);
                            if (!Common.IsConnection)
                            {
                                ConnectionAction?.Invoke(this, false);
                                Common.GlobalRetStatus = Common.GlobalRetStatus.Select(q => { q = false; return q; }).ToList();
                                continue;
                            }
                            PLCControllerManager.GetInstance().InquirePositionStatus();
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
                    //Thread.EndThreadAffinity();
                }
            });
        }

        /// <summary>
        /// 查询硬件状态信息-非Bool型，四字节或双字节数据
        /// </summary>
        private void InquireHandwareParamaters()
        {
            TaskPool.GetInstance().AddAndStartTask(TaskList.InquireParamater, (Action)delegate ()
            {
                try
                {
                    while (isAlive)
                    {
                        try
                        {
                            Thread.Sleep(100);
                            if (!Common.IsConnection)
                            {
                                continue;
                            }
                            PLCControllerManager.GetInstance().InquireHardwareStatus();
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
            TaskPool.GetInstance().AddAndStartTask(TaskList.PlcConnection, (Action)delegate () { PLCControllerManager.GetInstance().Connect(PlcIPAddress,PlcPort); });
        }

        private void PlcConnectionCallback()
        {
            InquirePlcPosition();
            InquireHandwareParamaters();
            PlcConnection();
        }
    }
}
