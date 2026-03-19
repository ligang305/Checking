using BG_Entities;
using BG_Services;
using BG_WorkFlow;
using BGModel;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shell;

using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.CommonFunc;

namespace BGUserControl
{
    [Export("PageModule", typeof(IConditionView))]
    [CustomExportMetadata(1, "FastCheck_SystemSetting", "快检系统控制", "ZZW", "1.0.0")]
    public class FastCheck_SystemSetting : BaseModules
    {
        Grid _MainGrid = new Grid();
        DockPanel dp;
     
        Border _MainBorder = new Border()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Margin = new Thickness(0),
            BorderThickness = new Thickness(2),
            BorderBrush = StrToBrush("#3F96E6"),
            CornerRadius = new CornerRadius(6),
            Background = StrToBrush("#F2F2F2")
        };
        public FastCheck_SystemSetting() : base(ControlVersion.FastCheck)
        {
            Loaded += SelfWorking_SystemSetting_Loaded;
            Unloaded += SelfWorking_SystemSetting_Unloaded;
        }
        /// <summary>
        /// 页面卸载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelfWorking_SystemSetting_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            IsModuleOpen = false;
            //Log.GetDistance().WriteInfoLogs("离开加速器模块界面");
            //BuryingPoint("离开加速器模块界面");

        }
        /// <summary>
        /// 页面加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelfWorking_SystemSetting_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            base.Base_SwitchLanguage(ConfigServices.GetInstance().localConfigModel.LANGUAGE);
            ButtonInvoke.SwitchLanguageEvent -= SwitchLanguage;
            ButtonInvoke.SwitchLanguageEvent += SwitchLanguage;
            ButtonInvoke.SwitchFontSizeEvent -= SwitchFontSize;
            ButtonInvoke.SwitchFontSizeEvent += SwitchFontSize;
            IsModuleOpen = true;
            Task.Run(() => { SetCommand(CommandDic[Command.AutoMode], false); });
            InitContent();
        }
        /// <summary>
        /// 生成内容
        /// </summary>
        private void InitContent()
        {
            _MainGrid = InitGrid();
            _MainBorder.Child = _MainGrid;
            Content = _MainBorder;
        }
        public void SwitchFontSize(string language)
        {
            SelfWorking_SystemSetting_Loaded(null, null);
        }
        public void SwitchLanguage(string language)
        {
            //base.Base_SwitchLanguage(language);
            SelfWorking_SystemSetting_Loaded(null, null);
        }

        private Grid InitGrid()
        {
            Grid _MainGrid = MakeMainGrid();

            #region 顶部栏
            dp = new DockPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                LastChildFill = true,
            };
            Border bd = new Border()
            {
                BorderThickness = new Thickness(0, 0, 0, 0),
                BorderBrush = (LinearGradientBrush)this.FindResource("LnearBorderColor"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 32,
                Background = (LinearGradientBrush)this.FindResource("TitleBackGround"),
            };

            Canvas cs = new Canvas()
            {
                Style = (Style)this.FindResource("diyCloseCanvas"),
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 1, 10, 0),
                Width = 14,
                Height = 14
            };
            cs.PreviewMouseLeftButtonDown += Bd_PreviewMouseLeftButtonDown;

            bd.Child = cs;
            #region 内容框
            MakeContentTabControl(_MainGrid);
            #endregion
            dp.MouseDown += Dp_MouseDown;
            dp.Children.Add(bd);
            Grid.SetColumn(dp, 0);
            Grid.SetRow(dp, 0);
            _MainGrid.Children.Add(dp);
            #endregion
            return _MainGrid;
        }
        /// <summary>
        /// 初始化TabControl组件
        /// </summary>
        /// <param name="MainGrid"></param>
        private void MakeContentTabControl(Grid MainGrid)
        {
            TabControl tc = new TabControl() { Style = (Style)this.TryFindResource("DiyTabControl") };
            TabItem FlowSetting = new TabItem()
            {
                Header = UpdateStatusNameAction("FlowSetting"),
                Style = (Style)this.TryFindResource("DiyTabItemStyle"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch
            };
            TabItem BoostingSetting = new TabItem()
            {
                Header = UpdateStatusNameAction("BoostingControl"),
                Style = (Style)this.TryFindResource("DiyTabItemStyle"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Background = Brushes.Green
            };
            TabItem DoseSetting = new TabItem()
            {
                Header = UpdateStatusNameAction("DoseControl"),
                Style = (Style)this.TryFindResource("DiyTabItemStyle"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Background = Brushes.Green
            };
            var SingleModel = ContentPlugins?.First(q => q.Metadata.Name == Modules.FastCheck_FlowSettingModule).Value;
            SingleModel.SetCarVersion(cv);
            //流程控制
            FlowSetting.Content = SingleModel;
            var BoostingSettingModel = ContentPlugins?.First(q => q.Metadata.Name == Modules.FastCheck_BoostSettingModule)?.Value;
            BoostingSettingModel?.SetCarVersion(cv);
            //加速器设置
            BoostingSetting.Content = BoostingSettingModel;

            var DoseModule = ContentPlugins?.First(q => q.Metadata.Name == Modules.DoseSettingModule)?.Value;
            DoseModule?.SetCarVersion(cv);
            DoseSetting.Content = DoseModule;

            
            tc.Items.Add(FlowSetting);
            var CurrentRole = ConfigServices.GetInstance().localConfigModel.Login?.LoginCode;
            if (!ConfigServices.GetInstance().localConfigModel.IsLogin || (CurrentRole != null && CurrentRole.Equals("jjAdmin")))
            {
                tc.Items.Add(BoostingSetting);
                //tc.Items.Add(CarSpeedSetting);
                tc.Items.Add(DoseSetting);
            }
            dp.Children.Add(tc);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Grid MakeMainGrid()
        {
            Grid _MainGrid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(25, GridUnitType.Star) });
            return _MainGrid;
        }
        /// <summary>
        /// 顶部导航栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dp_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                this.CurrentWindow?.DragMove();
            }
            catch (Exception ex)
            {
                CommonDeleget.WriteLogAction(ex.Message, LogType.ApplicationError, true);
            }
        }
        /// <summary>
        /// 关闭按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bd_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 获取名称
        /// </summary>
        /// <returns></returns>
        public override string GetName()
        {
            return BoosttingSetting;
        }
        /// <summary>
        /// 获取软件高度
        /// </summary>
        /// <returns></returns>
        public override double GetHeight()
        {
            return 500;
        }
        /// <summary>
        /// 获取软件宽度
        /// </summary>
        /// <returns></returns>
        public override double GetWidth()
        {
            return 900;
        }
        /// <summary>
        /// 判断是否进行连接
        /// </summary>
        /// <returns></returns>
        public override bool IsConnectionEquipment()
        {
            return Common.IsConnection;
        }

        public override void Show(Window _OwnerWin)
        {
            CurrentWindow = _OwnerWin;
            WindowChrome wc = WindowChrome.GetWindowChrome(CurrentWindow);
            wc = (WindowChrome)this.FindResource("WindowChromeKey");
            _OwnerWin.Width = GetWidth();
            _OwnerWin.Height = GetHeight();
            _OwnerWin.Title = GetName();
            _OwnerWin.Content = this;
            _OwnerWin.Show();
        }
    }
}
