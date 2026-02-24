using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace AutoStartup
{
    public class AppHelper
    {
        private const string AppStartupName = "BGCMW";
        const int MaxPathLen = 260; //windows 允许路径的最大长度就是 260 个字节

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetLongPathName(
        [MarshalAs(UnmanagedType.LPTStr)] string shortPath,
        [MarshalAs(UnmanagedType.LPTStr)] StringBuilder longPath,
        int longLen
        );
        /// <summary>
        /// 启动或禁用开机自动启动
        /// </summary>
        /// <param name="on"></param>
        public static void EnabledAutoStartup(bool on)
        {
            if (on)
            {
                var appPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CMW.exe");
                AppHelper.AddToStartUp(AppStartupName, appPath);
            }
            else
            {
                AppHelper.RemoveFromStartUp(AppStartupName);
            }
        }
        /// <summary>
        /// 将指定的exe添加到开机启动项
        /// </summary>
        /// <param name="exeName">exe名称，将在开机启动项中记录</param>
        /// <param name="exePath">exe文件的完整路径</param>
        public static void AddToStartUp(string exeName, string exePath)
        {
            RegistryKey currentUser = Registry.LocalMachine;
            RegistryKey run =
                currentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true) ??
                currentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");                

            if (run != null)
            {
                StringBuilder longExePath = new StringBuilder(MaxPathLen);
                int ret = GetLongPathName(exePath, longExePath, MaxPathLen);
                Console.WriteLine(longExePath);
                run.SetValue(exeName, longExePath.ToString(), RegistryValueKind.String);
            }
        }
        /// <summary>
        /// 从开机启动项中移除指定的启动项名称
        /// </summary>
        /// <param name="exeName"></param>
        public static void RemoveFromStartUp(string exeName)
        {
            RegistryKey currentUser = Registry.LocalMachine;
            RegistryKey run = currentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

            try
            {
                if (run != null)
                {
                    Console.WriteLine(exeName);
                    run.DeleteValue(exeName);
                }
            }
            catch (Exception e)
            {

            }
        }
   
    }
}
