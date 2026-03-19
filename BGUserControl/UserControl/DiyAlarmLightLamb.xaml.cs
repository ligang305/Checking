using CMW.Common.Utilities;
using BGModel;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static CMW.Common.Utilities.Common;

namespace BGUserControl
{
    /// <summary>
    /// DiyAlarmLightLamb.xaml 的交互逻辑
    /// </summary>
    public partial class DiyAlarmLightLamb : BaseWindows
    {
        bool? isVisible;
        public DiyAlarmLightLamb()
        {
            InitializeComponent();
            IsVisibleChanged += DiyAlarmLightLamb_IsVisibleChanged;
        }

        private SignalModel SignalModelProp { get { return (SignalModel)GetValue(SignalModelProperty); } set { SetValue(SignalModelProperty, value); SetContent(value); } }

        public static readonly DependencyProperty SignalModelProperty =
            DependencyProperty.Register("SignalModelProp", typeof(SignalModel), typeof(DiyAlarmLightLamb),
                new PropertyMetadata(null, (s, e) => { ((DiyAlarmLightLamb)s).SignalModelProp = (SignalModel)e.NewValue;}));

        private void DiyAlarmLightLamb_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            isVisible = e.NewValue as bool?;
            if (isVisible != null && isVisible == true)
            {
                SearchSignal();
            }
        }

        private void SearchSignal()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (isVisible == false) return;
                    try
                    {
                        SignalModelProp.SignalColor = GlobalRetStatus[SignalModelProp.SignalIndex].ToString().ToUpper() == SignalModelProp.SignalValue.ToUpper()?
                        (DrawingBrush)this.TryFindResource("lightLampGreen"): (DrawingBrush)this.TryFindResource("lightLampRed");
                    }
                    catch (System.Exception ex)
                    {
                        CommonDeleget.HandTaskException(ex);
                    }
                }
            });
        }
        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="sm"></param>
        public void SetContent(SignalModel sm)
        {
            SignalModelProp = sm;
            MainGrid.DataContext = SignalModelProp;
        }
    }
}
