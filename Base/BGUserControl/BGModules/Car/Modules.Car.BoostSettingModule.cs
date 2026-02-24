using CMW.Common.Utilities;
using BGCommunication;
using BGModel;

using BGUserControl;

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
using System.Windows.Shell;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.CommonFunc;
using static CMW.Common.Utilities.Common;
using BG_Services;
using BG_Entities;
using BG_WorkFlow;

namespace BGUserControl
{
    [Export("ContentPage",typeof(BaseModules))]
    [CustomExportMetadata(1, Modules.Car_BoostSettingModule, "加速器设置与维护", "ZZW", "1.0.0")] 
    public class CarBoostSettingModule : BaseModules
    {
        Grid _MainGrid = new Grid();
        Border _MainBorder = new Border()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
        };
        ObservableCollection<CommonSettingModel> CommonSettingModelList = new ObservableCollection<CommonSettingModel>();
        ObservableCollection<StatusModel> HitchList = new ObservableCollection<StatusModel>();
        Car_BoostSettingBLL cbs = new Car_BoostSettingBLL(ControlVersion.Car);
        bool? isVisible = false;
        /// <summary>
        /// 用来控制控件显影的对象
        /// </summary>
        Visibsliy _visibleObject = new Visibsliy();

        private void CarBoostSettingModule_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            isVisible = e.NewValue as bool?;
            if (isVisible != null && isVisible == true)
            {
                Task.Factory.StartNew(() => { InqureStatusThread();  });
            }
        }
        public CarBoostSettingModule() : base(ControlVersion.Car)
        {
            CommonSettingModule_Loaded(null,null);
            Unloaded += CommonSettingModule_Unloaded;
            IsVisibleChanged += CarBoostSettingModule_IsVisibleChanged;
        }


        //WPF 机制如此，控件消失，先调用isVisibles 后调用Unloaded 不然双向绑定事件无法触发
        private void CommonSettingModule_Unloaded(object sender, RoutedEventArgs e)
        {
            BuryingPoint($"{UpdateStatusNameAction("AcceleratorModuleLeave")}");
        }

        /// <summary>
        /// 获取名称
        /// </summary>
        /// <returns></returns>
        public override string GetName()
        {
            return BoosttingSetting;
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

        protected void CommonSettingModule_Loaded(object sender, RoutedEventArgs e)
        {
            BuryingPoint("AcceleratorModuleEnter");
            base.Base_SwitchLanguage(ConfigServices.GetInstance().localConfigModel.LANGUAGE);
            InitListData();
            InitContent();
            InitActionAndThread();
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

        private void InitListData()
        {
            CommonSettingModelList.Clear(); HitchList.Clear();
            cbs.GetCarBoostSettingConfigDataModel(SystemDirectoryConfig.GetInstance().GetBOOSTCONFIG(Common.controlVersion)).ForEach(q => CommonSettingModelList.Add(q));
            HitChModelBLL.GetInstance().GetHitchModelDataModel
                (SystemDirectoryConfig.GetInstance().GetHittingConfig(ControlVersion.Car)).Where(q => q.StatusOwner.Contains("status_Hitch_") && !string.IsNullOrEmpty(q.StatusName)).ToList().ForEach(q => HitchList.Add(q));
        }

        private void InitActionAndThread()
        {
            ButtonInvoke.SwitchLanguageEvent -= SwitchLanguage;
            ButtonInvoke.SwitchLanguageEvent += SwitchLanguage;

            ButtonInvoke.SwitchFontSizeEvent -= SwitchFontSize;
            ButtonInvoke.SwitchFontSizeEvent += SwitchFontSize;
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
                InqureStatus();
            }
        }


        public void SwitchLanguage(string language)
        {
            CommonSettingModule_Loaded(this,null);
        }

        public void SwitchFontSize(string FontSize)
        {
            CommonSettingModule_Loaded(this, null);
        }

        private Grid InitGrid()
        {
            Grid _MainGrid = MakeMainGrid();

            #region 顶部栏
            //DockPanel dp = new DockPanel()
            //{
            //    HorizontalAlignment = HorizontalAlignment.Stretch,
            //    VerticalAlignment = VerticalAlignment.Stretch,
            //    LastChildFill = true,
            //    Background = (LinearGradientBrush)this.FindResource("TitleBackGround"),
            //    Margin = new Thickness(0)
            //};
            //Border bd = new Border()
            //{
            //    BorderThickness = new Thickness(0, 0, 0, 0),
            //    BorderBrush = (LinearGradientBrush)this.FindResource("LnearBorderColor"),
            //    HorizontalAlignment = HorizontalAlignment.Right,
            //    VerticalAlignment = VerticalAlignment.Stretch,
            //    Width = 32,
            //    Background = StrToBrush("#00FFFFFF")
            //};
            //bd.PreviewMouseLeftButtonDown += Bd_PreviewMouseLeftButtonDown;
            //Canvas cs = new Canvas()
            //{
            //    Style = (Style)this.FindResource("diyCloseCanvas"),
            //    HorizontalAlignment = HorizontalAlignment.Right,
            //    Margin = new Thickness(0, 1, 10, 0)
            //};
            //bd.Child = cs;
            //Label _lblTitle = new Label()
            //{
            //    Content = GetName(),
            //    HorizontalAlignment = HorizontalAlignment.Left,
            //    FontSize = 18,
            //    FontWeight = FontWeights.Bold,
            //    Foreground = StrToBrush("#FFFFFF"),
            //    FontFamily = new FontFamily("宋体"),
            //    VerticalAlignment = VerticalAlignment.Stretch,
            //    HorizontalContentAlignment = HorizontalAlignment.Center,
            //    VerticalContentAlignment = VerticalAlignment.Center
            //};
            //dp.MouseDown += Dp_MouseDown;
            //dp.Children.Add(_lblTitle);
            //dp.Children.Add(bd);
            //Grid.SetColumn(dp, 0);
            //Grid.SetRow(dp, 0);
            //Grid.SetColumnSpan(dp, 3);
            //_MainGrid.Children.Add(dp);
            #endregion



            #region 手动出束
            DiyHandRay dhr = new DiyHandRay();
            Grid.SetColumn(dhr, 0);
            Grid.SetRow(dhr, CommonSettingModelList.Count+1);

            if (ConfigServices.GetInstance().localConfigModel.IsLogin && ConfigServices.GetInstance().localConfigModel?.Login?.LoginCode != RoleList.jjAdmin )
            {
                dhr.Visibility = Visibility.Collapsed;
            }

            _MainGrid.Children.Add(dhr);
            #endregion

            MakeMainButton(_MainGrid);

            MakeContentButton(_MainGrid);

            foreach (CommonSettingModel CommonSetting in CommonSettingModelList)
            {
                int index = CommonSettingModelList.IndexOf(CommonSetting) + 1;
                Grid _gd = MakeChildPanel(CommonSetting);
                Grid.SetRow(_gd, index);
                Grid.SetColumn(_gd, 0);
                _MainGrid.Children.Add(_gd);
            }

       

            return _MainGrid;
        }
     

        private void MakeContentButton(Grid _MainGrid)
        {
            Border outerBd = new Border()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                BorderBrush = StrToBrush("#A5B0BE"),
                BorderThickness = new Thickness(2, 2, 1, 1),
                Margin = new Thickness(0, 15, 10, 0),
                CornerRadius = new CornerRadius(2)
            };

            Grid HitchGrid = MakeHitchPanel(HitchList);
            HitchGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
            HitchGrid.VerticalAlignment = VerticalAlignment.Stretch;
            outerBd.Child = HitchGrid;

            Grid.SetColumn(outerBd, 1);
            Grid.SetRow(outerBd, 1);
            
            Grid.SetRowSpan(outerBd, (_MainGrid.RowDefinitions.Count - 3 == 0 ? 3 : _MainGrid.RowDefinitions.Count - 3)); 

            _MainGrid.Children.Add(outerBd);
        }

        private void MakeMainButton(Grid _MainGrid)
        {
            Border CorrectBtn = GetBtn();
            CorrectBtn.PreviewMouseDown -= CorrectBtn_PreviewMouseDown;
            CorrectBtn.PreviewMouseDown += CorrectBtn_PreviewMouseDown;
            CorrectBtn.Width = 190;
            CorrectBtn.Height = 30;
            CorrectBtn.SetBinding(Border.VisibilityProperty, new Binding("IsShow") { Source = _visibleObject });
            Grid.SetColumn(CorrectBtn, 1);
            Grid.SetRow(CorrectBtn, CommonSettingModelList.Count + 1);
            Grid.SetRowSpan(CorrectBtn, 2);
            _MainGrid.Children.Add(CorrectBtn);
        }

        private Grid MakeMainGrid()
        {
            Grid _MainGrid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Width = 880
            };
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Pixel) });
            for (int i = 0; i < CommonSettingModelList.Count; i++)
            {
                _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Star) });
            }
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Star) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Pixel) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(25, GridUnitType.Star) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30, GridUnitType.Star) });
            return _MainGrid;
        }

        private void Dp_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.CurrentWindow?.DragMove();
        }

        private void Bd_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 故障复位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CorrectBtn_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                WriteLogAction("发送故障复位状态！",LogType.ScanStep,true);
                if (!IsConnectionEquipment())
                {
                    BG_MESSAGEBOX.Show(CurrentWindow, UpdateStatusNameAction("Tip"), UnConnectionWithPlc);
                    return;
                }
                SendReset();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 弹出执行结果
        /// </summary>
        private void Result()
        {
            if (_visibleObject.IsShow == Visibility.Collapsed)
            {
                BG_MESSAGEBOX.Show(CurrentWindow, UpdateStatusNameAction("Tip"), EquipmentResetSuccess);
            }
            else
            {
                BG_MESSAGEBOX.Show(Application.Current?.MainWindow, UpdateStatusNameAction("Tip"), EquipmentResetFair);
            }
        }

        /// <summary>
        /// 故障重置
        /// </summary>
        private static void SendReset()
        {
            Task.Run(() =>
            {
                try
                {
                    Common.SetCommand(Common.CommandDic[Command.HitchReset], true);
                    //bool ExcuteResult = Common.GlobalPLCProtocol.Execute(22, 7, true);
                    Thread.Sleep(1000);
                    Common.SetCommand(Common.CommandDic[Command.HitchReset], false);
                }
                catch (System.Exception ex)
                {
                    CommonDeleget.HandTaskException(ex);
                }
            }).Wait();
        }

        private Grid MakeHitchPanel(ObservableCollection<StatusModel> HitchModelList)
        {
            Grid _dp = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch };
            int ColumnNum = HitchModelList.Count / 8;
            if (HitchModelList.Count % 8 != 0)
            {
                ColumnNum += 1;
            }
            for (int i = 0; i < ColumnNum; i++)
            {
                _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }
            for (int i = 0; i < 8; i++)
            {
                _dp.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            }
            int Index = 0;
            Index = CheckHitchStatus(HitchModelList, _dp, Index);
            return _dp;
        }
        /// <summary>
        /// 检测面板状态
        /// </summary>
        /// <param name="HitchModelList"></param>
        /// <param name="_dp"></param>
        /// <param name="Index"></param>
        /// <returns></returns>
        private int CheckHitchStatus(ObservableCollection<StatusModel> HitchModelList, Grid _dp, int Index)
        {
            decimal RowMaxNum = 8;
            decimal ColumnNUm = Math.Ceiling((decimal)HitchModelList.Count / (decimal)RowMaxNum);
            foreach (var HitchItem in HitchModelList)
            {
                Label lblHitch = new Label()
                {
                    Foreground = StrToBrush("#000000"),
                    Style = (Style)this.FindResource("diyLabel"),
                    FontSize = UpdateFontSizeAction(CMWFontSize.Normal),
                };
                lblHitch.Background = StrToBrush("#F4F4F4");
                if (!string.IsNullOrEmpty(HitchItem.StatusName))
                {
                    HitchItem.StatusName = UpdateStatusNameAction(HitchItem.StatusName);
                }
                lblHitch.SetBinding(Label.ContentProperty, new Binding("StatusName") { Source = HitchItem, Mode = BindingMode.TwoWay });
                lblHitch.SetBinding(Label.ForegroundProperty, new Binding("StatusCode") { Source = HitchItem, Mode = BindingMode.TwoWay, Converter = new HitchColorConvert() });

                Grid.SetColumn(lblHitch, Index / 8);
                Grid.SetRow(lblHitch, Index % 8);
                _dp.Children.Add(lblHitch);
                Index++;
            }

            return Index;
        }

        public Border GetBtn()
        {
            var BtnStyle = (Style)this.FindResource("diyBtnCarHandGreen");
            Border ParentBorder = new Border()
            {
                Style = BtnStyle
            };
            Label btnInner = new Label()
            {
                Content = UpdateStatusNameAction("ErrorReset"),
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#FFFFFF")
            };
            ParentBorder.Child = btnInner;
            return ParentBorder;
        }

        private Grid MakeChildPanel(CommonSettingModel _csm)
        {
            Grid _dp = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch };
            _dp.DataContext = _csm;
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
            Grid.SetColumn(ClycleBd, 0);
            Label lblLeft = new Label()
            {
                //Content = _csm.CommonSettingName,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblLeft.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            GetLastestValue(_csm);
            _csm.CommonSettingDisplayName = UpdateStatusNameAction(_csm.CommonSettingName);
            lblLeft.SetBinding(Label.ContentProperty, new Binding("CommonSettingDisplayName") { Source = _csm, Mode = BindingMode.TwoWay });
            Border BtnSendCommand = new Border()
            {
                Width = 50,
                Height = 30,
                Style = (Style)this.FindResource("diyBtnCarHand"),
            };
            BtnSendCommand.MouseLeftButtonDown += BtnSendCommand_PreviewMouseUp;
            Label btnInner= new Label()
            {
                Content = UpdateStatusNameAction("Setting"),
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#FFFFFF")
            };
            if (_csm.CommonSettingType != "Button" && _csm.CommonSettingType != "ToggleButton")
            {
                BtnSendCommand.Child = btnInner;
                _dp.Children.Add(BtnSendCommand);
                Grid.SetColumn(BtnSendCommand, 3);
                Grid.SetRow(BtnSendCommand, 0);
                BtnSendCommand.SetBinding(Border.TagProperty, new Binding(".") { Source = _csm, Mode = BindingMode.TwoWay });
                BtnSendCommand.DataContext = CommonSettingModelList;
            }
            ///如果当前是频率项
            if (_csm.CommonSettingName == "FrequencySetting")
            {
                ConfigServices.GetInstance().localConfigModel.Freeze = _csm.CommonSettingValue;
                CommonDeleget.UpdateConfigs("Frequency", _csm.CommonSettingValue,Section.SOFT);
            }
            //如果不是下拉类型的就生成TxtBox
            if (_csm.CommonSettingType == "TextBox")
            {
                MakeTextBox(_csm, _dp, btnInner);
            }
            else if (_csm.CommonSettingType == "DropDownList")
            {
                MakeDropDownList(_csm, _dp, btnInner);
            }
            else if (_csm.CommonSettingType == "ToggleButton")
            {
                MakeToggleButtonList(_csm, _dp, btnInner);
            }

            _dp.Children.Add(lblLeft);
            Grid.SetColumn(lblLeft, 1);
            return _dp;
        }

        private static void MakeTextBox(CommonSettingModel _csm, Grid _dp, Label btnInner)
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
            txtBox.SetBinding(TextBox.TextProperty, new Binding("CommonSettingValue") { Source = _csm, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay });
            _dp.Children.Add(txtBox);
            Grid.SetColumn(txtBox, 2);
            btnInner.SetBinding(Label.TagProperty, new Binding(".") { Source = txtBox });
        }

        private void MakeDropDownList(CommonSettingModel _csm, Grid _dp, Label btnInner)
        {
            ComboBox cbBox = new ComboBox()
            {
                Style = (Style)this.FindResource("stlComboBox"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(0, 3, 3, 3),
                Height = 35,
                Width = 98
            };
            foreach (var itemSelectObject in _csm.selectObject)
            {
                itemSelectObject.SelectText = UpdateStatusNameAction(itemSelectObject.SelectText);
            }
            cbBox.SelectedValuePath = "SelectValue";
            cbBox.DisplayMemberPath = "SelectText";
            cbBox.SetBinding(ComboBox.ItemsSourceProperty, new Binding("selectObject") { });
            cbBox.SetBinding(ComboBox.SelectedValueProperty, new Binding("CommonSettingValue") { Source = _csm, Mode = BindingMode.TwoWay });
            _dp.Children.Add(cbBox);
            Grid.SetColumn(cbBox, 2);
            btnInner.SetBinding(Label.TagProperty, new Binding(".") { Source = cbBox });
        }

        /// <summary>
        /// 产生ToggleButton按钮
        /// </summary>
        /// <param name="_csm"></param>
        /// <param name="_dp"></param>
        /// <param name="btnInner"></param>
        private void MakeToggleButtonList(CommonSettingModel _csm, Grid _dp, Label btnInner)
        {
            ToggleSwitchButton tsBtn = new ToggleSwitchButton()
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(0, 3, 5, 3),
                Height = 36,
                Width = 60
            };
            ///如果是预热设置
            ///先查询一下当前状态是不是已经是预热中了
            if (_csm.CommonSettingName == "LowerHanded")
            {
                tsBtn.IsChecked = GlobalRetStatus[17];

                if (!tsBtn.IsChecked) _csm.CommonSettingValue = "0";
                else _csm.CommonSettingValue = "1";
            }
            else
            {
                tsBtn.IsChecked = false;
            }
          
            tsBtn.Tag = _csm;
            tsBtn.Checked -= TsBtn_Checked;
            tsBtn.Checked += TsBtn_Checked;
            tsBtn.UnChecked -= TsBtn_UnChecked;
            tsBtn.UnChecked += TsBtn_UnChecked;
            _dp.Children.Add(tsBtn);
            Grid.SetColumn(tsBtn, 2);
            Grid.SetColumnSpan(tsBtn,2);
            btnInner.SetBinding(Label.TagProperty, new Binding(".") { Source = tsBtn });
        }
        /// <summary>
        /// ToggleButton取消选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TsBtn_UnChecked(object sender, RoutedEventArgs e)
        {
            CommonSettingModel Model = (sender as ToggleSwitchButton).Tag as CommonSettingModel;
            Model.CommonSettingValue = "0";
            SendToggleButtonCommand(sender,Model);
        }
        /// <summary>
        /// 发送ToggleButton指令
        /// </summary>
        /// <param name="Model"></param>
        private void SendToggleButtonCommand(object sender, CommonSettingModel Model)
        {
            ToggleSwitchButton tsb = (sender as ToggleSwitchButton);
            BuryingPoint($"Setting{Model.CommonSettingDisplayName.ToString()}Command");
            if (!IsConnectionEquipment())
            {
                tsb.IsShowStoryBoard = false;
                BG_MESSAGEBOX.Show(Application.Current?.MainWindow, UpdateStatusNameAction("Tip"), UnConnectionWithPlc);
                return;
            }
            tsb.IsShowStoryBoard = true;
            string Position = Model.CommonSettingPLCValue;
            byte StartPosition = Convert.ToByte(Position.Split('.')[0]);
            byte EndPosition = Convert.ToByte(Position.Split('.')[1]);
            bool StatusIndex = Convert.ToBoolean(Convert.ToInt32(Model.CommonSettingValue));
            ////var Status = Common.GlobalRetStatus[StatusIndex];
            //如果已经加了高压/低压 就取消
            Task.Run(() =>
            {
                try
                {
                    PLCControllerManager.GetInstance().WritePositionValue(Position, StatusIndex);
                    //Common.GlobalPLCProtocol.Execute(StartPosition, EndPosition, StatusIndex);
                }
                catch (System.Exception ex)
                {
                    CommonDeleget.HandTaskException(ex);
                }
            }).Wait();
            Thread.Sleep(50);
            cbs.SaveConfigDataModel(CommonSettingModelList);
        }

        /// <summary>
        /// ToggleButton选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TsBtn_Checked(object sender, RoutedEventArgs e)
        {
            CommonSettingModel Model = (sender as ToggleSwitchButton).Tag as CommonSettingModel;
            Model.CommonSettingValue = "1";
            SendToggleButtonCommand(sender,Model);
        }

        public void GetLastestValue(CommonSettingModel csm)
        {
            switch (csm.CommonSettingName)
            {
                case "EnergySettings":
                    csm.CommonSettingDisplayName = Common.SetDoubleOrSingle();
                    if (!string.IsNullOrEmpty(csm.CommonSettingDisplayName))
                    {
                        csm.CommonSettingValue = csm.selectObject.FirstOrDefault(q => q.SelectText == csm.CommonSettingDisplayName)?.SelectValue;
                    }

                    break;
                default:
                    break;
            }
        }

        private void BtnSendCommand_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (!IsConnectionEquipment())
                {
                    BG_MESSAGEBOX.Show(Application.Current?.MainWindow, UpdateStatusNameAction("Tip"), UnConnectionWithPlc);
                    //MessageBox.Show(UnConnectionWithPlc);
                    return;
                }
                bool isSendSuccess = false;
                isSendSuccess = SendCommand(sender);
                if (isSendSuccess)
                {
                    cbs.SaveConfigDataModel(CommonSettingModelList);
                }

                string Message = isSendSuccess ? UpdateStatusNameAction("SettingSuccess") : UpdateStatusNameAction("SettingFaild");
            }
            catch (Exception ex)
            {
                BG_MESSAGEBOX.Show(Application.Current?.MainWindow, UpdateStatusNameAction("Tip"), ex.Message);
                //MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 状态查询
        /// </summary>
        private void InqureStatus()
        {

            try
            {
                //Common.Inquire();
                if (!Common.IsConnection)
                {
                    foreach (var HitchItem in HitchList)
                    {
                        HitchItem.StatusCode = "0";
                    }
                    _visibleObject.IsShow = Visibility.Visible;
                }
                else
                {
                    foreach (var HitchItem in HitchList)
                    {
                        int ItemIndex = Convert.ToInt32(HitchItem.StatusIndex);
                        if (ItemIndex < Common.GlobalRetStatus.Count)
                        {
                            HitchItem.StatusCode = Common.GlobalRetStatus[ItemIndex] ? "0" : "2";
                        }
                    }
                    if (HitchList.Count(q => q.StatusCode == "0") == 0)
                    {
                        _visibleObject.IsShow = Visibility.Collapsed;
                    }
                    else
                    {
                        _visibleObject.IsShow = Visibility.Visible;
                    }
                }
            }
            catch (Exception ex)
            {
                CommonDeleget.HandTaskException(ex);
            }
        }
    }
}
