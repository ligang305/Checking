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

namespace BGUserControl
{
    /// <summary>
    /// DiyCacheDelete.xaml 的交互逻辑
    /// </summary>
    public partial class DiyCacheDelete : UserControl
    {
        ClearCacheMvvm clearCacheMvvm = new ClearCacheMvvm();
        public DiyCacheDelete()
        {
            InitializeComponent();
            MainGrid.DataContext = clearCacheMvvm;
        }

        private void btnClearImageCache_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border current = sender as Border;
            current.IsEnabled = false;
            clearCacheMvvm.ClearCache(current.Tag as string);
            current.IsEnabled = true;
        }
    }
}
