using CMW.Common.Utilities;
using BGModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonFunc;
using static CMW.Common.Utilities.CommonDeleget;
using BG_Services;
using BG_WorkFlow;
using BG_Entities;

namespace BGUserControl
{
    /// <summary>
    /// 自定义向前向后的控件
    /// </summary>
    public class DiyPreviewOrBack : BaseWindows
    {
        Visibsliy visibsliy = new Visibsliy();
        ObservableCollection<CarCantileverModel> CarCantileverModellList = new ObservableCollection<CarCantileverModel>();
        CarCantileverModelBLL cb = new CarCantileverModelBLL();
        bool? isVisible = false;
        Border PreButton;
        Border BackButton;
        public DiyPreviewOrBack()
        {
            Loaded += DiyPreviewOrBack_Loaded;
            IsVisibleChanged += DiyPreviewOrBack_IsVisibleChanged;
            ButtonInvoke.SwitchLanguageEvent -= SwitchLanguage;
            ButtonInvoke.SwitchLanguageEvent += SwitchLanguage;
            ButtonInvoke.SwitchFontSizeEvent -= SwitchFontSize;
            ButtonInvoke.SwitchFontSizeEvent += SwitchFontSize;

        }

        private void DiyPreviewOrBack_Loaded(object sender, RoutedEventArgs e)
        {
            InitListData();
            Content = MakeScan();
            SearchScanPreview();
            HorizontalAlignment = HorizontalAlignment.Stretch;
        }

        
        /// <summary>
        /// 切换字体
        /// </summary>
        /// <param name="Language"></param>
        private void SwitchFontSize(string Language)
        {
            DiyPreviewOrBack_Loaded(null, null);
        }
        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="Language"></param>
        private void SwitchLanguage(string Language)
        {
            Base_SwitchLanguage(Language);
            DiyPreviewOrBack_Loaded(null,null);
        }

        private void DiyPreviewOrBack_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            isVisible = e.NewValue as bool?;
            if (isVisible != null && isVisible == true)
            {
                SearchScanPreview();
            }
        }
          
        private void SearchScanPreview()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        if (!isVisible == true) break;
                        Thread.Sleep(150);
                        //false 为向前，True 为向后
                        bool previewOrBack = PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.ScanForWard);
                        this.Dispatcher.BeginInvoke((Action)delegate ()
                        {
                            if(BackButton!=null)
                            {
                                BackButton.Background = previewOrBack ? (Brush)this.TryFindResource("ButtonSelect") : (Brush)this.TryFindResource("ButtonUnSelect");
                                PreButton.Background = !previewOrBack ? (Brush)this.TryFindResource("ButtonSelect") : (Brush)this.TryFindResource("ButtonUnSelect");
                            }
                            visibsliy.DisplayName = (!PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.ScanForWard) ? UpdateStatusNameAction("Forward") : UpdateStatusNameAction("Backward"));
                            //PreButton.IsChecked = !previewOrBack;
                            //BackButton.IsChecked = !PreButton.IsChecked;
                        });
                    }
                    catch (System.Exception ex)
                    {
                        CommonDeleget.HandTaskException(ex);
                    }
                }
            });
        }

        private void InitListData()
        {
            CarCantileverModellList.Clear();
            cb.GetCarCantileverModel(SystemDirectoryConfig.GetInstance().GetCarCantilever())
                .ForEach(q => CarCantileverModellList.Add(q));
        }

        public StackPanel MakeScan()
        {
            StackPanel ScanSp = new StackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Orientation = Orientation.Horizontal,
            };
            Label lblPreViewTemp = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85"),
                FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.Big),
                Margin = new Thickness(0,0,0,0),
            };
            lblPreViewTemp.Content = UpdateStatusNameAction("ScanTrack") + "：";
            Label lblPreView = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Center,
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85"),
                FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.Big),
                Margin = new Thickness(0, 0, 20, 0),
            };
            lblPreView.SetBinding(Label.ContentProperty, new Binding("DisplayName") { Source = visibsliy });
            CarCantileverModel _carModel = CarCantileverModellList.FirstOrDefault(q => q.CarPropName == "扫描方向");

            //todo 这个要掉去，向前向后 不能这么用了
            PreButton = MakeSettingBorder(true,_carModel, "Forward");
            BackButton = MakeSettingBorder(false, _carModel, "Backward");

            PreButton.SetBinding(RadioButton.TagProperty, new Binding(".") { Source = _carModel, Mode = BindingMode.TwoWay });
            BackButton.SetBinding(RadioButton.TagProperty, new Binding(".") { Source = _carModel, Mode = BindingMode.TwoWay });
            PreButton.MouseDown += PreButton_Click;
            BackButton.MouseDown += PreButton_Click;
            //false 为向前，True 为向后
            bool previewOrBack = GlobalRetStatus[58];
            PreButton.Background = !previewOrBack? (Brush)this.TryFindResource("ButtonSelect") : (Brush)this.TryFindResource("ButtonUnSelect");
            BackButton.Background = previewOrBack ? (Brush)this.TryFindResource("ButtonSelect") : (Brush)this.TryFindResource("ButtonUnSelect");
            ScanSp.Children.Add(lblPreViewTemp);
            ScanSp.Children.Add(lblPreView);
            ScanSp.Children.Add(PreButton);
            ScanSp.Children.Add(BackButton);
            return ScanSp;
        }

        /// 生成复位按钮
        /// </summary>
        /// <param name="_csm"></param>
        /// <param name="_dp"></param>
        /// <returns></returns>
        private Border MakeSettingBorder(bool IsForward,CarCantileverModel _ccm = null, string LabelName = "Reset")
        {
            Border BtnSendCommand = new Border()
            {
                Width = 95,
                Height = 50,
                Style = (Style) this.FindResource("diyBtnCarHand"),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            StackPanel sp = new StackPanel()
            {
                Orientation = Orientation.Horizontal,HorizontalAlignment = HorizontalAlignment.Center
            };

            Label btnInner = new Label(){Width = 10,Height = 10};
            Label lblContent = new Label()
            {
                Content = UpdateStatusNameAction(LabelName),
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = Brushes.White,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.Middle),
            };
           
            if (IsForward)
            {
                btnInner.Background = (DrawingBrush) this.TryFindResource("leftImage");
                BtnSendCommand.Name = "PreButton";
                sp.Children.Add(btnInner);
                sp.Children.Add(lblContent);
            }
            else
            {
                btnInner.Background = (DrawingBrush)this.TryFindResource("rightImage");
                BtnSendCommand.Name = "BackButton";
                sp.Children.Add(lblContent);
                sp.Children.Add(btnInner); 
            }

           
            BtnSendCommand.Child = sp;
            if (_ccm != null)
            {
                BtnSendCommand.SetBinding(Border.TagProperty,
                    new Binding(".") { Source = _ccm, Mode = BindingMode.TwoWay });
            }
            return BtnSendCommand;
        }



        private void PreButton_Click(object sender, RoutedEventArgs e)
        {
            BGLogs.Log.GetDistance().WriteInfoLogs("PreButton_Click");
            var btn = sender as Border;
            var tag = btn.Tag as CarCantileverModel;

            //判断是不是选中了
            //if (btn.IsChecked == true)
            {
                if (!Common.IsConnection)
                {
                    btn.Background = (Brush)this.TryFindResource("ButtonUnSelect");
                    BG_MESSAGEBOX.Show(Application.Current?.MainWindow, UpdateStatusNameAction("Tip"), UpdateStatusNameAction("UnConnectionWithPlc"));
                    return;
                }
                //向前
                if (btn.Name ==   "PreButton")  
                {
                    BGLogs.Log.GetDistance().WriteInfoLogs("PreButton");
                    Task.Run(()=> {
                        try
                        {
                            BGLogs.Log.GetDistance().WriteInfoLogs("Preview Before");
                            var isSuccess = SetCommand(CommandDic[Command.Preview], false);
                            BGLogs.Log.GetDistance().WriteInfoLogs("Preview After");
                            this.Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                BackButton.Background = (Brush)this.TryFindResource("ButtonUnSelect");
                                PreButton.Background = (Brush)this.TryFindResource("ButtonSelect");
                                tag.CarPropStatus = "False";
                            });
                        }
                        catch (Exception ex)
                        {
                            BGLogs.Log.GetDistance().WriteInfoLogs(ex.StackTrace);
                            BGLogs.Log.GetDistance().WriteInfoLogs(ex.Message);
                        }
                    });
                }
                else
                {
                    BGLogs.Log.GetDistance().WriteInfoLogs("!PreButton");
                    Task.Run(() => {
                        try
                        {
                            BGLogs.Log.GetDistance().WriteInfoLogs("Preview true Before");
                            SetCommand(CommandDic[Command.Preview], true);
                            BGLogs.Log.GetDistance().WriteInfoLogs("Preview true After");
                            this.Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                PreButton.Background = (Brush)this.TryFindResource("ButtonUnSelect");
                                BackButton.Background = (Brush)this.TryFindResource("ButtonSelect");
                                tag.CarPropStatus = "True";
                            });

                        }
                        catch (Exception ex)
                        {
                            BGLogs.Log.GetDistance().WriteInfoLogs(ex.StackTrace);
                            BGLogs.Log.GetDistance().WriteInfoLogs(ex.Message);
                        }
                    });
                }
            }
        }
    }
}
