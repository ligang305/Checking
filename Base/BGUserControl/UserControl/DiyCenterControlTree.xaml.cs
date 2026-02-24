using BGModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonFunc;
using static CMW.Common.Utilities.CommonDeleget;
using CMW.Common.Utilities;
using BG_Entities;

namespace BGUserControl
{
    /// <summary>
    /// DiyCenterControlTree.xaml 的交互逻辑
    /// </summary>
    public partial class DiyCenterControlTree : DiyUserModule
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public DiyCenterControlTree()
        {
            InitializeComponent();

            IsVisibleChanged -= DiyCenterControlTree_IsVisibleChanged;
            IsVisibleChanged += DiyCenterControlTree_IsVisibleChanged;
            Loaded += DiyCenterControlTree_Loaded;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
        }

        private void DiyCenterControlTree_Loaded(object sender, RoutedEventArgs e)
        {

            foreach (var item in RootBorder)
            {
                PenControlTreeLine(item);
            }
            if (RootBorder[0].AllChildrensMaxShowHeight > this.Height)
            {
                this.Height = RootBorder[0].AllChildrensMaxShowHeight +20;
            }
        }
        /// <summary>
        /// 获取最大高度
        /// </summary>
        public int MaxOwnHeight { get; private set; }
        bool? isVisible = false;
        /// <summary>
        /// 属性，用来接收外部传输进来的数据对象
        /// </summary>
        public ObservableCollection<StatusTreeModel> ItemSource
        {
            get
            {
                return (ObservableCollection<StatusTreeModel>)GetValue(ItemSourceProperty);
            }
            set { SetValue(ItemSourceProperty, value); }
        }
        /// <summary>
        /// 数据源依赖属性
        /// </summary>
        public static readonly DependencyProperty ItemSourceProperty =
            DependencyProperty.Register("ItemSource", typeof(ObservableCollection<StatusTreeModel>), typeof(DiyCenterControlTree),
                 new PropertyMetadata(new ObservableCollection<StatusTreeModel>(), new PropertyChangedCallback(OnValueChange)));


        /// <summary>
        /// 接收到值 回调的方法
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiyCenterControlTree safePanel = d as DiyCenterControlTree;
            if (safePanel != null && safePanel.ItemSource != null)
            {
                safePanel.lblMainContent.Content = safePanel.MakeHitchPanel(safePanel.ItemSource);
                safePanel.RePosition();
            }
        }

        /// <summary>
        /// 异步线程查询状态
        /// </summary>
        public void InqureStatus(ObservableCollection<StatusTreeModel> StatusModelList)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (isVisible == false)
                    {
                        return;
                    }

                    Thread.Sleep(100);
                    ForAroundStatus(StatusModelList);
                    SearchRootNode();
                }
            });
        }

        private void SearchRootNode()
        {
            foreach (var item in RootBorder)
            {
                var getValue = item.GetValue;
            }
        }

        /// <summary>
        /// 循环面板状态
        /// </summary>
        /// <param name="StatusModelList"></param>
        private static void ForAroundStatus(ObservableCollection<StatusTreeModel> StatusModelList)
        {
            foreach (var item in StatusModelList)
            {
                StatusTreeModel sm = item as StatusTreeModel;
                int ItemIndex = Convert.ToInt32(sm.Bg_StatusIndex);
                //循环的时候如果断开链接，全部标红
                if (!IsConnection)
                {
                    if (ItemIndex == 11)
                    {
                        //探测器需要去找扫描站的连接状态，如果判断到扫描站的状态是未连接
                        sm.StatusCode = IsScanCanScan() ? "1" : "0";
                        continue;
                    }
                    sm.StatusCode = "0";
                    continue;
                }
              
                if (sm.Bg_TreeName != null && ItemIndex == 11)
                {
                    //探测器需要去找扫描站的连接状态，如果判断到扫描站的状态是未连接
                    sm.StatusCode = IsScanCanScan() ? "1" : "0";
                    continue;
                }
                //判断下临界值，如果当前的序号没有超过总数
                if (ItemIndex < GlobalRetStatus.Count && ItemIndex != -1)
                {
                    if (sm.IsDefalutBool.ToLower() == "false")
                    {
                        sm.StatusCode = GlobalRetStatus[ItemIndex] ? "0" : "1";
                    }
                    else
                    {
                        sm.StatusCode = GlobalRetStatus[ItemIndex] ? "1" : "0";
                    }
                    
                }
            }
        }
        int Rows = 0;
        Dictionary<int, Label> RowsButotns = new Dictionary<int, Label>();
        Dictionary<int, DiyTreeModel> BorderDic = new Dictionary<int, DiyTreeModel>();
        Dictionary<int, List<DiyTreeModel>> BorderList = new Dictionary<int, List<DiyTreeModel>>();
        List<DiyTreeModel> RootBorder = new List<DiyTreeModel>();
        Canvas MainCanvas;
        /// <summary>
        /// 通过数据源来生成状态面板
        /// </summary>
        /// <param name="HitchModelList"></param>
        /// <returns></returns>
        public Canvas MakeHitchPanel(ObservableCollection<StatusTreeModel> HitchModelList)
        {
            MainCanvas = new Canvas()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            var TreeList = ItemSource.Where(q => (q as StatusTreeModel).Bg_Parent_Id == "-1");

            Label bd = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Height = this.Height,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Stretch,
                MinWidth = 130,
            };
            Grid sp = new Grid()
            {
                VerticalAlignment = VerticalAlignment.Top
                //Orientation = Orientation.Vertical,
                //Height = 60,
            };
            bd.Content = sp;
            RowsButotns.Add(Rows, bd);
            BorderList.Add(Rows, new List<DiyTreeModel>());
            Canvas.SetLeft(bd, Rows * 10);
            MainCanvas.Children.Add(bd);
            foreach (var item in TreeList)
            {
                var ChildrenButton = MakeStatusBox(item);
                DiyTreeModel dtm = new DiyTreeModel();
                dtm.Parent = null;
                dtm.bd = ChildrenButton;
                dtm.StatusTreeModel = item;
                (RowsButotns[Rows].Content as Grid).Children.Add(ChildrenButton);
                BorderList[Rows].Add(dtm);
                Rows++;

                InsertTreeModleIntoGrid(MainCanvas, item, Rows, ref dtm);
                RootBorder.Add(dtm);
            }
            MaxOwnHeight = RootBorder[0].AllChildrensMaxShowHeight+50;
            return MainCanvas;
        }
        /// <summary>
        /// 递归绘画树状的流程树
        /// </summary>
        /// <param name="ContentGrid"></param>
        /// <param name="partentRoot"></param>
        public void InsertTreeModleIntoGrid(Canvas ContentGrid, object partentRoot, int RowsIndex, ref DiyTreeModel _ParentButton)
        {
            var TreeList = ItemSource.Where(q => (q as StatusTreeModel).Bg_Parent_Id == (partentRoot as StatusTreeModel).StatusId);
            if (TreeList == null)
            {
                return;
            }

            foreach (var item in TreeList)
            {
                var ChildrenButton = MakeStatusBox(item);
                DiyTreeModel dtm = new DiyTreeModel();
                dtm.Parent = _ParentButton;
                dtm.bd = ChildrenButton;
                dtm.StatusTreeModel = item;
                if (_ParentButton.Children == null)
                {
                    _ParentButton.Children = new List<DiyTreeModel>();
                }
                _ParentButton.Children.Add(dtm);
                if (!BorderList.ContainsKey(RowsIndex))
                {
                    BorderList.Add(RowsIndex, new List<DiyTreeModel>());
                }

                BorderList[RowsIndex].Add(dtm);
                if (!RowsButotns.ContainsKey(RowsIndex))
                {
                    Label bd = new Label()
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Height = this.Height,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        VerticalContentAlignment = VerticalAlignment.Stretch,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        MinWidth = 130,
                    };
                    Grid sp = new Grid()
                    {
                        VerticalAlignment = VerticalAlignment.Top
                    };
                    bd.Content = sp;
                    RowsButotns.Add(RowsIndex, bd);
                    Canvas.SetLeft(bd, RowsIndex * 190);
                    ContentGrid.Children.Add(bd);
                }
                BorderDic.Add(Convert.ToInt32(item.StatusId), dtm);
                //如果不需要展现，就不写入
                if ((item as StatusTreeModel).IsShow.ToLower() != "false")
                {
                    (RowsButotns[RowsIndex].Content as Grid).Children.Add(ChildrenButton);
                }
            }

            RowsIndex++;
            foreach (var itemChildren in TreeList)
            {
                var TempBorder = BorderDic[Convert.ToInt32(itemChildren.StatusId)];
                InsertTreeModleIntoGrid(ContentGrid, itemChildren, RowsIndex, ref TempBorder);
            }
        }


        private Border MakeStatusBox(StatusTreeModel StatusTreeModel)
        {
            Border OutBorder = new Border()
            {
                Width = 180,
                Height = 40,
                Background = StrToBrush("#E4E9EF"),
                BorderBrush = StrToBrush("#AAB8C9"),
                CornerRadius = new CornerRadius(2),
                BorderThickness = new Thickness(1),
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 0)
            };
            DiyStatusLabel lblHitch = new DiyStatusLabel()
            {
                LblTextForground = (SolidColorBrush)this.TryFindResource("defaultBlack"),
                FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.Normal),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            //故障名称
            StatusTreeModel.Bg_TreeName = UpdateStatusNameAction(StatusTreeModel.Bg_TreeName);
            lblHitch.SetBinding(DiyStatusLabel.LabelTextProperty, new Binding("Bg_TreeName") { Source = StatusTreeModel, Mode = BindingMode.TwoWay });
            lblHitch.SetBinding(DiyStatusLabel.LabmForecolorProperty, new Binding("StatusCode") { Source = StatusTreeModel, Mode = BindingMode.TwoWay, Converter = new HitchLambColorConvert() });

            OutBorder.Child = lblHitch;
            return OutBorder;
        }

        /// <summary>
        /// 控件显隐性
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiyCenterControlTree_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            isVisible = e.NewValue as bool?;
            if (isVisible != null && isVisible == true)
            {
                InqureStatus(ItemSource);
            }
        }

        /// <summary>
        /// 重新排布按钮的位置
        /// </summary>
        public void RePosition()
        {
            foreach (var item in RootBorder)
            {
                MaxNotRepeatRow(item, 0);
            }
        }

        /// <summary>
        /// 布局各个按钮的排版
        /// </summary>
        /// <param name="RootButton">当前节点</param>
        /// <param name="StartHeight">当前节点距离上边框的距离</param>
        public void MaxNotRepeatRow(DiyTreeModel RootButton, int StartHeight)
        {
            var TempButtonList = RootButton.Children;
            RootButton.bd.Margin = new Thickness(0, StartHeight + (RootButton.AllChildrensMaxShowHeight -10) / 2, 0, 0);
            if (TempButtonList != null)
            {
                foreach (var item in TempButtonList)
                {
                    int ChildrenStartHeight = StartHeight + TempButtonList.Where(q => TempButtonList.IndexOf(q) < TempButtonList.IndexOf(item))
                    .Sum(q => q.AllChildrensMaxShowHeight);
                    MaxNotRepeatRow(item as DiyTreeModel, ChildrenStartHeight);
                }
            }
        }


        /// <summary>
        /// 画连接线
        /// </summary>
        public void PenControlTreeLine(DiyTreeModel RootButton)
        {
            if (RootButton.StatusTreeModel.IsShow.ToLower() == "false")
            {
                return;
            }
            var TempButtonList = RootButton.Children;
            Point ParentPoint = RootButton.bd.TranslatePoint(new Point(), MainCanvas);
            if (TempButtonList == null)
            {
                return;
            }

            foreach (var item in TempButtonList)
            {
                if ((item as DiyTreeModel).StatusTreeModel.IsShow.ToLower() == "false")
                {
                    continue;
                }
                Point ChildrenPoint = item.bd.TranslatePoint(new Point(), MainCanvas);
                PenLine(ParentPoint, ChildrenPoint, RootButton.bd.ActualWidth, RootButton.bd.ActualHeight);
            }

            foreach (var itemButton in TempButtonList)
            {
                PenControlTreeLine(itemButton);
            }
        }
        /// <summary>
        /// 两个点画线
        /// </summary>
        /// <param name="StartPoint"></param>
        /// <param name="EndPoint"></param>
        public void PenLine(Point StartPoint, Point EndPoint, double BorderWidth, double BorderHeight)
        {

            Path pth = new Path();
            pth.Stroke = StrToBrush("#00345c");
            pth.StrokeThickness = 2;
            //pth.Fill = Brushes.Red; 
            GeometryGroup GG = new GeometryGroup();
            PathGeometry pg = new PathGeometry();
            PathFigure pf = new PathFigure();
            pf.StartPoint = new Point(StartPoint.X + BorderWidth, StartPoint.Y + BorderHeight / 2);
            //如果X相等，就画直线
            if (StartPoint.Y == EndPoint.Y)
            {
                pf.Segments.Add(new LineSegment(new Point(StartPoint.X + BorderWidth, StartPoint.Y + BorderHeight / 2), true));
                pf.Segments.Add(new LineSegment(new Point(EndPoint.X, EndPoint.Y + BorderHeight / 2), true));
            }
            //如果X不相等，就画折线
            else
            {
                pf.Segments.Add(new LineSegment(new Point(StartPoint.X + BorderWidth, StartPoint.Y + BorderHeight / 2), true));
                pf.Segments.Add(new LineSegment(new Point((EndPoint.X + StartPoint.X + BorderWidth) / 2, StartPoint.Y + BorderHeight / 2), true));
                pf.Segments.Add(new LineSegment(new Point((EndPoint.X + StartPoint.X + BorderWidth) / 2, EndPoint.Y + BorderHeight / 2), true));
                pf.Segments.Add(new LineSegment(new Point(EndPoint.X, EndPoint.Y + BorderHeight / 2), true));
            }
            GG.Children.Add(pg);
            pg.Figures.Add(pf);
            pth.Data = GG;
            pth.SetValue(Panel.ZIndexProperty, 9999);
            MainCanvas.Children.Add(pth);
        }


        /// <summary>
        /// 点击顶部移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CurrentWindow?.DragMove();
        }
        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CurrentWindow?.Close();
        }

        public override void Show(Window _OwnerWin)
        {
            CurrentWindow = _OwnerWin;
            _OwnerWin.MaxWidth = GetWidth();
            var Height = (ItemSource.Count / 5 + (ItemSource.Count % 5 == 0 ? 0 : 1)) * 60 + 100;
            _OwnerWin.MaxHeight = Height < GetHeight() ? GetHeight() : Height;
            _OwnerWin.Title = GetName();
            _OwnerWin.Content = this;
            _OwnerWin.Show();
        }
    }

    public class DiyTreeModel
    {
        private List<DiyTreeModel> _Children;
        public Border bd;
        public StatusTreeModel StatusTreeModel;
        public List<DiyTreeModel> Children
        {
            get { return _Children; }
            set { _Children = value; }
        }
        public DiyTreeModel Parent;
        #region 这三个变量是用来判断当前的矩形子节点所占的高度
        int ItemHeight { get; set; }
        public int AllChildrensMaxShowHeight { get { return GetAllChildrensMaxShowWidth(); } }
        /// <summary>
        /// Gets the maximum level.
        /// </summary>
        /// <returns>System.Int32.</returns>
        private int GetAllChildrensMaxShowWidth()
        {
            if (StatusTreeModel.IsShow.ToLower() == "false")
            {
                return 0;
            }
            else if (_Children == null || _Children.Count <= 0)
            {
                return ItemHeight + 44;
            }
            else
            {

                var totalHeight = Children.Sum(p => p == null ? 0 : p.AllChildrensMaxShowHeight);
                if (totalHeight == 0)
                {
                    return ItemHeight + 44;
                }
                return totalHeight;
            }
        }
        #endregion

        string value = "1";

        public string GetValue { get { return GetChildrenBoolValue(); } }

        public string GetChildrenBoolValue()
        {
            if (StatusTreeModel.Bg_StatusIndex == "-1")
            {
                if (_Children == null || _Children.Count <= 0)
                {
                    value = StatusTreeModel.StatusCode;
                    return value;
                }
                else
                {
                    bool isRount = false;
                    foreach (var item in Children)
                    {
                        if (item.GetValue == "0")
                        {
                            isRount = true;
                            StatusTreeModel.StatusCode = "0";
                            value = "0";
                        }
                       
                    }
                    if (!isRount)
                    { 
                        StatusTreeModel.StatusCode = "1";
                        value = "1";
                    }
                  
                    return value;
                }
            }
            else
            {
                if (Children != null)
                {
                    foreach (var item in Children)
                    {
                        if (item.GetValue == "0")
                        {

                        }
                    }
                }
                value = StatusTreeModel.StatusCode;
                return value;
            }
        }
    }
}
