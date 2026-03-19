using CMW.Common.Utilities;
using BGCommunication;
using BGModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.CommonFunc;
using static CMW.Common.Utilities.Common;
using System.Windows.Data;
using BGUserControl;
using BGDAL;
using BG_Services;
using BG_Entities;
using BG_WorkFlow;

namespace BGUserControl
{
    [Export("ContentPage", typeof(BaseModules))]
    [CustomExportMetadata(1, Modules.DoseSettingModule, "剂量连接设置", "ZZW", "1.0.0")]
    public class DoseSetting : BaseModules
    {
        Grid _MainGrid = new Grid();
        Border _MainBorder = new Border()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
        };
        DoseSettingMvvm doseSettingMvvm = new DoseSettingMvvm();
        [ImportingConstructor]
        public DoseSetting() : base(Common.controlVersion)
        {
            Loaded += DoseSetting_Loaded;
            DataContext = doseSettingMvvm;
            doseSettingMvvm.SwitchLanguageAction -= SwitchLanguage;
            doseSettingMvvm.SwitchLanguageAction += SwitchLanguage;
        }
        private void DoseSetting_Loaded(object sender, RoutedEventArgs e)
        {
            InitContent();
        }
        /// <summary>
        /// 初始化内容接口
        /// </summary>
        private void InitContent()
        {
            doseSettingMvvm.InitListData();
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
            _MainGrid = MakeMainGrid();
            foreach (Bg_Dose DoseItem in doseSettingMvvm.DoseList)
            {
                int index = doseSettingMvvm.DoseList.IndexOf(DoseItem) + 1;
                Grid _gd = MakeChildPanel(DoseItem, index);
                Grid.SetRow(_gd, index);
                Grid.SetColumn(_gd, 0);
                _MainGrid.Children.Add(_gd);
            }

            return _MainGrid;
        }
        /// <summary>
        /// 设置主Grid
        /// </summary>
        /// <returns></returns>
        private Grid MakeMainGrid()
        {
            Grid _MainGrid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Pixel) });
            for (int i = 0; i < doseSettingMvvm.DoseList.Count; i++)
            {
                _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Star) });
            }
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Star) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Pixel) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(95, GridUnitType.Star) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            return _MainGrid;
        }
        /// <summary>
        /// 生成单行的子空间面板
        /// </summary>
        /// <param name="CarSpeedItem"></param>
        /// <param name="Index"></param>
        /// <returns></returns>
        public Grid MakeChildPanel(Bg_Dose DoseItem, int Index)
        {
            Grid MainGrid = MakeGrid();
            Label lblCarSpeedIndex = MakeLabel("Index");
            lblCarSpeedIndex.Content += Index.ToString();
            Label lblAddress = MakeLabel("LinkAddress");
            TextBox tbAddress = MakeTextBox(DoseItem, "IPAddress");
            Label lblPort = MakeLabel("LinkPort");
            TextBox tbPort = MakeTextBox(DoseItem, "Port");
            tbPort.Tag = Index;
            Border SettingButton = MakeAddButton("Setting");
            SettingButton.MouseDown += SettingButton_MouseDown;
            SettingButton.Tag = DoseItem;
            AddElementToGrid(MainGrid, lblCarSpeedIndex, 0, 0);
            AddElementToGrid(MainGrid, lblAddress, 0, 1);
            AddElementToGrid(MainGrid, tbAddress, 0, 2);
            AddElementToGrid(MainGrid, lblPort, 0, 3);
            AddElementToGrid(MainGrid, tbPort, 0, 4);
            AddElementToGrid(MainGrid, SettingButton, 0, 5);
            return MainGrid;
        }
        private void SettingButton_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                Bg_Dose DoseItem = (sender as Border).Tag as Bg_Dose;
                if(!string.IsNullOrEmpty(DoseItem.Error))
                {
                    BG_MESSAGEBOX.Show(Application.Current?.MainWindow, UpdateStatusNameAction("Tip"), UpdateStatusNameAction("ErrorInfo"));
                    return;
                }
                if (MessageBox.Show(UpdateStatusNameAction("NeedRestart"), UpdateStatusNameAction("Tip"), MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    DoseBLL.GetInstance().SaveDoseModel(DoseItem);
                    Application.Current?.Shutdown();
                    RestartSystemEvent();
                }
            }
            catch (Exception ex)
            {
                CommonDeleget.WriteLogAction(ex.Message, LogType.ApplicationError, true);
            }
        }
        /// <summary>
        /// 向表格中添加元素
        /// </summary>
        /// <param name="ParentGrid">父容器</param>
        /// <param name="CurrentFFE">添加的元素</param>
        /// <param name="Row">行</param>
        /// <param name="Column">列</param>
        /// <param name="RowSpan">行合并</param>
        /// <param name="ColumnSpan">列合并</param>
        private void AddElementToGrid(Grid ParentGrid, FrameworkElement CurrentFFE, int Row, int Column, int RowSpan = 1, int ColumnSpan = 1)
        {
            ParentGrid.Children.Add(CurrentFFE);
            Grid.SetColumn(CurrentFFE, Column);
            Grid.SetRow(CurrentFFE, Row);
            Grid.SetColumnSpan(CurrentFFE, ColumnSpan);
            Grid.SetRowSpan(CurrentFFE, RowSpan);
        }
        /// <summary>
        /// 生成TextBox
        /// </summary>
        /// <param name="CarSpeed"></param>
        /// <param name="BindAtt"></param>
        /// <returns></returns>
        private TextBox MakeTextBox(Bg_Dose CarSpeed, string BindAtt)
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
            txtBox.SetBinding(TextBox.TextProperty, new Binding(BindAtt)
            {
                Source = CarSpeed,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                ValidatesOnExceptions = true,
                NotifyOnValidationError = true,
                ValidatesOnDataErrors = true,
                Mode = BindingMode.TwoWay
            });
            txtBox.SetBinding(TextBox.ToolTipProperty, new Binding("(Validation.Errors)[0].ErrorContent") { RelativeSource = RelativeSource.Self });
            return txtBox;
        }
        /// <summary>
        /// 生成单行表格
        /// </summary>
        /// <returns></returns>
        private Grid MakeGrid()
        {
            Grid MainGrid = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30, GridUnitType.Star) });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(40, GridUnitType.Star) });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(20, GridUnitType.Star) });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(20, GridUnitType.Star) });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(20, GridUnitType.Star) });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70, GridUnitType.Pixel) });
            return MainGrid;
        }
        public void SwitchLanguage()
        {
            InitContent();
        }

        /// <summary>
        /// 获取名称
        /// </summary>
        /// <returns></returns>
        public override string GetName()
        {
            return "DoseControl";
        }
    }

    public class DoseSettingMvvm : BaseMvvm
    {
        public ObservableCollection<Bg_Dose> DoseList = new ObservableCollection<Bg_Dose>();
        public DoseSettingMvvm()
        {
            DetecotrControllerManager.GetInstance().DetecotorConnectionAction -= DetecotorConnection;
            DetecotrControllerManager.GetInstance().DetecotorConnectionAction += DetecotorConnection;
        }

        /// <summary>
        /// 初始化列表
        /// </summary>
        public void InitListData()
        {
            DoseList.Clear();
            DoseBLL.GetInstance().GetDoseModel().ForEach(q => DoseList.Add(q));
        }
        public override void LoadUIText()
        {
            SwitchLanguageAction?.Invoke();
        }
        protected override void InquirePlcStatus(List<bool> StatusList)
        {

        }
        protected override void ConnectionStatus(bool ConnectionStatus)
        {

        }
        public void DetecotorConnection(bool DetecotrConnection)
        {
        }
    }
}
