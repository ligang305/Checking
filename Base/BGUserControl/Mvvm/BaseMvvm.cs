using BG_Services;
using CMW.Common.Utilities;
using BGModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
using BG_Entities;
using GalaSoft.MvvmLight;
using BG_WorkFlow;

namespace BGUserControl
{
    public abstract class BaseMvvm : ViewModelBase
    {
        public static FontConfigModel _fontSizeModel = new FontConfigModel();
        public FontConfigModel fontSizeModel { get => _fontSizeModel;set { _fontSizeModel = value; } }
        public Action SwitchLanguageAction;
        public Action SwitchFontsizeAction;
        protected IPLCEquipment PlcManager;
        /// <summary>
        /// 命令字典
        /// </summary>
        protected Dictionary<Command, string> MouduleCommandDic = new Dictionary<Command, string>();

        public BaseMvvm()
        {
            ButtonInvoke.SwitchLanguageEvent -= SwitchLanguage;
            ButtonInvoke.SwitchLanguageEvent += SwitchLanguage;
               
            ButtonInvoke.SwitchFontSizeEvent -= SwitchFontSize;
            ButtonInvoke.SwitchFontSizeEvent += SwitchFontSize;

            ButtonInvoke.InquirePlcStatusEvent -= InquirePlcStatus;
            ButtonInvoke.InquirePlcStatusEvent += InquirePlcStatus;


            PLCControllerManager.GetInstance().HandwareParamaterCallback -= InquirePlcStandardStatus;
            PLCControllerManager.GetInstance().HandwareParamaterCallback += InquirePlcStandardStatus;


            PlcService.GetInstance().ConnectionAction -= ConnectionStatusCallback;
            PlcService.GetInstance().ConnectionAction += ConnectionStatusCallback;

            if (ConfigServices.GetInstance().localConfigModel.IsUserBS && controlVersion != ControlVersion.BS)
            {
                BSPlcService.GetInstance().ConnectionAction += BSConnectionStatusCallback;
                BSPLCControllerManager.GetInstance().HandwareParamaterCallback -= BSInquirePlcStandardStatus;
                BSPLCControllerManager.GetInstance().HandwareParamaterCallback += BSInquirePlcStandardStatus;
            }

            AccelatorService.GetInstance().ConnectionAction -= AccelatorConnectionStatus;
            AccelatorService.GetInstance().ConnectionAction += AccelatorConnectionStatus;

            DetecotrControllerManager.GetInstance().DetecotorConnectionAction -= DetecotorConnection;
            DetecotrControllerManager.GetInstance().DetecotorConnectionAction += DetecotorConnection;
            if (controlVersion != ControlVersion.BS && ConfigServices.GetInstance().localConfigModel.IsUserBS)
            {
                PlcManager = BSPLCControllerManager.GetInstance();
                MouduleCommandDic = BSPLCControllerManager.GetInstance().BSCommandDic;
            }
            else
            {
                PlcManager = PLCControllerManager.GetInstance();
                MouduleCommandDic = CommandDic;
            }
        }

        private void SwitchLanguage(string Language)
        {
            LoadUIText();
        }

        private void SwitchFontSize(string Fontsize)
        {
            LoadUIFontSize();
        }

        protected virtual void InquirePlcStatus(List<bool> StatusList)
        {

        }

        protected virtual void InquirePlcStandardStatus(PLCDBStatus PlcDbStatus)
        {

        }
        bool Value = true;
        protected virtual void BSInquirePlcStandardStatus(PLCDBStatus PlcDbStatus)
        {
           
        }

        protected virtual void ConnectionStatusCallback(object services, bool ConnectStatus)
        {
            try
            {
                ConnectionStatus(ConnectStatus);
            }
            catch
            {

            }
        }
        protected virtual void ConnectionStatus(bool ConnectStatus)
        {

        }
        protected virtual void BSConnectionStatusCallback(object services, bool ConnectStatus)
        {
            try
            {
                BSConnectionStatus(ConnectStatus);
            }
            catch
            {

            }
        }
        protected virtual void BSConnectionStatus(bool ConnectStatus)
        {

        }
        protected virtual void AccelatorConnectionStatus(bool ConnectStatus)
        {

        }

        public virtual void LoadUIText()
        {

        }

        public virtual void LoadUIFontSize()
        {
            fontSizeModel.normalMiddle = UpdateFontSizeAction(CMWFontSize.NormalMiddle);
            fontSizeModel.small = UpdateFontSizeAction(CMWFontSize.Small);
            fontSizeModel.middle = UpdateFontSizeAction(CMWFontSize.Middle);
            fontSizeModel.normal = UpdateFontSizeAction(CMWFontSize.Normal);
            fontSizeModel.superBig = UpdateFontSizeAction(CMWFontSize.SuperBig);
        }
        protected virtual void DetecotorConnection(int DetecotrConnection)
        {

        }

        public override void Cleanup()
        {
            base.Cleanup();
            //ButtonInvoke.SwitchLanguageEvent -= SwitchLanguage;
            //ButtonInvoke.SwitchFontSizeEvent -= SwitchFontSize;
            //ButtonInvoke.InquirePlcStatusEvent -= InquirePlcStatus;
            //PlcService.GetInstance().ConnectionAction -= ConnectionStatus;
            //AccelatorService.GetInstance().ConnectionAction -= AccelatorConnectionStatus;
            //DetecotrControllerManager.GetInstance().DetecotorConnectionAction -= DetecotorConnection;
        }
    }
}
