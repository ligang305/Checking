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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CMW.Common.Utilities;
using BGModel;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.CommonFunc;
using static CMW.Common.Utilities.ImageImportDll;
using static CMW.Common.Utilities.KeyBoard;
using BG_Entities;
using BG_Services;

namespace BGUserControl
{
    /// <summary>
    /// 手动出束的
    /// </summary>
    public partial class DiyBetratronHandRay : BaseWindows
    {
        Visibsliy RayVisibleObject = new Visibsliy();
        bool? isVisible = false;
        /// <summary>
        /// 手动出束的自定义控件
        /// </summary>
        public DiyBetratronHandRay()
        {
            InitializeComponent();
            InitStopPanel();
            IsVisibleChanged += DiyHandRay_IsVisibleChanged;
        }

        private void DiyHandRay_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            isVisible = e.NewValue as bool?;
            if (isVisible != null && isVisible == true)
            {
                Task.Factory.StartNew(() => {
                    InqureStatusThread();
                });
            }
        }

        
        /// <summary>
        /// 手动出束按钮
        /// </summary>
        /// <param name="spBtnPanel"></param>
        /// <returns></returns>
        private void InitStopPanel()
        {
            StackPanel spBtnPanel = new StackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Right,VerticalAlignment = VerticalAlignment.Stretch,
                Orientation = Orientation.Horizontal,Margin = new Thickness(0,0,10,0)
            };
            Border BtnOutRay = new Border()
            {
                Width = 90,
                Name = "StartRay",
                Height = 30,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 5, 0),
                Style = (Style)this.FindResource("diyBtnCarHand"),
            };

            Label btnOutRay = new Label()
            {
                Content = (string)this.TryFindResource("Out Beam"),
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#FFFFFF")
            };
            BtnOutRay.Child = btnOutRay;
            Border BtnStopRay = new Border()
            {
                Width = 90,
                Height = 30,
                Name = "StopRay",
                Style = (Style)this.FindResource("diyBtnCarHand"),
            };
            Label btnStopRay = new Label()
            {
                Content = (string)this.TryFindResource("Stop Beam"),
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#FFFFFF")
            };
            BtnStopRay.Child = btnStopRay;
            BtnStopRay.Tag = "BtnStopRay";
            BtnOutRay.Tag = "BtnOutRay";
            BtnStopRay.MouseDown += BtnStopRay_PreviewMouseUp;
            BtnOutRay.MouseDown += BtnStopRay_PreviewMouseUp;
            Label lblRay = new Label()
            {
                Margin = new Thickness(10, 0, 10, 0),
                Foreground = StrToBrush("#1A4F85"),
                Style = (Style)this.FindResource("diyLabel"),
                FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.Normal),
            };
            lblRay.SetBinding(Label.ContentProperty, new Binding("DisplayName") { Source = RayVisibleObject });
            spBtnPanel.Children.Add(lblRay);
            spBtnPanel.Children.Add(BtnOutRay);
            spBtnPanel.Children.Add(BtnStopRay);
            Content = spBtnPanel;
        }

        /// <summary>
        /// 状态查询
        /// </summary>
        private void InqureStatus()
        {
            if (BoostingControllerManager.GetInstance().IsRayOut())
            {
                RayVisibleObject.DisplayName = (string)this.TryFindResource("OutOfBeam");
            }
            else
            {
                RayVisibleObject.DisplayName = (string)this.TryFindResource("NotBeaming");
            }
        }

        /// <summary>
        /// 查询状态的线程
        /// </summary>
        private void InqureStatusThread()
        {
            while (true)
            {
                Thread.Sleep(150);
                if (isVisible != true) return;
                InqureStatus();
            }
        }

        private void BtnStopRay_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                string Flag = (sender as Border).Tag as string;
                if (Flag == "BtnOutRay")
                {
                    if (!Common.IsConnection)
                    {
                        BG_MESSAGEBOX.Show(Application.Current.MainWindow, (string)this.TryFindResource("Tip"), $"{UnConnectionWithPlc}");
                    }

                    if (!(Common.GlobalRetStatus[12] || isSafeSerices))
                    {
                        string Content = (string) this.TryFindResource("HandRaySure");
                        if (MessageBox.Show(Content, (string)this.TryFindResource("Tip"), MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            //判断如果是出束中点击出束就不进行任何操作
                            if (!BoostingControllerManager.GetInstance().IsRayOut())
                            {
                                SetCommand(CommandDic[Command.StopRay], true);
                                //var isSuccess = GlobalPLCProtocol.Execute(21, 2, true);

                                BG_MESSAGEBOX.Show(Application.Current.MainWindow, (string)this.TryFindResource("Tip"), $"命令已发出");
                            }
                        }
                        return;
                    }

                    //判断如果是出束中点击出束就不进行任何操作
                    if (!BoostingControllerManager.GetInstance().IsRayOut())
                    {
                        SetCommand(CommandDic[Command.StopRay], true);
                        //var isSuccess = GlobalPLCProtocol.Execute(21, 2, true);

                        BG_MESSAGEBOX.Show(Application.Current.MainWindow, (string)this.TryFindResource("Tip"), $"命令已发出");
                    }
                }
                else
                {
                    if (!Common.IsConnection)
                    {
                        BG_MESSAGEBOX.Show(Application.Current.MainWindow, (string)this.TryFindResource("Tip"), UnConnectionWithPlc);
                    } 

                    //判断如果是停束状态点击出束就不进行任何操作 19位为0非出束 不做任何操作
                    if (BoostingControllerManager.GetInstance().IsRayOut())
                    {
                        BoostingControllerManager.GetInstance().StopRay();
                        var isSuccess = BoostingControllerManager.GetInstance().IsRayOut();
                        ////查询一下是否停束
                        if (isSuccess)
                        {
                            BG_MESSAGEBOX.Show(Application.Current.MainWindow, (string)this.TryFindResource("Tip"), $"停束失败，请重新点击");
                        }
                        else
                        {
                            BG_MESSAGEBOX.Show(Application.Current.MainWindow, (string)this.TryFindResource("Tip"), $"命令已发出");
                        }
                    }
                    else
                    {
                        BG_MESSAGEBOX.Show(Application.Current.MainWindow, (string)this.TryFindResource("Tip"), (string)this.FindResource("SettingSuccess"));
                    }
                }
            }
            catch (Exception ex)
            {
                BG_MESSAGEBOX.Show(Application.Current.MainWindow, (string)this.TryFindResource("Tip"), "软件异常，异常原因：" + ex.StackTrace);
                throw ex;
            }
        }

    }
}
