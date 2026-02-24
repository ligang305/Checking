using BG_Entities;
using BG_Services;
using BGLogs;
using BGModel;
using CMW.Common.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.ImageImportDll;

namespace BGUserControl
{
    /// <summary>
    /// EnhanceScanPanel.xaml 的交互逻辑
    /// </summary>
    public partial class EnhanceScanPanel : UserControl
    {
        EnhanceScanPanelMvvm enhanceScanPanelMvvm = new EnhanceScanPanelMvvm();
        public EnhanceScanPanel()
        {
            InitializeComponent();
            DataContext = enhanceScanPanelMvvm;
        }

        private void Btn_Delete(object sender, MouseButtonEventArgs e)
        {
            Border btn = (sender as Border);
            enhanceScanPanelMvvm.Delect(btn.Tag);
        }
        private void Btn_Scan(object sender, MouseButtonEventArgs e)
        {
            Border btn = (sender as Border);
            enhanceScanPanelMvvm.ScanEnhance(btn.Tag);
        }

        private void Delete_Loaded(object sender, RoutedEventArgs e)
        {
            Label lbl = (sender as Label);
            lbl.SetBinding(ContentProperty,new Binding { Source = enhanceScanPanelMvvm ,Path = new PropertyPath("Delete")});
        }
        private void Scan_Loaded(object sender, RoutedEventArgs e)
        {
            Label lbl = (sender as Label);
            lbl.SetBinding(ContentProperty, new Binding { Source = enhanceScanPanelMvvm, Path = new PropertyPath("Scan") });
        }
    }

    public class EnhanceScanPanelMvvm : BaseMvvm
    {

        #region 属性
        private string _EnhanceScan { get; set; }
        public string EnhanceScan
        {
            get { return _EnhanceScan; }
            set 
            {
                _EnhanceScan = value;
                RaisePropertyChanged();
            }
        }
        private bool _IsShowEnhanceScanPanel { get; set; } = false;
        public bool IsShowEnhanceScanPanel
        {
            get { return _IsShowEnhanceScanPanel; }
            set
            {
                _IsShowEnhanceScanPanel = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 车辆扫描起始位置
        /// </summary>
        private string _StartCarPosition { get; set; }
        public string StartCarPosition 
        {
            get { return _StartCarPosition; }
            set
            {
                _StartCarPosition = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 车辆扫描终点位置
        /// </summary>
        private string _EndCarPosition { get; set; }
        public string EndCarPosition
        {
            get { return _EndCarPosition; }
            set
            {
                _EndCarPosition = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 车牌
        /// </summary>
        private string _licensePlate { get; set; }
        public string licensePlatetxt
        {
            get { return _licensePlate; }
            set
            {
                _licensePlate = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 车长
        /// </summary>
        private string _carLength { get; set; }
        public string carLength
        {
            get { return _carLength; }
            set
            {
                _carLength = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 任务ID
        /// </summary>
        private string _TaskId { get; set; }
        public string TaskId
        {
            get { return _TaskId; }
            set
            {
                _TaskId = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        private string _Delete { get; set; }
        public string Delete
        {
            get { return _Delete; }
            set
            {
                _Delete = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        private string _Scan { get; set; }
        public string Scan
        {
            get { return _Scan; }
            set
            {
                _Scan = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 停止扫描
        /// </summary>
        private string _StopScann { get; set; }
        public string StopScan
        {
            get { return _StopScann; }
            set
            {
                _StopScann = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        public ICommand _DelectCommand;
        public ICommand _ScanCommand;
        /// <summary>
        /// 删除
        /// </summary>
        public ICommand DelectCommand
        {
            get => _DelectCommand;
            set
            {
                _DelectCommand = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 扫描
        /// </summary>
        public ICommand ScanCommand
        {
            get => _ScanCommand;
            set
            {
                _ScanCommand = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 当前列表展示的所有 LogRecords
        /// </summary>
        private ObservableCollection<EnhanceScanCarInfo> _enhanceScanCarInfos = new ObservableCollection<EnhanceScanCarInfo>();

        public ObservableCollection<EnhanceScanCarInfo> enhanceScanCarInfos
        {
            get { return _enhanceScanCarInfos; }
            set
            {
                _enhanceScanCarInfos = value;
                RaisePropertyChanged();
            }
        }
        EnhanceScanCarInfo CurrentEnhanceScanCarInfo;
        public EnhanceScanPanelMvvm()
        {
            LoadUIText();
            LoadUI();
            LoadUIFontSize();
            BegoodServerController.GetInstance().EnhanceScan += EnhanceScanMsg;
            DelectCommand = new DrawerDefectCommand(Delect);
            ScanCommand = new DrawerDefectCommand(ScanEnhance);
            ScanImageService.GetInstance().MessaageShowAction += EnhanceScanEnd;
            ScanImageService.GetInstance().ClickAction += EnhanceScanStatus;
        }

        public override void LoadUIText()
        {
            EnhanceScan = CommonDeleget.UpdateStatusNameAction("EnhanceScan");
            StartCarPosition = CommonDeleget.UpdateStatusNameAction("StartCarPosition");
            EndCarPosition = CommonDeleget.UpdateStatusNameAction("EndCarPosition");
            licensePlatetxt = CommonDeleget.UpdateStatusNameAction("licensePlate");
            carLength = CommonDeleget.UpdateStatusNameAction("carLength");
            TaskId = CommonDeleget.UpdateStatusNameAction("TaskId");
            Delete = CommonDeleget.UpdateStatusNameAction("Delete");
            Scan = CommonDeleget.UpdateStatusNameAction("StartScan");
        }

        public void LoadUI()
        {
            EnhanceScan = CommonDeleget.UpdateStatusNameAction("EnhanceScan");
            StartCarPosition = CommonDeleget.UpdateStatusNameAction("StartCarPosition");
            EndCarPosition = CommonDeleget.UpdateStatusNameAction("EndCarPosition");
            licensePlatetxt = CommonDeleget.UpdateStatusNameAction("licensePlate");
            carLength = CommonDeleget.UpdateStatusNameAction("carLength");
            TaskId = CommonDeleget.UpdateStatusNameAction("TaskId");
            Delete = CommonDeleget.UpdateStatusNameAction("Delete");
            Scan = CommonDeleget.UpdateStatusNameAction("StartScan");
        }

        public void LoadEnhanceScanName()
        {
           
        }
        public void EnhanceScanBar(object pc)
        {
           
        }
        /// <summary>
        /// 接收到增强扫描信息之后 添加到界面
        /// </summary>
        /// <param name="EnhanceScanMsgs"></param>
        public void EnhanceScanMsg(List<string> EnhanceScanMsgs)
        {
            //只有主动模式需要显示增强扫描信息，被动先不做
            if(PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.ScanMode))
            {
                IsShowEnhanceScanPanel = true;
                foreach (var EnhanceScanItem in EnhanceScanMsgs)
                {
                    EnhanceScanCarInfo enhanceScanCarInfo = CommonFunc.JsonToObject<EnhanceScanCarInfo>(EnhanceScanItem);
                    Application.Current.Dispatcher.BeginInvoke(new Action(() => { enhanceScanCarInfos.Add(enhanceScanCarInfo); }));
                }
            }
            ///TODO 这里收到消息之后准备把数据给PLC并且告诉PLC是要增强了
        }
        /// <summary>
        /// 主动模式下增强扫描完成之后，将增强扫描信号归位
        /// </summary>
        /// <param name="Msg"></param>
        private void EnhanceScanEnd(string Msg)
        {
            //将增强扫描面板隐藏
            IsShowEnhanceScanPanel = false;
            if (CommandDic.ContainsKey(Command.EnhanceScan))
            {
                //TODO 发送PLC 增强扫描信号为0
                PLCControllerManager.GetInstance().WritePositionValue(CommandDic[Command.EnhanceScan], false);
            }
        }
        /// <summary>
        /// 扫描状态变化
        /// </summary>
        /// <param name="Msg"></param>
        private void EnhanceScanStatus(bool isScaning)
        {
            if (PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.ScanMode))
            {
                if (!isScaning)
                {
                    //将增强扫描面板隐藏
                    IsShowEnhanceScanPanel = false;
                    if(CommandDic.ContainsKey(Command.EnhanceScan))
                    {
                        //TODO 发送PLC 增强扫描信号为0
                        PLCControllerManager.GetInstance().WritePositionValue(CommandDic[Command.EnhanceScan], false);
                    }
                }
            }
        }
        public void Delect(object pc)
        {
            try
            {

                if (MessageBox.Show(UpdateStatusNameAction("AskDelete"), UpdateStatusNameAction("Warning"), MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        CurrentEnhanceScanCarInfo = pc as EnhanceScanCarInfo;
                        enhanceScanCarInfos.Remove(CurrentEnhanceScanCarInfo);
                        CurrentEnhanceScanCarInfo = null;
                        if (enhanceScanCarInfos.Count == 0)
                        {
                            IsShowEnhanceScanPanel = false; //CurrentEnhanceScanCarInfo
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.GetDistance().WriteErrorLogs(ex.StackTrace);
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                WriteLogAction(ex.StackTrace, LogType.ApplicationError);
            }
        }
        public void ScanEnhance(object pc)
        {
            try
            {
                CurrentEnhanceScanCarInfo = pc as EnhanceScanCarInfo;
                if(CurrentEnhanceScanCarInfo!=null)
                {
                    if (CommandDic.ContainsKey(Command.EnhanceScan))
                    {
                        PLCControllerManager.GetInstance().WritePositionValue(CommandDic[Command.EnhanceStartPosition], CurrentEnhanceScanCarInfo.StartCarPosition);
                        PLCControllerManager.GetInstance().WritePositionValue(CommandDic[Command.EnhanceEndPosition], CurrentEnhanceScanCarInfo.EndCarPosition);
                        PLCControllerManager.GetInstance().WritePositionValue(CommandDic[Command.EnhanceTotalPosition], CurrentEnhanceScanCarInfo.ImageWidth);
                        PLCControllerManager.GetInstance().WritePositionValue(CommandDic[Command.EnhanceScan], true);
                        EnhanceScan(true, CurrentEnhanceScanCarInfo);
                        //扫描完成后如果这个不为空，就把这个删除
                        enhanceScanCarInfos.Remove(CurrentEnhanceScanCarInfo);
                        if (enhanceScanCarInfos.Count == 0)
                        {
                            IsShowEnhanceScanPanel = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLogAction(ex.StackTrace, LogType.ApplicationError);
            }
        }
    }
}
