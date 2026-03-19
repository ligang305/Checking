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
using System.Collections.ObjectModel;

namespace BGUserControl
{
    [Export("ContentPage", typeof(BaseModules))]
    [CustomExportMetadata(1, Modules.StandardStatusPanel, "标准化显示状态信息", "ZZW", "1.0.0")]
    public class StandardStatusPanel : BaseModules
    {
        StandardStatusPanelViewModel standardStatusPanelViewModel = new StandardStatusPanelViewModel();
        Grid _MainGrid = new Grid();
        Grid IPositionGrid = new Grid();GroupBox IGB = new GroupBox();
        Grid OPositionGrid = new Grid(); GroupBox OGB = new GroupBox();
        Grid IntPositionGrid = new Grid(); GroupBox IntGB = new GroupBox();
        Grid DIntPositionGrid = new Grid(); GroupBox DIntGB = new GroupBox();
        Grid FloatPositionGrid = new Grid(); GroupBox FloatGB = new GroupBox();
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
        public StandardStatusPanel():base(controlVersion)
        {
            DataContext = standardStatusPanelViewModel;
            Loaded += StandardStatusPanel_Loaded;
        }
        private Grid InitGrid()
        {
            Grid _MainGrid = MakeMainGrid();
            //DockPanel dp = InitTitle(_MainGrid);
            //dp.MouseLeftButtonDown += Dp_MouseDown;
            Grid ContentGrid = InitContent(_MainGrid);
            PlcDbStatus();
            return _MainGrid;
        }
        private Grid InitContent(Grid MainGrid)
        {
            ScrollViewer scrollViewer = new ScrollViewer();
            IPositionGrid = new Grid();OPositionGrid = new Grid();
            IntPositionGrid = new Grid();FloatPositionGrid = new Grid();
            DIntPositionGrid = new Grid();
            IGB = new GroupBox();OGB = new GroupBox();
            IntGB = new GroupBox();FloatGB = new GroupBox();
            DIntGB = new GroupBox(); 
            Grid RowGd = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            for (int i = 0; i < 4; i++)
            {
                RowGd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            }
            RowGd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            RowGd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            RowGd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            RowGd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            RowGd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            for (int i = 0; i < 160; i++)
            {
                IPositionGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Pixel) });
            }
            for (int i = 0; i < 160; i++)
            {
                OPositionGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Pixel) });
            }
            for (int i = 0; i < 20; i++)
            {
                IntPositionGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Pixel) });
            }
            for (int i = 0; i < 20; i++)
            {
                DIntPositionGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Pixel) });
            }
            
            for (int i = 0; i < 20; i++)
            {
                FloatPositionGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Pixel) });
            }
            IGB.Header = new Label() { Content = UpdateStatusNameAction("I Position"), Foreground = StrToBrush("#3F96E6"), FontWeight = FontWeights.Bold };
            OGB.Header = new Label() { Content = UpdateStatusNameAction("O Position"), Foreground = StrToBrush("#3F96E6"), FontWeight = FontWeights.Bold }; 
            IntGB.Header = new Label() { Content = UpdateStatusNameAction("Int Position"), Foreground = StrToBrush("#3F96E6"), FontWeight = FontWeights.Bold };
            FloatGB.Header = new Label() { Content = UpdateStatusNameAction("Float Position"), Foreground = StrToBrush("#3F96E6"), FontWeight = FontWeights.Bold }; 
            DIntGB.Header = new Label() { Content = UpdateStatusNameAction("DInt Position"), Foreground = StrToBrush("#3F96E6"), FontWeight = FontWeights.Bold }; 
            IGB.Content = IPositionGrid;
            DIntGB.Content = DIntPositionGrid;
            OGB.Content = OPositionGrid;
            IntGB.Content = IntPositionGrid;
            FloatGB.Content = FloatPositionGrid;
            Grid.SetRow(IGB, 0); Grid.SetRow(OGB, 0); Grid.SetRow(IntGB, 0); Grid.SetRow(FloatGB, 0); Grid.SetRow(DIntGB, 0);
            Grid.SetColumn(IGB, 0); Grid.SetColumn(OGB, 1); Grid.SetColumn(IntGB, 2); Grid.SetColumn(DIntGB, 3); Grid.SetColumn(FloatGB,4);
            RowGd.Children.Add(IGB);
            RowGd.Children.Add(OGB);
            RowGd.Children.Add(IntGB);
            RowGd.Children.Add(DIntGB);
            RowGd.Children.Add(FloatGB);
            scrollViewer.Content = RowGd;
            MainGrid.Children.Add(scrollViewer);
            Grid.SetColumn(scrollViewer, 0);
            Grid.SetRow(scrollViewer, 1);
            return RowGd;
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
            //bd.PreviewMouseLeftButtonDown += Bd_PreviewMouseLeftButtonDown;
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

        private void PlcDbStatus()
        {
            for (int i = 0; i < standardStatusPanelViewModel.IPostitions.Length; i++)
            {
                SetDBStatusToContentGrid(0,i,  "IPosition", standardStatusPanelViewModel.IPostitions[i]);
            }
            for (int i = 0; i < standardStatusPanelViewModel.OPostitions.Length; i++)
            {
                SetDBStatusToContentGrid(1, i, "OPosition", standardStatusPanelViewModel.OPostitions[i]);
            }
            for (int i = 0; i < standardStatusPanelViewModel.IntPositions.Length; i++)
            {
                SetDBStatusToContentGrid(2, i, "IntPosition", standardStatusPanelViewModel.IntPositions[i]);
            }
            for (int i = 0; i < standardStatusPanelViewModel.FloatPositions.Length; i++)
            {
                SetDBStatusToContentGrid(3, i, "FloatPosition", standardStatusPanelViewModel.FloatPositions[i]);
            }
            for (int i = 0; i < standardStatusPanelViewModel.FloatPositions.Length; i++)
            {
                SetDBStatusToContentGrid(3, i, "DIntPosition", standardStatusPanelViewModel.DIntPositions[i]);
            }
        }
        private void SetDBStatusToContentGrid(int ColumnIndex,int RowIndex,string Type,object value)
        {
            switch (Type)
            {
                case "IPosition":
                    string ITitle = $@"{RowIndex/8}.{RowIndex%8}";
                    Border border = InitBorder($@"I {ITitle}", value);
                    Grid.SetColumn(border, ColumnIndex);
                    Grid.SetRow(border, RowIndex);
                    IPositionGrid.Children.Add(border);
                    break;
                case "OPosition":
                    string OTitle = $@"{(RowIndex) / 8}.{(RowIndex + 160) % 8}";
                    Border OBorder = InitBorder($@"O {OTitle}", value);
                    Grid.SetColumn(OBorder, ColumnIndex);
                    Grid.SetRow(OBorder, RowIndex);
                    OPositionGrid.Children.Add(OBorder);
                    break;
                case "IntPosition":
                    Border IntBorder = InitLabel(RowIndex.ToString(), value);
                    Grid.SetColumn(IntBorder, ColumnIndex);
                    Grid.SetRow(IntBorder, RowIndex);
                    IntPositionGrid.Children.Add(IntBorder);
                    break;
                case "DIntPosition":
                    Border DIntBorder = InitLabel(RowIndex.ToString(), value);
                    Grid.SetColumn(DIntBorder, ColumnIndex);
                    Grid.SetRow(DIntBorder, RowIndex);
                    DIntPositionGrid.Children.Add(DIntBorder);
                    break;
                    
                case "FloatPosition":
                    Border FloatBorder = InitLabel(RowIndex.ToString(), value);
                    Grid.SetColumn(FloatBorder, ColumnIndex);
                    Grid.SetRow(FloatBorder, RowIndex);
                    FloatPositionGrid.Children.Add(FloatBorder);
                    break;
                default:
                    break;
            }
        }

        private Border InitLabel(string labelType,object label)
        {
            Border border = new Border()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            DockPanel stackPanel = new DockPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                LastChildFill = true,
            };
            Label lblPosition = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                Content = labelType,
                Style = (Style)this.FindResource("diyLabel"),
                FontSize = UpdateFontSizeAction(CMWFontSize.NormalMiddle),
            };
            Label lblPostitionValue = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Stretch,
                Style = (Style)this.FindResource("diyLabel"),
                FontSize = UpdateFontSizeAction(CMWFontSize.NormalMiddle),
            };
            lblPostitionValue.SetBinding(Label.ContentProperty,new Binding("StatusCode") { Source = label });
            DockPanel.SetDock(lblPosition,Dock.Left);
            DockPanel.SetDock(lblPostitionValue, Dock.Right);
            stackPanel.Children.Add(lblPosition);
            stackPanel.Children.Add(lblPostitionValue);
            border.Child = stackPanel;
            return border;
        }
        private Border InitBorder(string labelType, object label)
        {
            Border border = new Border()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            DockPanel stackPanel = new DockPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                LastChildFill = true,
            };
            TextBlock lblName = new TextBlock()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = UpdateFontSizeAction(CMWFontSize.NormalMiddle),
                MaxWidth = 120,
                FontWeight = FontWeights.Bold,
                Foreground = StrToBrush("#3F96E6")
            };
            lblName.SetBinding(TextBlock.TextProperty, new Binding("StatusDisplayName") { Source = label });
            lblName.SetBinding(TextBlock.ToolTipProperty, new Binding("StatusDisplayName") { Source = label });
            Label lblPosition = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                Content = labelType,
                Style = (Style)this.FindResource("diyLabel"),
                FontSize = UpdateFontSizeAction(CMWFontSize.NormalMiddle),
            };
            Border BorderPostitionValue = new Border()
            {
                Width = 24,
                CornerRadius = new CornerRadius(12),
                BorderBrush = Brushes.Gray,
                HorizontalAlignment = HorizontalAlignment.Right,
                BorderThickness = new Thickness(2),
                Height = 24,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            BorderPostitionValue.SetBinding(Border.BackgroundProperty, new Binding("StatusCode") { Source = label, Converter = new ColorConvert() }); ;
            DockPanel.SetDock(lblPosition, Dock.Left);
            DockPanel.SetDock(lblName, Dock.Left);
            DockPanel.SetDock(BorderPostitionValue, Dock.Right);
            stackPanel.Children.Add(lblPosition);
            stackPanel.Children.Add(lblName);
            stackPanel.Children.Add(BorderPostitionValue);
            border.Child = stackPanel;
            return border;
        }
        private void StandardStatusPanel_Loaded(object sender, RoutedEventArgs e)
        {
            ButtonInvoke.SwitchLanguageEvent -= SwitchLanguage;
            ButtonInvoke.SwitchLanguageEvent += SwitchLanguage;
            ButtonInvoke.SwitchFontSizeEvent -= SwitchFontSize;
            ButtonInvoke.SwitchFontSizeEvent += SwitchFontSize;
            _MainGrid = InitGrid();
            //_MainBorder.Child = _MainGrid;
            Content = _MainGrid;
        }

        public void SwitchFontSize(string FontSize)
        {
            StandardStatusPanel_Loaded(null, null);
        }

        public void SwitchLanguage(string language)
        {
            base.Base_SwitchLanguage(language);
            StandardStatusPanel_Loaded(null, null);
        }

        private static Grid MakeMainGrid()
        {
            Grid _MainGrid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = StrToBrush("#FFFFFF"),
            };
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Auto) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20, GridUnitType.Star) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            return _MainGrid;
        }

        public override string GetName()
        {
            return UpdateStatusNameAction("StandardStatusPanel");
        }
        public override void Show(Window _OwnerWin)
        {
            _OwnerWin.ResizeMode = ResizeMode.CanResize;
            _OwnerWin.Title = GetName();
            _OwnerWin.Content = this;
            CurrentWindow = _OwnerWin;
            _OwnerWin.Show();
        }
    }

    public class StandardStatusPanelViewModel : BaseMvvm
    {
        PLCDBStatus _PLCDBStatus = new PLCDBStatus();

        public PLCDBStatus PLCDBStatus
        {
            get => _PLCDBStatus;
            set { _PLCDBStatus = value; RaisePropertyChanged("PLCDBStatus"); }
        }
        public StatusModel[] IPostitions = new StatusModel[160];
        public StatusModel[] OPostitions = new StatusModel[160];
        public StatusModel[] DIntPositions = new StatusModel[20];
        public StatusModel[] IntPositions = new StatusModel[20];
        public StatusModel[] FloatPositions = new StatusModel[20];

        public StandardStatusPanelViewModel()
        {
            InitPosisions();
        }

        private void InitPosisions()
        {
            var IPositionLists = PositionBLL.GetInstance().GetPositionModel(controlVersion, PositionConfigType.IPosition);
            IPositionLists.ForEach(q => q.StatusDisplayName = UpdateStatusNameAction(q.StatusName));
            IPostitions = IPositionLists.ToArray();

            var OPositionLists = PositionBLL.GetInstance().GetPositionModel(controlVersion, PositionConfigType.OPosition);
            OPositionLists.ForEach(q => q.StatusDisplayName = UpdateStatusNameAction(q.StatusName));
            OPostitions = OPositionLists.ToArray();

            var DIntPositionsLists = PositionBLL.GetInstance().GetPositionModel(controlVersion, PositionConfigType.DIntPosition);
            DIntPositionsLists.ForEach(q => q.StatusDisplayName = UpdateStatusNameAction(q.StatusName));
            DIntPositions = DIntPositionsLists.ToArray();

            var IntPositionsLists = PositionBLL.GetInstance().GetPositionModel(controlVersion, PositionConfigType.IntPosition);
            IntPositionsLists.ForEach(q => q.StatusDisplayName = UpdateStatusNameAction(q.StatusName));
            IntPositions = IntPositionsLists.ToArray();

            var FloatPositionsLists = PositionBLL.GetInstance().GetPositionModel(controlVersion, PositionConfigType.FloatPosition);
            FloatPositionsLists.ForEach(q => q.StatusDisplayName = UpdateStatusNameAction(q.StatusName));
            FloatPositions = FloatPositionsLists.ToArray();
        }

        public override void LoadUIText()
        {
            if (IPostitions.Length != 0)
            {
                var tempIPosition = IPostitions.ToList();
                tempIPosition.ForEach(q => q.StatusDisplayName = UpdateStatusNameAction(q.StatusName));
                IPostitions = tempIPosition.ToArray();
            }
            if (OPostitions.Length != 0)
            {
                var tempOPosition = OPostitions.ToList();
                tempOPosition.ForEach(q => q.StatusDisplayName = UpdateStatusNameAction(q.StatusName));
                OPostitions = tempOPosition.ToArray();
            }
            if (IntPositions.Length != 0)
            {
                var tempIntPosition = IntPositions.ToList();
                tempIntPosition.ForEach(q => q.StatusDisplayName = UpdateStatusNameAction(q.StatusName));
                IntPositions = tempIntPosition.ToArray();
            }
            if (DIntPositions.Length != 0)
            {
                var tempDIntPosition = DIntPositions.ToList();
                tempDIntPosition.ForEach(q => q.StatusDisplayName = UpdateStatusNameAction(q.StatusName));
                DIntPositions = tempDIntPosition.ToArray();
            }
            if (FloatPositions.Length != 0)
            {
                var tempFloatPosition = FloatPositions.ToList();
                tempFloatPosition.ForEach(q => q.StatusDisplayName = UpdateStatusNameAction(q.StatusName));
                FloatPositions = tempFloatPosition.ToArray();
            }
        }

        public override void LoadUIFontSize()
        {
            base.LoadUIFontSize();
        }

        protected override void InquirePlcStandardStatus(PLCDBStatus _PLCDBStatus)
        {
            PLCDBStatus = _PLCDBStatus;
            ConvertPlcToCommonSettingModel(_PLCDBStatus);
        }

        private void ConvertPlcToCommonSettingModel(PLCDBStatus _PLCDBStatus)
        {
            if (IPostitions.Count() != 0)
            {
                for (int i = 0; i < PLCDBStatus.IPositions.Count; i++)
                {
                    if (IPostitions[i] == null) IPostitions[i] = new StatusModel();
                    IPostitions[i].StatusCode = PLCDBStatus.IPositions[i].ToString();
                }
            }

            if (OPostitions.Count() != 0)
            {
                for (int i = 0; i < PLCDBStatus.OPositions.Count; i++)
                {
                    if (OPostitions[i] == null) OPostitions[i] = new StatusModel();
                    OPostitions[i].StatusCode = PLCDBStatus.OPositions[i].ToString();
                }
            }

            if (DIntPositions.Count() != 0)
            {
                for (int i = 0; i < PLCDBStatus.DIntArray.Count; i++)
                {
                    if (DIntPositions[i] == null) DIntPositions[i] = new StatusModel();
                    DIntPositions[i].StatusCode = PLCDBStatus.DIntArray[i].ToString();
                }
            }


            if (IntPositions.Count() != 0)
            {
                for (int i = 0; i < PLCDBStatus.IntArray.Count; i++)
                {
                    if (IntPositions[i] == null) IntPositions[i] = new StatusModel();
                    IntPositions[i].StatusCode = PLCDBStatus.IntArray[i].ToString();
                }

            }
            if (FloatPositions.Count() != 0)
            {

                for (int i = 0; i < PLCDBStatus.FloatArray.Count; i++)
                {
                    if (FloatPositions[i] == null) FloatPositions[i] = new StatusModel();
                    FloatPositions[i].StatusCode = PLCDBStatus.FloatArray[i].ToString();
                }
            }
        }
    }
}
