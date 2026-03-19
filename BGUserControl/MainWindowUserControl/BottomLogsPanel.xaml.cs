using CMW.Common.Utilities;
using BGModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using BG_Entities;
using BGUserControl;

namespace BGUserControl
{
    /// <summary>
    /// BottomLogsPanel.xaml 的交互逻辑
    /// </summary>
    public partial class BottomLogsPanel : UserControl
    {
        BottomLogsPanelMvvm bottomLogsPanelMvvm = new BottomLogsPanelMvvm();
        public BottomLogsPanel()
        {
            InitializeComponent();
            DataContext = bottomLogsPanelMvvm;
            lv_Status.ItemsSource = bottomLogsPanelMvvm.ListViewLogViewModeList;
            Loaded += BottomLogsPanel_Loaded;
        }

        private void BottomLogsPanel_Loaded(object sender, RoutedEventArgs e)
        {
            bottomLogsPanelMvvm.ListViewHeaderLength = lv_Status.ActualWidth - 200;
        }
    }

    public class BottomLogsPanelMvvm : BaseMvvm
    {
        private string _runLog = string.Empty;
        public string runLog
        {
            get => _runLog;
            set
            {
                _runLog = value;
                RaisePropertyChanged("runLog");
            }
        }

        private string _time = string.Empty;
        public string time
        {
            get => _time;
            set
            {
                _time = value;
                RaisePropertyChanged("time");
            }
        }

        private double _ListViewHeaderLength = double.NaN;
        public double ListViewHeaderLength
        {
            get => _ListViewHeaderLength;
            set
            {
                _ListViewHeaderLength = value;
                RaisePropertyChanged("ListViewHeaderLength");
            }
        }

        /// <summary>
        /// 记录前台操作的全局对象
        /// </summary>
        public UIList<ListViewLogViewMode> ListViewLogViewModeList = new UIList<ListViewLogViewMode>() { };

        public BottomLogsPanelMvvm()
        {
            InitAction();
            LoadUIText();
            LoadUIFontSize();
        }
        private void InitAction()
        {
            CommonDeleget.BuryingPointActionEvent -= BuryingPointActionEvent;
            CommonDeleget.BuryingPointActionEvent += BuryingPointActionEvent;
        }

        public override void LoadUIText()
        {
            runLog = UpdateStatusNameAction("RunLog");
            time = UpdateStatusNameAction("Time");
        }

        private void BuryingPointActionEvent(string Message)
        {
            WriteLogAction(Message, LogType.ScanStep, true);
            var First = new ListViewLogViewMode() { EquipmentTime = DateTime.Now.ToString(), EquipmentRunLog = Message };
            Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                if (ListViewLogViewModeList.Count > 99)
                {
                    ListViewLogViewModeList.RemoveAt(ListViewLogViewModeList.Count - 1);
                }
                ListViewLogViewModeList.InsertAt(0, First);
            });
        }
    }
}
