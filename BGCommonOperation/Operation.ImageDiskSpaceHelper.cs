using BG_Entities;
using System;
using System.IO;
using System.Linq;

namespace CMW.Common.Utilities
{
    /// <summary>
    /// 图像存储磁盘的空间助手类
    /// </summary>
    public static class ImageDiskSpaceHelper
    {
        /// <summary>
        /// 图像文件存储根路径
        /// </summary>
        private static string _storeRoot;

        /// <summary>
        /// 用于获取图像文件夹磁盘信息
        /// </summary>
        private static DirectoryInfo _dirDriveInfo;
        /// <summary>
        /// 用于获取日志文件夹磁盘信息
        /// </summary>
        private static DirectoryInfo _dirLogDriveInfo;
        /// <summary>
        /// 用于获取磁盘信息
        /// </summary>
        private static DriveInfo _diskDriveInfo;
        /// <summary>
        /// 获取当前的磁盘空间总大小：字节数
        /// </summary>
        public static long TotalSize
        {
            get
            {
                if (_diskDriveInfo == null)
                {
                    _diskDriveInfo = new DriveInfo("D");
                }
                return _diskDriveInfo.TotalSize; 
            }
        }

        /// <summary>
        /// 获取当前的文件夹：字节数
        /// </summary>
        public static long DirTotalSize
        {
            get
            {
                if (_dirDriveInfo == null)
                {
                    _dirDriveInfo = new DirectoryInfo(SystemDirectoryConfig.GetInstance().GetScanImageFile());
                }
                _dirDriveInfo.Refresh();
                return FileSize(_dirDriveInfo);
            }
        }

        /// <summary>
        /// 获取当前的文件夹：字节数
        /// </summary>
        public static long DirLogTotalSize
        {
            get
            {
                if (_dirLogDriveInfo == null)
                {
                    _dirLogDriveInfo = new DirectoryInfo(SystemDirectoryConfig.GetInstance().GetLogDir());
                }
                _dirLogDriveInfo.Refresh();
                return FileSize(_dirLogDriveInfo);
            }
        }

        public static long FileSize(DirectoryInfo di)
        {
            long fileSize = 0;
            if (di.Exists)
            {
                foreach (DirectoryInfo item in di.GetDirectories())
                {
                    if (di.Exists)
                        fileSize += FileSize(item);
                }
                if (di.GetFiles().Count() != 0)
                {
                    foreach (var item in di.GetFiles())
                    {
                        fileSize += item.Length;
                    }
                }
            }
            return fileSize;
        }


        /// <summary>
        /// 获取当前的磁盘剩余空间总大小：字节数
        /// </summary>
        public static long TotalFreeSpace
        {
            get
            {
                if (_diskDriveInfo == null)
                {
                    _diskDriveInfo = new DriveInfo("D");
                }
                return _diskDriveInfo.TotalFreeSpace;
            }
        }

        public static double TotalUsedSpaceGB => TotalSizeGB - TotalFreeGB;

        /// <summary>
        /// 总剩余GB
        /// </summary>
        public static double TotalFreeGB => TotalFreeSpace /1024.0 /1024.0 /1024.0;

        public static double TotalSizeGB => TotalSize /1024.0 /1024.0 /1024.0;

        public static double TotalImageFileSizeGB => DirTotalSize / 1024.0 / 1024.0 / 1024.0;
        public static double TotalLogFileSizeGB => DirLogTotalSize / 1024.0 / 1024.0 / 1024.0;
        public static string DiskName => _diskDriveInfo.Name;

        /// <summary>
        /// 获取最新的空闲空间占比
        /// </summary>
        public static double GetFreeSpaceRatio()
        {
            if (TotalSize > 0)
            {
                return TotalFreeSpace / (double)TotalSize;
            }
            else
            {
                return 1;
            }
        }

        static ImageDiskSpaceHelper()
        {
            try
            {
                //var _storeRoot = Configger.ReadStr(ConfigItemXPaths)
                if (string.IsNullOrEmpty(_storeRoot))
                {
                    _storeRoot = "D:/BGCT/Data";
                }
            }
            catch (Exception exception)
            {
                CommonDeleget.WriteLogAction(exception.Message, LogType.ApplicationError, true);
            }

            try
            {
                var root = Path.GetPathRoot(_storeRoot);
                if (string.IsNullOrEmpty(root))
                {
                    root = "D";
                }

                _diskDriveInfo = new DriveInfo(root);
            }
            catch (Exception exception)
            {
                CommonDeleget.WriteLogAction(exception.Message, LogType.ApplicationError, true);
            }
        }
    }
}
