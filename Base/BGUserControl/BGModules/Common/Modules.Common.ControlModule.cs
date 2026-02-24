using BG_Entities;
using BG_Services;
using BG_WorkFlow;
using BGCommunication;
using BGModel;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
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

namespace BGUserControl
{
    [Export("PageModule", typeof(IConditionView))]
    [CustomExportMetadata(1, Modules.ControlModule, "监测模块", "ZZW", "1.0.0")]
    public class ControlModule : BaseModules
    {
        Dictionary<string, string> TempDic = new Dictionary<string, string>
        {
            { "RadiationSourceOrAcceleratorReady","Boosting" },
            {"DriveReady","CarHandStatus"},
            {"MainSystemReady","CenterControlTree"},
        };

        #region 车载的树数据列表

        ObservableCollection<StatusModel> StatusModellList = new ObservableCollection<StatusModel>();
        ObservableCollection<StatusModel> StatusDisplayScreenModellList = new ObservableCollection<StatusModel>();
        /// <summary>
        /// 被动模式流程树
        /// </summary>
        ObservableCollection<StatusTreeModel> TreeModelList = new ObservableCollection<StatusTreeModel>();
        /// <summary>
        /// 主动模式流程树
        /// </summary>
        ObservableCollection<StatusTreeModel> TreeModelForZDList = new ObservableCollection<StatusTreeModel>();
        /// <summary>
        /// 主动模式低速度扫描流程树
        /// </summary>
        ObservableCollection<StatusTreeModel> ZDModeTreeModelList = new ObservableCollection<StatusTreeModel>();
        /// <summary>
        /// 手动出束流程树
        /// </summary>
        ObservableCollection<StatusTreeModel> HandRayTreeModelList = new ObservableCollection<StatusTreeModel>();
        /// <summary>
        /// 臂架展开动作条件树
        /// </summary>
        ObservableCollection<StatusTreeModel> HandOpenActionList = new ObservableCollection<StatusTreeModel>();
        /// <summary>
        /// 臂架收回动作条件树
        /// </summary>
        ObservableCollection<StatusTreeModel> HandCloseActionList = new ObservableCollection<StatusTreeModel>();
        /// <summary>
        /// 主系统就绪树
        /// </summary>
        ObservableCollection<StatusTreeModel> MainSystemReadyTree = new ObservableCollection<StatusTreeModel>();

        #endregion

        #region 快检的数据表
        /// <summary>
        /// 主系统就绪树
        /// </summary>
        ObservableCollection<StatusTreeModel> FastCheckMainSystemReadyTree = new ObservableCollection<StatusTreeModel>();
        ObservableCollection<StatusTreeModel> PassageCarMainSystemReadyTree = new ObservableCollection<StatusTreeModel>();
        #endregion
        Dictionary<string, ObservableCollection<StatusTreeModel>> SourceList = new Dictionary<string, ObservableCollection<StatusTreeModel>>();
        Dictionary<int, List<int>> DicIndex = new Dictionary<int, List<int>>();
        public string TabName = string.Empty;
     
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
            Background = Brushes.White,
        }
        ;

        [ImportingConstructor]
        public ControlModule() : base( ControlVersion.Car)
        {
            Loaded += Car_ControlModule_Loaded;
            Unloaded += Car_ControlModule_Unloaded;
        }
        /// <summary>
        /// 从配置文件取出原始数据
        /// </summary>
        private void InitData()
        {
            StatusModellList.Clear();
            HitChModelBLL.GetInstance().GetHitchModelDataModel
                (SystemDirectoryConfig.GetInstance().GetHittingConfig(cv)).Where(q =>
                !string.IsNullOrEmpty(q.StatusName))
                .ToList().Where(q => q.Bg_Own != "NormalStatus" && !string.IsNullOrEmpty(q.Bg_Own)).ToList().ForEach(q => StatusModellList.Add(q));

            StatusDisplayScreenModellList.Clear();
            HitChModelBLL.GetInstance().GetHitchModelDataModel
                (SystemDirectoryConfig.GetInstance().GetHittingConfig(cv)).Where(q => !string.IsNullOrEmpty(q.StatusName))
                .ToList().Where(q => q.Bg_Own != "NormalStatus" && !string.IsNullOrEmpty(q.Bg_Own))
                .GroupBy(q => q.Bg_Status_Position_Index)
                .ToList()
                .Select(item => item.First())
                .ToList().
                ForEach(p => StatusDisplayScreenModellList.Add(p));

            InitTreeData();
            
            foreach (var item in StatusModellList)
            {
                int PostitionIndex = Convert.ToInt32(item.Bg_Status_Position_Index);
                int ItemIndex = Convert.ToInt32(item.StatusIndex);
                if (DicIndex.ContainsKey(PostitionIndex))
                {
                    if (!DicIndex[PostitionIndex].Contains(ItemIndex))
                    {
                        DicIndex[PostitionIndex].Add(ItemIndex);
                    }
                }
                else
                {
                    List<int> intArray = new List<int>();
                    intArray.Add(ItemIndex);
                    DicIndex.Add(PostitionIndex, intArray);
                }
            }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void InitTreeData()
        {
            SourceList.Clear();
            if (cv == ControlVersion.Car)
            {
                TreeModelList.Clear();
                CenterTreeBLL.GetInstance().GetStatusTreeModelDic()
                    .Where(q => (q as StatusTreeModel).Bg_Parent_Id != string.Empty).ToList()
                    .ForEach(p => TreeModelList.Add(p as StatusTreeModel));

                ZDModeTreeModelList.Clear();
                CenterTreeBLL.GetInstance()
                    .GetStatusTreeModelDataModel(SystemDirectoryConfig.GetInstance().GetZDModeTreeConfig(cv))
                    .Where(q => (q as StatusTreeModel).Bg_Parent_Id != string.Empty).ToList()
                    .ForEach(p => ZDModeTreeModelList.Add(p as StatusTreeModel));

                HandRayTreeModelList.Clear();
                CenterTreeBLL.GetInstance()
                    .GetStatusTreeModelDataModel(SystemDirectoryConfig.GetInstance().GetHandRayModeTreeConfig(cv))
                    .Where(q => (q as StatusTreeModel).Bg_Parent_Id != string.Empty).ToList()
                    .ForEach(p => HandRayTreeModelList.Add(p as StatusTreeModel));

                HandOpenActionList.Clear();
                CenterTreeBLL.GetInstance()
                    .GetStatusTreeModelDataModel(SystemDirectoryConfig.GetInstance()
                        .GetHandOpenActionConditionTreeConfig(cv))
                    .Where(q => (q as StatusTreeModel).Bg_Parent_Id != string.Empty).ToList()
                    .ForEach(p => HandOpenActionList.Add(p as StatusTreeModel));
                HandCloseActionList.Clear();
                CenterTreeBLL.GetInstance()
                    .GetStatusTreeModelDataModel(SystemDirectoryConfig.GetInstance()
                        .GetHandCloseActionConditionTreeConfig(cv))
                    .Where(q => (q as StatusTreeModel).Bg_Parent_Id != string.Empty).ToList()
                    .ForEach(p => HandCloseActionList.Add(p as StatusTreeModel));
                TreeModelForZDList.Clear();
                CenterTreeBLL.GetInstance()
                    .GetStatusTreeModelDataModel(SystemDirectoryConfig.GetInstance().GetTreeConfigForZD(cv))
                    .Where(q => (q as StatusTreeModel).Bg_Parent_Id != string.Empty).ToList()
                    .ForEach(p => TreeModelForZDList.Add(p as StatusTreeModel));
                MainSystemReadyTree.Clear();
                CenterTreeBLL.GetInstance()
                    .GetStatusTreeModelDataModel(SystemDirectoryConfig.GetInstance().GetMainReadyTree(cv))
                    .Where(q => (q as StatusTreeModel).Bg_Parent_Id != string.Empty).ToList()
                    .ForEach(p => MainSystemReadyTree.Add(p as StatusTreeModel));

                SourceList.Add("ZDCenterControlTree", TreeModelForZDList);
                SourceList.Add("BDCenterControlTree", TreeModelList);
                SourceList.Add("ZDLowSpeedCenterControlTree", ZDModeTreeModelList);
                SourceList.Add("HandRayCenterControlTree", HandRayTreeModelList);
                SourceList.Add("HandOpenActionContionTree", HandOpenActionList);
                SourceList.Add("HandCloseActionContionTree", HandCloseActionList);
                SourceList.Add("MainSystemReady", MainSystemReadyTree);
            }
            else if (cv == ControlVersion.FastCheck)
            {
                FastCheckMainSystemReadyTree.Clear();
                CenterTreeBLL.GetInstance().GetStatusTreeModelDataModel(SystemDirectoryConfig.GetInstance()
                    .GetMainReadyTree(cv)).ForEach(q => FastCheckMainSystemReadyTree.Add(q));
                SourceList.Add("MainSystemReady", FastCheckMainSystemReadyTree);
            }
            else if (cv == ControlVersion.PassengerCar)
            {
                PassageCarMainSystemReadyTree.Clear();
                CenterTreeBLL.GetInstance().GetStatusTreeModelDataModel(SystemDirectoryConfig.GetInstance()
                    .GetMainReadyTree(cv)).ForEach(q => PassageCarMainSystemReadyTree.Add(q));
                SourceList.Add("MainSystemReady", PassageCarMainSystemReadyTree);
            }
        }

        #region 接口
        public override void SetSelectTabName(string TabName)
        {
            this.TabName = TabName;
        }
        public override string GetName()
        {
            return "状态及故障监测";
        }

        public override bool IsConnectionEquipment()
        {
            return Common.IsConnection;
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
            return 1000;
        }

        public override double GetWidth()
        {
            return 980;
        }

        #endregion


        private Grid InitGrid()
        {
            Grid _MainGrid = InitMainGird();

            DockPanel dp = new DockPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                LastChildFill = true,
            };
            Border bd = new Border()
            {
                BorderThickness = new Thickness(0, 0, 0, 0),
                BorderBrush = (LinearGradientBrush)this.FindResource("LnearBorderColor"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 32,
                Background = (LinearGradientBrush)this.FindResource("TitleBackGround"),
            };

            Canvas cs = new Canvas()
            {
                Style = (Style)this.FindResource("diyCloseCanvas"),
                HorizontalAlignment = HorizontalAlignment.Right, 
                Margin = new Thickness(0, 1, 20, 0),
                Width = 14,
                Height = 14
            };
            cs.SetValue(Panel.ZIndexProperty, 1001);
            cs.PreviewMouseLeftButtonDown += Bd_PreviewMouseLeftButtonDown; 
            bd.Child = cs;
            var TabControl = MakeHitchPanel(StatusDisplayScreenModellList);
            dp.Children.Add(TabControl);
            dp.Children.Add(bd);
            Grid.SetColumn(dp, 0);
            Grid.SetRow(dp, 0);
            Grid.SetColumnSpan(dp, 3);
            _MainGrid.Children.Add(dp);
            dp.MouseLeftButtonDown += Dp_MouseDown;
            return _MainGrid;
        }

        private static Grid InitMainGird()
        {
            Grid _MainGrid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30, GridUnitType.Star) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10, GridUnitType.Pixel) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30, GridUnitType.Star) });
            return _MainGrid;
        }

        private void Dp_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    CurrentWindow.DragMove();
            }
            catch (Exception ex)
            {
                BGLogs.Log.GetDistance().WriteErrorLogs(ex.StackTrace);
            }
        }

        private void _lblTitle_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            bool result = IsSearchStatusSuccess;
        }

        private TabControl MakeHitchPanel(ObservableCollection<StatusModel> HitchModelList)
        {
            int GroupNum = HitchModelList.Distinct(q => q.Bg_Own).Count(q => !string.IsNullOrEmpty(q.Bg_Own));
            var tempGroup = HitchModelList.GroupBy(q => q.Bg_Own);// 按照组别分组，看下一共有多少组
            TabControl tabControl = MakeTabControl(tempGroup);                                                                                                  
            return tabControl;
        }
        /// <summary>
        /// 通过数据源来生成状态面板
        /// </summary>
        /// <param name="HitchModelList"></param>
        /// <returns></returns>
        public ScrollViewer MakeSingleHitchPanel(ObservableCollection<object> HitchModelList)
        {
           
            Grid _dp = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                Background = Brushes.White,
                Width = GetWidth() - 14,
                //Height= GetHeight()
            };
            ScrollViewer sv = new ScrollViewer()
            {
                Content = _dp,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Width = GetWidth()
            };
            int maxColumnsNum = 5;
            int TempRows = 0;
            int ColumnSpan = Convert.ToInt32(Math.Ceiling(((decimal)HitchModelList.Count() / (decimal)maxColumnsNum)));
            TempRows += ColumnSpan;
            for (int i = 0; i < maxColumnsNum; i++)
            {
                _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }
            for (int i = 0; i < TempRows + 1; i++)
            {
                _dp.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            }
            int Index = 0;
            foreach (var HitchItem in HitchModelList)
            {
                int ColumnIndex = HitchModelList.IndexOf(HitchItem) % maxColumnsNum;
                int RowsIndex = HitchModelList.IndexOf(HitchItem) / maxColumnsNum;
                var smItem = (HitchItem as StatusModel);

                Border OutBorder = new Border()
                {
                    Width = 175,
                    Height = 50,
                    Background = StrToBrush("#E4E9EF"),
                    BorderBrush = StrToBrush("#AAB8C9"),
                    CornerRadius = new CornerRadius(2),
                    BorderThickness = new Thickness(1),
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(5, 20, 2, 0)
                };
                DiyStatusLabel lblHitch = new DiyStatusLabel()
                {
                    LblTextForground = (SolidColorBrush)this.TryFindResource("defaultBlack"),
                    FontSize = UpdateFontSizeAction(CMWFontSize.Normal),
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                };
                OutBorder.Child = lblHitch;

                //故障名称
                smItem.StatusDisplayName = UpdateStatusNameAction(smItem.StatusName);
                lblHitch.SetBinding(DiyStatusLabel.LabelTextProperty, new Binding("StatusDisplayName") { Source = HitchItem, Mode = BindingMode.TwoWay });
                lblHitch.SetBinding(DiyStatusLabel.LabmForecolorProperty, new Binding("StatusCode") { Source = HitchItem, Mode = BindingMode.TwoWay, Converter = new HitchLambColorConvert() });

                Grid.SetColumn(OutBorder, ColumnIndex);
                Grid.SetRow(OutBorder, RowsIndex);
                _dp.Children.Add(OutBorder);
                Index++;
            }
            return sv;
        }

        /// <summary>
        /// 初始化TabControl
        /// </summary>
        /// <param name="Groupings"></param>
        /// <returns></returns>
        public TabControl MakeTabControl(IEnumerable<IGrouping<string, StatusModel>> Groupings)
        {
            TabControl tc = new TabControl()
            {
                Style = (Style)this.TryFindResource("DiyTabControl"),
                Width = GetWidth()-14
            };

            tc.BorderThickness = new Thickness(0);
            foreach (var GroupItem in Groupings)
            {
                MakeTabItem(tc, MakeSingleHitchPanel(new ObservableCollection<object>(GroupItem.ToArray())), GroupItem.Key);
            }
            //由于是多版本共用同一个模块，当是自行走模块的时候就不生成中控树
            if (cv == ControlVersion.Car || cv == ControlVersion.FastCheck || cv == ControlVersion.PassengerCar)
            {
                MakeCenterTree(tc);
            }
            
            MakeDetratronBoard(tc);

            if (BoostingControllerManager.GetInstance().GetCurrentEquipmentModel() == "FH_Betratron")
            {
                MakeBetratron_FHBoostingBoard(tc);
            }
            return tc;
        }
        /// <summary>
        /// 完成中控流程树
        /// </summary>
        /// <param name="tc"></param>
        private void MakeCenterTree(TabControl tc)
        {
            try
            {
                TabControl tcCenter = new TabControl()
                {
                    Style = (Style)this.TryFindResource("DiyTabControl"),
                    Margin = new Thickness(0, 5, -14, 0),
                    Width = GetWidth()
                };
                foreach (var SourceItem in SourceList)
                {
                    DiyCenterControlTree diyCenterPanel = new DiyCenterControlTree();
                    diyCenterPanel.Width = 1000;
                    diyCenterPanel.MaxWidth = GetWidth()+20;
                    diyCenterPanel.ItemSource = SourceItem.Value;
                    diyCenterPanel.Height = diyCenterPanel.MaxOwnHeight + 20;
                    diyCenterPanel.Margin = new Thickness(0, 0, 0, 0);
                    ScrollViewer sv = new ScrollViewer()
                    {
                        Content = diyCenterPanel,
                        HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
                        VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                        Width = GetWidth()
                    };
                    if (SourceList.Count == 1)
                    {
                        tcCenter.Style = (Style) this.TryFindResource("DiyTabControlNoHeader");
                        MakeTabItem(tcCenter, sv, SourceItem.Key,Visibility.Collapsed);
                    }
                    else
                    {
                        MakeTabItem(tcCenter, sv, SourceItem.Key, Visibility.Visible);
                    }
                }
                MakeTabItem(tc, tcCenter, "CenterControlTree");
            }
            catch (Exception ex)
            {
            }
        }


        private void MakeDetratronBoard(TabControl tc)
        {
            var DetratronContent  =  ContentPlugins?.First(q => q.Metadata.Name == Modules.DetratronBoardModule)?.Value;
            DetratronContent?.SetCarVersion(cv);
            MakeTabItem(tc, DetratronContent, "DetectorBoardControl");
            
        }


        private void MakeBetratron_FHBoostingBoard(TabControl tc)
        {
            
            var DetratronContent = ContentPlugins?.First(q => q.Metadata.Name == Modules.FHBetratronStatusCheckModule)?.Value;
            DetratronContent?.SetCarVersion(cv);
            MakeTabItem(tc, DetratronContent, "BetratronBoostingBoard");
        }


        /// <summary>
        /// 生成TabItem
        /// </summary>
        /// <param name="tc"></param>
        /// <param name="Content"></param>
        private void MakeTabItem(TabControl tc, object Content, string title,Visibility isShowHeader = Visibility.Visible)
        {
            TabItem tiItem = new TabItem()
            {
                Style = (Style)this.TryFindResource("DiyTabItemStyle"),
                Header = UpdateStatusNameAction(title),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
            };
            tiItem.Visibility = isShowHeader;
            tiItem.Content = Content;
            tc.Items.Add(tiItem);

            //这段代码是为了跳转用的
            if (TempDic.ContainsKey(this.TabName))
            {
                if (TempDic[this.TabName] == title)
                {
                    tc.SelectedItem = tiItem;
                }
            }
            if (this.TabName.Contains("MainSystemReady") && title.Contains("MainSystemReady"))
            {
                tc.SelectedItem = tiItem;
            }
        }

        private void Bd_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Close();
        }

        private void Car_ControlModule_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            BuryingPoint($"{UpdateStatusNameAction("ControlModuleLeave")}");
        }

        private void Car_ControlModule_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender != null)
            {
                BuryingPoint($"{UpdateStatusNameAction("ControlModuleEnter")}");
            }
           
            InitContent();
            InitAction();
        }

        private void InitContent()
        {
            InitData();

            _MainGrid = InitGrid();
            _MainBorder.Child = _MainGrid;
            Content = _MainBorder;
        }

        private void InitAction()
        {
            ButtonInvoke.SwitchLanguageEvent -= SwitchLanguage;
            ButtonInvoke.SwitchLanguageEvent += SwitchLanguage;
            ButtonInvoke.SwitchFontSizeEvent -= SwitchFontSize;
            ButtonInvoke.SwitchFontSizeEvent += SwitchFontSize;
            


            Task.Run(() =>
            {
                _timer_Elapsed();
            });
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
        private void _timer_Elapsed()
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(120);
                    if (!IsVisible)
                    {
                        break;
                    }

                    InqureStatus();
                }
                catch (System.Exception ex)
                {
                    CommonDeleget.HandTaskException(ex);
                    break;
                }
            }
        }
        private void InqureStatus()
        {
            try
            {
                if (!IsConnectionEquipment())
                {
                    ConnectionNotSuccessStatusCheck();
                    return;
                }
                ConnectionSuccessStatusCheck();
            }
            catch (Exception ex)
            {
                BuryingPoint(ex.StackTrace);
            }
        }
        private Dispatcher UIDispatcher => Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
        /// <summary>
        /// 连接状态正常时 的面板检测状态
        /// </summary>
        private async void ConnectionSuccessStatusCheck()
        {
            await UIDispatcher.InvokeAsync(() =>
            {
                bool result = IsSearchStatusSuccess;
                foreach (var HitchItem in StatusModellList)
                {
                    int ItemIndex = Convert.ToInt32(HitchItem.StatusIndex);
                    int PositionIndex = Convert.ToInt32(HitchItem.Bg_Status_Position_Index);

                    if (ItemIndex < GlobalRetStatus.Count)
                    {
                        var Item = StatusDisplayScreenModellList.FirstOrDefault(q => q.Bg_Status_Position_Index == PositionIndex.ToString());
                        List<int> IndexList = DicIndex[PositionIndex];
                        var StatusResult = GlobalRetStatus[ItemIndex];
                        if (Item == null)
                        {
                            return;
                        }
                        
                        ///探测器单独判断
                        if (Item.StatusName.Contains("DetectorReady") || Item.StatusName.Contains("探测器通讯"))
                        {
                            //探测器需要去找扫描站的连接状态，如果判断到扫描站的状态是未连接
                            Item.StatusCode = IsScanCanScan() ? "2" : "0";
                            continue;
                        }

                        if (Item.StatusName.Contains("TransferCaseEngagementStatus") || Item.StatusName.Contains("分动器"))
                        {
                            Item.StatusName = PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.TransferCaseEngagementStatus) ? UpdateStatusNameAction("TransferCaseEngagementClose")
                                : UpdateStatusNameAction("TransferCaseEngagementOpen");
                            Item.StatusCode = GlobalRetStatus[ItemIndex] ? "1" : "0";
                            continue;
                        }

                        ///探测器单独判断
                        if (Item.StatusName.Contains("安全联锁"))
                        {
                            // 探测器需要去找扫描站的连接状态，如果判断到扫描站的状态是未连接
                            Item.StatusCode = PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.SafetyInterlockReady) || Common.isSafeSerices ? "2" : "0";
                            continue;
                        }

                        //如果所在的组只有1个，那就跟原来一样
                        if (IndexList.Count == 1)
                        {
                            GroupOnlyOne(Item, StatusResult);
                        }
                        //如果所在组不止一个
                        else
                        {
                            GourpNotOnlyOne(Item, IndexList);
                        }
                    }
                }
            });
        }


        /// <summary>
        /// 连接状态不正常时 的面板检测状态
        /// </summary>
        private void ConnectionNotSuccessStatusCheck()
        {
            bool result = IsSearchStatusSuccess;
            foreach (var HitchItem in StatusModellList)
            {
                int ItemIndex = Convert.ToInt32(HitchItem.StatusIndex);
                int PositionIndex = Convert.ToInt32(HitchItem.Bg_Status_Position_Index);

                if (ItemIndex < GlobalRetStatus.Count)
                {
                    var Item = StatusDisplayScreenModellList.FirstOrDefault(q => q.Bg_Status_Position_Index == PositionIndex.ToString());
                    List<int> IndexList = DicIndex[PositionIndex];
                    var StatusResult = GlobalRetStatus[ItemIndex];
                    if (Item == null)
                    {
                        return;
                    }

                    ///探测器单独判断
                    if (Item.StatusName.Contains("DetectorReady") || Item.StatusName.Contains("探测器通讯"))
                    {
                        // 探测器需要去找扫描站的连接状态，如果判断到扫描站的状态是未连接
                        Item.StatusCode = IsScanCanScan() ? "2" : "0";
                        continue;
                    }

                    ///探测器单独判断
                    if (Item.StatusName.Contains("安全联锁"))
                    {
                        // 探测器需要去找扫描站的连接状态，如果判断到扫描站的状态是未连接
                        Item.StatusCode = "0";
                        continue;
                    }

                    //如果所在的组只有1个，那就跟原来一样
                    if (IndexList.Count == 1)
                    {
                        GroupOnlyOne(Item, StatusResult);
                    }
                    //如果所在组不止一个
                    else
                    {
                        GourpNotOnlyOne(Item, IndexList);
                    }
                }
            }
        }

        //分组不止一个
        private void GourpNotOnlyOne(StatusModel Item, List<int> IndexList)
        {
            int trueIndex = -1;
            //循环改组所有的状态i
            foreach (var item in IndexList)
            {
                if (GlobalRetStatus[item])
                {
                    trueIndex = item;
                }
            }
            //如果该组之前都没有状态为true
            //如果不为-1说明,有状态为true的，此时判断，当前item是不是那种故障需要取反的 状态，如果是，
            //此时true就要变为1，如果是取反的就是0表示为true
            if (trueIndex != -1)
            {
                if (!IsConnection)
                {
                    Item.StatusCode = "0";
                }
                else
                {
                    Item.StatusCode = GlobalRetStatus[trueIndex]?"1":"0";
                    //(Item.Bg_Own.Contains("SafeStatus") || Item.Bg_Own.Contains("RadiationSourceOrAcceleratorReady")
                    //                  || Out.Contains(Item.StatusIndex) || Item.Bg_Own.Contains("SafeSeriers")) ? "0" :
                    //Item.Bg_Own.Contains("NormalStatus") ? "1" : "2";
                }

                string TempStr = StatusModellList.FirstOrDefault(q => q.StatusIndex == trueIndex.ToString())?.StatusName;
                Item.StatusDisplayName = UpdateStatusNameAction(TempStr);
            }
            //需要把当前东西变为false 
            //但是有的false是正常的，有的不是正常的，所以需要区分
            //当Bg_Own.Contains("SafeStatus") 的Item的时候，此时false说明是正常的，需要给黑色/绿色
            //其他的就要标红色了
            else
            {
                if (!IsConnection)
                {
                    Item.StatusCode = "0";
                }
                else
                {
                    Item.StatusCode = (GlobalRetStatus[Convert.ToInt32(Item.StatusIndex)] == Convert.ToBoolean(Item.DefaultValue)) ? "1" : "0";
                        //(Item.Bg_Own.Contains("SafeStatus") || Item.Bg_Own.Contains("RadiationSourceOrAcceleratorReady") 
                        //             || (Out.Contains(Item.StatusIndex)) || Item.Bg_Own.Contains("SafeSeriers"))  ? "2" : "0";
                }
            }
        }

        // <summary>
        // 分组只有一个
        // </summary>
        // <param name="Item"></param>
        // <param name="StatusResult"></param>
        private void GroupOnlyOne(StatusModel Item, bool StatusResult)
        {
            if (Item.Bg_Own.Contains("SafeStatus") || Item.Bg_Own.Contains("RadiationSourceOrAcceleratorReady") ||
                Item.Bg_Own.Contains("SafeSeriers") || Item.Bg_Own.Contains("CarHandStatus"))
            {
                if (!IsConnection)
                {
                    Item.StatusCode = "0";
                }
                else
                {
                    if (Item.StatusName.Contains("RadiationSourceOrAcceleratorReady") && cv == ControlVersion.CombinedMovementBetatron)
                    {
                        Item.StatusCode = BoostingControllerManager.GetInstance().IsReady() == Convert.ToBoolean(Item.DefaultValue) ? "2" : "0";// Common.SearchFHBoostingReady()
                        return;
                    }
                    Item.StatusCode = StatusResult == Convert.ToBoolean(Item.DefaultValue)? "2" : "0";
                }

            }
            else if (Item.Bg_Own.Contains("NormalStatus"))
            {
                if (!IsConnection)
                {
                    Item.StatusCode = "0";
                }
                else
                {
                    Item.StatusCode = StatusResult == Convert.ToBoolean(Item.DefaultValue) ? "1" : "0";
                }
            }
            else
            {
                if (Item.StatusName.Contains("RadiationSourceOrAcceleratorReady") && cv == ControlVersion.CombinedMovementBetatron)
                {
                    Item.StatusCode = BoostingControllerManager.GetInstance().IsReady()== Convert.ToBoolean(Item.DefaultValue) ? "2" : "0";// Common.SearchFHBoostingReady() 
                    return;
                }
                if (!IsConnection)
                {
                    Item.StatusCode = "0";
                }
                else
                {
                    Item.StatusCode = StatusResult == Convert.ToBoolean(Item.DefaultValue) ? "2" : "0";
                }
            }
        }
    }
}
