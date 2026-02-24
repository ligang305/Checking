using BG_Entities;
using BG_Services;
using BGCommunication;
using BGModel;
using BGUserControl;
using CMW.Common.Utilities;
using System;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using static CMW.Common.Utilities.ImageImportDll;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonFunc;
using static CMW.Common.Utilities.CommonDeleget;
using BG_WorkFlow;

namespace BGUserControl
{
    [Export("PageModule", typeof(IConditionView))]
    [CustomExportMetadata(1, Modules.CommonSettingModule, "通用设置模块", "ZZW", "1.0.0")]
    public class CommonSettingModule : BaseModules
    {
        /// <summary>
        /// 外部可能
        /// </summary>
        public BaseScanProtocol _ScanProtocol
        {
            get; set;
        }

       
        Grid _MainGrid = new Grid();
        Border _MainBorder = new Border()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Margin = new Thickness(0),
            BorderThickness = new Thickness(2),
            BorderBrush = StrToBrush("#3F96E6"),
            CornerRadius = new CornerRadius(6, 6, 6, 6),
            Background = StrToBrush("#F2F2F2"),
        };
        ScrollViewer SettingTb = null;//
        ComboBox ddlFontSize;

        [ImportingConstructor]
        public CommonSettingModule() : base(Common.controlVersion)
        {
           
            Loaded += Car_ControlModule_Loaded;
            Unloaded += Car_ControlModule_Unloaded;
            _MainGrid.Width = 600; _MainGrid.Height = 400;
        }
        public override string GetName()
        {
            return UpdateStatusNameAction("CommonSetting");
        }

        public override bool IsConnectionEquipment()
        {
            return IsConnection;
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
            return 680;
        }

        public override double GetWidth()
        {
            return 760;
        }

        TextBox txtPlcAddress; TextBox txtPlcPort;
        TextBox txtScanAddress; TextBox txtScanPort;
        private Grid InitGrid()
        {
            Grid _MainGrid = MakeMainGrid();
            Grid _ContentGrid = MakeContentGrid(_MainGrid);
            #region 设置标题
            DockPanel dp = InitTitle(_MainGrid);
            #endregion
            #region 内容

            MakeTitle(_ContentGrid);

            if(controlVersion != ControlVersion.BS)
            {
                MakelblScanTitle(_ContentGrid);
            }

            InitTrimeModel(_ContentGrid);

            InitFontSizeItems(_ContentGrid);

            MakeCachePanel(_ContentGrid);

            #endregion
            #region 加载TabControl
            LoadParamConfig(SettingTb, dp);
            #endregion
            dp.MouseDown += Dp_MouseDown;
            return _MainGrid;
        }
        /// <summary>
        /// 绘制字体选择的项
        /// </summary>
        /// <param name="grid"></param>
        private void MakeFontSize(Grid grid)   
        {
            Expander gb = new Expander()
            {
                Header = new Label()
                {
                    Content = UpdateStatusNameAction("FontSize"),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Style = (Style)this.FindResource("diyLabel"),
                    Foreground = StrToBrush("#1A4F85"),
                    FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.Big),
                    
                },
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                IsExpanded = true
            };
            Grid.SetRow(gb, 3);
            Grid.SetColumn(gb, 0);
            Grid.SetColumnSpan(gb,5);
            grid.Children.Add(gb);
            Grid _dp = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch };
            gb.Content = _dp;
            _dp.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(14, GridUnitType.Star) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(8, GridUnitType.Star) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(8, GridUnitType.Star) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(8, GridUnitType.Star) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(12, GridUnitType.Star) });

            Label lblFontSizeName = new Label()
            {
                Content = UpdateStatusNameAction("FontSize"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblFontSizeName.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            ddlFontSize = new ComboBox()
            {
                Style = (Style)TryFindResource("stlComboBox"),
                Height = 35,
                Width = 100,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            AddFramewordElement(lblFontSizeName, 0, 0, _dp);
            AddFramewordElement(ddlFontSize, 0, 1, _dp);
        }

        private void AddFramewordElement(FrameworkElement FFe,int Row,int Col,Grid grid)
        {
            Grid.SetRow(FFe, Row);
            Grid.SetColumn(FFe, Col);
            grid.Children.Add(FFe);
        }

        /// <summary>
        /// 初始化语言选项
        /// </summary>
        private void InitFontSizeItems(Grid _grid)
        {
            try
            {
                MakeFontSize(_grid);
                foreach (var FontSizeItem in FontSizeServices.GetInstance().FontsizeList)
                {
                    (FontSizeItem as FontSizeModel).FontSizeName = UpdateStatusNameAction((FontSizeItem as FontSizeModel).FontSize);
                }
                ddlFontSize.ItemsSource = FontSizeServices.GetInstance().FontsizeList;
                ddlFontSize.DisplayMemberPath = "FontSizeName"; 
                ddlFontSize.SelectedValuePath = "FontSize";
                ddlFontSize.SelectionChanged -= DdlFontSize_SelectionChanged;
                ddlFontSize.SelectionChanged += DdlFontSize_SelectionChanged;
                foreach (var item in ddlFontSize.Items)
                {
                    if ((item as FontSizeModel).FontSize.ToLower() == ConfigServices.GetInstance().localConfigModel.FontSize.ToLower())
                    {
                        ddlFontSize.SelectedItem = item;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void DdlFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox mi = sender as ComboBox;
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            CommonDeleget.UpdateConfigs("fontsize", mi.SelectedValue as string, Section.SOFT);
            ConfigServices.GetInstance().localConfigModel.LANGUAGE = mi.SelectedValue as string;
            BuryingPoint($"{UpdateStatusNameAction("SwitchFont")}:{ConfigServices.GetInstance().localConfigModel.LANGUAGE}");
            ButtonInvoke.SwitchFontSizeAction(mi.SelectedValue as string);
        }
    
        private void InitTrimeModel(Grid _MainGrid)
        {
            Expander gb = new Expander()
            {
                Header = new Label()
                {
                    Content = UpdateStatusNameAction("SelectTraial"),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Style = (Style)this.FindResource("diyLabel"),
                    Foreground = StrToBrush("#1A4F85"),
                    FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.Big),
              
                },
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                IsExpanded = true
            };
            Grid _dp = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch };
            gb.Content = _dp;
            _dp.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(14, GridUnitType.Star) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(8, GridUnitType.Star) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(8, GridUnitType.Star) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(8, GridUnitType.Star) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(12, GridUnitType.Star) });

            Label lblName = new Label()
            {
                Content = UpdateStatusNameAction("SelectTraial"),
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            lblName.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            ComboBox cbBox = new ComboBox()
            {
                Style = (Style)this.FindResource("stlComboBox"),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(0, 3, 3, 3),
                Height = 35,
                Width = 100
            };
            //cbBox.Items.Add(new SelectObject() { SelectText = UpdateStatusNameAction("LocalTraialImage"), SelectValue = "local" });
            cbBox.Items.Add(new SelectObject() { SelectText = UpdateStatusNameAction("LineTraialImage"), SelectValue = "net" });
            cbBox.DisplayMemberPath = "SelectText";
            cbBox.SelectedValuePath = "SelectValue";
            cbBox.SelectedValue = ConfigServices.GetInstance().localConfigModel.TrialImageMode.ToLower();
            Grid.SetRow(lblName, 0);
            Grid.SetRow(cbBox, 0);
            Grid.SetColumn(lblName, 0);
            Grid.SetColumn(cbBox, 1);
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
            Grid.SetColumn(btnSetting, 4);
            _dp.Children.Add(btnSetting);
            Grid.SetRow(gb, 2);
            Grid.SetColumn(gb, 0);
            Grid.SetColumnSpan(gb, 5);
            _MainGrid.Children.Add(gb);
        }

        private void MakeTxtScanPort(Grid _MainGrid)
        {
            txtScanPort = new TextBox()
            {
                Text = ConfigServices.GetInstance().localConfigModel.ScanPort,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Center,
                Height = 27,
                Width = 70
            };
            Grid.SetRow(txtScanPort, 0);
            Grid.SetColumn(txtScanPort, 3);
            _MainGrid.Children.Add(txtScanPort);
        }

        private void MakeLblScanPort(Grid _MainGrid)
        {
            Label lblScanPort = new Label()
            {
                Content = UpdateStatusNameAction("LinkPort"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblScanPort.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            Grid.SetRow(lblScanPort,0);
            Grid.SetColumn(lblScanPort, 2);
            _MainGrid.Children.Add(lblScanPort);
        }

        private void MaketxtScanAddress(Grid _MainGrid)
        {
            txtScanAddress = new TextBox()
            {
                Text = ConfigServices.GetInstance().localConfigModel.ScanIpAddress,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Center,
                Height = 27,
                Width = 100
            };
            Grid.SetRow(txtScanAddress, 0);
            Grid.SetColumn(txtScanAddress, 1);
            _MainGrid.Children.Add(txtScanAddress);
        }

        private void MakelblScanAddress(Grid _MainGrid)
        {
            Label lblScanAddress = new Label()
            {
                Content = UpdateStatusNameAction("LinkAddress"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblScanAddress.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            Grid.SetRow(lblScanAddress, 0);
            Grid.SetColumn(lblScanAddress, 0);
            _MainGrid.Children.Add(lblScanAddress);
        }

        private void MakelblScanTitle(Grid _MainGrid)
        {
            Expander gb = new Expander()
            {
                Header = new Label()
                {
                    Content = UpdateStatusNameAction("Scan"),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Style = (Style)this.FindResource("diyLabel"),
                    Foreground = StrToBrush("#1A4F85"),
                    FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.Big),
                 
                },
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                IsExpanded = true
            };
            Grid _dp = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch };
            gb.Content = _dp;
            _dp.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(14, GridUnitType.Star) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(8, GridUnitType.Star) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(8, GridUnitType.Star) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(8, GridUnitType.Star) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(12, GridUnitType.Star) });
            MakeScanButton(_dp);
            MakelblScanAddress(_dp);
            MaketxtScanAddress(_dp);
            MakeLblScanPort(_dp);
            MakeTxtScanPort(_dp);
            Grid.SetRow(gb, 1);
            Grid.SetColumn(gb, 0);
            Grid.SetColumnSpan(gb, 5);
            _MainGrid.Children.Add(gb);
        }

        private void MakeScanButton(Grid _MainGrid)
        {
            Border BtnScanSendCommand = MakeSettingBorder();
            BtnScanSendCommand.Tag = "Scan";
            BtnScanSendCommand.PreviewMouseDown -= BtnSendCommand_PreviewMouseDown;
            BtnScanSendCommand.PreviewMouseDown += BtnSendCommand_PreviewMouseDown;
            Grid.SetRow(BtnScanSendCommand, 0);
            Grid.SetColumn(BtnScanSendCommand, 4);
            _MainGrid.Children.Add(BtnScanSendCommand);
        }

        private void MakelblPlcAddress(Grid _MainGrid)
        {
            Label lblPlcAddress = new Label()
            {
                Content = UpdateStatusNameAction("LinkAddress"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblPlcAddress.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            Grid.SetRow(lblPlcAddress, 0);
            Grid.SetColumn(lblPlcAddress, 0);
            _MainGrid.Children.Add(lblPlcAddress);
        }

        private void MakeTxtPlcPort(Grid _MainGrid)
        {
            txtPlcPort = new TextBox()
            {
                Text = ConfigServices.GetInstance().localConfigModel.Port,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Center,
                Height = 27,
                Width = 70
            };
            Grid.SetRow(txtPlcPort, 0);
            Grid.SetColumn(txtPlcPort, 3);
            _MainGrid.Children.Add(txtPlcPort);
        }

        private void MakePlcLbl(Grid _MainGrid)
        {
            Label lblPlcPort = new Label()
            {
                Content = UpdateStatusNameAction("LinkPort"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblPlcPort.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            Grid.SetRow(lblPlcPort, 0);
            Grid.SetColumn(lblPlcPort, 2);
            _MainGrid.Children.Add(lblPlcPort);
        }

        private void MakeTextPlc(Grid _MainGrid)
        {
            txtPlcAddress = new TextBox()
            {
                Text = ConfigServices.GetInstance().localConfigModel.IpAddress,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Center,
                Height = 27,
                Width = 100
            };
            Grid.SetRow(txtPlcAddress, 0);
            Grid.SetColumn(txtPlcAddress, 1);
            _MainGrid.Children.Add(txtPlcAddress);
        }

        private void MakePlcButton(Grid _MainGrid)
        {
            Border BtnPlcSendCommand = MakeSettingBorder();
            BtnPlcSendCommand.Tag = "PLC";
            BtnPlcSendCommand.PreviewMouseDown -= BtnSendCommand_PreviewMouseDown;
            BtnPlcSendCommand.PreviewMouseDown += BtnSendCommand_PreviewMouseDown;
            Grid.SetRow(BtnPlcSendCommand, 0);
            Grid.SetColumn(BtnPlcSendCommand, 5);
            _MainGrid.Children.Add(BtnPlcSendCommand);
        }

        private void MakeTitle(Grid _MainGrid)
        {
            Expander gb = new Expander()
            {
                Header = new Label()
                {
                    Content = "PLC",
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Style = (Style)this.FindResource("diyLabel"),
                    Foreground = StrToBrush("#1A4F85"),
                    FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.Big),
                    
                },
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                IsExpanded = true
            };
            Grid _dp = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch };
            gb.Content = _dp;
            _dp.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(14, GridUnitType.Star) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(8, GridUnitType.Star) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(8, GridUnitType.Star) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(8, GridUnitType.Star) });
            _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(12, GridUnitType.Star) });
            MakelblPlcAddress(_dp);

            MakeTextPlc(_dp);

            MakePlcLbl(_dp);

            MakeTxtPlcPort(_dp);

            MakePlcButton(_dp);
            Grid.SetRow(gb, 0);
            Grid.SetColumn(gb, 0);
            Grid.SetColumnSpan(gb, 5);
            _MainGrid.Children.Add(gb);
        }

        /// <summary>
        /// 缓存面板
        /// </summary>
        private void MakeCachePanel(Grid _MainGrid)
        {
            Expander ep = new Expander()
            {
                Header = new Label()
                {
                    Content = UpdateStatusNameAction("ClearCache"),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Style = (Style)this.FindResource("diyLabel"),
                    Foreground = StrToBrush("#1A4F85"),
                    FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.Big),
                },
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                IsExpanded = true
            };
            DiyCacheDelete cacheDelete = new DiyCacheDelete() { Height = 150,HorizontalAlignment = HorizontalAlignment.Stretch};
            ep.Content = cacheDelete;
            Grid.SetRow(ep,4); Grid.SetColumn(ep, 0);Grid.SetColumnSpan(ep,5);
            _MainGrid.Children.Add(ep);
        }

        /// <summary>
        /// 初始化主面板
        /// </summary>
        /// <returns></returns>
        private static Grid MakeMainGrid()
        {
            Grid _MainGrid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            //_MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Pixel) });
            //_MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            //_MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(8, GridUnitType.Star) });
            //_MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(14, GridUnitType.Star) });
            //_MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(8, GridUnitType.Star) });
            //_MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(8, GridUnitType.Star) });
            //_MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(12, GridUnitType.Star) });

            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Pixel) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(25, GridUnitType.Star) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(40, GridUnitType.Pixel) });
            return _MainGrid;
        }
        /// <summary>
        /// 初始化内容面板
        /// </summary>
        /// <returns></returns>
        private  Grid MakeContentGrid(Grid _MainGrid)
        {
            SettingTb = new ScrollViewer()
            {
                Template = (ControlTemplate)this.TryFindResource("MyScrollViewer"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalAlignment = VerticalAlignment.Stretch,
                VerticalScrollBarVisibility = ScrollBarVisibility.Visible
            };
            Grid _ContentGrid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,MaxHeight = 600
            };
            _ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            _ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            _ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            _ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            _ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            _ContentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(8, GridUnitType.Star) });
            _ContentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(14, GridUnitType.Star) });
            _ContentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(8, GridUnitType.Star) });
            _ContentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(8, GridUnitType.Star) });
            _ContentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(12, GridUnitType.Star) });
            SettingTb.Content = _ContentGrid;
        
            //_MainGrid.Children.Add(SettingTb);
            return _ContentGrid;
        }
        private void BtnSetting_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ComboBox cbx = (sender as Border).Tag as ComboBox;
            BuryingPoint($"{UpdateStatusNameAction("SwitchMode")}{cbx.SelectedValue.ToString()}");
            UpdateConfigs("TrailImageMode", cbx.SelectedValue.ToString(), Section.SOFT);
        }

        private void BtnSendCommand_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Border bd = sender as Border;
            string Flag = bd.Tag as string;
            switch (Flag)
            {
                case "PLC":
                    if (!IsAddress(txtPlcAddress.Text))
                    {
                        BG_MESSAGEBOX.Show(Application.Current?.MainWindow, UpdateStatusNameAction("Tip"), UpdateStatusNameAction("IpAddressNotMatch"));
                        return;
                    }

                    if (!IsPort(txtPlcPort.Text))
                    {
                        BG_MESSAGEBOX.Show(Application.Current?.MainWindow, UpdateStatusNameAction("Tip"), UpdateStatusNameAction("PortNotMatch"));
                        return;
                    }

                    InitPlc();
                    break;
                case "Scan":
                    if (!IsAddress(txtScanAddress.Text))
                    {
                        BG_MESSAGEBOX.Show(UpdateStatusNameAction("Tip"), UpdateStatusNameAction("IpAddressNotMatch"));
                        return;
                    }

                    if (!IsPort(txtScanPort.Text))
                    {
                        BG_MESSAGEBOX.Show(UpdateStatusNameAction("Tip"), UpdateStatusNameAction("PortNotMatch"));
                        return;
                    }

                    InitScan();
                    break;
                default:
                    break;
            }
        }
        private void InitScan()
        {
            SettingAddress(txtScanAddress.Text, txtScanPort.Text, "Scan");
            SX_Disconnect(intPtr);
            if (MessageBox.Show(UpdateStatusNameAction("NeedRestart"), UpdateStatusNameAction("Tip"), MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Application.Current?.Shutdown();
                RestartSystemEvent();
            }
        }

        private void InitPlc()
        {
            PLCControllerManager.GetInstance().DisConnect();
            SettingAddress(txtPlcAddress.Text, txtPlcPort.Text, "Plc");
            if (MessageBox.Show(UpdateStatusNameAction("NeedRestart"), UpdateStatusNameAction("Tip"), MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Application.Current?.Shutdown();
              
                RestartSystemEvent();
            }
        }

        private Border MakeSettingBorder()
        {
            Border BtnSendCommand = new Border()
            {
                Width = 50,
                Height = 30,
                Style = (Style)this.FindResource("diyBtnCarHand"),
            };
            Label btnScanInner = new Label()
            {
                Content = UpdateStatusNameAction("Setting"),
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#FFFFFF")
            };
            btnScanInner.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            BtnSendCommand.Child = btnScanInner;
            return BtnSendCommand;
        }

        /// <summary>
        /// 设置软件标题
        /// </summary>
        /// <param name="_MainGrid"></param>
        /// <returns></returns>
        private DockPanel InitTitle(Grid _MainGrid)
        {
            DockPanel dp = new DockPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                LastChildFill = true
            };
            Border bd = new Border()
            {
                Margin = new Thickness(0, 0, 0, 0),
                BorderThickness = new Thickness(0, 0, 0, 0),
                BorderBrush = (LinearGradientBrush)this.FindResource("LnearBorderColor"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 32, Height = 32,
                Background = (LinearGradientBrush)this.FindResource("TitleBackGround"),
            };
            bd.PreviewMouseLeftButtonDown += Bd_PreviewMouseLeftButtonDown;
            Canvas cs = new Canvas()
            {
                Style = (Style)this.FindResource("diyCloseCanvas"),
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 1, 10, 0),
                Width = 14,
                Height = 14
            };
            bd.Child = cs;
            Panel.SetZIndex(bd,99);
            Grid.SetColumn(dp, 0);
            Grid.SetColumnSpan(dp, 2);
            Grid.SetRowSpan(dp, 2);
            Grid.SetRow(dp, 0);
            Grid.SetColumn(bd,1);
            Grid.SetRow(bd,0);
            _MainGrid.Children.Add(dp);
            _MainGrid.Children.Add(bd);
            return dp;
        }

        private void Dp_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CurrentWindow.DragMove();
        }

        private void Bd_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Close();
        }

        private void Car_ControlModule_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            IsModuleOpen = false;
            BuryingPoint($"{UpdateStatusNameAction("GeneralModuleLeave")}");
            Task.Run(() => {SetCommand(CommandDic[Command.AutoMode], true);});
        }

        private void Car_ControlModule_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ButtonInvoke.SwitchLanguageEvent -= SwitchLanguage;
            ButtonInvoke.SwitchFontSizeEvent -= SwitchFontSize;
            BuryingPoint($"{UpdateStatusNameAction("GeneralModuleEnter")}");
            _MainGrid = InitGrid();
            Panel.SetZIndex(_MainBorder,999);
            _MainBorder.Child = _MainGrid;
            
            Content = _MainBorder;
            ButtonInvoke.SwitchLanguageEvent += SwitchLanguage;
            ButtonInvoke.SwitchFontSizeEvent += SwitchFontSize;
            IsModuleOpen = true;
            Task.Run(() => { SetCommand(CommandDic[Command.AutoMode], false); });
        }

        public void SwitchFontSize(string FontSize)
        {
            Car_ControlModule_Loaded(null, null);
        }
        public void SwitchLanguage(string language)
        {
            base.Base_SwitchLanguage(language);
            Car_ControlModule_Loaded(null, null);
        }

        #region LoadParamConfig
        /// <summary>
        /// 加载参数设置模块
        /// </summary>
        /// <param name="ContentGrid"></param>
        private void LoadParamConfig(FrameworkElement Content, DockPanel dp)
        {
            TabControl tc = new TabControl() { Style = (Style)this.TryFindResource("DiyTabControl") };
            //TabItem ParamConfig = new TabItem()
            //{
            //    Header = UpdateStatusNameAction("ParamConfig"),
            //    Style = (Style)this.TryFindResource("DiyTabItemStyle"),
            //    HorizontalAlignment = HorizontalAlignment.Stretch,
            //    HorizontalContentAlignment = HorizontalAlignment.Stretch,
            //};
            TabItem CommonSetting = new TabItem()
            {
                Header = UpdateStatusNameAction("CommonSetting"),
                Style = (Style)this.TryFindResource("DiyTabItemStyle"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
            };
            //TabItem PluginSetting = new TabItem()
            //{
            //    Header = UpdateStatusNameAction("PluginSetting"),
            //    Style = (Style)this.TryFindResource("DiyTabItemStyle"),
            //    HorizontalAlignment = HorizontalAlignment.Stretch,
            //    HorizontalContentAlignment = HorizontalAlignment.Stretch,
            //};
            ////参数设置
            //var ParamConfigModel = ContentPlugins?.First(q => q.Metadata.Name == Modules.ParamConfigModule)?.Value;
            //ParamConfigModel?.SetCarVersion(cv);
            //ParamConfig.Content = ParamConfigModel;

            ////插件设置
            //var ModulesPluginModel = ContentPlugins?.First(q => q.Metadata.Name == Modules.PluginsModule)?.Value;
            //ModulesPluginModel?.SetCarVersion(cv);
            //PluginSetting.Content = ModulesPluginModel;


            CommonSetting.Content = Content;
            DockPanel.SetDock(tc,Dock.Left);
            tc.Items.Add(CommonSetting);
            //tc.Items.Add(ParamConfig);
            //tc.Items.Add(PluginSetting);
            dp.Children.Add(tc);
        }
        #endregion

    }
}
