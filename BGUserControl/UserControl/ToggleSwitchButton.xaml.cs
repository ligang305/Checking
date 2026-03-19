using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BGUserControl
{
    /// <summary>
    /// ToggleSwitchButton.xaml 的交互逻辑
    /// </summary>
    public partial class ToggleSwitchButton : BaseWindows
    {
        public static readonly DependencyProperty IsCheckedProperty =
             DependencyProperty.Register("IsChecked", typeof(bool), typeof(ToggleSwitchButton), new PropertyMetadata(default(bool), OnIsCheckedChanged));


        public event RoutedEventHandler Checked;
        public event RoutedEventHandler UnChecked;
        Storyboard sto = new Storyboard();
        ColorAnimation da = new ColorAnimation();
        Storyboard ButtonSto = new Storyboard();
        ThicknessAnimation ta = new ThicknessAnimation();

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public bool IsShowStoryBoard = true;

        public ToggleSwitchButton()
        {
            InitializeComponent();
        }

        private static void OnIsCheckedChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            (obj as ToggleSwitchButton).OnIsCheckedChanged(args);
        }

        private void OnIsCheckedChanged(DependencyPropertyChangedEventArgs args)
        {
            if (IsChecked && Checked != null)
            {
                Checked(this, new RoutedEventArgs());
            }

            if (!IsChecked && UnChecked != null)
            {
                UnChecked(this, new RoutedEventArgs());
            }

            if (IsShowStoryBoard)
            {
                StartStoryBoard();
            }
        }
        /// <summary>
        /// 让按钮有动画感
        /// </summary>
        private void StartStoryBoard()
        {
            if (IsChecked)
            {
                ButtonSto.Stop();
                ButtonSto.Children.Clear();
                ta.From = new Thickness(0, 0, 0, 0);
                ta.To = new Thickness(22, 0, 0, 0);
                ta.Duration = new Duration(TimeSpan.FromMilliseconds(150));
                Storyboard.SetTarget(ta, slideBorder);
                Storyboard.SetTargetProperty(ta, new PropertyPath("Margin"));
                ButtonSto.Children.Add(ta);
                ButtonSto.Begin();

                sto.Stop();
                sto.Children.Clear();
                da.From = Color.FromRgb(128,128,128);
                da.To = Color.FromRgb(0, 90, 159);
                da.Duration = new Duration(TimeSpan.FromMilliseconds(150));
                Storyboard.SetTarget(da, fillRectangle);
                Storyboard.SetTargetProperty(da, new PropertyPath("(Border.Background).(SolidColorBrush.Color)"));
                sto.Children.Add(da);
                sto.Begin();
            }
            else
            {
                ButtonSto.Stop();
                ButtonSto.Children.Clear();
                ta.From = new Thickness(22, 0, 0, 0);
                ta.To = new Thickness(0, 0, 0, 0);
                ta.Duration = new Duration(TimeSpan.FromMilliseconds(150));
                Storyboard.SetTarget(ta, slideBorder);
                Storyboard.SetTargetProperty(ta, new PropertyPath("Margin"));
                ButtonSto.Children.Add(ta);
                ButtonSto.Begin();

                sto.Stop();
                sto.Children.Clear();
                da.From = Color.FromRgb(0, 90, 159);
                da.To = Color.FromRgb(128, 128, 128);
                da.Duration = new Duration(TimeSpan.FromMilliseconds(150));
                Storyboard.SetTarget(da, fillRectangle);
                Storyboard.SetTargetProperty(da, new PropertyPath("(Border.Background).(SolidColorBrush.Color)"));
                sto.Children.Add(da);
                sto.Begin();
            }
        }


        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs args)
        {
                IsChecked ^= true;
                args.Handled = true;
                base.OnMouseLeftButtonDown(args);
        }
    }
}
