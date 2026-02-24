using BG_Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;

namespace BGUserControl
{
    public class DiyTextBlock:Label
    {
        public string DiyText
        {
            get { return (string)GetValue(DiyTextBlockProperty); }
            set { SetValue(DiyTextBlockProperty, value); }
        }
        public static readonly DependencyProperty DiyTextBlockProperty =
            DependencyProperty.Register("DiyText", typeof(string), typeof(DiyTextBlock), new PropertyMetadata(default(string), OnDiyTextPropertyChanged));

        private static void OnDiyTextPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DiyTextBlock statusModel = obj as DiyTextBlock;
            if (statusModel != null && statusModel.DiyText != null)
            {
                string s = statusModel.DiyText;
                int a = s.Length;
                statusModel.Content = "";
                StackPanel sp =new StackPanel()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Orientation = Orientation.Vertical
                };
                for (int i = 0; i < a; i++)
                {
                    TextBlock tb = new TextBlock()
                    {
                        Padding = new Thickness(0),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = UpdateFontSizeAction(CMWFontSize.Small)
                    };
                    tb.Text = s.Substring(i, 1);
                    if (IsHanZi(tb.Text))
                    {
                        tb.Margin = new Thickness(-3, 0, 0, 0);
                        sp.Children.Add(tb);
                        statusModel.Content = sp;
                        continue;
                    }
                    if (i != 0)
                    {
                        tb.Margin = new Thickness(0, -7, 0, 0);
                    }
                    else
                    {
                        tb.Margin = new Thickness(0, -1, 0, 0);
                    }
                    sp.Children.Add(tb);
                    statusModel.Content = sp;
                }
            }
        }
        private static bool IsHanZi(string ch)
        {
            byte[] byte_len = System.Text.Encoding.Default.GetBytes(ch);
            if (byte_len.Length == 2) { return true; }
            return false;
        }

    }
}
