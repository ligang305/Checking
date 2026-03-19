using BG_Services;
using CMW.Common.Utilities;
using BGModel;
using System;
using System.Collections.Generic;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.ImageImportDll;
using BG_Entities;
using BGUserControl;
using BG_WorkFlow;

namespace BGUserControl
{
    /// <summary>
    /// PreviewScanImageCtl.xaml 的交互逻辑
    /// </summary>
    public partial class PreviewScanImageCtl : UserControl
    {
        PreviewScanImageMvvm previewScanImageMvvm = null;

        public PreviewScanImageCtl()
        {
            InitializeComponent();
            //因为背散界面是多视角切不是单独H986设备，所以如果当前插件是背散版本的则不进行显示
            if (ConfigServices.GetInstance().localConfigModel.IsUserBS)
            {
                this.Visibility = Visibility.Collapsed;
            }
            previewScanImageMvvm = new PreviewScanImageMvvm(bdScanImage);
            DataContext = previewScanImageMvvm;
        }
    }

    public class PreviewScanImageMvvm : BaseMvvm
    {
        Visibsliy _ScanImageVisibsliy = new Visibsliy()
        {
            IsShow = Visibility.Hidden 
        };
        Visibsliy _ScanImageDoubleTipVisibsliy = new Visibsliy() { IsShow = Visibility.Hidden};
        public ICommand _DrawerDefectCommand;
        public ICommand _OpenDoubleClickCommand;
        Storyboard imageSto = new Storyboard();
        ThicknessAnimation imageDa = new ThicknessAnimation();
        StackPanel bdScanImage = new StackPanel();
        public Visibsliy ScanImageVisibsliy 
        {
            get => _ScanImageVisibsliy;
            set {
                _ScanImageVisibsliy = value; RaisePropertyChanged("ScanImageVisibsliy");
            }
        }
        public Visibsliy ScanImageDoubleTipVisibsliy 
        {
            get => _ScanImageDoubleTipVisibsliy;
            set 
            {
                _ScanImageDoubleTipVisibsliy = value; 
                RaisePropertyChanged("ScanImageDoubleTipVisibsliy"); 
            }
        }
        /// <summary>
        /// 抽拉命令
        /// </summary>
        public ICommand DrawerDefectCommand
        {
            get => _DrawerDefectCommand;
            set
            {
                _DrawerDefectCommand = value;
                RaisePropertyChanged("DrawerDefectCommand");
            }
        }
        /// <summary>
        /// 双击打开大图命令
        /// </summary>
        public ICommand DoubleClickCommand
        {
            get => _OpenDoubleClickCommand;
            set
            {
                _OpenDoubleClickCommand = value;
                RaisePropertyChanged("DoubleClickCommand");
            }
        }
        public PreviewScanImageMvvm(StackPanel _bdScanImage)
        {
            LoadUIText();
            LoadUIFontSize();
            _DrawerDefectCommand = new DrawerDefectCommand(DrawerDefect);
            _OpenDoubleClickCommand = new DrawerDefectCommand(DoubleOpenClickImage);
            bdScanImage = _bdScanImage;
        }
        public void DrawerDefect(object pc)
        {
            try
            {
                VisualOrHiddenForImage(ScanImageVisibsliy.IsShow);
            }
            catch (Exception ex)
            {
                WriteLogAction(ex.StackTrace, LogType.ApplicationError);
            }
        }
        public void DoubleOpenClickImage(object pc)
        {
            try
            {
                ButtonInvoke.DoubleClickToOpenAction(!(_ScanImageDoubleTipVisibsliy.IsShow == Visibility.Hidden));
                _ScanImageDoubleTipVisibsliy.IsShow = _ScanImageDoubleTipVisibsliy.IsShow == 
                    Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
            }
            catch (Exception ex)
            {
                WriteLogAction(ex.StackTrace, LogType.ApplicationError);
            }
        }
        public void VisualOrHiddenForImage(Visibility vb)
        {
            if (vb == Visibility.Visible)
            {
                ScanImageVisibsliy.IsShow = Visibility.Hidden;
                imageSto.Stop();
                imageSto.Children.Clear();
                imageDa.From = new Thickness(0, 0, 0, 0);
                imageDa.To = new Thickness(0, 0, -310, 0);
                imageDa.Duration = new Duration(TimeSpan.FromSeconds(0.5));
                Storyboard.SetTarget(imageDa, bdScanImage);
                Storyboard.SetTargetProperty(imageDa, new PropertyPath("Margin"));
                imageSto.Children.Add(imageDa);
                imageSto.Begin();
            }
            else
            {
                imageSto.Stop();
                imageSto.Children.Clear();
                ScanImageVisibsliy.IsShow = Visibility.Visible;
                imageSto.Stop();
                imageSto.Children.Clear();
                imageDa.From = new Thickness(0, 0, -310, 0);
                imageDa.To = new Thickness(0, 0, 0, 0);
                imageDa.Duration = new Duration(TimeSpan.FromSeconds(0.5));
                Storyboard.SetTarget(imageDa, bdScanImage);
                Storyboard.SetTargetProperty(imageDa, new PropertyPath("Margin"));
                imageSto.Children.Add(imageDa);
                imageSto.Begin();
            }
        }
        public override void LoadUIText()
        {
            ScanImageVisibsliy.DisplayName = UpdateStatusNameAction("DoubleClickToOpenBigPic");
            ScanImageDoubleTipVisibsliy.DisplayName = UpdateStatusNameAction("ScanPriview");
        }
    }
}
