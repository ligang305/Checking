using CMW.Common.Utilities;
using BGModel;

using BGUserControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shell;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.CommonFunc;
using static CMW.Common.Utilities.Common;
using System.Windows.Data;
using BGCommunication;
using BG_Services;
using BG_WorkFlow;
using BG_Entities;

namespace BGUserControl
{
    [Export("ContentPage", typeof(BaseModules))]
    [CustomExportMetadata(1, Modules.HandSettingModule, "车体行进控制模块_通用", "ZZW", "1.0.0")]
    public class HandSettingModule : BaseModules
    {
        HandSettingMvvm handSettingMvvm = new HandSettingMvvm();
        Armlgb ElectronAralmlgb = new Armlgb();
        /// <summary>
        /// 用来控制控件显影的对象
        /// </summary>
        bool? isVisible = false;
        /// <summary>
        /// 用来控制控件显影的对象
        /// </summary>
        Border MainBorder = new Border()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        Grid MainGrid = new Grid()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
        };
       
        [ImportingConstructor]
        public HandSettingModule() : base(controlVersion)
        {
            Loaded += HandSettingModule_Loaded;
            handSettingMvvm.SwitchLanguageAction -= SwitchLanguage;
            handSettingMvvm.SwitchLanguageAction += SwitchLanguage;
            handSettingMvvm.SwitchFontsizeAction -= SwitchFontSize;
            handSettingMvvm.SwitchFontsizeAction += SwitchFontSize;
            MessageBoxEvent += HandSettingModule_MessageBoxEvent;
            DataContext = handSettingMvvm;
            IsVisibleChanged += HandSettingModule_IsVisibleChanged;
        }

        private void HandSettingModule_MessageBoxEvent(string MessageBoxMsg)
        {
            MessageBox.Show(UpdateStatusNameAction(MessageBoxMsg), UpdateStatusNameAction("Warning"), MessageBoxButton.YesNo);
        }

        private void HandSettingModule_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            isVisible = e.NewValue as bool?;
            if (isVisible != null && isVisible == true)
            {
                Task.Factory.StartNew(() => {
                    while (true)
                    {
                        Thread.Sleep(150);
                        if (isVisible == false) return;
                        handSettingMvvm.newConnectionStatus(PLCControllerManager.GetInstance().IsConnect());
                    }
                });
            }
        }

        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="language"></param>
        public void SwitchLanguage()
        {
            HandSettingModule_Loaded(this, null);
        }
        public void SwitchFontSize()
        {
            HandSettingModule_Loaded(this, null);
        }
        private void HandSettingModule_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            MainGrid = controlVersion == ControlVersion.PassengerCar ? InitMainGridControlElecter() : controlVersion == ControlVersion.BGV7700 ? InitBGV7700MainGrid():InitMainGrid();
            MainBorder.Child = MainGrid;
            Content = MainBorder;
            ElectronAralmlgb.AroundArm = (LinearGradientBrush)this.FindResource("GreenPoliceLight");
            ElectronAralmlgb.MoveArm = (LinearGradientBrush)this.FindResource("GreenPoliceLight");
        }

        private void Bd_HitchResetPreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string tagFlag = (sender as Border)?.Tag as string;
            Task.Run(() => {
                try
                {
                    PLCControllerManager.GetInstance().WritePositionValue(CommandDic[Command.AntiCollisionReset], true);
                    Thread.Sleep(1000);
                    PLCControllerManager.GetInstance().WritePositionValue(CommandDic[Command.AntiCollisionReset], false);
                    return;
                }
                catch (Exception ex)
                {
                    CommonDeleget.HandTaskException(ex);
                }
            });
        }
        /// <summary>
        /// 初始化面板
        /// </summary>
        /// <returns></returns>
        private Grid InitMainGrid()
        {
            Grid gd = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch,VerticalAlignment = VerticalAlignment.Stretch};
            gd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            gd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            gd.RowDefinitions.Add(new RowDefinition() { Height =new GridLength(30, GridUnitType.Pixel) });
            Label lblBandIsOpen = new Label()
            {
                Content = UpdateStatusNameAction("ScanMode"),
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85"),HorizontalAlignment = HorizontalAlignment.Stretch,HorizontalContentAlignment= HorizontalAlignment.Center
            };
         
            try
            {
                foreach (var CommonSettingItem in handSettingMvvm.CommonSettingModelList)
                {
                    GroupBox gb = new GroupBox()
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,HorizontalContentAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Header = new Label() {Margin =new Thickness(0,0,0,0), Content = UpdateStatusNameAction(CommonSettingItem.CommonSettingName),
                            FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.NormalMiddle),
                            FontFamily = new FontFamily("微软雅黑") }
                    };
                    Grid MainGd = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                    MainGd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    MainGd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Pixel) });
                    MainGd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
                    DiyRadioButton drb = new DiyRadioButton();
                    drb.HorizontalAlignment = HorizontalAlignment.Center;
                    drb.ItemSource = CommonSettingItem;
                    Grid.SetRow(drb,1);
                    MainGd.Children.Add(drb);
                    drb.MouseButonEnent += _BtnSendCommand_PreviewMouseUp;
                    gd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
                    if (CommonSettingItem.CommonSettingName.Contains("Drive"))
                    {
                        lblBandIsOpen.FontSize = UpdateFontSizeAction(CMWFontSize.NormalMiddle);
                        lblBandIsOpen.SetBinding(Label.ContentProperty, new Binding("CurrentStatus") { Mode = BindingMode.TwoWay });
                        Grid.SetRow(lblBandIsOpen, 0);
                        MainGd.Children.Add(lblBandIsOpen);
                    }

                    gb.Content = MainGd;
                    Grid.SetRow(gb, handSettingMvvm.CommonSettingModelList.IndexOf(CommonSettingItem) + 1);
                    if(CommonSettingItem.CommonSettingName.Contains("ForwardSteering"))
                    {
                        gb.Visibility = Visibility.Hidden;
                    }
                    gd.Children.Add(gb);
                }

                GroupBox ScanModeGb = new GroupBox()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Header = new Label() { Margin = new Thickness(0, 0, 0, 0), Content = UpdateStatusNameAction("ScanMode"),
                        FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.NormalMiddle),
                        FontFamily = new FontFamily("微软雅黑") }
                };
                //扫描模式
                Grid PrehotGd = MakeScanMode();
                ScanModeGb.Content = PrehotGd;
                Grid.SetRow(ScanModeGb, 1);
                //Grid.SetRowSpan(ScanModeGb, 2);
                Grid.SetColumn(ScanModeGb, 1);
                gd.Children.Add(ScanModeGb);

                //GroupBox Police =  InitPolice();
                //Grid.SetRow(Police, 2);
                //Grid.SetColumn(Police, 1);
                //gd.Children.Add(Police);
            }
            catch (Exception ex)
            {
                WriteLogAction(ex.StackTrace,LogType.ApplicationError,false);
            }
            return gd;
        }

        /// <summary>
        /// 初始化乘用车面板
        /// </summary>
        /// <returns></returns>
        private Grid InitMainGridControlElecter()
        {
            Grid gd = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Width = 880 };
            gd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            gd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            gd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Pixel) });
            gd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            Label lblEntry = new Label()
            {
                Content = UpdateStatusNameAction("ScanMode"),
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(80,0,0,0)
            };
            Label lblExit = new Label()
            {
                Content = UpdateStatusNameAction("ScanMode"),
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 100, 0)
            };
            try
            {
                foreach (var CommonSettingItem in handSettingMvvm.CommonSettingModelList)
                {
                    GroupBox gb = new GroupBox()
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        HorizontalContentAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Header = new Label()
                        {
                            Margin = new Thickness(0, 0, 0, 0),
                            Content = UpdateStatusNameAction(CommonSettingItem.CommonSettingName),
                            FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.NormalMiddle),
                            FontFamily = new FontFamily("微软雅黑")
                        }
                    };
                    Grid MainGd = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                    MainGd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    MainGd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    MainGd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    DiyRadioButton drb = new DiyRadioButton();
                    drb.ItemSource = CommonSettingItem;
                    Grid.SetRow(drb, 1);
                    MainGd.Children.Add(drb);
                    drb.MouseButonEnent += _BtnSendCommand_PreviewMouseUp;
                    gd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    if (CommonSettingItem.CommonSettingName.Contains("Drive"))
                    {
                        lblEntry.FontSize = UpdateFontSizeAction(CMWFontSize.NormalMiddle);
                        lblEntry.SetBinding(Label.ContentProperty, new Binding("Entry") { Mode = BindingMode.TwoWay });
                        Grid.SetRow(lblEntry, 0);
                        MainGd.Children.Add(lblEntry);

                        lblExit.FontSize = UpdateFontSizeAction(CMWFontSize.NormalMiddle);
                        lblExit.SetBinding(Label.ContentProperty, new Binding("Exit") { Mode = BindingMode.TwoWay });
                        Grid.SetRow(lblExit, 0);
                        MainGd.Children.Add(lblExit);
                    }

                    gb.Content = MainGd;
                    Grid.SetRow(gb, handSettingMvvm.CommonSettingModelList.IndexOf(CommonSettingItem) + 1);
                    if (CommonSettingItem.CommonSettingName.Contains("ForwardSteering"))
                    {
                        gb.Visibility = Visibility.Hidden;
                    }
                    gd.Children.Add(gb);
                }

                GroupBox ScanModeGb = new GroupBox()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Header = new Label()
                    {
                        Margin = new Thickness(0, 0, 0, 0),
                        Content = UpdateStatusNameAction("ScanMode"),
                        FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.NormalMiddle),
                        FontFamily = new FontFamily("微软雅黑")
                    }
                };
                //扫描模式
                Grid PrehotGd = MakeScanMode();
                ScanModeGb.Content = PrehotGd;
                Grid.SetRow(ScanModeGb, 1);
                //Grid.SetRowSpan(ScanModeGb, 2);
                Grid.SetColumn(ScanModeGb, 1);
                gd.Children.Add(ScanModeGb);

                //GroupBox Police = InitPolice();
                //Grid.SetRow(Police, 2);
                //Grid.SetColumn(Police, 1);
                //gd.Children.Add(Police);
            }
            catch (Exception ex)
            {
                WriteLogAction(ex.StackTrace, LogType.ApplicationError, false);
            }
            return gd;
        }

        /// <summary>
        /// 初始化面板
        /// </summary>
        /// <returns></returns>
        private Grid InitBGV7700MainGrid()
        {
            Grid gd = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            gd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            gd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            gd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Pixel) });
            Label lblBandIsOpen = new Label()
            {
                Content = UpdateStatusNameAction("ScanMode"),
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };

            try
            {
                foreach (var CommonSettingItem in handSettingMvvm.CommonSettingModelList)
                {
                    GroupBox gb = new GroupBox()
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        HorizontalContentAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Header = new Label()
                        {
                            Margin = new Thickness(0, 0, 0, 0),
                            Content = UpdateStatusNameAction(CommonSettingItem.CommonSettingName),
                            FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.NormalMiddle),
                            FontFamily = new FontFamily("微软雅黑")
                        }
                    };
                    Grid MainGd = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                    MainGd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    MainGd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Pixel) });
                    MainGd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
                    DiyRadioButton drb = new DiyRadioButton();
                    drb.HorizontalAlignment = HorizontalAlignment.Center;
                    drb.ItemSource = CommonSettingItem;
                    Grid.SetRow(drb, 1);
                    MainGd.Children.Add(drb);
                    drb.MouseButonEnent += _BtnSendCommand_PreviewMouseUp;
                    gd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
                    if (CommonSettingItem.CommonSettingName.Contains("Drive"))
                    {
                        lblBandIsOpen.FontSize = UpdateFontSizeAction(CMWFontSize.NormalMiddle);
                        lblBandIsOpen.SetBinding(Label.ContentProperty, new Binding("CurrentStatus") { Mode = BindingMode.TwoWay });
                        Grid.SetRow(lblBandIsOpen, 0);
                        MainGd.Children.Add(lblBandIsOpen);
                    }

                    gb.Content = MainGd;
                    Grid.SetRow(gb, handSettingMvvm.CommonSettingModelList.IndexOf(CommonSettingItem) + 1);
                    if (CommonSettingItem.CommonSettingName.Contains("ForwardSteering"))
                    {
                        gb.Visibility = Visibility.Hidden;
                    }
                    gd.Children.Add(gb);
                }

                GroupBox ScanModeGb = new GroupBox()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Header = new Label()
                    {
                        Margin = new Thickness(0, 0, 0, 0),
                        Content = UpdateStatusNameAction("ScanMode"),
                        FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.NormalMiddle),
                        FontFamily = new FontFamily("微软雅黑")
                    }
                };
                //扫描模式
                Grid PrehotGd = MakeScanMode();
                ScanModeGb.Content = PrehotGd;
                Grid.SetRow(ScanModeGb, 1);
                //Grid.SetRowSpan(ScanModeGb, 2);
                Grid.SetColumn(ScanModeGb, 1);
                gd.Children.Add(ScanModeGb);


                GroupBox LighteGb = new GroupBox()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Header = new Label()
                    {
                        Margin = new Thickness(0, 0, 0, 0),
                        Content = UpdateStatusNameAction("Light"),
                        FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.NormalMiddle),
                        FontFamily = new FontFamily("微软雅黑")
                    }
                };
                //补光灯
                Grid LightGd = MakeLight();
                LighteGb.Content = LightGd;
                Grid.SetRow(LighteGb, 2);
                Grid.SetColumn(LighteGb, 1);
                gd.Children.Add(LighteGb);
                

                //GroupBox Police =  InitPolice();
                //Grid.SetRow(Police, 2);
                //Grid.SetColumn(Police, 1);
                //gd.Children.Add(Police);
            }
            catch (Exception ex)
            {
                WriteLogAction(ex.StackTrace, LogType.ApplicationError, false);
            }
            return gd;
        }
        /// <summary>
        /// 扫描模式
        /// </summary>
        /// <returns></returns>
        private Grid MakeScanMode()
        {
            Grid _dp = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch };
            _dp.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Star) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70, GridUnitType.Pixel) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30, GridUnitType.Star) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30, GridUnitType.Star) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70, GridUnitType.Pixel) });
            Border ClycleBd = new Border()
            {
                Width = 10,
                Height = 10,
                BorderBrush = StrToBrush("#0000FF"),
                BorderThickness = new Thickness(1),
                Background = (LinearGradientBrush)this.FindResource("BluePoliceLight"),
                CornerRadius = new CornerRadius(5),
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 10, 0)
            };
            _dp.Children.Add(ClycleBd);
            Label lbl = new Label()
            {
                Content = UpdateStatusNameAction("ScanMode"),
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lbl.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            Label lblRay = new Label()
            {
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblRay.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            ToggleSwitchButton RayBtn = new ToggleSwitchButton()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(0, 3, 0, 3),
                Height = 36,
                Width = 60,
                Name = "ScanMode"
            };
            RayBtn.IsChecked = !PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.ScanMode);
            RayBtn.Checked += TsBtn_Checked;
            RayBtn.UnChecked += TsBtn_UnChecked;
            Grid.SetColumn(lbl, 1);
            _dp.Children.Add(lbl);
            _dp.Children.Add(RayBtn);
            Grid.SetColumn(RayBtn, 3);
            Grid.SetColumnSpan(RayBtn, 2);
            return _dp;
        }
        /// <summary>
        /// 补光灯
        /// </summary>
        /// <returns></returns>
        private Grid MakeLight()
        {
            Grid _dp = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch };
            _dp.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Star) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70, GridUnitType.Pixel) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30, GridUnitType.Star) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30, GridUnitType.Star) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70, GridUnitType.Pixel) });
            Border ClycleBd = new Border()
            {
                Width = 10,
                Height = 10,
                BorderBrush = StrToBrush("#0000FF"),
                BorderThickness = new Thickness(1),
                Background = (LinearGradientBrush)this.FindResource("BluePoliceLight"),
                CornerRadius = new CornerRadius(5),
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 10, 0)
            };
            _dp.Children.Add(ClycleBd);
            Label lbl = new Label()
            {
                Content = UpdateStatusNameAction("Light"),
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lbl.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            Label lblRay = new Label()
            {
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblRay.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            ToggleSwitchButton LightBtn = new ToggleSwitchButton()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(0, 3, 0, 3),
                Height = 36,
                Width = 60,
                Name = "Light"
            };
            //RayBtn.IsChecked = !PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.lig);
            LightBtn.Checked += TsBtn_Checked;
            LightBtn.UnChecked += TsBtn_UnChecked;
            Grid.SetColumn(lbl,1);
            _dp.Children.Add(lbl);
            _dp.Children.Add(LightBtn);
            Grid.SetColumn(LightBtn, 3);
            Grid.SetColumnSpan(LightBtn, 2);
            return _dp;
        }
        /// <summary>
        /// ToggleButton取消选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TsBtn_UnChecked(object sender, RoutedEventArgs e)
        {
            ToggleSwitchButton tsb = (sender as ToggleSwitchButton);
            string btnName = tsb.Name as string;
            switch (btnName)
            {
                case "ScanMode":
                    if (DetecotrControllerManager.GetInstance().DetectorConnection != 3)
                    {
                        CommonDeleget.MessageBoxActionAction("由于探测器未完成初始化，请探测器连接状态灯稳定绿灯后在设置。");
                        tsb.IsShowStoryBoard = false;
                        return;
                    }
                    Task.Run(() => {
                        PLCControllerManager.GetInstance().WritePositionValue(CommandDic[Command.FastCheckMode], false);
                        BoostingControllerManager.GetInstance().SwitchEngerHAndI(false,true,true);
                    });
                    break;
                case "Light":
                    Task.Run(() => {
                        PLCControllerManager.GetInstance().WritePositionValue(CommandDic[Command.LightOpen], false);
                    });
                    break;
                default:
                    break;
            }
            tsb.IsShowStoryBoard = true;
        }
        /// <summary>
        /// ToggleButton按钮效果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TsBtn_Checked(object sender, RoutedEventArgs e)
        {
            ToggleSwitchButton tsb = (sender as ToggleSwitchButton);
            string btnName = tsb.Name as string;
            switch (btnName)
            {
                case "ScanMode":
                    if (DetecotrControllerManager.GetInstance().DetectorConnection != 3)
                    {
                        CommonDeleget.MessageBoxActionAction("由于探测器未完成初始化，请探测器连接状态灯稳定绿灯后在设置。");
                        tsb.IsShowStoryBoard = false;
                        return;
                    }
                    Task.Run(() => {
                         PLCControllerManager.GetInstance().WritePositionValue(CommandDic[Command.FastCheckMode], true);
                         PLCControllerManager.GetInstance().WritePositionValue(CommandDic[Command.Preview], false);
                         BoostingControllerManager.GetInstance().SwitchEngerHAndI(true, true, true);
                     });
                    break;
                case "Light":
                    Task.Run(() => {
                        PLCControllerManager.GetInstance().WritePositionValue(CommandDic[Command.LightOpen], true);
                    });
                    break;
                default:
                    break;
            }
            tsb.IsShowStoryBoard = true;
        }
        /// <summary>
        /// 查询状态的线程
        /// </summary>

        private void _BtnSendCommand_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Border _bd = sender as Border;
            Label _lbl = _bd.Child as Label;
            CommonSettingModel csm = _lbl.Tag as CommonSettingModel;
            BuryingPoint($"Setting{_lbl.Content.ToString()}Command");
            if (!IsConnectionEquipment())
            {
                BG_MESSAGEBOX.Show(Application.Current?.MainWindow, UpdateStatusNameAction("Tip"), UnConnectionWithPlc);
                return;
            }
            string Position = (_bd.Tag as string).Split('?')[0];
            //byte StartPosition = Convert.ToByte(Position.Split('.')[0]);
            //byte EndPosition = Convert.ToByte(Position.Split('.')[1]);
            //如果已经加了高压/低压 就取消
            Task.Run(() =>
            {
                try
                {
                    PLCControllerManager.GetInstance().WritePositionValue(Position, true);
                    Thread.Sleep(1000);
                    PLCControllerManager.GetInstance().WritePositionValue(Position, false);
                }
                catch (System.Exception ex)
                {
                    CommonDeleget.HandTaskException(ex);
                }
            });
             StrToBrush("#FFFFFF");
        }
        /// <summary>
        /// 获取名称
        /// </summary>
        /// <returns></returns>
        public override string GetName()
        {
            return "CarBodyControl";
        }
        /// <summary>
        /// 获取软件高度
        /// </summary>
        /// <returns></returns>
        public override double GetHeight()
        {
            return 400;
        }
        /// <summary>
        /// 获取软件宽度
        /// </summary>
        /// <returns></returns>
        public override double GetWidth()
        {
            return 740;
        }
        /// <summary>
        /// 判断是否进行连接
        /// </summary>
        /// <returns></returns>
        public override bool IsConnectionEquipment()
        {
            return IsConnection;
        }
        public override void Show(Window _OwnerWin)
        {
            CurrentWindow = _OwnerWin;
            WindowChrome wc = WindowChrome.GetWindowChrome(CurrentWindow);
            wc = (WindowChrome)this.FindResource("WindowChromeKey");
            _OwnerWin.Width = GetWidth();
            _OwnerWin.Height = GetHeight();
            _OwnerWin.Title = GetName();
            _OwnerWin.Content = this;
            _OwnerWin.Show();
        }
    }

    public class HandSettingMvvm:BaseMvvm
    {
        HandBll cbs = new HandBll(ControlVersion.SelfWorking);
        public ObservableCollection<CommonSettingModel> CommonSettingModelList = new ObservableCollection<CommonSettingModel>();
        public string _ScanMode;
        public string ScanMode
        {
            get => _ScanMode;
            set { _ScanMode = value;  }
        }
        public string _CurrentStatus;
        public string CurrentStatus
        {
            get => _CurrentStatus;
            set { _CurrentStatus = value; RaisePropertyChanged("CurrentStatus"); }
        }
        public string _Entry;
        public string Entry
        {
            get => _Entry;
            set { _Entry = value; RaisePropertyChanged("Entry"); }
        }
        public string _Exit;
        public string Exit
        {
            get => _Exit;
            set { _Exit = value; RaisePropertyChanged("Exit"); }
        }
        public string _BandIsOpen;
        public string BandIsOpen
        {
            get => _BandIsOpen;
            set { _BandIsOpen = value; }
        }
        public HandSettingMvvm()
        {
            InitData();
            
        }
        private void InitData()
        {
            CommonSettingModelList.Clear();
            cbs.GetHandConfigDataModel(SystemDirectoryConfig.GetInstance().GetHAND_CONFIG(controlVersion)).ForEach(q => CommonSettingModelList.Add(q));
        }

        public void newConnectionStatus(bool ConnectionStatus)
        {
            ScanMode = UpdateStatusNameAction(BoostingControllerManager.GetInstance().SearchScanMode());
            CurrentStatus = SearchCurrentCarStatus();
            BandIsOpen = string.Format($@"{ UpdateStatusNameAction("BandTypeBrakeOpen")}:
                        { UpdateStatusNameAction(PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.BandTypeBrakeOpen) ? "Open" : "Close")}");
        }

        public string SearchCurrentCarStatus()
        {
            if (PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.EquipmentForward))
            {
                Entry = UpdateStatusNameAction("Go");
              
            }
            else
            {
                Entry = UpdateStatusNameAction("Stop");
            }
            if (PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.EquipmentBackward))
            {
                Exit = UpdateStatusNameAction("Go");
            }
            else
            {
                Exit = UpdateStatusNameAction("Stop");
            }

            if (PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.EquipmentForward))
            {
                CurrentStatus = UpdateStatusNameAction("Go");
            }
            else if(PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.EquipmentBackward))
            {
                CurrentStatus = UpdateStatusNameAction("Back");
            }
            else
            {
                CurrentStatus = UpdateStatusNameAction("Stop");
            }
            return CurrentStatus;
        }
    }
}
