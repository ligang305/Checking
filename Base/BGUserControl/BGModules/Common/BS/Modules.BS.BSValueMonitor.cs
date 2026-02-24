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
    [CustomExportMetadata(1, Modules.BSValueMonitor, "背散值类型监测面板", "ZZW", "1.0.0")]
    public class BSValueMonitor : BSBaseModules
    {
        Grid paramaterGrid = new Grid()
        {
            HorizontalAlignment= HorizontalAlignment.Stretch,
            VerticalAlignment= VerticalAlignment.Stretch
        };
        BSValueMonitorMvvm BSValueMonitorMvvm = new BSValueMonitorMvvm();
        [ImportingConstructor]
        public BSValueMonitor() : base(ControlVersion.BS)
        {
            Loaded += BSValueMonitorMvvm_Loaded;
            DataContext = BSValueMonitorMvvm;
            BSValueMonitorMvvm.SwitchLanguageAction -= SwitchLanguage;
            BSValueMonitorMvvm.SwitchLanguageAction += SwitchLanguage;
        }
        private void BSValueMonitorMvvm_Loaded(object sender, RoutedEventArgs e)
        {
            InitDoseStatusBar();
            Content = paramaterGrid;
        }
        /// <summary>
        /// 初始化右侧剂量值面板
        /// </summary>
        private void InitDoseStatusBar()
        {
            BSValueMonitorMvvm.ParamaterLabelList.Clear();
            paramaterGrid.Children.Clear();
            paramaterGrid.ColumnDefinitions.Clear();
            int Colums =  4;
            for (int i = 0; i < Colums; i++)
            {
                paramaterGrid.ColumnDefinitions.Add(new ColumnDefinition() {Width = new GridLength(1,GridUnitType.Star) });
            }
            int Rows = BSValueMonitorMvvm.DataSource.Count / Colums + (BSValueMonitorMvvm.DataSource.Count % Colums == 0 ? 0 : 1);
            for (int i = 0; i < Rows; i++) 
            {
                paramaterGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });
            }

            if (BSValueMonitorMvvm.ParamaterLabelList.Count == (BSValueMonitorMvvm.DataSource as ObservableCollection<DoseModel>).Count)
            {
                return;
            }
            foreach (var item in BSValueMonitorMvvm.DataSource)
            {
                ParamaterLabel pl = new ParamaterLabel();
                pl.ParamaterName = UpdateStatusNameAction((item as DoseModel).BgDoseName);
                pl.ParamaterNameFontSize = UpdateFontSizeAction(CMWFontSize.Middle);
                if (Convert.ToInt32((item as DoseModel).BgDoseIndex) <= GlobalDoseStatus.Count)
                {
                    pl.ParamaterValue = (item as DoseModel).BgDoseIndex == "0" ? string.Empty : GlobalDoseStatus[Convert.ToInt32((item as DoseModel).BgDoseIndex) - 1].ToString();
                }
                BSValueMonitorMvvm.ParamaterLabelList.Add(pl);
                ReflashDoseGrid(pl, BSValueMonitorMvvm.DataSource.IndexOf(item));
            }
        }


        private void ReflashDoseGrid(ParamaterLabel pl,int Index)
        {
            DisplayParamaterLabel displayParamaterLabel = new DisplayParamaterLabel(pl);
            paramaterGrid.Children.Add(displayParamaterLabel);
            Grid.SetColumn(displayParamaterLabel, Index % 4);
            Grid.SetRow(displayParamaterLabel, Index / 4);
        }

        private void ReflashBSDoseGrid(ParamaterLabel pl)
        {
            DisplayParamaterLabel displayParamaterLabel = new DisplayParamaterLabel(pl);
            paramaterGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });
            paramaterGrid.Children.Add(displayParamaterLabel);
            Grid.SetColumn(displayParamaterLabel, 0);
            Grid.SetRow(displayParamaterLabel, paramaterGrid.RowDefinitions.Count - 1);
        }

        public void SwitchLanguage()
        {
            InitDoseStatusBar();
        }

        /// <summary>
        /// 获取名称
        /// </summary>
        /// <returns></returns>
        public override string GetName()
        {
            return "BSValueMonitor";
        }

        public override double GetWidth()
        {
            return 1000;
        }

        public override double GetHeight()
        {
            return 1000;
        }
    }

    public class BSValueMonitorMvvm : BaseMvvm
    {
        public ObservableCollection<DoseModel> DataSource = new ObservableCollection<DoseModel>();
        public ObservableCollection<ParamaterLabel> ParamaterLabelList = new ObservableCollection<ParamaterLabel>();
        public CarDoseBLL _carDoseBll = new CarDoseBLL();

        public BSValueMonitorMvvm()
        {
            InitDoseList();
        }

        /// <summary>
        /// 初始化列表
        /// </summary>
        public void InitDoseList()
        {
            DataSource.Clear();
            _carDoseBll.GetDoseModelDataModel(SystemDirectoryConfig.GetInstance().GetDoseConfig(ControlVersion.BS)).Where(q => q.BgIsShow != "0")
                .ToList().ForEach(q => { DataSource.Add(q); });
        }
        protected override void BSConnectionStatus(bool ConnectStatus)
        {
            foreach (var item in DataSource)
            {
                int Index = DataSource.IndexOf(item);
                if (item.BgDoseName.Contains("Dose") && item.BgDoseName.Length <= 6)
                {
                    int DoseIndex = Convert.ToInt32(item.BgDoseName.TrimStart('D').TrimStart('o').TrimStart('s').TrimStart('e'));
                    if (DoseServices.Service.GlogbalDoseProtocols.ContainsKey(DoseIndex))
                    {
                        ParamaterLabelList[Index].ParamaterValue = CommonFunc.StrToDose(DoseServices.Service.GlogbalDoseProtocols[DoseIndex].ReadStr);
                        if (EquipmentManager.GetInstance().PlcManager.GetStatusByPositionEnum(item.BgDoseName))
                        {
                            ParamaterLabelList[Index].ParamaterForeColor = "#FF0000";
                        }
                        else
                        {
                            ParamaterLabelList[Index].ParamaterForeColor = "#014087";
                        }
                        continue;
                    }
                }
                if (item.BgDoseName.ToUpper() == "CARSPEED" || item.BgDoseName.ToUpper() == "CARLENGTH")
                {
                    ParamaterLabelList[Index].ParamaterValue = item.BgDoseIndex == "0" ? string.Empty :
                        (EquipmentManager.GetInstance().BSGlobalDoseStatus[Convert.ToInt32(item.BgDoseIndex) - 1] / 100.00f).ToString("F2") + Unit(item);
                    continue;
                }
                else if (Convert.ToInt32(item.BgDoseIndex) < 25)
                {
                    double multiple = 10;
                    if (!string.IsNullOrEmpty(item.BG_multiple))
                    {
                        multiple = Convert.ToDouble(item.BG_multiple);
                    }
                    if (multiple == 0) multiple = 10;

                    ParamaterLabelList[Index].ParamaterValue = (item.BgDoseIndex == "0" ? string.Empty :
                        (EquipmentManager.GetInstance().BSGlobalDoseStatus[Convert.ToInt32(item.BgDoseIndex) - 1] / multiple).ToString("F2"))
                        + Unit(item);

                    if (item.BgDoseName.Contains("HangingLeftRanging") || item.BgDoseName.Contains("HangingRightRanging"))
                    {
                        if (EquipmentManager.GetInstance().PlcManager.GetStatusByPositionEnum(item.BgDoseName))
                        {
                            ParamaterLabelList[Index].ParamaterForeColor = "#FF0000";
                        }
                        else
                        {
                            ParamaterLabelList[Index].ParamaterForeColor = "#014087";
                        }
                    }
                    if (item.BgDoseName.Contains("OilTankSurplus"))
                    {
                        if (EquipmentManager.GetInstance().BSGlobalDoseStatus[Convert.ToInt32(item.BgDoseIndex) - 1] / 10.00f < 15)
                        {
                            ParamaterLabelList[Index].ParamaterForeColor = "#FF0000";
                        }
                        else
                        {
                            ParamaterLabelList[Index].ParamaterForeColor = "#014087";
                        }
                    }
                }
                else
                {
                    ParamaterLabelList[Index].ParamaterValue = GetBetratronInterDose(Convert.ToInt32(item.BgDoseIndex));
                }
            }
        }

        private string Unit(DoseModel Key)
        {
            if (!string.IsNullOrEmpty(Key.BgUnit))
            {
                return $@"  {Key.BgUnit}";
            }
            if (Key.BgDoseName.Contains("Temperature"))
            {
                return " ℃";
            }
            else if (Key.BgDoseName.Contains("CarSpeed"))
            {
                if (!PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.ScanMode))
                {
                    return " km/h";
                }
                else
                {
                    return " m/s";
                }
            }
            else if (Key.BgDoseName.Contains("Pressure"))
            {
                return " Bar";
            }
            else if (Key.BgDoseName.Contains("Ranging"))
            {
                return " mm";
            }
            else if (Key.BgDoseName.Contains("OilTankSurplus"))
            {
                return " %";
            }
            else if (Key.BgDoseName.Contains("CarLength"))
            {
                return " m";
            }
            else if (Key.BgDoseName.Contains("Humidity"))
            {
                return " %";
            }
            else if (Key.BgDoseName.Contains("Vibrate"))
            {
                return " mm/s";
            }
            return "";
        }


        public override void LoadUIText()
        {
            foreach (var item in DataSource)
            {
                int Index = DataSource.IndexOf(item);
                ParamaterLabelList[Index].ParamaterName = UpdateStatusNameAction(item.BgDoseName);
                ParamaterLabelList[Index].ParamaterFontSize = UpdateFontSizeAction(CMWFontSize.NormalMiddle);
                ParamaterLabelList[Index].ParamaterNameFontSize = UpdateFontSizeAction(CMWFontSize.Middle);
            }
        }
        public override void LoadUIFontSize()
        {
            foreach (var item in DataSource)
            {
                int Index = DataSource.IndexOf(item);
                ParamaterLabelList[Index].ParamaterName = UpdateStatusNameAction(item.BgDoseName);
                ParamaterLabelList[Index].ParamaterFontSize = UpdateFontSizeAction(CMWFontSize.NormalMiddle);
                ParamaterLabelList[Index].ParamaterNameFontSize = UpdateFontSizeAction(CMWFontSize.Middle);
            }
        }
        /// <summary>
        /// 根据不同的序号 去不同的值中找到需要的
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        private string GetBetratronInterDose(int Index)
        {
            switch (Index)
            {
                case 26:
                    return BoostingControllerManager.GetInstance().DoseRate();
                case 27:
                    return BoostingControllerManager.GetInstance().GetThyristor();
                case 28:
                    return BoostingControllerManager.GetInstance().GetPulseConverterTemperature();
                case 29:
                    return BoostingControllerManager.GetInstance().GetRadiatorTemperature();
                case 30:
                    return BoostingControllerManager.GetInstance().GetActureInject();
                case 31:
                    return BoostingControllerManager.GetInstance().GetActureInject();
                case 32:
                    return BoostingControllerManager.GetInstance().GetIGBTTemp();
                default:
                    return "0";

            }
        }
    }
}
