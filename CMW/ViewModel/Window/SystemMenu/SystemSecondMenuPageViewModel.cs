using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BG_Entities;
using CMW.Common.Utilities;
using GalaSoft.MvvmLight;

namespace CMW
{
    public class SystemSecondMenuPageViewModel : PageViewModelBase
    {
        public SystemSecondMenuPageViewModel()
        {
            CommonDeleget.WriteLogAction("USMS.UI.SMU.ViewModel.Pages.SystemMenu.SystemSecondMenuPageViewModel",LogType.NormalLog);


            CommonDeleget.WriteLogAction("USMS.UI.SMU.ViewModel.Pages.SystemMenu.SystemSecondMenuPageViewModel",LogType.NormalLog);
        }

        /// <summary>
        /// 窗口关闭事件：在窗口关闭，清理最后一个页面占用的资源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosed()
        {

        }
        public override void OnKeyDown(KeyEventArgs args)
        {
            
        }

        public override void Cleanup()
        {
            OnClosed();
            MessengerInstance.Unregister(this);
            base.Cleanup();
        }
    }
}
