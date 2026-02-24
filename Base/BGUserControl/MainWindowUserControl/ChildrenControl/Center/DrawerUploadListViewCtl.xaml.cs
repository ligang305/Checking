using BGModel;
using BGUserControl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;

namespace BGUserControl
{
    /// <summary>
    /// DrawerUploadListViewCtl.xaml 的交互逻辑
    /// </summary>
    public partial class DrawerUploadListViewCtl : UserControl
    {
        DrawerUploadListViewCtlMvvm drawerUploadListView = null;
        public DrawerUploadListViewCtl()
        {
            InitializeComponent();
            drawerUploadListView = new DrawerUploadListViewCtlMvvm(DiyImage);
            DataContext = drawerUploadListView;
        }
    }


    public class DrawerUploadListViewCtlMvvm : BaseMvvm
    {
        Visibsliy _ListViewVisibsliy = new Visibsliy() { IsShow = Visibility.Hidden};
        Storyboard sto = new Storyboard();
        ThicknessAnimation da = new ThicknessAnimation();
        DiyUploadListView DiyImage = new DiyUploadListView();
        ICommand _DrawerDeffectCommand = null;
        public ICommand DrawerDeffectCommand { get { return _DrawerDeffectCommand; } set { DrawerDeffectCommand = value; } }
        public Visibsliy ListViewVisibsliy
        {
            get { return _ListViewVisibsliy; }
            set { _ListViewVisibsliy = value; }
        }

        public DrawerUploadListViewCtlMvvm(DiyUploadListView _DiyImage)
        {
            DiyImage = _DiyImage;
            _DrawerDeffectCommand = new DrawerDefectCommand(DrawerDefect);
            LoadUIText();
            LoadUIFontSize();
        }

        /// <summary>
        /// 控制动画的方法 上传图片
        /// </summary>
        /// <param name="vb"></param>
        public void VisualOrHidden(Visibility vb)
        {
            Debug.WriteLine("-------------------------ganggang_upload_img9-----------------------");
            if (vb == Visibility.Visible)
            {
                ListViewVisibsliy.IsShow = Visibility.Hidden;
                sto.Stop();
                sto.Children.Clear();
                da.From = new Thickness(0, -300, -30, 0);
                da.To = new Thickness(0, -300, -450, 0);
                da.Duration = new Duration(TimeSpan.FromSeconds(0.5));
                Storyboard.SetTarget(da, DiyImage);
                Storyboard.SetTargetProperty(da, new PropertyPath("Margin"));
                sto.Children.Add(da);
                sto.Begin();
            }
            else
            {
                sto.Stop();
                sto.Children.Clear();
                ListViewVisibsliy.IsShow = Visibility.Visible;
                da.From = new Thickness(0, -300, -450, 0);
                da.To = new Thickness(0, -300, -30, 0);
                da.Duration = new Duration(TimeSpan.FromSeconds(0.5));
                Storyboard.SetTarget(da, DiyImage);
                Storyboard.SetTargetProperty(da, new PropertyPath("Margin"));
                sto.Children.Add(da);
                sto.Begin();
            }
        }

        public void DrawerDefect(object pc)
        {
            VisualOrHidden(ListViewVisibsliy.IsShow);
        }

        public override void LoadUIText()
        {
            ListViewVisibsliy.DisplayName = UpdateStatusNameAction("ScanRecode");
        }
   }
}
