/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:CMW"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using CommonServiceLocator;
using System.Windows;
using BGUserControl;
using BG_Entities;
using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using BG_WorkFlow;
using BGLogs;
using CMW.Common.Utilities;
using System.IO;
using System.Linq;

namespace CMW.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        private CompositionContainer container = null;
        //你要导入的类型和元数据数组，所有继承IConditionView接口并导出的页面都在这个数组里  
        [ImportMany("PageModule", typeof(IConditionView), Source = ImportSource.Local, RequiredCreationPolicy = CreationPolicy.NonShared)]
        //保存插件的内存对象
        public Lazy<IConditionView, IMetaData>[] UserControlPlugins { get; set; }
        public IConditionView CurrentModule;


        public static ViewModelLocator Locator
        {
            get { return Application.Current.FindResource("Locator") as ViewModelLocator; }
        }
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<TopStatusMonitorPanelMvvm>();
            SetupNavigation();
            ExcuteLoadWindow();
        }
        /// <summary>
        /// 窗体加载时调用插件的方法
        /// </summary>
        private void ExcuteLoadWindow()
        {
            try
            {
                SystemDirectoryConfig.AppDir = AppDomain.CurrentDomain.BaseDirectory;
                var dir = new DirectoryInfo(SystemDirectoryConfig.AppDir + GetModulesPath());
                if (dir.Exists)
                {
                    //就是这里，读取所有符合条件的dll
                    var catalog = new DirectoryCatalog(dir.FullName, "BGUserControl.dll");
                    container = new CompositionContainer(catalog);
                    try
                    {
                        container.ComposeParts(this);
                    }
                    catch (Exception ce)
                    {
                        Log.GetDistance().WriteErrorLogs(ce.StackTrace);
                    }
                    UserControlPlugins.OrderBy(p => p.Metadata.Priority);
                }
                StartUgrApplication.Services.StartUgr();
            }
            catch (Exception ex)
            {
                Log.GetDistance().WriteInfoLogs($"Module File  Error:{ex.StackTrace}");
            }
        }
        private string GetModulesPath()
        {
            return string.Empty;
        }

        private void SetupNavigation()
        {
            var navigationService = new PageNanigationservice();
            navigationService.AddPage(WindowKeys.MainMenuKey,  new Uri("pack://application:,,,/View/SystemMenu/SystemMenuPage.xaml", UriKind.Absolute));
            navigationService.AddPage(WindowKeys.SecondMainMenuKey, new Uri("pack://application:,,,/View/SystemMenu/SystemSecondMenuPage.xaml", UriKind.Absolute));
            navigationService.AddPage(WindowKeys.SettingPage, new Uri("pack://application:,,,/View/SettingPage/SettingPage.xaml", UriKind.Absolute));
            
            SimpleIoc.Default.Register<IPageNavigationService>(() => navigationService, "MenuPageNavigationService");
        }
        public static IPageNavigationService MenuPageNavigationService
        {
            get { return ServiceLocator.Current.GetInstance<IPageNavigationService>("MenuPageNavigationService"); }
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }
        public TopStatusMonitorPanelMvvm TopStatusMonitorPanelMvvm
        {
            get
            {
                return ServiceLocator.Current.GetInstance<TopStatusMonitorPanelMvvm>();
            }
        }
        /// <summary>
        /// 菜单窗口
        /// </summary>
        public SystemMenuWindowViewModel SystemMenuWindowVm
        {
            get { return new SystemMenuWindowViewModel(MenuPageNavigationService); }
        }
        /// <summary>
        /// 二级菜单窗口
        /// </summary>
        public SystemSecondMenuPageViewModel SystemSecondMenuWindowVm
        {
            get { return new SystemSecondMenuPageViewModel(); }
        }

        /// <summary>
        /// 设置界面 ViewModel
        /// </summary>
        public SettingPageViewModel SettingPageVm
        {
            get { return new SettingPageViewModel(); }
        }
        /// <summary>
        /// 主菜单界面Vm
        /// </summary>
        public SystemMenuPageViewModel SystemMenuPageVm
        {
            get { return new SystemMenuPageViewModel(MenuPageNavigationService); }
        }
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}