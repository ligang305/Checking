using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using CMW.Common.Utilities;
using BGCommunication;
using BGModel;
using BGUserControl;
using static  CMW.Common.Utilities.CommonDeleget;
using static  CMW.Common.Utilities.CommonFunc;
using static  CMW.Common.Utilities.Common;

using BG_Services;
using BG_Entities;
using BG_WorkFlow;

namespace BGUserControl
{
    [Export("ContentPage", typeof(BaseModules))]
    [CustomExportMetadata(1, Modules.FastCheck_FlowSettingModule, "流程及模式设置", "ZZW", "1.0.0")]
    public class FastCheck_FlowSetting: BaseModules
    {
        Grid _MainGrid = new Grid();
        Border _MainBorder = new Border()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
        };
        FlowBLL FFB = new FlowBLL(ControlVersion.FastCheck);
        ObservableCollection<CommonSettingModel> CommonSettingModelList = new ObservableCollection<CommonSettingModel>();
        bool? isVisible = false;
        [ImportingConstructor]
        public FastCheck_FlowSetting() : base(ControlVersion.FastCheck)
        {
            Loaded += FastCheck_FlowSetting_Loaded; 
            Unloaded += FastCheck_FlowSetting_Unloaded;
            IsVisibleChanged += FastCheck_FlowSetting_IsVisibleChanged;
        }

        private void FastCheck_FlowSetting_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            isVisible = e.NewValue as bool?;
            if (isVisible != null && isVisible == true)
            {
                Task.Factory.StartNew(() => { InquireValueThread(); });
            }
        }
        /// <summary>
        /// 这是为了防止PLC断电，所以第一次进入的时候就去读取一下PLC保存的值，然后赋值
        /// </summary>
        private void InquireValueThread()
        {
            //while(true)
            //{
            //if (isVisible != true) break;
            //Thread.Sleep(500);
            try
            {
                var CommonModelList = CommonSettingModelList.Where(q => q.CommonSettingType == "TextBox");
                foreach (var CommonItem in CommonModelList)
                {
                    if (CommonItem.CommonSettingName.Contains("BoostingTO") ||
                     CommonItem.CommonSettingName.Contains("EnterTO") ||
                     CommonItem.CommonSettingName.Contains("ExitTO") ||
                     CommonItem.CommonSettingName.Contains("ControlRoomTO"))
                    {
                        StartBytePositionDefaultText(CommonItem, (Convert.ToInt32(CommonItem.CommonSettingValue) * 10).ToString());
                        continue;
                    }
                    StartBytePositionDefaultText(CommonItem, CommonItem.CommonSettingValue);
                }
            }
            catch (Exception ex)
            {
            }
        }


        private void FastCheck_FlowSetting_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
            }
        }

        private void FastCheck_FlowSetting_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            base.Base_SwitchLanguage(ConfigServices.GetInstance().localConfigModel.LANGUAGE);
            InitListData();
            InitContent();
        }
        private void InitListData()
        {
            CommonSettingModelList.Clear(); 
            FFB.GetCarBoostSettingConfigDataModel(SystemDirectoryConfig.GetInstance().GetFlowConfig(Common.controlVersion)).ForEach(q => CommonSettingModelList.Add(q));
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
        private Grid InitGrid()
        {
            Grid _MainGrid = MakeMainGrid();
            var CurrentRole = ConfigServices.GetInstance().localConfigModel.Login?.LoginCode;
            if (!ConfigServices.GetInstance().localConfigModel.IsLogin || CurrentRole != null && CurrentRole.Equals("jjAdmin") )
            {
                MakeDynamicOrFixFreeze(_MainGrid);
            }

            foreach (CommonSettingModel CommonSetting in CommonSettingModelList)
            {
                int index = CommonSettingModelList.IndexOf(CommonSetting)  + ((!ConfigServices.GetInstance().localConfigModel.IsLogin || CurrentRole.Equals("jjAdmin")) ? 2 : 1);
                if (CommonSetting.CommonSettingName.Contains("ModeSetting") && ConfigServices.GetInstance().localConfigModel.IsLogin && !CurrentRole.Equals("jjAdmin"))
                {
                    Grid _tempgd = MakeChildPanel(CommonSetting);
                    Grid.SetRow(_tempgd, index);
                    Grid.SetColumn(_tempgd, 0);
                    _MainGrid.Children.Add(_tempgd);
                    break;
                }
                Grid _gd = MakeChildPanel(CommonSetting);
                Grid.SetRow(_gd, index);
                Grid.SetColumn(_gd, 0);
                _MainGrid.Children.Add(_gd);
            }
          
            return _MainGrid;
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
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30, GridUnitType.Star) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            return _MainGrid;
        }
        private Grid MakeChildPanel(CommonSettingModel _csm)
        {
            Grid _dp = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch };
            _dp.DataContext = _csm;
            _dp.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Star) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70, GridUnitType.Pixel) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(45, GridUnitType.Star) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(120, GridUnitType.Pixel) });
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
            Label btnInner = new Label()
            {
                Content = UpdateStatusNameAction("Setting"),
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#FFFFFF")
            };
            btnInner.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
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
                CommonDeleget.UpdateConfigs("Frequency", _csm.CommonSettingValue, Section.SOFT);
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
            txtBox.SetBinding(TextBox.TextProperty, new Binding("CommonSettingValue") { Source = _csm, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                ValidatesOnExceptions = true,
                NotifyOnValidationError = true,
                ValidatesOnDataErrors = true, Mode = BindingMode.TwoWay });
            txtBox.SetBinding(TextBox.ToolTipProperty,new Binding("(Validation.Errors)[0].ErrorContent"){RelativeSource = RelativeSource.Self});

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
                //GlobalRetStatus[18] ? GlobalRetStatus[18] : GlobalRetStatus[17];

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
            SendToggleButtonCommand(sender, Model);
        }
        /// <summary>
        /// 发送ToggleButton指令
        /// </summary>
        /// <param name="Model"></param>
        private void SendToggleButtonCommand(object sender, CommonSettingModel Model)
        {
            ToggleSwitchButton tsb = (sender as ToggleSwitchButton);
            BuryingPoint($"Command {Model.CommonSettingDisplayName.ToString()}  Command");
            if (!IsConnectionEquipment())
            {
                tsb.IsShowStoryBoard = false;
                BG_MESSAGEBOX.Show(Application.Current?.MainWindow, UpdateStatusNameAction("Tip"), UnConnectionWithPlc);
                return;
            }
            tsb.IsShowStoryBoard = true;
            string Position = Model.CommonSettingPLCValue;
            //byte StartPosition = Convert.ToByte(Position.Split('.')[0]);
            //byte EndPosition = Convert.ToByte(Position.Split('.')[1]);
            bool value = Convert.ToBoolean(Convert.ToInt32(Model.CommonSettingValue));
            ////var Status = Common.GlobalRetStatus[StatusIndex];
            //如果已经加了高压/低压 就取消
            Task.Run(() =>
            {
                try
                {
                    PLCControllerManager.GetInstance().WritePositionValue(Position, value);
                    //Common.GlobalPLCProtocol.Execute(StartPosition, EndPosition, StatusIndex);
                }
                catch (System.Exception ex)
                {
                    CommonDeleget.HandTaskException(ex);
                }
            }).Wait();
            Thread.Sleep(50);
            FFB.SaveConfigDataModel(CommonSettingModelList);
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
                if (!Common.IsConnection)
                {
                    BG_MESSAGEBOX.Show(Application.Current?.MainWindow, UpdateStatusNameAction("Tip"), UnConnectionWithPlc);
                    //MessageBox.Show(UnConnectionWithPlc);
                    return;
                }
                bool isSendSuccess = false;
                isSendSuccess = SendCommand(sender);
                //if (isSendSuccess)
                //{
                    FFB.SaveConfigDataModel(CommonSettingModelList);
                //}

                string Message =  UpdateStatusNameAction("SettingSuccess");
            }
            catch (Exception ex)
            {
                BG_MESSAGEBOX.Show(Application.Current?.MainWindow, UpdateStatusNameAction("Tip"), ex.Message);
                //MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// 设置是固定频率还是根据车速的可变频率
        /// </summary>
        public void MakeDynamicOrFixFreeze(Grid ParentGrid)
        {

            Grid _dp = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch };
            _dp.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Star) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70, GridUnitType.Pixel) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(45, GridUnitType.Star) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(120, GridUnitType.Pixel) });
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
            Label lblName = new Label()
            {
                Content = UpdateStatusNameAction("SelectFreezeMode"),
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            lblName.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            ComboBox cbBox = new ComboBox()
            {
                Style = (Style)this.FindResource("stlComboBox"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(0, 3, 3, 3),
                Height = 35,
                Width = 98
            };
            cbBox.Items.Add(new SelectObject() { SelectText = UpdateStatusNameAction("FixMode"), SelectValue = "FIXED" });
            cbBox.Items.Add(new SelectObject() { SelectText = UpdateStatusNameAction("Dynamic"), SelectValue = "DYNAMIC" });
            cbBox.DisplayMemberPath = "SelectText";
            cbBox.SelectedValuePath = "SelectValue";
            cbBox.SelectedValue = ConfigServices.GetInstance().localConfigModel.FreezeMode;
            Grid.SetRow(lblName, 0);
            Grid.SetRow(cbBox, 0);
            Grid.SetColumn(lblName, 1);
            Grid.SetColumn(cbBox, 2);
            _dp.Children.Add(lblName);
            _dp.Children.Add(cbBox);
            Border btnSetting = new Border()
            {
                Width = 50,
                Height = 30,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Style = (Style)this.FindResource("diyBtnCarHand"),
            };
            btnSetting.Tag = cbBox;
            btnSetting.PreviewMouseDown += BtnSetting_PreviewMouseDown;
            Label lblSetting = new Label()
            {
                Content = UpdateStatusNameAction("Setting"),
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#FFFFFF")
            };
            lblSetting.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            btnSetting.Child = lblSetting;
            Grid.SetColumn(btnSetting, 3);
            Grid.SetRow(btnSetting, 0);
            _dp.Children.Add(btnSetting);
            Grid.SetRow(_dp, 1);
            Grid.SetColumn(_dp, 0);
            ParentGrid.Children.Add(_dp);
        }
        private void BtnSetting_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ComboBox cbx = (sender as Border).Tag as ComboBox;
            BuryingPoint($"Setting The frequency mode is {cbx.SelectedValue.ToString()}");
            //if (cbx.SelectedValue.ToString().Equals("dynamic"))
            //{
            //    Common.SetCommand("");
            //}
            UpdateConfigs("FreezeMode", cbx.SelectedValue.ToString(), Section.SOFT);
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
    }
}
