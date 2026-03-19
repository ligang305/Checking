using CMW.Common.Utilities;
using BGModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonFunc;
using static CMW.Common.Utilities.CommonDeleget;
using BG_Services;
using System.Collections.Generic;
using BG_Entities;

namespace BGUserControl
{
    /// <summary>
    /// DiySafeCondition.xaml 的交互逻辑
    /// </summary>
    public partial class DiySafeCondition : DiyUserModule
    {
        bool? isVisible = false;
        SafeConfitionMvvm safeConfitionMvvm = new SafeConfitionMvvm();
        /// <summary>
        /// 属性，用来接收外部传输进来的数据对象
        /// </summary>
        public ObservableCollection<object> ItemSource
        {
            get
            {
                return (ObservableCollection<object>)GetValue(ItemSourceProperty);
            }
            set { SetValue(ItemSourceProperty, value); }
        }
        /// <summary>
        /// 数据源依赖属性
        /// </summary>
        public static readonly DependencyProperty ItemSourceProperty =
            DependencyProperty.Register("ItemSource", typeof(ObservableCollection<object>), typeof(DiySafeCondition),
                 new PropertyMetadata(new ObservableCollection<object>(), new PropertyChangedCallback(OnValueChange)));

        public string Title
        {
            get
            {
                return (string)GetValue(TitleProperty);
            }
            set { SetValue(TitleProperty, value); }

        }
        /// <summary>
        /// 数据源依赖属性
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(DiySafeCondition),
                 new PropertyMetadata("", new PropertyChangedCallback(OnTitleValueChange)));
        /// <summary>
        /// 接收到值 回调的方法
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiySafeCondition safePanel = d as DiySafeCondition;
            if (safePanel != null && safePanel.ItemSource != null)
            {
                safePanel.lblMainContent.Content = safePanel.MakeHitchPanel(safePanel.ItemSource);
                safePanel.safeConfitionMvvm.ItemSource = safePanel.ItemSource;
            }
        }
        /// <summary>
        /// 接收到值 回调的方法
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnTitleValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiySafeCondition safePanel = d as DiySafeCondition;
            if (safePanel != null && safePanel.ItemSource != null)
            {
                safePanel.TitleName = safePanel.Title;
            }
        }

     
        /// <summary>
        /// 通过数据源来生成状态面板
        /// </summary>
        /// <param name="HitchModelList"></param>
        /// <returns></returns>
        public Grid MakeHitchPanel(ObservableCollection<object> HitchModelList)
        {
            Grid _dp = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch,VerticalAlignment = VerticalAlignment.Top };
            int maxColumnsNum = 5;
            int TempRows = 0;
            int ColumnSpan = Convert.ToInt32(Math.Ceiling(((decimal)HitchModelList.Count() / (decimal)maxColumnsNum)));
            TempRows += ColumnSpan;

            for (int i = 0; i < TempRows; i++)
            {
                _dp.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            }
            for (int i = 0; i < maxColumnsNum; i++)
            {
                
                _dp.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
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
                    BorderBrush = StrToBrush("#AAB8C9"),CornerRadius = new CornerRadius(2),
                    BorderThickness = new Thickness(1),VerticalAlignment = VerticalAlignment.Top,Margin = new Thickness(0,10,0,0)
                };
                DiyStatusLabel lblHitch = new DiyStatusLabel()
                {
                    LblTextForground = (SolidColorBrush)this.TryFindResource("defaultBlack"),
                    FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.Normal),
                    VerticalAlignment = VerticalAlignment.Stretch,HorizontalAlignment = HorizontalAlignment.Stretch,
                };
                if (smItem.Bg_IsClick.ToLower() == "true")
                {
                    OutBorder.Tag = smItem;
                    OutBorder.MouseLeftButtonDown += OutBorder_MouseLeftButtonDown;
                }
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
            return _dp;
        }
        private void OutBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                StatusModel  sm = (sender as Border).Tag as StatusModel;
                CommonDeleget.JummpPageAccordKeyWordEventAction(sm.StatusName);
            }
            catch(Exception ex)
            {
                BGLogs.Log.GetDistance().WriteInfoLogs(ex.Message);
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public DiySafeCondition()
        {
            InitializeComponent();
            SwitchFontSize(); SwitchLanguage();
            safeConfitionMvvm.SwitchFontsizeAction += SwitchFontSize;
            safeConfitionMvvm.SwitchLanguageAction += SwitchLanguage;
            DataContext = safeConfitionMvvm;
        }
        public void SwitchFontSize()
        {
            lblMainContent.Content = MakeHitchPanel(ItemSource);
            lblTitle.Content = UpdateStatusNameAction(GetName());
        }
        public void SwitchLanguage()
        {
            lblMainContent.Content = MakeHitchPanel(ItemSource);
            lblTitle.Content = UpdateStatusNameAction(GetName());
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
            _OwnerWin.MaxHeight = Height< GetHeight()? GetHeight():Height;
            _OwnerWin.Title = GetName();
            lblTitle.Content = UpdateStatusNameAction(GetName());
            _OwnerWin.Content = this;
            _OwnerWin.Show();
        }
    }
    public class SafeConfitionMvvm : BaseMvvm
    {
        public ObservableCollection<object> ItemSource = new ObservableCollection<object>();

        public override void LoadUIText()
        {
            SwitchLanguageAction?.Invoke();
        }

        public SafeConfitionMvvm()
        {
       
        }
    }
}
