using BG_Entities;
using BG_Services;
using BG_WorkFlow;
using BGModel;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using static BG_WorkFlow.HitChModelBLL;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.CommonFunc;
using Application = System.Windows.Application;
using Binding = System.Windows.Data.Binding;
using GroupBox = System.Windows.Controls.GroupBox;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Label = System.Windows.Controls.Label;
using Orientation = System.Windows.Controls.Orientation;
using ProgressBar = System.Windows.Controls.ProgressBar;
using TextBox = System.Windows.Controls.TextBox;
using UserControl = System.Windows.Controls.UserControl;

namespace BGUserControl
{
        [Export("ContentPage", typeof(BaseModules))]
        [Export("BS_ContentPage", typeof(BaseModules))]
        [CustomExportMetadata(1, Modules.VJ_BetratronBoostingSetting, "VJ射线源", "ZZW", "1.0.0")]
        public class VJ_BetratronBoostingSetting : BSBaseModules
        {
            string SelfWorkingBoosting;
            Border MainBorder = new Border()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(0, 0, 0, 20)
            };
            Grid MainGrid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            VJ_AcceleatorMvvm AcceleatorMvvm = new VJ_AcceleatorMvvm();
            // 添加Dispatcher用于线程切换
            private Dispatcher UIDispatcher => Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;

            [ImportingConstructor]
            public VJ_BetratronBoostingSetting() : base(ControlVersion.BS)
            {
                SelfWorkingBoosting = "VJ_BetratronBoostingSetting";
                Loaded += FH_BetratronBoostingSetting_Loaded;
                AcceleatorMvvm.SwitchFontsizeAction += SwitchFontSize;
                AcceleatorMvvm.SwitchLanguageAction += SwitchLanguage;
                DataContext = AcceleatorMvvm;
            }

            /// <summary>
            /// 初始化内容控件（改为异步）
            /// </summary>
            public async Task InitContentAsync()
            {
                MainGrid = await InitMainGridAsync();
                MainBorder.Child = MainGrid;
                Content = MainBorder;
            }

            /// <summary>
            /// 初始化主Grid（改为异步）
            /// </summary>
            private async Task<Grid> InitMainGridAsync()
            {
                Grid gd = new Grid()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                };
                gd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30, GridUnitType.Star) });
                gd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30, GridUnitType.Star) });
                gd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30, GridUnitType.Star) });

                // 先加载基础控件（快速显示）
                InitOneKreWarmUpBtn(gd);
                InitOneKeyElectricalBtn(gd);
                InitAutoOrHand(gd);

                // 异步加载耗时的射线源面板
                await InitContentRaySourcePanelAsync(gd);

                return gd;
            }

            private void InitOneKreWarmUpBtn(Grid ParentGrid)
            {
                ParentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });
                Border OneKeyWarmUpBtn = new Border()
                {
                    Width = 100,
                    Height = 40,
                    Style = (Style)this.FindResource("diyBtnCarHand"),
                    Margin = new Thickness(5)
                };
                OneKeyWarmUpBtn.MouseLeftButtonDown += OneKeyWarmUpBtn_MouseLeftButtonDown;
                Label btnInner = new Label()
                {
                    Content = UpdateStatusNameAction("OneKeyWarmup"),
                    Style = (Style)this.FindResource("diyLabel"),
                    Foreground = StrToBrush("#FFFFFF")
                };
                OneKeyWarmUpBtn.Child = btnInner;
                ParentGrid.Children.Add(OneKeyWarmUpBtn);
                Grid.SetRow(OneKeyWarmUpBtn, 0);
                Grid.SetColumn(OneKeyWarmUpBtn, 0);
            }

            private void InitOneKeyElectricalBtn(Grid ParentGrid)
            {
                Border OneKeyWarmUpBtn = new Border()
                {
                    Width = 100,
                    Height = 40,
                    Style = (Style)this.FindResource("diyBtnCarHand"),
                    Margin = new Thickness(5)
                };
                OneKeyWarmUpBtn.MouseLeftButtonDown += OneKeyElectricalBtn_MouseLeftButtonDown;
                Label btnInner = new Label()
                {
                    Content = UpdateStatusNameAction("OneKeyElectrical"),
                    Style = (Style)this.FindResource("diyLabel"),
                    Foreground = StrToBrush("#FFFFFF")
                };
                OneKeyWarmUpBtn.Child = btnInner;
                ParentGrid.Children.Add(OneKeyWarmUpBtn);
                Grid.SetRow(OneKeyWarmUpBtn, 0);
                Grid.SetColumn(OneKeyWarmUpBtn, 1);
            }

            /// <summary>
            /// 设置手自动
            /// </summary>
            private void InitAutoOrHand(Grid ParentGrid)
            {
                StackPanel stackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
                Label btnInner = new Label()
                {
                    Content = UpdateStatusNameAction("ElecMode"),
                    Style = (Style)this.FindResource("diyLabel"),
                    Foreground = StrToBrush("#017CD1")
                };
                DiyRadioButton OneKeyAutoBtn = new DiyRadioButton()
                {
                    Width = 220,
                    Height = 40,
                    Margin = new Thickness(5)
                };
                CommonSettingModel _csm = new CommonSettingModel()
                {
                    CommonSettingIndex = "0",
                    CommonSettingName = "AutoMode",
                    CommonSettingDisplayName = "手自动模式",
                    CommonSettingPLCValue = "DB55.1.0",
                    CommonSettingValue = "0",
                    CommonSettingValueType = "BOOL",
                    CommonSettingType = "Button",
                    CommonSettingDataSource = "DB55.1.0?0:MT;DB55.1.0?1:AT",
                    CommonSettingSendType = "SingleButton",
                };
                OneKeyAutoBtn.MouseButonEnent += RadioButtonMouseDown;
                OneKeyAutoBtn.SetBinding(DiyRadioButton.ItemSourceProperty, new Binding(".")
                {
                    Source = _csm,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.TwoWay,
                    ValidatesOnExceptions = true,
                    NotifyOnValidationError = true,
                    ValidatesOnDataErrors = true,
                });
                stackPanel.Children.Add(btnInner);
                stackPanel.Children.Add(OneKeyAutoBtn);
                ParentGrid.Children.Add(stackPanel);
                Grid.SetRow(stackPanel, 0);
                Grid.SetColumn(stackPanel, 2);
            }

            private void OneKeyWarmUpBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                Task.Run(() => {
                    Parallel.Invoke(() =>
                    {
                        PlcManager.WritePositionValue(MouduleCommandDic[Command.TopCabinWarmUp], true);
                        Thread.Sleep(500);
                        PlcManager.WritePositionValue(MouduleCommandDic[Command.TopCabinWarmUp], false);
                    }, () =>
                    {
                        PlcManager.WritePositionValue(MouduleCommandDic[Command.InLetCabinWarmUp], true);
                        Thread.Sleep(500);
                        PlcManager.WritePositionValue(MouduleCommandDic[Command.InLetCabinWarmUp], false);
                    }, () =>
                    {
                        PlcManager.WritePositionValue(MouduleCommandDic[Command.SideCabinWarmUp], true);
                        Thread.Sleep(500);
                        PlcManager.WritePositionValue(MouduleCommandDic[Command.SideCabinWarmUp], false);
                    });
                });
            }

            private void OneKeyElectricalBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                Task.Run(() => {
                    Parallel.Invoke(() =>
                    {
                        PlcManager.WritePositionValue(MouduleCommandDic[Command.TopCabinFlyingSaucer], true);
                    }, () =>
                    {
                        PlcManager.WritePositionValue(MouduleCommandDic[Command.InLetFlyingSaucer], true);
                    }, () =>
                    {
                        PlcManager.WritePositionValue(MouduleCommandDic[Command.SideCabinFlyingSaucer], true);
                    });
                });
            }

            /// <summary>
            /// 异步初始化射线源面板（核心优化）
            /// </summary>
            private async Task InitContentRaySourcePanelAsync(Grid ParentGrid)
            {
                int rowIndex = 1;
                var raySourceItems = AcceleatorMvvm.VJ_RaySourceDicList.ToList();
                const int batchSize = 2; // 每批加载数量，可根据性能调整

                // 预处理数据（在后台线程）preparedItems = [主舱射线装置，顶舱射线装置，侧舱射线装置]
                var preparedItems = await Task.Run(() =>
                    raySourceItems.Select(item => new
                    {
                        Item = item,
                        HeaderContent = UpdateStatusNameAction($"View{item.Key}")
                    }).ToList()
                );

                // 分批创建UI元素
                for (int i = 0; i < preparedItems.Count; i += batchSize)
                {
                    var batch = preparedItems.Skip(i).Take(batchSize);

                    // 在UI线程执行UI操作
                    await UIDispatcher.InvokeAsync(() =>
                    {
                        foreach (var item in batch)
                        {
                            ParentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                            GroupBox childGroupBox = MakeVJ_SingleViewConentControl(item.Item.Value, rowIndex);
                            childGroupBox.BorderBrush = StrToBrush("#A5B0BE");
                            childGroupBox.BorderThickness = new Thickness(1);
                            childGroupBox.Header = CreateGroupBoxHeader(item.HeaderContent);

                            Grid.SetColumn(childGroupBox, 0);
                            Grid.SetRow(childGroupBox, rowIndex);
                            Grid.SetColumnSpan(childGroupBox, 3);
                            ParentGrid.Children.Add(childGroupBox);
                            rowIndex++;
                        }
                    }, DispatcherPriority.Background);

                    // 每批加载后给UI线程喘息时间
                    await Task.Delay(10);
                }
            }

            // 提取Header创建方法
            private Label CreateGroupBoxHeader(string content) // 主舱射线装置、顶舱射线装置、侧舱射线装置
        {
                return new Label()
                {
                    Content = content,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Style = (Style)this.FindResource("diyLabel"),
                    Foreground = StrToBrush("#1A4F85")
                };
            }

            // 修改为异步创建子内容控件
            private GroupBox MakeVJ_SingleViewConentControl(ObservableCollection<CommonSettingModel> ViewSources, int Index)
            {
                GroupBox groupBox = new GroupBox()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                };
                Grid ViewControlPanel = new Grid()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                };
                ViewControlPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                ViewControlPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.5, GridUnitType.Star) });
                ViewControlPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.5, GridUnitType.Star) });

                // 异步加载子面板，不阻塞当前线程
                _ = LoadChildControlsAsync(ViewControlPanel, ViewSources, Index);

                groupBox.Content = ViewControlPanel;
                return groupBox;
            }

        // 异步加载子控件
        // 异步加载子控件
        private async Task LoadChildControlsAsync(Grid viewControlPanel, ObservableCollection<CommonSettingModel> viewSources, int index)
        {
            foreach (CommonSettingModel commonSetting in viewSources)
            {
                // 关键修改：在UI线程创建子面板（因为包含UI元素）
                var childPanel = await UIDispatcher.InvokeAsync(() =>
                    MakeChildPanel(commonSetting, index),
                    DispatcherPriority.Normal
                );

                // UI线程添加子面板
                await UIDispatcher.InvokeAsync(() =>
                {
                    viewControlPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    int rowIndex = viewSources.IndexOf(commonSetting);
                    Grid.SetRow(childPanel, rowIndex);
                    Grid.SetColumn(childPanel, 0);
                    viewControlPanel.Children.Add(childPanel);
                });

                // 处理故障网格（同样确保UI元素在UI线程创建）
                if (commonSetting.CommonSettingType == "WarmUpButton")
                {
                    var falutGrid = await UIDispatcher.InvokeAsync(() =>
                        MakeFalutGrid(commonSetting, index),
                        DispatcherPriority.Normal
                    );
                    if (falutGrid != null)
                    {
                        await UIDispatcher.InvokeAsync(() =>
                        {
                            Grid.SetRow(falutGrid, 0);
                            Grid.SetRowSpan(falutGrid, viewSources.Count);
                            Grid.SetColumn(falutGrid, 1);
                            viewControlPanel.Children.Add(falutGrid);
                        });
                    }
                }
            }

            // 加载监控控件的代码保持不变（已正确使用UI线程）
            if (AcceleatorMvvm.VJ_RaySourceDoseValueFileList.ContainsKey(index - 1))
            {
                string doseFile = await Task.Run(() => AcceleatorMvvm.VJ_RaySourceDoseValueFileList[index - 1]);
                await UIDispatcher.InvokeAsync(() =>
                {
                    VJ_RaySourceMonitorControl monitorControl = new VJ_RaySourceMonitorControl()
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        HorizontalContentAlignment = HorizontalAlignment.Stretch
                    };
                    monitorControl.ItemSource = doseFile;
                    viewControlPanel.Children.Add(monitorControl);
                    Grid.SetRow(monitorControl, 0);
                    Grid.SetRowSpan(monitorControl, viewControlPanel.RowDefinitions.Count);
                    Grid.SetColumn(monitorControl, 2);
                });
            }
        }

        private Grid MakeChildPanel(CommonSettingModel _csm, int Index)
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

                if (_csm.CommonSettingType == "TextBox")
                {
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
                        BtnSendCommand.DataContext = _csm;
                    }
                    MakeTextBox(_csm, _dp, btnInner);
                }
                else if (_csm.CommonSettingType == "GroupButton")
                {
                    MakeDiyRadioButton(_csm, _dp, Index);
                }
                else if (_csm.CommonSettingType == "GroupButtonAdjustAngle")
                {
                    MakeDiyAdjustAngleButton(_csm, _dp, Index);
                }
                else if (_csm.CommonSettingType == "GroupButtonStartingDynamo")
                {
                    MakeDiyRadioButtonByStartingDynamo(_csm, _dp, Index);
                }
                else if (_csm.CommonSettingType == "WarmUpButton")
                {
                    MakeDiyPreviewButton(_csm, _dp, Index);
                }

                _dp.Children.Add(lblLeft);
                Grid.SetColumn(lblLeft, 1);
                return _dp;
            }

            private void MakeTextBox1(CommonSettingModel _csm, Grid _dp, Label btnInner)
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
                    Source = _csm,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.TwoWay,
                    ValidatesOnExceptions = true,
                    NotifyOnValidationError = true,
                    ValidatesOnDataErrors = true,
                });
                txtBox.SetBinding(TextBox.ToolTipProperty, new Binding("(Validation.Errors)[0].ErrorContent") { RelativeSource = RelativeSource.Self });
                _dp.Children.Add(txtBox);
                Grid.SetColumn(txtBox, 2);
                btnInner.SetBinding(Label.TagProperty, new Binding(".") { Source = txtBox });
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
                    Source = _csm,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.TwoWay,
                    ValidatesOnExceptions = true,
                    NotifyOnValidationError = true,
                    ValidatesOnDataErrors = true,
                });

                // 修复：使用安全的ToolTip绑定方式
                var toolTipBinding = new Binding("(Validation.Errors)")
                {
                    RelativeSource = RelativeSource.Self,
                    Converter = new ValidationErrorToToolTipConverter()
                };
                txtBox.SetBinding(TextBox.ToolTipProperty, toolTipBinding);

                _dp.Children.Add(txtBox);
                Grid.SetColumn(txtBox, 2);
                btnInner.SetBinding(Label.TagProperty, new Binding(".") { Source = txtBox });
            }
         private void MakeDiyRadioButton(CommonSettingModel _csm, Grid _dp, int Index)
            {
                DiyRadioButton radioBtn = new DiyRadioButton()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Right
                };
                radioBtn.MouseButonEnent += RadioButtonMouseDown;
                radioBtn.SetBinding(DiyRadioButton.ItemSourceProperty, new Binding(".")
                {
                    Source = _csm,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.TwoWay,
                    ValidatesOnExceptions = true,
                    NotifyOnValidationError = true,
                    ValidatesOnDataErrors = true,
                });
                _dp.Children.Add(radioBtn);
                Grid.SetColumn(radioBtn, 2);
                Grid.SetColumnSpan(radioBtn, 2);
        }
           

            private void MakeDiyAdjustAngleButton(CommonSettingModel _csm, Grid _dp, int Index)
            {
                DiyRadioButton AdjustAngleBtn = new DiyRadioButton()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Right,
                };
                AdjustAngleBtn.MouseButonEnent += AdjustAngleMouseDown;
                AdjustAngleBtn.SetBinding(DiyRadioButton.ItemSourceProperty, new Binding(".")
                {
                    Source = _csm,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.TwoWay,
                    ValidatesOnExceptions = true,
                    NotifyOnValidationError = true,
                    ValidatesOnDataErrors = true,
                });
                _dp.Children.Add(AdjustAngleBtn);

                Grid.SetColumn(AdjustAngleBtn, 1);
                Grid.SetColumnSpan(AdjustAngleBtn, 3);
            }

            private void MakeDiyRadioButtonByStartingDynamo(CommonSettingModel _csm, Grid _dp, int Index)
            {
                DiyRadioButton radioBtn = new DiyRadioButton()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Right
                };
                radioBtn.MouseButonEnent += RadioButtonMouseDown;
                radioBtn.SetBinding(DiyRadioButton.ItemSourceProperty, new Binding(".")
                {
                    Source = _csm,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.TwoWay,
                    ValidatesOnExceptions = true,
                    NotifyOnValidationError = true,
                    ValidatesOnDataErrors = true,
                });
                _dp.Children.Add(radioBtn);

                StartingDynamoIcon IconContentControl = new StartingDynamoIcon()
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center
                };
                CommonSettingModelHardwareState commonSettingModelHardwareState = AcceleatorMvvm.VJ_StartingDynamoLabelList[Index];
                IconContentControl.SetBinding(StartingDynamoIcon.IconForeColorProperty, new Binding("LabmForecolor")
                {
                    Source = commonSettingModelHardwareState,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.TwoWay
                });
                IconContentControl.SetBinding(StartingDynamoIcon.IconBkgColorProperty, new Binding("BackForeColor")
                {
                    Source = commonSettingModelHardwareState,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.TwoWay
                });
                IconContentControl.SetBinding(StartingDynamoIcon.IsShowFlashProperty, new Binding("StatusValue")
                {
                    Source = commonSettingModelHardwareState,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.TwoWay
                });
                IconContentControl.SetBinding(StartingDynamoIcon.UpdateSourceProperty, new Binding("UpdateSource")
                {
                    Source = commonSettingModelHardwareState,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.TwoWay
                });
                Grid.SetColumn(IconContentControl, 2);
                _dp.Children.Add(IconContentControl);

                Grid.SetColumn(radioBtn, 2);
                Grid.SetColumnSpan(radioBtn, 2);
            }
            private void MakeDiyPreviewButton(CommonSettingModel _csm, Grid _dp, int Index)
            {
                Border BtnPreviewCommand = new Border()
                {
                    Width = 90,
                    Height = 40,
                    Style = (Style)this.FindResource("diyBtnCarHand"),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(0, 0, 5, 0)
                };
                BtnPreviewCommand.MouseLeftButtonDown += BtnSendCommand_PreviewMouseUp;
                Label btnInner = new Label()
                {
                    Content = UpdateStatusNameAction("PreHot"),
                    Style = (Style)this.FindResource("diyLabel"),
                    Foreground = StrToBrush("#FFFFFF")
                };
                CommonSettingModelWarmUpHardwareState commonSettingModelHardwareState = AcceleatorMvvm.VJ_RaySourceWarmUpStatusLabelList[Index];
                btnInner.SetBinding(Label.ContentProperty, new Binding("BtnForText")
                {
                    Source = commonSettingModelHardwareState,
                    Mode = BindingMode.TwoWay,
                });
                BtnPreviewCommand.Child = btnInner;
                BtnPreviewCommand.SetBinding(Border.TagProperty, new Binding(".") { Source = _csm, Mode = BindingMode.TwoWay });
                BtnPreviewCommand.DataContext = _csm;

                MetroProgressBar btnWarm = new MetroProgressBar()
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    BorderThickness = new Thickness(1)
                };
                btnWarm.MetroProgress.SetBinding(ProgressBar.MaximumProperty, new Binding("WarmupTotalTime") { Source = commonSettingModelHardwareState, Mode = BindingMode.TwoWay });
                btnWarm.MetroProgress.SetBinding(ProgressBar.ValueProperty, new Binding("WarmupCurrentTime") { Source = commonSettingModelHardwareState, Mode = BindingMode.TwoWay });

                btnWarm.lblprogressTotalTimeBar.SetBinding(Label.ContentProperty, new Binding("WarmupTotalTime") { Source = commonSettingModelHardwareState, Mode = BindingMode.TwoWay });
                btnWarm.lblprogressCurrentTimeBar.SetBinding(Label.ContentProperty, new Binding("WarmupCurrentTime") { Source = commonSettingModelHardwareState, Mode = BindingMode.TwoWay });

                Grid.SetColumn(btnWarm, 2);
                _dp.Children.Add(BtnPreviewCommand);

                _dp.Children.Add(btnWarm);
                Grid.SetColumn(BtnPreviewCommand, 2);
                Grid.SetColumnSpan(BtnPreviewCommand, 2);
            }

            private UserControl MakeFalutGrid(CommonSettingModel _csm, int Index)
            {
                if (_csm.CommonSettingType == "WarmUpButton")
                {
                    VJ_RaySourceFalutPanel vJ_RaySourceFalutPanel = new VJ_RaySourceFalutPanel();
                    CommonSettingModelWarmUpHardwareState commonSettingModelHardwareState = AcceleatorMvvm.VJ_RaySourceWarmUpStatusLabelList[Index];
                    vJ_RaySourceFalutPanel.SetBinding(VJ_RaySourceFalutPanel.VJ_FalutCodeProperty, new Binding("FalutCode") { Source = commonSettingModelHardwareState });
                    return vJ_RaySourceFalutPanel;
                }
                else
                {
                    return null;
                }
            }

            public override bool IsConnectionEquipment()
            {
                return PlcManager.IsConnect();
            }
        private async void BtnSendCommand_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            await UIDispatcher.InvokeAsync(() => {
                try
                {
                    if (!IsConnectionEquipment())
                    {
                        BG_MESSAGEBOX.Show(Application.Current?.MainWindow, UpdateStatusNameAction("Tip"), UnConnectionWithPlc);
                        return;
                    }
                    bool isSendSuccess = false;
                    isSendSuccess = SendCommand(sender);
                    AcceleatorMvvm.SaveCommonSettingModelList();
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
                });
            }

            private async void RadioButtonMouseDown(object sender, MouseButtonEventArgs e)
            {
            await UIDispatcher.InvokeAsync(() => {
                Border _bd = sender as Border;
                Label _lbl = _bd.Child as Label;
                
                CommonSettingModel csm = _bd.Tag as CommonSettingModel;
                BuryingPoint($"Setting{_lbl.Content.ToString()}Command");
                if (!IsConnectionEquipment())
                {
                    BG_MESSAGEBOX.Show(Application.Current?.MainWindow, UpdateStatusNameAction("Tip"), UnConnectionWithPlc);
                    return;
                }
                string Position = (_bd.Tag as string).Split('?')[0];
                bool Value = (_bd.Tag as string).Split('?')[1] == "1";

                //Debug.WriteLine("ganggang_" + Position + " | " + Value);

                Task.Run(() =>
                {
                    try
                    {
                        PlcManager.WritePositionValue(Position, Value);
                    }
                    catch (System.Exception ex)
                    {
                        CommonDeleget.HandTaskException(ex);
                    }
                });
                StrToBrush("#FFFFFF");
            });
        }

            private async void AdjustAngleMouseDown(object sender, MouseButtonEventArgs e)
            {
            await UIDispatcher.InvokeAsync(() => {
                Border _bd = sender as Border;
                Label _lbl = _bd.Child as Label;
                CommonSettingModel csm = _bd.Tag as CommonSettingModel;
                BuryingPoint($"Setting{_lbl.Content.ToString()}Command");
                if (!IsConnectionEquipment())
                {
                    BG_MESSAGEBOX.Show(Application.Current?.MainWindow, UpdateStatusNameAction("Tip"), UnConnectionWithPlc);
                    return;
                }
                string Position = (_bd.Tag as string).Split('?')[0];
                bool Value = (_bd.Tag as string).Split('?')[1] == "1";
                Task.Run(() =>
                {
                    try
                    {
                        PlcManager.WritePositionValue(Position, Value);
                        Thread.Sleep(1000);
                        PlcManager.WritePositionValue(Position, !Value);
                    }
                    catch (System.Exception ex)
                    {
                        CommonDeleget.HandTaskException(ex);
                    }
                });
                StrToBrush("#FFFFFF");
            });
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

            // 加载事件改为异步
            private async void FH_BetratronBoostingSetting_Loaded(object sender, RoutedEventArgs e)
            {
                // 可以添加加载指示器
                await InitContentAsync();
            }

        /// <summary>
        /// 获取名称
        /// </summary>
        /// <returns></returns>
        public override string GetName()
        {
            return "BS_VJRayMachine";
        }
    }
    
    public class VJ_AcceleatorMvvm : BaseMvvm
    {
        public Dictionary<int, ObservableCollection<CommonSettingModel>> VJ_RaySourceDicList = new Dictionary<int, ObservableCollection<CommonSettingModel>>();
        public Dictionary<int, string> VJ_RaySourceFileList = new Dictionary<int, string>();
        public Dictionary<int, string> VJ_RaySourceDoseValueFileList = new Dictionary<int, string>();
        public Dictionary<int, CommonSettingModelHardwareState> VJ_RaySourceRayStatusLabelList = new Dictionary<int, CommonSettingModelHardwareState>();
        public Dictionary<int, CommonSettingModelHardwareState> VJ_StartingDynamoLabelList = new Dictionary<int, CommonSettingModelHardwareState>();
        public Dictionary<int, CommonSettingModelWarmUpHardwareState> VJ_RaySourceWarmUpStatusLabelList = new Dictionary<int, CommonSettingModelWarmUpHardwareState>();
        public VJ_AcceleatorMvvm()
        {
            InitData();
            InitPanel();
        }
        private void InitData()
        {
            Car_BoostSettingBLL car_BoostSettingBLL = new Car_BoostSettingBLL(controlVersion);
            VJ_RaySourceDicList.Clear();
            string VJ_RaySourceDir = SystemDirectoryConfig.GetInstance().GetVJ_RaySourceDir(controlVersion);//E:\\badSvn_cmw\\CMW_OUTPUT\\CONFIG\\BS2000\\BS2000RaySource
            if (!Directory.Exists(VJ_RaySourceDir))
            {
                return;
            }
            int Index = 1;
            
            //if (controlVersion == ControlVersion.BGV6000BS)
            //{
            //    Index = 2;
            //}
            
            foreach (var VJ_RaySource in Directory.GetFiles(VJ_RaySourceDir)) // VJ_RaySource = E:\\badSvn_cmw\\CMW_OUTPUT\\CONFIG\\BS2000\\BS2000RaySource\\VJ_View1.xml
            {
                if(File.Exists(VJ_RaySource))
                {
                    ObservableCollection<CommonSettingModel> VJ_RaySourceList = new ObservableCollection<CommonSettingModel>();
                    car_BoostSettingBLL.GetCarBoostSettingConfigDataModel(VJ_RaySource).ToList().ForEach(q => VJ_RaySourceList.Add(q));
                    VJ_RaySourceDicList[Index] = VJ_RaySourceList; //VJ_RaySourceList = [出束,启动电机...] 共7个，取自 BS2000RaySource\\VJ_View1.xml
                    VJ_RaySourceFileList[Index] = VJ_RaySource;
                    VJ_RaySourceRayStatusLabelList[Index] = new CommonSettingModelHardwareState() 
                    {
                        PositionIndex = (PLCPositionEnum)Enum.Parse(typeof(PLCPositionEnum),
                        VJ_RaySourceList.First(q => q.CommonSettingName == "RayOn").CommonSettingPLCValue)
                    };
                    VJ_StartingDynamoLabelList[Index] = new CommonSettingModelHardwareState()
                    {
                        PositionIndex = (PLCPositionEnum)Enum.Parse(typeof(PLCPositionEnum),
                        VJ_RaySourceList.First(q => q.CommonSettingName == "StartingDynamo").CommonSettingPLCValue)
                    };

                    VJ_RaySourceWarmUpStatusLabelList[Index] = new CommonSettingModelWarmUpHardwareState()
                    {
                        StandStatusValue = VJ_RaySourceList.First(q => q.CommonSettingName == "PreheatStatus").Warmup_TimeSource,
                    };
                    Index++;
                }
            }
        }
        private void InitPanel()
        {
            string VJ_DoseDir = SystemDirectoryConfig.GetInstance().GetVJ_RaySourceDoseValueDir(BG_Entities.ControlVersion.BS); // VJ_DoseDir = E:\\badSvn_cmw\\CMW_OUTPUT\\CONFIG\\BS2000\\BS2000RaySourceDoseValue
            if (!Directory.Exists(VJ_DoseDir))
            {
                return;
            }
            int Index = 0;
            foreach (var DoseFileItem in Directory.GetFiles(VJ_DoseDir))
            {
                if (File.Exists(DoseFileItem))
                {
                    VJ_RaySourceDoseValueFileList[Index] = DoseFileItem;
                    Index++;
                }
            }
        }
        public override void LoadUIText()
        {

        }
        protected override void InquirePlcStatus(List<bool> StatusList)
        {
          
        }

        protected override void ConnectionStatus(bool ConnectionStatus)
        {
            if(controlVersion != ControlVersion.BS)
            {
                return;
            }
            ReflashUI(ConnectionStatus);
        }

        protected override void BSConnectionStatus(bool ConnectionStatus)
        {
            if (controlVersion == ControlVersion.BS)
            {
                return;
            }
            ReflashUI(ConnectionStatus);
        }
        private Dispatcher UIDispatcher => Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
        private async void ReflashUI(bool ConnectionStatus)
        {
            await UIDispatcher.InvokeAsync(() =>
            {

                foreach (var RayStatusItem in VJ_RaySourceRayStatusLabelList)
                {
                    if (!ConnectionStatus)
                    {
                        RayStatusItem.Value.StatusValue = false;
                        RayStatusItem.Value.LabmForecolor = "#000000";
                        continue;
                    }
                    bool PositionValue = PlcManager.GetStatusByPositionEnum(RayStatusItem.Value.PositionIndex);
                    RayStatusItem.Value.StatusValue = PositionValue;
                    RayStatusItem.Value.UpdateSource = DateTime.Now;
                }
                foreach (var StartingDynamoItem in VJ_StartingDynamoLabelList)
                {
                    if (!ConnectionStatus)
                    {
                        StartingDynamoItem.Value.StatusValue = false;
                        StartingDynamoItem.Value.LabmForecolor = "#000000";
                        continue;
                    }
                    bool PositionValue = PlcManager.GetStatusByPositionEnum(StartingDynamoItem.Value.PositionIndex);
                    StartingDynamoItem.Value.StatusValue = PositionValue;
                    StartingDynamoItem.Value.UpdateSource = DateTime.Now;
                }
                foreach (var WarmupStatus in VJ_RaySourceWarmUpStatusLabelList)
                {
                    if (!ConnectionStatus)
                    {
                        WarmupStatus.Value.BtnForText = UpdateStatusNameAction("PreHot");
                        continue;
                    }
                    string[] WramTime = WarmupStatus.Value.StandStatusValue.Split(';');
                    WarmupStatus.Value.WarmupTotalTime = PlcManager.GetStatusByDIntPositionEnum(WramTime[0]);
                    WarmupStatus.Value.WarmupCurrentTime = PlcManager.GetStatusByDIntPositionEnum(WramTime[1]);
                    //int PositionIntValue = PlcManager.GetStatusBUIntPositionEnum(WramTime[3]);
                    WarmupStatus.Value.BtnForText = ValueTransforToBrush(WarmupStatus.Value.WarmupCurrentTime, WarmupStatus.Value.WarmupTotalTime);
                    WarmupStatus.Value.FalutCode = PlcManager.GetStatusBUIntPositionEnum(WramTime[3]);
                }
            });
        }

        private string ValueTransforToBrush(int CurrentWarmTimeCount,int TotalWarmTimeCount)
        {
            if(CurrentWarmTimeCount == TotalWarmTimeCount) return UpdateStatusNameAction("PreHot");
            else return UpdateStatusNameAction("StopPreHot");
            //Byte[] FalutBytes = BitConverter.GetBytes(Value);
            //int FalutCode = BitHelper.GetBit(FalutBytes[3],6);
            //switch (FalutCode)
            //{
            //    case 0:
            //        return UpdateStatusNameAction("PreHot");
            //    case 1:
            //        return UpdateStatusNameAction("StopPreHot");
            //    case 2:
            //        return UpdateStatusNameAction("PreHot");
            //    default:
            //        return UpdateStatusNameAction("PreHot");
            //}
        }

        protected override void AccelatorConnectionStatus(bool ConnectionStatus)
        {
            try
            {
              
            }
            catch (Exception ex)
            {
                CommonDeleget.HandTaskException(ex);
            }
        }
     
        public void SaveCommonSettingModelList()
        {
            foreach (var RaySourceItem in VJ_RaySourceDicList)
            {
                Car_BoostSettingBLL cbs = new Car_BoostSettingBLL(VJ_RaySourceFileList[RaySourceItem.Key]);
                cbs.SaveConfigDataModel(RaySourceItem.Value, VJ_RaySourceFileList[RaySourceItem.Key], ControlVersion.BS);
            }
        }
        public void DetecotorConnection(bool DetecotrConnection)
        {

        }
    }

    public class ValidationErrorToToolTipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ReadOnlyObservableCollection<ValidationError> errors && errors.Count > 0)
            {
                return errors[0].ErrorContent;
            }
            return null; // 没有错误时返回null，不显示ToolTip
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
