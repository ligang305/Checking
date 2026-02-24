using BG_Entities;
using BGModel;
using CMW.Common.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace BG_WorkFlow
{
    public class CommandConfigBLL
    {
        public List<CommandPlc> CommandPlcList;
        XmlBaseDataDAL xmlObject = new XmlBaseDataDAL();

        public CommandConfigBLL(ControlVersion controlVersion)
        {
            CommandPlcList = GetCommandPlcDataModel(controlVersion);
        }
        public CommandConfigBLL()
        {
        }
        public List<CommandPlc> GetCommandPlcDataModel(ControlVersion controlVersion)
        {
            //将xml路径先创立 创建节点
            return xmlObject.GetObjects<CommandPlc>(GetPath(controlVersion), GlogbalBGModel.BG_COMMAND_CONFIGS).ToList();
        }
        private string GetPath(ControlVersion controlVersion)
        {
            return SystemDirectoryConfig.GetInstance().GetCommandConfig(controlVersion);
        }

        public bool SaveCommandPlcDataModel(CommandPlc _TCommandPlcModel, ControlVersion controlVersion = ControlVersion.Car)
        {
            return xmlObject.SaveSingleDataByCondition
                (GetPath(controlVersion), _TCommandPlcModel, GlogbalBGModel.BG_COMMAND_CONFIGS, "BG_PLC_ENUM", _TCommandPlcModel.PlcEnum, GlogbalBGModel.BG_COMMAND_CONFIG);
        }
        public bool SaveConfigDataModel(IEnumerable<CommandPlc> _TCommandPlcs, ControlVersion controlVersion = ControlVersion.Car)
        {
            bool isSuccess = false;
            foreach (var item in _TCommandPlcs)
            {
                isSuccess = SaveCommandPlcDataModel(item, controlVersion);
                if (!isSuccess) break;
            }
            return isSuccess;
        }
    }
}
