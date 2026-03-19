using CMW.Common.Utilities;
using BGCommunication;
using BGModel;
using BGUserControl;
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
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
using BG_Entities;

namespace BGUserControl
{
    [Export("ContentPage", typeof(BaseModules))]
    [CustomExportMetadata(1, Modules.DetectorBoardMonitorModule, "探测器板链监测模块", "ZZW", "1.0.0")]
    public class DetectorBoardMonitorModule:BaseModules
    {
        [ImportingConstructor]
        public DetectorBoardMonitorModule() : base(ControlVersion.Car)
        {
            Loaded += DetectorBoardMonitorModule_Loaded;
        }
        ObservableCollection<DetectorBoard> DetectorBoardList = new ObservableCollection<DetectorBoard>();
        Dictionary<int, int> BoardLineDic = new Dictionary<int, int>();
        private void DetectorBoardMonitorModule_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(()=> {
                Common.IsExistBoardError = ImageImportDll.SX_GetBoardError(ImageImportDll.intPtr, out Common.BoardNum,
                out Common.BoardLine, out Common.BoardLineIndex) == 1;
                int TotalBoardNum = 0;
               
                for (int i = 0; i < 10; i++)
                {
                    int Result =  ImageImportDll.SX_GetBoardNumber(ImageImportDll.intPtr,out TotalBoardNum,i);
                    if (Result == 0)
                    {
                        BoardLineDic[i] = TotalBoardNum;
                        //Common.BoardNum += TotalBoardNum;
                    }
                }
                this.Dispatcher.BeginInvoke((Action)delegate () { InitContent(); });
            } );
            InitAction();
        }

        public string TabName = string.Empty;
        Grid _MainGrid = new Grid();
        ScrollViewer  _MainBorder = new ScrollViewer()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Margin = new Thickness(0),
            BorderBrush = CommonFunc.StrToBrush("#3F96E6"),
        } ;
        #region 接口
        public override void SetSelectTabName(string TabName)
        {
            this.TabName = TabName;
        }
        public override string GetName()
        {
            return "探测器板监测";
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
        private void InitContent()
        {
            _MainGrid = InitGrid();
            _MainBorder.Content = _MainGrid;
            Content = _MainBorder;

        }
        private void InitAction()
        {
            ButtonInvoke.SwitchLanguageEvent -= SwitchLanguage;
            ButtonInvoke.SwitchLanguageEvent += SwitchLanguage;

            ButtonInvoke.SwitchFontSizeEvent -= SwitchFontSize;
            ButtonInvoke.SwitchLanguageEvent += SwitchFontSize;
        }
        public void SwitchFontSize(string FontSize)
        {
            DetectorBoardMonitorModule_Loaded(null, null);
        }
        public void SwitchLanguage(string language)
        {
            base.Base_SwitchLanguage(language);
            DetectorBoardMonitorModule_Loaded(null, null);
        }



        private Grid InitGrid()
        {
            Grid _MainGrid = InitMainGird();
            Grid _ContentGrid = ContentGrid();
            Grid.SetColumn(_ContentGrid,1);
            _MainGrid.Children.Add(_ContentGrid);
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
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            return _MainGrid;
        }

        public Grid ContentGrid()
        {
            Grid gd = new Grid()
            {HorizontalAlignment = HorizontalAlignment.Stretch,VerticalAlignment=VerticalAlignment.Stretch};
            int RowNum = Common.BoardNum / 8 + (Common.BoardNum % 8 == 0 ? 0 : 1);
            int ColumnNum = 8;
            for (int i = 0; i < RowNum; i++)
            {
                gd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(200,GridUnitType.Pixel)});
            }
            for (int i = 0; i < ColumnNum; i++)
            {
                gd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50, GridUnitType.Star) });
            }
            for (int i = 0; i < Common.BoardNum; i++)
            {
                DiyDetectorPanel detectorPanel = new DiyDetectorPanel() {Margin = new Thickness(3) ,Height = 200,Width = 100, HorizontalAlignment = HorizontalAlignment.Stretch,HorizontalContentAlignment =HorizontalAlignment.Center,VerticalAlignment = VerticalAlignment.Center};
                Grid.SetRow(detectorPanel,i / 8);
                Grid.SetColumn(detectorPanel, i % 8);
                gd.Children.Add(detectorPanel);
                detectorPanel.BoardIndex = (i+1).ToString();
                detectorPanel.BoardLineIndex = GetBoardLine(i).ToString();
                if (BoardLine <= Convert.ToInt32(detectorPanel.BoardLineIndex) && Common.BoardLineIndex !=0 && Common.BoardLineIndex <= GetBoardLineIndex(i))
                {
                    detectorPanel.StatusPreColor = Brushes.Red;
                    detectorPanel.BoardStatus = "AbNormal";
                    continue;
                }
                detectorPanel.StatusPreColor = Brushes.Green;
                detectorPanel.BoardStatus = "Normal";
            }
            return gd;
        }

        /// <summary>
        /// 通过当前索引来判断是第几条链
        /// </summary>
        /// <param name="CurrentIndex"></param>
        /// <returns></returns>
        public int GetBoardLine(int CurrentIndex)
        {
            int TempInt = 0;
            foreach (var item in BoardLineDic)
            {
                TempInt += item.Value;
                if (TempInt > CurrentIndex)
                    return item.Key +1 ;
            }
            return BoardLineDic.Keys.Max() + 1;
        }
        /// <summary>
        /// 通过当前索引来判断是第几条链
        /// </summary>
        /// <param name="CurrentIndex"></param>
        /// <returns></returns>
        public int GetBoardLineIndex(int CurrentIndex)
        {
            int TempInt = 0;
            foreach (var item in BoardLineDic)
            {
                TempInt += item.Value;
                if (TempInt > CurrentIndex)
                {
                     return item.Value - (TempInt - CurrentIndex);
                }
            }
            return 0;
        }

}
}
