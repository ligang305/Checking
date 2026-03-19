using BG_Entities;
using BG_Services;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonFunc;
using static CMW.Common.Utilities.CommonDeleget;
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
using BG_WorkFlow;

namespace BGUserControl
{
    [Export("PageModule", typeof(IConditionView))]
    [CustomExportMetadata(1, "CombinedMovement_SystemSetting", "组合移动系统设置", "ZZW", "1.0.0")]
    public class CombinedMovement_SystemSetting : BaseModules
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
        public CombinedMovement_SystemSetting() : base(ControlVersion.Car)
        {
            Loaded += CombinedMovement_SystemSetting_Loaded;
            Unloaded += CombinedMovement_SystemSetting_Unloaded;
        }
        /// <summary>
        /// 页面卸载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CombinedMovement_SystemSetting_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            IsModuleOpen = false;
            BuryingPoint($"{UpdateStatusNameAction("LeaveBoostingModules")}");
        }
        /// <summary>
        /// 页面加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CombinedMovement_SystemSetting_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            BuryingPoint($"{UpdateStatusNameAction("EnterSystemControlLoadModules")}");
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
        public void SwitchFontSize(string FontSize)
        {
            CombinedMovement_SystemSetting_Loaded(null, null);
        }
        /// <summa
        public void SwitchLanguage(string language)
        {
            CombinedMovement_SystemSetting_Loaded(null, null);
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
            MakeContentTabControl();
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
        private void MakeContentTabControl()
        {
            TabControl tc = new TabControl() { Style = (Style)this.TryFindResource("DiyTabControl") };
            TabItem BoostingSetting = new TabItem()
            {
                Header = UpdateStatusNameAction("BoostingControl"),
                Style = (Style)this.TryFindResource("DiyTabItemStyle"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Background = Brushes.Green
            };
            //加速器设置
            var BoostingSettingModel = ContentPlugins?.First(q => q.Metadata.Name == Modules.CombinedMovement_BoostSetting)?.Value;
            if(cv == ControlVersion.CombinedMovementBetatron)
            {
                BoostingSettingModel = ContentPlugins?.First(q => q.Metadata.Name == Modules.CombinedMovement_FHBetratronBoostSetting)?.Value;
            }//CombinedMovement_FHBetratronBoostSetting
            BoostingSettingModel?.SetCarVersion(cv);
            BoostingSetting.Content = BoostingSettingModel;
            tc.Items.Add(BoostingSetting);
            TabItem HandMoveSetting = new TabItem()
            {
                Style = (Style)this.TryFindResource("DiyTabItemStyle"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Background = Brushes.Green
            };
            var CombinedMovementHandModel = ContentPlugins?.First(q => q.Metadata.Name == Modules.CombinedMovement_HandSettingModule).Value;
            CombinedMovementHandModel.SetCarVersion(cv);
            HandMoveSetting.Content = CombinedMovementHandModel;
            HandMoveSetting.Header = UpdateStatusNameAction(CombinedMovementHandModel.GetName());
            tc.Items.Add(HandMoveSetting);

            TabItem DoseSetting = new TabItem()
            {
                Header = UpdateStatusNameAction("DoseControl"),
                Style = (Style)this.TryFindResource("DiyTabItemStyle"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Background = Brushes.Green
            };
            var DoseModule = ContentPlugins?.First(q => q.Metadata.Name == Modules.DoseSettingModule)?.Value;
            DoseModule?.SetCarVersion(cv);
            DoseSetting.Content = DoseModule;
            tc.Items.Add(DoseSetting);

            if (cv != ControlVersion.CombinedMovementBetatron)
            {
                TabItem GateSetting = new TabItem()
                {
                    Header = UpdateStatusNameAction("FlowSetting"),
                    Style = (Style)this.TryFindResource("DiyTabItemStyle"),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch
                };
                var CarComponentModule = ContentPlugins?.First(q => q.Metadata.Name == Modules.SelfWorkingCarComponentModule).Value;
                CarComponentModule.SetCarVersion(cv);
                GateSetting.Content = CarComponentModule;
                GateSetting.Header = UpdateStatusNameAction(CarComponentModule.GetName());
                tc.Items.Add(GateSetting);
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
