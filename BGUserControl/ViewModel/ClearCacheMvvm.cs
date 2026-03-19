using BG_Entities;
using BGModel;
using CMW.Common.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;

namespace BGUserControl
{
    public class ClearCacheMvvm: BaseNotifyPropertyChanged
    {
        #region Mvvm Model
        private CachePageModel _cachePageModel;
        public CachePageModel cachePageModel
        {
            get => _cachePageModel;
            set { _cachePageModel = value;RaisePropertyChanged("cachePageModel"); }
        }
        #endregion

        #region Command
        ICommand ClearCommand;
        #endregion
        public ClearCacheMvvm()
        {
            cachePageModel = new CachePageModel();
            ButtonInvoke.SwitchFontSizeEvent += SwitchFontSize;
            ButtonInvoke.SwitchLanguageEvent += SwitchLanguage;
            InitControlText();
            InitCacheSize();
            InitCommand();
        }
        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="FontSize"></param>
        public void SwitchLanguage(string language)
        {
            InitControlText();
        }
        /// <summary>
        /// 切换字体大小
        /// </summary>
        /// <param name="FontSize"></param>
        public void SwitchFontSize(string FontSize)
        {
            InitControlText();
        }

        private void InitCacheSize()
        {
            cachePageModel.lblDataImagecacheSize=cachePageModel.lblLogscacheSize = cachePageModel.lblCacheSize = CommonDeleget.UpdateStatusNameAction("Calculating");
            Task.Run(() => {
                cachePageModel.lblCacheSize = $@"{ImageDiskSpaceHelper.TotalImageFileSizeGB.ToString("F2")}G/{ImageDiskSpaceHelper.TotalFreeGB.ToString("F2")}G({CommonDeleget.UpdateStatusNameAction("Intal")}：{ImageDiskSpaceHelper.TotalSizeGB.ToString("F2")}G)";
                cachePageModel.lblLogscacheSize = $@"{ImageDiskSpaceHelper.TotalLogFileSizeGB.ToString("F2")}G/{ImageDiskSpaceHelper.TotalFreeGB.ToString("F2")}G({CommonDeleget.UpdateStatusNameAction("Intal")}：{ImageDiskSpaceHelper.TotalSizeGB.ToString("F2")}G)";
            });
            Task.Run(() => {
                cachePageModel.lblDataImagecacheSize = CommonDeleget.SearchStringDataAction();
            });
        }

        private void InitCommand()
        {
           // ClearCommand = new RelayCommand(ClearCache, CanUpdateNameExecute);
        }

        bool CanUpdateNameExecute()
        {
            return true;
        }
        public void ClearCache(string DirOrFilePath)
        {
            DirOrFilePath = TagFileStr(DirOrFilePath);
            if (string.IsNullOrEmpty(DirOrFilePath))
            {
                return;
            }
            if(DirOrFilePath.Equals("DbImage"))
            {
                Task.Run(() => {
                    CommonDeleget.DeleteDataAction();
                    InitCacheSize();
                }).Wait();
                return;
            }
            Task.Run(() => {
                var dir = new DirectoryInfo(DirOrFilePath);
                foreach (FileInfo item in dir.GetFiles())
                {
                    try
                    {
                        item.Delete();
                    }
                    catch (Exception ex)
                    {
                        CommonDeleget.WriteLogAction(ex.Message, LogType.ApplicationError, true);
                    }
                }
                foreach (DirectoryInfo item in dir.GetDirectories())
                {
                    ClearCache(item.FullName);
                    if(item.GetFiles().Count()  == 0 || item.GetDirectories().Count() == 0)
                    {
                        try
                        {
                            item.Delete();
                        }
                        catch (Exception ex)
                        {
                            CommonDeleget.WriteLogAction(ex.Message, LogType.ApplicationError, true);
                        }
                     
                    }
                }
            }).Wait();
            InitCacheSize();
        }

        public string TagFileStr(string TagName)
        {
            string FIle = TagName;
            switch (TagName)
            {
                case "LOG":
                    cachePageModel.lblLogscacheSize = CommonDeleget.UpdateStatusNameAction("Clearing");
                    return SystemDirectoryConfig.GetInstance().GetLogDir();
                case "IMAGE":
                    cachePageModel.lblCacheSize = CommonDeleget.UpdateStatusNameAction("Clearing");
                    return SystemDirectoryConfig.GetInstance().GetScanImageFile();
                case "DbImage":
                    cachePageModel.lblDataImagecacheSize = CommonDeleget.UpdateStatusNameAction("Clearing");
                    return "DbImage";
                default:
                    break;
            }
            return FIle;
        }



        /// <summary>
        /// 初始化文本
        /// </summary>
        public void InitControlText()
        {
            cachePageModel.lblCacheTitleFontsize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.Normal);

            cachePageModel.lblCacheTitle =  CommonDeleget.UpdateStatusNameAction("ImageCache");

            cachePageModel.lblDataImageCacheTitle =  CommonDeleget.UpdateStatusNameAction("DBCache"); 

            cachePageModel.lblLogCacheTitle =  CommonDeleget.UpdateStatusNameAction("LogCache");

            cachePageModel.btnCacheTitle = CommonDeleget.UpdateStatusNameAction("Clear");

            cachePageModel.btnDataImageCacheTitle =  CommonDeleget.UpdateStatusNameAction("Clear");

            cachePageModel.btnLogCacheTitle =  CommonDeleget.UpdateStatusNameAction("Clear");

        }
    }

    /// <summary>
    /// 缓存页面Mvvm的 实体对象
    /// </summary>
    public class CachePageModel: BaseNotifyPropertyChanged
    {
        private double _lblCacheTitleFontsize;

        private string _lblCacheTitle;
        private string _lblLogscacheTitle;
        private string _lblDataImagecacheTitle;

        private string _btnCacheTitle;
        private string _btnLogscacheTitle;
        private string _btnDataImagecacheTitle;

        private string _lblCacheSize;
        private string _lblLogscacheSize;
        private string _lblDataImagecacheSize;



        /// <summary>
        /// lbl缓存大小
        /// </summary>
        public string lblCacheSize
        {
            get { return _lblCacheSize; }
            set
            {
                _lblCacheSize = value;
                RaisePropertyChanged("lblCacheSize");
            }
        }

        /// <summary>
        /// lbl日志缓存大小
        /// </summary>
        public string lblLogscacheSize
        {
            get { return _lblLogscacheSize; }
            set
            {
                _lblLogscacheSize = value;
                RaisePropertyChanged("lblLogscacheSize");
            }
        }
        /// <summary>
        /// lbl lbl数据库缓存大小
        /// </summary>
        public string lblDataImagecacheSize
        {
            get { return _lblDataImagecacheSize; }
            set
            {
                _lblDataImagecacheSize = value;
                RaisePropertyChanged("lblDataImagecacheSize");
            }
        }

        /// <summary>
        /// lbl缓存的字体
        /// </summary>
        public double lblCacheTitleFontsize { get { return _lblCacheTitleFontsize; } set { _lblCacheTitleFontsize = value;
                RaisePropertyChanged("lblCacheTitleFontsize");  } }

        /// <summary>
        /// lbl缓存的title
        /// </summary>
        public string lblCacheTitle { get { return _lblCacheTitle;  } set { _lblCacheTitle = value;
                RaisePropertyChanged("lblCacheTitle"); } }
        /// <summary>
        /// lbl 日志缓存title
        /// </summary>
        public string lblDataImageCacheTitle { get { return _lblDataImagecacheTitle; } set { _lblDataImagecacheTitle = value;
                RaisePropertyChanged("lblDataImageCacheTitle"); } }
        /// <summary>
        /// lbl 日志缓存title
        /// </summary>
        public string lblLogCacheTitle { get { return _lblLogscacheTitle; } set { _lblLogscacheTitle = value;
                RaisePropertyChanged("lblLogCacheTitle");
            } }

        /// <summary>
        /// btn 缓存的title
        /// </summary>
        public string btnCacheTitle { get { return _btnCacheTitle; } set { _btnCacheTitle = value;
                RaisePropertyChanged("btnCacheTitle");
            } }
        /// <summary>
        /// btn 日志缓存title
        /// </summary>
        public string btnDataImageCacheTitle { get { return _btnLogscacheTitle; } set { _btnLogscacheTitle = value;
                RaisePropertyChanged("btnDataImageCacheTitle");
            } }
        /// <summary>
        /// btn 日志缓存title
        /// </summary>
        public string btnLogCacheTitle { get { return _btnDataImagecacheTitle; } set { _btnDataImagecacheTitle = value;
                RaisePropertyChanged("btnLogCacheTitle");
            } }
    }
}
