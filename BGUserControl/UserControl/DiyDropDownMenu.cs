
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.ComponentModel.Composition;
using BGModel;
using static CMW.Common.Utilities.CommonDeleget;
using CMW.Common.Utilities;
using BG_Entities;

namespace BGUserControl
{
    public class DiyDropDownMenu:BaseWindows
    {
        Border _MainBorder = new Border() {Width = 150,Height = 0 };
        Grid _MainGd = new Grid();
        ObservableCollection<object> StatusModellList = new ObservableCollection<object>();
        public DiyDropDownMenu(ObservableCollection<object> _HitchModellList):base()
        {
            StatusModellList = _HitchModellList;
            Loaded += DiyDropDownMenu_Loaded;
            _MainGd = MakeGrid(StatusModellList);
            _MainBorder.Child = _MainGd;
            Content = _MainBorder;
        }


        private void DiyDropDownMenu_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
           
        }

        public Grid MakeGrid(ObservableCollection<object> HitchModellList)
        {
            Grid _grid = InitContentGrid();
            int index = 0;
            foreach (var HitchItem in HitchModellList)
            {

                Style temp = (Style)this.FindResource("PopupLabel");
                Label lblHitch = InitHitchLabel(HitchItem, temp);
                InitBd(_grid, index, lblHitch);
                index++;
            }
            return _grid;
        }

        private void InitBd(Grid _grid, int index, Label lblHitch)
        {
            Border bd = new Border()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                BorderBrush = (SolidColorBrush)this.FindResource("PopupBorderBrush"),
                BorderThickness = new Thickness(1, 0, 1, 1)
            };
            _grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            bd.Child = lblHitch;
            Grid.SetColumn(bd, 0);
            Grid.SetRow(bd, index);
            _grid.Children.Add(bd);
        }

        private Label InitHitchLabel(object HitchItem, Style temp)
        {
            Label lblHitch = new Label()
            {
                Style = temp,
                Foreground = (SolidColorBrush)this.FindResource("PopupLabelForColor")
            };
          
            //lblHitch.tri   = TextTrimming.WordEllipsis;
            _MainBorder.Height += 35;
            StatusModel sm = HitchItem as StatusModel;
            sm.StatusName = UpdateStatusNameAction(sm.StatusName);
            TextBlock tb = new TextBlock()
            {
                Style = (Style)this.FindResource("PopupTextBlock"),
                HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch,
                TextTrimming = TextTrimming.CharacterEllipsis,
            };
            tb.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            lblHitch.Content = tb;
            tb.SetBinding(Label.ForegroundProperty, new Binding("StatusCode")
            { Source = HitchItem, Mode = BindingMode.TwoWay, Converter = new HitchColorConvert() });
            tb.SetBinding(TextBlock.ToolTipProperty, new Binding("StatusName")
            { Source = HitchItem, Mode = BindingMode.TwoWay });
            tb.SetBinding(TextBlock.TextProperty, new Binding("StatusName")
            { Source = HitchItem, Mode = BindingMode.TwoWay });
            lblHitch.SetBinding(Label.ForegroundProperty, new Binding("StatusCode")
            { Source = HitchItem, Mode = BindingMode.TwoWay, Converter = new HitchColorConvert() });
            return lblHitch;
        }

        private Grid InitContentGrid()
        {
            Grid _grid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = (SolidColorBrush)this.FindResource("PopupBackGrand"),

            };
            _grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            return _grid;
        }
    }
}
