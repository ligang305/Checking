using BG_Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace CMW.Common.Utilities
{
    public static class CommonFunc
    {
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

                window.ResizeMode = ResizeMode.NoResize;//设置为可调整窗体大小
                window.Width = 800;
                window.Height = 600;

                //获取窗口句柄 
                var handle = new WindowInteropHelper(window).Handle;
                //获取当前显示器屏幕
                System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromHandle(handle);

                window.Left = (screen.Bounds.Width - window.Width) / 2;
                window.Top = (screen.Bounds.Height - window.Height) / 2;

                window.WindowState = WindowState.Minimized;
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

                //获取当前显示器屏幕
                System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromHandle(handle);

                //调整窗口最大化,全屏的关键代码就是下面3句
                window.MaxWidth = screen.Bounds.Width;
                window.MaxHeight = screen.Bounds.Height;
                window.WindowState = WindowState.Maximized;


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

        #region AES加密
        /// <summary>
        ///  AES 加密
        /// </summary>
        /// <param name="str">明文（待加密）</param>
        /// <param name="key">密文</param>
        /// <returns></returns>
        public static string AesEncrypt(string str, bool isAes = false,string key = "begood-123456789")
        {
            if (string.IsNullOrEmpty(str)) return null;
            if (!isAes)
            {
                //CommonDeleget.WriteLogAction($@"isAes:{isAes},str:{str}", LogType.NormalLog);
                return str;
            }
            //CommonDeleget.WriteLogAction($@"isAes:{isAes},str:{str}", LogType.NormalLog);
            Byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);

            System.Security.Cryptography.RijndaelManaged rm = new System.Security.Cryptography.RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = System.Security.Cryptography.CipherMode.CBC,
                Padding = System.Security.Cryptography.PaddingMode.PKCS7,
                IV = Encoding.UTF8.GetBytes("16-Bytes--String"),
            };

            System.Security.Cryptography.ICryptoTransform cTransform = rm.CreateEncryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        /// <summary>
        ///  AES 解密
        /// </summary>
        /// <param name="str">明文（待解密）</param>
        /// <param name="key">密文</param>
        /// <returns></returns>
        public static string AesDecrypt(string str, bool isAes = false, string key = "begood-123456789")
        {
            if (string.IsNullOrEmpty(str)) return null;
            if (!isAes) return str;
            Byte[] toEncryptArray = Convert.FromBase64String(str);

            System.Security.Cryptography.RijndaelManaged rm = new System.Security.Cryptography.RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = System.Security.Cryptography.CipherMode.CBC,
                Padding = System.Security.Cryptography.PaddingMode.PKCS7,
                IV = Encoding.UTF8.GetBytes("16-Bytes--String"),
            };

            System.Security.Cryptography.ICryptoTransform cTransform = rm.CreateDecryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }
        #endregion

        public static bool PingIp(string IpAddress)
        {
            try
            {
                bool IsResult = false;
                Ping pingSender = new Ping();
                PingReply reply = pingSender.Send(IpAddress, 120);//第一个参数为ip地址，第二个参数为ping的时间 
                if (reply.Status == IPStatus.Success)
                {
                    IsResult = true;
                }
                else
                {
                    IsResult = false;
                }
                return IsResult;
            }
            catch 
            {
                return false;
            }
        }
        /// <summary>
        /// C#判断是否能ping通端口
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="portNum"></param>
        public static bool AddressPort(string ipAddress, int portNum)
        {
            try
            {
                IPAddress ip = IPAddress.Parse(ipAddress);
                IPEndPoint point = new IPEndPoint(ip, portNum);
                using (Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    sock.Connect(point);
                    sock.Close();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        
        public static string MillToHour(string millseconds)
        {
            if (string.IsNullOrEmpty(millseconds)) return "00:00:00";
            TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(millseconds));
            string str = "";
            if(ts.Days > 0)
            {
                str =  (ts.Hours + ts.Days * 24).ToString() + ":" + String.Format("{0:00}", ts.Minutes) + ":" + String.Format("{0:00}", ts.Seconds);
                return str;
            }
            if (ts.Hours > 0)
            {
                str = String.Format("{0:00}", ts.Hours) + ":" + String.Format("{0:00}", ts.Minutes) + ":" + String.Format("{0:00}", ts.Seconds);
                return str;
            }
            if (ts.Hours == 0 && ts.Minutes > 0)
            {
                str = "00:" + String.Format("{0:00}", ts.Minutes) + ":" + String.Format("{0:00}", ts.Seconds);
                return str;
            }
            if (ts.Hours == 0 && ts.Minutes == 0)
            {
                str = "00:00:" + String.Format("{0:00}", ts.Seconds);
                return str;
            }
            return str;
        }

        public static string GetSoftVersion()
        {
            var serverFileVersion = "";
            try
            {
                string loadexeName = System.Windows.Forms.Application.ExecutablePath;

                FileVersionInfo fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(loadexeName);

                serverFileVersion = string.Format("V{0}.{1}.{2}.{3}", fileVersionInfo.FileMajorPart, fileVersionInfo.FileMinorPart, fileVersionInfo.FileBuildPart, fileVersionInfo.FilePrivatePart);
            }
            catch
            {
                throw;
            }

            return serverFileVersion;
        }

        public static List<T> GetChildObjectsByVisualTree<T>(DependencyObject obj, Type typename) where T : FrameworkElement
        {
            DependencyObject child = null;
            List<T> childList = new List<T>();

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);

                if (child is T && (((T)child).GetType() == typename))
                {
                    childList.Add((T)child);
                }
                childList.AddRange(GetChildObjectsByVisualTree<T>(child, typename));
            }
            return childList;
        }

        public static List<T> GetChildObjectsByLogicTree<T>(DependencyObject obj, Type typename) where T : FrameworkElement
        {
            DependencyObject child = null;
            List<T> childList = new List<T>();

            foreach (var item in LogicalTreeHelper.GetChildren(obj))
            {
                if (item is DependencyObject)
                {
                    child = item as DependencyObject;

                    if (child is T && (((T)child).GetType() == typename))
                    {
                        childList.Add((T)child);
                    }
                    childList.AddRange(GetChildObjectsByLogicTree<T>(child, typename));
                }
            }
            return childList;
        }

        static public IEnumerable<T> GetVisualDescendants<T>(this DependencyObject item) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(item); ++i)
            {
                DependencyObject child = VisualTreeHelper.GetChild(item, i);
                if (typeof(T) == (child.GetType()))
                {
                    yield return (T)child;
                }
                foreach
                    (T descendant in GetVisualDescendants<T>(child))
                {
                    yield return descendant;
                }
            }
        }
        static public T FindVisualDescendant<T>(this DependencyObject item, string descendantName) where T : DependencyObject
        {
            return GetVisualDescendants<T>(item).Where(descendant =>
            {
                var frameworkElement = descendant as FrameworkElement; return frameworkElement != null && frameworkElement.Name == descendantName;
            }
            ).FirstOrDefault();
        }

   
        public static System.Windows.Media.Brush StrToBrush(string ColorStr)
        {
            System.Windows.Media.Brush color = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(ColorStr));
            return color;
        }

        //public static string StringToUTF8(string SourceValue)
        //{
        //    byte[] temp;
        //    temp = Encoding.Default.GetBytes(SourceValue);
        //    temp = Encoding.Convert(Encoding.GetEncoding("gb2312"), Encoding.GetEncoding("big5"), temp);
        //    //将  byte 数组 转换为 string 
        //    textBox2.Text = Encoding.Default.GetString(temp);
        //}


        /// <summary> 解析字符型数据 </summary>
        /// <param name="objIn"></param>
        /// <returns></returns>
        public static string ParseStr(object objIn)
        {
            if (objIn == null)
            {
                return "";
            }

            if (objIn.GetType() == typeof(System.DBNull))
            {
                return "";
            }

            return objIn.ToString().Trim();
        }
        /// <summary>
        /// 判断是否是Ip合法地址
        /// </summary>
        /// <param name="IpAddress"></param>
        /// <returns></returns>
        public static bool IsAddress(string IpAddress)
        {
            IPAddress ip;
            if (IPAddress.TryParse(IpAddress, out ip))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 生成GUID
        /// </summary>
        /// <returns></returns>
        public static string GetGuid()
        {
            return Guid.NewGuid().ToString();
        }
        /// <summary>
        /// 按照yyyyMMddhhmmss
        /// </summary>
        /// <returns></returns>
        public static string GetDateTime()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }
        /// <summary>
        /// 判断是否是合法Port
        /// </summary>
        /// <param name="IpAddress"></param>
        /// <returns></returns>
        public static bool IsPort(string IpPort)
        {
            int port = Convert.ToInt32(IpPort);
            if (port > 2 && port < 65534)
            { return true; }
            else { return false; }
        }

        public static string StrToDose(UInt32 DoseValue)
        {
            if (DoseValue < 1000000)
            {
                return $@"{(DoseValue / 1000.00).ToString("F2")} μSv/h";
            }
            else if (DoseValue < 1000000000)
            {
                return $@"{(DoseValue / 1000000.00).ToString()} mSv/h";
            }
            else
            {
                return $@"{(DoseValue / 1000000000.00).ToString()} Sv/h";
            }
        }

        /// <summary>
        /// 将SocketError转化为LoginStatus
        /// </summary>
        /// <returns></returns>
        public static LoginStatus ConvertSocketErrorToLogginStatus(SocketError httpSocketError)
        {
            switch (httpSocketError)
            {
                case SocketError.Success:
                    return LoginStatus.Success;
                case SocketError.TimedOut:
                    return LoginStatus.TimeOut;
                case SocketError.Interrupted:
                case SocketError.AccessDenied:
                case SocketError.Fault:
                case SocketError.InvalidArgument:
                case SocketError.TooManyOpenSockets:
                case SocketError.WouldBlock:
                case SocketError.InProgress:
                case SocketError.AlreadyInProgress:
                case SocketError.NotSocket:
                case SocketError.DestinationAddressRequired:
                case SocketError.MessageSize:
                case SocketError.ProtocolType:
                case SocketError.ProtocolOption:
                case SocketError.ProtocolNotSupported:
                case SocketError.SocketNotSupported:
                case SocketError.OperationNotSupported:
                case SocketError.ProtocolFamilyNotSupported:
                case SocketError.AddressFamilyNotSupported:
                case SocketError.AddressAlreadyInUse:
                case SocketError.AddressNotAvailable:
                case SocketError.NetworkDown:
                case SocketError.NetworkUnreachable:
                case SocketError.NetworkReset:
                case SocketError.ConnectionAborted:
                case SocketError.ConnectionReset:
                case SocketError.NoBufferSpaceAvailable:
                case SocketError.IsConnected:
                case SocketError.NotConnected:
                case SocketError.Shutdown:
                case SocketError.ConnectionRefused:
                case SocketError.SocketError:
                case SocketError.HostDown:
                case SocketError.HostUnreachable:
                case SocketError.ProcessLimit:
                case SocketError.SystemNotReady:
                case SocketError.VersionNotSupported:
                case SocketError.NotInitialized:
                case SocketError.Disconnecting:
                case SocketError.TypeNotFound:
                case SocketError.HostNotFound:
                case SocketError.TryAgain:
                case SocketError.NoRecovery:
                case SocketError.NoData:
                case SocketError.IOPending:
                case SocketError.OperationAborted:
                    return LoginStatus.Faild;
                default:
                    return LoginStatus.Faild;
            }
        }


        #region NewTonJson转换方法
        public static T JsonToObject<T>(string Json) where T
           : new()
        {   if (string.IsNullOrEmpty(Json)) return default(T);
            //var JsonObject = (JObject)JsonConvert.DeserializeObject(Json);
            return JsonConvert.DeserializeObject<T>(Json);
        }

        public static string ObjectToJson<T>(T Object) where T
           : new()
        {
            return JsonConvert.SerializeObject(Object);
        }
        #endregion

        public static double GetApplicationMemory()
        {
            double usedMemory = 0;
            Process p = Process.GetProcesses().Where(x => x.ProcessName.Contains(System.Diagnostics.Process.GetCurrentProcess().ProcessName)).FirstOrDefault();
            if (p != null)
            {
                p.Refresh();
                string procName = p.ProcessName;
                using (PerformanceCounter pc = new PerformanceCounter("Process", "Working Set - Private", procName))
                {
                    usedMemory = pc.NextValue() / 1024.0 / 1024.0;
                }
            }
            return usedMemory;
        }
    }
}
