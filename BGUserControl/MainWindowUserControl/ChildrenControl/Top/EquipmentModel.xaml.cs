using BG_Services;
using BG_WorkFlow;
using BGModel;
using BGUserControl;
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

namespace BGUserControl
{
    /// <summary>
    /// EquipmentTypeCtl.xaml 的交互逻辑
    /// </summary>
    public partial class EquipmentModelCtl : UserControl
    {
        /// <summary>
        /// 设备型号
        /// </summary>
        EquipmentModelCtlMvvm equipmentModelCtlMvvm = new EquipmentModelCtlMvvm();
        public EquipmentModelCtl()
        {
            InitializeComponent();
            DataContext = equipmentModelCtlMvvm;
            equipmentModelCtlMvvm.SearchEquipmentModel();
        }
    }

    public class EquipmentModelCtlMvvm : BaseMvvm
    {
        private HardwareState _EquipmentModel = new HardwareState();
        public HardwareState EquipmentModel
        {
            get => _EquipmentModel;
            set {
                _EquipmentModel = value;
            }
        }
        /// <summary>
        /// 查询设备编号
        /// </summary>
        public void SearchEquipmentModel()
        {
            Task.Run(() =>
            {
                EquipmentModel.LabelText = ConfigServices.GetInstance().localConfigModel.Type;
            });
        }
    }
}
