using CMW.Common.Utilities;
using BGCommunication;
using BGDAL;
using BGModel;

using BGUserControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.CommonFunc;
using BG_Entities;
using BG_WorkFlow;

namespace BGUserControl
{
    [Export("ContentPage", typeof(BaseModules))]
    [CustomExportMetadata(1, Modules.TrialImageModules, "审像模块", "ZZW", "1.0.0")]
    public class TrialImageModule : BaseModules
    {
        Grid _MainGrid = new Grid();
        ParamaterModel<LocalUploadImage> LocalCondition;
        ObservableCollection<LocalUploadImage> LocalUploadImages = new ObservableCollection<LocalUploadImage>();
        LocalUploadImageBLL llb = new LocalUploadImageBLL();
        List<int> numList = new List<int>() { 5, 10, 30, 100 };
        DataPager dp;
        DiyListView<LocalUploadImage> dlv;
        //ICommand UpLoad 

        [ImportingConstructor]
        public TrialImageModule():base(controlVersion)
        {
            Loaded += TrialImageModule_Loaded;
        }

        private void TrialImageModule_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ButtonInvoke.SwitchLanguageEvent -= SwitchLanguage;
            ButtonInvoke.SwitchLanguageEvent += SwitchLanguage;
            ButtonInvoke.SwitchFontSizeEvent -= SwitchFontSize;
            ButtonInvoke.SwitchFontSizeEvent += SwitchFontSize;
            _MainGrid = InitGrid();
            Content = _MainGrid;
        }

        private Grid InitGrid()
        {
            Grid _MainGrid = MakeMainGrid();
            InitContent(_MainGrid);
            return _MainGrid;
        }

        /// <summary>
        /// 刷新当前图片信息
        /// </summary>
        /// <param name="_LocalUploadImage"></param>
        public void ReflashObject(LocalUploadImage _LocalUploadImage)
        {
            var single = LocalUploadImages.FirstOrDefault(q => q.IMAGE_NAME == _LocalUploadImage.IMAGE_NAME);
            single = _LocalUploadImage;
            dlv.Reflash(LocalUploadImages);
        }

        private void InitContent(Grid MainGrid)
        {
            if (ConfigServices.GetInstance().localConfigModel.TrialImageMode.ToLower() == "local")
            {
                Label lb = new Label()
                {
                    Content = UpdateStatusNameAction("LocalDataMode"),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Style = (Style)this.FindResource("diyLabel"),
                    Foreground = StrToBrush("#1A4F85")
                };
                Grid.SetRow(lb, 1);
                Grid.SetColumn(lb, 0);
                Grid.SetColumnSpan(lb, 2);
                MainGrid.Children.Add(lb);
            }
            else
            {
                InitOnlineImage(MainGrid);
            }
        }

        /// <summary>
        /// 构建集中审像组件
        /// </summary>
        private void InitOnlineImage(Grid MainGrid)
        {
            Grid OnlineGrid = MakeOnlineMainGrid();
            InitDataPager(OnlineGrid);
            InitDataListView(OnlineGrid);
            Grid.SetRow(OnlineGrid, 0);
            Grid.SetRowSpan(OnlineGrid, 3);
            Grid.SetColumn(OnlineGrid, 0);
            Grid.SetColumnSpan(OnlineGrid, 2);
            MainGrid.Children.Add(OnlineGrid);
            dp.btnRefresh_MouseClick(null, null);
        }

        private Grid MakeOnlineMainGrid()
        {
            Grid gd = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            gd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            gd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Pixel) });
            gd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            return gd;
        }

        private void InitDataPager(Grid gd)
        {
            LocalCondition = new ParamaterModel<LocalUploadImage>();
            Border bd = new Border() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            dp = new DataPager(numList) { };
            dp.OnPageGetList += DataPager_OnPageGetList;
            bd.Child = dp;
            Grid.SetColumn(bd, 0);
            Grid.SetRow(bd, 1);
            gd.Children.Add(bd);
        }

        private void InitDataListView(Grid gd)
        {
            dlv = new DiyListView<LocalUploadImage>(LocalUploadImages)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            dlv.DataContext = this;
            Grid.SetColumn(dlv, 0);
            Grid.SetRow(dlv, 0);
            gd.Children.Add(dlv);
        }

        int DataPager_OnPageGetList(int start, int num, ref int count)
        {
            //int tempCount = 0;
            //Task.Run(() =>
            //{
            //    LocalCondition.num = num;
            //    LocalCondition.start = start;
            //    LocalUploadImages = llb.GetLocalUpLoadImage(LocalCondition);        
            //    foreach (var UploadImages in LocalUploadImages)
            //    {
            //        UploadImages.IMAGE_FONTSIZE = UpdateFontSizeAction(CMWFontSize.Normal);
            //    }
            //    Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
            //    {
            //        dlv.Reflash(LocalUploadImages);
            //    }));
            //});
            //count = tempCount;
            //return tempCount;
            LocalCondition.num = num;
            LocalCondition.start = start - 1;
            return SearchImages(count);
        }
        private int SearchImages(int value)
        {
            return SearchImagesAsync(value).Result;
        }

        private Task<int> SearchImagesAsync(int value)
        {
            return Task.Run(() => {
                value = llb.GetAllConut(LocalCondition.Model);
                LocalUploadImages = llb.GetLocalUpLoadImage(LocalCondition);
                foreach (var UploadImages in LocalUploadImages)
                {
                    UploadImages.IMAGE_FONTSIZE = UpdateFontSizeAction(CMWFontSize.Normal);
                    UploadImages.IMAGE_UPLOAD_STATUS = UpdateStatusNameAction(UploadImages.IMAGE_UPLOAD_STATUS);
                }
                Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                {
                    dlv.Reflash(LocalUploadImages);
                }));
                return value;
            });
        }
        private Grid MakeMainGrid()
        {
            Grid _MainGrid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Star) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Star) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Star) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(7, GridUnitType.Star) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(14, GridUnitType.Star) });
            return _MainGrid;
        }

        public void SwitchFontSize(string language)
        {
            TrialImageModule_Loaded(null, null);
        }

        public void SwitchLanguage(string language)
        {
            base.Base_SwitchLanguage(language);
            TrialImageModule_Loaded(null, null);
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

        public override string GetName()
        {
            return "TraialImageMode";
        }

        public override bool IsConnectionEquipment()
        {
            return IsConnection;
        }

        public override double GetHeight()
        {
            if (ConfigServices.GetInstance().localConfigModel.TrialImageMode.ToLower() == "local")
            {
                return 300;
            }
            else { return 600; }
        }

        public override double GetWidth()
        {
            if (ConfigServices.GetInstance().localConfigModel.TrialImageMode.ToLower() == "local")
            {
                return 400;
            }
            else { return 800; }
        }
    }
}
