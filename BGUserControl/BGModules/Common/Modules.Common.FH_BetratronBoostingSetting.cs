using BG_Entities;
using BG_Services;
using BG_WorkFlow;
using BGModel;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using static BG_WorkFlow.HitChModelBLL;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.CommonFunc;

namespace BGUserControl
{
    [Export("ContentPage", typeof(BaseModules))]
    [CustomExportMetadata(1, Modules.FH_BetratronBoostingSetting, "FHBetratron加速器设置模块", "ZZW", "1.0.0")]
    public class FH_BetratronBoostingSetting : BaseModules
    {
        string SelfWorkingBoosting;
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
        FH_AcceleatorMvvm AcceleatorMvvm = new FH_AcceleatorMvvm();
        
        [ImportingConstructor]
        public FH_BetratronBoostingSetting() : base(ControlVersion.CombinedMovementBetatron)
        {
            SelfWorkingBoosting = "FH_BetratronBoostingSetting";
            Loaded += FH_BetratronBoostingSetting_Loaded;
            AcceleatorMvvm.SwitchFontsizeAction += SwitchFontSize;
            AcceleatorMvvm.SwitchLanguageAction += SwitchLanguage;
            DataContext = AcceleatorMvvm;
        }

    
        /// <summary>
        /// 初始化内容控件
        /// </summary>
        public void InitContent()
        {
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
            gd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            gd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(25, GridUnitType.Star) });
            gd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30, GridUnitType.Star) });

            //预热停止预热
            Grid RayGd = MakePrehot();
            Grid.SetRow(RayGd, 0);
            gd.Children.Add(RayGd);

            //手动出束
            Grid PrehotGd = MakeHandRay();
            Grid.SetRow(PrehotGd, 1);
            gd.Children.Add(PrehotGd);

            ///设置频率
            Grid Frence = MakeSetFrece();
            Grid.SetRow(Frence, 2);
            gd.Children.Add(Frence);

            //设置高低能
            Grid doubleOrsingle = MakeDoubleOrSingle();
            Grid.SetRow(doubleOrsingle,3);
            gd.Children.Add(doubleOrsingle);

            //设置模拟/默认模式
            Grid simulationOrDefault = MakeSimulationOrDefault();
            Grid.SetRow(simulationOrDefault, 4);
            gd.Children.Add(simulationOrDefault);

            foreach (var BoostringModel in AcceleatorMvvm.BoostingModelList)
            {
                Grid tempgd=  MakeItemContent(BoostringModel);
                gd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                Grid.SetRow(tempgd, AcceleatorMvvm.BoostingModelList.IndexOf(BoostringModel) + 4);
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
            Grid HitchPanel = MakeHitchPanel(AcceleatorMvvm.StatusModelList);
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

        /// <summary>
        /// 预热/非预热
        /// </summary>
        /// <returns></returns>
        private Grid MakePrehot()
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
                Content = UpdateStatusNameAction("Preheat-Status"),
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            Label lblRay = new Label()
            {
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            Border PreviewHot = MakeAddButton("PreHot");
            PreviewHot.Name = "PreviewHot";

            Border StopPreviewHot = MakeAddButton("StopPreHot");
            StopPreviewHot.Name = "StopPreviewHot";

            PreviewHot.MouseLeftButtonDown += PreviewHot_MouseLeftButtonDown;
            StopPreviewHot.MouseLeftButtonDown += PreviewHot_MouseLeftButtonDown;
            Grid.SetColumn(lbl, 1);
            _dp.Children.Add(lbl);
            _dp.Children.Add(PreviewHot);
            _dp.Children.Add(StopPreviewHot);
            Grid.SetColumn(PreviewHot, 2);
            Grid.SetColumn(StopPreviewHot, 3);
            return _dp;
        }


        private void PreviewHot_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Border tsb = (sender as Border);
          
            string btnName = tsb.Name as string;
            switch (btnName)
            {
                case "HandRay":
                    BoostingControllerManager.GetInstance().Ray();
                    break;
                case "PreviewHot":
                    Debug.WriteLine(CommandDic[Command.BoostReay] + "------------------------------------------------------------------gang点击预热按钮----------------------------------------------------------");
                    BoostingControllerManager.GetInstance().SendCommandWaitWorkStop();
                    break;
                case "StopPreviewHot":
                    BoostingControllerManager.GetInstance().StopRay();
                    break;
                case "CancelHandRay":
                    BoostingControllerManager.GetInstance().StopRay();
                    break;
                case "Reset":
                    BoostingControllerManager.GetInstance().Reset();
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
            tb.ItemsSource = AcceleatorMvvm.InitFrenceData();
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
        /// 设置单双能
        /// </summary>
        /// <returns></returns>
        private Grid MakeDoubleOrSingle()
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
                Content = UpdateStatusNameAction("EnergyMode"),
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
            tb.ItemsSource = AcceleatorMvvm.InitEnergerModeData();
            tb.DisplayMemberPath = "SelectDisplayText";
            tb.SelectedValuePath = "SelectValue";
            Border SettingButton = MakeAddButton("Setting");
            SettingButton.Tag = tb;
            SettingButton.MouseDown += SettingDoubleOrSingle;
            Grid.SetColumn(lbl, 1);
            _dp.Children.Add(lbl);
            _dp.Children.Add(tb);
            Grid.SetColumn(tb, 2);
            _dp.Children.Add(SettingButton);
            Grid.SetColumn(SettingButton, 3);
            return _dp;
        }

        private Grid MakeSimulationOrDefault()
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
                //Content = UpdateStatusNameAction("EnergyMode"),
                Content = UpdateStatusNameAction("ScanMode"),
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
            tb.ItemsSource = AcceleatorMvvm.InitModeData();
            tb.DisplayMemberPath = "SelectDisplayText";
            tb.SelectedValuePath = "SelectValue";
            Border SettingButton = MakeAddButton("Setting");
            SettingButton.Tag = tb;
            SettingButton.MouseDown += SettingSimulationOrDefault;
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
                });
            
            }
        }

        /// <summary>
        /// 设置单双能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingDoubleOrSingle (object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DetecotrControllerManager.GetInstance().DetectorConnection != 3)
            {
                CommonDeleget.MessageBoxActionAction("由于探测器未完成初始化，请探测器连接状态灯稳定绿灯后在设置。");
                return;
            }
            ComboBox SettingBoostintModel = (sender as Border).Tag as ComboBox;
            var SelectObject = SettingBoostintModel.SelectedItem as SelectObject;
            if (SelectObject != null)
            {
                Task.Run(() => {
                    bool value = Convert.ToUInt16(SelectObject.SelectValue) == 0 ?true:false;
                    BoostingControllerManager.GetInstance().SwitchEngerHAndI(value, false,true);
                    //bool SendResult = BoostingControllerManager.GetInstance().SendCommandEnergyMode(Convert.ToUInt16(SelectObject.SelectValue));
                    //Thread.Sleep(100);
                    //DetecotrControllerManager.GetInstance().SX_SetEnergy(Convert.ToInt32(SelectObject.SelectValue));
                });
            }
        }

        private void SettingSimulationOrDefault(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            /*if (DetecotrControllerManager.GetInstance().DetectorConnection != 3)
            {
                CommonDeleget.MessageBoxActionAction("由于探测器未完成初始化，请探测器连接状态灯稳定绿灯后在设置。");
                return;
            }*/
            ComboBox SettingBoostintModel = (sender as Border).Tag as ComboBox;
            var SelectObject = SettingBoostintModel.SelectedItem as SelectObject; // 1 为默认 0为模拟(测试模式)

            if (SelectObject != null)
            {
                Task.Run(() => {
                    if (Convert.ToUInt16(SelectObject.SelectValue) == 1)
                    {//测试模式
                        Common.SetCommand(CommandDic[Command.TestMode], true);
                    }
                    else {
                        Common.SetCommand(CommandDic[Command.TestMode], false);
                    }
                    // BoostingControllerManager.GetInstance().SwitchEngerHAndI(value, false, true);
                    // bool SendResult = BoostingControllerManager.GetInstance().SendCommandEnergyMode(Convert.ToUInt16(SelectObject.SelectValue));
                    // Thread.Sleep(100);
                    // DetecotrControllerManager.GetInstance().SX_SetEnergy(Convert.ToInt32(SelectObject.SelectValue));
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
                        break;
                    case "HeightEnergyMC":
                        SendResult = BoostingControllerManager.GetInstance().SetHMCNum(Convert.ToUInt16(SettingBoostintModel.Value));
                        break;
                    case "LowerEnergy":
                        SendResult = BoostingControllerManager.GetInstance().SetLMC(Convert.ToUInt16(SettingBoostintModel.Value));
                        break;
                    case "LowerEnergyMC":
                        SendResult = BoostingControllerManager.GetInstance().SetLMCNum(Convert.ToUInt16(SettingBoostintModel.Value));
                        break;
                    case "InjectionCurrent":
                        SendResult = BoostingControllerManager.GetInstance().ExecuteInjection(Convert.ToUInt16(Convert.ToUInt16(SettingBoostintModel.Value) * 12));
                        break;
                    case "ExposureTime":
                        SendResult = BoostingControllerManager.GetInstance().SendCommandExposureTime(Convert.ToUInt16(SettingBoostintModel.Value));
                        break;
                    case "InnerDose":
                        SendResult = BoostingControllerManager.GetInstance().SendCommandDoseInternal(Convert.ToUInt16(SettingBoostintModel.Value));
                        break;
                    default:
                        break;
                }
                AcceleatorMvvm.SaveCommonSettingModelList();
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
                    HitchItem.BoostingDisplayName = UpdateStatusNameAction(HitchItem.Bg_BoostingName);
                }
                lblHitch.SetBinding(Label.ContentProperty, new Binding("BoostingDisplayName") { Source = HitchItem, Mode = BindingMode.TwoWay });
                lblHitch.SetBinding(Label.ForegroundProperty, new Binding("Bg_StatusCode") { Source = HitchItem, Mode = BindingMode.TwoWay, Converter = new HitchColorConvert() });

                Grid.SetColumn(lblHitch, Index / 8);
                Grid.SetRow(lblHitch, Index % 8);

                _dp.Children.Add(lblHitch);
                Index++;
            }

            return Index;
        }
        public void SwitchFontSize()
        {
            FH_BetratronBoostingSetting_Loaded(null, null);
        }
        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="language"></param>
        public void SwitchLanguage()
        {
            FH_BetratronBoostingSetting_Loaded(this, null);
        }
        private void FH_BetratronBoostingSetting_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            InitContent();
        }
        /// <summary>
        /// 获取名称
        /// </summary>
        /// <returns></returns>
        public override string GetName()
        {
            return "BoostingControl";
        }
    }
    public class FH_AcceleatorMvvm : BaseMvvm
    {
        public ObservableCollection<BoostingStatusModel> StatusModelList = new ObservableCollection<BoostingStatusModel>();
        public ObservableCollection<BoostingModel> BoostingModelList = new ObservableCollection<BoostingModel>();
        Self_BoostSettingBLL sbb = new Self_BoostSettingBLL(controlVersion);
        public Visibsliy _visibleObject = new Visibsliy() { DisplayName = "未出束" };
        public FH_AcceleatorMvvm()
        {
            InitData();
        }
        private void InitData()
        {
            StatusModelList.Clear();
            BoostingStatusBLL.GetInstance().GetBoostingStatusModelModelDataModel
            (SystemDirectoryConfig.GetInstance().GetBetatronBoosting(ControlVersion.CombinedMovementBetatron)).ToList().ForEach(q => StatusModelList.Add(q));

            BoostingModelList.Clear();
            sbb.BoostingModelList.ToList().ForEach(q => BoostingModelList.Add(q));
        }
        public ObservableCollection<SelectObject> InitFrenceData()
        {
            ObservableCollection<SelectObject> SelectObjectList = new ObservableCollection<SelectObject>();
            SelectObjectList.Add(new SelectObject { SelectText = "300", SelectValue = "0" });
            SelectObjectList.Add(new SelectObject { SelectText = "150", SelectValue = "1" });
            SelectObjectList.Add(new SelectObject { SelectText = "100", SelectValue = "2" });
            return SelectObjectList;
        }

        public ObservableCollection<SelectObject> InitEnergerModeData()
        {
            ObservableCollection<SelectObject> SelectObjectList = new ObservableCollection<SelectObject>();
            SelectObjectList.Add(new SelectObject { SelectText = "DoubleEnergy", SelectValue = "1" });
            SelectObjectList.Add(new SelectObject { SelectText = "SingleEnergyIndication", SelectValue = "0" });
            return SelectObjectList;
        }

        public ObservableCollection<SelectObject> InitModeData()
        {
            ObservableCollection<SelectObject> SelectObjectList = new ObservableCollection<SelectObject>();
            SelectObjectList.Add(new SelectObject { SelectText = "InitiativeMode", SelectValue = "0" });
            SelectObjectList.Add(new SelectObject { SelectText = "TestMode", SelectValue = "1" });
            return SelectObjectList;
        }

        public override void LoadUIText()
        {

        }
        protected override void InquirePlcStatus(List<bool> StatusList)
        {
          
        }
        protected override void ConnectionStatus(bool ConnectionStatus)
        {

        }
        protected override void AccelatorConnectionStatus(bool ConnectionStatus)
        {
            try
            {
                if (!ConnectionStatus)
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
                RollStatusIfRaying();
            }
            catch (Exception ex)
            {
                CommonDeleget.HandTaskException(ex);
            }
        }
        public void SaveCommonSettingModelList()
        {
            sbb.SaveConfigDataModel(BoostingModelList, Common.controlVersion);
        }
        public void DetecotorConnection(bool DetecotrConnection)
        {

        }
        private void RollStatusIfRaying()
        {
            try
            {
                if (!PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.SafetyInterlockReady))
                {
                    BoostingControllerManager.GetInstance().StopRay();
                    return;
                }
            }
            catch (Exception ex)
            {
                CommonDeleget.HandTaskException(ex);
            }
        }
    }
}
