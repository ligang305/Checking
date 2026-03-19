using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BGModel;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using BGCommunication;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using BGUserControl;
using System.Threading;
using BGDAL;
using BG_Services;
using BG_WorkFlow;
using CMW.Common.Utilities;
using BG_Entities;
using static CMW.Common.Utilities.CommonDeleget;
namespace BGUserControl
{
    [Export("ContentPage", typeof(BaseModules))]
    [CustomExportMetadata(1,Modules.FastCheck_CarSpeedFreezeModule, "控制车速模块", "ZZW", "1.0.0")]
    public class FastCheck_CarSpeedSetting : BaseModules
    {
        Grid _MainGrid = new Grid();
        ScrollViewer _MainBorder = new ScrollViewer()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
        };
        ObservableCollection<Bg_Carspeed> Bg_CarspeedList = new ObservableCollection<Bg_Carspeed>();
        [ImportingConstructor]
        public FastCheck_CarSpeedSetting() : base(ControlVersion.FastCheck)
        {
            Loaded -= FastCheck_FlowSetting_Loaded;
            Loaded += FastCheck_FlowSetting_Loaded;
            Unloaded -= FastCheck_FlowSetting_Unloaded;
            Unloaded += FastCheck_FlowSetting_Unloaded;
        }

        private void FastCheck_FlowSetting_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
            }
        }

        private void FastCheck_FlowSetting_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            base.Base_SwitchLanguage(ConfigServices.GetInstance().localConfigModel.LANGUAGE);
            
            InitContent();
        }
        private void InitListData()
        {
            //Task.Run(() =>
            //{
                Bg_CarspeedList.Clear();
                CarSpeedBLL.GetInstance().GetCarSpeedList().Where(q => q.BayNumber == ConfigServices.GetInstance().localConfigModel.CMW_FastCheckBayNumber).ToList()
                    .ForEach(q => Bg_CarspeedList.Add(q));
            //});
        }
        private void InitContent()
        {
            _MainGrid = InitGrid();
            _MainBorder.Content = _MainGrid;
            Content = _MainBorder;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;

            VerticalContentAlignment = VerticalAlignment.Stretch;
            InitListData();
        }
        private Grid InitGrid()
        {
            Grid _MainGrid = MakeMainGrid();
            Grid BayGrid = MakeTCQBayNumber();
            Grid.SetColumn(BayGrid,0); Grid.SetRow(BayGrid, 0);
            _MainGrid.Children.Add(BayGrid);

            foreach (Bg_Carspeed CarSpeedItem in Bg_CarspeedList)
            {
                int index = Bg_CarspeedList.IndexOf(CarSpeedItem) + 1;
                Grid _gd = MakeChildPanel(CarSpeedItem, index);
                Grid.SetRow(_gd, index);
                Grid.SetColumn(_gd, 0);
                _MainGrid.Children.Add(_gd);
            }

            return _MainGrid;
        }
     


        /// <summary>
        /// 生成单行的子空间面板
        /// </summary>
        /// <param name="CarSpeedItem"></param>
        /// <param name="Index"></param>
        /// <returns></returns>
        public Grid MakeChildPanel(Bg_Carspeed CarSpeedItem,int Index)
        {
            Grid MainGrid = MakeGrid();
            Label lblCarSpeedIndex = MakeLabel("Index");
            lblCarSpeedIndex.Content += Index.ToString();
            Label lblMin = MakeLabel("Min");
            TextBox tbMin = MakeTextBox(CarSpeedItem, "SpeedMin");
            Label lblMax = MakeLabel("Max");
            TextBox tbMax = MakeTextBox(CarSpeedItem, "SpeedMax");
            tbMax.Tag = Index;
            tbMax.TextChanged += TbMax_TextChanged;
            Label lblFreeze = MakeLabel("FrequencySetting");
            TextBox tbFreeze = MakeTextBox(CarSpeedItem, "Freez");
            Border SaveButton = MakeAddButton("Save");
            Border DeleteButton = MakeAddButton("Delete");
            DeleteButton.Tag = CarSpeedItem;
            Border AddButton = MakeAddButton("Add");
            AddButton.MouseDown += AddButton_PreviewMouseDown;
            SaveButton.MouseDown += SaveButton_MouseDown;
            DeleteButton.MouseDown += DeleteButton_MouseDown;
            AddElementToGrid(MainGrid, lblCarSpeedIndex,0,0);
            AddElementToGrid(MainGrid, lblMin, 0, 1);
            AddElementToGrid(MainGrid, tbMin, 0, 2);
            AddElementToGrid(MainGrid, lblMax, 0, 3);
            AddElementToGrid(MainGrid, tbMax, 0, 4);
            AddElementToGrid(MainGrid, lblFreeze, 0, 5);
            AddElementToGrid(MainGrid, tbFreeze, 0, 6);
            if (Index == Bg_CarspeedList.Count)
            {
                AddElementToGrid(MainGrid, DeleteButton, 0, 8);
                AddElementToGrid(MainGrid, AddButton, 0, 7);
                AddElementToGrid(MainGrid, SaveButton, 0, 9);
            }
            else
            {
                AddElementToGrid(MainGrid, DeleteButton, 0, 7);
            }
            return MainGrid;
        }

        private void DeleteButton_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Border bd = (sender as Border);
            Bg_Carspeed bcs = (bd.Tag as Bg_Carspeed);
            int Index = Bg_CarspeedList.IndexOf(bcs);
            if (Index == -1) return;
            if (Index != Bg_CarspeedList.Count -1 && Index!=0)
            {
                Bg_CarspeedList[Index + 1].SpeedMin = Bg_CarspeedList[Index - 1].SpeedMax;
            }
            else if(Index == 0)
            {
                if (Bg_CarspeedList.Count > 1) Bg_CarspeedList[1].SpeedMin = "0";
                else
                { BG_MESSAGEBOX.Show("提示", "仅剩一个不能删除"); return; }
            }
            CarSpeedBLL.GetInstance().DeleteCarSpeedModel(bcs, ControlVersion.FastCheck);
            Bg_CarspeedList.Remove(bcs);
            //foreach (var item in Bg_CarspeedList)
            //{
            //    item.No = (Bg_CarspeedList.IndexOf(item) + 1).ToString();
            //}
            InitContent();
        }

        /// <summary>
        /// 如果最大值的Max变了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TbMax_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            int CurrentIndex = Convert.ToInt32(tb.Tag);
            if (string.IsNullOrEmpty(tb.Text)) return;
            if(CurrentIndex != Bg_CarspeedList.Count)
            {
                Bg_CarspeedList[CurrentIndex].SpeedMax = Bg_CarspeedList[CurrentIndex].SpeedMax;
                Bg_CarspeedList[CurrentIndex].SpeedMin = tb.Text;
            }
            Bg_CarspeedList[CurrentIndex - 1].SpeedMin = Bg_CarspeedList[CurrentIndex - 1].SpeedMin;
        }

        private void SaveButton_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(Bg_CarspeedList.Any(q=>!string.IsNullOrEmpty(q.Error)))
            {
                BG_MESSAGEBOX.Show(UpdateStatusNameAction("Tip"),"请检查数据错误！");
                return;
            }
            CarSpeedBLL.GetInstance().SaveDoseModel(Bg_CarspeedList,ControlVersion.FastCheck);
        }

        private void AddButton_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                Bg_Carspeed bc = new Bg_Carspeed()
                {
                    No = CommonFunc.GetGuid(),
                    SpeedMin = Bg_CarspeedList.Last().SpeedMax,
                    SpeedMax =
                         (Convert.ToDecimal(Bg_CarspeedList.Last().SpeedMax) + 20).ToString(),
                    Freez = (Convert.ToInt32(Bg_CarspeedList.Last().Freez) + 50).ToString(),
                    BayNumber = ConfigServices.GetInstance().localConfigModel.CMW_FastCheckBayNumber
                };
                CarSpeedBLL.GetInstance().AddCarSpeedModel(bc);
                InitContent();
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
        private void AddElementToGrid(Grid ParentGrid,FrameworkElement CurrentFFE,int Row,int Column,int RowSpan = 1,int ColumnSpan = 1)
        {
            ParentGrid.Children.Add(CurrentFFE);
            Grid.SetColumn(CurrentFFE,Column);
            Grid.SetRow(CurrentFFE, Row);
            Grid.SetColumnSpan(CurrentFFE, ColumnSpan);
            Grid.SetRowSpan(CurrentFFE, RowSpan);
        }
        /// <summary>
        /// 生成单行表格
        /// </summary>
        /// <returns></returns>
        private Grid MakeGrid()
        {
            Grid MainGrid = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch,VerticalAlignment = VerticalAlignment.Stretch};
            MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1,GridUnitType.Star)});
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(25, GridUnitType.Star) });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(20, GridUnitType.Star) });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(120, GridUnitType.Pixel) });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(20, GridUnitType.Star) });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(120, GridUnitType.Pixel) });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(20, GridUnitType.Star) });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(120, GridUnitType.Pixel) });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(60, GridUnitType.Pixel) });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(60, GridUnitType.Pixel) });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(60, GridUnitType.Pixel) });

            return MainGrid;
        }
 

        private ComboBox MakeDropDownList(ObservableCollection<SelectObject> ItemSources)
        {
            ComboBox cbBox = new ComboBox()
            {
                Style = (Style)this.FindResource("stlComboBox"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(0, 3, 3, 3),
                Height = 35,
                Width = 98
            };
            cbBox.SelectedValuePath = "SelectValue";
            cbBox.DisplayMemberPath = "SelectText";
            cbBox.ItemsSource = ItemSources;
            return cbBox;
        }

        /// <summary>
        /// 生成TextBox
        /// </summary>
        /// <param name="CarSpeed"></param>
        /// <param name="BindAtt"></param>
        /// <returns></returns>
        private TextBox MakeTextBox(Bg_Carspeed CarSpeed,string BindAtt)
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
        /// 设置主Grid
        /// </summary>
        /// <returns></returns>
        private Grid MakeMainGrid()
        {
            Grid _MainGrid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Width = 860
            };
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Pixel) });
            for (int i = 0; i < Bg_CarspeedList.Count; i++)
            {
                _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Star) });
            }
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Star) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Pixel) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(99, GridUnitType.Star) });
            return _MainGrid;
        }

        /// <summary>
        /// 设置探测器排数
        /// </summary>
        /// <returns></returns>
        public Grid MakeTCQBayNumber()
        {
            Grid gd = MakeGrid();
            ObservableCollection<SelectObject> SelectObjects = new ObservableCollection<SelectObject>()
            {
                new SelectObject(){SelectText= "1",SelectValue = "1" },
                new SelectObject(){SelectText= "2",SelectValue = "2" },
                new SelectObject(){SelectText= "4",SelectValue = "4" },
            };
            var lblName = MakeLabel("BayNumber");
            var ddlDown = MakeDropDownList(SelectObjects);
            ddlDown.SelectedValue = ConfigServices.GetInstance().localConfigModel.CMW_FastCheckBayNumber;
            var btnSetting = MakeAddButton("Setting");
            btnSetting.Tag = ddlDown;
            btnSetting.MouseDown += BtnSetting_PreviewMouseDown;
            gd.Children.Add(lblName);
            gd.Children.Add(ddlDown);
            gd.Children.Add(btnSetting);
            Grid.SetColumn(lblName,0); Grid.SetColumn(ddlDown, 1); Grid.SetColumnSpan(ddlDown,2); Grid.SetColumn(btnSetting, 3);
            return gd;
        }
        /// <summary>
        /// 这里设置探测器排数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSetting_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                ComboBox cb = (sender as Border).Tag as ComboBox;
                string SelectBay = cb.SelectedValue as string;
                //向配置文件写入排数
                //TODO 还要调用方海涛的写入排数函数
                if(!string.IsNullOrEmpty(SelectBay))
                {
                    ConfigServices.GetInstance().localConfigModel.CMW_FastCheckBayNumber = SelectBay;
                    CommonDeleget.UpdateConfigs("CMW_FastCheckBayNumber", SelectBay, Section.SOFT);
                }
                CarSpeedBLL.GetInstance().SaveDoseModel(Bg_CarspeedList,ControlVersion.FastCheck);
                //设置完成，先保存下当前的配置，再去更新界面取到当前排数对应的车速频率
                InitContent();
            }
            catch (Exception ex)
            {
                CommonDeleget.WriteLogAction(ex.Message, LogType.ApplicationError, true);
            }
        }
    }
}
