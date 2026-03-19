using BGModel;
using BGUserControl;
using CMW.Common.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
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
using System.Windows.Threading;

namespace BGUserControl
{
    /// <summary>
    /// StatusBar.xaml 的交互逻辑
    /// </summary>
    public partial class StatusBar : UserControl
    {
        StatusBarMvvm statusBarMvvm = new StatusBarMvvm();
        public StatusBar()
        {
            InitializeComponent();
            DataContext = statusBarMvvm;
        }

        /// <summary>
        /// 表示当前的等待状态
        /// </summary>
        [Bindable(true)]
        public IEnumerable DataSource
        {
            get
            {
                return (IEnumerable)GetValue(DataSourceProperty);
            }
            set { SetValue(DataSourceProperty, value); }
        }

        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(IEnumerable), typeof(StatusBar),
                new PropertyMetadata(new ObservableCollection<SignalModel>(), new PropertyChangedCallback(OnValueChange)));
        /// <summary>
        /// 设置文本
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StatusBar statusBar = d as StatusBar;
            if (statusBar != null && statusBar.DataSource != null)
            {
                statusBar.statusBarMvvm.DataSource = statusBar.DataSource;
                MakeStatusBar(statusBar);
            }
        }

        private static void MakeStatusBar(StatusBar statusBar)
        {
            statusBar.SignalLabelGrid.ColumnDefinitions.Clear();
            statusBar.SignalLabelGrid.Children.Clear();
            int Index = 0;
            foreach (var signalModelItem in statusBar.DataSource)
            {
                statusBar.SignalLabelGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                MakeSignal(statusBar, signalModelItem as SignalModel, Index);
                Index++;
            }
        }

        private static void MakeSignal(StatusBar statusBar, SignalModel signalModel, int Index)
        {
            Border border = new Border()
            {
                BorderBrush = (SolidColorBrush)statusBar.TryFindResource("MainPageRightLine"),
                BorderThickness = new Thickness(0.75, 0.75, 0, 0),
            };
            DiySignalLabel diySignalLabel = new DiySignalLabel()
            {
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            diySignalLabel.SetBinding(DiySignalLabel.SignalModelProperty, new Binding(".") { Source = signalModel, Mode = BindingMode.TwoWay });
            diySignalLabel.SetBinding(DiySignalLabel.SignalModelTypeProperty, new Binding("SignalType") { Source = signalModel, Mode = BindingMode.TwoWay });
            diySignalLabel.SetBinding(DiySignalLabel.SignalModelValueProperty, new Binding("SignalValue") { Source = signalModel, Mode = BindingMode.TwoWay });
            diySignalLabel.SetBinding(DiySignalLabel.SignalModelNameProperty, new Binding("SignalName") { Source = signalModel, Mode = BindingMode.TwoWay});
            diySignalLabel.SetBinding(DiySignalLabel.SignalModelFontsizeProperty, new Binding("fontSizeModel.normalMiddle") { Mode = BindingMode.TwoWay} );
            
            border.Child = diySignalLabel;
            Grid.SetColumn(border, Index);
            Grid.SetRow(border, 0);
            statusBar.SignalLabelGrid.Children.Add(border);
        }
    }

    public class StatusBarMvvm:BaseMvvm
    {
        private IEnumerable _DataSource = new ObservableCollection<SignalModel>();
        public IEnumerable DataSource
        {
            get { return _DataSource; }
            set { _DataSource = value; RaisePropertyChanged("DataSource"); }
        }

        public StatusBarMvvm()
        {
            LoadUIFontSize();
        }

        private Dispatcher UIDispatcher => Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
        protected async override void ConnectionStatus(bool ConnectionStatus)
        {
            await UIDispatcher.InvokeAsync(() =>
            {
          
                foreach (var SignalModelItem in DataSource)
                {
                    if ((SignalModelItem as SignalModel).SignalType == (int)SignalModelTypeEnum.Label)
                    {
                        (SignalModelItem as SignalModel).SignalValue = string.Empty;
                        (SignalModelItem as SignalModel).SignalValue = CommonDeleget.UpdateStatusNameAction((SignalModelItem as SignalModel).SearchAction());
                    }
                    else
                    {
                        (SignalModelItem as SignalModel).SignalValue = (SignalModelItem as SignalModel).SearchAction();
                    }
                }
            });
        }
    }
}
