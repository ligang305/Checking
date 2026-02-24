using BG_Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CMW.Common.Utilities
{
    //定义异常参数处理
    public class AggregateExceptionArgs : EventArgs
    {
        public AggregateException AggregateException { get; set; }
    }
    public static class CommonDeleget
    {
        #region 静态回调函数
        public delegate void CallBackAction();
        public delegate void NotifyBooleanCallBackAction(bool Result);
        public delegate void ImageXCallbackAction(int Result);
        public delegate void ImageXBSSpeedCallbackAction(int Speed);
        public delegate void CallBackParamaterAction(CallBackAction callBackAction);
        public delegate void MessageBoxAction(string MessageBoxMsg); //打印弹框日志信息
        public delegate void RequestTask();
        public delegate void LogAction(string logMsg, string LogType,bool IsInsert = false); //打印日志信息
        public delegate void InsertLogtoDb(string logMsg,string logType);
        public delegate string SearchStringData();
        public delegate bool DeleteData();
        public delegate void SettingAddressAction(string IpAddress, string Port, string Flag);
        public delegate void BuryingPointAction(string Message);
        public delegate void LoginAction(LoginStatus isLoginSuccess, List<string> ButtonList);
        public delegate void LoginSuccess(List<string> ButtonList);
        public delegate void UpdateConfig(string key, string value, string Section, string isAsync , string IsShow);
        public delegate void RestartSystem();
        public delegate void UploadSystem(object obj);
        public delegate void ShowModules(string ModulesName);
        public delegate void UploadImage(int status, long Carrid, string Path);
        public delegate void JummpPageAccordKeyWord(string Key);
        public delegate string UpdateStatusName(string Status);
        public delegate double UpdateFontSize(string Key);
        public delegate void InitParamater();
        public delegate void ReflashCarStatusDelegate(int Status);
        public delegate bool CheckSystemCondition();
        public delegate bool WaitWorkStop();
        public delegate bool RadiationOn();
        public delegate bool UploadImageInfoToWebBySocket(string Data);
        public delegate bool UploadAlarmInfoToWebBySocket(string Data);
        public delegate void ReflashStopPanel(List<string> Data);
        public delegate bool SetCommond(string Position,bool value);
        public delegate void Ray();
        public delegate void StopPrehot();
        public delegate bool SetDoseValue(byte Position, UInt32 Value);
        public delegate bool StateFeedback(bool IsSafe);
        public delegate bool SafeLinkeStateFeedback(bool IsSafe);
        public delegate void EnhanceScanAction(bool IsScan,object EnhanceScanCarInfo);


        public static event LogAction WriteLogEvent = null;
        public static event InsertLogtoDb InsertLogtoDbEvent = null;
        public static event RequestTask RequestTaskEvent = null;
        public static event MessageBoxAction MessageBoxEvent = null;
        public static event SearchStringData SearchStringDataEvent = null; //查找String类型的数据
        public static event DeleteData DeleteDataEvent = null; //清除图片数据的事件
        public static event SettingAddressAction SettingAddressActionEvent = null;
        public static event BuryingPointAction BuryingPointActionEvent = null;
        public static event LoginAction LoginResultEvent = null;
        public static event LoginSuccess LoginSuccessEvent = null;
        public static event UpdateConfig UpdateConfigAction = null;
        public static event RestartSystem RestartSystemAction = null;
        public static event UploadSystem UploadSystemAction = null;
        public static event ShowModules ShowModule = null;
        public static event StateFeedback StateFeedbackAction = null;
        public static event StateFeedback BSStateFeedbackAction = null;
        public static event SafeLinkeStateFeedback SafeLinkeStateFeedbackAction = null;
        public static event UploadImage UploadImageAction = null;
        public static event EventHandler<AggregateExceptionArgs> AggregateExceptionCatched;
        public static event JummpPageAccordKeyWord JummpPageAccordKeyWordEvent = null;
        public static event UpdateStatusName UpdateStatusNameEvent = null;
        public static event UpdateFontSize UpdateFontSizeEvent = null;
        public static event InitParamater InitParameter = null;
        public static event ReflashCarStatusDelegate ReflashCarStatus = null;
        public static event UploadImageInfoToWebBySocket UploadImageInfoToWebBySocketEvent;
        public static event UploadAlarmInfoToWebBySocket UploadAlarmInfoToWebBySocketEvent;
        public static event ReflashStopPanel ReflashStopPanelEvent;
        public static event SetCommond SetCommondEvent;
        public static event Ray BetratronRayEvent;
        public static event StopPrehot BetratronStopPrehotEvent;
        public static event SetDoseValue SetDoseValueEvent;
        public static event CheckSystemCondition CheckSystemConditionEvent;
        public static event WaitWorkStop WaitWorkStopEvent;
        public static event RadiationOn RadiationOnEvent;
        public static event CallBackAction CancelTaskEvent;
        public static event EnhanceScanAction EnhanceScanEvent;
        /// <summary>
        /// 适用于扫描类回调通知背散|或者后续成像 执行ImageX Start和Stop方法，原成像 扫描流程和ImageX杂在了一起
        /// 1: 开始；0：结束
        /// </summary>
        public static event ImageXCallbackAction ImageXStartAndStopCallbackEvent;
        /// <summary>
        /// 适用于扫描类回调通知背散|或者后续成像 执行ImageX SetMode，原成像 扫描流程和ImageX杂在了一起
        /// 1: 开始；0：结束
        /// </summary>
        public static event ImageXCallbackAction ImageXSetModeEvent;
        /// <summary>
        /// 适用于扫描类回调通知背散|或者后续成像 执行ImageX SetDirection，原成像 扫描流程和ImageX杂在了一起
        /// 1: 开始；0：结束
        /// </summary>
        public static event ImageXCallbackAction ImageXSetDirectionEvent;
        /// <summary>
        /// 适用于扫描类回调通知背散|或者后续成像 执行ImageX Speed，原成像 扫描流程和ImageX杂在了一起
        /// 1: 开始；0：结束
        /// </summary>
        public static event ImageXBSSpeedCallbackAction ImageXSetSpeedEvent;
        #endregion

        /// <summary>
        /// 适用于扫描类回调通知背散|或者后续成像 执行ImageX Start和Stop方法，原成像 扫描流程和ImageX杂在了一起
        /// 1: 开始；0：结束
        /// </summary>
        public static void ImageXSetSpeed(int Speed)
        {
            if (ImageXSetSpeedEvent != null && ImageXSetSpeedEvent.GetInvocationList().Length != 0)
            {
                ImageXSetSpeedEvent.Invoke(Speed);
            }
        }
        /// <summary>
        /// 适用于扫描类回调通知背散|或者后续成像 执行ImageX Start和Stop方法，原成像 扫描流程和ImageX杂在了一起
        /// 1: 开始；0：结束
        /// </summary>
        public static void ImageXStartAndStop(int StartOrStop)
        {
            if (ImageXStartAndStopCallbackEvent != null && ImageXStartAndStopCallbackEvent.GetInvocationList().Length != 0)
            {
                ImageXStartAndStopCallbackEvent.Invoke(StartOrStop);
            }
        }
        /// <summary>
        /// 适用于扫描类回调通知背散|或者后续成像 执行ImageX SetMode，原成像 扫描流程和ImageX杂在了一起
        /// 1：Mode 0:Mode
        /// </summary>
        public static void ImageXSetMode(int Mode)
        {
            if (ImageXSetModeEvent != null && ImageXSetModeEvent.GetInvocationList().Length != 0)
            {
                ImageXSetModeEvent.Invoke(Mode);
            }
        }
        /// <summary>
        /// 适用于扫描类回调通知背散|或者后续成像 执行ImageX SetDirection，原成像 扫描流程和ImageX杂在了一起
        /// 1:向前 0：向后
        /// </summary>
        public static void ImageXSetDirection(int Direction)
        {
            if (ImageXSetDirectionEvent != null && ImageXSetDirectionEvent.GetInvocationList().Length != 0)
            {
                ImageXSetDirectionEvent.Invoke(Direction);
            }
        }

        /// <summary>
        /// 安全联锁状态回调
        /// </summary>
        public static void EnhanceScan(bool IsScan,object EnhanceScanCarInfo)
        {
            if (EnhanceScanEvent != null && EnhanceScanEvent.GetInvocationList().Length != 0)
            {
                EnhanceScanEvent.Invoke(IsScan,EnhanceScanCarInfo);
            }
        }

        /// <summary>
        /// 安全联锁状态回调
        /// </summary>
        public static void SafeLinkStateFeedbackEvent(bool IsSafeLink)
        {
            if (SafeLinkeStateFeedbackAction != null && SafeLinkeStateFeedbackAction.GetInvocationList().Length != 0)
            {
                SafeLinkeStateFeedbackAction.Invoke(IsSafeLink);
            }
        }
        /// <summary>
        /// 安全联锁状态回调
        /// </summary>
        public static void StateFeedbackEvent(bool IsSafeLink)
        {
            if (StateFeedbackAction != null && StateFeedbackAction.GetInvocationList().Length != 0)
            {
                Debug.WriteLine("-------------------------ganggang safe1-----------------------" + IsSafeLink);
                StateFeedbackAction.Invoke(IsSafeLink);
            }
        }

        /// <summary>
        /// 安全联锁状态回调
        /// </summary>
        public static void StateBSFeedbackEvent(bool IsSafeLink)
        {
            if (BSStateFeedbackAction != null && BSStateFeedbackAction.GetInvocationList().Length != 0)
            {
                BSStateFeedbackAction.Invoke(IsSafeLink);
            }
        }
        /// <summary>
        /// 取消任务
        /// </summary>
        public static void CancelTaskAction()
        {
            if (CancelTaskEvent != null && CancelTaskEvent.GetInvocationList().Length != 0)
            {
                CancelTaskEvent.Invoke();
            }
        }

        public static string SearchStringDataAction()
        {
            if (SearchStringDataEvent != null && SearchStringDataEvent.GetInvocationList().Length != 0)
            {
                return SearchStringDataEvent.Invoke();
            }
            return "0";
        }
        public static bool SetDoseValueEventAction(byte Position, UInt32 Value)
        {
            if (SetDoseValueEvent != null && SetDoseValueEvent.GetInvocationList().Length != 0)
            {
                return SetDoseValueEvent.Invoke(Position, Value);
            }
            return false;
        }
        public static bool DeleteDataAction()
        {
            if (DeleteDataEvent != null && DeleteDataEvent.GetInvocationList().Length != 0)
            {
                return DeleteDataEvent.Invoke();
            }
            return false;
        }
        public static void MessageBoxActionAction(string MessageBoxMsg)
        {
            if (MessageBoxEvent != null && MessageBoxEvent.GetInvocationList().Length != 0)
            {
                MessageBoxEvent.Invoke(MessageBoxMsg);
            }
        }
        public static void InitParameterAction()
        {
            if (InitParameter != null && InitParameter.GetInvocationList().Length != 0)
            {
                InitParameter.Invoke();
            }
        }
        public static void ReflashCarStatusUi(int Status)
        {
            if (ReflashCarStatus != null && ReflashCarStatus.GetInvocationList().Length != 0)
            {
                ReflashCarStatus.Invoke(Status);
            }
        }
        public static string UpdateStatusNameAction(string Status)
        {
            if (UpdateStatusNameEvent != null && UpdateStatusNameEvent.GetInvocationList().Length != 0)
            {
                string status = UpdateStatusNameEvent.Invoke(Status);
                return status;
                //return UpdateStatusNameEvent.Invoke(Status);
            }
            return string.Empty;
        }
        public static double UpdateFontSizeAction(string FontSize)
        {
            if (UpdateFontSizeEvent != null && UpdateFontSizeEvent.GetInvocationList().Length != 0)
            {
                return UpdateFontSizeEvent.Invoke(FontSize);
            }
            return 9;
        }
        public static void RequestTaskAction()
        {
            if (RequestTaskEvent != null && RequestTaskEvent.GetInvocationList().Length != 0)
            {
                RequestTaskEvent.Invoke();
            }
        }
        public static void WriteLogAction(string logInfo, string LogType, bool IsInsert = false)
        {
            if (WriteLogEvent != null && WriteLogEvent.GetInvocationList().Length != 0)
            {
                WriteLogEvent.Invoke(logInfo, LogType, IsInsert);
            }
        }

        public static void InsertLogtoDbAction(string logInfo, string LogType)
        {
            if (InsertLogtoDbEvent != null && InsertLogtoDbEvent.GetInvocationList().Length != 0)
            {
                InsertLogtoDbEvent.Invoke(logInfo, LogType);
            }
        }

        public static bool SetCommondAction(string Position, bool Value)
        {
            if (SetCommondEvent != null && SetCommondEvent.GetInvocationList().Length != 0)
            {
                return SetCommondEvent.Invoke(Position, Value);
            }
            return false;
        }
        public static void HandTaskException(Exception ex)
        {
            if (AggregateExceptionCatched != null && AggregateExceptionCatched.GetInvocationList().Length != 0)
            {
                AggregateExceptionCatched.Invoke(null, new AggregateExceptionArgs() { AggregateException = new AggregateException(ex)});
            }
        }
        /// <summary>
        /// 通过面板跳转
        /// </summary>
        /// <param name="Key"></param>
        public static void JummpPageAccordKeyWordEventAction(string Key)
        {
            if (JummpPageAccordKeyWordEvent != null && JummpPageAccordKeyWordEvent.GetInvocationList().Length != 0)
            {
                JummpPageAccordKeyWordEvent.Invoke(Key);
            }
        }
        /// <summary>
        /// 设置Ip地址
        /// </summary>
        /// <param name="IpAddress"></param>
        /// <param name="Port"></param>
        /// <param name="Flag"></param>
        public static void SettingAddress(string IpAddress, string Port, string Flag)
        {
            if (SettingAddressActionEvent != null && SettingAddressActionEvent.GetInvocationList().Length != 0)
            {
                SettingAddressActionEvent.Invoke(IpAddress, Port, Flag);
            }
        }
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="IpAddress"></param>
        /// <param name="Port"></param>
        /// <param name="Flag"></param>
        public static void UploadImageOnline(int status,long Carrid,string Path)
        {
            Debug.WriteLine("-------------------------ganggang_upload_img1-----------------------");
            if (UploadImageAction != null && UploadImageAction.GetInvocationList().Length != 0)
            {
                UploadImageAction.Invoke(status, Carrid,Path);
            }
        }

        /// <summary>
        /// 显示模块
        /// </summary>
        public static void ShowModuleEvent(string ModuleNames)
        {
            if (ShowModule != null && ShowModule.GetInvocationList().Length != 0)
            {
                ShowModule.Invoke(ModuleNames);
            }
        }
        /// <summary>
        /// 重启系统事件
        /// </summary>
        public static void RestartSystemEvent()
        {
            if (RestartSystemAction != null && RestartSystemAction.GetInvocationList().Length != 0)
            {
                RestartSystemAction.Invoke();
            }
        }
        public static void UploadSystemActionEvent(Object obj)
        {
            if (UploadSystemAction != null && UploadSystemAction.GetInvocationList().Length != 0)
            {
                UploadSystemAction.Invoke(obj);
            }
        }
        /// <summary>
        /// 埋点的公用方法
        /// </summary>
        /// <param name="Message"></param>
        public static void BuryingPoint(string Message)
        {
            if (BuryingPointActionEvent != null && BuryingPointActionEvent.GetInvocationList().Length != 0)
            {
                BuryingPointActionEvent.Invoke(Message);
            }
        }
        public static void UpdateConfigs(string key, string value,string Section, string isAsync = "false", string IsShow = "false")
        {
            if (UpdateConfigAction != null && UpdateConfigAction.GetInvocationList().Length != 0)
            {
                UpdateConfigAction.Invoke(key, value,Section, isAsync,IsShow);
            }
        }
        public static void LoginResultAction(LoginStatus IsResult, List<string> ButtonList)
        {
            if (LoginResultEvent != null && LoginResultEvent.GetInvocationList().Length != 0)
            {
                LoginResultEvent.Invoke(IsResult, ButtonList);
            }
        }
        public static void LoginSuccessAction(List<string> ButtonList)
        {
            if (LoginSuccessEvent != null && LoginSuccessEvent.GetInvocationList().Length != 0)
            {
                LoginSuccessEvent.Invoke(ButtonList);
            }
        }
        /// <summary>
        /// 通过Socket方式将图片信息上传至服务器后台
        /// </summary>
        /// <param name="ImageInfo"></param>
        public static bool UploadImageToWebBySocket(string ImageInfo)
        {
            Debug.WriteLine("-------------------------ganggang_upload_img3-----------------------");
            if (UploadImageInfoToWebBySocketEvent != null && UploadImageInfoToWebBySocketEvent.GetInvocationList().Length != 0)
            {
                return UploadImageInfoToWebBySocketEvent.Invoke(ImageInfo);
            }
            return false;
        }
        /// <summary>
        /// 上传报警信息至后台
        /// </summary>
        /// <param name="AlarmInfo"></param>
        /// <returns></returns>
        public static bool UploadAlarmInfoBySocket(string AlarmInfo)
        {
            Debug.WriteLine("-------------------------ganggang_upload_img2-----------------------");
            if (UploadAlarmInfoToWebBySocketEvent != null && UploadAlarmInfoToWebBySocketEvent.GetInvocationList().Length != 0)
            {
                return UploadAlarmInfoToWebBySocketEvent.Invoke(AlarmInfo);
            }
            return false;
        }
        public static bool CheckSystemConditionAction()
        {
            if (CheckSystemConditionEvent != null && CheckSystemConditionEvent.GetInvocationList() != null && CheckSystemConditionEvent.GetInvocationList().Length != 0)
            {
                return CheckSystemConditionEvent.Invoke();
            }
            return false;
        }
        public static bool WaitWorkStopAction()
        {
            if (WaitWorkStopEvent != null && WaitWorkStopEvent.GetInvocationList().Length != 0)
            {
                return WaitWorkStopEvent.Invoke();
            }
            return false;
        }
        public static bool RadiationOnAction()
        {
            if (RadiationOnEvent != null && RadiationOnEvent.GetInvocationList().Length != 0)
            {
                return RadiationOnEvent.Invoke();
            }
            return false;
        }
        public static void StopPrehotAction()
        {
            if (BetratronStopPrehotEvent != null && BetratronStopPrehotEvent.GetInvocationList().Length != 0)
            {
                BetratronStopPrehotEvent.Invoke();
            }
        }
        public static void RayAction()
        {
            if (BetratronRayEvent != null && BetratronRayEvent.GetInvocationList().Length != 0)
            {
                BetratronRayEvent.Invoke();
            }
        }
    }
}
