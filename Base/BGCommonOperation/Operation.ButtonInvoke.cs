using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMW.Common.Utilities
{
    public class ButtonInvoke
    {
        public delegate void ButtonCommandInvoke(object CommandName  );
        //按钮触发的事件
        public static ButtonCommandInvoke ButtonInvokeEvent;

             
        public delegate void SwitchLanguageInvoke(string language);     
        public static SwitchLanguageInvoke SwitchLanguageEvent;

        public delegate void SwitchFontSizeInvoke(string FontSize);
        public static SwitchFontSizeInvoke SwitchFontSizeEvent;

        public delegate void InquirePlcStatus(List<bool> StatusList);
        public static InquirePlcStatus InquirePlcStatusEvent;

        public delegate void DoubleClickToOpen(bool OpenOrClose);
        public static DoubleClickToOpen DoubleClickToOpenEvent;

        public static void DoubleClickToOpenAction(bool OpenOrClose)
        {
            if (ButtonInvoke.DoubleClickToOpenEvent != null && ButtonInvoke.DoubleClickToOpenEvent.GetInvocationList().Count() != 0)
            {
                ButtonInvoke.DoubleClickToOpenEvent?.Invoke(OpenOrClose);
            }
        }

        public static void SwitchFontSizeAction(string key)
        {
            if (ButtonInvoke.SwitchFontSizeEvent != null && ButtonInvoke.SwitchFontSizeEvent.GetInvocationList().Count() != 0)
            {
                ButtonInvoke.SwitchFontSizeEvent?.Invoke(key); 
            }
        }
        public static void SwitchLanguageAction(string key)
        {
            if (ButtonInvoke.SwitchLanguageEvent != null && ButtonInvoke.SwitchLanguageEvent.GetInvocationList().Count() != 0)
            {
                ButtonInvoke.SwitchLanguageEvent?.Invoke(key);
            }
        }
        public static void InquirePlcStatusAction(List<bool> StatusList)
        {
            if (ButtonInvoke.InquirePlcStatusEvent != null && ButtonInvoke.InquirePlcStatusEvent.GetInvocationList().Count() != 0)
            {
                ButtonInvoke.InquirePlcStatusEvent?.Invoke(StatusList);
            }
        }

    }
}
