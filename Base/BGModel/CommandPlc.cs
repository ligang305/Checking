using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGModel
{
    [BaseAttribute(Name = "BG_COMMAND_CONFIG", Description = "Plc通用命令")]
    public class CommandPlc
    {
        [BaseAttribute(Name = "BG_PLC_COMMAND_FLAG", Description = "Plc地址")]
        public string PlcPosition { get; set; }
        [BaseAttribute(Name = "BG_PLC_COMMAND_NAME", Description = "Plc名字")]
        public string PlcName { get; set; }
        [BaseAttribute(Name = "BG_PLC_ENUM", Description = "Plc枚举")]
        public string PlcEnum { get; set; }
    }
    [BaseAttribute(Name = "Bg_ControlVersion", Description = "系统版本")]
    public class Bg_ControlVersion
    {
        [BaseAttribute(Name = "ControlversionKey", Description = "模块代号")]
        public string ControlversionKey { get; set; }
        [BaseAttribute(Name = "ControlVersionName", Description = "系统名称")]
        public string ControlVersionName { get; set; }
        [BaseAttribute(Name = "ControlPath", Description = "组件读取文件夹")]
        public string ControlPath { get; set; }
        [BaseAttribute(Name = "BoostingEquipment", Description = "加速器设备型号")]
        public string BoostingEquipment { get; set; }
        [BaseAttribute(Name = "DetectorEquipment", Description = "探测器设备型号")]
        public string DetectorEquipment { get; set; }
        [BaseAttribute(Name = "PlcEquipment", Description = "Plc设备型号")]
        public string PlcEquipment { get; set; }
        [BaseAttribute(Name = "Scan", Description = "加速器扫描方法")]
        public string Scan { get; set; }
    }

   

}
