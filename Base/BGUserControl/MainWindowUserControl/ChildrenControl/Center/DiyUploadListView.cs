using BG_Entities;
using BG_Services;
using BG_WorkFlow;
using BGDAL;
using BGModel;
using CMW.Common.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using static CMW.Common.Utilities.CommonDeleget;

namespace BGUserControl
{
    public class DiyUploadListView : BaseWindows
    {
        bool? isVisible = false;
        ListView lv; ListView historyLv;
        private ObservableCollection<LocalUploadImage> _TList = new ObservableCollection<LocalUploadImage>();
        LocalUploadImageBLL llb = new LocalUploadImageBLL();
        RecvMessage rm;
        public ObservableCollection<LocalUploadImage> TList
        {
            get { return _TList; }
            set { _TList = value; }
        }
        private ObservableCollection<LocalUploadImage> _HistoryList = new ObservableCollection<LocalUploadImage>();
        public ObservableCollection<LocalUploadImage> HistoryList
        {
            get { return _HistoryList; }
            set { _HistoryList = value; }
        }
        private Label lb;
        Grid MainGrid = new Grid()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Background = CommonFunc.StrToBrush("#00345c")
        };
        ConcurrentQueue<ScanImageCache> ScanImageCacheQueue = new ConcurrentQueue<ScanImageCache>();
        public DiyUploadListView()
        {
            MainGrid.Children.Clear();
            Content = MainGrid;
            InitDataSource();
            InitGridContent();
            //sp.Children.Add(MainGrid);
            lv.Loaded += DiyUploadListView_Loaded;
            IsVisibleChanged += DiyUploadListView_IsVisibleChanged;
            ButtonInvoke.SwitchLanguageEvent -= SwitchLanguage;
            ButtonInvoke.SwitchLanguageEvent += SwitchLanguage;

            ButtonInvoke.SwitchFontSizeEvent -= SwitchFontSize;
            ButtonInvoke.SwitchFontSizeEvent += SwitchFontSize;

            UploadLocalImageToOnlineForBS<LocalUploadImage>.GetInstance().UpdateHistoryListViewDataAction += GetDataFromLocalDataBase;
            UploadLocalImageToOnlineForBS<LocalUploadImage>.GetInstance().UpdateRealTimeListViewDataAction += GetDataFromRealTime;
        }


        /// <summary>
        /// 切换字体
        /// </summary>
        /// <param name="Language"></param>
        private void SwitchFontSize(string FontSize)
        {
            lb.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            foreach (var item in HistoryList)
            {
                item.IMAGE_FONTSIZE = UpdateFontSizeAction(CMWFontSize.Small);
            }
            foreach (var item in TList)
            {
                item.IMAGE_FONTSIZE = UpdateFontSizeAction(CMWFontSize.Small);
            }
        }
        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="Language"></param>
        private void SwitchLanguage(string Language)
        {
            InitDataSource();
            lb.Content = UpdateStatusNameAction("More");
            Reflash(TList);
            ReflashHistoryList(HistoryList);
        }
        /// <summary>
        /// 初始化数据源
        /// </summary>
        private void InitDataSource()
        {
            TList = new ObservableCollection<LocalUploadImage>();
            TList.Add(
                new LocalUploadImage()
                {
                    IMAGE_NAME = UpdateStatusNameAction("ImageName"),
                    IAMGE_LOCALFILE_CREATETIME = UpdateStatusNameAction("ImageCreateTime"),
                    IMAGE_UPDATETIME = UpdateStatusNameAction("ImageUploadTime"),
                    IMAGE_UPLOAD_STATUS = ImageUploadStatus.SubmitStatus,
                    IMAGE_FONTSIZE = UpdateFontSizeAction(CMWFontSize.Small)
                });
            HistoryList = new ObservableCollection<LocalUploadImage>();
            GetDataFromLocalDataBase();
        }

        /// <summary>
        /// 从数据库中读取数据
        /// </summary>
        private void GetDataFromLocalDataBase()
        {
            Task.Run(() =>
            {
                ParamaterModel<LocalUploadImage> LocalCondition = new ParamaterModel<LocalUploadImage>();
                LocalCondition.num = 4;
                LocalCondition.start = 0;
                HistoryList = llb.GetLocalUpLoadImage(LocalCondition);
                HistoryList.Insert(0, new LocalUploadImage()
                {
                    IMAGE_NAME = UpdateStatusNameAction("ImageName"),
                    IAMGE_LOCALFILE_CREATETIME = UpdateStatusNameAction("ImageCreateTime"),
                    IMAGE_UPDATETIME = UpdateStatusNameAction("ImageUploadTime"),
                    IMAGE_UPLOAD_STATUS = ImageUploadStatus.SubmitStatus,
                    IMAGE_FONTSIZE = UpdateFontSizeAction(CMWFontSize.Small)
                });
                Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ReflashHistoryList(HistoryList);
                }));
            });
        }
        /// <summary>
        /// 实时上传图片的实时状态
        /// </summary>
        private void GetDataFromRealTime(LocalUploadImage localUploadImage,bool removeOrAdd)
        {
            Debug.WriteLine("-------------------------ganggang_upload_img8-----------------------");
            if (removeOrAdd)
            {
                Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                {
                    TList.Insert(1, localUploadImage);
                }));
            }
            else
            {
                Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                {
                    TList.Remove(localUploadImage);
                }));
            }
        }
        private void DiyUploadListView_Loaded(object sender, RoutedEventArgs e)
        {
            var scrollView = CommonFunc.GetVisualDescendants<ScrollViewer>(lv);
            if (scrollView.Count() > 0)
            {
                (scrollView.First() as ScrollViewer).VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            }
            TaskPool.GetInstance().AddAndStartTask(TaskList.loopUploadScanImage, HandScanImageCacheThread);
        }

        private void CommonDeleget_UploadImageAction(int status, long Carrid, string Path)
        {
            ScanImageCache scanImageCache;
            scanImageCache.status = status;
            scanImageCache.ImagePath = Path;
            scanImageCache.Carrid = Carrid;
            ScanImageCacheQueue.Enqueue(scanImageCache);
            //Task.Run(()=> { UploadImage(Path); });
        }

        private void DiyUploadListView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            isVisible = e.NewValue as bool?;
            if (isVisible != null && isVisible == true)
            {
                CommonDeleget.UploadImageAction += CommonDeleget_UploadImageAction;
                CommonDeleget.RequestTaskEvent += CommonDeleget_RequestTaskEvent;
                CommonDeleget.CancelTaskEvent += CommonDeleget_CancelTaskEvent;
            }
            else
            {
                CommonDeleget.UploadImageAction -= CommonDeleget_UploadImageAction;
                CommonDeleget.RequestTaskEvent -= CommonDeleget_RequestTaskEvent;
            }
        }

        private void CommonDeleget_RequestTaskEvent()
        {
            try
            {
                if (ConfigServices.GetInstance().localConfigModel.IsEnabledSocketToServer)
                {
                    if (BegoodServerController.GetInstance().TaskIdQueue.Count > 0)
                    {
                        WriteLogAction($@"RequestTask : TaskIdQueue.Count{BegoodServerController.GetInstance().TaskIdQueue.Count}", LogType.SocketServices, true);
                        rm = BegoodServerController.GetInstance().TaskIdQueue.Pop();
                        WriteLogAction($@"RequestTask: CurrentTaskId is {rm?.Data}", LogType.SocketServices, true);
                        BegoodServerController.GetInstance().TaskIdQueue.Clear();
                    }
                }
                else
                {
                    rm = UploadLocalImageToOnline<LocalUploadImage>.GetInstance().GetTaskIdAndPutFTP();
                    WriteLogAction($@"RequestTask Http Data: CurrentTaskId is {rm?.Data}", LogType.SocketServices, true);
                }
            }
            catch (Exception ex)
            {
                //rm = null;
                CommonDeleget.WriteLogAction(ex.Message, LogType.ApplicationError, true);
            }
        }
        /// <summary>
        /// 任务取消
        /// </summary>
        private void CommonDeleget_CancelTaskEvent()
        {
            try
            {
                if (rm == null)
                {
                    if (BegoodServerController.GetInstance().TaskIdQueue.Count > 0)
                        rm = BegoodServerController.GetInstance().TaskIdQueue.Pop();
                    if (rm == null)
                    { return; }
                }
                if (rm.Code == "1")
                {
                    string TaskId = CommonFunc.JsonToObject<TaskInfo>(rm?.Data).TaskId;
                    WriteLogAction($@"CancelTask Data Start: CancelTaskId is {TaskId}", LogType.SocketServices, true);
                    BegoodServerController.GetInstance().SendScanFaildTask(TaskId, "Scan Stop");
                    WriteLogAction($@"CancelTask Data End: CancelTaskId is {TaskId}", LogType.SocketServices, true);
                    rm = null;
                }
            }
            catch (Exception ex)
            {
                WriteLogAction($@"CancelTask Data End has Occuar Exception: {ex.StackTrace}", LogType.SocketServices, true);
            }
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="FilePath"></param>
        public void UploadImage(ScanImageCache scanImageCache)
        {
            try
            {
                Debug.WriteLine("-------------------------gang  gang_upload_img6-----------------------");
                string FilePath = scanImageCache.ImagePath;
                if (!File.Exists(FilePath)) return;
                List<LocalUploadImage> LocalUploadImageList = new List<LocalUploadImage>();
                LocalUploadImage lui = null;
                CommonDeleget.WriteLogAction($@"FilePath：{FilePath}，Carrid：{scanImageCache.Carrid}", LogType.NormalLog, false);
                FileInfo FileItem = new FileInfo(FilePath);
                CommonDeleget.WriteLogAction($@"FileItem FilePath：{FilePath}，Carrid：{scanImageCache.Carrid}", LogType.NormalLog, false);
                if (FileItem.Name.Contains("Correction")) return;
                if (!FileItem.Name.ToUpper().Contains("_C")) return;
                string FileName = Path.GetFileNameWithoutExtension(FileItem.Name);
                string FileExtention = Path.GetExtension(FileItem.Name);
                string FileFullName = FileItem.FullName;
                int ImageCount = 1;
                IntPtr Tempptr = IntPtr.Zero;
                //此处做图像分隔
                try
                {
                    //如果需要分割再调用
                    if(ConfigServices.GetInstance().localConfigModel.IsPartition)
                    {
                        //CommonDeleget.WriteLogAction($@"Realtime Process Memory ImageImportDll.IX_Open({FilePath}){CommonFunc.GetApplicationMemory()}", LogType.NormalLog, false);
                        Tempptr = ImageImportDll.IX_Open(FilePath);
                        ImageCount = ImageImportDll.IX_Partition(Tempptr, FilePath);
                        ImageImportDll.IX_Close(Tempptr);
                        //CommonDeleget.WriteLogAction($@"Realtime Process Memory ImageImportDll.IX_Close({Tempptr}){CommonFunc.GetApplicationMemory()}", LogType.NormalLog, false);
                    }
                }
                catch (Exception e)
                {
                    ImageImportDll.IX_Close(Tempptr);
                    CommonDeleget.WriteLogAction(e.InnerException.Message, LogType.ImageImportDllError, true);
                    CommonDeleget.WriteLogAction(e.StackTrace, LogType.ImageImportDllError, true);
                }
                if (ImageCount <= 1)
                {
                    GetThumbImageAndImage(FilePath, FileFullName, ref LocalUploadImageList, ref lui, scanImageCache);
                }
                else
                {
                    for (int i = 1; i < ImageCount + 1; i++)
                    {
                        FilePath = $@"{FileItem.Directory.FullName}\{FileName}_{i}{FileExtention}";
                        GetThumbImageAndImage(FilePath, FileFullName, ref LocalUploadImageList, ref lui, scanImageCache);
                    }
                }
                GetDataFromRealTime(lui,true);
                var Result = UploadLocalImageToOnline<LocalUploadImage>.GetInstance().UpLoadImageOnline(LocalUploadImageList, ref rm);
                if (Result)
                {
                    lui.IAMGE_LOCALFILE_CREATETIME = DateTime.Now.ToString("yyyyMMddHHmmss");
                    GetDataFromRealTime(lui,false);
                }
                GetDataFromLocalDataBase();
                if (scanImageCache.status == 0)
                {
                    rm = null;
                }
            }
            catch (Exception ex)
            {
                CommonDeleget.WriteLogAction(ex.StackTrace, LogType.ApplicationError);
            }
        }
        /// <summary>
        /// 获取缩略图和图片
        /// </summary>
        /// <param name="FilePath">本地文件路径</param>
        /// <param name="FileName">文件名称不含后缀</param>
        /// <param name="FileFullName">文件全称</param>
        /// <param name="LocalUploadImageList">传进来的待生成的图片列表</param>
        /// <param name="ImageIndex">分隔的当前图片序号</param>
        /// <param name="lui">生成的图片对象</param>
        /// <returns></returns>
        private bool GetThumbImageAndImage(string FilePath, string FileFullName, ref List<LocalUploadImage> LocalUploadImageList, ref LocalUploadImage lui, ScanImageCache CarrageScan)
        {
            FileInfo FileItem;
            string FileName = Path.GetFileNameWithoutExtension(FilePath);
            FileItem = new FileInfo(FilePath);
            lui = new LocalUploadImage();
            lui.IMAGE_ID = CommonFunc.ObjectToJson(CarrageScan);
            //矫正后的raw的图片路径
            lui.IMAGE_NAME = $"{FileName}";
            lui.IMAGE_FILE_PATH = $"{FilePath}";
            //原始图raw,没有矫正后的
            lui.IMAGESOURCE_NAME = $"{FileName.Split('_')[0]}";
            lui.IMAGESOURCEFILE_PATH = $@"{FileItem.DirectoryName}\{lui.IMAGESOURCE_NAME}{FileItem.Extension}";
            lui.IAMGE_LOCALFILE_CREATETIME = FileItem.LastWriteTime.ToString("yyyyMMddHHmmss");
            //生成缩略图
            var ThumbNailImage = $@"{FileItem.DirectoryName}\{FileName}.jpg";
            //生成缩略图
            bool TResult = ImageImportDll.OpenImage(FilePath, ThumbNailImage, true);
            if (TResult)
            {
                lui.IMAGE_THUMBNAIL = ThumbNailImage;
            }
            //生成tiff图
            var TifImage = $@"{FileItem.DirectoryName}\{FileName}.tif";
            bool TiffResult = ImageImportDll.OpenImage(FilePath, TifImage, false);
            if (TiffResult)
            {
                lui.IMAGE_TIFIMAGE = TifImage;
            }
            var HEMDImage = $@"{FileItem.DirectoryName}\{FileName}.png";
            bool HemdImageResult = ImageImportDll.OpenImage(FilePath, HEMDImage, true, true);
            if (HemdImageResult)
            {
                lui.IMAGE_HEMDIMAGE = HEMDImage;
            }
            if (TList.Any(q => q.IMAGE_NAME.Contains($"{FileName}")))
            {
                return true;
            }
            LocalUploadImageList.Add(lui);
            return false;
        }

        /// <summary>
        /// 初始化自定义上传ListView
        /// </summary>
        public void InitGridContent()
        {
            Debug.WriteLine("-------------------------ganggang_upload_img7-----------------------");
            MakeLiv(TList, ref lv);
            MakeLiv(HistoryList, ref historyLv);
            DockPanel sp = new DockPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                LastChildFill = true
            };
            lb = new Label()
            {
                Style = (Style)this.TryFindResource("LinkLabel"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(0, 0, 30, 0),
                Content = UpdateStatusNameAction("More"),
                FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.NormalMiddle),
            };
            lb.PreviewMouseDown += Lb_PreviewMouseDown;
            DockPanel.SetDock(historyLv, Dock.Bottom);
            DockPanel.SetDock(lb, Dock.Top);
            sp.Children.Add(lb);
            sp.Children.Add(historyLv);
            MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(5, GridUnitType.Pixel) });
            MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            GridSplitter gs = new GridSplitter() { Height = 5, HorizontalAlignment = HorizontalAlignment.Stretch, Background = CommonFunc.StrToBrush("#00000000") };

            Grid.SetColumn(lv, 0); Grid.SetColumn(sp, 0); Grid.SetColumn(gs, 0);
            Grid.SetRow(lv, 0); Grid.SetRow(sp, 2); Grid.SetRow(gs, 1);
            MainGrid.Children.Add(lv);
            MainGrid.Children.Add(sp);
            MainGrid.Children.Add(gs);
        }

        private void Lb_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CommonDeleget.ShowModuleEvent("Modulesvehicle");
        }

        public void MakeLiv(ObservableCollection<LocalUploadImage> IList, ref ListView lvPara)
        {
            lvPara = new ListView()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(0, 0, 30, 0),
                Background = CommonFunc.StrToBrush("#00345c"),
                ItemContainerStyle = (Style)this.TryFindResource("diyUploadImageList"),
            };
            lvPara.ItemsSource = IList;
            GridView gv = new GridView() { AllowsColumnReorder = false, };
        }

        public void Reflash(ObservableCollection<LocalUploadImage> IList)
        {
            TList = IList;
            lv.ItemsSource = TList;
            lv.DataContext = TList;
        } 
        public void ReflashHistoryList(ObservableCollection<LocalUploadImage> HList)
        {
            HistoryList = HList;

            if (historyLv != null)
            {
                historyLv.ItemsSource = HistoryList;
            }
        }

        public void HandScanImageCacheThread()
        {
            ScanImageCache scanImageCache;
            while (true)
            {
                Thread.Sleep(20);
                while (ScanImageCacheQueue.TryDequeue(out scanImageCache))
                {
                    CommonDeleget.WriteLogAction($@"HandScanImageCacheThread，scanImageCache.Path:{scanImageCache.ImagePath}", LogType.NormalLog, false);
                    UploadImage(scanImageCache);
                }
            }
        }

        public struct ScanImageCache
        {
            /// <summary>
            /// 0最有一张图；1不是最后一张图
            /// </summary>
            public int status;
            /// <summary>
            /// 车厢序号
            /// </summary>
            public long Carrid;
            /// <summary>
            /// 扫描图片路径
            /// </summary>
            public string ImagePath;
        }
    }
}
