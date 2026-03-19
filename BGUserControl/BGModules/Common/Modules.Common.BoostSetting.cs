using BG_Services;
using CMW.Common.Utilities;
using BGCommunication;
using BGModel;
using BGUserControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shell;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.CommonFunc;
using BG_Entities;
using BG_WorkFlow;

namespace BGUserControl
{
    [Export("ContentPage", typeof(BaseModules))]
    [CustomExportMetadata(1,Modules.BegoodBoostSettingModule, "达胜直线加速器", "ZZW", "1.0.0")]
    public class BegoodBoostSettingModule : BaseModules
    {
        Grid _MainGrid = new Grid();
        Border _MainBorder = new Border()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Margin = new Thickness(0),
            Background = StrToBrush("#F2F2F2")
        };
        AcceleatorMvvm _AcceleatorMvvm = new AcceleatorMvvm();
        [ImportingConstructor]
        public BegoodBoostSettingModule() : base(Common.controlVersion)
        {
            Loaded += CommonSettingModule_Loaded;
            Unloaded += CommonSettingModule_Unloaded;
        }

        //WPF 机制如此，控件消失，先调用isVisibles 后调用Unloaded 不然双向绑定事件无法触发
        private void CommonSettingModule_Unloaded(object sender, RoutedEventArgs e)
        {
            ButtonInvoke.SwitchLanguageEvent -= SwitchLanguage;
        }
        /// <summary>
        /// 获取名称
        /// </summary>
        /// <returns></returns>
        public override string GetName()
        {
            return "BoosttingSetting";
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
            return 840;
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
            _OwnerWin.MaxWidth = GetWidth();
            _OwnerWin.MaxHeight = GetHeight();
            _OwnerWin.Title = GetName();
            _OwnerWin.Content = this;
            _OwnerWin.Show();
        }
        protected void CommonSettingModule_Loaded(object sender, RoutedEventArgs e)
        {
            base.Base_SwitchLanguage(ConfigServices.GetInstance().localConfigModel.LANGUAGE);
            _MainGrid = InitGrid();
            _MainBorder.Child = _MainGrid;
            Content = _MainBorder;
            InitAction();

        }
        private void InitAction()
        {
            ButtonInvoke.SwitchLanguageEvent -= SwitchLanguage;
            ButtonInvoke.SwitchLanguageEvent += SwitchLanguage;
            ButtonInvoke.SwitchFontSizeEvent -= SwitchFontSize;
            ButtonInvoke.SwitchFontSizeEvent += SwitchFontSize;
        }
     
        public void SwitchFontSize(string language)
        {
            CommonSettingModule_Loaded(null, null);

        }
        public void SwitchLanguage(string language)
        {
            CommonSettingModule_Loaded(null, null);
        }
        private Grid InitGrid()
        {
            Grid _MainGrid = InitMainGrid();

            #region 顶部栏
            //InitTop(_MainGrid);
            #endregion

            InitCorrentButton(_MainGrid);

            InitOutBorder(_MainGrid);

            foreach (CommonSettingModel CommonSetting in _AcceleatorMvvm.CommonSettingModelList)
            {
                int index = _AcceleatorMvvm.CommonSettingModelList.IndexOf(CommonSetting) + 1;
                Grid _gd = MakeChildPanel(CommonSetting);
                Grid.SetRow(_gd, index);
                Grid.SetColumn(_gd, 0);
                _MainGrid.Children.Add(_gd);
            }


            #region 出束停束按钮
            DiyHandRay dhr = new DiyHandRay();
            Grid.SetColumn(dhr, 0);
            Grid.SetRow(dhr, _AcceleatorMvvm.CommonSettingModelList.Count + 1);
            _MainGrid.Children.Add(dhr);
            #endregion

            return _MainGrid;
        }
        /// <summary>
        /// 右侧监控栏
        /// </summary>
        /// <param name="_MainGrid"></param>
        private void InitOutBorder(Grid _MainGrid)
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

            Grid HitchGrid = MakeHitchPanel(_AcceleatorMvvm.HitchList);
            HitchGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
            HitchGrid.VerticalAlignment = VerticalAlignment.Stretch;
            outerBd.Child = HitchGrid;

            Grid.SetColumn(outerBd, 1);
            Grid.SetRow(outerBd, 1);
            if (_MainGrid.RowDefinitions.Count - 3 !=0)
            {
                Grid.SetRowSpan(outerBd, _MainGrid.RowDefinitions.Count - 3);
            }
            _MainGrid.Children.Add(outerBd);
        }
        private void InitCorrentButton(Grid _MainGrid)
        {
            Border CorrectBtn = GetBtn();
            CorrectBtn.PreviewMouseDown -= CorrectBtn_PreviewMouseDown;
            CorrectBtn.PreviewMouseDown += CorrectBtn_PreviewMouseDown;
            CorrectBtn.Width = 190;
            CorrectBtn.Height = 30;
            CorrectBtn.SetBinding(Border.VisibilityProperty, new Binding("IsShow") { Source = _AcceleatorMvvm._visibleObject });
            Grid.SetColumn(CorrectBtn, 1);
            Grid.SetRow(CorrectBtn, _AcceleatorMvvm.CommonSettingModelList.Count);
            Grid.SetRowSpan(CorrectBtn, 3);
            _MainGrid.Children.Add(CorrectBtn);
        }
        private Grid InitMainGrid()
        {
            Grid _MainGrid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Pixel) });
            for (int i = 0; i < _AcceleatorMvvm.CommonSettingModelList.Count; i++)
            {
                _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Star) });
            }
            //这一行是加给出束停束按钮的
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Star) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Pixel) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(40, GridUnitType.Star) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30, GridUnitType.Star) });
            return _MainGrid;
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
                
                Task.Run(() =>
                {
                    bool Result = false;
                    try
                    {
                        bool ExcuteResult = SetCommand(CommandDic[Command.RayReset], true);
                        Thread.Sleep(500);
                        Result = SetCommand(CommandDic[Command.RayReset], false);
                        Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            if (Result)
                            {
                                BG_MESSAGEBOX.Show(CurrentWindow, UpdateStatusNameAction("Tip"), EquipmentResetSuccess);
                            }
                            else
                            {
                                BG_MESSAGEBOX.Show(Application.Current?.MainWindow, UpdateStatusNameAction("Tip"), EquipmentResetFair);
                            }
                        }));
                    }
                    catch (System.Exception ex)
                    {
                        CommonDeleget.HandTaskException(ex);
                    }

                });
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
            foreach (var HitchItem in HitchModelList)
            {
                Label lblHitch = new Label()
                {
                    Foreground = StrToBrush("#000000"),
                    Style = (Style)this.FindResource("diyLabel"),
                    FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.Normal),
                };
                if ((Index + Index / 8) % 2 == 0)
                {
                    lblHitch.Background = StrToBrush("#F4F4F4");
                }
                else
                {
                    lblHitch.Background = StrToBrush("#FFFFFF");
                }
                if (!string.IsNullOrEmpty(HitchItem.StatusName))
                {
                    HitchItem.StatusDisplayName = UpdateStatusNameAction(HitchItem.StatusName);
                }
                lblHitch.SetBinding(Label.ContentProperty, new Binding("StatusDisplayName") { Source = HitchItem, Mode = BindingMode.TwoWay });
                lblHitch.SetBinding(Label.ForegroundProperty, new Binding("StatusCode") { Source = HitchItem, Mode = BindingMode.TwoWay, Converter = new HitchColorConvert() });

                Grid.SetColumn(lblHitch, Index / 8);
                Grid.SetRow(lblHitch, Index % 8);
                _dp.Children.Add(lblHitch);
                Index++;
            }
            return _dp;
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
            //GetLastestValue(_csm);
            _csm.CommonSettingDisplayName = UpdateStatusNameAction(_csm.CommonSettingName);
            lblLeft.SetBinding(Label.ContentProperty, new Binding("CommonSettingDisplayName") { Source = _csm, Mode = BindingMode.TwoWay });
            Border BtnSendCommand = new Border()
            {
                Width = 50,
                Height = 30,
                Style = (Style)this.FindResource("diyBtnCarHand"),
            };
            BtnSendCommand.MouseLeftButtonDown += BtnSendCommand_PreviewMouseUp;
            Label btnInner = new Label()
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
                BtnSendCommand.DataContext = _AcceleatorMvvm.CommonSettingModelList;
            }
            ///如果当前是频率项
            if (_csm.CommonSettingName == "FrequencySetting")
            {
                _csm.CommonSettingValue = ConfigServices.GetInstance().localConfigModel.Freeze;
                //CommonDeleget.UpdateConfigs("Frequency", _csm.CommonSettingValue);
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
        /// <summary>
        /// 获取能量的值
        /// </summary>
        /// <param name="csm"></param>
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
        private void MakeTextBox(CommonSettingModel _csm, Grid _dp, Label btnInner)
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
            txtBox.Style = (Style)this.FindResource("txtValid");
            txtBox.SetBinding(TextBox.TextProperty, new Binding("CommonSettingValue")
            {
                Source = _csm, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay,
                ValidatesOnExceptions = true,
                NotifyOnValidationError = true,
                ValidatesOnDataErrors = true,
            });
            txtBox.SetBinding(TextBox.ToolTipProperty, new Binding("(Validation.Errors)[0].ErrorContent") { RelativeSource = RelativeSource.Self });
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
                tsBtn.IsChecked = PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.PreheatEnding) ?
                                  PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.PreheatEnding) :
                                  PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.Preheating);

                if (!tsBtn.IsChecked) _csm.CommonSettingValue = "0";
                else _csm.CommonSettingValue = "1";
            }
            else if (_csm.CommonSettingName.Contains("IsOutTouch"))
            {
                tsBtn.IsChecked = true;
            }
            else
            {
                tsBtn.IsChecked = _csm.CommonSettingValue == "1";
            }
            
            tsBtn.Tag = _csm;
            tsBtn.Checked -= TsBtn_Checked;
            tsBtn.Checked += TsBtn_Checked;
            tsBtn.UnChecked -= TsBtn_UnChecked;
            tsBtn.UnChecked += TsBtn_UnChecked;
            _dp.Children.Add(tsBtn);
            Grid.SetColumn(tsBtn, 2);
            Grid.SetColumnSpan(tsBtn, 2);
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
            ToggleSwitchButton tsb = sender as ToggleSwitchButton;
            BuryingPoint($"Setting {Model.CommonSettingDisplayName.ToString()} Command");
            if (!IsConnectionEquipment())
            {
                BG_MESSAGEBOX.Show(Application.Current?.MainWindow, UpdateStatusNameAction("Tip"), UnConnectionWithPlc);
                tsb.IsShowStoryBoard = false;
                return;
            }
            string Position = Model.CommonSettingPLCValue;
            bool value = Convert.ToBoolean(Convert.ToInt32(Model.CommonSettingValue));
            //如果已经加了高压/低压 就取消
            Task.Run(() =>
            {
                try
                {
                    PLCControllerManager.GetInstance().WritePositionValue(Position, value);
                }
                catch (System.Exception ex)
                {
                    CommonDeleget.HandTaskException(ex);
                }
            }).Wait();
            Thread.Sleep(50);
            _AcceleatorMvvm.SaveCommonSettingModelList();
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
            SendToggleButtonCommand(sender, Model);
        }
        private void BtnSendCommand_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)  
        {
            try
            {
                if (!IsConnectionEquipment())
                {
                    BG_MESSAGEBOX.Show(Application.Current?.MainWindow, UpdateStatusNameAction("Tip"), UnConnectionWithPlc);
                    return;
                }
                bool isSendSuccess = false;
                isSendSuccess = SendCommand(sender);
                _AcceleatorMvvm.SaveCommonSettingModelList();

                string Message = isSendSuccess ? UpdateStatusNameAction("SettingSuccess") : UpdateStatusNameAction("SettingFaild");
                if (!isSendSuccess)
                {
                    BG_MESSAGEBOX.Show(Application.Current?.MainWindow, UpdateStatusNameAction("Tip"), Message);
                }
            }
            catch (Exception ex)
            {
                BG_MESSAGEBOX.Show(Application.Current?.MainWindow, UpdateStatusNameAction("Tip"), ex.Message);
            }
        }

    }


    public class AcceleatorMvvm:BaseMvvm
    {
        public ObservableCollection<CommonSettingModel> CommonSettingModelList = new ObservableCollection<CommonSettingModel>();
        public ObservableCollection<StatusModel> HitchList = new ObservableCollection<StatusModel>();
        public Car_BoostSettingBLL cbs = new Car_BoostSettingBLL(Common.controlVersion);

        public Visibsliy _visibleObject = new Visibsliy(){DisplayName = "未出束"};
        public AcceleatorMvvm()
        {
            InitData();
            DetecotrControllerManager.GetInstance().DetecotorConnectionAction -= DetecotorConnection;
            DetecotrControllerManager.GetInstance().DetecotorConnectionAction += DetecotorConnection;
        }
        private void InitData()
        {
            CommonSettingModelList.Clear(); HitchList.Clear();
            cbs.GetCarBoostSettingConfigDataModel(SystemDirectoryConfig.GetInstance().GetBOOSTCONFIG(Common.controlVersion)).ForEach(q => CommonSettingModelList.Add(q));
            HitChModelBLL.GetInstance().GetHitchModelDataModel
                (SystemDirectoryConfig.GetInstance().GetHittingConfig(Common.controlVersion)).Where(q => q.StatusOwner.Contains("status_Hitch_")).ToList().ForEach(q => HitchList.Add(q));
        }
        public override void LoadUIText()
        {

        }
        protected override void InquirePlcStatus(List<bool> StatusList)
        {
            try
            {
                if (!IsSearchStatusSuccess)
                {
                    foreach (var HitchItem in HitchList)
                    {
                        HitchItem.StatusCode = "0";
                    }
                }
                else
                {
                    foreach (var HitchItem in HitchList)
                    {
                        int ItemIndex = Convert.ToInt32(HitchItem.StatusIndex);
                        if (ItemIndex < GlobalRetStatus.Count)
                        {
                            HitchItem.StatusCode = GlobalRetStatus[ItemIndex] == Convert.ToBoolean(HitchItem.DefaultValue) ? "2" : "0";
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
        protected override void ConnectionStatus(bool ConnectionStatus)
        {

        }
        public void SaveCommonSettingModelList()
        {
            cbs.SaveConfigDataModel(CommonSettingModelList);
        }
        public void DetecotorConnection(bool DetecotrConnection)
        {

        }
    }
}
