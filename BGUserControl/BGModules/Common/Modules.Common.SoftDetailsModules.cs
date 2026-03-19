using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BGCommunication;
using BGModel;
using static CMW.Common.Utilities.CommonFunc;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
using System.Windows.Media;
using CMW.Common.Utilities;
using System.Threading;
using System.Runtime.InteropServices;
using BG_Services;
using BG_Entities;
using BG_WorkFlow;

namespace BGUserControl
{
    [Export("PageModule", typeof(IConditionView))]
    [CustomExportMetadata(1, Modules.SoftDetailsModules, "软件信息", "ZZW", "1.0.0")]
    public class SoftDetailsModules: CommonBaseModules
    {
        Grid _MainGrid = new Grid();
        Visibsliy DetectorRunTime = new Visibsliy();
        Visibsliy DetectorScanTime = new Visibsliy();
        Visibsliy PlcVersion = new Visibsliy();
        Visibsliy SoftWareVersion = new Visibsliy();
        Visibsliy BoostingVersion = new Visibsliy();
        Visibsliy DetectorVersion = new Visibsliy();
        Visibsliy DetectorSoftVersion = new Visibsliy();
        Visibsliy BoostingRunTime = new Visibsliy();
        Visibsliy BoostingRayTime = new Visibsliy();

        Visibsliy EquipmentOnTime = new Visibsliy();
        Visibsliy EquipmentOffTime = new Visibsliy();
        Visibsliy EquipmentPowerOnTotalTime = new Visibsliy();
        Visibsliy EquipmentPowerOnTotalDay = new Visibsliy();

        Visibsliy BoostingType = new Visibsliy();//加速器类型
        Visibsliy PLCType = new Visibsliy();
        Visibsliy DdetectorVersion = new Visibsliy();
        bool? isVisible = false;
        Border _MainBorder = new Border()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Margin = new Thickness(0),
            BorderThickness = new Thickness(2),
            BorderBrush = StrToBrush("#3F96E6"),
            CornerRadius = new CornerRadius(6, 6, 6, 6),
            Background = StrToBrush("#FFFFFF"),
        };
        [ImportingConstructor]
        public SoftDetailsModules()
        {
            Loaded += SoftDetailsModules_Loaded; 
            Unloaded += SoftDetailsModules_Unloaded;
            IsVisibleChanged += SoftDetailsModules_IsVisibleChanged;
        }
        private void SoftDetailsModules_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            isVisible = e.NewValue as bool?;
            if (isVisible != null && isVisible == true)
            {
                Task.Factory.StartNew(() => { InqureStatusThread(); });
            }
        }
        private Grid InitGrid()
        {
            Grid _MainGrid = MakeMainGrid();
            #region 设置标题
            DockPanel dp = InitTitle(_MainGrid);
            #endregion
            InitContent(_MainGrid);
            #region 内容

            #endregion
            dp.MouseLeftButtonDown += Dp_MouseDown;
            return _MainGrid;
        }
        private void InitContent(Grid MainGrid)
        {
            Grid RowGd = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            for (int i = 0; i < 17; i++)
            {
                RowGd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            }
            RowGd.ColumnDefinitions.Add(new ColumnDefinition(){Width = new GridLength(300,GridUnitType.Pixel)});
            RowGd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200, GridUnitType.Star) });
            
            Label lblDetectorTitle = new Label(){HorizontalAlignment = HorizontalAlignment.Stretch,HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Content = UpdateStatusNameAction("DetectorTitle"),Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblDetectorTitle.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            Label lblDetectorRunTime = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblDetectorRunTime.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            lblDetectorRunTime.SetBinding(Label.ContentProperty, new Binding("DisplayName"){Source = DetectorRunTime });
            
            Label lblDetectorScanTimeTitle = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Content = UpdateStatusNameAction("DetectorScanTimeTitle"),
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblDetectorScanTimeTitle.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            Label lblDetectorScanTime = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblDetectorScanTime.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            lblDetectorScanTime.SetBinding(Label.ContentProperty, new Binding("DisplayName") { Source = DetectorScanTime });
            Label lblPlcVersionTitle = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Content = UpdateStatusNameAction("PlcVersion"),
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblPlcVersionTitle.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            Label lblPlcVersion = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblPlcVersion.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            lblPlcVersion.SetBinding(Label.ContentProperty, new Binding("DisplayName") { Source = PlcVersion });
            Label lblSoftwareTitle = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Content = UpdateStatusNameAction("SoftwareVersion"),
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblSoftwareTitle.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            Label lblSoftwareVersion = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblSoftwareVersion.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            lblSoftwareVersion.SetBinding(Label.ContentProperty, new Binding("DisplayName") { Source = SoftWareVersion });

            Border btnCheckUpdate = MakeSetButton("CheckUpdate");
            btnCheckUpdate.HorizontalAlignment = HorizontalAlignment.Right;
            btnCheckUpdate.MouseLeftButtonDown += BtnCheckUpdate_MouseLeftButtonDown;
            Label lblBoostingTitle = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Content = UpdateStatusNameAction("BoostingVersion"),
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblBoostingTitle.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            Label lblBoostingVersion = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblBoostingVersion.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            lblBoostingVersion.SetBinding(Label.ContentProperty, new Binding("DisplayName") { Source = BoostingVersion });
            
            Label lblDetectorVersionTitle = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Content = UpdateStatusNameAction("DetectorVersion"),
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblDetectorVersionTitle.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            Label lblDetectorVersion = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblDetectorVersion.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            Label lblDetectorSoftVersionTitle = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Content = UpdateStatusNameAction("DetectorSoftVersion"),
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblDetectorSoftVersionTitle.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            Label lblDetectorSoftVersion = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblDetectorSoftVersion.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            lblDetectorSoftVersion.SetBinding(Label.ContentProperty, new Binding("DisplayName") { Source = DetectorSoftVersion });
            Label lblBoostingRunTimeTitle = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Content = UpdateStatusNameAction("BoostingRunTimeTitle"),
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblBoostingRunTimeTitle.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            Label lblBoostingRunTime = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblBoostingRunTime.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            lblBoostingRunTime.SetBinding(Label.ContentProperty, new Binding("DisplayName") { Source = BoostingRunTime });
            Label lblBoostingRayTimeTitle = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Content = UpdateStatusNameAction("BoostingRayTimeTitle"),
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblBoostingRayTimeTitle.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            Label lblBoostingRayTime = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblBoostingRayTime.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            lblBoostingRayTime.SetBinding(Label.ContentProperty, new Binding("DisplayName") { Source = BoostingRayTime });


            Label lblEquipmentOnTimeTitle = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Content = UpdateStatusNameAction("EquipmentOnTimeTitle"),
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblEquipmentOnTimeTitle.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            Label lblEquipmentOnTime = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblEquipmentOnTime.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            lblEquipmentOnTime.SetBinding(Label.ContentProperty, new Binding("DisplayName") { Source = EquipmentOnTime });


            Label lblEquipmentOffTimeTitle = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Content = UpdateStatusNameAction("EquipmentOffTimeTitle"),
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblEquipmentOffTimeTitle.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            Label lblEquipmentOffTime = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblEquipmentOffTime.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            lblEquipmentOffTime.SetBinding(Label.ContentProperty, new Binding("DisplayName") { Source = EquipmentOffTime });


            Label lblEquipmentPowerOnTimeTitle = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Content = UpdateStatusNameAction("EquipmentPowerOnTimeTitle"),
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblEquipmentPowerOnTimeTitle.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            Label lblEquipmentPowerOnTime = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblEquipmentPowerOnTime.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            lblEquipmentPowerOnTime.SetBinding(Label.ContentProperty, new Binding("DisplayName") { Source = EquipmentPowerOnTotalTime });


            Label lblEquipmentPowerOnDayTitle = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Content = UpdateStatusNameAction("EquipmentPowerOnDayTitle"),
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblEquipmentPowerOnDayTitle.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            Label lblEquipmentPowerOnDay = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85")
            };
            lblEquipmentPowerOnDay.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            lblEquipmentPowerOnDay.SetBinding(Label.ContentProperty, new Binding("DisplayName") { Source = EquipmentPowerOnTotalDay });

            lblDetectorVersion.SetBinding(Label.ContentProperty, new Binding("DisplayName") { Source = DetectorVersion });
            Label lblBoostingType = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85"),
                Content = UpdateStatusNameAction("BoostingType"),
            };
            lblBoostingType.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            Label lblBoostingTypeContent = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85"),
            };
            lblBoostingTypeContent.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            lblBoostingTypeContent.SetBinding(Label.ContentProperty, new Binding("DisplayName") { Source = BoostingType });

            Label lblPLCModel = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85"),
                Content = UpdateStatusNameAction("PLCType"),
            };
            lblPLCModel.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            Label lblPLCModelContent = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85"),
            };
            lblPLCModelContent.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            lblPLCModelContent.SetBinding(Label.ContentProperty, new Binding("DisplayName") { Source = PLCType });

            Label lblDetectorModel = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85"),
                Content = UpdateStatusNameAction("ImageX"),
            };
            lblDetectorModel.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            Label lblDetectorContent = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85"),
            };
            lblDetectorContent.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            lblDetectorContent.SetBinding(Label.ContentProperty, new Binding("DisplayName") { Source = DdetectorVersion });

            Grid.SetColumn(lblDetectorTitle,0); Grid.SetRow(lblDetectorTitle, 0);
            Grid.SetColumn(lblDetectorRunTime, 1); Grid.SetRow(lblDetectorRunTime, 0);
            Grid.SetColumn(lblDetectorScanTimeTitle, 0); Grid.SetRow(lblDetectorScanTimeTitle, 1);
            Grid.SetColumn(lblDetectorScanTime, 1); Grid.SetRow(lblDetectorScanTime, 1);
            Grid.SetColumn(lblPlcVersionTitle, 0); Grid.SetRow(lblPlcVersionTitle, 2);
            Grid.SetColumn(lblPlcVersion, 1); Grid.SetRow(lblPlcVersion, 2);
            Grid.SetColumn(lblSoftwareTitle, 0); Grid.SetRow(lblSoftwareTitle, 3);
            Grid.SetColumn(lblSoftwareVersion, 1); Grid.SetRow(lblSoftwareVersion, 3);
            Grid.SetColumn(btnCheckUpdate, 1); Grid.SetRow(btnCheckUpdate, 3);
            Grid.SetColumn(lblBoostingTitle, 0); Grid.SetRow(lblBoostingTitle, 4);
            Grid.SetColumn(lblBoostingVersion, 1); Grid.SetRow(lblBoostingVersion, 4);
            Grid.SetColumn(lblDetectorVersionTitle, 0); Grid.SetRow(lblDetectorVersionTitle, 5);
            Grid.SetColumn(lblDetectorVersion, 1); Grid.SetRow(lblDetectorVersion, 5);
            Grid.SetColumn(lblBoostingRunTimeTitle, 0); Grid.SetRow(lblBoostingRunTimeTitle, 7);
            Grid.SetColumn(lblBoostingRunTime, 1); Grid.SetRow(lblBoostingRunTime, 7);
            Grid.SetColumn(lblBoostingRayTimeTitle, 0); Grid.SetRow(lblBoostingRayTimeTitle, 8);
            Grid.SetColumn(lblBoostingRayTime, 1); Grid.SetRow(lblBoostingRayTime, 8);

            Grid.SetColumn(lblEquipmentOnTimeTitle, 0); Grid.SetRow(lblEquipmentOnTimeTitle, 9);
            Grid.SetColumn(lblEquipmentOnTime, 1); Grid.SetRow(lblEquipmentOnTime, 9);
            Grid.SetColumn(lblEquipmentOffTimeTitle, 0); Grid.SetRow(lblEquipmentOffTimeTitle, 10);
            Grid.SetColumn(lblEquipmentOffTime, 1); Grid.SetRow(lblEquipmentOffTime, 10);
            Grid.SetColumn(lblEquipmentPowerOnDayTitle, 0); Grid.SetRow(lblEquipmentPowerOnDayTitle, 11);
            Grid.SetColumn(lblEquipmentPowerOnDay, 1); Grid.SetRow(lblEquipmentPowerOnDay, 11);
            Grid.SetColumn(lblEquipmentPowerOnTimeTitle, 0); Grid.SetRow(lblEquipmentPowerOnTimeTitle, 12);
            Grid.SetColumn(lblEquipmentPowerOnTime, 1); Grid.SetRow(lblEquipmentPowerOnTime, 12);

            Grid.SetColumn(lblBoostingTypeContent, 1); Grid.SetRow(lblBoostingTypeContent, 13);
            Grid.SetColumn(lblBoostingType, 0); Grid.SetRow(lblBoostingType, 13);
            Grid.SetColumn(lblPLCModel, 0); Grid.SetRow(lblPLCModel, 14);
            Grid.SetColumn(lblPLCModelContent, 1); Grid.SetRow(lblPLCModelContent, 14);
            Grid.SetColumn(lblDetectorModel, 0); Grid.SetRow(lblDetectorModel, 15);
            Grid.SetColumn(lblDetectorContent, 1); Grid.SetRow(lblDetectorContent, 15);
            Grid.SetColumn(lblDetectorSoftVersionTitle, 0); Grid.SetRow(lblDetectorSoftVersionTitle, 6);
            Grid.SetColumn(lblDetectorSoftVersion, 1); Grid.SetRow(lblDetectorSoftVersion, 6);
            RowGd.Children.Add(lblDetectorTitle); RowGd.Children.Add(lblDetectorRunTime);
            RowGd.Children.Add(lblDetectorScanTimeTitle); RowGd.Children.Add(lblDetectorScanTime);
            RowGd.Children.Add(lblPlcVersionTitle); RowGd.Children.Add(lblPlcVersion);
            RowGd.Children.Add(lblSoftwareTitle); RowGd.Children.Add(lblSoftwareVersion);
            RowGd.Children.Add(lblBoostingTitle); RowGd.Children.Add(lblBoostingVersion);
            RowGd.Children.Add(lblDetectorVersionTitle); RowGd.Children.Add(lblDetectorVersion);
            RowGd.Children.Add(lblBoostingRunTimeTitle); RowGd.Children.Add(lblBoostingRunTime);
            RowGd.Children.Add(lblBoostingRayTimeTitle); RowGd.Children.Add(lblBoostingRayTime);
            RowGd.Children.Add(lblDetectorSoftVersion); RowGd.Children.Add(lblDetectorSoftVersionTitle);
            RowGd.Children.Add(lblBoostingType); RowGd.Children.Add(lblBoostingTypeContent);
            RowGd.Children.Add(lblPLCModel); RowGd.Children.Add(lblPLCModelContent);
            RowGd.Children.Add(lblDetectorModel); RowGd.Children.Add(lblDetectorContent);
            RowGd.Children.Add(lblEquipmentOffTimeTitle); RowGd.Children.Add(lblEquipmentOffTime);
            RowGd.Children.Add(lblEquipmentOnTimeTitle); RowGd.Children.Add(lblEquipmentOnTime);
            RowGd.Children.Add(lblEquipmentPowerOnDayTitle); RowGd.Children.Add(lblEquipmentPowerOnDay);
            RowGd.Children.Add(lblEquipmentPowerOnTimeTitle); RowGd.Children.Add(lblEquipmentPowerOnTime);
            MainGrid.Children.Add(RowGd); RowGd.Children.Add(btnCheckUpdate);
            Grid.SetColumn(RowGd,0);
            Grid.SetRow(RowGd, 1);
        }
        /// <summary>
        /// 检查更新按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCheckUpdate_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                StartUgrApplication.Services.StartUgr();
            }
            catch (Exception ex)
            {
                BGLogs.Log.GetDistance().WriteInfoLogs(ex.StackTrace);
            }
        }

        private void SoftDetailsModules_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }
        private void SoftDetailsModules_Loaded(object sender, RoutedEventArgs e)
        {
            ButtonInvoke.SwitchLanguageEvent -= SwitchLanguage;
            ButtonInvoke.SwitchLanguageEvent += SwitchLanguage;
            ButtonInvoke.SwitchFontSizeEvent -= SwitchFontSize;
            ButtonInvoke.SwitchFontSizeEvent += SwitchFontSize;
            _MainGrid = InitGrid();
            _MainBorder.Child = _MainGrid;
            Content = _MainBorder;
            InitVersionParam();
        }
        public void InitVersionParam()
        {
            int Runtime = 0;
            int Scantime = 0;

            IntPtr detectInptr = IntPtr.Zero;
            if(DetecotrControllerManager.GetInstance().detectorEquipment !=null)
            {
                detectInptr = DetecotrControllerManager.GetInstance().detectorEquipment.DetectorIntPtr;
            }
            StringBuilder SerialNumber = new StringBuilder();
            StringBuilder softVersion = new StringBuilder();
            if (detectInptr!=IntPtr.Zero)
            {
                ImageImportDll.SX_GetTimes(detectInptr, out Runtime, out Scantime);
                
                ImageImportDll.SX_GetSerialNumber(detectInptr, SerialNumber);
               
                ImageImportDll.SX_GetVersion(detectInptr, softVersion);
            }
      
           
            PlcVersion.DisplayName = PLCControllerManager.GetInstance().GetPlcBlockStr(ConfigServices.GetInstance().localConfigModel.EquipmentAddress, Convert.ToUInt16(ConfigServices.GetInstance().localConfigModel.EquipmentLength));
            SoftWareVersion.DisplayName = StartUgrApplication.Services.getVersionInfo().serverSoftware.Version;//CommonFunc.GetSoftVersion();
            BoostingVersion.DisplayName = PLCControllerManager.GetInstance().GetPlcBlockStr
                (ConfigServices.GetInstance().localConfigModel.BoostingVersionAddress,Convert.ToUInt16(ConfigServices.GetInstance().localConfigModel.BoostingVersionLength)).TrimStart().TrimEnd();
            DetectorVersion.DisplayName = SerialNumber.ToString();
            DetectorSoftVersion.DisplayName = softVersion.ToString();
        }
        public void InqureStatusThread()
        {
        
            while (true)
            {
                if (isVisible == null || isVisible == false) break;
                BoostingRayTime.DisplayName = CommonFunc.MillToHour(PLCControllerManager.GetInstance().GetPlcBlockStr
              ( ConfigServices.GetInstance().localConfigModel.BoostingRayTimeAddress, Convert.ToUInt16(ConfigServices.GetInstance().localConfigModel.BoostingRayTimeAddressLength), InqureType.IUInt32));
                Thread.Sleep(1000);
                BoostingRunTime.DisplayName = CommonFunc.MillToHour(PLCControllerManager.GetInstance().GetPlcBlockStr
                    (ConfigServices.GetInstance().localConfigModel.BoostingRunTimeAddress, Convert.ToUInt16(ConfigServices.GetInstance().localConfigModel.BoostingRunTimeAddressLength), InqureType.IUInt32));
                int Runtime = 0;
                int Scantime = 0;

                IntPtr detectInptr = IntPtr.Zero;
                if (DetecotrControllerManager.GetInstance().detectorEquipment != null)
                {
                    detectInptr = DetecotrControllerManager.GetInstance().detectorEquipment.DetectorIntPtr;
                }
                if(detectInptr!= IntPtr.Zero)
                {
                    ImageImportDll.SX_GetTimes(detectInptr, out Runtime, out Scantime);
                }
         
                DetectorRunTime.DisplayName = CommonFunc.MillToHour(Runtime.ToString());
                DetectorScanTime.DisplayName = CommonFunc.MillToHour(Scantime.ToString());
                BoostingType.DisplayName = BoostingControllerManager.GetInstance().GetCurrentEquipmentModel();        
                PLCType.DisplayName = PLCControllerManager.GetInstance().GetCurrentEquipmentModel();
                //DdetectorVersion.DisplayName
                IntPtr sb = ImageImportDll.SX_Version();
                DdetectorVersion.DisplayName = Marshal.PtrToStringAnsi(sb);

                #region MyRegion
                    EquipmentOnTime.DisplayName =PLCControllerManager.GetInstance().GetPlcBlockStr
              (ConfigServices.GetInstance().localConfigModel.EquipmentOnTImeAddress,
              Convert.ToUInt16(ConfigServices.GetInstance().localConfigModel.EquipmentOnTImeAddressLength), InqureType.IString);

                EquipmentOffTime.DisplayName= PLCControllerManager.GetInstance().GetPlcBlockStr
              (ConfigServices.GetInstance().localConfigModel.EquipmentOffTImeAddress, 
              Convert.ToUInt16(ConfigServices.GetInstance().localConfigModel.EquipmentOffTImeAddressLength), InqureType.IString);

                EquipmentPowerOnTotalDay.DisplayName = $@"{PLCControllerManager.GetInstance().GetPlcBlockStr
              (ConfigServices.GetInstance().localConfigModel.EquipmentTotalDayAddress,
              Convert.ToUInt16(ConfigServices.GetInstance().localConfigModel.EquipmentTotalDayAddressLength), InqureType.IUInt32)}{UpdateStatusNameAction("Days")}";

                EquipmentPowerOnTotalTime.DisplayName = CommonFunc.MillToHour(PLCControllerManager.GetInstance().GetPlcBlockStr
                (ConfigServices.GetInstance().localConfigModel.EquipmentTotalTimeAddress,
                Convert.ToUInt16(ConfigServices.GetInstance().localConfigModel.EquipmentTotalTimeAddressLength), InqureType.IUInt32));
                #endregion


                #region 背散设备

                #endregion
            }
        }
        public void SwitchFontSize(string FontSize)
        {
            SoftDetailsModules_Loaded(null, null);
        }
        public void SwitchLanguage(string language)
        {
            base.Base_SwitchLanguage(language);
            SoftDetailsModules_Loaded(null, null);
        }
        private static Grid MakeMainGrid()
        {
            Grid _MainGrid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Pixel) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20, GridUnitType.Star) });    
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(14, GridUnitType.Star) });
            return _MainGrid;
        }
        /// <summary>
        /// 设置Border
        /// </summary>
        private Border MakeSetButton(string tag)
        {
            Border BtnSendCommand = new Border()
            {
                Width = 100,
                Height = 30,
                Style = (Style)this.FindResource("diyBtnCarHand"),
                Tag = tag
            };
            Label lblContent = MakeLabel(tag);
            lblContent.Foreground = Brushes.White;
            BtnSendCommand.Child = lblContent;
            return BtnSendCommand;
        }
        /// <summary>
        /// 生成Label
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        protected Label MakeLabel(string Name)
        {
            Label lblMax = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85"),
                Content = UpdateStatusNameAction(Name)
            };
            lblMax.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            return lblMax;
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
                LastChildFill = true,
                Background = (LinearGradientBrush)this.FindResource("TitleBackGround"),
            };
            Border bd = new Border()
            {
                BorderThickness = new Thickness(0, 0, 0, 0),
                BorderBrush = (LinearGradientBrush)this.FindResource("LnearBorderColor"),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Stretch,
                Width = 32,
                Background = StrToBrush("#00FFFFFF")
            };
            bd.PreviewMouseLeftButtonDown += Bd_PreviewMouseLeftButtonDown;
            Canvas cs = new Canvas()
            {
                Style = (Style)this.FindResource("diyCloseCanvas"),
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 1, 10, 0)
            };
            bd.Child = cs;
            Label _lblTitle = new Label()
            {
                Content = GetName(),
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.Big),
                FontWeight = FontWeights.Bold,
                Foreground = StrToBrush("#FFFFFF"),
                FontFamily = new FontFamily("宋体"),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            dp.Children.Add(_lblTitle);
            dp.Children.Add(bd);
            Grid.SetColumn(dp, 0);
            Grid.SetRow(dp, 0);
            Grid.SetColumnSpan(dp, 5);
            _MainGrid.Children.Add(dp);
            return dp;
        }
        public override void Show(Window _OwnerWin)
        {
            _OwnerWin.ResizeMode = ResizeMode.CanResize;
            _OwnerWin.MaxWidth = GetWidth();
            _OwnerWin.MaxHeight = GetHeight();
            _OwnerWin.Title = GetName();
            _OwnerWin.Content = this;
            CurrentWindow = _OwnerWin;
            _OwnerWin.Show();
        }
        public override string GetName()
        {
            return UpdateStatusNameAction("SoftDetails");
        }
        public override bool IsConnectionEquipment()
        {
            return IsConnection;
        }
        public override double GetHeight()
        {
            return 600;
        }
        public override double GetWidth()
        {
            return 800;
        }
    }
}
