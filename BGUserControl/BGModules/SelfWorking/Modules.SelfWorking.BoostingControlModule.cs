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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonFunc;
using static CMW.Common.Utilities.CommonDeleget;

using BG_Services;
using BG_WorkFlow;
using BG_Entities;
using static BG_WorkFlow.HitChModelBLL;

namespace BGUserControl
{
    //[Export("ContentPage", typeof(IConditionView))]
    //[CustomExportMetadata(1, "SelfWorking_BoostingControlModule", "自行走加速器模块", "ZZW", "1.0.0")]
    public class SelfWorking_BoostingControlModule : BaseModules
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
        bool? isVisible = false;
        [ImportingConstructor]
        public SelfWorking_BoostingControlModule() : base(ControlVersion.SelfWorking)
        {
            SelfWorkingBoosting = "SelfWorkingBoosting";
            Unloaded += SelfWorking_BoostingControlModule_Unloaded;
            Loaded += SelfWorking_BoostingControlModule_Loaded;
            IsVisibleChanged += SelfWorking_BoostingControlModule_IsVisibleChanged;
            ButtonInvoke.SwitchLanguageEvent -= SwitchLanguage;
            ButtonInvoke.SwitchLanguageEvent += SwitchLanguage;

            ButtonInvoke.SwitchFontSizeEvent -= SwitchFontSize;
            ButtonInvoke.SwitchFontSizeEvent += SwitchFontSize;

        }

        private void SelfWorking_BoostingControlModule_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            isVisible = e.NewValue as bool?;
            if (isVisible != null && isVisible == true)
            {
                Task.Factory.StartNew(() => { InqureStatusThread(); });
            }
            else
            {
                sbb.SaveConfigDataModel(BoostingModelList);
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
        /// <summary>
        /// 状态查询
        /// </summary>
        private void InqureStatus()
        {

            try
            {
                //Common.Inquire();IsConnect
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
                        if (Convert.ToInt32(HitchItem.Bg_BoostingCode, 16) == BoostingControllerManager.GetInstance().GetTandI().H)
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
            (SystemDirectoryConfig.GetInstance().GetBetatronBoosting(ControlVersion.FastCheck)).ToList().ForEach(q => StatusModelList.Add(q));

            BoostingModelList.Clear();
            sbb.GetBoostingModelList(Common.controlVersion).ForEach(q => BoostingModelList.Add(q));
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


            //预热停止预热
            Grid RayGd = MakePrehot();
            Grid.SetRow(RayGd, 0);
            gd.Children.Add(RayGd);

            if (SearchScanMode() == "InitiativeMode")
            {
                //手动出束
                Grid PrehotGd = MakeHandRay();
                Grid.SetRow(PrehotGd, 1);
                gd.Children.Add(PrehotGd);
             
            }

            
            //复位
            Grid Reset = MakeBoostingReset();
            Grid.SetRow(Reset, 2);
            gd.Children.Add(Reset);
            



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
            Grid.SetColumn(lbl, 1);
            _dp.Children.Add(lbl);
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
                Content = UpdateStatusNameAction("PreheatStatus"),
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            Label lblRay = new Label()
            {
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            Border PreviewHot =  MakeAddButton("PreHot");
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
            if (BoostingControllerManager.GetInstance().GetWorkingJob() == WorkingJob.WJ_KeyNotOpen)
            {
                BG_MESSAGEBOX.Show(Application.Current?.MainWindow, "Tip", "加速器钥匙未打开！");
                return;
            }
            string btnName = tsb.Name as string;
            bool Result = false;
            switch (btnName)
            {
                case "HandRay":
                    HandRay(sender, true);
                    break;
                case "CancelHandRay":
                    HandRay(sender, false);
                    break;
                case "PreviewHot":
                    BoostingControllerManager.GetInstance().Ray();
                    //BetratronBoostingController.GetInstance().Ray();
                    break;
                case "StopPreviewHot":
                    BoostingControllerManager.GetInstance().StopRay();
                    //BetratronBoostingController.GetInstance().StopRay();// StopPreHot();
                    break;
                case "Reset":
                    BoostingControllerManager.GetInstance().Reset();
                    //BetratronBoostingController.GetInstance().Reset();
                    //Result = (GlobalBetatronProtocol as BetatronProtocol).Reset();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Reset加速器
        /// </summary>
        /// <returns></returns>
        private Grid MakeBoostingReset()
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
                Content = UpdateStatusNameAction("Reset"),
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            Border RayBtn = MakeSettingBorder();
            RayBtn.HorizontalAlignment = HorizontalAlignment.Center;
            RayBtn.MouseLeftButtonDown -= RayBtn_MouseLeftButtonDown;
            RayBtn.MouseLeftButtonDown += RayBtn_MouseLeftButtonDown;
            Grid.SetColumn(lbl, 1);
            _dp.Children.Add(lbl);
            _dp.Children.Add(RayBtn);
            Grid.SetColumn(RayBtn, 2);
            return _dp;
        }
        /// <summary>
        /// 复位按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RayBtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                Task.Factory.StartNew(() => {
                    BoostingControllerManager.GetInstance().Reset();
                    //(GlobalBetatronProtocol as BetatronProtocol).Reset();
                });
            }
            catch (Exception ex)
            {
                CommonDeleget.WriteLogAction(ex.Message, LogType.ApplicationError, true);
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
            tb.SetBinding(TextBox.TextProperty,new Binding("Value") { Source = bm ,UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay });
            Grid.SetColumn(lbl, 1);
            _dp.Children.Add(lbl);
            _dp.Children.Add(tb);
            Grid.SetColumn(tb, 2);
            return _dp;
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
                    FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.Normal),
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

        
        public void SwitchFontSize(string language)
        {
            SelfWorking_BoostingControlModule_Loaded(this, null);
        }
        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="language"></param>
        public void SwitchLanguage(string language)
        {
            SelfWorking_BoostingControlModule_Loaded(this, null);
        }


        private void SelfWorking_BoostingControlModule_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            InitContent();
        }

        private void SelfWorking_BoostingControlModule_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }
        /// <summary>
        /// 手动出束
        /// </summary>
        /// <param name="isOpen"></param>
        private void HandRay(object sender,bool isOpen)
        {
            if (isOpen)
            {
                SetCommand(CommandDic[Command.StopRay],true);
                //(GlobalBetatronProtocol as BetatronProtocol).OutputBeam();
            }
            else
            {
                if (!Common.GlobalRetStatus[18])
                {
                    BG_MESSAGEBOX.Show(Application.Current?.MainWindow, UpdateStatusNameAction("Tip"), $"加速器状态未预热完成！");
                    return;
                }
                SetCommand(CommandDic[Command.StopRay], false);
            }
        }
    }
}
