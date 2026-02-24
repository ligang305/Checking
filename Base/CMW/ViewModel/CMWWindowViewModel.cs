using BG_Entities;
using BG_Services;
using BG_WorkFlow;
using BGModel;
using BGUserControl;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonFunc;
using static CMW.Common.Utilities.CommonDeleget;

namespace CMW.ViewModel
{
    public partial class CMWWindowViewModel : BaseMvvm
    {
        public CMWWindowViewModel()
        {
            Start();
        }
        public void Start()
        {
            InitUI();
            InitContent();
            InitList();
            Task.Run(() => {
                Thread.Sleep(1000);
                PLCControllerManager.GetInstance().WritePositionValue(CommandDic[Command.SysetmStartEnd], true);
                PLCControllerManager.GetInstance().WritePositionValue(CommandDic[Command.AutoMode], true);
            });
         
        }
        public void Stop()
        {
            PLCControllerManager.GetInstance().WritePositionValue(CommandDic[Command.SysetmStartEnd], false);
            PLCControllerManager.GetInstance().WritePositionValue(CommandDic[Command.AutoMode], false);
            CommonDeleget.InsertLogtoDbEvent += CommonDeleget_InsertLogtoDbEvent;
           
        }
        private void InitContent()
        {
            try
            {
                string ModuleNames = _App.ControlVersionList.FirstOrDefault(q => q.ControlversionKey == ConfigServices.GetInstance().localConfigModel.CMW_Version)?.ControlVersionName;
                if (!string.IsNullOrEmpty(ModuleNames))
                {
                    var modules = SystemStartStopController.GetIns().Plugins.First((q => q.Metadata.Name == ModuleNames))?.Value;
                    if (modules == null) { BG_MESSAGEBOX.Show("提示", $"加载界面失败检查是否少了{ModuleNames}dll必要文件！"); return; }
                    currentModule = modules;
                }
            }
            catch (Exception ex)
            {
                BGLogs.Log.GetDistance().WriteErrorLogs(ex.StackTrace);
            }
        }
        private void CommonDeleget_InsertLogtoDbEvent(string logMsg, string logType)
        {
            try
            {
                BG_Logs bG_Logs = new BG_Logs() { LogContent = logMsg, LogName = logMsg, LogDataTime = DateTime.Now.ToString("yyyyMMddHHmmss"), LogType = logType };
                logBLL.Insert(bG_Logs);
            }
            catch (Exception ex)
            {
                BGLogs.Log.GetDistance().WriteErrorLogs(ex.StackTrace);
            }
        }
        private void InitList()
        {
            HitchList.Clear();
            HitChModelBLL.GetInstance().GetHitchModelDataModel
              (SystemDirectoryConfig.GetInstance().GetHittingConfig(controlVersion)).Where(q => q.StatusOwner.Contains("status_Hitch_")).ToList().ForEach(q => HitchList.Add(q));
            HitchModels = HitChModelBLL.GetInstance().GetHitModelDic("MainPage", controlVersion);
        }
        protected override void InquirePlcStatus(List<bool> StatusList)
        {
            try
            {
                foreach (var HitchItems in HitchModels)
                {
                    foreach (var item in HitchItems.Value)
                    {
                        StatusModel sm = item as StatusModel;
                        if (!IsConnection)
                        {
                            if (sm.StatusName.Contains("DetectorReady"))
                            {
                                //探测器需要去找扫描站的连接状态，如果判断到扫描站的状态是未连接
                                sm.StatusCode = IsScanCanScan() ? "1" : "0";
                                continue;
                            }
                            sm.StatusCode = "0";
                            continue;
                        }
                        int ItemIndex = Convert.ToInt32(sm.StatusIndex);
                        if (sm.StatusName.Contains("DetectorReady"))
                        {
                            //探测器需要去找扫描站的连接状态，如果判断到扫描站的状态是未连接
                            sm.StatusCode = IsScanCanScan() ? "1" : "0";
                            continue;
                        }
                        if (ItemIndex < GlobalRetStatus.Count)
                        {
                            if (HitchItems.Key.Equals("MainPageSafeSeriesLinkModule"))
                            {
                                sm.StatusCode = !GlobalRetStatus[ItemIndex] ? "1" : "0";
                            }
                            else
                            {
                                sm.StatusCode = GlobalRetStatus[ItemIndex] ? "1" : "0";
                            }
                        }
                        RefalshHitchItems(HitchItems);
                    }
                    RefalshHitchItems(HitchItems);
                }
            }
            catch (Exception ex)
            {
                BGLogs.Log.GetDistance().WriteErrorLogs(ex.StackTrace);
            }
        }
        /// <summary>
        /// 刷新状态项
        /// </summary>
        /// <param name="HitchItems"></param>
        /// <returns></returns>
        private void RefalshHitchItems(KeyValuePair<string, ObservableCollection<object>> HitchItems)
        {
            if (HitchItems.Key.Equals("MainPageScanConditionModule"))
            {
                CommonDeleget.StateFeedbackEvent(HitchItems.Value.Count(q => (q as StatusModel).StatusCode == "0") == 0);
            }
        }
        protected override void ConnectionStatus(bool ConnectionStatus)
        {
            if (ConnectionStatus)
            {
                if (!LastConnectionStatus) { PLCControllerManager.GetInstance().WritePositionValue(CommandDic[Command.SysetmStartEnd], true); }
            }
            LastConnectionStatus = ConnectionStatus;
        }
        //初始化UI   
        private void InitUI()
        {
            if (currentModule == null) return;
            StatusBarStatus = currentModule.GetStatusBarStatus();
            EnergyStopPoints = currentModule.GetEnergyStopPanel();
            RayImageSource = currentModule.GetRayImage();
            RayPoints = currentModule.GetRayPointList();
            MainImagePath = currentModule.GetMainImage();
        }
    }
}
