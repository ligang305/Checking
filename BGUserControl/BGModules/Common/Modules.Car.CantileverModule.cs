using BG_Services;
using CMW.Common.Utilities;
using BGModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.CommonFunc;
using BG_Entities;
using BG_WorkFlow;

namespace BGUserControl
{
    [Export("ContentPage", typeof(BaseModules))]
    [CustomExportMetadata(1, Modules.CarCantileverModule, "车载车体与悬臂控制与检测", "ZZW", "1.0.0")]
    public class CarCantileverModule : BaseModules
    {
        System.Timers.Timer _timer = new System.Timers.Timer();
        Grid _MainGrid = new Grid();

        Border _MainBorder = new Border()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Margin = new Thickness(0),
            BorderThickness = new Thickness(0),
            BorderBrush = StrToBrush("#3F96E6"),
            CornerRadius = new CornerRadius(0, 0, 6, 6),
            Background = StrToBrush("#F2F2F2"),
        };

        /// <summary>
        /// 这是放车辆
        /// </summary>
        Image _IMG = null;

        ObservableCollection<CarCantileverModel> CarCantileverModellList =
            new ObservableCollection<CarCantileverModel>();

        ObservableCollection<CommonSettingModel>
            CommonSettingModelList = new ObservableCollection<CommonSettingModel>();

        CarCantileverModelBLL cb = null; //
        ConfigDataBLL cbb = new ConfigDataBLL();
        Car_BoostSettingBLL cbs = new Car_BoostSettingBLL(ControlVersion.Car);
        Visibsliy visibsliy = new Visibsliy();
        Visibsliy _visibleObject = new Visibsliy();
        Armlgb ElectronAralmlgb = new Armlgb();

        [ImportingConstructor]
        public CarCantileverModule() : base(ControlVersion.Car)
        {
            Loaded += CarCantileverModule_Loaded;
            Unloaded += CarCantileverModule_Unloaded;
        }

        private void CarCantileverModule_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                BuryingPoint($"{UpdateStatusNameAction("CarBodyControlLeave")}");
                _timer.Stop();
                this.Close();
            }
            catch (Exception ex)
            {
            }
        }

        private void CarCantileverModule_Loaded(object sender, RoutedEventArgs e)
        {
            cb = new CarCantileverModelBLL(cv);
            BuryingPoint($"{UpdateStatusNameAction("CarBodyControl")}");
            ElectronAralmlgb.AroundArm = (LinearGradientBrush) this.FindResource("GreenPoliceLight");
            ElectronAralmlgb.MoveArm = (LinearGradientBrush) this.FindResource("GreenPoliceLight");

            InitListData();

            InitContent();
            //SwitchLanguage();
            InitThreadAndAction();
        }

        private void InitContent()
        {
            _MainGrid = InitGrid();
            _MainBorder.Child = _MainGrid;
            Content = _MainBorder;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Stretch;
        }

        private void InitThreadAndAction()
        {
            ButtonInvoke.SwitchLanguageEvent -= SwitchLanguage;
            ButtonInvoke.SwitchLanguageEvent += SwitchLanguage;
            ButtonInvoke.SwitchFontSizeEvent -= SwitchFontSize;
            ButtonInvoke.SwitchFontSizeEvent += SwitchFontSize;

            _timer.Elapsed -= _timer_Elapsed;
            _timer.Elapsed += _timer_Elapsed;
            _timer.Interval = 150;
            _timer.Start();

            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        if (!IsVisible)
                        {
                            break;
                        }

                        Thread.Sleep(150);
                        visibsliy.DisplayName = !PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.ScanForWard) ? "向前" : "向后";
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
            cb.GetCarCantileverModel(SystemDirectoryConfig.GetInstance().GetCarCantilever(cv))
                .ForEach(q => CarCantileverModellList.Add(q));

            CommonSettingModelList.Clear();
            cbb.GetConfigDataModel(SystemDirectoryConfig.GetInstance().GetCarBodyDirection(cv))
                .ForEach(q => CommonSettingModelList.Add(q));
        }

        public void SwitchLanguage(string language)
        {
            base.Base_SwitchLanguage(language);
            CarCantileverModule_Loaded(null, null);
        }
        public void SwitchFontSize(string FontSize)
        {
            CarCantileverModule_Loaded(null, null);
        }

        /// <summary>
        /// 通过一个每隔2秒的定时器去获取PLC各个组件的状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            InqureStatus();
            SearchHandStatus();
        }

        private void SearchHandStatus()
        {
            _visibleObject.IsShow = SearchScanMode() == "InitiativeMode" ? Visibility.Collapsed : Visibility.Visible;
            _visibleObject.DisplayName = UpdateStatusNameAction(SearchScanMode());

            Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
            {
                //先判断竖臂是否展开
                if (IsVercatilHandOpen())
                {
                    //if(visibsliy.ImageBitmapSource.UriSource.)
                    visibsliy.ImageBitmapSource = bitImgDic[HandStatus.Vercatical];
                }
                //在判断横臂是否展开
                else if (IsHornalictalHandOpen())
                {
                    visibsliy.ImageBitmapSource = bitImgDic[HandStatus.HorniacalOpen];
                }
                //在判断斜臂是否展开
                else if (IsItalyHandOpen())
                {
                    visibsliy.ImageBitmapSource = bitImgDic[HandStatus.LinarcOpen];
                }
                //啥都没展开就是各个臂都关闭得状态
                else
                {
                    visibsliy.ImageBitmapSource = bitImgDic[HandStatus.Close];
                }
            }));
        }

        private void InqureStatus()
        {
            if (!IsSearchStatusSuccess)
            {
                foreach (var HitchItem in CarCantileverModellList)
                {
                    if (HitchItem.CarPropName == "扫描方向")
                    {
                        HitchItem.CarPropStatus = "False";
                    }
                    else
                    {
                        HitchItem.CarPropStatus = "0";
                    }
                }
            }
            else
            {
                if (CheckElectronAlarm())
                {
                    this.Dispatcher.BeginInvoke((Action) delegate()
                    {
                        ElectronAralmlgb.AroundArm = (LinearGradientBrush) this.FindResource("RedPoliceLight");
                    });
                    //tingche 
                    //StopSystem();
                }
                else
                {
                    this.Dispatcher.BeginInvoke((Action) delegate()
                    {
                        ElectronAralmlgb.AroundArm = (LinearGradientBrush) this.FindResource("GreenPoliceLight");
                    });
                }


                if (MoveAlarm())
                {
                    this.Dispatcher.BeginInvoke((Action) delegate()
                    {
                        ElectronAralmlgb.MoveArm = (LinearGradientBrush) this.FindResource("RedPoliceLight");
                    });
                    //如果检测到了电子围栏出现故障就让系统停止工作
                    //StopSystem();
                }
                else
                {
                    this.Dispatcher.BeginInvoke((Action) delegate()
                    {
                        ElectronAralmlgb.MoveArm = (LinearGradientBrush) this.FindResource("GreenPoliceLight");
                    });
                }

                this.Dispatcher.BeginInvoke((Action) delegate()
                {
                    //_gd.ColumnDefinitions.Clear();
                    _gd = MakeStatusGrid();
                    ImgTitleBd.Child = _gd;
                });



                foreach (var HitchItem in CarCantileverModellList)
                {
                    int ItemIndex = Convert.ToInt32(HitchItem.CarPropSatusIndex);
                    if (ItemIndex < GlobalRetStatus.Count)
                    {
                        if (HitchItem.CarPropName == "扫描方向")
                        {
                            HitchItem.CarPropStatus = GlobalRetStatus[ItemIndex] ? "True" : "False";
                        }
                        else
                        {
                            HitchItem.CarPropStatus = GlobalRetStatus[ItemIndex] ? "1" : "0";
                        }
                    }
                }
            }
        }

        private Grid MakeChildPanel(CommonSettingModel _csm)
        {
            Grid _dp = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            _dp.DataContext = _csm;
            _dp.RowDefinitions.Add(new RowDefinition() {Height = new GridLength(30, GridUnitType.Star)});
            _dp.ColumnDefinitions.Add(new ColumnDefinition() {Width = new GridLength(25, GridUnitType.Pixel)});
            _dp.ColumnDefinitions.Add(new ColumnDefinition() {Width = new GridLength(150, GridUnitType.Pixel)});
            _dp.ColumnDefinitions.Add(new ColumnDefinition() {Width = new GridLength(1, GridUnitType.Auto)});
            _dp.ColumnDefinitions.Add(new ColumnDefinition() {Width = new GridLength(60, GridUnitType.Pixel)});
            _dp.ColumnDefinitions.Add(new ColumnDefinition() {Width = new GridLength(65, GridUnitType.Pixel)});

            Border ClycleBd = new Border()
            {
                Width = 10,
                Height = 10,
                BorderBrush = StrToBrush("#0000FF"),
                BorderThickness = new Thickness(1),
                Background = (LinearGradientBrush) this.FindResource("BluePoliceLight"),
                CornerRadius = new CornerRadius(5),
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 10, 0)
            };
            _dp.Children.Add(ClycleBd);
            Grid.SetColumn(ClycleBd, 0);

            Label lblLeft = new Label()
            {
                //Content = _csm.CommonSettingName,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Style = (Style) this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            _csm.CommonSettingDisplayName = UpdateStatusNameAction(_csm.CommonSettingName);
            lblLeft.SetBinding(Label.ContentProperty,
                new Binding("CommonSettingDisplayName") {Source = _csm, Mode = BindingMode.TwoWay});
            Label btnInner = MakeSettingBtn(_csm, _dp);
            //如果不是下拉类型的就生成TxtBox
            if (_csm.CommonSettingType == "TextBox")
            {
                TextBox txtBox = new TextBox()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 3, 3, 3),
                    Height = 30,
                    Width = 98
                };
                txtBox.SetBinding(TextBox.TextProperty,
                    new Binding("CommonSettingValue") {Source = _csm, Mode = BindingMode.TwoWay});
                _dp.Children.Add(txtBox);
                Grid.SetColumn(txtBox, 2);
                btnInner.SetBinding(Label.TagProperty, new Binding(".") {Source = txtBox});
            }
            else
            {
                ComboBox cbBox = new ComboBox()
                {
                    Style = (Style) this.FindResource("stlComboBox"),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Margin = new Thickness(0, 3, 3, 3),
                    Height = 35,
                    Width = 98
                };
                cbBox.SelectedValuePath = "SelectValue";
                cbBox.DisplayMemberPath = "SelectText";
                cbBox.SetBinding(ComboBox.ItemsSourceProperty, new Binding("selectObject") {Source = _csm});
                cbBox.SetBinding(ComboBox.SelectedValueProperty,
                    new Binding("CommonSettingValue") {Source = _csm, Mode = BindingMode.TwoWay});
                _dp.Children.Add(cbBox);
                Grid.SetColumn(cbBox, 2);
                btnInner.SetBinding(Label.TagProperty, new Binding(".") {Source = cbBox});
            }


            _dp.Children.Add(lblLeft);
            Grid.SetColumn(lblLeft, 1);
            return _dp;
        }

        /// <summary>
        /// 生成设置按钮
        /// </summary>
        /// <param name="_csm"></param>
        /// <param name="_dp"></param>
        /// <returns></returns>
        private Label MakeSettingBtn(CommonSettingModel _csm, Grid _dp)
        {
            Border BtnSendCommand = new Border()
            {
                Width = 50,
                Height = 30,
                Style = (Style) this.FindResource("diyBtnCarHand"),
            };
            BtnSendCommand.MouseLeftButtonDown += BtnSendCommand_PreviewMouseUp;
            Label btnInner = new Label()
            {
                Content = UpdateStatusNameAction("Setting"),//"设置",
                Style = (Style) this.FindResource("diyLabel"),
                Foreground = StrToBrush("#FFFFFF")
            };
            BtnSendCommand.Child = btnInner;
            _dp.Children.Add(BtnSendCommand);
            Grid.SetColumn(BtnSendCommand, 3);
            Grid.SetRow(BtnSendCommand, 0);
            BtnSendCommand.SetBinding(Border.TagProperty, new Binding(".") {Source = _csm, Mode = BindingMode.TwoWay});
            return btnInner;
        }

        private void BtnSendCommand_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (!IsConnectionEquipment())
                {
                    BG_MESSAGEBOX.Show(Application.Current?.MainWindow, (string) this.FindResource("Tip"),
                        UnConnectionWithPlc);
                    //MessageBox.Show(UnConnectionWithPlc);
                    return;
                }

                bool isSendSuccess = false;
                isSendSuccess = SendCommand(sender);
                if (isSendSuccess)
                {
                    cbs.SaveConfigDataModel(CommonSettingModelList);
                    cbb.SaveConfigDataModel(CommonSettingModelList,
                        SystemDirectoryConfig.GetInstance().GetCarBodyDirection(cv));
                }
            }
            catch (Exception ex)
            {
                BG_MESSAGEBOX.Show(Application.Current?.MainWindow, (string) this.FindResource("Tip"), ex.Message);
            }
        }

        Grid _gd = new Grid();
        Border ImgTitleBd = new Border();

        private Grid InitGrid()
        {
            Grid _MainGrid = InitMainGrid();

            #region 前进按钮

            //InitPreviewButton(_MainGrid);

            #endregion

            #region 向前后退

            InitPreviewOrBack(_MainGrid);

            #endregion

            #region 选臂展角度

            InitAngle(_MainGrid);
            if (cv == ControlVersion.SelfWorking)
            {
                InitOneKeyReset(_MainGrid);
            }
            #endregion

            #region 报警

            InitPolice(_MainGrid);

            #endregion

            #region 臂展图片

            InitHandImage(_MainGrid);

            #endregion

            return _MainGrid;
        }

        /// <summary>
        /// 初始化主Grid
        /// </summary>
        /// <returns></returns>
        private Grid InitMainGrid()
        {
            Grid _MainGrid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            _MainGrid.RowDefinitions.Add(new RowDefinition() {Height = new GridLength(1, GridUnitType.Auto) });
            for (int i = 0; i < CommonSettingModelList.Count; i++)
            {
                _MainGrid.RowDefinitions.Add(new RowDefinition() {Height = new GridLength(1, GridUnitType.Star)});
            }

            if (cv == ControlVersion.SelfWorking)
            {
                _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Auto) });
            }
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() {Height = new GridLength(5, GridUnitType.Pixel) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() {Width = new GridLength(29, GridUnitType.Star)});
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() {Width = new GridLength(20, GridUnitType.Star)});
            return _MainGrid;
        }


        private void InitHandImage(Grid _MainGrid)
        {
            Border TotalRightDB = new Border()
            {
                Height = 303,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(-70, 0, 10, 0),
                BorderBrush = StrToBrush("#A5B0BE"),
                BorderThickness = new Thickness(1),
                //Background = StrToBrush("#FFFFFF")
            };
            StackPanel ImgPan = new StackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Orientation = Orientation.Vertical, //Background = StrToBrush("#FF0000")
            };
            TotalRightDB.Child = ImgPan;

            ImgTitleBd = new Border()
            {
                Background = (LinearGradientBrush) this.FindResource("CarTitleBackGround"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                //Height = 30
            };
            _gd = MakeStatusGrid();
            ImgTitleBd.Child = _gd;
            ImgPan.Children.Add(ImgTitleBd);

            _IMG = new Image()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(10)
            };
            _IMG.SetBinding(Image.SourceProperty,
                new Binding("ImageBitmapSource") {Source = visibsliy, Mode = BindingMode.TwoWay});
            ImgPan.Children.Add(_IMG);

            Grid.SetColumn(TotalRightDB, 1);
            Grid.SetRow(TotalRightDB, 0);
            Grid.SetRowSpan(TotalRightDB, 5);
            _MainGrid.Children.Add(TotalRightDB);
        }

        /// <summary>
        /// 初始化配置文件
        /// </summary>
        /// <param name="_MainGrid"></param>
        private void InitAngle(Grid _MainGrid)
        {

            foreach (var CommonSettingModelItem in CommonSettingModelList)
            {
                GroupBox gb = new GroupBox()
                {
                    Header = new Label()
                        {Content = UpdateStatusNameAction(CommonSettingModelItem.CommonSettingName)},
                    Margin = new Thickness(0, 0, 80, 0),
                };
                Border _ParentBd = new Border()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Margin = new Thickness(0, 0, 0, 0),
                    Width = 400
                };

                CommonSettingModel Angle = CommonSettingModelItem;
                Grid _lblTouch = MakeChildPanel(Angle);
               
                _ParentBd.Child = _lblTouch;
                // 由于这个模块 公用了，所以有点冗余，所以这里单独判断，如果
                //当前版本是车载的话，那就在角度旁边添加复位按钮
                if (cv == ControlVersion.Car)
                {
                    Border bd = MakeSettingBorder(Angle);
                    bd.MouseDown -= Bd_PreviewMouseDown;
                    bd.MouseDown += Bd_PreviewMouseDown;
                    Grid.SetColumn(bd, 4);
                    Grid.SetRow(bd, 0);
                    _lblTouch.Children.Add(bd);
                }
                gb.Content = _ParentBd;
                Grid.SetColumn(gb, 0);
                Grid.SetRow(gb, CommonSettingModelList.IndexOf(CommonSettingModelItem)+1);
                _MainGrid.Children.Add(gb);
            }
        }
        /// <summary>
        /// 添加 一键复位和一键偏转，仅限制于自行走
        /// </summary>
        /// <param name="_MainGrid"></param>
        private void InitOneKeyReset(Grid _MainGrid)
        {
            Grid BtnReset = new Grid(){HorizontalAlignment= HorizontalAlignment.Stretch,VerticalAlignment = VerticalAlignment.Stretch,Height = 30};
            BtnReset.ColumnDefinitions.Add(new ColumnDefinition(){Width = new GridLength(1,GridUnitType.Star)});
            BtnReset.ColumnDefinitions.Add(new ColumnDefinition() {Width = new GridLength(1, GridUnitType.Star)});

            Border OneKeyReset = MakeSettingBorder(null, "AngleOneKeyReset");
            OneKeyReset.Tag = "KeyReset";
            OneKeyReset.Width = 100;
            OneKeyReset.HorizontalAlignment = HorizontalAlignment.Center;
            OneKeyReset.MouseDown -= Bd_PreviewMouseDown;
            OneKeyReset.MouseDown += Bd_PreviewMouseDown;
            Grid.SetColumn(OneKeyReset, 0);
            BtnReset.Children.Add(OneKeyReset);

            Border OneKeyXz = MakeSettingBorder(null, "AngleOneKeyXz");
            OneKeyXz.Tag = "KeyXz";
            OneKeyXz.Width = 100;
            OneKeyXz.HorizontalAlignment = HorizontalAlignment.Center;
            OneKeyXz.MouseDown -= Bd_PreviewMouseDown;
            OneKeyXz.MouseDown += Bd_PreviewMouseDown;
            Grid.SetColumn(OneKeyXz, 1);
            
            BtnReset.Children.Add(OneKeyXz);
            Grid.SetRow(BtnReset, 3);
            _MainGrid.Children.Add(BtnReset);
        }


        private void Bd_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string tagFlag = (sender as Border)?.Tag as string;
            Task.Run(() => {
                try
                {
                    if (cv == ControlVersion.Car)
                    {
                        BuryingPoint($"{UpdateStatusNameAction("SetAngleReset")}");
                        Common.SetCommand(CommandDic[Command.EightyAngle], false);
                        Common.SetCommand(CommandDic[Command.NinetyAngle], false);
                        Common.SetCommand(CommandDic[Command.NinetyFiveAngle], false);
                        Common.SetCommand(CommandDic[Command.ZeroAngle], false);
                    }
                    else if (cv  == ControlVersion.SelfWorking)
                    {
                        switch (tagFlag)
                        {
                            case "KeyReset":
                                Common.SetCommand(CommandDic[Command.SmallAngleOneKeyReset], true);
                                return;
                            case "KeyXz":
                                Common.SetCommand(CommandDic[Command.SmallAngleOneKeyDeflection], true);
                                return;
                            default:
                                return;
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    CommonDeleget.HandTaskException(ex);
                }
            });

        }

        private void InitPolice(Grid _MainGrid)
        {

            GroupBox gb = new GroupBox()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Header = new Label() { Content = UpdateStatusNameAction("SaftPolice") }
            };
            DockPanel TouchSp = new DockPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                LastChildFill = true,
                Margin = new Thickness(0, 0, 0, 0),
            };
            Border ParentBd = new Border() { Height = 40, Width = 144, BorderThickness = new Thickness(1), Margin = new Thickness(10, 0, 0, 0) };
            TouchSp.Children.Add(ParentBd);
            StackPanel TouchLeftSp = new StackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(20, 0, 0, 0),
            };
            ParentBd.Child = TouchLeftSp;
            Border TouchBorder = new Border()
            {
                Height = 18,
                Width = 18,
                CornerRadius = new CornerRadius(9),
                BorderBrush = StrToBrush("#00FF00"),
                BorderThickness = new Thickness(1)
            };
            TouchBorder.SetBinding(Border.BackgroundProperty, new Binding("MoveArm") { Source = ElectronAralmlgb });
            Label lblTouch = new Label() { Content = UpdateStatusNameAction("MoveTouchAlarm"), Style = (Style)this.FindResource("diyLabel") };
            TouchLeftSp.Children.Add(TouchBorder);
            TouchLeftSp.Children.Add(lblTouch);
            //自行走和快检模式没有电子围栏
            if (cv != ControlVersion.SelfWorking)
            {
                Border ParentRightBd = new Border() { Height = 40, Width = 144, BorderThickness = new Thickness(1), Margin = new Thickness(0, 0, 10, 0) };
                TouchSp.Children.Add(ParentRightBd);
                StackPanel TouchRightSp = new StackPanel()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(20, 0, 0, 0),
                };
                ParentRightBd.Child = TouchRightSp;
                Border FenceBorder = new Border() { Height = 18, Width = 18, CornerRadius = new CornerRadius(9), BorderBrush = StrToBrush("#00FF00"), BorderThickness = new Thickness(1) };
                FenceBorder.SetBinding(Border.BackgroundProperty, new Binding("AroundArm") { Source = ElectronAralmlgb });// Background = ElectronAralmlgb
                Label lblFence = new Label() { Content = UpdateStatusNameAction("EleAroundAlarm"), Style = (Style)this.FindResource("diyLabel") };
                TouchRightSp.Children.Add(FenceBorder);
                TouchRightSp.Children.Add(lblFence);
            }
          
            gb.Content = TouchSp;
            Grid.SetColumn(gb, 0);
            Grid.SetRow(gb, 2 + CommonSettingModelList.Count-1);
            _MainGrid.Children.Add(gb);
        }

        private void InitPreviewOrBack(Grid _MainGrid)
        {
            GroupBox gb = new GroupBox() { Header = new Label() { Content = UpdateStatusNameAction("ScanDirection") }, Margin = new Thickness(0, 0, 80, 0), };
            Border MiddleLeftBD = new Border()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Stretch,
                Height = 80,
                Width = 355,
                Margin = new Thickness(0, 0, 0, 0),
            };
            StackPanel spTop = new StackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                Height = 80,
                Width = 400,
            };
            MiddleLeftBD.Child = spTop;
            gb.SetBinding(GroupBox.VisibilityProperty, new Binding("IsShow") { Source = _visibleObject, Mode = BindingMode.TwoWay });
            DiyPreviewOrBack dpb = new DiyPreviewOrBack()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Stretch,
                Height = 80,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Stretch,
                FontSize = UpdateFontSizeAction(CMWFontSize.Normal)
            };
            spTop.Children.Add(dpb);
            gb.Content = MiddleLeftBD;
            Grid.SetColumn(gb, 0);
            Grid.SetRow(gb, 0);
            _MainGrid.Children.Add(gb);
        }

        private void Dp_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.CurrentWindow?.DragMove();
        }

        private void PreButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as RadioButton;
            var tag = btn.Tag as CarCantileverModel;
            var name = btn.Name;

            //判断是不是选中了
            //if (btn.IsChecked == true)
            {
                if (!Common.IsConnection)
                {
                    btn.IsChecked = false;
                    BG_MESSAGEBOX.Show(Application.Current?.MainWindow, UpdateStatusNameAction("Tip"), UpdateStatusNameAction("UnConnectionWithPlc"));
                    return;
                }
                //tag.IsChecked = !tag.IsChecked;
                //向前
                if (btn.Name == "PreButton")
                {
                    var isSuccess = SetCommand(CommandDic[Command.Preview], false);
                    tag.CarPropStatus = "False";
                }
                else
                {
                    SetCommand(CommandDic[Command.Preview], true);
                    tag.CarPropStatus = "True";
                }
            }
        }

        private void Bd_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Close();
        }

        public Border GetBtn(string btnText, int flag)
        {
            var BtnStyle = (Style)this.FindResource("diyBtnCarHand");
            Border ParentBorder = new Border()
            {
                Style = BtnStyle,
                Margin = new Thickness(25, 0, 0, 0),
                Tag = flag
            };
            ParentBorder.PreviewMouseUp += ParentBorder_PreviewMouseUp;
            StackPanel SP = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            Canvas icon = new Canvas()
            {
                Width = 15,
                Height = 15,
            };
            if (flag == 1)
            {
                icon.Background = (DrawingBrush)this.FindResource("leftImage");
            }
            else if (flag == 2)
            {
                icon.Background = (DrawingBrush)this.FindResource("StopImage");
            }
            else
            {
                icon.Background = (DrawingBrush)this.FindResource("rightImage");
            }
            Label btnInner = new Label()
            {
                Content = btnText,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Foreground = StrToBrush("#E2ECFF"),
                FontFamily = new FontFamily("宋体"),
                FontSize = UpdateFontSizeAction(CMWFontSize.NormalMiddle),
                FontWeight = FontWeights.Medium
            };
            SP.Children.Add(icon);
            SP.Children.Add(btnInner);
            ParentBorder.Child = SP;
            return ParentBorder;
        }

        private void ParentBorder_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //判断系统是否连接到了PLC
            if (!Common.IsConnection)
            {
                BG_MESSAGEBOX.Show(Application.Current?.MainWindow, UpdateStatusNameAction("Tip"), UpdateStatusNameAction("UnConnectionWithPlc"));
                //MessageBox.Show("设置成功！");
                return;
            }

            int Flag = Convert.ToInt32((sender as Border).Tag);
            switch (Flag)
            {
                //前进 M22.3
                case 1:
                    BuryingPoint($"{UpdateStatusNameAction("SetForwardCommand")}");
                    SetCommand(CommandDic[Command.CarGo], true);
                    break;
                case 2:
                    //终止 M22.4
                    BuryingPoint($"{UpdateStatusNameAction("SetStopCommand")}");
                    SetCommand(CommandDic[Command.CarStop], true);
                    break;
                //后退 M22.5
                case 3:
                    BuryingPoint($"{UpdateStatusNameAction("SetBackwardCommand")}");
                    SetCommand(CommandDic[Command.CarBack], true);
                    break;
                default:
                    break;
            }
        }
        ObservableCollection<CarCantileverModel> CarHandList = new ObservableCollection<CarCantileverModel>();
        private Grid MakeStatusGrid()
        {
            CarHandList.Clear();
            CarCantileverModellList.Where(q => q.CarPropName.Contains("臂")).ToList().ForEach(q => CarHandList.Add(q));
            _gd = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            _gd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            bool StatusCount = true;
            foreach (var CarCantileverModelItem in CarHandList)
            {
                if (GlobalRetStatus[Convert.ToInt32(CarCantileverModelItem.CarPropSatusIndex)])
                {
                    Label lblStatus = new Label()
                    {
                        Content = CarCantileverModelItem.CarDisPlayName,
                        Style = (Style)this.FindResource("diyLabel"),
                        FontSize = UpdateFontSizeAction(CMWFontSize.Normal)
                    };
                    _gd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    Grid.SetColumn(lblStatus, _gd.ColumnDefinitions.Count - 1);
                    Grid.SetRow(lblStatus, 0);
                    _gd.Children.Add(lblStatus);
                    StatusCount = false;
                }
            }
            if (StatusCount)
            {
                Label lblStatus = new Label()
                {
                    Content = UpdateStatusNameAction("HandAllClose"),
                    Style = (Style)this.FindResource("diyLabel"),
                    FontSize = UpdateFontSizeAction(CMWFontSize.Normal)
                };
                _gd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                Grid.SetColumn(lblStatus, _gd.ColumnDefinitions.Count - 1);
                Grid.SetRow(lblStatus, 0);
                _gd.Children.Add(lblStatus);
            }
            return _gd;
        }


        public override string GetName()
        {
            return "CarBodyControlAndChecking";
        }

        public override bool IsConnectionEquipment()
        {
            return Common.IsConnection;
        }

        public override void Show(Window _OwnerWin)
        {
            CurrentWindow = _OwnerWin;
            _OwnerWin.MaxWidth = GetWidth();
            _OwnerWin.MaxHeight = GetHeight();
            _OwnerWin.Title = GetName();
            _OwnerWin.Content = this;
            _OwnerWin.Show();
        }
        public override double GetHeight()
        {
            return 410;
        }

        public override double GetWidth()
        {
            return 760;
        }
    }

}
