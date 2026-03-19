using BG_Services;
using CMW.Common.Utilities;
using BGDAL;
using BGModel;
using BGUserControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.KeyBoard;
using BG_Entities;
using BG_WorkFlow;

namespace CMW.ViewModel
{
    public partial class CMWWindowViewModel
    {
        public App _App = (Application.Current as App);
        public LogBLL logBLL = new LogBLL();
        public CommandConfigBLL _CommandConfigBLL = new CommandConfigBLL(controlVersion);
        public ControlVersionsBLL _ControlVersionsBLL = new ControlVersionsBLL();
        public ObservableCollection<StatusModel> HitchList = new ObservableCollection<StatusModel>();
        public Dictionary<string, ObservableCollection<object>> HitchModels;
        public HookHandlerDelegate proc = new HookHandlerDelegate(HookCallback);
        bool LastConnectionStatus = false;

        #region 主页面读取插件的信息
        /// <summary>
        /// 获取右上角的状态栏
        /// </summary>
        /// <returns></returns>
        private IEnumerable _StatusBarStatus;
        public IEnumerable StatusBarStatus
        {
            get => _StatusBarStatus;
            set 
            {
                _StatusBarStatus = value;
                RaisePropertyChanged("StatusBarStatus");
            }
        }

        /// <summary>
        /// 获取急停面板信息
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, PLCPositionEnum> _EnergyStopPoints;
        public Dictionary<string, PLCPositionEnum> EnergyStopPoints
        {
            get => _EnergyStopPoints;
            set
            {
                _EnergyStopPoints = value;
            }
        }

        /// <summary>
        /// 获取出束小图
        /// </summary>
        /// <returns></returns>
        private BitmapImage _RayImageSource;
        public BitmapImage RayImageSource
        {
            get => _RayImageSource;
            set
            {
                _RayImageSource = value;
            }
        }
        /// <summary>
        /// 获取出束的Potins,当出束的时候可以运行动画
        /// </summary>
        /// <returns></returns>
        private List<Tuple<Point, Point>> _RayPoints;
        public List<Tuple<Point, Point>> RayPoints
        {
            get => _RayPoints;
            set
            {
                _RayPoints = value;
            }
        }

        private BitmapImage _MainImagePath;
        public BitmapImage MainImagePath
        {
            get => _MainImagePath;
            set
            {
                _MainImagePath = value;
            }
        }

        #endregion
    }
}
