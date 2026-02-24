using BGModel;
using CMW.Common.Utilities;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
namespace BGUserControl
{
    /// <summary>
    /// DiyAlarmLightLamb.xaml 的交互逻辑
    /// </summary>
    public partial class DiySignalLabel : UserControl
    {
        static bool? isVisible;
        public DiySignalLabel():base()
        {
            InitializeComponent();
            if (SignalModelTypeProp == (int)SignalModelTypeEnum.Label)
            {
                lblSignalValue.Visibility = Visibility.Visible;
            }
            else if (SignalModelTypeProp == (int)SignalModelTypeEnum.Icon)
            {
                XrayIcon.Visibility = Visibility.Visible;
            }
            else if (SignalModelTypeProp == (int)SignalModelTypeEnum.StartingDynamoIcon)
            {
               StartingDynamoIcon.Visibility = Visibility.Visible;
            }
        }
        
        public static readonly DependencyProperty SignalModelProperty =
        DependencyProperty.Register("SignalModelProp", typeof(SignalModel), typeof(DiySignalLabel),
             new PropertyMetadata(new SignalModel(), new PropertyChangedCallback(OnValueChange)));

        public static readonly DependencyProperty SignalModelNameProperty =
      DependencyProperty.Register("SignalModelNameProp", typeof(string), typeof(DiySignalLabel),
           new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnNameValueChange)));

        public static readonly DependencyProperty SignalModelValueProperty =
     DependencyProperty.Register("SignalModelValueProp", typeof(string), typeof(DiySignalLabel),
          new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnLabelValueChange)));


        public static readonly DependencyProperty SignalModelFontsizeProperty =
DependencyProperty.Register("SignalModelFontsizeProp", typeof(double), typeof(DiySignalLabel),
     new PropertyMetadata(12.0, new PropertyChangedCallback(OnLabelFontsizeValueChange)));


        public static readonly DependencyProperty SignalModelTypeProperty =
                DependencyProperty.Register("SignalModelTypeProp", typeof(int), typeof(DiySignalLabel),
                     new PropertyMetadata(0, new PropertyChangedCallback(OnLabelTypeValueChange)));


        /// <summary>
        /// 属性，用来接收外部传输进来的数据对象
        /// </summary>
        public SignalModel SignalModelProp
        {
            get
            {
                return (SignalModel)GetValue(SignalModelProperty);
            }
            set { SetValue(SignalModelProperty, value);  }
        }

        /// <summary>
        /// 属性，用来接收外部传输进来的数据对象
        /// </summary>
        public double SignalModelFontsizeProp
        {
            get
            {
                return (double)GetValue(SignalModelFontsizeProperty);
            }
            set { SetValue(SignalModelFontsizeProperty, value); }

        }

        /// <summary>
        /// 属性，用来接收外部传输进来的数据对象
        /// </summary>
        public string SignalModelNameProp
        {
            get
            {
                return (string)GetValue(SignalModelNameProperty);
            }
            set { SetValue(SignalModelNameProperty, value); }

        }

        /// <summary>
        /// 属性，用来接收外部传输进来的数据对象
        /// </summary>
        public string SignalModelValueProp
        {
            get
            {
                return (string)GetValue(SignalModelValueProperty);
            }
            set { SetValue(SignalModelValueProperty, value); }

        }

        /// <summary>
        /// 属性，用来接收外部传输进来的数据对象
        /// </summary>
        public int SignalModelTypeProp
        {
            get
            {
                return (int)GetValue(SignalModelTypeProperty);
            }
            set { SetValue(SignalModelTypeProperty, value); }

        }
        /// <summary>
        /// 异步线程 查询传递进来的事件
        /// </summary>

        public static void OnValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiySignalLabel signalModel = d as DiySignalLabel;
            if (signalModel != null )
            {
                signalModel.lblSignalName.Content = CommonDeleget.UpdateStatusNameAction(signalModel.SignalModelNameProp);
                if (signalModel.SignalModelTypeProp == (int)SignalModelTypeEnum.Label)
                {
                    signalModel.lblSignalValue.Content = signalModel.SignalModelValueProp;
                }
                else if (signalModel.SignalModelTypeProp == (int)SignalModelTypeEnum.Icon)
                {
                    signalModel.XrayIcon.IsShowFlash = Convert.ToBoolean(signalModel.SignalModelValueProp);
                }
                else
                {
                    signalModel.StartingDynamoIcon.IsShowFlash = Convert.ToBoolean(signalModel.SignalModelValueProp);
                }
                signalModel.lblSignalName.Content = CommonDeleget.UpdateStatusNameAction(signalModel.SignalModelNameProp);
            }
        }
        public static void OnNameValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiySignalLabel signalModel = d as DiySignalLabel;
            if (signalModel != null && signalModel.SignalModelProp != null)
            {
                signalModel.lblSignalName.Content = CommonDeleget.UpdateStatusNameAction(signalModel.SignalModelNameProp);
            }
        }
        public static void OnLabelValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiySignalLabel signalModel = d as DiySignalLabel;
            if (signalModel != null && signalModel.SignalModelProp != null)
            {
                if (signalModel.SignalModelTypeProp == (int)SignalModelTypeEnum.Label)
                {
                    signalModel.lblSignalValue.Content = signalModel.SignalModelValueProp;
                }
                else if (signalModel.SignalModelTypeProp == (int)SignalModelTypeEnum.Icon)
                {
                    signalModel.XrayIcon.IsShowFlash = Convert.ToBoolean(signalModel.SignalModelValueProp);
                }
                else
                {
                    signalModel.StartingDynamoIcon.IsShowFlash = Convert.ToBoolean(signalModel.SignalModelValueProp);
                }
                signalModel.lblSignalName.Content = CommonDeleget.UpdateStatusNameAction(signalModel.SignalModelNameProp);
            }
        }
        public static void OnLabelFontsizeValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiySignalLabel signalModel = d as DiySignalLabel;
            if (signalModel != null && signalModel.SignalModelFontsizeProp != null)
            {

                signalModel.lblSignalValue.FontSize = signalModel.SignalModelFontsizeProp;
                signalModel.lblSignalName.FontSize = signalModel.SignalModelFontsizeProp;
            }
        }

        public static void OnLabelTypeValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiySignalLabel signalModel = d as DiySignalLabel;
            if (signalModel != null && signalModel.SignalModelTypeProp!= null)
            {
                if (signalModel.SignalModelTypeProp == (int)SignalModelTypeEnum.Label)
                {
                    signalModel.lblSignalValue.Visibility = Visibility.Visible;
                }
                else if (signalModel.SignalModelTypeProp == (int)SignalModelTypeEnum.Icon)
                {
                    signalModel.XrayIcon.Visibility = Visibility.Visible;
                }
                else
                {
                    signalModel.StartingDynamoIcon.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
