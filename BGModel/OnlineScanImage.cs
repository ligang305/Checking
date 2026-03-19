using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGModel
{
    [BaseAttribute(Name = "BG_IMAGE")]
    public class LocalUploadImage : BaseNotifyPropertyChanged
    {
        public double _FontSize ;
        public string _ImageId;
        public string _ImageName;
        public string _ImageSourceName;
        public string _ImageFilePath;
        public string _ImageSourceFilePath;
        public string _ImageUploadStatus;
        public string _TaskId;
        public string _Image_ClientSource;
        public string _ImageLocalFileCreateTime;
        public string _UpdateTime = string.Empty;
        public string _ImageThumbnail = string.Empty;
        public string _ImageTif = string.Empty;
        public string _ImageHEMD = string.Empty;

        [BaseAttribute(Name = "IMAGE_TASKID", Description = "图片关联的任务ID")]
        public string IMAGE_TASKID
        {
            get
            {
                return _TaskId;
            }
            set
            {
                _TaskId = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("IMAGE_TASKID"));
            }
        }
        [BaseAttribute(Name = "IMAGE_FONTSIZE", Description = "图像列表字体",IsInsertDB =false)]
        public double IMAGE_FONTSIZE
        {
            get
            {
                return _FontSize;
            }
            set
            {
                _FontSize = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("IMAGE_FONTSIZE"));
            }
        }

        [BaseAttribute(Name = "IMAGE_ID", Description = "图片ID")]
        public string IMAGE_ID
        {
            get
            {
                return _ImageId;
            }
            set
            {
                _ImageId = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("IMAGE_ID"));
            }
        }

        [BaseAttribute(Name = "IMAGE_THUMBNAIL", Description = "图片缩略图路径")]
        public string IMAGE_THUMBNAIL
        {
            get
            {
                return _ImageThumbnail;
            }
            set
            {
                _ImageThumbnail = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("IMAGE_THUMBNAIL"));
            }
        }

        [BaseAttribute(Name = "IMAGE_TIFIMAGE", Description = "tif图片")]
        public string IMAGE_TIFIMAGE
        {
            get
            {
                return _ImageTif;
            }
            set
            {
                _ImageTif = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("IMAGE_TIFIMAGE"));
            }
        }
        [BaseAttribute(Name = "IMAGE_HEMDIMAGE", Description = "mehd图片")]
        public string IMAGE_HEMDIMAGE
        {
            get
            {
                return _ImageHEMD;
            }
            set
            {
                _ImageHEMD = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("IMAGE_TIFIMAGE"));
            }
        }
        [BaseAttribute(Name = "IMAGESOURCE_NAME", Description = "原始图名称")]
        public string IMAGESOURCE_NAME
        {
            get
            {
                return _ImageSourceName;
            }
            set
            {
                _ImageSourceName = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("IMAGESOURCE_NAME"));
            }
        }
        [BaseAttribute(Name = "IMAGESOURCEFILE_PATH", Description = "原始图路径")]
        public string IMAGESOURCEFILE_PATH
        {
            get
            {
                return _ImageSourceFilePath;
            }
            set
            {
                _ImageSourceFilePath = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("IMAGESOURCEFILE_PATH"));
            }
        }


        [BaseAttribute(Name = "IMAGE_NAME", Description = "图片名称")]
        [DataGridViewAttribute(ColumnBindingName = "IMAGE_NAME", ColumnDisplayName = "ImageName", IsShow = true)]
        public string IMAGE_NAME
        {
            get
            {
                return _ImageName;
            }
            set
            {
                _ImageName = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("IMAGE_NAME"));
            }
        }

        [BaseAttribute(Name = "IMAGE_FILE_PATH", Description = "图片名称")]
        public string IMAGE_FILE_PATH
        {
            get
            {
                return _ImageFilePath;
            }
            set
            {
                _ImageFilePath = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("IMAGE_FILE_PATH"));
            }
        }

        /// <summary>
        /// -1:提交失败；-3:推送文件服务器失败;-4:提交图片信息失败;0:提交失败;1:上传中;6:UnCommited;
        /// 2:正在获取任务ID;3:正在推送至文件服务器;4:正在提交图片信息;5:上传完成;10:提交状态
        /// -1:CommitFaild;-3:PutFTPFaild;-4:CommitImageInfoFaild;0:CommitFaild;1:UpLoading;
        /// 2:GetTaskIDing;3:PutFTPing;4:SubmitImageInfoing;5:UploadComplete;10:SubmitStatus
        /// </summary>
        [BaseAttribute(Name = "IMAGE_UPLOAD_STATUS", Description = "图片上传状态")]
        [DataGridViewAttribute(ColumnBindingName = "IMAGE_UPLOAD_STATUS", ColumnDisplayName = "SubmitStatus", IsShow = true)]
        public string IMAGE_UPLOAD_STATUS
        {
            get
            {
                return _ImageUploadStatus;
            }
            set
            {
                _ImageUploadStatus = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("IMAGE_UPLOAD_STATUS"));
            }
        }
        [BaseAttribute(Name = "IMAGE_CLIENT_SOURCE", Description = "图片来源")]
        public string IMAGE_CLIENT_SOURCE
        {
            get
            {
                return _Image_ClientSource;
            }
            set
            {
                _Image_ClientSource = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("IMAGE_CLIENT_SOURCE"));
            }
        }

        [BaseAttribute(Name = "IAMGE_LOCALFILE_CREATETIME", Description = "图片本地创建时间")]
        [DataGridViewAttribute(ColumnBindingName = "IAMGE_LOCALFILE_CREATETIME", ColumnDisplayName = "ImageCreateTime", IsShow = true)]
        public string IAMGE_LOCALFILE_CREATETIME
        {
            get
            {
                try
                {
                    _ImageLocalFileCreateTime = Convert.ToDateTime(_ImageLocalFileCreateTime).ToString("yyyy-MM-dd HH:mm:ss");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
               
                return _ImageLocalFileCreateTime;
            }
            set
            {
                _ImageLocalFileCreateTime = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("IAMGE_LOCALFILE_CREATETIME"));
            }
        }
        [BaseAttribute(Name = "IMAGE_UPDATETIME", Description = "图片上传时间")]
        [DataGridViewAttribute(ColumnBindingName = "IMAGE_UPDATETIME", ColumnDisplayName = "ImageUploadTime", IsShow = true)]
        public string IMAGE_UPDATETIME
        {
            get
            {
                try
                {
                    _UpdateTime = Convert.ToDateTime(_UpdateTime).ToString("yyyy-MM-dd HH:mm:ss");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            
                return _UpdateTime;
            }
            set
            {
                _UpdateTime = value;
                RaisePropertyChanged(new PropertyChangedEventArgs("IMAGE_UPDATETIME"));
            }
        }
    }



    /// <summary>
    /// 用户增强扫描的车辆信息
    /// </summary>
    public class EnhanceScanCarInfo1:BaseNotifyPropertyChanged
    {
        /// <summary>
        /// 车辆扫描起始位置
        /// </summary>
        private float _StartCarPosition;
        public float StartCarPosition
        {
            get { return _StartCarPosition; }
            set { _StartCarPosition = value; RaisePropertyChanged(new PropertyChangedEventArgs("StartCarPosition")); }
        }
        /// <summary>
        /// 车辆扫描起始位置
        /// </summary>
        private float _EndCarPosition;
        public float EndCarPosition
        {
            get { return _EndCarPosition; }
            set { _EndCarPosition = value; RaisePropertyChanged(new PropertyChangedEventArgs("EndCarPosition")); }
        }
        /// <summary>
        /// 车辆总长度
        /// </summary>
        private float _CarTotalPosition;
        public float CarTotalPosition
        {
            get { return _CarTotalPosition; }
            set { _CarTotalPosition = value; RaisePropertyChanged(new PropertyChangedEventArgs("CarTotalPosition")); }
        }
        /// <summary>
        /// 车牌
        /// </summary>
        private string _licensePlate;
        public string licensePlate
        {
            get { return _licensePlate; }
            set { _licensePlate = value; RaisePropertyChanged(new PropertyChangedEventArgs("licensePlate")); }
        }

        private float _carLength;
        public float carLength
        {
            get { return _carLength; }
            set { _carLength = value; RaisePropertyChanged(new PropertyChangedEventArgs("carLength")); }
        }
        private string _TaskId;
        public string TaskId
        {
            get { return _TaskId; }
            set { _TaskId = value; RaisePropertyChanged(new PropertyChangedEventArgs("TaskId")); }
        }

    }

    /// <summary>
    /// 用户增强扫描的车辆信息
    /// </summary>
    public class EnhanceScanCarInfo : BaseNotifyPropertyChanged
    {
        /// <summary>
        /// 增强区域左上角X坐标
        /// </summary>
        private float _EnhanceScanLeftXPosition;
        public float EnhanceScanLeftXPosition
        {
            get { return _EnhanceScanLeftXPosition; }
            set { _EnhanceScanLeftXPosition = value; RaisePropertyChanged(new PropertyChangedEventArgs("EnhanceScanLeftXPosition")); }
        }
        /// <summary>
        /// 增强区域左上角Y坐标
        /// </summary>
        private float _EnhanceScanLeftYPosition;
        public float EnhanceScanLeftYPosition
        {
            get { return _EnhanceScanLeftYPosition; }
            set { _EnhanceScanLeftYPosition = value; RaisePropertyChanged(new PropertyChangedEventArgs("EnhanceScanLeftYPosition")); }
        }
        /// <summary>
        /// 增强扫描区域矩形宽
        /// </summary>
        private float _EnhanceScanRectWidth;
        public float EnhanceScanRectWidth
        {
            get { return _EnhanceScanRectWidth; }
            set { _EnhanceScanRectWidth = value; RaisePropertyChanged(new PropertyChangedEventArgs("EnhanceScanRectWidth")); }
        }
        /// <summary>
        /// 增强扫描区域矩形高
        /// </summary>
        private float _EnhanceScanRectHeight;
        public float EnhanceScanRectHeight
        {
            get { return _EnhanceScanRectHeight; }
            set { _EnhanceScanRectHeight = value; RaisePropertyChanged(new PropertyChangedEventArgs("EnhanceScanRectHeight")); }
        }
        /// <summary>
        /// 图像区域总高
        /// </summary>
        private float _ImageHeight;
        public float ImageHeight
        {
            get { return _ImageHeight; }
            set { _ImageHeight = value; RaisePropertyChanged(new PropertyChangedEventArgs("ImageHeight")); }
        }

        /// <summary>
        /// 图像区域总宽
        /// </summary>
        private float _ImageWidth;
        public float ImageWidth
        {
            get { return _ImageWidth; }
            set { _ImageWidth = value; RaisePropertyChanged(new PropertyChangedEventArgs("ImageWidth")); }
        }
        /// <summary>
        /// 车辆扫描起始位置
        /// </summary>
        private float _StartCarPosition;
        public float StartCarPosition
        {
            get { _StartCarPosition = EnhanceScanLeftXPosition; return _StartCarPosition; }
            set {
                _StartCarPosition = value; 
                RaisePropertyChanged(new PropertyChangedEventArgs("StartCarPosition")); }
        }
        /// <summary>
        /// 车辆扫描起始位置
        /// </summary>
        private float _EndCarPosition;
        public float EndCarPosition
        {
            get { _EndCarPosition = EnhanceScanLeftXPosition + EnhanceScanRectWidth; return _EndCarPosition; }
            set { _EndCarPosition = value; RaisePropertyChanged(new PropertyChangedEventArgs("EndCarPosition")); }
        }

        private float _carLength;
        public float carLength
        {
            get { return _carLength; }
            set { _carLength = value; RaisePropertyChanged(new PropertyChangedEventArgs("carLength")); }
        }

        /// <summary>
        /// 车牌
        /// </summary>
        private string _licensePlate;
        public string licensePlate
        {
            get { return _licensePlate; }
            set { _licensePlate = value; RaisePropertyChanged(new PropertyChangedEventArgs("licensePlate")); }
        }

        private string _TaskId;
        public string TaskId
        {
            get { return _TaskId; }
            set { _TaskId = value; RaisePropertyChanged(new PropertyChangedEventArgs("TaskId")); }
        }
    }
}
