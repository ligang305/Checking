using CMW.Common.Utilities;
using BGModel;
using BGUserControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BG_Entities;

namespace BGUserControl
{
    public class CommonBaseModules : BaseWindows, IConditionView
    {
        /// <summary>
        /// 切换中英文
        /// </summary>
        public virtual void Base_SwitchLanguage(string Language)
        {
       
        }


        protected void Bd_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Close();
        }
        protected void Dp_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CurrentWindow?.DragMove();
        }

        public virtual string GetName()
        {
            return "模块";
        }
        public virtual double GetHeight()
        {
            return 400;
        }

        public virtual double GetWidth()
        {
            return 600;
        }

        public virtual bool IsConnectionEquipment()
        {
            return false;
        }


        public virtual void Show(Window _OwnerWin)
        {

        }
        public virtual void Close()
        {
            CurrentWindow?.Close();
        }

        public virtual void SetCarVersion(ControlVersion cv)
        {
            
        }

        public void SetSelectTabName(string TabName)
        {
           
        }
    }
}
