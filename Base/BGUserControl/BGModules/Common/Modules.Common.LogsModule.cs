using CMW.Common.Utilities;
using BGCommunication;
using BGDAL;
using BGModel;

using BGUserControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.CommonFunc;
using BG_Entities;
using BG_WorkFlow;

namespace BGUserControl
{
    [Export("ContentPage", typeof(BaseModules))]
    [CustomExportMetadata(1, Modules.LogsModule, "日志查看模块", "ZZW", "1.0.0")]
    public class LogsModule : BaseModules
    {
        Grid _MainGrid = new Grid();
        ParamaterModel<BG_Logs> LocalCondition;
        BG_Logs SearchLogModel = new BG_Logs();
        ObservableCollection<BG_Logs> Logs = new ObservableCollection<BG_Logs>();
        LogBLL llb = new LogBLL();
        List<int> numList = new List<int>() { 5, 10, 30, 100 };
        DataPager dp;
        DiyListView<BG_Logs> dlv;
        public LogsModule():base(controlVersion)
        {
            Loaded -= LogsModule_Loaded;
            Loaded += LogsModule_Loaded;
        }

        private void LogsModule_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ButtonInvoke.SwitchLanguageEvent -= SwitchLanguage;
            ButtonInvoke.SwitchLanguageEvent += SwitchLanguage;
            ButtonInvoke.SwitchFontSizeEvent -= SwitchFontSize;
            ButtonInvoke.SwitchFontSizeEvent += SwitchFontSize;
            _MainGrid = InitGrid();
            Content = _MainGrid;
        }

        private Grid InitGrid()
        {
            Grid _MainGrid = MakeMainGrid();
            InitContent(_MainGrid);
            return _MainGrid;
        }

        /// <summary>
        /// 刷新当前图片信息
        /// </summary>
        /// <param name="_LocalUploadImage"></param>
        public void ReflashObject()
        {
            dlv.Reflash(Logs);
        }

        private void InitContent(Grid MainGrid)
        {
            InitOnlineImage(MainGrid);
        }

        /// <summary>
        /// 构建日志列表组件
        /// </summary>
        private void InitOnlineImage(Grid MainGrid)
        {
            Grid OnlineGrid = MakeOnlineMainGrid();
            InitDataPager(OnlineGrid);
            InitDataListView(OnlineGrid);
            InitSearchConditionPanel(OnlineGrid);
            Grid.SetRow(OnlineGrid, 0);
            Grid.SetColumn(OnlineGrid, 0);
            MainGrid.Children.Add(OnlineGrid);
            dp.btnRefresh_MouseClick(null, null);
        }

        private Grid MakeOnlineMainGrid()
        {
            Grid gd = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch};
            gd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(45, GridUnitType.Pixel) });
            gd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            gd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Pixel) });
            gd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            return gd;
        }

        private void InitDataPager(Grid gd)
        {
            LocalCondition = new ParamaterModel<BG_Logs>(SearchLogModel);
            Border bd = new Border() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            dp = new DataPager(numList) { };
            dp.OnPageGetList += DataPager_OnPageGetList;
            bd.Child = dp;
            Grid.SetColumn(bd, 0);
            Grid.SetRow(bd, 2);
            gd.Children.Add(bd);
        }

        private void InitDataListView(Grid gd)
        {
            dlv = new DiyListView<BG_Logs>(Logs)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            dlv.DataContext = this;
            Grid.SetColumn(dlv, 0);
            Grid.SetRow(dlv, 1);
            gd.Children.Add(dlv);
        }

        private void InitSearchConditionPanel(Grid OnlineGrid)
        {
            Grid ConditionPanel = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch,VerticalAlignment = VerticalAlignment.Stretch,Height = 45 };
            Label lblLogsContent = MakeLabel("LogName");
            TextBox txtLogs = new TextBox()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 3, 3, 0),
                Height = 30,
                Width = 120
            };
            txtLogs.SetBinding(TextBox.TextProperty, new Binding("LogContent") { Source = SearchLogModel,Mode = BindingMode.TwoWay,UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            Label lblLogsStartDatatime = MakeLabel("DatetimeStart");
            DatePicker datePickerStartDatatime = new DatePicker()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 3, 3, 0),
                Height = 30,
                Width = 120,
                SelectedDate = DateTime.Now,
               
            };
            datePickerStartDatatime.SetBinding(DatePicker.SelectedDateProperty, new Binding("LogStartDataTime") { Source = SearchLogModel, Mode = BindingMode.TwoWay, StringFormat = "yyyyMMddHHmmss" });
            Label lblLogsEndDatatime = MakeLabel("DatetimeEnd");
            DatePicker datePickerEndDatatime = new DatePicker()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 3, 3, 0),
                Height = 30,
                Width = 120,
                SelectedDate = DateTime.Now,
            };
            datePickerEndDatatime.SetBinding(DatePicker.SelectedDateProperty, new Binding("LogEndDataTime") { Source = SearchLogModel, Mode = BindingMode.TwoWay,StringFormat = "yyyyMMddHHmmss" });
            Border bd = new Border()
            {
                Width = 50,
                Height = 30,
                Style = (Style)this.FindResource("diyBtnCarHand"),
            };
            Label lblSearch = MakeLabel("Search");
            lblSearch.Foreground = Brushes.White;
            bd.Child = lblSearch;
            ConditionPanel.RowDefinitions.Add(new RowDefinition() {Height = new GridLength(1,GridUnitType.Star) });
            ConditionPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(15, GridUnitType.Star) });
            ConditionPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(20, GridUnitType.Star) });
            ConditionPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(15, GridUnitType.Star) });
            ConditionPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(20, GridUnitType.Star) });
            ConditionPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(15, GridUnitType.Star) });
            ConditionPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(20, GridUnitType.Star) });
            ConditionPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(20, GridUnitType.Star) });
            ConditionPanel.Children.Add(lblLogsContent);
            ConditionPanel.Children.Add(txtLogs);
            ConditionPanel.Children.Add(lblLogsStartDatatime);
            ConditionPanel.Children.Add(datePickerStartDatatime);
            ConditionPanel.Children.Add(lblLogsEndDatatime);
            ConditionPanel.Children.Add(datePickerEndDatatime);
            ConditionPanel.Children.Add(bd);
            Grid.SetColumn(lblLogsContent,0); Grid.SetColumn(txtLogs, 1);
            Grid.SetColumn(lblLogsStartDatatime, 2); Grid.SetColumn(datePickerStartDatatime, 3);
            Grid.SetColumn(lblLogsEndDatatime, 4);  Grid.SetColumn(datePickerEndDatatime, 5);
            Grid.SetColumn(bd, 6);
            OnlineGrid.Children.Add(ConditionPanel);
            bd.MouseDown += Bd_MouseDown;
        }

        private void Bd_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                dp.btnRefresh_MouseClick(null, null);
            }
            catch (Exception ex)
            {
                WriteLogAction(ex.StackTrace,LogType.ApplicationError,true);
            }
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

        int DataPager_OnPageGetList(int start, int num, ref int count)
        {
            LocalCondition.num = num;
            LocalCondition.start = start-1;
            return SearchLogs(count);
        }

        private int SearchLogs(int value)
        {
           return SearchLogsAsync(value).Result;
        }

        private Task<int> SearchLogsAsync(int value)
        {
            return Task.Run(() => { value = llb.GetAllConut(SearchLogModel); 
                Logs = llb.GetLogs(LocalCondition);
                foreach (var Log in Logs)
                {
                    Log.LogFontSize = UpdateFontSizeAction(CMWFontSize.Normal);
                    Log.LogType = UpdateStatusNameAction(Log.LogType);
                }
                Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                {
                    dlv.Reflash(Logs);
                })); 
                return value;
            });
        }

        private Grid MakeMainGrid()
        {
            Grid _MainGrid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Star) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(7, GridUnitType.Star) });
            return _MainGrid;
        }

        public void SwitchFontSize(string fontsize)
        {
            LogsModule_Loaded(null, null);
        }

        public void SwitchLanguage(string language)
        {
            base.Base_SwitchLanguage(language);
            LogsModule_Loaded(null, null);
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

        public override string GetName()
        {
            return "BackupLog";
        }

        public override bool IsConnectionEquipment()
        {
            return IsConnection;
        }

        public override double GetHeight()
        {
             return 500;
        }

        public override double GetWidth()
        {
            if (ConfigServices.GetInstance().localConfigModel.TrialImageMode.ToLower() == "local")
            {
                return 400;
            }
            else { return 800; }
        }
    }
}
