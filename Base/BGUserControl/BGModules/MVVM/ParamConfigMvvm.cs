using BG_Entities;
using BG_Services;
using BGDAL;
using BGModel;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BGUserControl
{
    public class ParamConfigMvvm:BaseNotifyPropertyChanged
    {
        public ObservableCollection<ParamConfig> ParamConfigs;

        ParamConfigDal paramConfigDal = new ParamConfigDal();
        public ICommand ParamConfigModifyCommand;
        public ICommand ParamConfigUpdateCommand;
        public ICommand ParamConfigCancelCommand;
        public ParamConfigMvvm()
        {
            ParamConfigs = new ObservableCollection<ParamConfig>();
            InitData();
        }
        private void InitData()
        {
            ParamConfigs.Clear();
            List<ParamConfig> paramConfigs = paramConfigDal.QueryParamConfig();
            foreach (var item in paramConfigs)
            {
                if(Convert.ToBoolean(item.IsShow))
                {
                    if("TestMode".Equals(item.Key)){
                        if (PLCControllerManager.GetInstance().GetStatusByPositionEnum(PLCPositionEnum.TestMode)) {
                            item.Value = "1";
                        }
                        else
                        {
                            item.Value = "0";
                        }
                    }
                    ParamConfigs.Add(item);
                }
            }
            InitCommand();
        }

        private void InitCommand()
        {
            ParamConfigModifyCommand = new ParamConfigCommand(ModifyConfig);
            ParamConfigUpdateCommand = new ParamConfigCommand(UpdateConfig);
            ParamConfigCancelCommand = new ParamConfigCommand(CancelConfig);
        }
        public void ModifyConfig(Object pc)
        {
            ParamConfig paramConfig = pc as ParamConfig;
            paramConfig.ismodifing.IsShow = System.Windows.Visibility.Collapsed;
        }
        public void UpdateConfig(Object pc)
        {
            ParamConfig paramConfig = pc as ParamConfig;
            CommonDeleget.UpdateConfigs(paramConfig.Key, paramConfig.Value,string.Empty, "false", paramConfig.IsShow);
            //HardwareParamaterServices.Service.SubmitParamater(ref paramConfig);
            if ("TestMode".Equals(paramConfig.Key))
            {
                Common.SetCommand(Common.CommandDic[Command.TestMode], paramConfig.Value == "1" ? true : false);
            }
            paramConfig.ismodifing.IsShow = System.Windows.Visibility.Visible;
        }
        public void CancelConfig(Object pc)
        {
            ParamConfig paramConfig = pc as ParamConfig;
            paramConfig.ismodifing.IsShow = System.Windows.Visibility.Visible;
        }
    }
}
