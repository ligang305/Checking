using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace BGUserControl
{
    public class DiyMainPageButton:Button
    {

    }

    public class MainPageModulesButton : DiyMainPageButton
    {
        Image image;
        public MainPageModulesButton()
        {
            InitUI();
            //PreviewMouseLeftButtonDown += MainPageModulesButton_PreviewMouseLeftButtonDown;
        }

        //private void MainPageModulesButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    if(string.IsNullOrEmpty(ModulesName))
        //    {
        //        //TODO 
        //        //弹框显示模块缺失
        //    }
        //    //TODO 弹框显示模块
        //}

        private void InitUI()
        {
            StackPanel mainStackPanel = new StackPanel() { Orientation = System.Windows.Controls.Orientation.Horizontal };
            Content = mainStackPanel;
            image = new Image() { Width = 21, Height = 21, Stretch = System.Windows.Media.Stretch.UniformToFill };
            mainStackPanel.Children.Add(image);
        }
        /// <summary>
        /// 表示当前的等待状态
        /// </summary>
        public string ImageSource
        {
            get
            {
                return (string)GetValue(ImageSourceProperty);
            }
            set { SetValue(ImageSourceProperty, value); }
        }

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(string), typeof(MainPageModulesButton),
                new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnValueChange)));
        /// <summary>
        /// 设置文本
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MainPageModulesButton mainPageModulesButton = d as MainPageModulesButton;
            if (mainPageModulesButton != null && mainPageModulesButton.ImageSource != null)
            {
                mainPageModulesButton.image.Source = new BitmapImage(new Uri(mainPageModulesButton.ImageSource));
            }
        }
        /// <summary>
        /// 表示当前的等待状态
        /// </summary>
        public string ModulesName
        {
            get
            {
                return (string)GetValue(ModulesNameProperty);
            }
            set { SetValue(ModulesNameProperty, value); }
        }

        public static readonly DependencyProperty ModulesNameProperty =
            DependencyProperty.Register("ModulesName", typeof(string), typeof(MainPageModulesButton),
                new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnModulesNameValueChange)));
        /// <summary>
        /// 设置绑定的模块名
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnModulesNameValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }
    }
}
