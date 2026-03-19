using BG_Services;
using BG_Entities;
using BGModel;
using System;
using System.Collections.Generic;
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
using BG_WorkFlow;
using BGUserControl;
using CMW.Common.Utilities;

namespace BGUserControl
{
    /// <summary>
    /// EquipmentTypeCtl.xaml 的交互逻辑
    /// </summary>
    public partial class EquipmentTypeCtl : UserControl
    {
        EquipmentTypeCtlMvvm equipmentTypeCtlMvvm = new EquipmentTypeCtlMvvm();
        public EquipmentTypeCtl()
        {
            InitializeComponent();
            DataContext = equipmentTypeCtlMvvm;
            equipmentTypeCtlMvvm.SearchEquipmentType();
        }
    }

    public class EquipmentTypeCtlMvvm: BaseMvvm
    {
        private HardwareState _EquipmentTypee = new HardwareState();
        public HardwareState EquipmentType
        {
            get => _EquipmentTypee;
            set {
                _EquipmentTypee = value;
            }
        }
        /// <summary>
        /// 查询设备编号
        /// </summary>
        public void SearchEquipmentType()
        {
            Task.Run(() =>
            {
                EquipmentType.LabelText = ConfigServices.GetInstance().localConfigModel.EquipmentNo;
                System.Diagnostics.Debug.WriteLine($"gang左上角型号：{EquipmentType.LabelText} ");

                string tempEquipment = string.Empty;
                while (string.IsNullOrEmpty(tempEquipment))
                {
                    Thread.Sleep(500);
                    if (PLCControllerManager.GetInstance().IsConnect())
                    {
                        tempEquipment = PLCControllerManager.GetInstance().GetPlcBlockStr(ConfigServices.GetInstance().localConfigModel.EquipmentAddress, Convert.ToUInt16(ConfigServices.GetInstance().localConfigModel.EquipmentLength));
                    }
                }
#if DEBUG
                EquipmentType.LabelText = tempEquipment.TrimEnd().ToUpper();
                ConfigServices.GetInstance().localConfigModel.EquipmentNo = EquipmentType.LabelText;
#else
                EquipmentType.LabelText = tempEquipment;
                IniDll.Write(BG_Entities.Section.SOFT, "EquipmentNo", EquipmentType.LabelText);
                ConfigServices.GetInstance().localConfigModel.EquipmentNo = tempEquipment;
#endif
            });
        }
    }
}
