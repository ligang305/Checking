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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.CommonFunc;
using BG_Entities;

namespace BGUserControl
{
    /// <summary>
    /// DiyRadioButton.xaml 的交互逻辑
    /// </summary>
    public partial class DiyRadioButton : BaseWindows
    {

        public MouseButtonEventHandler MouseButonEnent;

        /// <summary>
        /// 属性，用来接收外部传输进来的数据对象
        /// </summary>
        public CommonSettingModel ItemSource
        {
            get
            {
                return (CommonSettingModel)GetValue(ItemSourceProperty);
            }
            set { SetValue(ItemSourceProperty, value); }
        }

        /// <summary>
        /// 数据源依赖属性
        /// </summary>
        public static readonly DependencyProperty ItemSourceProperty =
            DependencyProperty.Register("ItemSource", typeof(CommonSettingModel), typeof(DiyRadioButton),
                 new PropertyMetadata(new CommonSettingModel(), new PropertyChangedCallback(OnValueChange)));

        public DiyRadioButton()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 接收到值 回调的方法
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiyRadioButton Radiobutton = d as DiyRadioButton;
            if (Radiobutton != null && Radiobutton.ItemSource != null)
            {
                Radiobutton.lblMainContent.Content = Radiobutton.MakeButton(Radiobutton.ItemSource);
            }
        }

        /// <summary>
        /// 制作面板
        /// </summary>
        /// <param name="_csm"></param>
        /// <returns></returns>
        private Grid MakeButton(CommonSettingModel _csm)
        {
            Grid grid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(20,0,0,0)
            };
            try
            {
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                foreach (var itemSelectObject in _csm.selectObject)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                    int value = Convert.ToInt32(itemSelectObject.SelectValue.Split('?')[1]);

                    Border _BtnSendCommand = new Border()
                    {
                        Width = 90,
                        Height = 30,
                        Style = (Style)this.FindResource("diyBtnCarHand"),
                        Margin = new Thickness(5, 0, 0, 0)
                    };
                    _BtnSendCommand.MouseDown -= _BtnSendCommand_PreviewMouseUp;
                    _BtnSendCommand.MouseDown += _BtnSendCommand_PreviewMouseUp;
                    _BtnSendCommand.BorderBrush = StrToBrush("#497274");

                    Label _btnInner = new Label()
                    {
                        Content = UpdateStatusNameAction(itemSelectObject.SelectText),
                        Style = (Style)this.TryFindResource("diyLabel"),
                        Foreground = StrToBrush("#FFFFFF")
                    };
                    _btnInner.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
                    _btnInner.Foreground = value == 0 ? StrToBrush("#20B3F8") : StrToBrush("#FFFFFF");
                    _BtnSendCommand.Tag = itemSelectObject.SelectValue;
                    _BtnSendCommand.Child = _btnInner;
                    _btnInner.Tag = _csm;
                    Grid.SetRow(_BtnSendCommand, 0);
                    Grid.SetColumn(_BtnSendCommand, _csm.selectObject.IndexOf(itemSelectObject));
                    grid.Children.Add(_BtnSendCommand);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return grid;
        }


        private void _BtnSendCommand_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MouseButonEnent?.Invoke(sender,e);
        }
    }
}
