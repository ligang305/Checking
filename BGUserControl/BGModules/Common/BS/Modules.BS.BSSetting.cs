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
using System.ComponentModel.Composition.Primitives;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using TextBox = System.Windows.Controls.TextBox;

namespace BGUserControl
{
    [Export("ContentPage", typeof(BaseModules))]
    [Export("BS_ContentPage", typeof(BaseModules))]
    [CustomExportMetadata(1, Modules.BS2000SettingModule, "背散探测器设备连接设置", "ZZW", "1.0.0")]
    public class BSSetting : BSBaseModules
    {
        Grid _MainGrid = new Grid();
        Border _MainBorder = new Border()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
        };
        BSSettingMvvm BSSettingMvvm = new BSSettingMvvm();


        [ImportingConstructor]
        public BSSetting() : base(ControlVersion.BS)
        {
            Loaded += BSSetting_Loaded;
            DataContext = BSSettingMvvm;
            BSSettingMvvm.SwitchLanguageAction -= SwitchLanguage;
            BSSettingMvvm.SwitchLanguageAction += SwitchLanguage;
        }
        private void BSSetting_Loaded(object sender, RoutedEventArgs e)
        {
            InitContent();
        }
        /// <summary>
        /// 初始化内容接口
        /// </summary>
        private void InitContent()
        {
            BSSettingMvvm.InitListData();
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
            MakeAddButton(_MainGrid);
            foreach (BG_BS2000 BSItem in BSSettingMvvm.BS2000List)
            {
                int index = BSSettingMvvm.BS2000List.IndexOf(BSItem) + 1;
                Grid _gd = MakeChildPanel(BSItem, index);
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
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Star) });
            for (int i = 0; i < BSSettingMvvm.BS2000List.Count; i++)
            {
                _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Star) });
            }
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Pixel) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(95, GridUnitType.Star) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            return _MainGrid;
        }

        private Grid MakeAddButton(Grid MainGrid)
        {
            Grid _AddGrid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            Border SettingButton = MakeAddButton("Add");
            SettingButton.Width = 120;
            SettingButton.Height = 40;
            SettingButton.HorizontalAlignment = HorizontalAlignment.Left;
            SettingButton.MouseDown += NewButton_MouseDown;
            _AddGrid.Children.Add(SettingButton);
            Grid.SetColumn(_AddGrid,0);
            Grid.SetColumnSpan(_AddGrid,2);
            Grid.SetRow(_AddGrid,0);
            MainGrid.Children.Add(_AddGrid);
            return _AddGrid;
        }

        private void NewButton_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Task.Run(() => {
                BG_BS2000 bG_BS2000 = new BG_BS2000()
                {
                    ID = CommonFunc.GetGuid(),
                    IPAddress = "127.0.0.1",
                    CommandPort = "3000",
                    DataPort = "4001",
                    ViewCount = "1"
                };
                BS2000BLL.GetInstance().InsertBSModel(bG_BS2000);
                Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                    InitContent();
                }));
            });
        }

        /// <summary>
        /// 生成单行的子空间面板
        /// </summary>
        /// <param name="CarSpeedItem"></param>
        /// <param name="Index"></param>
        /// <returns></returns>
        public Grid MakeChildPanel(BG_BS2000 BSItem, int Index)
        {
            Grid MainGrid = MakeGrid();
            Label lblIndex = MakeLabel("Index");
            lblIndex.Content += Index.ToString();
            Label lblAddress = MakeLabel("LinkAddress");
            TextBox tbAddress = MakeTextBox(BSItem, "IPAddress");
            Label lblCommandPort = MakeLabel("LinkPort");
            TextBox tbCommandPort = MakeTextBox(BSItem, "CommandPort");
            tbCommandPort.Tag = Index;
            tbCommandPort.Width = 60;
            Label lblDataPort = MakeLabel("DataPort");
            TextBox tbDataPort = MakeTextBox(BSItem, "DataPort");
            tbDataPort.Width = 60;
            Label lblRemark = MakeLabel("Remark");
            TextBox tbRemark = MakeTextBox(BSItem, "Remark");
            tbRemark.Width = 100;
            Label lblViewCount = MakeLabel("ViewCount");
            TextBox tbViewCount = MakeTextBox(BSItem, "ViewCount");
            tbViewCount.Width = 50;
            Border SettingButton = MakeAddButton("Setting");
            SettingButton.MouseDown += SettingButton_MouseDown;
            SettingButton.Tag = BSItem;

            Border DeleteButton = MakeAddButton("Delete");
            DeleteButton.MouseDown += DeleteButton_MouseDown;
            DeleteButton.Tag = BSItem;

            AddElementToGrid(MainGrid, lblIndex, 0, 0);
            AddElementToGrid(MainGrid, lblAddress, 0, 1);
            AddElementToGrid(MainGrid, tbAddress, 0, 2);
            AddElementToGrid(MainGrid, lblCommandPort, 0, 3);
            AddElementToGrid(MainGrid, tbCommandPort, 0, 4);
            AddElementToGrid(MainGrid, lblDataPort, 0, 5);
            AddElementToGrid(MainGrid, tbDataPort, 0, 6);
            AddElementToGrid(MainGrid, lblRemark, 0, 7);
            AddElementToGrid(MainGrid, tbRemark, 0, 8);
            AddElementToGrid(MainGrid, lblViewCount, 0, 9);
            AddElementToGrid(MainGrid, tbViewCount, 0, 10);
            AddElementToGrid(MainGrid, SettingButton, 0, 11);
            AddElementToGrid(MainGrid, DeleteButton, 0, 12);
            return MainGrid;
        }
        private void DeleteButton_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (MessageBox.Show(UpdateStatusNameAction("BSDelete"), UpdateStatusNameAction("Tip"), MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    BG_BS2000 bsItem = (sender as Border).Tag as BG_BS2000;
                    BS2000BLL.GetInstance().DeleteBSModel(bsItem);
                        InitContent();
                }
            }
            catch (Exception ex)
            {
                CommonDeleget.WriteLogAction(ex.Message, LogType.ApplicationError, true);
            }
        }
        private void SettingButton_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                BG_BS2000 bsItem = (sender as Border).Tag as BG_BS2000;
                if(!string.IsNullOrEmpty(bsItem.Error))
                {
                    BG_MESSAGEBOX.Show(Application.Current?.MainWindow, UpdateStatusNameAction("Tip"), UpdateStatusNameAction("ErrorInfo"));
                    return;
                }
                BS2000BLL.GetInstance().SaveBSModel(bsItem);
                if (MessageBox.Show(UpdateStatusNameAction("NeedRestart"), UpdateStatusNameAction("Tip"), MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
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
        private TextBox MakeTextBox(BG_BS2000 BS2000View, string BindAtt)
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
                Source = BS2000View,
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
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100, GridUnitType.Pixel) });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(80, GridUnitType.Pixel) });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(20, GridUnitType.Star) });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(80, GridUnitType.Pixel) });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10, GridUnitType.Star) });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(80, GridUnitType.Pixel) });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10, GridUnitType.Star) });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(80, GridUnitType.Pixel) });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10, GridUnitType.Star) });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(80, GridUnitType.Pixel) });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70, GridUnitType.Pixel) });
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
            return "BSViewControl";
        }
    }

    public class BSSettingMvvm : BaseMvvm
    {
        public ObservableCollection<BG_BS2000> BS2000List = new ObservableCollection<BG_BS2000>();
        public BSSettingMvvm()
        {
            
        }

        /// <summary>
        /// 初始化列表
        /// </summary>
        public void InitListData()
        {
            BS2000List.Clear();
            BS2000BLL.GetInstance().GetBSModel().ForEach(q => BS2000List.Add(q));
        }
        public override void LoadUIText()
        {
            SwitchLanguageAction?.Invoke();
        }
    }
}
