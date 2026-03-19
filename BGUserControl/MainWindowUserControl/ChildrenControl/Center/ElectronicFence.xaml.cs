using BG_Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace BGUserControl
{
    /// <summary>
    /// ElectronicFence.xaml 的交互逻辑
    /// </summary>
    public partial class ElectronicFence : UserControl
    {
        ElectronicFenceViewModel _ElectronicFenceViewModel;

        #region prop
        /// <summary>
        /// 表示当前的等待状态
        /// </summary>
        [Bindable(true)]
        public string ElectronFenceOneBattery
        {
            get
            {
                return (string)GetValue(ElectronFenceOneBatteryProperty);
            }
            set { SetValue(ElectronFenceOneBatteryProperty, value); }
        }

        public static readonly DependencyProperty ElectronFenceOneBatteryProperty =
            DependencyProperty.Register("ElectronFenceOneBattery", typeof(string), typeof(ElectronicFence),
                new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnValueChange)));

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ElectronicFence ElectronicFence = d as ElectronicFence;
            if (ElectronicFence != null && ElectronicFence.ElectronFenceOneBattery != null)
            {
                ElectronicFence.ElectronFenceLeftUp.ElectronFenceBattery = ElectronicFence.ElectronFenceOneBattery;
            }
        }
        /// <summary>
        /// 表示当前的等待状态
        /// </summary>
        [Bindable(true)]
        public string ElectronFenceTwoBattery
        {
            get
            {
                return (string)GetValue(ElectronFenceTwoBatteryProperty);
            }
            set { SetValue(ElectronFenceTwoBatteryProperty, value); }
        }

        public static readonly DependencyProperty ElectronFenceTwoBatteryProperty =
            DependencyProperty.Register("ElectronFenceTwoBattery", typeof(string), typeof(ElectronicFence),
                new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnTwoValueChange)));

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnTwoValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ElectronicFence ElectronicFence = d as ElectronicFence;
            if (ElectronicFence != null && ElectronicFence.ElectronFenceTwoBattery != null)
            {
                ElectronicFence.ElectronFenceRightUp.ElectronFenceBattery = ElectronicFence.ElectronFenceTwoBattery;
            }
        }
        /// <summary>
        /// 表示当前的等待状态
        /// </summary>
        [Bindable(true)]
        public string ElectronFenceThreeBattery
        {
            get
            {
                return (string)GetValue(ElectronFenceThreeBatteryProperty);
            }
            set { SetValue(ElectronFenceThreeBatteryProperty, value); }
        }

        public static readonly DependencyProperty ElectronFenceThreeBatteryProperty =
            DependencyProperty.Register("ElectronFenceThreeBattery", typeof(string), typeof(ElectronicFence),
                new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnThreeValueChange)));

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnThreeValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ElectronicFence ElectronicFence = d as ElectronicFence;
            if (ElectronicFence != null && ElectronicFence.ElectronFenceThreeBattery != null)
            {
                ElectronicFence.ElectronFenceLeftDown.ElectronFenceBattery = ElectronicFence.ElectronFenceThreeBattery;
            }
        }

        /// <summary>
        /// 表示当前的等待状态
        /// </summary>
        [Bindable(true)]
        public string ElectronFenceFourBattery
        {
            get
            {
                return (string)GetValue(ElectronFenceFourBatteryProperty);
            }
            set { SetValue(ElectronFenceFourBatteryProperty, value); }
        }

        public static readonly DependencyProperty ElectronFenceFourBatteryProperty =
            DependencyProperty.Register("ElectronFenceFourBattery", typeof(string), typeof(ElectronicFence),
                new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnFourValueChange)));

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnFourValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ElectronicFence ElectronicFence = d as ElectronicFence;
            if (ElectronicFence != null && ElectronicFence.ElectronFenceFourBattery != null)
            {
                ElectronicFence.ElectronFenceRightDown.ElectronFenceBattery = ElectronicFence.ElectronFenceFourBattery;
            }
        }
        #endregion

        public ElectronicFence()
        {
            InitializeComponent();
            _ElectronicFenceViewModel = new ElectronicFenceViewModel();
            DataContext = _ElectronicFenceViewModel;

            //ElectronFenceLeftUp.SetBinding(SingleElectronicFence.ElectronFenceAlarmProperty, new Binding("ElectronFenceUpAlarm") { Mode = BindingMode.TwoWay, Source = _ElectronicFenceViewModel });
            //ElectronFenceLeftDown.SetBinding(SingleElectronicFence.ElectronFenceAlarmProperty, new Binding("ElectronFenceDownAlarm") { Mode = BindingMode.TwoWay, Source = _ElectronicFenceViewModel });
            //ElectronFenceRightUp.SetBinding(SingleElectronicFence.ElectronFenceAlarmProperty, new Binding("ElectronFenceLeftAlarm") { Mode = BindingMode.TwoWay, Source = _ElectronicFenceViewModel });
            //ElectronFenceRightDown.SetBinding(SingleElectronicFence.ElectronFenceAlarmProperty, new Binding("ElectronFenceRightAlarm") { Mode = BindingMode.TwoWay, Source = _ElectronicFenceViewModel });

            ElectronFenceLeftUp.SetBinding(SingleElectronicFence.ElectronFenceBatteryProperty,new Binding("ElectronFenceOneBattery") { Mode = BindingMode.TwoWay,Source = _ElectronicFenceViewModel });
            ElectronFenceLeftDown.SetBinding(SingleElectronicFence.ElectronFenceBatteryProperty, new Binding("ElectronFenceThreeBattery") { Mode = BindingMode.TwoWay, Source = _ElectronicFenceViewModel });
            ElectronFenceRightUp.SetBinding(SingleElectronicFence.ElectronFenceBatteryProperty, new Binding("ElectronFenceTwoBattery") { Mode = BindingMode.TwoWay, Source = _ElectronicFenceViewModel });
            ElectronFenceRightDown.SetBinding(SingleElectronicFence.ElectronFenceBatteryProperty, new Binding("ElectronFenceFourBattery") { Mode = BindingMode.TwoWay, Source = _ElectronicFenceViewModel });
        }
    }

    public class ElectronicFenceViewModel : BaseMvvm
    {
        private string _ElectronFenceOneBattery;
        private string _ElectronFenceTwoBattery;
        private string _ElectronFenceThreeBattery;
        private string _ElectronFenceFourBattery;

        private Brush _ElectronFenceUpAlarm;
        private Brush _ElectronFenceDownAlarm;
        private Brush _ElectronFenceLeftAlarm;
        private Brush _ElectronFenceRightAlarm;

        public string ElectronFenceOneBattery
        {
            get => _ElectronFenceOneBattery;
            set { _ElectronFenceOneBattery = value;RaisePropertyChanged("ElectronFenceOneBattery"); }
        }
        public string ElectronFenceTwoBattery
        {
            get => _ElectronFenceTwoBattery;
            set { _ElectronFenceTwoBattery = value; RaisePropertyChanged("ElectronFenceTwoBattery"); }
        }
        public string ElectronFenceThreeBattery
        {
            get => _ElectronFenceThreeBattery;
            set { _ElectronFenceThreeBattery = value; RaisePropertyChanged("ElectronFenceThreeBattery"); }
        }
        public string ElectronFenceFourBattery
        {
            get => _ElectronFenceFourBattery;
            set { _ElectronFenceFourBattery = value; RaisePropertyChanged("ElectronFenceFourBattery"); }
        }

        public Brush ElectronFenceUpAlarm
        {
            get => _ElectronFenceUpAlarm;
            set { _ElectronFenceUpAlarm = value; RaisePropertyChanged("ElectronFenceUpAlarm"); }
        }
        public Brush ElectronFenceDownAlarm
        {
            get => _ElectronFenceDownAlarm;
            set { _ElectronFenceDownAlarm = value; RaisePropertyChanged("ElectronFenceDownAlarm"); }
        }
        public Brush ElectronFenceLeftAlarm
        {
            get => _ElectronFenceLeftAlarm;
            set { _ElectronFenceLeftAlarm = value; RaisePropertyChanged("ElectronFenceLeftAlarm"); }
        }
        public Brush ElectronFenceRightAlarm
        {
            get => _ElectronFenceRightAlarm;
            set { _ElectronFenceRightAlarm = value; RaisePropertyChanged("ElectronFenceRightAlarm"); }
        }

        protected override void ConnectionStatus(bool ConnectStatus)
        {
            if (ConnectStatus)
            {
                ElectronFenceOneBattery = PLCControllerManager.GetInstance().GetPLCDBStatus().IntArray[0].ToString();
                ElectronFenceTwoBattery = PLCControllerManager.GetInstance().GetPLCDBStatus().IntArray[1].ToString();
                ElectronFenceThreeBattery = PLCControllerManager.GetInstance().GetPLCDBStatus().IntArray[2].ToString();
                ElectronFenceFourBattery = PLCControllerManager.GetInstance().GetPLCDBStatus().IntArray[3].ToString();

                ElectronFenceUpAlarm = PLCControllerManager.GetInstance().GetStatusByPositionEnum(BG_Entities.PLCPositionEnum.AfterProtectionZonecrashes) ? Brushes.Red : Brushes.Green;
                ElectronFenceDownAlarm = PLCControllerManager.GetInstance().GetStatusByPositionEnum(BG_Entities.PLCPositionEnum.BeforeProtectionZonecrashes) ? Brushes.Red : Brushes.Green;
                ElectronFenceLeftAlarm = PLCControllerManager.GetInstance().GetStatusByPositionEnum(BG_Entities.PLCPositionEnum.LeftProtectionZonecrashes) ? Brushes.Red : Brushes.Green;
                ElectronFenceRightAlarm = PLCControllerManager.GetInstance().GetStatusByPositionEnum(BG_Entities.PLCPositionEnum.RightProtectionZonecrashes) ? Brushes.Red : Brushes.Green;
            }
        }
    }
}
