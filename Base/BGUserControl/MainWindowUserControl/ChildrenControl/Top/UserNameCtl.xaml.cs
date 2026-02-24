using BG_Services;
using BG_WorkFlow;
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

namespace BGUserControl
{
    /// <summary>
    /// UserNameCtl.xaml 的交互逻辑
    /// </summary>
    public partial class UserNameCtl : UserControl
    {
        public UserNameCtl()
        {
            InitializeComponent();
            lbl_UserName.Content = UpdateStatusNameAction(ConfigServices.GetInstance().localConfigModel.Login.LoginCode);
        }
    }
}
