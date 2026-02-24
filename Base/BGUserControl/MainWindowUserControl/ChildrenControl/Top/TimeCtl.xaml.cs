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
using System.Windows.Threading;

namespace BGUserControl
{
    /// <summary>
    /// TimeCtl.xaml 的交互逻辑
    /// </summary>
    public partial class TimeCtl : UserControl
    {
        DispatcherTimer DataTimer;
        public TimeCtl()
        {
            InitializeComponent();
            IsVisibleChanged += TimeCtl_IsVisibleChanged;
        }

        private void TimeCtl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue as bool? != null && e.NewValue as bool? == false)
            {
                DataTimer?.Stop();
            }
            else if(e.NewValue as bool? != null && e.NewValue as bool? == true)
            {
                StartTime();
            }
        }

        public void StartTime()
        {
            DataTimer = new DispatcherTimer() { Interval = new TimeSpan(1000)};
            DataTimer.Tick += DataTimer_Elapsed;
            DataTimer.Start();
        }

        private void DataTimer_Elapsed(object sender, EventArgs e)
        {
            lbl_Time.Content = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }
    }
}
