using BG_Services;
using CMW.Common.Utilities;
using BGCommunication;

using BGModel;
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
using System.Windows.Media;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.CommonFunc;
using BG_Entities;
using BG_WorkFlow;
using static BG_WorkFlow.HitChModelBLL;

namespace BGUserControl
{
    [Export("PageModule", typeof(IConditionView))]
    [CustomExportMetadata(1, Modules.BSControlModule, "监测模块", "ZZW", "1.0.0")]
    public class BSControlModule : BSBaseModules
    {
        #region 车载的树数据列表
        ObservableCollection<StatusModel> StatusModellList = new ObservableCollection<StatusModel>();
        ObservableCollection<StatusModel> StatusDisplayScreenModellList = new ObservableCollection<StatusModel>();
        #endregion

        Dictionary<int, List<int>> DicIndex = new Dictionary<int, List<int>>();
        public  string TabName = string.Empty;
     
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
        public BSControlModule() : base(ControlVersion.BS)
        {
            Loaded += BSControlModule_Loaded;
            Unloaded += BSControlModule_Unloaded;
        }
        /// <summary>
        /// 从配置文件取出原始数据
        /// </summary>
        private void InitData()
        {
            List<StatusModel> StatusModels=  HitChModelBLL.GetInstance().GetHitchModelDataModel
                (SystemDirectoryConfig.GetInstance().GetHittingConfig(ControlVersion.BS));
            StatusModellList.Clear();
            StatusModels.Where(q =>
                !string.IsNullOrEmpty(q.StatusName))
                .ToList().Where(q => q.Bg_Own != "NormalStatus" && !string.IsNullOrEmpty(q.Bg_Own)).ToList().ForEach(q => StatusModellList.Add(q));

            StatusDisplayScreenModellList.Clear();
            StatusModels.Where(q => !string.IsNullOrEmpty(q.StatusName))
                .ToList().Where(q => q.Bg_Own != "NormalStatus" && !string.IsNullOrEmpty(q.Bg_Own))
                .GroupBy(q => q.Bg_Status_Position_Index)
                .ToList()
                .Select(item => item.First())
                .ToList().
                ForEach(p => StatusDisplayScreenModellList.Add(p));
        
            
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
            return BSPLCControllerManager.GetInstance().IsConnect();
        }

        public override void Show(Window _OwnerWin)
        {
            SetCarVersion(ControlVersion.BS);
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
            CurrentWindow.DragMove();
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
            return tc;
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

            if (this.TabName.Contains("MainSystemReady") && title.Contains("MainSystemReady"))
            {
                tc.SelectedItem = tiItem;
            }
        }

        private void Bd_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Close();
        }

        private void BSControlModule_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            BuryingPoint($"{UpdateStatusNameAction("ControlModuleLeave")}");
        }

        private void BSControlModule_Loaded(object sender, System.Windows.RoutedEventArgs e)
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
                InqureStatusThread();
            });
        }
        public void SwitchFontSize(string FontSize)
        {
            BSControlModule_Loaded(null, null);
        }
        public void SwitchLanguage(string language)
        {
            base.Base_SwitchLanguage(language);
            BSControlModule_Loaded(null, null);
        } 
        private void InqureStatusThread()
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

        /// <summary>
        /// 连接状态正常时 的面板检测状态
        /// </summary>
        private void ConnectionSuccessStatusCheck()
        {
            foreach (var HitchItem in StatusModellList)
            {
                int ItemIndex = Convert.ToInt32(HitchItem.StatusIndex);
                int PositionIndex = Convert.ToInt32(HitchItem.Bg_Status_Position_Index);

                if (ItemIndex < EquipmentManager.GetInstance().BSGlobalRetStatus.Count)
                {
                    var Item = StatusDisplayScreenModellList.FirstOrDefault(q => q.Bg_Status_Position_Index == PositionIndex.ToString());
                    List<int> IndexList = DicIndex[PositionIndex];
                    var StatusResult = EquipmentManager.GetInstance().BSGlobalRetStatus[ItemIndex];
                    if (Item == null)
                    {
                        return;
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

                if (ItemIndex < EquipmentManager.GetInstance().BSGlobalRetStatus.Count)
                {
                    var Item = StatusDisplayScreenModellList.FirstOrDefault(q => q.Bg_Status_Position_Index == PositionIndex.ToString());
                    List<int> IndexList = DicIndex[PositionIndex];
                    var StatusResult = EquipmentManager.GetInstance().BSGlobalRetStatus[ItemIndex];
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

                    ///探测器单独判断
                    if (Item.StatusName.Contains("安全联锁"))
                    {
                        //探测器需要去找扫描站的连接状态，如果判断到扫描站的状态是未连接
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
                if (EquipmentManager.GetInstance().BSGlobalRetStatus[item])
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
                    Item.StatusCode = EquipmentManager.GetInstance().BSGlobalRetStatus[trueIndex]?"1":"0";
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
                    Item.StatusCode = (EquipmentManager.GetInstance().BSGlobalRetStatus[Convert.ToInt32(Item.StatusIndex)] == Convert.ToBoolean(Item.DefaultValue)) ? "1" : "0";
                        //(Item.Bg_Own.Contains("SafeStatus") || Item.Bg_Own.Contains("RadiationSourceOrAcceleratorReady") 
                        //             || (Out.Contains(Item.StatusIndex)) || Item.Bg_Own.Contains("SafeSeriers"))  ? "2" : "0";
                }
            }
        }

        /// <summary>
        /// 分组只有一个
        /// </summary>
        /// <param name="Item"></param>
        /// <param name="StatusResult"></param>
        private void GroupOnlyOne(StatusModel Item, bool StatusResult)
        {
            if (Item.Bg_Own.Contains("SafeStatus") || Item.Bg_Own.Contains("RadiationSourceOrAcceleratorReady") ||
                Item.Bg_Own.Contains("SafeSeriers") || Item.Bg_Own.Contains("CarHandStatus"))
            {
                if (!PlcManager.IsConnect())
                {
                    Item.StatusCode = "0";
                }
                else
                {
                    Item.StatusCode = StatusResult == Convert.ToBoolean(Item.DefaultValue)? "2" : "0";
                }

            }
            else if (Item.Bg_Own.Contains("NormalStatus"))
            {
                if (!PlcManager.IsConnect())
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
            
                if (!PlcManager.IsConnect())
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
