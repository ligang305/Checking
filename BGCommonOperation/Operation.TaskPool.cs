using BG_Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CMW.Common.Utilities
{
    /// <summary>
    /// 自定义线程池
    /// </summary>
    public class TaskPool:BaseInstance<TaskPool>
    {
        /// <summary>
        /// 每个Task绑定一个
        /// </summary>
        Dictionary<string,Action> ITaskList = new Dictionary<string, Action>();
        /// <summary>
        /// 每个Task绑定一个Bool
        /// </summary>
        Dictionary<string,bool> IBoolList = new Dictionary<string, bool>();

        /// <summary>
        /// 向线程池添加指定线程
        /// </summary>
        /// <param name="TaskName"></param>
        /// <param name="TaskAction"></param>
        /// <returns></returns>
        public bool AddTask(string TaskName,Action TaskAction)
        {
            if (ITaskList.ContainsKey(TaskName)) return false;
            if (IBoolList.ContainsKey(TaskName)) return false;
            ITaskList.Add(TaskName, TaskAction);
            IBoolList.Add(TaskName,false);
            return true;
        }
        /// <summary>
        /// 向线程池添加指定线程并立即启动
        /// </summary>
        /// <param name="TaskName"></param>
        /// <param name="TaskAction"></param>
        /// <returns></returns>
        public bool AddAndStartTask(string TaskName, Action TaskAction)
        {
            if (ITaskList.ContainsKey(TaskName)) return false;
            if (IBoolList.ContainsKey(TaskName)) return false;
            ITaskList.Add(TaskName, TaskAction);
            IBoolList.Add(TaskName, false);
            StartTask(TaskName,false);
            return true;
        }
        public bool EndTask(string TaskName)
        {
            if (!ITaskList.ContainsKey(TaskName)) return false;
            if (!IBoolList.ContainsKey(TaskName)) return false;
            IBoolList[TaskName] = false;
            return true;
        }

        /// <summary>
        /// 开始指定的线程
        /// </summary>
        /// <param name="TaskName"></param>
        /// <param name="isCycle"></param>
        /// <returns></returns>
        public bool StartTask(string TaskName,bool isCycle)
        {
            if (!ITaskList.ContainsKey(TaskName)) return false;
            if (!IBoolList.ContainsKey(TaskName)) return false;
            IBoolList[TaskName] = true;
            Action ExcuteTask = ITaskList[TaskName];
            Task.Run(() =>
            {
                while (IBoolList[TaskName])
                {
                    //Thread.Sleep(1);
                    //if (!Common.isExcuteTask) continue;
                    ExcuteTask.Invoke();
                }
            });
            return true;
        }

    }
}
