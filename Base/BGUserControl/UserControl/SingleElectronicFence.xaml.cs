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
    /// SingleElectronicFence.xaml 的交互逻辑
    /// </summary>
    public partial class SingleElectronicFence : UserControl
    {
        /// <summary>
        /// 表示当前的等待状态
        /// </summary>
        [Bindable(true)]
        public string ElectronFenceBattery
        {
            get
            {
                return (string)GetValue(ElectronFenceBatteryProperty);
            }
            set { SetValue(ElectronFenceBatteryProperty, value); }
        }

        public static readonly DependencyProperty ElectronFenceBatteryProperty =
            DependencyProperty.Register("ElectronFenceBattery", typeof(string), typeof(SingleElectronicFence),
                new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnValueChange)));

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SingleElectronicFence SingleElectronicFence = d as SingleElectronicFence;
            if (SingleElectronicFence != null && SingleElectronicFence.ElectronFenceBattery != null)
            {
                SingleElectronicFenceViewModel singleElectronicFenceVM = SingleElectronicFence.DataContext as SingleElectronicFenceViewModel;
                singleElectronicFenceVM.ElectronicFenceBattary = SingleElectronicFence.ElectronFenceBattery;
            }
        }
        /// <summary>
        /// 表示当前的等待状态
        /// </summary>
        [Bindable(true)]
        public string ElectronFenceAlarm
        {
            get
            {
                return (string)GetValue(ElectronFenceAlarmProperty);
            }
            set { SetValue(ElectronFenceAlarmProperty, value); }
        }

        public static readonly DependencyProperty ElectronFenceAlarmProperty =
            DependencyProperty.Register("ElectronFenceAlarm", typeof(string), typeof(SingleElectronicFence),
                new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnAlarmValueChange)));

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnAlarmValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SingleElectronicFence SingleElectronicFence = d as SingleElectronicFence;
            if (SingleElectronicFence != null && SingleElectronicFence.ElectronFenceAlarm != null)
            {
               
            }
        }

        SingleElectronicFenceViewModel singleElectronicFenceViewModel;
        public SingleElectronicFence()
        {
            InitializeComponent();
            InitViewModel();
            DataContext = singleElectronicFenceViewModel;
        }

        public void InitViewModel()
        {
            singleElectronicFenceViewModel = new SingleElectronicFenceViewModel();
        }
    }

    public class SingleElectronicFenceViewModel : BaseMvvm
    {
        /// <summary>
        /// 电子围栏报警
        /// </summary>
        private string _ElectronicFenceAlarm;
        public string ElectronicFenceAlarm
        { 
            get=> _ElectronicFenceAlarm;
            set {
                _ElectronicFenceAlarm = value;
                if (Convert.ToInt32(value) == 1)
                {
                    ElectronicFenceAlarmFrg = Brushes.Red;
                }
                else
                {
                    ElectronicFenceAlarmFrg = Brushes.Green;
                }
                RaisePropertyChanged("ElectronicFenceAlarm");
            }
        }

        /// <summary>
        /// 电子围栏电池电量
        /// </summary>
        private string _ElectronicFenceBattary = "100";
        public string ElectronicFenceBattary
        {
            get => _ElectronicFenceBattary;
            set
            {
                _ElectronicFenceBattary = value;
                if(Convert.ToInt32(value) < 30)
                {
                    ElectronicFenceBattaryFrg = Brushes.Red;
                }
                else
                {
                    ElectronicFenceBattaryFrg = Brushes.Green;
                }    
                RaisePropertyChanged("ElectronicFenceBattary");
            }
        }

        /// <summary>
        /// 电子围栏剂量前景色
        /// </summary>
        private Brush _ElectronicFenceAlarmFrg = Brushes.Green;
        public Brush ElectronicFenceAlarmFrg
        {
            get => _ElectronicFenceAlarmFrg;
            set
            {
                _ElectronicFenceAlarmFrg = value;
                RaisePropertyChanged("ElectronicFenceAlarmFrg");
            }
        }

        /// <summary>
        /// 电子围栏剂量电池前景色
        /// </summary>
        private Brush _ElectronicFenceBattaryFrg = Brushes.Green;
        public Brush ElectronicFenceBattaryFrg
        {
            get => _ElectronicFenceBattaryFrg;
            set
            {
                _ElectronicFenceBattaryFrg = value;
                RaisePropertyChanged("ElectronicFenceBattaryFrg");
            }
        }

        public SingleElectronicFenceViewModel()
        {
            ElectronicFenceBattaryFrg = Brushes.Green;
            ElectronicFenceAlarmFrg = Brushes.Green;
        }
    }
}
