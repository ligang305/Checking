using CMW.Common.Utilities;
using BGModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonFunc;
using static CMW.Common.Utilities.CommonDeleget;
using BG_Services;
using BG_Entities;

namespace BGUserControl
{
    /// <summary>
    /// 自定义向前向后的控件
    /// </summary>
    public class DiyWindowShades : BaseWindows
    {
        public DiyWindowShades()
        {
            Loaded += DiyWindowShades_Loaded;
            ButtonInvoke.SwitchLanguageEvent -= SwitchLanguage;
            ButtonInvoke.SwitchLanguageEvent += SwitchLanguage;
            ButtonInvoke.SwitchFontSizeEvent -= SwitchFontSize;
            ButtonInvoke.SwitchFontSizeEvent += SwitchFontSize;

        }

        private void DiyWindowShades_Loaded(object sender, RoutedEventArgs e)
        {
            Content = MakeScan();
            HorizontalAlignment = HorizontalAlignment.Stretch;
        }


        /// <summary>
        /// 切换字体
        /// </summary>
        /// <param name="Language"></param>
        private void SwitchFontSize(string FontSize)
        {
            DiyWindowShades_Loaded(null, null);
        }
        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="Language"></param>
        private void SwitchLanguage(string Language)
        {
            Base_SwitchLanguage(Language);
            DiyWindowShades_Loaded(null, null);
        }



        public StackPanel MakeScan()
        {
            StackPanel ScanSp = new StackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Orientation = Orientation.Horizontal,
            };
            Label lblPreViewTemp = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85"),
                FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.Big),
                Margin = new Thickness(0, 0, 0, 0),
            };
            lblPreViewTemp.Content = UpdateStatusNameAction("WindowShapes");
            ScanSp.Children.Add(lblPreViewTemp);
            ToggleSwitchButton tsb = new ToggleSwitchButton()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(0, 3, 0, 3),
                Height = 36,
                Width = 60,
                Name = "Light"
            };
            tsb.IsChecked = false;
            tsb.Checked += TsBtn_Checked;
            tsb.UnChecked += TsBtn_UnChecked;
            ScanSp.Children.Add(tsb);
            return ScanSp;
        }

        /// <summary>
        /// ToggleButton取消选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TsBtn_UnChecked(object sender, RoutedEventArgs e)
        {
            ToggleSwitchButton tsb = (sender as ToggleSwitchButton);
            string btnName = tsb.Name as string;
            switch (btnName)
            {
                case "Light":
                    Task.Run(() =>
                    {
                        Common.SetCommand(CommandDic[Command.ShutterOpenOrClose], false);
                    });
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// ToggleButton按钮效果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TsBtn_Checked(object sender, RoutedEventArgs e)
        {
            ToggleSwitchButton tsb = (sender as ToggleSwitchButton);
            string btnName = tsb.Name as string;
            switch (btnName)
            {
                case "Light":
                    Task.Run(() =>
                    {
                        Common.SetCommand(CommandDic[Command.ShutterOpenOrClose], true);
                    });
                    break;
                default:
                    break;
            }
        }
    }
}
