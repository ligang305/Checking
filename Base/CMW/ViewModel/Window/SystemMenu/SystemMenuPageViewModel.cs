using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BG_Entities;
using BGUserControl;
using CMW.Common.Utilities;
using GalaSoft.MvvmLight.Command;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;

namespace CMW
{
    public class SystemMenuPageViewModel:BaseMvvm
    {
        private IPageNavigationService _naviMainService;
        SystemSettingMvvm systemSettingMvvm = new SystemSettingMvvm();
        #region Commands
        public RelayCommand<string> NavigateToSecondPageCommand { get; set; }

        public RelayCommand ShutdownCommand { get; set; }

        public RelayCommand ExitToDesktopCommand { get; set; }

        public RelayCommand LogoutCommand { get; set; }

        public RelayCommand LoadedCommand { get; set; }
        #endregion Commands


        public SystemMenuPageViewModel(IPageNavigationService naviMainService)
        {
            CommonDeleget.WriteLogAction("CMW.SystemMenu.SystemMenuPageViewModel", LogType.NormalLog);

            _naviMainService = naviMainService ?? throw new ArgumentNullException("naviService");
            CreateCommands();
            CommonDeleget.WriteLogAction("CMW.SystemMenu.SystemMenuPageViewModel", LogType.NormalLog);
        }

        private void CreateCommands()
        {
            NavigateToSecondPageCommand = new RelayCommand<string>(NavigateToSecondPageCommandExecute);
            ExitToDesktopCommand = new RelayCommand(ExitToDesktopCommandExecute);
            LogoutCommand = new RelayCommand(LogoutCommandExecute);
            LoadedCommand = new RelayCommand(LoadCommandExecute);
        }

        /// <summary>
        /// 根据当前用户的状态，初始化所有菜单的可见性
        /// </summary>
        private void InitButtonsVisibility()
        {
            //foreach (var ModulesItem in systemSettingMvvm.SettingModules())
            //{
            //    TabItem ModulesItemSetting = new TabItem()
            //    {
            //        Header = ModulesItem.modulesPluginsName,//UpdateStatusNameAction()
            //        Style = (Style)this.TryFindResource("DiyTabItemStyle"),
            //        HorizontalAlignment = HorizontalAlignment.Stretch,
            //        HorizontalContentAlignment = HorizontalAlignment.Stretch,
            //    };
            //    var ModulesItemPlugin = SystemStartStopController.GetIns().ContentPlugins?.First(q => q.Metadata.Name == ModulesItem.modulesPluginsName)?.Value;
            //    ModulesItemPlugin?.SetCarVersion(cv);
            //    ModulesItemPlugin.Width = GetWidth();
            //    ModulesItemPlugin.Height = GetHeight();
            //    ModulesItemSetting.Content = ModulesItemPlugin;
            //    ModulesItemSetting.Header = UpdateStatusNameAction(ModulesItemPlugin.GetName());
            //}
        }

        private void NavigateToSecondPageCommandExecute(string PageIndex)
        {
            
        }

        /// <summary>
        /// 退出软件
        /// </summary>
        private void ExitToDesktopCommandExecute()
        {
            Application.Current.Shutdown();
        }

        private void LogoutCommandExecute()
        {
            CloseSystemMenuWindow();
        }
        private void LoadCommandExecute()
        {
            InitButtonsVisibility();
        }
        /// <summary>
        /// 关闭系统菜单窗口
        /// </summary>
        private void CloseSystemMenuWindow()
        {
            this.MessengerInstance.Send(new CloseWindowMessage(WindowKeys.SystemMenuWindowKey));
        }
    }
}
