using BG_Entities;
using BG_WorkFlow;
using BGDAL;
using BGLogs;
using BGModel;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BG_Services
{
    public class UploadLocalImageToOnlineForBS<T> : BaseInstance<UploadLocalImageToOnlineForBS<T>> where T : class, new()
    {
        public Action<LocalUploadImage,bool> UpdateRealTimeListViewDataAction;
        public Action UpdateHistoryListViewDataAction;

        LocalUploadImageBLL llb = new LocalUploadImageBLL();
        public UploadLocalImageToOnlineForBS()
        {
            llb.InsertImageEvent += InsertAfterEvent;
        }

        public bool UpLoadImageOnline(List<string> BSFiles, string MainBSFiles, ref RecvMessage rm)
        {
            List<LocalUploadImage> LocalUploadImageList = new List<LocalUploadImage>();
            if (File.Exists(MainBSFiles))
            {
                LocalUploadImage localUploadImage = RawImageToThm(MainBSFiles, "scanBSMain");
                LocalUploadImageList.Add(localUploadImage);
                UpdateRealTimeListViewDataAction(localUploadImage,true);
            }
            foreach (var childrenBSFile in BSFiles)
            {
                if (childrenBSFile == MainBSFiles) continue;
                if (File.Exists(childrenBSFile))
                {
                    LocalUploadImage localUploadImage = RawImageToThm(childrenBSFile, $@"scanBS{BSFiles.IndexOf(childrenBSFile) + 1}");
                    LocalUploadImageList.Add(localUploadImage);
                }
            }
            bool onlineRet = UpLoadImageOnline(LocalUploadImageList, ref rm);
            return onlineRet;
        }
        public LocalUploadImage RawImageToThm(string FilePath, string FileSource)
        {
            FileInfo FileItem = new FileInfo(FilePath);
            string FileName = Path.GetFileNameWithoutExtension(FilePath);
            LocalUploadImage localUploadImage = new LocalUploadImage();
            localUploadImage.IMAGESOURCEFILE_PATH = FilePath;
            localUploadImage.IMAGESOURCE_NAME = FileItem.Name;
            localUploadImage.IMAGE_FILE_PATH = FilePath;
            localUploadImage.IMAGE_NAME = FileItem.Name;
            localUploadImage.IMAGE_CLIENT_SOURCE = FileSource;
            //生成缩略图
            var ThumbNailImage = $@"{FileItem.DirectoryName}\{FileName}.jpg";
            //生成缩略图
            bool TResult = ImageImportDll.OpenImage(FilePath, ThumbNailImage, true);
            if (TResult)
            {
                localUploadImage.IMAGE_THUMBNAIL = ThumbNailImage;
            }
            //生成tiff图
            var TifImage = $@"{FileItem.DirectoryName}\{FileName}.tif";
            bool TiffResult = ImageImportDll.OpenImage(FilePath, TifImage, false);
            if (TiffResult)
            {
                localUploadImage.IMAGE_TIFIMAGE = TifImage;
            }
            return localUploadImage;
        }
        /// <summary>
        /// 集中审像上传图片方法
        /// </summary>
        /// <param name="lui"></param>
        /// <returns></returns>
        public bool UpLoadImageOnline(List<LocalUploadImage> t, ref RecvMessage rm)
        {
            Debug.WriteLine("-------------------------ganggang_upload_img27-----------------------");
            if (t == null) return false;
            bool isUploadImage = false;
            List<LocalUploadImage> LocalUploadImageList = t as List<LocalUploadImage>;
            LocalUploadImageList.ForEach(q => { q.IMAGE_UPLOAD_STATUS = ImageUploadStatus.GetTaskIDing; });
            try
            {
                if (rm == null)
                {
                    rm = GetTaskIdAndPutFTP();
                }
                if (rm.Code == "1")
                {
                    //3代表以及推送至FTP
                    LocalUploadImageList.ForEach(q => { q.IMAGE_UPLOAD_STATUS = ImageUploadStatus.WaitingPutFTPing; });
                    SendImageToFtp(rm, LocalUploadImageList);
                    //正在上传图SendImageToFtp片信息
                    Debug.WriteLine("-------------------------ganggang_upload_img28-----------------------");
                    LocalUploadImageList.ForEach(q => { q.IMAGE_UPLOAD_STATUS = ImageUploadStatus.UpSubmitImageInfoingLoading; });
                    isUploadImage = UploadImage(rm, LocalUploadImageList);
                    LocalUploadImageList.ForEach(q => { q.IMAGE_UPLOAD_STATUS = ImageUploadStatus.UploadComplete; });
                }
                else
                {
                    Log.GetDistance().WriteInfoLogs(rm?.Message);
                }
            }

            catch (Exception ex)
            {
                llb.Insert(LocalUploadImageList);
              
                UpdateRealTimeListViewDataAction(LocalUploadImageList[0],false);
                UploadWebException(LocalUploadImageList.First());
                //LocalUploadImageList.ForEach(q => { q.IMAGE_UPLOAD_STATUS = ImageUploadStatus.CommitFaild; });
                Log.GetDistance().WriteInfoLogs(ex.Message);
                throw ex;
            }
            if (isUploadImage)
            {
                //5代表全流程完成
                LocalUploadImageList.ForEach(q => { q.IMAGE_UPLOAD_STATUS = ImageUploadStatus.UploadComplete; });
                llb.Insert(LocalUploadImageList);
                
                UpdateRealTimeListViewDataAction(LocalUploadImageList[0], false);
            }
            if (!isUploadImage) LocalUploadImageList.ForEach(q => q.IMAGE_UPLOAD_STATUS = ImageUploadStatus.CommitFaild);
            return isUploadImage;
        }
        private bool UploadImage(RecvMessage _rm, List<LocalUploadImage> luiList)
        {
            bool isUploadImage = false;
            try
            {
                TaskInfo ti = CommonFunc.JsonToObject<TaskInfo>(CommonFunc.AesDecrypt(_rm.Data, ConfigServices.GetInstance().localConfigModel.IsAES));
                List<PathInfo> files = new List<PathInfo>();
                foreach (var lui in luiList)
                {
                    string FileName = Path.GetFileName(lui.IMAGE_FILE_PATH);
                    string ThumbnailFileName = lui.IMAGE_THUMBNAIL == null ? string.Empty : Path.GetFileName(lui.IMAGE_THUMBNAIL);
                    string TifImageFileName = lui.IMAGE_TIFIMAGE == null ? string.Empty : Path.GetFileName(lui.IMAGE_TIFIMAGE);
                    string SourceImageFileName = Path.GetFileName(lui.IMAGESOURCEFILE_PATH);
                    string FileExtention = Path.GetExtension(lui.IMAGE_FILE_PATH).TrimStart('.');
                    string ThumbnailFileExtention = lui.IMAGE_THUMBNAIL == null ? string.Empty : Path.GetExtension(lui.IMAGE_THUMBNAIL).TrimStart('.');
                    string TifImageExtention = lui.IMAGE_TIFIMAGE == null ? string.Empty : Path.GetExtension(lui.IMAGE_TIFIMAGE).TrimStart('.');
                    PathInfo pathInfo = new PathInfo()
                    {
                        fileType = GetFileAccrodingFileExting(FileExtention),
                        path = string.Format($@"{ti?.FileInfos}\{FileName}"),
                        ViewNo = (luiList.IndexOf(lui) + 1).ToString(),
                        pictureNo = "1",
                    };
                    PathInfo thumbnailPathInfo = new PathInfo()
                    {
                        fileType = GetFileAccrodingFileExting(ThumbnailFileExtention),
                        path = string.Format($@"{ti?.FileInfos}\{ThumbnailFileName}"),
                        ViewNo = (luiList.IndexOf(lui) + 1).ToString(),
                        pictureNo = "1",
                    };
                    PathInfo tifImagePathInfo = new PathInfo()
                    {
                        fileType = GetFileAccrodingFileExting(TifImageExtention),
                        path = string.Format($@"{ti?.FileInfos}\{TifImageFileName}"),
                        ViewNo = (luiList.IndexOf(lui) + 1).ToString(),
                        pictureNo = "1",
                    };
              
                    PathInfo sourceImagePathInfo = new PathInfo()
                    {
                        fileType = lui.IMAGE_CLIENT_SOURCE,
                        path = string.Format($@"{ti?.FileInfos}\{SourceImageFileName}"),
                        ViewNo = (luiList.IndexOf(lui) + 1).ToString(),
                        pictureNo = "1",
                    };
                    files.Add(pathInfo);
                    files.Add(thumbnailPathInfo);
                    files.Add(tifImagePathInfo);
                    files.Add(sourceImagePathInfo);
                }
                RequestModel sri = new RequestModel();
                sri.Token = ConfigServices.GetInstance().localConfigModel.Login.sccessToken;
                sri.OrderType = UploadImageOrderType.SubmitScanBSData;
                sri.Data = CommonFunc.AesEncrypt(CommonFunc.ObjectToJson(new SubmitCheckInDataInfo() { TaskId = ti?.TaskId, Files = files, ImageInfo = luiList.First().IMAGE_ID }), ConfigServices.GetInstance().localConfigModel.IsAES);
                RecvMessage rm = UploadImageToWeb(sri);
                if (rm.Code == "1")
                {
                    luiList.ForEach(q => q.IMAGE_UPDATETIME = DateTime.Now.ToString("yyyyMMddHHmmss"));
                    luiList.ForEach(q => q.IMAGE_UPLOAD_STATUS = ImageUploadStatus.UpSubmitImageInfoingLoading);
                    isUploadImage = true;
                }
                else
                {
                    Log.GetDistance().WriteInfoLogs(rm?.Message);
                    luiList.ForEach(q => q.IMAGE_UPLOAD_STATUS = ImageUploadStatus.CommitImageInfoFaild);
                    isUploadImage = false;
                }
            }
            catch
            {
                luiList.ForEach(q => q.IMAGE_UPLOAD_STATUS = ImageUploadStatus.CommitImageInfoFaild);
                llb.Insert(luiList);
                UpdateRealTimeListViewDataAction(luiList[0], false);
                throw;
            }

            return isUploadImage;
        }
        /// <summary>
        /// 通过文件后缀获取上传图片类型
        /// </summary>
        /// <param name="FileExtention"></param>
        /// <returns></returns>
        public string GetFileAccrodingFileExting(string FileExtention)
        {
            Debug.WriteLine("-------------------------ganggang_upload_img26-----------------------");
            switch (FileExtention)
            {
                case "jpg":
                    return UploadImageType.scanJpgBS;
                case "png":
                    return UploadImageType.scanHemdBS;
                case "raw":
                    return UploadImageType.scanbgpBS;
                case "tif":
                    return UploadImageType.scanBS;
                default:
                    return string.Empty;
            }
        }
        /// <summary>
        /// 把图片发送到FTP
        /// </summary>
        /// <param name="_rm"></param>
        /// <param name="lui"></param>
        /// <returns></returns>
        private bool SendImageToFtp(RecvMessage _rm, List<LocalUploadImage> luiList)
        {
            bool isResult = true;
            string SourceData = _rm.Data;
            TaskInfo ti = CommonFunc.JsonToObject<TaskInfo>(CommonFunc.AesDecrypt(SourceData, ConfigServices.GetInstance().localConfigModel.IsAES));
            luiList.ForEach(q => { q.IMAGE_TASKID = ti?.TaskId;});
            foreach (var lui in luiList)
            {
                try
                {
                    //矫正图
                    string FileNameExtention = Path.GetFileName(lui.IMAGE_FILE_PATH);
                    //缩略图
                    string ImageThumbnailExtention = Path.GetFileName(lui.IMAGE_THUMBNAIL);
                    //TIF图
                    string ImageTifExtention = Path.GetFileName(lui.IMAGE_TIFIMAGE);
                    //原始图
                    string ImageSourceExtention = Path.GetFileName(lui.IMAGESOURCEFILE_PATH);
                    lui.IMAGE_TASKID = ti.TaskId;
                    string FileName = string.Format($@"{ti.FileInfos}\{FileNameExtention}");
                    string ThumbnailFileName = string.Format($@"{ti.FileInfos}\{ImageThumbnailExtention}");
                    string TifFileName = string.Format($@"{ti.FileInfos}\{ImageTifExtention}");
                    CommonDeleget.WriteLogAction($"BSFTP--------File      FileName--------BS: {FileName}", LogType.NormalLog);
                    CommonDeleget.WriteLogAction($"BSFTP--------Thumbnail FileName--------BS: {ThumbnailFileName}", LogType.NormalLog);
                    CommonDeleget.WriteLogAction($"BSFTP--------TIFImage  FileName--------BS: {TifFileName}", LogType.NormalLog);

                    Parallel.Invoke(
                        () => { Common._ftpHelper?.Upload(lui.IMAGE_FILE_PATH, FileName); },
                        () => { Common._ftpHelper?.Upload(lui.IMAGE_THUMBNAIL, ThumbnailFileName); },
                        () => { Common._ftpHelper?.Upload(lui.IMAGE_TIFIMAGE, TifFileName); });
                    DeleteLocalImage(lui);
                    CommonDeleget.WriteLogAction($"BSFTP--------Success--------BS", LogType.NormalLog);
                }
                catch (Exception ex)
                {
                    lui.IMAGE_UPLOAD_STATUS = ImageUploadStatus.PutFTPFaild;
                    CommonDeleget.WriteLogAction($@"BSFTP--------Success--------BS Error:{ex.Message},ImageUploadStatus.PutFTPFaild", LogType.ApplicationError);
                    throw ex;
                }
            }
            return isResult;
        }
        /// <summary>
        /// 通过接口获取任务ID
        /// </summary>
        /// <param name="ihp"></param>
        /// <returns></returns>
        public RecvMessage GetTaskIdAndPutFTP()
        {
            RequestModel ihp = new RequestModel();
            ihp.Token = ConfigServices.GetInstance().localConfigModel.Login.sccessToken;
            ihp.OrderType = UploadImageOrderType.GetTaskIdByDevice;
            ihp.Data = CommonFunc.AesEncrypt(@"{""DeviceNo"":""" + ConfigServices.GetInstance().localConfigModel.EquipmentNo + @"""}", ConfigServices.GetInstance().localConfigModel.IsAES);
            string PostData = CommonFunc.ObjectToJson<RequestModel>(ihp);
            CommonDeleget.WriteLogAction($"BSTASK--------GET    TASKID: {PostData}", LogType.NormalLog);
            var ResposeStr = UploadImageServices.GetInstance().GetTaskIdFromWeb(PostData);
            CommonDeleget.WriteLogAction($"BSTASK--------RETURN TASKID : {ResposeStr}", LogType.NormalLog);
            RecvMessage rm = CommonFunc.JsonToObject<RecvMessage>(ResposeStr);
            return rm;
        }
        /// <summary>
        /// 获取ID之后把图片上传
        /// </summary>
        /// <param name="ihp"></param>
        /// <returns></returns>
        private RecvMessage UploadImageToWeb(RequestModel ihp)
        {
            Debug.WriteLine("-------------------------ganggang_upload_img25-----------------------");
            string PostData = CommonFunc.ObjectToJson<RequestModel>(ihp);
            CommonDeleget.WriteLogAction($"BSIMAGE_TO_ONLINE--------BEFORE DATA: {PostData}", LogType.NormalLog);
            string ResposeStr = "Init";
            RecvMessage rm;
            if (ConfigServices.GetInstance().localConfigModel.IsEnabledSocketToServer)
            {
                rm = new RecvMessage() { Code = CommonDeleget.UploadImageToWebBySocket(PostData) ? "1" : "0" };
            }
            else
            {
                ResposeStr = UploadImageServices.GetInstance().UploadImageToWeb(PostData);
                rm = CommonFunc.JsonToObject<RecvMessage>(ResposeStr);
            }
            CommonDeleget.WriteLogAction($"BSIMAGE_TO_ONLINE--------AFTER DATA: {ResposeStr}", LogType.NormalLog);
            return rm;
        }
        /// <summary>
        /// 将图片信息异常情况上传给后台
        /// </summary>
        /// <param name="ihp"></param>
        /// <returns></returns>
        private async void UploadWebException(LocalUploadImage ImageFiaid)
        {
            await Task.Run(() =>
            {
                try
                {
                    Debug.WriteLine("-------------------------ganggang_upload_img24-----------------------");
                    RequestModel sri = new RequestModel();
                    sri.OrderType = UploadImageOrderType.SaveScanExceptionMessage; sri.Token = ConfigServices.GetInstance().localConfigModel.Login.sccessToken;
                    sri.Data = CommonFunc.AesEncrypt(CommonFunc.ObjectToJson(new WebExceptionModel() { TaskId = ImageFiaid?.IMAGE_TASKID, Message = ImageFiaid.IMAGE_UPLOAD_STATUS }), ConfigServices.GetInstance().localConfigModel.IsAES);
                    string PostData = CommonFunc.ObjectToJson<RequestModel>(sri);
                    CommonDeleget.WriteLogAction($"BSIMAGE_TO_ONLINE--------ERROR BEFORE: {PostData}", LogType.NormalLog);
                    var ResposeStr = UploadImageServices.GetInstance().UploadImageToWeb(PostData);
                    CommonDeleget.WriteLogAction($"BSIMAGE_TO_ONLINE--------ERROR AFTER: {ResposeStr}", LogType.NormalLog);
                    RecvMessage rm = CommonFunc.JsonToObject<RecvMessage>(ResposeStr);
                    //return rm;
                }
                catch (Exception ex)
                {
                    CommonDeleget.WriteLogAction(ex.Message, LogType.ApplicationError, false);
                }
            });
        }
        /// <summary>
        /// 删除本地图片
        /// </summary>
        /// <param name="localUploadImage"></param>
        private void DeleteLocalImage(LocalUploadImage localUploadImage)
        {
            if (ConfigServices.GetInstance().localConfigModel.IsSaveImage)
            {
                return;
            }
            try
            {
                if (File.Exists(localUploadImage.IMAGE_FILE_PATH))
                {
                    File.Delete(localUploadImage.IMAGE_FILE_PATH);
                }
                if (File.Exists(localUploadImage.IMAGE_THUMBNAIL))
                {
                    File.Delete(localUploadImage.IMAGE_THUMBNAIL);
                }
                if (File.Exists(localUploadImage.IMAGE_TIFIMAGE))
                {
                    File.Delete(localUploadImage.IMAGE_TIFIMAGE);
                }
                if (File.Exists(localUploadImage.IMAGESOURCEFILE_PATH))
                {
                    File.Delete(localUploadImage.IMAGESOURCEFILE_PATH);
                }
                if (File.Exists(localUploadImage.IMAGE_HEMDIMAGE))
                {
                    File.Delete(localUploadImage.IMAGE_HEMDIMAGE);
                }
                string Temp_CFile = $@"{Path.GetFileNameWithoutExtension(localUploadImage.IMAGESOURCEFILE_PATH)}_C.raw";
                string Temp_CDir = $@"{Path.GetDirectoryName(localUploadImage.IMAGESOURCEFILE_PATH)}\{Temp_CFile}";
                if (File.Exists(Temp_CDir))
                {
                    File.Delete(Temp_CDir);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"DeleteLocalImage Exception {ex.StackTrace}");
            }
        }

        /// <summary>
        /// 插入数据库之后，需要刷新数据列表
        /// </summary>
        private void InsertAfterEvent()
        {
            UpdateHistoryListViewDataAction();
        }
    }
}
