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
    public class UploadLocalImageToOnline<T> : BaseInstance<UploadLocalImageToOnline<T>> where T : class, new()
    {
        LocalUploadImageBLL llb = new LocalUploadImageBLL();
    
        public bool UpLoadImageOnline(List<T> t, ref RecvMessage rm)
        {
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
                    //正在上传图片信息
                    Debug.WriteLine("-------------------------ganggang_upload_img22-----------------------");
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
                    string ThumbnailFileName = lui.IMAGE_THUMBNAIL==null?string.Empty: Path.GetFileName(lui.IMAGE_THUMBNAIL);
                    string TifImageFileName = lui.IMAGE_TIFIMAGE == null ? string.Empty : Path.GetFileName(lui.IMAGE_TIFIMAGE);
                    string SourceImageFileName = Path.GetFileName(lui.IMAGESOURCEFILE_PATH);
                    string HemdImageFileName = lui.IMAGE_HEMDIMAGE == null?string.Empty : Path.GetFileName(lui.IMAGE_HEMDIMAGE);
                    string FileExtention = Path.GetExtension(lui.IMAGE_FILE_PATH).TrimStart('.');
                    string ThumbnailFileExtention = lui.IMAGE_THUMBNAIL == null ? string.Empty : Path.GetExtension(lui.IMAGE_THUMBNAIL).TrimStart('.');
                    string TifImageExtention = lui.IMAGE_TIFIMAGE == null ? string.Empty : Path.GetExtension(lui.IMAGE_TIFIMAGE).TrimStart('.');
                    string HemdImageExtention = lui.IMAGE_HEMDIMAGE == null ? string.Empty : Path.GetExtension(lui.IMAGE_HEMDIMAGE).TrimStart('.');
                    PathInfo pathInfo = new PathInfo()
                    {
                        fileType = GetFileAccrodingFileExting(FileExtention),
                        path = string.Format($@"{ti?.FileInfos}\{FileName}"),
                        pictureNo = (luiList.IndexOf(lui) + 1) .ToString()
                    };
                    PathInfo thumbnailPathInfo = new PathInfo()
                    {
                        fileType = GetFileAccrodingFileExting(ThumbnailFileExtention),
                        path = string.Format($@"{ti?.FileInfos}\{ThumbnailFileName}"),
                        pictureNo = (luiList.IndexOf(lui) + 1).ToString()
                    };
                    PathInfo tifImagePathInfo = new PathInfo()
                    {
                        fileType = GetFileAccrodingFileExting(TifImageExtention),
                        path = string.Format($@"{ti?.FileInfos}\{TifImageFileName}"),
                        pictureNo = (luiList.IndexOf(lui) + 1).ToString()
                    };
                    if (!string.IsNullOrEmpty(HemdImageFileName))
                    {
                        PathInfo hemdImagePathInfo = new PathInfo()
                        {
                            fileType = GetFileAccrodingFileExting(HemdImageExtention),
                            path = string.Format($@"{ti?.FileInfos}\{HemdImageFileName}"),
                            pictureNo = (luiList.IndexOf(lui) + 1).ToString()
                        };
                        files.Add(hemdImagePathInfo);
                    }
                    PathInfo sourceImagePathInfo = new PathInfo()
                    {
                        fileType = "SourceRaw",
                        path = string.Format($@"{ti?.FileInfos}\{SourceImageFileName}"),
                        pictureNo = (luiList.IndexOf(lui) + 1).ToString()
                    };
                    files.Add(pathInfo);
                    files.Add(thumbnailPathInfo);
                    files.Add(tifImagePathInfo);
                    files.Add(sourceImagePathInfo);
                }
                RequestModel sri = new RequestModel();
                sri.Token = ConfigServices.GetInstance().localConfigModel.Login.sccessToken;
                sri.OrderType = UploadImageOrderType.SubmitScanData;
                sri.Data = CommonFunc.AesEncrypt(CommonFunc.ObjectToJson(new SubmitCheckInDataInfo() { TaskId = ti?.TaskId, Files = files,ImageInfo = luiList.First().IMAGE_ID }), ConfigServices.GetInstance().localConfigModel.IsAES);
                RecvMessage rm = UploadImageToWeb(sri);
                if (rm.Code == "1")
                {
                    luiList.ForEach(q=>q.IMAGE_UPDATETIME = DateTime.Now.ToString("yyyyMMddHHmmss"));
                    luiList.ForEach(q => q.IMAGE_UPLOAD_STATUS = ImageUploadStatus.UpSubmitImageInfoingLoading);
                    isUploadImage = true;
                }
                else
                {
                    Log.GetDistance().WriteInfoLogs(rm?.Message);
                    luiList.ForEach(q => q.IMAGE_UPLOAD_STATUS = ImageUploadStatus.CommitImageInfoFaild);
                    //lui.IMAGE_UPLOAD_STATUS = ImageUploadStatus.CommitImageInfoFaild;
                    isUploadImage = false;
                }
                if (Common.controlVersion == ControlVersion.Car)
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            Thread.Sleep(3000);
                            sri.OrderType = UploadImageOrderType.ModifyRadiationPassData;
                            sri.Data = CommonFunc.AesEncrypt(CommonFunc.ObjectToJson(new RPMModel() { taskid = ti?.TaskId, passState = Common.RPMDataToWebData() }), ConfigServices.GetInstance().localConfigModel.IsAES);
                            RecvMessage RpmRm = UploadRPMToWeb(sri);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    });
                }
            }
            catch
            {
                luiList.ForEach(q => q.IMAGE_UPLOAD_STATUS = ImageUploadStatus.CommitImageInfoFaild);
                llb.Insert(luiList);
                //lui.IMAGE_UPLOAD_STATUS = ImageUploadStatus.CommitImageInfoFaild;
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
            Debug.WriteLine("-------------------------ganggang_upload_img20-----------------------");
            switch (FileExtention)
            {
                case "jpg":
                    return UploadImageType.scanJpg;
                case "png":
                    return UploadImageType.scanHemd;
                case "raw":
                    return UploadImageType.scanbgp;  
                case "tif":
                    return UploadImageType.scan;
                default:
                    return string.Empty;
            }
        }
        /// <summary>
        /// 集中审像上传图片方法
        /// </summary>
        /// <param name="lui"></param>
        /// <returns></returns>
    
        /// <summary>
        /// 把图片发送到FTP
        /// </summary>
        /// <param name="_rm"></param>
        /// <param name="lui"></param>
        /// <returns></returns>
        private bool SendImageToFtp(RecvMessage _rm, List<LocalUploadImage> luiList)
        {
            Debug.WriteLine("-------------------------ganggang_upload_img21-----------------------");
            bool isResult = true;
            string SourceData = _rm.Data;
            TaskInfo ti = CommonFunc.JsonToObject<TaskInfo>(CommonFunc.AesDecrypt(SourceData, ConfigServices.GetInstance().localConfigModel.IsAES));
            luiList.ForEach(q => { q.IMAGE_TASKID = ti?.TaskId; });
            foreach (var lui in luiList)
            {
                try
                {
                    //throw new Exception("test ");
                    //矫正图
                    string FileNameExtention = Path.GetFileName(lui.IMAGE_FILE_PATH);
                    //缩略图
                    string ImageThumbnailExtention = Path.GetFileName(lui.IMAGE_THUMBNAIL);
                    //TIF图
                    string ImageTifExtention = Path.GetFileName(lui.IMAGE_TIFIMAGE);
                    //HEMD图
                    string ImageHemdExtention = lui.IMAGE_HEMDIMAGE == null ?string.Empty:Path.GetFileName(lui.IMAGE_HEMDIMAGE);
                    //原始图
                    string ImageSourceExtention = Path.GetFileName(lui.IMAGESOURCEFILE_PATH);
                    lui.IMAGE_TASKID = ti.TaskId;
                    string FileName = string.Format($@"{ti.FileInfos}\{FileNameExtention}");
                    string ThumbnailFileName = string.Format($@"{ti.FileInfos}\{ImageThumbnailExtention}");
                    string TifFileName = string.Format($@"{ti.FileInfos}\{ImageTifExtention}");
                    string HemdFileName = string.Format($@"{ti.FileInfos}\{ImageHemdExtention}");
                    string SourceFileName = string.Format($@"{ti.FileInfos}\{ImageSourceExtention}");
                    CommonDeleget.WriteLogAction($"Upload FTP Correct File FileName: {FileName} \n", LogType.NormalLog);
                    CommonDeleget.WriteLogAction($"Upload FTP Thumbnail FileName: {ThumbnailFileName} \n", LogType.NormalLog);
                    CommonDeleget.WriteLogAction($"Upload FTP TIF Image FileName: {TifFileName} \n", LogType.NormalLog);
                    CommonDeleget.WriteLogAction($"Upload FTP Hemd FileName: {HemdFileName} \n", LogType.NormalLog);
                    CommonDeleget.WriteLogAction($"Upload FTP Source  FileName: {SourceFileName} \n", LogType.NormalLog);
                      
                    Task[] TaskList = new Task[5];
                    bool test1 = false; bool test2 = false; bool test3 = false; bool test4 = false;bool test5 = false;
                    TaskList[0] = Task.Factory.StartNew(() =>{
                        test1 = true; Common._ftpHelper?.Upload(lui.IMAGE_FILE_PATH, FileName);
                    }); //上传扫描图
                    Debug.WriteLine("-------------------------ganggang_upload_img23-----------------------");
                    TaskList[1] = Task.Factory.StartNew(() => { test2 = true; Common._ftpHelper?.Upload(lui.IMAGE_THUMBNAIL, ThumbnailFileName);  }); //上传缩略图
                    TaskList[2] = Task.Factory.StartNew(() => { test3 = true; Common._ftpHelper?.Upload(lui.IMAGE_TIFIMAGE, TifFileName); }); //上传TIF图
                    TaskList[3] = Task.Factory.StartNew(() => { test4 = true; Common._ftpHelper?.Upload(lui.IMAGESOURCEFILE_PATH, SourceFileName);  }); //上传原始图
                    if(!string.IsNullOrEmpty(ImageHemdExtention))
                    {
                        TaskList[4] = Task.Factory.StartNew(() => { test4 = true; Common._ftpHelper?.Upload(lui.IMAGE_HEMDIMAGE, HemdFileName); }); //上传高能物质识别图
                    }
                    else
                    {
                        TaskList[4] = Task.Factory.StartNew(() => { test4 = true;}); //上传高能物质识别图
                    }
                    Task.WhenAll(TaskList).Wait();
                    DeleteLocalImage(lui);
                    CommonDeleget.WriteLogAction($"Upload FTP Success！", LogType.NormalLog);
                }
            
            catch (Exception ex)
            {
                lui.IMAGE_UPLOAD_STATUS = ImageUploadStatus.PutFTPFaild;
                CommonDeleget.WriteLogAction(ex.Message + "  ImageUploadStatus.PutFTPFaild", LogType.ApplicationError);
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
            CommonDeleget.WriteLogAction($"Get TaskID PostData: {PostData} \n",LogType.NormalLog);
            var ResposeStr = UploadImageServices.GetInstance().GetTaskIdFromWeb(PostData);
            CommonDeleget.WriteLogAction($"Receive gang TaskID PostData: {ResposeStr} \n", LogType.NormalLog);
            RecvMessage rm = CommonFunc.JsonToObject<RecvMessage>(ResposeStr);
            return rm;
        }
        /// <summary>
        /// 上传RPM数据
        /// </summary>
        /// <param name="ihp"></param>
        /// <returns></returns>
        private RecvMessage UploadRPMToWeb(RequestModel ihp)
        {
            Debug.WriteLine("-------------------------ganggang_upload_img17-----------------------");
            string PostData = CommonFunc.ObjectToJson<RequestModel>(ihp);
            CommonDeleget.WriteLogAction($"UploadRPMToWeb: {PostData} \n", LogType.NormalLog);
            var ResposeStr = UploadWebServiceControl.GetInstance().CreateWebServicesControl(RPMServices.GetInstance()).UploadData(PostData);
            CommonDeleget.WriteLogAction($"UploadRPMToWeb: {ResposeStr} \n", LogType.NormalLog);
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
            Debug.WriteLine("-------------------------ganggang_upload_img19-----------------------");
            string PostData = CommonFunc.ObjectToJson<RequestModel>(ihp);
            CommonDeleget.WriteLogAction($"Upload ImageFile FileName: {PostData} \n", LogType.NormalLog);
            string ResposeStr = "Init";
            RecvMessage rm;
            if (ConfigServices.GetInstance().localConfigModel.IsEnabledSocketToServer)
            {
                 rm = new RecvMessage() { Code = CommonDeleget.UploadImageToWebBySocket(PostData)?"1":"0" };
            }
            else
            {
                 ResposeStr = UploadImageServices.GetInstance().UploadImageToWeb(PostData);
                 rm = CommonFunc.JsonToObject<RecvMessage>(ResposeStr);
            }
            CommonDeleget.WriteLogAction($"Upload ImageFile FileName: {ResposeStr} \n", LogType.NormalLog);
            return rm;
        }
        /// <summary>
        /// 将图片信息异常情况上传给后台
        /// </summary>
        /// <param name="ihp"></param>
        /// <returns></returns>
        private async void UploadWebException(LocalUploadImage ImageFiaid)
        {
            
            await Task.Run(() => {
                try
                {
                    Debug.WriteLine("-------------------------ganggang_upload_img18-----------------------");
                    RequestModel sri = new RequestModel();
                    sri.OrderType = UploadImageOrderType.SaveScanExceptionMessage; sri.Token = ConfigServices.GetInstance().localConfigModel.Login.sccessToken;
                    sri.Data = CommonFunc.AesEncrypt(CommonFunc.ObjectToJson(new WebExceptionModel() { TaskId = ImageFiaid?.IMAGE_TASKID, Message = ImageFiaid.IMAGE_UPLOAD_STATUS }),ConfigServices.GetInstance().localConfigModel.IsAES);
                    string PostData = CommonFunc.ObjectToJson<RequestModel>(sri);
                    CommonDeleget.WriteLogAction($"UploadWebException FileName: {PostData} \n", LogType.NormalLog);
                    var ResposeStr = UploadImageServices.GetInstance().UploadImageToWeb(PostData);
                    CommonDeleget.WriteLogAction($"UploadWebException FileName: {ResposeStr} \n", LogType.NormalLog);
                    RecvMessage rm = CommonFunc.JsonToObject<RecvMessage>(ResposeStr);
                    //return rm;
                }
                catch (Exception ex)
                {
                    CommonDeleget.WriteLogAction(ex.Message,LogType.ApplicationError,false);
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
    }
}
