using BG_Entities;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CMW.Common.Utilities
{
    public class NotifyIconHelp
    {
        #region Lazy Singleton

        private static NotifyIconHelp instance = null;

        private static readonly object syncRoot = new object();

        private NotifyIconHelp()
        {
        }

        public static NotifyIconHelp GetIns()
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new NotifyIconHelp();
                    }
                }
            }
            return instance;
        }

        #endregion

        private readonly System.Windows.Forms.NotifyIcon nIcon = new System.Windows.Forms.NotifyIcon();
        System.Windows.Window window;
        public void initNotifyIcon(System.Windows.Window _window)
        {
            window = _window;
            nIcon.Visible = true;
            nIcon.Icon = new Icon(AppDomain.CurrentDomain.BaseDirectory + $@"/Car.ico");
            nIcon.Text = "CMW";
            nIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(show_Click);
            nIcon.ContextMenu = new System.Windows.Forms.ContextMenu();
            System.Windows.Forms.MenuItem show = new System.Windows.Forms.MenuItem(CommonDeleget.UpdateStatusNameAction("Open"));
            show.Click += new EventHandler(show_Click);
            nIcon.ContextMenu.MenuItems.Add(show);
            System.Windows.Forms.MenuItem exit = new System.Windows.Forms.MenuItem(CommonDeleget.UpdateStatusNameAction("Close"));
            exit.Click += new EventHandler(exit_Click);
            nIcon.ContextMenu.MenuItems.Add(exit);
            window.StateChanged += Window_StateChanged;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (window.WindowState == WindowState.Minimized) window.Hide();
            else
            {
                ResizeWindow();
            }
        }

        private void exit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void show_Click(object Sender, EventArgs e)
        {
            window.Topmost = true;
            window.ResizeMode = ResizeMode.CanResize;
            ResizeWindow();
            window.Show();
            window.Activate();
            window.ResizeMode = ResizeMode.NoResize;
            window.WindowState = WindowState.Normal;
            window.Topmost = false;
        }

        private void ResizeWindow()
        {
            DisplayMonitor.RefreshActualScreens();
            CommonDeleget.WriteLogAction($@"show_Click DisplayMonitor.ActualScreens.Count():{DisplayMonitor.ActualScreens.Count()}", LogType.ApplicationError);
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
            CommonDeleget.WriteLogAction($@"Width:{window.Width},Height:{window.Height}", LogType.ApplicationError);
            window.Top = 0.0;
            window.Left = 0.0;
        }

        protected void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            window.Hide();
        }
    }


    public  class DisplayMonitor
    {
        public const int WM_DISPLAYCHANGE = 0x007e;
        [DllImport("user32")]
        private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lpRect, MonitorEnumProc callback, int dwData);

        private delegate bool MonitorEnumProc(IntPtr hDesktop, IntPtr hdc, ref Rect pRect, int dwData);

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        public static List<MonitorInfo> ActualScreens = new List<MonitorInfo>();

        public static void RefreshActualScreens()
        {
            ActualScreens.Clear();
            MonitorEnumProc callback = (IntPtr hDesktop, IntPtr hdc, ref Rect prect, int d) =>
            {
                ActualScreens.Add(new MonitorInfo()
                {
                    Bounds = new Rectangle()
                    {
                        X = prect.left,
                        Y = prect.top,
                        Width = prect.right - prect.left,
                        Height = prect.bottom - prect.top,
                    },
                    IsPrimary = (prect.left == 0) && (prect.top == 0),
                });


                return true;
            };
            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, callback, 0);
        }
    }
    public class MonitorInfo
    {
        public bool IsPrimary = false;
        public Rectangle Bounds = new Rectangle();
    }
}
