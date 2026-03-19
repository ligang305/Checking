using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BG_Entities;
using BGUserControl;
using GalaSoft.MvvmLight.Messaging;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;

namespace CMW
{
    /// <summary>
    /// SettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingPage : Page
    {
        SettingPageViewModel SettingPageViewModel;
        public SettingPage()
        {
            InitializeComponent();

            Loaded += SettingPage_Loaded;

            SettingPageViewModel = this.DataContext as SettingPageViewModel;

            Messenger.Default.Register<CloseWindowMessage>(this, ClosePageMessageAction);
        }

        private void SettingPage_Loaded(object sender, RoutedEventArgs e)
        {
            SettingPageViewModel.LoadCommand.Execute(this);
            MakeContentTabControl();
        }

        /// <summary>
        /// 关闭Page时，注销Messenger
        /// </summary>
        /// <param name="msg"></param>
        private void ClosePageMessageAction(CloseWindowMessage msg)
        {
            if (msg.WindowKey == WindowKeys.SettingPage)
            {
                Messenger.Default.Unregister(this);
            }
        }


        /// <summary>
        /// 初始化TabControl组件
        /// </summary>
        /// <param name="MainGrid"></param>
        private void MakeContentTabControl()
        {
            DiyTabControl.Items.Clear();
            foreach (var ModulesItem in SettingPageViewModel.SettingModules())
            {
                TabItem ModulesItemSetting = new TabItem()
                {
                    Header = ModulesItem.modulesPluginsName,//UpdateStatusNameAction()
                    Style = (Style)this.TryFindResource("DiyTabItemStyle"),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                };
                var ModulesItemPlugin = SettingPageViewModel.ContentPlugins?.First(q => q.Metadata.Name == ModulesItem.modulesPluginsName)?.Value;
                ModulesItemPlugin?.SetCarVersion(controlVersion);
                ModulesItemPlugin.Width = this.ActualWidth;
                ModulesItemPlugin.Height = this.ActualHeight;
                ModulesItemSetting.Content = ModulesItemPlugin;
                ModulesItemSetting.Header = UpdateStatusNameAction(ModulesItemPlugin.GetName());
                DiyTabControl.Items.Add(ModulesItemSetting);
            }
        }
    }
}
