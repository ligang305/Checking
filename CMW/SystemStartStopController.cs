using System;
using System.IO;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel.Composition;
using BGModel;
using System.ComponentModel.Composition.Hosting;
using BGLogs;
using System.Linq;
using BGDAL;
using System.Collections.Generic;
using BG_Services;
using BG_WorkFlow;
using BG_Entities;
using CMW.Common.Utilities;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonFunc;
namespace CMW
{
    /// <summary>
    /// 系统初始化单例类
    /// </summary>
    class SystemStartStopController
    {

        #region Prop
        CompositionContainer container = null;
        [ImportMany("UserPage", typeof(IUserControlView), AllowRecomposition = true)]
        //保存插件的内存对象
        public Lazy<IUserControlView, IMetaData>[] Plugins { get; set; }
        #endregion

     

        #region Lazy Singleton

        private static SystemStartStopController instance = null;

        private static readonly object syncRoot = new object();

        private SystemStartStopController()
        {
        }

        public static SystemStartStopController GetIns()
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new SystemStartStopController();
                    }
                }
            }
            return instance;
        }

        #endregion

        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            Log.GetDistance().WriteInfoLogs($"Application Start");
            ExcuteLoadWindow();
            Log.GetDistance().WriteInfoLogs($"Application ExcuteLoadWindow");
            ConfigServices.GetInstance().Start();
            Log.GetDistance().WriteInfoLogs($"Application ConfigServices");
            InitGlobeControlVersion();
            Log.GetDistance().WriteInfoLogs($"Application InitGlobeControlVersion");
            FontSizeServices.GetInstance().Start();
            Log.GetDistance().WriteInfoLogs($"Application FontSizeServices");
            LanguageServices.GetInstance().Start();
            Log.GetDistance().WriteInfoLogs($"Application LanguageServices");
            EquipmentManager.GetInstance().Start();
            Log.GetDistance().WriteInfoLogs($"Application Start Endding");
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop() 
        {
            ConfigServices.GetInstance().Stop();
            FontSizeServices.GetInstance().Stop();
            LanguageServices.GetInstance().Stop();
            EquipmentManager.GetInstance().Stop();
        }

        /// <summary>
        /// 窗体加载时调用插件的方法
        /// </summary>
        private void ExcuteLoadWindow()
        {
            try
            {
                SystemDirectoryConfig.AppDir = AppDomain.CurrentDomain.BaseDirectory;
                //获取工作目录
                var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                if (dir.Exists)
                {
                    //就是这里，读取所有符合条件的dll
                    var catalog = new DirectoryCatalog(dir.FullName, "*.dll");
                    container = new CompositionContainer(catalog);
                    try
                    {
                        container.ComposeParts(this);
                    }
                    catch (Exception ce)
                    {
                        Log.GetDistance().WriteInfoLogs(ce.Message);
                    }
                    Plugins.OrderBy(p => p.Metadata.Priority);
                }
            }
            catch (Exception ex)
            {
                Log.GetDistance().WriteInfoLogs($"Load Module Error:{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 初始化全局的版本
        /// </summary>
        public void InitGlobeControlVersion()
        {
            switch (ConfigServices.GetInstance().localConfigModel.CMW_Version)
            {
                case "1":
                    controlVersion = ControlVersion.Car;
                    return;
                case "2":
                    controlVersion = ControlVersion.FastCheck;
                    return;
                case "3":
                    controlVersion = ControlVersion.SelfWorking; // V7600
                    return;
                case "4":
                    controlVersion = ControlVersion.CombinedMovement;
                    return;
                case "5":
                    controlVersion = ControlVersion.CombinedMovementBetatron; 
                    return;
                case "6":
                    controlVersion = ControlVersion.PassengerCar;
                    return;
                case "7":
                    controlVersion = ControlVersion.BGV7000;
                    return;
                case "8":
                    controlVersion = ControlVersion.BGV7700;
                    return;
                case "9":
                    controlVersion = ControlVersion.BGV8000;
                    return;
                case "10":
                    controlVersion = ControlVersion.BS;
                    return;
                case "11":
                    controlVersion = ControlVersion.BGV5100;
                    return;
                case "12":
                    controlVersion = ControlVersion.BGV5100FH;
                    return;
                //case "13":
                    //controlVersion = ControlVersion.BGV6000BS;
                    //return;
                default:
                    return;
            }
        }
    }
}
