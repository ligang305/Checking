using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BGUserControl
{
    public partial class DiyEvent : ResourceDictionary
    {
        //public DiyEvent() { InitializeComponent(); }

        //public void InitializeComponent()
        //{

        //}

        private void Bd_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("你成功了！");
        }
    }
}
