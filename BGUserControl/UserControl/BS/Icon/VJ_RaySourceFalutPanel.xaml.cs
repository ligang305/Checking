using BG_Entities;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// VJ_RaySourceFalutPanel.xaml 的交互逻辑
    /// </summary>
    public partial class VJ_RaySourceFalutPanel : UserControl
    {
        /// <summary>
        /// VJ_故障码
        /// </summary>
        public Int32 VJ_FalutCode
        {
            get
            {
                return (Int32)GetValue(VJ_FalutCodeProperty);
            }
            set { SetValue(VJ_FalutCodeProperty, value); }
        }

        /// <summary>
        /// 数据源依赖属性
        /// </summary>
        public static readonly DependencyProperty VJ_FalutCodeProperty =
            DependencyProperty.Register("VJ_FalutCode", typeof(Int32), typeof(VJ_RaySourceFalutPanel),
                 new PropertyMetadata(0, new PropertyChangedCallback(OnValueChange)));


        public VJ_RaySourceFalutPanel()
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
            VJ_RaySourceFalutPanel VJ_RaySourceFalutPanel = d as VJ_RaySourceFalutPanel;
            if (VJ_RaySourceFalutPanel != null && VJ_RaySourceFalutPanel.VJ_FalutCode != null)
            {
                VJ_RaySourceFalutPanel.UIntToBit(VJ_RaySourceFalutPanel.VJ_FalutCode, VJ_RaySourceFalutPanel.FalutCodeGrid);
            }
        }

        public List<string> UIntToBit(Int32 Float, UniformGrid grid)
        {
            grid.Children.Clear();
            List<string> FalutStr = new List<string>();
            Byte[] FalutBytes = BitConverter.GetBytes(Float);
            for (int i = 0; i < FalutBytes.Length; i++)
            {
                for (short j = 0; j < 8; j++)
                {
                    int bitValur =  BitHelper.GetBit(FalutBytes[i],j);
                    if(bitValur > 0)
                    {
                        int TotalIndex = i * 8 + j;
                        FalutStr.Add(GetFalutByIndex(TotalIndex));
                    }
                }
            }
            for (int i = 0; i < FalutStr.Count; i++)
            {
                Label lblFalut = new Label() 
                { 
                    Style = (Style)this.FindResource("diyLabel"),
                    FontSize = UpdateFontSizeAction(CMWFontSize.Small),
                    Content = FalutStr[i]
                };
                grid.Children.Add(lblFalut);
            }
            return FalutStr;
        }

        public static string GetFalutByIndex(int Position)
        {
            string Falut = string.Empty;
            switch (Position)
            {
                case 0:
                    Falut  = UpdateStatusNameAction("Pre-warn INDICATION");
                    break;
                case 1:
                    Falut = UpdateStatusNameAction("Prep-expired fault");
                    break;
                case 2:
                    Falut = UpdateStatusNameAction("Duty cycle mode INDICATION");
                    break;
                case 3:
                    Falut = UpdateStatusNameAction("Overvoltage (Total kV) fault");
                    break;
                case 4:
                    Falut = UpdateStatusNameAction("Overvoltage (Cathode kV) fault");
                    break;
                case 5:
                    Falut = UpdateStatusNameAction("Overvoltage (Anode kV) fault");
                    break;
                case 6:
                    Falut = UpdateStatusNameAction("Overvoltage (Total OVP) fault");
                    break;
                case 7:
                    Falut = UpdateStatusNameAction("Overcurrent (Total mA) fault");
                    break;
                case 8:
                    Falut = UpdateStatusNameAction("Overpower fault");
                    break;
                case 9:
                    Falut = UpdateStatusNameAction("Regulation fault");
                    break;
                case 10:
                    Falut = UpdateStatusNameAction("Watchdog1 expired fault");
                    break;
                case 11:
                    Falut = UpdateStatusNameAction("Watchdog3 expired fault");
                    break;
                case 12:
                    Falut = UpdateStatusNameAction("FRAM read failure");
                    break;
                case 13:
                    Falut = UpdateStatusNameAction("FRAM write failure");
                    break;
                case 14:
                    Falut = UpdateStatusNameAction("Cathode/Anode Differential Voltage fault");
                    break;
                case 15:
                    Falut = UpdateStatusNameAction("(Spare fault 2)");
                    break;
                case 16:
                    Falut = UpdateStatusNameAction("Low powerline voltage fault");
                    break;
                case 17:
                    Falut = UpdateStatusNameAction("HV interlock fault");
                    break;
                case 18:
                    Falut = UpdateStatusNameAction("Interlock fault");
                    break;
                case 19:
                    Falut = UpdateStatusNameAction("Cooler fault");
                    break;
                case 20:
                    Falut = UpdateStatusNameAction("X-ray tube overtemp fault");
                    break;
                case 21:
                    Falut = UpdateStatusNameAction("X-ray-on lamp monitoring fault");
                    break;
                case 22:
                    Falut = UpdateStatusNameAction("Watchdog2 expired fault");
                    break;
                case 23:
                    Falut = UpdateStatusNameAction("Cathode driver overtemp fault");
                    break;
                case 24:
                    Falut = UpdateStatusNameAction("Anode driver overtemp fault");
                    break;
                case 25:
                    Falut = UpdateStatusNameAction("Cathode tank overtemp fault");
                    break;
                case 26:
                    Falut = UpdateStatusNameAction("Anode tank overtemp fault");
                    break;
                case 27:
                    Falut = UpdateStatusNameAction("Arc shutdown fault");
                    break;
                case 28:
                    Falut = UpdateStatusNameAction("Cathode arc INDICATION");
                    break;
                case 29:
                    Falut = UpdateStatusNameAction("Anode arc INDICATION");
                    break;
                case 30:
                    Falut = UpdateStatusNameAction("X-ray driver-running INDICATION");
                    break;
                case 31:
                    Falut = UpdateStatusNameAction("(Spare fault)");
                    break;
                default:
                    Falut = "Falut";
                    break;
            }
            return Falut + Position.ToString();
        }
    }
}
