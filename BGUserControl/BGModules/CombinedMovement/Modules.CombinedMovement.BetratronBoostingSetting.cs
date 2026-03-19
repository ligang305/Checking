using CMW.Common.Utilities;
using BGModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
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
using BG_Entities;
using BG_WorkFlow;
using static BG_WorkFlow.HitChModelBLL;

namespace BGUserControl
{
    [Export("ContentPage", typeof(BaseModules))]
    [CustomExportMetadata(1,Modules.CombinedMovement_FHBetratronBoostSetting, "FHBetratron加速器设置模块", "ZZW", "1.0.0")]
    public class CombinedMovement_BetratronBoostingSetting : BaseModules
    {
        string SelfWorkingBoosting;
        //判定是否预热
        static bool isPreHot = false;
        Border MainBorder = new Border()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
        };
        Grid MainGrid = new Grid()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
        };
        ObservableCollection<BoostingStatusModel> StatusModelList = new ObservableCollection<BoostingStatusModel>();
        ObservableCollection<BoostingModel> BoostingModelList = new ObservableCollection<BoostingModel>();
        Self_BoostSettingBLL sbb = new Self_BoostSettingBLL(controlVersion);
        ObservableCollection<SelectObject> SelectObjectList = new ObservableCollection<SelectObject>();
        bool? isVisible = false;
        [ImportingConstructor]
        public CombinedMovement_BetratronBoostingSetting() : base(ControlVersion.CombinedMovementBetatron)
        {
            SelfWorkingBoosting = "CombinedMovement_BetratronBoosting";
            Unloaded += CombinedMovement_BetratronBoostingSetting_Unloaded;
            Loaded += CombinedMovement_BetratronBoostingSetting_Loaded;
            IsVisibleChanged += CombinedMovement_BetratronBoostingSetting_IsVisibleChanged;
            ButtonInvoke.SwitchLanguageEvent -= SwitchLanguage;
            ButtonInvoke.SwitchLanguageEvent += SwitchLanguage;

            ButtonInvoke.SwitchFontSizeEvent -= SwitchFontSize;
            ButtonInvoke.SwitchFontSizeEvent += SwitchFontSize;

        }

        private void CombinedMovement_BetratronBoostingSetting_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            isVisible = e.NewValue as bool?;
            if (isVisible != null && isVisible == true)
            {
                Task.Factory.StartNew(() => { InqureStatusThread(); RollStatus(); });
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
                InqureStatus();
            }
        }

        private void InitFrenceData()
        {
            SelectObjectList.Clear();
            SelectObjectList.Add(new SelectObject { SelectText = "300", SelectValue = "0" });
            SelectObjectList.Add(new SelectObject { SelectText = "150", SelectValue = "1" });
            SelectObjectList.Add(new SelectObject { SelectText = "100", SelectValue = "2" });
        }

        private void RollStatus()
        {
            while (true)
            {
                Thread.Sleep(150);
                if (isVisible == false) return;
                RollStatusIfRaying();
            }
        }

        /// <summary>
        /// 状态查询
        /// </summary>
        private void RollStatusIfRaying()
        {
            try
            {
                if (!GlobalRetStatus[12])
                {
                    BoostingControllerManager.GetInstance().StopRay();
                    //Cancel();
                    return;
                }
            }
            catch (Exception ex)
            {
                CommonDeleget.HandTaskException(ex);
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
                if (!BoostingControllerManager.GetInstance().IsConnection())
                {
                    foreach (var HitchItem in StatusModelList)
                    {
                        HitchItem.Bg_StatusCode = "0";
                    }
                }
                else
                {
                    foreach (var HitchItem in StatusModelList)
                    {
                        if (Convert.ToInt32(HitchItem.Bg_BoostingCode, 16) == BoostingControllerManager.GetInstance().GetTandI()?.H)
                        {
                            HitchItem.Bg_StatusCode = "0";
                        }
                        else
                        {
                            HitchItem.Bg_StatusCode = "2";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonDeleget.HandTaskException(ex);
            }
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitData()
        {
            StatusModelList.Clear();
            BoostingStatusBLL.GetInstance().GetBoostingStatusModelModelDataModel
            (SystemDirectoryConfig.GetInstance().GetBetatronBoosting(ControlVersion.CombinedMovementBetatron)).ToList().ForEach(q => StatusModelList.Add(q));

            BoostingModelList.Clear();
            sbb.BoostingModelList.ToList().ForEach(q => BoostingModelList.Add(q));

            InitFrenceData();
        }

        /// <summary>
        /// 初始化内容控件
        /// </summary>
        public void InitContent()
        {
            InitData();
            MainGrid = InitMainGrid();
            MainBorder.Child = MainGrid;
            Content = MainBorder;
        }
        /// <summary>
        /// 初始化主Grid
        /// </summary>
        /// <returns></returns>
        private Grid InitMainGrid()
        {
            Grid gd = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Width = 800
            };
            gd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            gd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            gd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            gd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            gd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(25, GridUnitType.Star) });
            gd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30, GridUnitType.Star) });


            ////预热停止预热
            //Grid RayGd = MakePrehot();
            //Grid.SetRow(RayGd, 0);
            //gd.Children.Add(RayGd);
            //手动出束
            Grid PrehotGd = MakeHandRay();
            Grid.SetRow(PrehotGd, 1);
            gd.Children.Add(PrehotGd);

            ///设置频率
            Grid Frence = MakeSetFrece();
            Grid.SetRow(Frence, 2);
            gd.Children.Add(Frence);

            
            foreach (var BoostringModel in BoostingModelList)
            {
                Grid tempgd=  MakeItemContent(BoostringModel);
                gd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                Grid.SetRow(tempgd, BoostingModelList.IndexOf(BoostringModel) + 3);
                gd.Children.Add(tempgd);

            }



            Border outerBd = new Border()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                BorderBrush = StrToBrush("#A5B0BE"),
                BorderThickness = new Thickness(2, 2, 1, 1),Margin = new Thickness(80,30,0,0),
                CornerRadius = new CornerRadius(2),
            };
            Grid HitchPanel = MakeHitchPanel(StatusModelList);
            outerBd.Child = HitchPanel;
            Grid.SetColumn(outerBd, 1);
            Grid.SetRow(outerBd, 0);
            Grid.SetRowSpan(outerBd, gd.RowDefinitions.Count - 1);
            gd.Children.Add(outerBd);
            return gd;
        }

        /// <summary>
        /// 手动出束
        /// </summary>
        /// <returns></returns>
        private Grid MakeHandRay()
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
            Grid.SetColumn(ClycleBd, 0);
            Label lbl = new Label()
            {
                Content = UpdateStatusNameAction("HandRay"),
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lbl.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            Border HandRay = MakeAddButton("Out Beam");
            HandRay.Name = "HandRay";

            Border CancelHandRay = MakeAddButton("Stop Beam");
            CancelHandRay.Name = "CancelHandRay";

            HandRay.MouseLeftButtonDown += PreviewHot_MouseLeftButtonDown;
            CancelHandRay.MouseLeftButtonDown += PreviewHot_MouseLeftButtonDown;
            _dp.Children.Add(HandRay);
            _dp.Children.Add(CancelHandRay);
            Grid.SetColumn(HandRay, 2);
            Grid.SetColumn(CancelHandRay, 3);
            Grid.SetColumn(lbl, 1);
            _dp.Children.Add(lbl);
            return _dp;
        }
        private void PreviewHot_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Border tsb = (sender as Border);
          
            string btnName = tsb.Name as string;
            bool Result = false;
            switch (btnName)
            {
                case "HandRay":
                    BoostingControllerManager.GetInstance().Ray();
                    //BetratronBoostingFHController.GetInstance().Ray();
                    //Ray();
                    break;
                case "CancelHandRay":
                    BoostingControllerManager.GetInstance().StopRay();
                    //BetratronBoostingFHController.GetInstance().StopRay();
                    //Cancel();
                    break;
                case "Reset":
                    BoostingControllerManager.GetInstance().Reset();
                    //BetratronBoostingFHController.GetInstance().Reset();
                    //Result = (GlobalBetatronProtocol as BetatronProtocol).Reset();
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 高能
        /// </summary>
        /// <returns></returns>
        private Grid MakeItemContent(BoostingModel bm)
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
                Content = UpdateStatusNameAction(bm.Name),
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lbl.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            TextBox tb = new TextBox()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 3, 3, 3),
                Height = 30,
                Width = 98
            };
            Border SettingButton = MakeAddButton("Setting");
            SettingButton.MouseDown += SettingButton_MouseDown; 
            SettingButton.Tag = bm;
            tb.SetBinding(TextBox.TextProperty,new Binding("Value") { Source = bm ,UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay });
            Grid.SetColumn(lbl, 1);
            _dp.Children.Add(lbl);
            _dp.Children.Add(tb);
            Grid.SetColumn(tb, 2);
            _dp.Children.Add(SettingButton);
            Grid.SetColumn(SettingButton, 3);
            return _dp;
        }

        private Grid MakeSetFrece()
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
                Content = UpdateStatusNameAction("Frequency"),
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lbl.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            ComboBox tb = new ComboBox()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 3, 3, 3),
                Height = 30,
                Width = 98,
                Style = (Style)this.FindResource("stlComboBox"),
            };
            tb.ItemsSource = SelectObjectList;
            tb.DisplayMemberPath = "SelectText";
            tb.SelectedValuePath = "SelectValue";
            Border SettingButton = MakeAddButton("Setting");
            SettingButton.Tag = tb;
            SettingButton.MouseDown += SettingFrence; 
            Grid.SetColumn(lbl, 1);
            _dp.Children.Add(lbl);
            _dp.Children.Add(tb);
            Grid.SetColumn(tb, 2);
            _dp.Children.Add(SettingButton);
            Grid.SetColumn(SettingButton, 3);
            return _dp;
        }
        /// <summary>
        /// 设置频率
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingFrence(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ComboBox SettingBoostintModel = (sender as Border).Tag as ComboBox;
            var SelectObject = SettingBoostintModel.SelectedItem as SelectObject;
            if(SelectObject!=null)
            {
                Task.Run(() => {
                    bool SendResult = BoostingControllerManager.GetInstance().SendCommandMainMagWorkFreMode(Convert.ToUInt16(SelectObject.SelectValue));
                    //BetratronBoostingFHController.GetInstance().SendCommandMainMagWorkFreMode(Convert.ToUInt16(SelectObject.SelectValue));
                    //(Common.GlobalBetatronProtocol as BetratronProtocolForFH).SendCommandMainMagWorkFreMode(Convert.ToUInt16(SelectObject.SelectValue));
                    //BoostingControllerManager.GetInstance().SendCommandMainMagWorkFreMode(Convert.ToUInt16(SelectObject.SelectValue));
                });
            
            }
        }

        private void SettingButton_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BoostingModel SettingBoostintModel = (sender as Border).Tag as BoostingModel;
            try
            {
                bool SendResult = false;
                switch (SettingBoostintModel.Name)
                {
                
                    case "HeightEnergy":
                        SendResult = BoostingControllerManager.GetInstance().SetHMC(Convert.ToUInt16(SettingBoostintModel.Value));
                        //BetratronBoostingFHController.GetInstance().SetHMC(Convert.ToUInt16(SettingBoostintModel.Value));
                        //(Common.GlobalBetatronProtocol as BetratronProtocolForFH).SendCommandHighEnergy(Convert.ToUInt16(SettingBoostintModel.Value));
                        break;
                    case "HeightEnergyMC":
                        SendResult = BoostingControllerManager.GetInstance().SetHMCNum(Convert.ToUInt16(SettingBoostintModel.Value));
                        //BetratronBoostingFHController.GetInstance().SetHMCNum(Convert.ToUInt16(SettingBoostintModel.Value));
                        //(Common.GlobalBetatronProtocol as BetratronProtocolForFH).SendCommandHighPulses(Convert.ToUInt16(SettingBoostintModel.Value));
                        break;
                    case "LowerEnergy":
                        SendResult = BoostingControllerManager.GetInstance().SetLMC(Convert.ToUInt16(SettingBoostintModel.Value));
                        //BetratronBoostingFHController.GetInstance().SetLMC(Convert.ToUInt16(SettingBoostintModel.Value));
                        //(Common.GlobalBetatronProtocol as BetratronProtocolForFH).SendCommandLowEnergy(Convert.ToUInt16(SettingBoostintModel.Value));
                        break;
                    case "LowerEnergyMC":
                        SendResult = BoostingControllerManager.GetInstance().SetLMCNum(Convert.ToUInt16(SettingBoostintModel.Value));
                        //BetratronBoostingFHController.GetInstance().SetLMCNum(Convert.ToUInt16(SettingBoostintModel.Value));
                        //(Common.GlobalBetatronProtocol as BetratronProtocolForFH).SendCommandLowPulsees(Convert.ToUInt16(SettingBoostintModel.Value));
                        break;
                    case "InjectionCurrent":
                        SendResult = BoostingControllerManager.GetInstance().ExecuteInjection(Convert.ToUInt16(Convert.ToUInt16(SettingBoostintModel.Value) * 12));
                        //BetratronBoostingFHController.GetInstance().ExecuteInjection(Convert.ToUInt16(Convert.ToUInt16(SettingBoostintModel.Value) * 12));
                        //(Common.GlobalBetatronProtocol as BetratronProtocolForFH).SendCommandInjectionCurrent(Convert.ToUInt16(Convert.ToUInt16(SettingBoostintModel.Value) * 12));
                        break;
                    case "ExposureTime":
                        SendResult = BoostingControllerManager.GetInstance().SendCommandExposureTime(Convert.ToUInt16(SettingBoostintModel.Value));
                        //BetratronBoostingFHController.GetInstance().SendCommandExposureTime(Convert.ToUInt16(SettingBoostintModel.Value));
                        //(Common.GlobalBetatronProtocol as BetratronProtocolForFH).SendCommandExposureTime(Convert.ToUInt16(SettingBoostintModel.Value));
                        break;
                    case "InnerDose":
                        SendResult = BoostingControllerManager.GetInstance().SendCommandDoseInternal(Convert.ToUInt16(SettingBoostintModel.Value));
                        //BetratronBoostingFHController.GetInstance().SendCommandDoseInternal(Convert.ToUInt16(SettingBoostintModel.Value));
                        //(Common.GlobalBetatronProtocol as BetratronProtocolForFH).SendCommandDoseInternal(Convert.ToUInt16(SettingBoostintModel.Value));
                        break;
                    default:
                        break;
                }
                sbb.SaveConfigDataModel(BoostingModelList,cv);
            }
            catch (Exception ex)
            {
                CommonDeleget.WriteLogAction(ex.Message, LogType.ApplicationError, true);
            }
        }

        private Grid MakeHitchPanel(ObservableCollection<BoostingStatusModel> HitchModelList)
        {
            Grid _dp = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch,VerticalAlignment = VerticalAlignment.Stretch };
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
        private int CheckHitchStatus(ObservableCollection<BoostingStatusModel> HitchModelList, Grid _dp, int Index)
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
                if (!string.IsNullOrEmpty(HitchItem.Bg_BoostingName))
                {
                    HitchItem.Bg_BoostingName = UpdateStatusNameAction(HitchItem.Bg_BoostingName);
                }
                lblHitch.SetBinding(Label.ContentProperty, new Binding("Bg_BoostingName") { Source = HitchItem, Mode = BindingMode.TwoWay });
                lblHitch.SetBinding(Label.ForegroundProperty, new Binding("Bg_StatusCode") { Source = HitchItem, Mode = BindingMode.TwoWay, Converter = new HitchColorConvert() });

                Grid.SetColumn(lblHitch, Index / 8);
                Grid.SetRow(lblHitch, Index % 8);

                _dp.Children.Add(lblHitch);
                Index++;
            }

            return Index;
        }

        public void SwitchFontSize(string FontSize)
        {
            CombinedMovement_BetratronBoostingSetting_Loaded(null, null);
        }
        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="language"></param>
        public void SwitchLanguage(string language)
        {
            CombinedMovement_BetratronBoostingSetting_Loaded(this, null);
        }



        private void CombinedMovement_BetratronBoostingSetting_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            InitContent();
        }

        private void CombinedMovement_BetratronBoostingSetting_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    
    }
}
