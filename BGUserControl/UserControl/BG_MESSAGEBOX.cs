using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shell;
using static CMW.Common.Utilities.CommonFunc;
using static CMW.Common.Utilities.CommonDeleget;
using BG_Entities;

namespace BGUserControl
{
    public class BG_MESSAGEBOX: BaseShowWindow
    {
        private Grid MainGrid;
        private Border OutBorder =new Border()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Margin = new Thickness(0),
            BorderThickness = new Thickness(2),
            BorderBrush = StrToBrush("#3F96E6"),
            CornerRadius = new CornerRadius(6),
            Background = StrToBrush("#F2F2F2")
        };
        private static string _Title;
        public string _Content;
        public static double Height = 200;
        public static double Width = 300;

        public BG_MESSAGEBOX(string _content,string _title)
        {
            Loaded += BG_MESSAGEBOX_Loaded;
            _Title = _title;
            _Content = _content;
        }

        private void BG_MESSAGEBOX_Loaded(object sender, RoutedEventArgs e)
        {

            MainGrid = InitGrid();
            OutBorder.Child = MainGrid;
            Content = OutBorder;
           
           
        }

        public static void ShowLogin(Window _OwnerWin, string Title, string Content)
        {
            Application.Current?.Dispatcher.Invoke(new Action(() => {
                BG_MESSAGEBOX nb = new BG_MESSAGEBOX(Content, Title);
                nb.Title = GetName();
                nb.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                nb.WindowStyle = WindowStyle.None;
                nb.MaxWidth = GetWidth();
                nb.MaxHeight = GetHeight();
                nb.AllowsTransparency = true;
                nb.ShowInTaskbar = false;
                nb.Owner = _OwnerWin;
                nb.ShowDialog();
            }));
        }
        public static void Show(Window _OwnerWin,string Title,string Content)
        {
            try
            {
                Application.Current?.Dispatcher.Invoke(new Action(() => {
                    BG_MESSAGEBOX nb = new BG_MESSAGEBOX(Content, Title);
                    nb.Title = GetName();
                    nb.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    nb.WindowStyle = WindowStyle.None;
                    nb.MaxWidth = GetWidth();
                    nb.MaxHeight = GetHeight();
                    nb.AllowsTransparency = true;
                    nb.ShowInTaskbar = false;
                    nb.Owner = Application.Current?.MainWindow;
                    nb.ShowDialog();
                }));
            }
            catch
            {
                //throw ex;
            }
           
        }

        public static void Show(Window _OwnerWin, string Title, string Content,double Height,double Width)
        {
            try
            {
                Application.Current?.Dispatcher.Invoke(new Action(() => {
                    BG_MESSAGEBOX nb = new BG_MESSAGEBOX(Content, Title);
                    nb.Title = GetName();
                    nb.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    nb.WindowStyle = WindowStyle.None;
                    nb.MaxWidth = Width;
                    nb.MaxHeight = Height;
                    nb.AllowsTransparency = true;
                    nb.ShowInTaskbar = false;
                    nb.Owner = Application.Current?.MainWindow;
                    nb.ShowDialog();
                }));
            }
            catch
            {
                //throw ex;
            }

        }

        public static void Show(string Title, string Content)
        {
            Application.Current?.Dispatcher.Invoke(new Action(() => {
                BG_MESSAGEBOX nb = new BG_MESSAGEBOX(Content, Title);
                nb.Title = GetName();
                nb.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                nb.WindowStyle = WindowStyle.None;
                nb.MaxWidth = GetWidth();
                nb.MaxHeight = GetHeight();
                nb.AllowsTransparency = true;
                nb.ShowInTaskbar = false;
                nb.Owner = Application.Current?.MainWindow;
                nb.ShowDialog();
            }));
        }

        public static string GetName()
        {
            return _Title;
        }

        public static double GetWidth()
        {
            return Width;
        }
        public static double GetHeight()
        {
            return Height;
        }


        private Grid InitGrid()
        {
            Grid _MainGrid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Pixel) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Pixel) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(25, GridUnitType.Star) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30, GridUnitType.Star) });

            #region 顶部栏
            DockPanel dp = new DockPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                LastChildFill = true,
                Background = (LinearGradientBrush)this.FindResource("TitleBackGround"),
                Margin = new Thickness(0)
            };
            Border bd = new Border()
            {
                BorderThickness = new Thickness(0, 0, 0, 0),
                BorderBrush = (LinearGradientBrush)this.FindResource("LnearBorderColor"),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Stretch,
                Width = 32,
                Background = StrToBrush("#00FFFFFF")
            };
            bd.PreviewMouseLeftButtonDown += Bd_PreviewMouseLeftButtonDown;
            Canvas cs = new Canvas()
            {
                Style = (Style)this.FindResource("diyCloseCanvas"),
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 1, 10, 0)
            };
            bd.Child = cs;
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
            dp.MouseDown += Dp_MouseDown;
            dp.Children.Add(_lblTitle);
            dp.Children.Add(bd);
            Grid.SetColumn(dp, 0);
            Grid.SetRow(dp, 0);
            Grid.SetColumnSpan(dp, 3);
            _MainGrid.Children.Add(dp);
            #endregion

            #region 内容栏
            ScrollViewer sv = new ScrollViewer()
            {
               VerticalScrollBarVisibility =  ScrollBarVisibility.Auto
            }; 
            Label lblContent = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Content = new TextBlock() { Text = _Content,TextAlignment = TextAlignment.Left,TextWrapping = TextWrapping.WrapWithOverflow},
                Style = (Style)this.TryFindResource("diyLabel")
            };
            lblContent.FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.Normal);
            sv.Content = lblContent;
            Grid.SetColumn(sv, 0);
            Grid.SetColumnSpan(sv, 2);
            Grid.SetRow(sv, 1);
            _MainGrid.Children.Add(sv);
            #endregion


            #region 提示按钮

            StackPanel sp = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            
            Border _BtnSendCommand = new Border()
            {
                Width = 50,
                Height = 30,Margin = new Thickness(20,0,0,0),
                Style = (Style)this.FindResource("diyBtnCarHand"),
            };
            _BtnSendCommand.PreviewMouseDown += _BtnSecondSendCommand_PreviewMouseDown;
            Label _btnInner = new Label()
            {
                Content = UpdateStatusNameAction("Sure"),
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#FFFFFF")
            };
            _btnInner.FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.Normal);
            _BtnSendCommand.Child = _btnInner;

            sp.Children.Add(_BtnSendCommand);
            Grid.SetColumn(sp, 0);
            Grid.SetColumnSpan(sp, 2);
            Grid.SetRow(sp, 2);
            _MainGrid.Children.Add(sp);
            #endregion
            return _MainGrid;
        }

        private void _BtnSecondSendCommand_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dp_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bd_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
