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
    //[Export("ContentPage", typeof(IConditionView))]
    //[CustomExportMetadata(1, "SelfWorkingHandSettingModule", "车体行进控制模块", "ZZW", "1.0.0")]
    public class SelfWorkingHandSettingModule : BaseModules
    {
        HandBll cbs = new HandBll(ControlVersion.SelfWorking);
        ObservableCollection<CommonSettingModel> CommonSettingModelList = new ObservableCollection<CommonSettingModel>();
        Label lbl;
        Label lblCurrentStatus;
        Label lblBandIsOpen;
        bool? isVisible =false;
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
        Armlgb ElectronAralmlgb = new Armlgb(); 
        [ImportingConstructor]
        public SelfWorkingHandSettingModule() : base(ControlVersion.SelfWorking)
        {
            Unloaded += SelftWorkingHandBoostSettingModule_Unloaded;
            Loaded += SelftWorkingHandBoostSettingModule_Loaded;
            IsVisibleChanged += SelftWorkingHandBoostSettingModule_IsVisibleChanged;
            ButtonInvoke.SwitchLanguageEvent -= SwitchLanguage;
            ButtonInvoke.SwitchLanguageEvent += SwitchLanguage;
            ButtonInvoke.SwitchFontSizeEvent -= SwitchFontSize;
            ButtonInvoke.SwitchFontSizeEvent += SwitchFontSize;
        }
        public void InitData()
        {
            CommonSettingModelList.Clear(); 
            cbs.GetHandConfigDataModel(SystemDirectoryConfig.GetInstance().GetHAND_CONFIG(Common.controlVersion)).ForEach(q => CommonSettingModelList.Add(q));
        }
        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="language"></param>
        public void SwitchLanguage(string language)
        {
            SelftWorkingHandBoostSettingModule_Loaded(this, null);
        }
        public void SwitchFontSize(string language)
        {
            SelftWorkingHandBoostSettingModule_Loaded(this, null);
        }
        private void SelftWorkingHandBoostSettingModule_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            InitData();
          
            MainGrid = InitMainGrid();

            MainBorder.Child = MainGrid;
            Content = MainBorder;
            ElectronAralmlgb.AroundArm = (LinearGradientBrush)this.FindResource("GreenPoliceLight");
            ElectronAralmlgb.MoveArm = (LinearGradientBrush)this.FindResource("GreenPoliceLight");
        }
        //private GroupBox InitPolice()
        //{

        //    GroupBox gb = new GroupBox()
        //    {
        //        HorizontalAlignment = HorizontalAlignment.Stretch,
        //        HorizontalContentAlignment = HorizontalAlignment.Left,
        //        VerticalAlignment = VerticalAlignment.Stretch,
        //        Header = new Label() { Margin = new Thickness(0, 0, 0, 0), Content = UpdateStatusNameAction("SaftPolice"),
        //            FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.NormalMiddle),
        //            FontFamily = new FontFamily("微软雅黑") }
        //    };
        //    DockPanel TouchSp = new DockPanel()
        //    {
        //        HorizontalAlignment = HorizontalAlignment.Stretch,
        //        VerticalAlignment = VerticalAlignment.Stretch,
        //        LastChildFill = true,
        //        Margin = new Thickness(0, 0, 0, 0),
        //    };
        //    //Background = StrToBrush("#EFF4F7"), BorderBrush = StrToBrush("#A5B0BE"),
        //    Border ParentBd = new Border() { Height = 40, Width = 300, BorderThickness = new Thickness(1), Margin = new Thickness(10, 0, 0, 0) };
        //    TouchSp.Children.Add(ParentBd);
        //    StackPanel TouchLeftSp = new StackPanel()
        //    {
        //        HorizontalAlignment = HorizontalAlignment.Stretch,
        //        VerticalAlignment = VerticalAlignment.Stretch,
        //        Orientation = Orientation.Horizontal,
        //        Margin = new Thickness(20, 0, 0, 0),
        //    };
        //    ParentBd.Child = TouchLeftSp;
        //    Border TouchBorder = new Border()
        //    {
        //        Height = 18,
        //        Width = 18,
        //        CornerRadius = new CornerRadius(9),
        //        BorderBrush = StrToBrush("#00FF00"),
        //        BorderThickness = new Thickness(1)
        //    };
        //    TouchBorder.SetBinding(Border.BackgroundProperty, new Binding("MoveArm") { Source = ElectronAralmlgb });
        //    Label lblTouch = new Label() { Content = UpdateStatusNameAction("MoveTouchAlarm"), Style = (Style)this.FindResource("diyLabel") };
        //    lblTouch.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
        //    TouchLeftSp.Children.Add(TouchBorder);
        //    TouchLeftSp.Children.Add(lblTouch);

        //    Border MoveReset = MakeSettingBorder(null, "MoveHitchReset");
        //    MoveReset.Width = 100;
        //    MoveReset.HorizontalAlignment = HorizontalAlignment.Left;
        //    MoveReset.Margin = new Thickness(50, 0, 0, 0);
        //    MoveReset.MouseDown -= Bd_HitchResetPreviewMouseDown;
        //    MoveReset.MouseDown += Bd_HitchResetPreviewMouseDown;
        //    TouchLeftSp.Children.Add(MoveReset);

        //    gb.Content = TouchSp;
        //    return gb;
        //}
        private void Bd_HitchResetPreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string tagFlag = (sender as Border)?.Tag as string;
            Task.Run(() => {
                try
                {
                    Common.SetCommand(CommandDic[Command.AntiCollisionReset], true);
                    Thread.Sleep(1000);
                    Common.SetCommand(CommandDic[Command.AntiCollisionReset], false);
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
            Grid gd = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch,VerticalAlignment = VerticalAlignment.Stretch, Width = 880 };
            gd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            gd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            gd.RowDefinitions.Add(new RowDefinition() { Height =new GridLength(30, GridUnitType.Pixel) });
            lblBandIsOpen = new Label()
            {
                Content = UpdateStatusNameAction("ScanMode"),
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85"),HorizontalAlignment = HorizontalAlignment.Stretch,HorizontalContentAlignment= HorizontalAlignment.Left
            };
            lblBandIsOpen.Content = string.Format($@"{ UpdateStatusNameAction("BandTypeBrakeOpen")}:{ UpdateStatusNameAction(SearchBandIsOpen())}");
            Grid.SetRow(lblBandIsOpen, 0);
            gd.Children.Add(lblBandIsOpen);
            try
            {
                foreach (var CommonSettingItem in CommonSettingModelList)
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
                    MainGd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    MainGd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    DiyRadioButton drb = new DiyRadioButton();
                    drb.ItemSource = CommonSettingItem;
                    Grid.SetRow(drb,1);
                    MainGd.Children.Add(drb);
                    drb.MouseButonEnent += _BtnSendCommand_PreviewMouseUp;
                    gd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    if (CommonSettingItem.CommonSettingName.Contains("Drive"))
                    {
                        lblCurrentStatus = new Label()
                        {
                            Content = UpdateStatusNameAction("ScanMode"),
                            Style = (Style)this.FindResource("diyLabel"),
                            Foreground = StrToBrush("#1A4F85")
                        };
                        lblCurrentStatus.FontSize = UpdateFontSizeAction(CMWFontSize.NormalMiddle);
                        lblCurrentStatus.Content = UpdateStatusNameAction(SearchCurrentCarStatus());
                        Grid.SetRow(lblCurrentStatus, 0);
                        MainGd.Children.Add(lblCurrentStatus);
                    }

                    gb.Content = MainGd;
                    Grid.SetRow(gb, CommonSettingModelList.IndexOf(CommonSettingItem) + 1);
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
            lbl = new Label()
            {
                Content = UpdateStatusNameAction("ScanMode"),
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lbl.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            lbl.Content = UpdateStatusNameAction(SearchScanMode());
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
                    Task.Run(() => {
                        PLCControllerManager.GetInstance().WritePositionValue(CommandDic[Command.FastCheckMode], false);
                        BoostingControllerManager.GetInstance().SwitchEngerHAndI(false, true, true);
                    });
                    break;
                default:
                    break;
            }
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
                     Task.Run(() => {

                         PLCControllerManager.GetInstance().WritePositionValue(CommandDic[Command.FastCheckMode], true);
                         PLCControllerManager.GetInstance().WritePositionValue(CommandDic[Command.Preview], false);
                         BoostingControllerManager.GetInstance().SwitchEngerHAndI(true, true, true);
                     });
                    break;
                default:
                    break;
            }
        }
        private void SelftWorkingHandBoostSettingModule_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            isVisible = e.NewValue as bool?;
            if (isVisible != null && isVisible == true)
            {
                Task.Factory.StartNew(() => { InqureStatusThread(); });
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
                if (isVisible == false) return;

                Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                {
                    lbl.Content = UpdateStatusNameAction(SearchScanMode());
                    lblCurrentStatus.Content = UpdateStatusNameAction(SearchCurrentCarStatus());
                    lblBandIsOpen.Content = string.Format($@"{ UpdateStatusNameAction("BandTypeBrakeOpen")}:{ UpdateStatusNameAction(SearchBandIsOpen())}");
                }));
            }
        }
        private void SelftWorkingHandBoostSettingModule_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ButtonInvoke.SwitchLanguageEvent -= SwitchLanguage;
        }
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
            byte StartPosition = Convert.ToByte(Position.Split('.')[0]);
            byte EndPosition = Convert.ToByte(Position.Split('.')[1]);
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
            return Common.IsConnection;
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
}
