using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// VJ_RaySourceMonitor.xaml 的交互逻辑
    /// </summary>
    public partial class VJ_RaySourceMonitor : UserControl
    {
        VJ_RaySourceMonitorViewModel vJ_RaySourceMonitorViewModel = new VJ_RaySourceMonitorViewModel();
        public VJ_RaySourceMonitor()
        {
            InitializeComponent();
            DataContext = vJ_RaySourceMonitorViewModel;
            InitPanel();
        }


        private void InitPanel()
        {
            MainPanel.Children.Clear();
            string VJ_DoseDir = SystemDirectoryConfig.GetInstance().GetVJ_RaySourceDoseValueDir(BG_Entities.ControlVersion.BS);
            if(!Directory.Exists(VJ_DoseDir))
            {
                return;
            }
            foreach (var DoseFileItem in Directory.GetFiles(VJ_DoseDir))
            {
                if(File.Exists(DoseFileItem))
                {
                    VJ_RaySourceMonitorControl vJ_RaySourceMonitorControl = new VJ_RaySourceMonitorControl();
                    vJ_RaySourceMonitorControl.ItemSource = DoseFileItem;
                    MainPanel.Children.Add(vJ_RaySourceMonitorControl);
                }
            }
        }
    }
}
