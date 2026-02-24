using BG_Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace CMW.Common.Utilities
{
    public static class KeyBoard
    {

        #region 键盘钩子，屏蔽键盘用

        public struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
        public const int WM_KEYDOWN = 0x0100;
        public const int WH_KEYBOARD_LL = 13;
        public const int WM_SYSKEYDOWN = 0x0104;
        public delegate int HookHandlerDelegate(int nCode, IntPtr wparam, ref KBDLLHOOKSTRUCT lparam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookExW(int idHook, HookHandlerDelegate lpfn, IntPtr hmod, uint dwThreadID);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(String modulename);




        #endregion

        /// <summary>
        /// 窗体全屏
        /// </summary>
        /// <param name="window"></param>
        public static void FullOrMin(this Window window)
        {
            //如果是全屏,则最小化
            if (window.WindowState == WindowState.Maximized)
            {
                window.Topmost = false;
                window.WindowState = WindowState.Normal;
                window.WindowStyle = WindowStyle.SingleBorderWindow;

                DisplayMonitor.RefreshActualScreens();
                CommonDeleget.WriteLogAction($@"FullOrMin DisplayMonitor.ActualScreens_{DisplayMonitor.ActualScreens.Count()}", LogType.NormalLog);
                if (DisplayMonitor.ActualScreens.Count() >= 2)
                {
                    window.Width = DisplayMonitor.ActualScreens.Sum(q => q.Bounds.Width);
                    window.Height = SystemParameters.VirtualScreenHeight;
                }
                else
                {
                    window.Width = DisplayMonitor.ActualScreens.Sum(q => q.Bounds.Width);
                    window.Height = SystemParameters.PrimaryScreenHeight;
                }
                CommonDeleget.WriteLogAction($@"FullOrMin Width_{window.Width}.Height_{window.Height}", LogType.NormalLog);
                window.Top = 0.0;
                window.Left = 0.0;
                return;
            }

            //如果是窗口,则全屏
            if (window.WindowState == WindowState.Normal)
            {
                //变成无边窗体
                window.WindowState = WindowState.Normal;//假如已经是Maximized，就不能进入全屏，所以这里先调整状态
                window.WindowStyle = WindowStyle.None;
                window.ResizeMode = ResizeMode.NoResize;
                window.Topmost = true;//最大化后总是在最上面

                //获取窗口句柄 
                var handle = new WindowInteropHelper(window).Handle;

                ////获取当前显示器屏幕
                //System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromHandle(handle);

                ////调整窗口最大化,全屏的关键代码就是下面3句
                //window.MaxWidth = screen.Bounds.Width;
                //window.MaxHeight = screen.Bounds.Height;
                //window.WindowState = WindowState.Maximized;
                DisplayMonitor.RefreshActualScreens();
                CommonDeleget.WriteLogAction($@"DisplayMonitor.ActualScreens_{DisplayMonitor.ActualScreens.Count()}",LogType.NormalLog);
                if (DisplayMonitor.ActualScreens.Count() >= 2)
                {
                    window.Width = DisplayMonitor.ActualScreens.Sum(q => q.Bounds.Width);
                    window.Height = SystemParameters.VirtualScreenHeight;
                }
                else
                {
                    window.Width = DisplayMonitor.ActualScreens.Sum(q => q.Bounds.Width);
                    window.Height = SystemParameters.PrimaryScreenHeight;
                }
                CommonDeleget.WriteLogAction($@"Width_{window.Width}.Height_{window.Height}", LogType.NormalLog);
                window.Top = 0.0;
                window.Left = 0.0;

                //解决切换应用程序的问题
                window.Activated += new EventHandler(window_Activated);
                window.Deactivated += new EventHandler(window_Deactivated);
            }
        }

        /// <summary>
        /// 窗体全屏
        /// </summary>
        /// <param name="window"></param>
        public static void FullOrMinMutiScreen(this Window window)
        {
            //如果是全屏,则最小化
            if (window.WindowState == WindowState.Maximized)
            {
                window.Topmost = false;
                window.WindowState = WindowState.Normal;
                window.WindowStyle = WindowStyle.SingleBorderWindow;

                DisplayMonitor.RefreshActualScreens();
                CommonDeleget.WriteLogAction($@"FullOrMin DisplayMonitor.ActualScreens_{DisplayMonitor.ActualScreens.Count()}", LogType.NormalLog);
                if (DisplayMonitor.ActualScreens.Count() >= 2)
                {
                    window.Width = SystemParameters.PrimaryScreenWidth;
                    window.Height = SystemParameters.VirtualScreenHeight;
                }
                else
                {
                    window.Width = DisplayMonitor.ActualScreens.Sum(q => q.Bounds.Width);
                    window.Height = SystemParameters.PrimaryScreenHeight;
                }
                CommonDeleget.WriteLogAction($@"FullOrMin Width_{window.Width}.Height_{window.Height}", LogType.NormalLog);
                window.Top = 0.0;
                window.Left = 0.0;
                return;
            }

            //如果是窗口,则全屏
            if (window.WindowState == WindowState.Normal)
            {
                //变成无边窗体
                window.WindowState = WindowState.Normal;//假如已经是Maximized，就不能进入全屏，所以这里先调整状态
                window.WindowStyle = WindowStyle.None;
                window.ResizeMode = ResizeMode.NoResize;
                window.Topmost = true;//最大化后总是在最上面

                //获取窗口句柄 
                var handle = new WindowInteropHelper(window).Handle;

                ////获取当前显示器屏幕
                //System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromHandle(handle);

                ////调整窗口最大化,全屏的关键代码就是下面3句
                //window.MaxWidth = screen.Bounds.Width;
                //window.MaxHeight = screen.Bounds.Height;
                //window.WindowState = WindowState.Maximized;
                DisplayMonitor.RefreshActualScreens();
                CommonDeleget.WriteLogAction($@"DisplayMonitor.ActualScreens_{DisplayMonitor.ActualScreens.Count()}", LogType.NormalLog);
                if (DisplayMonitor.ActualScreens.Count() >= 2)
                {
                    window.Width = SystemParameters.PrimaryScreenWidth;
                    window.Height = SystemParameters.VirtualScreenHeight;
                }
                else
                {
                    window.Width = DisplayMonitor.ActualScreens.Sum(q => q.Bounds.Width);
                    window.Height = SystemParameters.PrimaryScreenHeight;
                }
                CommonDeleget.WriteLogAction($@"Width_{window.Width}.Height_{window.Height}", LogType.NormalLog);
                window.Top = 0.0;
                window.Left = 0.0;

                //解决切换应用程序的问题
                window.Activated += new EventHandler(window_Activated);
                window.Deactivated += new EventHandler(window_Deactivated);
            }
        }

        static void window_Deactivated(object sender, EventArgs e)
        {
            var window = sender as Window;
            window.Topmost = false;
        }

        static void window_Activated(object sender, EventArgs e)
        {
            var window = sender as Window;
            window.Topmost = true;
        }


        public static int HookCallback(int nCode, IntPtr wparam, ref KBDLLHOOKSTRUCT lparam)
        {
            if (
             nCode >= 0
             &&
             (wparam == (IntPtr)WM_KEYDOWN
             ||
             wparam == (IntPtr)WM_SYSKEYDOWN)
             )
            {
                if (lparam.vkCode == 91 || lparam.vkCode == 164 || lparam.vkCode == 9 || lparam.vkCode == 115)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            return 0;
        }

        /// <summary>
        /// 屏蔽按键
        /// </summary>
        public static void MakePro(HookHandlerDelegate proc)
        {
            using (Process curPro = Process.GetCurrentProcess())
            using (ProcessModule curMod = curPro.MainModule)
            {
                SetWindowsHookExW(WH_KEYBOARD_LL, proc, GetModuleHandle(curMod.ModuleName), 0);
            }
        }
    }
}
