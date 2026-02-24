using BG_Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace BG_Entities
{
    [Description("视图接口")]

    public interface IConditionView
    {
        /// <summary>
        /// 设置车体版本
        /// </summary>
        void SetCarVersion(ControlVersion cv);
        /// <summary>
        /// 设置TabName
        /// </summary>
        void SetSelectTabName(string TabName);
        /// <summary>
        /// 获取模块高度
        /// </summary>
        /// <returns></returns>
        double GetHeight();
        /// <summary>
        /// 获取模块宽度
        /// </summary>
        /// <returns></returns>
        double GetWidth();

        /// <summary>
        /// 获取模块名称
        /// </summary>
        /// <returns></returns>
        string GetName();
        /// <summary>
        /// 判断每次点击设备是否都链接了设备
        /// </summary>
        /// <returns></returns>
        bool IsConnectionEquipment();
        /// <summary>
        /// 把当前窗口给Show出来
        /// </summary>
        void Show(Window _OwnerWin);
        /// <summary>
        /// 把当前模块关闭
        /// </summary>
        void Close();
    }



    [Description("用户控件接口")]

    public interface IUserControlView
    {
        /// <summary>
        /// 设置车体版本
        /// </summary>
        void SetCarVersion(ControlVersion cv);

        /// <summary>
        /// 获取模块高度
        /// </summary>
        /// <returns></returns>
        double GetHeight();
        /// <summary>
        /// 获取模块宽度
        /// </summary>
        /// <returns></returns>
        double GetWidth();
        /// <summary>
        /// 获取模块名称
        /// </summary>
        /// <returns></returns>
        string GetName();
        /// <summary>
        /// 判断每次点击设备是否都链接了设备
        /// </summary>
        /// <returns></returns>
        bool IsConnectionEquipment();
        /// <summary>
        /// 把当前窗口给Show出来
        /// </summary>
        void Show(Window _OwnerWin); 
        /// <summary>
        /// 把当前模块关闭
        /// </summary>
        void Close();
      
        /// <summary>
        /// 获取右上角的状态栏
        /// </summary>
        /// <returns></returns>
        IEnumerable GetStatusBarStatus();
        /// <summary>
        /// 获取急停面板信息
        /// </summary>
        /// <returns></returns>
        Dictionary<string, PLCPositionEnum> GetEnergyStopPanel();
        /// <summary>
        /// 获取出束小图
        /// </summary>
        /// <returns></returns>
        BitmapImage GetRayImage();
        /// <summary>
        /// 获取出束的Potins,当出束的时候可以运行动画
        /// </summary>
        /// <returns></returns>
        List<Tuple<System.Windows.Point, System.Windows.Point>> GetRayPointList();
        /// <summary>
        /// 获取主界面大图
        /// </summary>
        /// <returns></returns>
        BitmapImage GetMainImage();
        /// <summary>
        /// 是否使用显示电子围栏
        /// </summary>
        /// <returns></returns>
        Visibility IsShowElctronicFence();
    }

    public abstract class UserControlView :UserControl, IUserControlView
    {
        public virtual void Close()
        {
            throw new NotImplementedException();
        }

        public virtual Dictionary<string, PLCPositionEnum> GetEnergyStopPanel()
        {
            throw new NotImplementedException();
        }

        public virtual double GetHeight()
        {
            throw new NotImplementedException();
        }

        public virtual BitmapImage GetMainImage()
        {
            throw new NotImplementedException();
        }

        public virtual string GetName()
        {
            throw new NotImplementedException();
        }

        public virtual BitmapImage GetRayImage()
        {
            throw new NotImplementedException();
        }

        public virtual List<Tuple<System.Windows.Point, System.Windows.Point>> GetRayPointList()
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable GetStatusBarStatus()
        {
            throw new NotImplementedException();
        }
        public virtual Visibility IsShowElctronicFence()
        {
            throw new NotImplementedException();
        }
        public virtual double GetWidth()
        {
            throw new NotImplementedException();
        }

        public virtual bool IsConnectionEquipment()
        {
            throw new NotImplementedException();
        }

        public virtual void SetCarVersion(ControlVersion cv)
        {
            throw new NotImplementedException();
        }

        public virtual void Show(Window _OwnerWin)
        {
            throw new NotImplementedException();
        }
    }
}
