using BG_Entities;
using BG_Services;
using BGModel;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shell;
using System.Windows.Threading;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.CommonFunc;
using Timer = System.Timers.Timer;

namespace BGUserControl
{
    public class BG_STOP : BaseWindows
    {
        private Grid MainGrid;
        private Border OutBorder = new Border()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Margin = new Thickness(0),
            BorderThickness = new Thickness(2),
            BorderBrush = StrToBrush("#7c0505"),
            CornerRadius = new CornerRadius(6),
            Background = StrToBrush("#F2F2F2")
        };
        private static string _Title;
        public object _Content;
        public static double Height = 300;
        public static double Width = 600;
        public Dictionary<string, PLCPositionEnum> ContentLists = new Dictionary<string, PLCPositionEnum>();
        private StackPanel Contents;
        SoundPlayer soundPlayer;
        BgStopMvvm bgStopMvvm = new BgStopMvvm();
        public BG_STOP()
        {
            ReadyTimer.Interval = 100;
            ReadyTimer.Elapsed += ReadyTimer_Elapsed;
            if (File.Exists(SystemDirectoryConfig.GetInstance().GetAlarmMusic()))
            {
                soundPlayer = new SoundPlayer();
                soundPlayer.SoundLocation = SystemDirectoryConfig.GetInstance().GetAlarmMusic();
                soundPlayer.Load();
            }
            Loaded += BG_STOP_Loaded;
            IsVisibleChanged += BG_STOP_IsVisibleChanged;
            bgStopMvvm.SwitchLanguageAction -= SwitchLanguage;
            bgStopMvvm.SwitchLanguageAction += SwitchLanguage;
            bgStopMvvm.SwitchFontsizeAction -= SwitchFontSize;
            bgStopMvvm.SwitchFontsizeAction += SwitchFontSize;
            DataContext = bgStopMvvm;
            this.SetBinding(BG_STOP.VisibilityProperty,new Binding("Visibility") { Source = bgStopMvvm ,Mode = BindingMode.TwoWay});
        }
        bool? isVisible;
        private void BG_STOP_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            isVisible = e.NewValue as bool?;
            if (isVisible != null && isVisible == false)
            {
                soundPlayer?.Stop();
                ReadyTimer.Stop();
            }
            else
            {
                soundPlayer?.PlayLooping();
                ReadyTimer.Start();
            }
        }

        Timer ReadyTimer = new Timer();

        private void ReadyTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
            {
                if(lblWarning!=null)
                lblWarning.Visibility = lblWarning.Visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
            }));
        }


        /// <summary>
        /// 切换字体
        /// </summary>
        /// <param name="Language"></param>
        private void SwitchFontSize()
        {
            BG_STOP_Loaded(null, null);
        }
        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="Language"></param>
        private void SwitchLanguage()
        {
            BG_STOP_Loaded(null, null);
        }

        private void BG_STOP_Loaded(object sender, RoutedEventArgs e)
        {
            ReadyTimer.Start();
            _Title = UpdateStatusNameAction("SoftwareEmergencyStop");
            _Content = UpdateStatusNameAction("StopHadPressed");
            MainGrid = InitGrid();
            ReflashPanel(ContentLists);
            OutBorder.Child = MainGrid;
            Content = OutBorder;
        }
        public void SetContent(Dictionary<string,PLCPositionEnum> Contents)
        {
            ContentLists = Contents;
        }
        public static string GetName()
        {
            return _Title;
        }

        private Label lblWarning;
        private Grid InitGrid()
        {
            Grid _MainGrid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Pixel) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(25, GridUnitType.Star) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30, GridUnitType.Star) });

            #region 顶部栏
            DockPanel dp = new DockPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                LastChildFill = true,
                Background = (LinearGradientBrush)this.FindResource("TitleBackGroundRed"),
                Margin = new Thickness(0)
            };
            lblWarning = new Label()
            {
                Width = 20,
                Height = 20,
                Background = (DrawingBrush)this.TryFindResource("WarningWhite")
            };
            Label _lblTitle = new Label()
            {
                Content = GetName(),
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.Big),
                FontWeight = FontWeights.Bold,
                Foreground = StrToBrush("#FFFFFF"),
                FontFamily = new FontFamily("宋体"),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            dp.Children.Add(lblWarning);
            dp.Children.Add(_lblTitle);
            //dp.Children.Add(bd);s
            Grid.SetColumn(dp, 0);
            Grid.SetRow(dp, 0);
            Grid.SetColumnSpan(dp, 3);
            _MainGrid.Children.Add(dp);
            #endregion

            #region 内容栏
            ScrollViewer sv = new ScrollViewer()
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            Contents = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            sv.Content = Contents;
            Grid.SetColumn(sv, 0);
            Grid.SetColumnSpan(sv, 2);
            Grid.SetRow(sv, 1);
            _MainGrid.Children.Add(sv);
            #endregion

            return _MainGrid;
        }
        public void ReflashPanel(Dictionary<string,PLCPositionEnum> Stops)
        {
            Contents?.Children.Clear();
            bgStopMvvm.StopContents.Clear();
            foreach (var ContentItem in Stops)
            {
                StopContentModel stopContentModel = new StopContentModel()
                {
                    IsShow = Visibility.Hidden,
                    DisplayContent = ContentItem.Key,
                    ValueContent = UpdateStatusNameAction(ContentItem.Key),
                    pLCPositionEnum = ContentItem.Value
                };
                AddStopPanel(stopContentModel);
                bgStopMvvm.StopContents.Add(stopContentModel);
            }
        }

        private void AddStopPanel(StopContentModel ContentItem)
        {
            StackPanel sp = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                //Height = 30
            };
            Label ContentLabel = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Style = (Style)this.TryFindResource("diyLabel"),
                Foreground = (LinearGradientBrush)this.FindResource("TitleBackGroundRed"),
                FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.Middle),
                //Height = 35
            };
            TextBlock txtBlock = new TextBlock()
            { TextAlignment = TextAlignment.Left, TextWrapping = TextWrapping.WrapWithOverflow };
            txtBlock.SetBinding(TextBlock.TextProperty,new Binding("ValueContent") { Source = ContentItem,Mode = BindingMode.TwoWay});
            ContentLabel.SetBinding(Label.HeightProperty, new Binding("LabelHeight") { Source = ContentItem, Mode = BindingMode.TwoWay });
            sp.SetBinding(StackPanel.HeightProperty, new Binding("LabelHeight") { Source = ContentItem, Mode = BindingMode.TwoWay });
            sp.SetBinding(StackPanel.VisibilityProperty, new Binding("IsShow") { Source = ContentItem, Mode = BindingMode.TwoWay });
            ContentLabel.Content = txtBlock;
            sp.Children.Add(ContentLabel);
            Contents?.Children.Add(sp);
        }
    }

    public class BgStopMvvm : BaseMvvm
    {
        private Visibility _visibility = Visibility.Visible;
        public Visibility Visibility { get { return _visibility; } set { _visibility = value; RaisePropertyChanged("Visibility"); } }
        /// <summary>
        /// 急停面板的内容
        /// </summary>
        public UIList<StopContentModel> StopContents = new UIList<StopContentModel>();

        protected override void InquirePlcStatus(List<bool> StatusList)
        {

        }
        private Dispatcher UIDispatcher => Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
        protected async override void ConnectionStatus(bool ConnectStatus)
        {
            await UIDispatcher.InvokeAsync(() =>
            {
                if (ConnectStatus)
                {
                    foreach (var stopContentsItem in StopContents)
                    {
                        if (stopContentsItem.pLCPositionEnum == PLCPositionEnum.Null)
                        {
                            continue;
                        }
                        stopContentsItem.IsShow = PLCControllerManager.GetInstance().GetStatusByPositionEnum(stopContentsItem.pLCPositionEnum)
                            ? Visibility.Visible : Visibility.Collapsed;
                        stopContentsItem.LabelHeight = 35;
                    }
                }
                else
                {
                    foreach (var stopContentsItem in StopContents)
                    {
                        if (stopContentsItem.pLCPositionEnum == PLCPositionEnum.Null)
                        {
                            stopContentsItem.IsShow = Visibility.Collapsed;
                            stopContentsItem.LabelHeight = 35;
                            continue;
                        }
                        stopContentsItem.IsShow = Visibility.Collapsed;
                        stopContentsItem.LabelHeight = 0;
                    }
                }
                if (StopContents.Any(q => q.IsShow == Visibility.Visible && q.pLCPositionEnum != PLCPositionEnum.Null))
                {
                    var Stop = StopContents.FirstOrDefault(q => q.pLCPositionEnum == PLCPositionEnum.Null);
                    if (Stop != null)
                    {
                        Stop.IsShow = Visibility.Visible;
                        Stop.LabelHeight = 35;
                    }
                    Common.IsReset = false;
                }
                if (Common.IsReset)
                {
                    var Stop = StopContents.FirstOrDefault(q => q.pLCPositionEnum == PLCPositionEnum.Null);
                    if (Stop != null)
                    {
                        Stop.IsShow = Visibility.Collapsed;
                    }
                }
                var stopModel = StopContents.FirstOrDefault(q => q.pLCPositionEnum == PLCPositionEnum.Null);
                if (stopModel != null)
                {
                    Visibility = stopModel.IsShow;
                }

                var stopContent = StopContents.FirstOrDefault(q => q.pLCPositionEnum == PLCPositionEnum.Null);
                //如果没有故障了，但是只剩了一个需要复位的操作，就直接点击复位
                if (!StopContents.Any(q => q.IsShow == Visibility.Visible && q.pLCPositionEnum != PLCPositionEnum.Null) &&
                    stopContent?.IsShow == Visibility.Visible
                    && !Common.IsReset)
                {
                    Task.Run(() => {
                        PLCControllerManager.GetInstance().WritePositionValue(Common.CommandDic[Command.HitchReset], true);
                        Thread.Sleep(200);
                        PLCControllerManager.GetInstance().WritePositionValue(Common.CommandDic[Command.HitchReset], false);
                        Common.IsReset = true;
                    });
                }
            }, DispatcherPriority.Background);
        }

        public override void LoadUIText()
        {
            foreach (var StopItem in StopContents)
            {
                StopItem.DisplayContent = UpdateStatusNameAction(StopItem.ValueContent);
            }
            SwitchFontsizeAction?.Invoke();
        }
    }

    public class StopContentModel:BaseNotifyPropertyChanged
    {
        private string _DisplayContent = string.Empty;
        public string DisplayContent { get { return _DisplayContent; }set { _DisplayContent = value; RaisePropertyChanged("DisplayContent");  } }

        private string _ValueContent = string.Empty;
        public string ValueContent { get { return UpdateStatusNameAction(DisplayContent); } set { _ValueContent = value; RaisePropertyChanged("ValueContent"); } }

        private Visibility _IsShow = Visibility.Collapsed;
        public Visibility IsShow { get { return _IsShow; }set { _IsShow = value; RaisePropertyChanged("IsShow"); } }

        private PLCPositionEnum _PlcPositionEnum;
        public PLCPositionEnum pLCPositionEnum { get { return _PlcPositionEnum; }set { _PlcPositionEnum = value; RaisePropertyChanged("pLCPositionEnum"); } }

        private double labelHeight = 35;
        public double LabelHeight { get { return labelHeight; }set { labelHeight = value;RaisePropertyChanged("LabelHeight"); } }
    }
}
