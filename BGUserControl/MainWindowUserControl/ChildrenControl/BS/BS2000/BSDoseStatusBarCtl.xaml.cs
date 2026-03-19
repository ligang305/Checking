using BG_Services;
using CMW.Common.Utilities;
using BGModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using BG_WorkFlow;
using BG_Entities;
using BGUserControl;

namespace BGUserControl
{
    /// <summary>
    /// BSDoseStatusBarCtl.xaml 的交互逻辑
    /// </summary>
    public partial class BSDoseStatusBarCtl : UserControl
    {
        BSDoseStatusBarMvvm doseStatusBarMvvm = new BSDoseStatusBarMvvm();
        public BSDoseStatusBarCtl()
        {
            InitializeComponent();
            DataContext = doseStatusBarMvvm;
            InitDoseStatusBar();
        }

        /// <summary>
        /// 初始化右侧剂量值面板
        /// </summary>
        private  void InitDoseStatusBar()
        {
            doseStatusBarMvvm.ParamaterLabelList.Clear();
            paramaterGrid.Children.Clear();
            if (doseStatusBarMvvm.ParamaterLabelList.Count == (doseStatusBarMvvm.DataSource as ObservableCollection<DoseModel>).Count)
            {
                return;
            }
            foreach (var item in doseStatusBarMvvm.DataSource)
            {
                ParamaterLabel pl = new ParamaterLabel();
                pl.ParamaterName = UpdateStatusNameAction((item as DoseModel).BgDoseName);
                pl.ParamaterNameFontSize = UpdateFontSizeAction(CMWFontSize.Middle);
                if (Convert.ToInt32((item as DoseModel).BgDoseIndex)  <= GlobalDoseStatus.Count)
                {
                    pl.ParamaterValue = (item as DoseModel).BgDoseIndex == "0" ? string.Empty : GlobalDoseStatus[Convert.ToInt32((item as DoseModel).BgDoseIndex) - 1].ToString();
                }
                doseStatusBarMvvm.ParamaterLabelList.Add(pl);
                ReflashDoseGrid(pl);
            }
        }


        private void ReflashDoseGrid(ParamaterLabel pl)
        {
            DisplayParamaterLabel displayParamaterLabel = new DisplayParamaterLabel(pl);
            paramaterGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });
            paramaterGrid.Children.Add(displayParamaterLabel);
            Grid.SetColumn(displayParamaterLabel, 0);
            Grid.SetRow(displayParamaterLabel, paramaterGrid.Children.Count - 1);
        }

        private void ReflashBSDoseGrid(ParamaterLabel pl)
        {
            DisplayParamaterLabel displayParamaterLabel = new DisplayParamaterLabel(pl);
            paramaterGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });
            paramaterGrid.Children.Add(displayParamaterLabel);
            Grid.SetColumn(displayParamaterLabel, 0);
            Grid.SetRow(displayParamaterLabel, paramaterGrid.RowDefinitions.Count-1);
        }
    }

    public class BSDoseStatusBarMvvm : BaseMvvm
    {
        private string controlNamePanel;

        public string ControlNamePanel
        {
            get => controlNamePanel;
            set
            {
                controlNamePanel = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<DoseModel> DataSource = new ObservableCollection<DoseModel>();
        public ObservableCollection<ParamaterLabel> ParamaterLabelList = new ObservableCollection<ParamaterLabel>();
        public CarDoseBLL _carDoseBll = new CarDoseBLL();

        public BSDoseStatusBarMvvm()
        {
            InitDoseList();
            ControlNamePanel = UpdateStatusNameAction(ControlVersion.BS.ToString());
        }

        public void InitDoseList()
        {
            DataSource.Clear();
            _carDoseBll.GetDoseModelDataModel(SystemDirectoryConfig.GetInstance().GetDoseConfig(ControlVersion.BS)).Where(q => q.BgIsShow == "1")
                .ToList().ForEach(q => { DataSource.Add(q); });
        }

        protected override void ConnectionStatus(bool ConnectStatus)
        {
           
        }


        protected override void BSConnectionStatus(bool ConnectStatus)
        {
            /*
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
            */
            Application.Current.Dispatcher.InvokeAsync(() =>
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
            });
            
        }

        private string Unit(DoseModel Key)
        {
            if(!string.IsNullOrEmpty(Key.BgUnit))
            {
                return $@"  {Key.BgUnit}";
            }
            if(Key.BgDoseName.Contains("Temperature"))
            {
                return " ℃";
            }
            else if(Key.BgDoseName.Contains("CarSpeed"))
            {
                if(!PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.ScanMode))
                {
                    return " km/h";
                }
                else
                {
                    return " m/s";
                }
            }
            else if(Key.BgDoseName.Contains("Pressure"))
            {
                return " Bar";
            }
            else if(Key.BgDoseName.Contains("Ranging"))
            {
                return " mm";
            }
            else if(Key.BgDoseName.Contains("OilTankSurplus"))
            {
                return " %";
            }
            else if (Key.BgDoseName.Contains("CarLength"))
            {
                return " m";
            }
            else if(Key.BgDoseName.Contains("Humidity"))
            {
                return " %";
            }
            else if(Key.BgDoseName.Contains("Vibrate"))
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
