
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
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonFunc;
using static CMW.Common.Utilities.CommonDeleget;
using BG_Services;
using BG_WorkFlow;
using CMW.Common.Utilities;
using BG_Entities;

namespace BGUserControl
{
    [Export("ContentPage", typeof(BaseModules))]
    [CustomExportMetadata(1,Modules.CombinedMovement_HandSettingModule, "组合移动车体行进控制模块", "ZZW", "1.0.0")]
    public class CombinedMovement_HandSettingModule : BaseModules
    {
        HandBll cbs = new HandBll(ControlVersion.CombinedMovement);
        string ModuleName;
        Label lbl;
        Label lblAutoMode;
        Label lblCurrentStatus;
        /// <summary>
        /// 用来控制控件显影的对象
        /// </summary>
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
        
        ObservableCollection<CommonSettingModel> CommonSettingModelList = new ObservableCollection<CommonSettingModel>();
        [ImportingConstructor]
        public CombinedMovement_HandSettingModule() : base(ControlVersion.CombinedMovement)
        {
            ModuleName = "CarBodyControl";
            Unloaded += CombinedMovement_HandSettingModule_Unloaded;
            Loaded += CombinedMovement_HandSettingModule_Loaded;
            IsVisibleChanged += CombinedMovement_HandSettingModule_IsVisibleChanged;
            ButtonInvoke.SwitchLanguageEvent -= SwitchLanguage;
            ButtonInvoke.SwitchLanguageEvent += SwitchLanguage;

            ButtonInvoke.SwitchFontSizeEvent -= SwitchFontSize;
            ButtonInvoke.SwitchFontSizeEvent += SwitchFontSize;
        }

        public void InitData()
        {
            CommonSettingModelList.Clear(); 
            cbs.GetHandConfigDataModel(SystemDirectoryConfig.GetInstance().GetHAND_CONFIG(ControlVersion.CombinedMovement)).ForEach(q => CommonSettingModelList.Add(q));
        }
        public void SwitchFontSize(string FontSize)
        {
            CombinedMovement_HandSettingModule_Loaded(null, null);
        }
        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="language"></param>
        public void SwitchLanguage(string language)
        {
            CombinedMovement_HandSettingModule_Loaded(this, null);
        }
        private void CombinedMovement_HandSettingModule_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            InitData();
          
            MainGrid = InitMainGrid();
            MainBorder.Child = MainGrid;
            Content = MainBorder;
        }

        /// <summary>
        /// 初始化面板
        /// </summary>
        /// <returns></returns>
        private Grid InitMainGrid()
        {
            Grid gd = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch };
            gd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(20, GridUnitType.Star) });
            gd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            gd.RowDefinitions.Add(new RowDefinition() { Height =new GridLength(35, GridUnitType.Star) });
            gd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Star) });
            gd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Star) });

            try
            {
                foreach (var CommonSettingItem in CommonSettingModelList)
                {
                    GroupBox gb = new GroupBox()
                    {
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        Header = new Label() {Margin =new Thickness(0,0,0,0), Content = UpdateStatusNameAction(CommonSettingItem.CommonSettingName), FontSize = UpdateFontSizeAction(CMWFontSize.NormalMiddle), FontFamily = new FontFamily("微软雅黑") }
                    };
                    gd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(33, GridUnitType.Star) });
                    Grid MainGd = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                    MainGd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    MainGd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Star) });
                    MainGd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Star) });
      
                    DiyRadioButton drb = new DiyRadioButton(){HorizontalAlignment = HorizontalAlignment.Stretch};
                    drb.ItemSource = CommonSettingItem;
                    Grid.SetRow(drb,1);
                    MainGd.Children.Add(drb);
                    drb.MouseButonEnent += _BtnSendCommand_PreviewMouseUp;
                    gd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    //gd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    if (CommonSettingItem.CommonSettingName.Contains("Drive"))
                    {
                        lblCurrentStatus = new Label()
                        {
                            Content = UpdateStatusNameAction("ScanMode"),
                            Style = (Style)this.FindResource("diyLabel"),
                            Foreground = StrToBrush("#1A4F85"),
                            FontSize = UpdateFontSizeAction(CMWFontSize.Big),
                            HorizontalAlignment = HorizontalAlignment.Left,Margin = new Thickness(180,0,0,0)
                        };
                        lblCurrentStatus.Content = UpdateStatusNameAction(SearchCurrentCarStatus());
                        Grid.SetRow(lblCurrentStatus, 0);
                        MainGd.Children.Add(lblCurrentStatus);
                    }

                    gb.Content = MainGd;
                    Grid.SetRow(gb, CommonSettingModelList.IndexOf(CommonSettingItem));
                    gd.Children.Add(gb);
                }

                gd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Star) });
                GroupBox ScanMode = MakeScanMode();
                Grid.SetColumn(ScanMode, 0);
                Grid.SetRow(ScanMode, CommonSettingModelList.Count + 1);

                GroupBox TestMode = MakeTestMode();
                Grid.SetColumn(TestMode, 0);
                Grid.SetRow(TestMode, CommonSettingModelList.Count + 2);
                
                gd.Children.Add(ScanMode);
                gd.Children.Add(TestMode);
            }
            catch (Exception ex)
            {
            }
            return gd;
        }
        /// <summary>
        /// 扫描模式
        /// </summary>
        /// <returns></returns>
        private GroupBox MakeScanMode()
        {
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
            lbl.Content = UpdateStatusNameAction(SearchScanMode());
            Label lblRay = new Label()
            {
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            ToggleSwitchButton RayBtn = new ToggleSwitchButton()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(0, 3, 0, 3),
                Height = 36,
                Width = 60,
                Name = "ScanMode"
            };
            RayBtn.IsChecked = BoostingControllerManager.GetInstance().SearchScanMode() == "InitiativeMode";
            RayBtn.Checked += TsBtn_Checked;
            RayBtn.UnChecked += TsBtn_UnChecked;
            Grid.SetColumn(lbl, 1);
            _dp.Children.Add(lbl);
            _dp.Children.Add(RayBtn);
            Grid.SetColumn(RayBtn, 3);
            Grid.SetColumnSpan(RayBtn, 2);
            ScanModeGb.Content = _dp;
            return ScanModeGb;
        }

        /// <summary>
        /// 扫描模式
        /// </summary>
        /// <returns></returns>
        private GroupBox MakeTestMode()
        {
            GroupBox ScanModeGb = new GroupBox()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Stretch,
                Header = new Label()
                {
                    Margin = new Thickness(0, 0, 0, 0),
                    Content = UpdateStatusNameAction("TestMode"),
                    FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.NormalMiddle),
                    FontFamily = new FontFamily("微软雅黑")
                }
            };
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
            lblAutoMode = new Label()
            {
                Content = UpdateStatusNameAction("TestMode"),
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblAutoMode.Content = UpdateStatusNameAction(SearchAutoMode());
            Label lblRay = new Label()
            {
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            ToggleSwitchButton RayBtn = new ToggleSwitchButton()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(0, 3, 0, 3),
                Height = 36,
                Width = 60,
                Name = "TestMode"
            };
            RayBtn.IsChecked = PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.TestMode);
            RayBtn.Checked += TsBtn_Checked;
            RayBtn.UnChecked += TsBtn_UnChecked;
            Grid.SetColumn(lblAutoMode, 1);
            _dp.Children.Add(lblAutoMode);
            _dp.Children.Add(RayBtn);
            Grid.SetColumn(RayBtn, 3);
            Grid.SetColumnSpan(RayBtn, 2);
            ScanModeGb.Content = _dp;
            return ScanModeGb;
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
                        Common.SetCommand(CommandDic[Command.FastCheckMode], false);
                    });
                    break;
                case "TestMode":
                    Task.Run(() => {
                        Common.SetCommand(CommandDic[Command.TestMode], false);
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
            if (!Common.IsConnection)
            {
                BG_MESSAGEBOX.Show(Application.Current?.MainWindow, UpdateStatusNameAction("Tip"), UnConnectionWithPlc);
                tsb.IsShowStoryBoard = false;
                return;
            }
            switch (btnName)
            {
                case "ScanMode":
                     Task.Run(() => {
                        Common.SetCommand(CommandDic[Command.FastCheckMode], true);
                       
                     });
                    break;
                case "TestMode":
                    Task.Run(() => {
                        Common.SetCommand(CommandDic[Command.TestMode], true);
                    });
                    break;
                default:
                    break;
            }
        }

        private void CombinedMovement_HandSettingModule_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
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
                    if(lbl !=null)
                    {
                        lbl.Content = UpdateStatusNameAction(SearchScanMode());
                    }
                   if(lblAutoMode!=null)
                    {
                        lblAutoMode.Content = UpdateStatusNameAction(SearchAutoMode());
                    }
                    lblCurrentStatus.Content = UpdateStatusNameAction(SearchCurrentCarStatus());
                }));
            }
        }
        private void CombinedMovement_HandSettingModule_Unloaded(object sender, System.Windows.RoutedEventArgs e)
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
            //var Status = Common.GlobalRetStatus[StatusIndex];
            //如果已经加了高压/低压 就取消
            Task.Run(() =>
            {
                try
                {
                    PLCControllerManager.GetInstance().WritePositionValue(Position,true);
                    //Common.GlobalPLCProtocol.Execute(StartPosition, EndPosition,true);
                    Thread.Sleep(1000);
                    PLCControllerManager.GetInstance().WritePositionValue(Position, false);
                    //Common.GlobalPLCProtocol.Execute(StartPosition, EndPosition, false);
                }
                catch (System.Exception ex)
                {
                    CommonDeleget.HandTaskException(ex);
                }
            });
            //Thread.Sleep(50);
             StrToBrush("#FFFFFF");
        }

        /// <summary>
        /// 获取名称
        /// </summary>
        /// <returns></returns>
        public override string GetName()
        {
            return ModuleName;
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


        /// <summary>
        /// 查询测试模式
        /// </summary>
        /// <returns></returns>
        public string SearchAutoMode()
        {
            return PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.TestMode) ? "TestMode" : "AutoMode";
        }
    }
}
