using BG_Entities;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BG_Services
{
    public class AccelatorService : BaseInstance<AccelatorService>
    {
        //发送锁
        private Mutex SendMutex = new Mutex();
        public Action<bool> ConnectionAction;

        private void InquireAccelatorStatus ()
        {
            TaskPool.GetInstance().AddAndStartTask(TaskList.AccelatorInquire, (Action)delegate ()
            {
                try
                {
                    Thread.BeginThreadAffinity();
                    ThreadService.SetThreadAffinityMask(ThreadService.GetCurrentThread(), new IntPtr(0x40));
                    while (true)
                    {
                        try
                        {
                            Thread.Sleep(100);
                            BoostingControllerManager.GetInstance().Inquire();
                            Task.Run(()=> { ConnectionAction?.Invoke(BoostingControllerManager.GetInstance().IsConnection());  });
                        }
                        catch (Exception ex)
                        {
                            BGLogs.Log.GetDistance().WriteInfoLogs(ex.StackTrace);
                        }
                    }
                }
                catch (Exception ex)
                {
                    BGLogs.Log.GetDistance().WriteInfoLogs(ex.StackTrace);
                }
                finally
                {
                    Thread.EndThreadAffinity();
                }
            });
        }
        public void Start()
        {
            BoostingControllerManager.GetInstance().Load(Common.controlVersion);
            InquireAccelatorStatus();
        }
        public void Stop()
        {
            BoostingControllerManager.GetInstance().DisConnection();
            ConnectionAction?.Invoke(false);
        }
    }
}
