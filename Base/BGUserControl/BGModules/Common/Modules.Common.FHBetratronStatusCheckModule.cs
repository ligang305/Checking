using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using BG_Services;
using CMW.Common.Utilities;
using BGCommunication;

using BGModel;
using BGUserControl;
using static CMW.Common.Utilities.CommonFunc;
using BG_Entities;
using BG_WorkFlow;

namespace BGUserControl
{
    [Export("ContentPage", typeof(BaseModules))]
    [CustomExportMetadata(1, Modules.FHBetratronStatusCheckModule, "泛化加速器状态监测模块（非设置模块）", "ZZW", "1.0.0")]
    public class FHBetratronStatusCheckModule : BaseModules
    {
        private ObservableCollection<DoseModel> BoostingList = new ObservableCollection<DoseModel>();
        CarDoseBLL _carDoseBll = new CarDoseBLL();
        Grid _MainGrid = new Grid();
        Border _MainBorder = new Border()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Margin = new Thickness(0),
            BorderThickness = new Thickness(2),
            Background = Brushes.White,
        };
        bool? isVisible = false;

        [ImportingConstructor]
        public FHBetratronStatusCheckModule() : base(ControlVersion.CombinedMovementBetatron)
        {
            Loaded += FHBetratronStatusCheckModule_Loaded;
            Unloaded += FHBetratronStatusCheckModule_Unloaded;
            ButtonInvoke.SwitchLanguageEvent -= SwitchLanguage;
            ButtonInvoke.SwitchLanguageEvent += SwitchLanguage;
            ButtonInvoke.SwitchFontSizeEvent -= SwitchFontSize;
            ButtonInvoke.SwitchFontSizeEvent += SwitchFontSize;
            IsVisibleChanged += FHBetratronStatusCheckModule_IsVisibleChanged;
        }

        private void FHBetratronStatusCheckModule_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            isVisible = e.NewValue as bool?;
            if (isVisible != null && isVisible == true)
            {
                Task.Factory.StartNew(() => { GetSignalStatus(); });
            }
        }
        public void SwitchFontSize(string FontSize)
        {
            FHBetratronStatusCheckModule_Loaded(null, null);
        }
        public void SwitchLanguage(string language)
        {
            base.Base_SwitchLanguage(language);
            FHBetratronStatusCheckModule_Loaded(null, null);
        }
        private void FHBetratronStatusCheckModule_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            
        }

        private void FHBetratronStatusCheckModule_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            InitConfigs();
        }
        /// <summary>
        /// 获取剂量初始化方法
        /// </summary>
        private void InitDoseList()
        {
            BoostingList.Clear();
            _carDoseBll.GetDoseModelDataModel(SystemDirectoryConfig.GetInstance().GetBetratron_FHBoostingStatus
                    (ControlVersion.CombinedMovementBetatron)).Where(q => q.BgIsShow == "1")
                .ToList().ForEach(q =>
                {
                    BoostingList.Add(q);
                });
        }
        /// <summary>
        /// 初始化配置文件加载到内存中
        /// </summary>
        private void InitConfigs()
        {
            InitDoseList();
            InitUI();
        }
        /// <summary>
        /// 通过加载出来的
        /// </summary>
        private void InitUI()
        {
            _MainGrid = InitGrid();
            InitData();
            _MainBorder.Child = _MainGrid;
            Content = _MainBorder;
            TaskPool.GetInstance().AddAndStartTask(TaskList.CheckBetratron_FH, GetSignalStatus);
        }


        private Grid InitGrid()
        {
            Grid _MainGrid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            int ColumnNum = 4;
            int Rows = BoostingList.Count / ColumnNum + (BoostingList.Count % ColumnNum == 0 ? 0 : 1);
        

            for (int i = 0; i < Rows + 1; i++)
            {
                _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(80, GridUnitType.Pixel) });
            }
            for (int i = 0; i < ColumnNum ; i++)
            {
                _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(80, GridUnitType.Star) });
            }
            return _MainGrid;
        }
        /// <summary>
        /// 将数字和UI进行绑定
        /// </summary>
        private void InitData()
        {
            foreach (var BoostingItem in BoostingList)
            {
                int rowIndex = BoostingList.IndexOf(BoostingItem) / 4 ;
                int colIndex = Convert.ToInt32(BoostingItem.BgDoseIndex) % 4;
                var gridElement =  MakeSignalPanel(BoostingItem);
                Grid.SetRow(gridElement,rowIndex + 1);
                Grid.SetColumn(gridElement,colIndex);
                _MainGrid.Children.Add(gridElement);
            }
        }
        /// <summary>
        /// 生成单个信号控件
        /// </summary>
        /// <param name="dm"></param>
        /// <returns></returns>
        public UIElement MakeSignalPanel(DoseModel dm)
        {
            Border MainBorder = new Border()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(10, 0, 10, 5),
                BorderThickness = new Thickness(1,1,1,1),
                BorderBrush = CommonFunc.StrToBrush("#017CD1"),
                MaxHeight = 75
            };
            Grid ElementGd = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            ElementGd.RowDefinitions.Add(new RowDefinition(){Height =  new GridLength(30,GridUnitType.Pixel)});
            ElementGd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Pixel) });
            ElementGd.ColumnDefinitions.Add(new ColumnDefinition(){Width = new GridLength(1,GridUnitType.Star)});
            Label lblTitle = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Center, 
                VerticalAlignment = VerticalAlignment.Stretch,
                Foreground = Brushes.White,
                Background = CommonFunc.StrToBrush("#017CD1"),
                FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.Small),
                FontFamily = new FontFamily("MicrosoftYaHei"),
                //Width = 100,
                //Height = 80,
            };
            Label lblValue = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Stretch,
                Foreground = CommonFunc.StrToBrush("#017CD1"),
                Background = Brushes.White,
                FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.Small),
                FontFamily = new FontFamily("MicrosoftYaHei"),
                //Width = 100,
                //Height = 80,
            };
            dm.BgDoseName = CommonDeleget.UpdateStatusNameAction(dm.BgDoseName);
            lblTitle.SetBinding(Label.ContentProperty, new Binding("BgDoseName") {Source = dm});
            lblValue.SetBinding(Label.ContentProperty, new Binding("BgIsShow") { Source = dm });
            ElementGd.Children.Add(lblTitle);
            ElementGd.Children.Add(lblValue);
            Grid.SetRow(lblTitle,0);
            Grid.SetRow(lblValue, 1);
            MainBorder.Child = ElementGd;
            return MainBorder;
        }
        /// <summary>
        /// 线程获取状态
        /// </summary>
        private void GetSignalStatus()
        {
            while (true)
            {
                if (isVisible == null || isVisible == false) break;
                Thread.Sleep(150);
                foreach (var BoostingItem in BoostingList)
                {
                    FindBetratronStatus(BoostingItem);
                }
            }
        }

        private void FindBetratronStatus(DoseModel dm)
        {
            switch (dm.BgDoseIndex)
            {
                case "0":
                    dm.BgIsShow = BoostingControllerManager.GetInstance().GetActureInject();// (Common.GlobalBetatronProtocol as BetratronProtocolForFH).FHParameter.InjecttionCurrent.ToString();
                    break;
                case "1":
                    dm.BgIsShow = BoostingControllerManager.GetInstance().GetIGBTTemp();// (Common.GlobalBetatronProtocol as BetratronProtocolForFH).FHParameter.IGBTTransistors.ToString();
                    break;
                case "2":
                    dm.BgIsShow = BoostingControllerManager.GetInstance().GetThyristor();// (Common.GlobalBetatronProtocol as BetratronProtocolForFH).FHParameter.Thyristor.ToString();
                    break;
                case "3":
                    dm.BgIsShow = BoostingControllerManager.GetInstance().GetPulseConverterTemperature(); //(Common.GlobalBetatronProtocol as BetratronProtocolForFH).FHParameter.PulseConverter.ToString();
                    break;
                case "4":
                    dm.BgIsShow = BoostingControllerManager.GetInstance().GetRadiatorTemperature(); //(Common.GlobalBetatronProtocol as BetratronProtocolForFH).FHParameter.Radiator.ToString();
                    break;
                case "5":
                    dm.BgIsShow = BoostingControllerManager.GetInstance().GetDACValueOfFilament(); //((Common.GlobalBetatronProtocol as BetratronProtocolForFH).FHParameter.DACOfFilament / 150).ToString();
                    break;
                case "6":
                    dm.BgIsShow = BoostingControllerManager.GetInstance().GetInjectDACValue(); //(Common.GlobalBetatronProtocol as BetratronProtocolForFH).FHParameter.DACOfInjector.ToString();
                    break;
                case "7":
                    dm.BgIsShow = BoostingControllerManager.GetInstance().GetConstrainerDACValue(); //(Common.GlobalBetatronProtocol as BetratronProtocolForFH).FHParameter.DACOfContractor.ToString();
                    break;
                case "8":
                    dm.BgIsShow = BoostingControllerManager.GetInstance().MaximumDoseRateSearchRrogress(); //(Common.GlobalBetatronProtocol as BetratronProtocolForFH).FHParameter.RSTProcess.ToString();
                    break;
                case "9":
                    dm.BgIsShow = BoostingControllerManager.GetInstance().DoseRate(); //((Common.GlobalBetatronProtocol as BetratronProtocolForFH).FHParameter.dr.Doserate / 100.00).ToString();
                    break;
            }
        }

    }
}
