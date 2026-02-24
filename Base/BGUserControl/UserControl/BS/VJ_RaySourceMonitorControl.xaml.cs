using BG_Entities;
using BG_Services;
using BG_WorkFlow;
using BGModel;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;

namespace BGUserControl
{
    /// <summary>
    /// VJ_RaySourceMonitorControl.xaml 的交互逻辑
    /// 用于显示单个视角VJ射线源 状态值的控件
    /// </summary>
    public partial class VJ_RaySourceMonitorControl : UserControl
    {
        VJ_RaySourceMonitorControlViewModel vJ_RaySourceMonitorControlViewModel = new VJ_RaySourceMonitorControlViewModel();
        /// <summary>
        /// 属性，用来接收外部传输进来的数据对象
        /// </summary>
        public string ItemSource
        {
            get
            {
                return (string)GetValue(ItemSourceProperty);
            }
            set { SetValue(ItemSourceProperty, value); }
        }

        /// <summary>
        /// 数据源依赖属性
        /// </summary>
        public static readonly DependencyProperty ItemSourceProperty =
            DependencyProperty.Register("ItemSource", typeof(string), typeof(VJ_RaySourceMonitorControl),
                 new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnValueChange)));
        public VJ_RaySourceMonitorControl()
        {
            InitializeComponent();
            DataContext = vJ_RaySourceMonitorControlViewModel;

        }

        /// <summary>
        /// 接收到值 回调的方法
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VJ_RaySourceMonitorControl VJ_RaySourceMonitorControl = d as VJ_RaySourceMonitorControl;
            if (VJ_RaySourceMonitorControl != null && VJ_RaySourceMonitorControl.ItemSource != null)
            {
                VJ_RaySourceMonitorControl.InitBSDoseStatusBar();
            }
        }

        /// <summary>
        /// 如果是H986设备 新加BS设备 则执行下面方法显示背散的剂量窗口
        /// </summary>
        private void InitBSDoseStatusBar()
        {
    
            vJ_RaySourceMonitorControlViewModel.InitDoseList(ItemSource);
            paramaterGrid.RowDefinitions.Clear();
            vJ_RaySourceMonitorControlViewModel.BSParamaterLabelList.Clear();
            if (vJ_RaySourceMonitorControlViewModel.BSParamaterLabelList.Count ==
                (vJ_RaySourceMonitorControlViewModel.DataSource as ObservableCollection<DoseModel>).Count)
            {
                return;
            }
            foreach (var item in vJ_RaySourceMonitorControlViewModel.DataSource)
            {
                ParamaterLabel pl = new ParamaterLabel();
                pl.ParamaterName = UpdateStatusNameAction((item as DoseModel).BgDoseName);
                pl.ParamaterNameFontSize = UpdateFontSizeAction(CMWFontSize.Middle);
                DoseModel DoseModelItem = item as DoseModel;

                if (DoseModelItem == null) return;

                if(DoseModelItem.BG_ValueType.ToLower() == "float" || string.IsNullOrEmpty(DoseModelItem.BG_ValueType.ToLower()))
                {
                    var count = EquipmentManager.GetInstance().plcValusStatus.FloatArray?.Count;
                    if (Convert.ToInt32((item as DoseModel).BgDoseIndex) <= count)
                    {
                        pl.ParamaterValue = (item as DoseModel).BgDoseIndex == "0" ? string.Empty : EquipmentManager.GetInstance().plcValusStatus.FloatArray[Convert.ToInt32((item as DoseModel).BgDoseIndex)].ToString();
                        pl.ParamaterForeColor = "#014087";
                    }
                }
                else if(DoseModelItem.BG_ValueType.ToLower() == "ushort")
                {
                    var count = EquipmentManager.GetInstance().plcValusStatus.IntArray?.Count;
                    if (Convert.ToInt32((item as DoseModel).BgDoseIndex) <= count)
                    {
                        pl.ParamaterValue = (item as DoseModel).BgDoseIndex == "0" ? string.Empty : EquipmentManager.GetInstance().plcValusStatus.IntArray[Convert.ToInt32((item as DoseModel).BgDoseIndex)].ToString();
                        pl.ParamaterForeColor = "#014087";
                    }
                }
                vJ_RaySourceMonitorControlViewModel.BSParamaterLabelList.Add(pl);
                ReflashBSDoseGrid(pl);

            }
        }
        private void ReflashBSDoseGrid(ParamaterLabel pl)
        {
            DisplayParamaterLabel displayParamaterLabel = new DisplayParamaterLabel(pl) { DisplayParamaterLabelHeight = 30, DisplayParamaterLabelWidth = 240 };
            paramaterGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Star) });
            paramaterGrid.Children.Add(displayParamaterLabel);
            Grid.SetColumn(displayParamaterLabel, 0);
            Grid.SetRow(displayParamaterLabel, paramaterGrid.RowDefinitions.Count - 1);
        }
    }
}
