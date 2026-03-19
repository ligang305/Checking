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
using BG_Entities;
using BG_WorkFlow;

namespace BGUserControl
{
    [Export("ContentPage", typeof(BaseModules))]
    [CustomExportMetadata(1, "SelfWorkingCarComponentModule", "SelfWorking_自行走独立设备设置", "ZZW", "1.0.0")]
    public class SelfWorkingCarComponentModule : BaseModules
    {
        ObservableCollection<BoostingModel> BoostingModelList = new ObservableCollection<BoostingModel>();
        SelfWorkingComponentBll cbs = new SelfWorkingComponentBll(ControlVersion.SelfWorking);
        Self_BoostSettingBLL sbb = new Self_BoostSettingBLL(controlVersion);
        string CarComponentControl;
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
        public SelfWorkingCarComponentModule() : base(ControlVersion.SelfWorking)
        {
            CarComponentControl = "AuxiliaryEquipmentControl";
            Unloaded += SelfWorkingCarComponentModule_Unloaded;
            Loaded += SelfWorkingCarComponentModule_Loaded;
            IsVisibleChanged += SelftWorkingHandBoostSettingModule_IsVisibleChanged;
            ButtonInvoke.SwitchLanguageEvent -= SwitchLanguage;
            ButtonInvoke.SwitchLanguageEvent += SwitchLanguage;

            ButtonInvoke.SwitchFontSizeEvent -= SwitchFontSize;
            ButtonInvoke.SwitchFontSizeEvent += SwitchFontSize;
        }

        public void InitData()
        {
            CommonSettingModelList.Clear(); 
            cbs.GetSelfWorkingComponentDataModel(SystemDirectoryConfig.GetInstance().GetComponentConfig(Common.controlVersion)).ForEach(q => CommonSettingModelList.Add(q));
           
            BoostingModelList.Clear();
            sbb.GetBoostingModelList(Common.controlVersion).ToList().ForEach(q => BoostingModelList.Add(q));
        }
        
        /// <summary>
        /// 切换字体
        /// </summary>
        /// <param name="language"></param>
        public void SwitchFontSize(string Fontsize)
        {
            SelfWorkingCarComponentModule_Loaded(this, null);
        }
        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="language"></param>
        public void SwitchLanguage(string language)
        {
            SelfWorkingCarComponentModule_Loaded(this, null);
        }
        private void SelfWorkingCarComponentModule_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            InitData();
          
            MainGrid = InitMainGrid();

            MainBorder.Child = MainGrid;
            Content = MainBorder;
        }

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
            try
            {
                foreach (var CommonSettingItem in CommonSettingModelList)
                {
                    if (CommonSettingItem.CommonSettingType == "ToggleButton")
                    {
                        MakeToggleButtonList(CommonSettingItem, gd);
                    }
                    else if (CommonSettingItem.CommonSettingType == "Button")
                    {
                        MakeSingleButton(gd, CommonSettingItem);
                    }
                    
                }
            }
            catch (Exception ex)
            {
            }
            return gd;
        }
        /// <summary>
        /// 产生ToggleButton按钮
        /// </summary>
        /// <param name="_csm"></param>
        /// <param name="_dp"></param>
        /// <param name="btnInner"></param>
        private void MakeToggleButtonList(CommonSettingModel _csm, Grid _MainGrid)
        {
            Grid _dp = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch };
            _dp.DataContext = _csm;
            _dp.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Star) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70, GridUnitType.Pixel) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(35, GridUnitType.Star) });
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
            _csm.CommonSettingDisplayName = UpdateStatusNameAction(_csm.CommonSettingName);
            lblLeft.SetBinding(Label.ContentProperty, new Binding("CommonSettingDisplayName") { Source = _csm, Mode = BindingMode.TwoWay });
            _dp.Children.Add(lblLeft);
            Grid.SetColumn(lblLeft, 1);
            ToggleSwitchButton tsBtn = new ToggleSwitchButton()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(0, 3, 5, 3),
                Height = 36,
                Width = 60
            };
            tsBtn.IsChecked = _csm.CommonSettingValue =="1"?true:false;
            tsBtn.Tag = _csm;
            tsBtn.Checked -= TsBtn_Checked;
            tsBtn.Checked += TsBtn_Checked;
            tsBtn.UnChecked -= TsBtn_UnChecked;
            tsBtn.UnChecked += TsBtn_UnChecked;
            _dp.Children.Add(tsBtn);
            Grid.SetColumn(tsBtn, 2);
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            Grid.SetRow(_dp, CommonSettingModelList.IndexOf(_csm));
            _MainGrid.Children.Add(_dp);
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
            CommonSettingModelList.First(q => q == Model).CommonSettingValue = "1";
            SendToggleButtonCommand(sender, Model);
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
        /// ToggleButton取消选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TsBtn_UnChecked(object sender, RoutedEventArgs e)
        {
            CommonSettingModel Model = (sender as ToggleSwitchButton).Tag as CommonSettingModel;
            Model.CommonSettingValue = "0";
            CommonSettingModelList.First(q => q == Model).CommonSettingValue = "0";
            SendToggleButtonCommand(sender, Model);
        }
        private void MakeSingleButton(Grid gd, CommonSettingModel CommonSettingItem)
        {
            Grid MainGd = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Center };
            MainGd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            MainGd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70, GridUnitType.Pixel) });
            MainGd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(35, GridUnitType.Star) });
            MainGd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30, GridUnitType.Star) });
            MainGd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70, GridUnitType.Pixel) });
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
            MainGd.Children.Add(ClycleBd);
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
            MainGd.Children.Add(lblLeft);
            CommonSettingItem.CommonSettingDisplayName = UpdateStatusNameAction(CommonSettingItem.CommonSettingName);
            lblLeft.SetBinding(Label.ContentProperty, new Binding("CommonSettingDisplayName") { Source = CommonSettingItem, Mode = BindingMode.TwoWay });
            Grid.SetColumn(lblLeft, 1);
            DiyRadioButton drb = new DiyRadioButton();
            drb.ItemSource = CommonSettingItem;
            Grid.SetRow(drb, 0);
            Grid.SetColumn(drb, 2);
            MainGd.Children.Add(drb);
            drb.MouseButonEnent += _BtnSendCommand_PreviewMouseUp;
            gd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            //gb.Content = MainGd;
            Grid.SetRow(MainGd, CommonSettingModelList.IndexOf(CommonSettingItem) );
            gd.Children.Add(MainGd);
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
                Thread.Sleep(100);
            }
        }
        private void SelfWorkingCarComponentModule_Unloaded(object sender, System.Windows.RoutedEventArgs e)
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
            return CarComponentControl;
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
