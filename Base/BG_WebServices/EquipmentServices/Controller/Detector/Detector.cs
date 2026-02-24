using BG_WorkFlow;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static CMW.Common.Utilities.ImageImportDll;

namespace BG_Services
{
    /// <summary>
    /// 探测器
    /// </summary>
    public abstract class Detector : IDetectorEquipment
    {
        public Action ReflashImageAction;
        public Action<int, int, string> ScanImagePathAction;
        public Action<int> DetecotorConnectionAction;
        /// <summary>
        /// 探测器句柄
        /// </summary>
        public IntPtr DetectorIntPtr = IntPtr.Zero;

        public int SingleDetectorConnection = 0;
        public virtual bool Connection(string IPAdress, short port, short ImagePort)
        {
            return true;
        }
        public virtual bool Connection()
        {
            return true;
        }

        public virtual void DisConnection()
        {
            
        }

        public virtual bool IsConnection()
        {
            return true;
        }

        public virtual void Load(string IpAddress = "127.0.0.1", string Port = "3000", string CommandPort = "4001")
        {

        }

        public virtual void Load(DetectorEquipmenntEnum DetectorEquipmennt, string IpAddress = "127.0.0.1", string Port = "3000", string CommandPort = "4001")
        {
            
        }

        public virtual void SX_SetEnergy(int energy)
        {

        }

        public virtual void SX_SetFrequency(int Frequency)
        {

        }
        public virtual int SX_SetSpeed(int Speed)
        {
            return 0;
        }
        /// <summary>
        /// 创建句柄
        /// </summary>
        /// <returns></returns>
        public virtual void SX_Create()
        {
            DetectorIntPtr = ImageImportDll.SX_Create();
        }


        /// <summary>
        /// 销毁句柄
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        public virtual int SX_Destroy()
        {
           return ImageImportDll.SX_Destroy(DetectorIntPtr);
        }


        /// <summary>
        /// 设置配置参数
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        public virtual int SX_SetConfig(ConfigX config)
        {
            return ImageImportDll.SX_SetConfig(DetectorIntPtr, config);
        }
        /// <summary>
        /// 设置配置参数
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        public virtual int SX_SetConfigFile(string path)
        {
            string Dir = Directory.GetParent(path).ToString();
            if (!Directory.Exists(Dir))
            {
                Directory.CreateDirectory(Dir);
            }
            return ImageImportDll.SX_SetConfigFile(DetectorIntPtr, path);
        }
        /// <summary>
        /// 读取版本
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        public virtual string IX_Version()
        {
            return ImageImportDll.IX_Version();
        }
        /// <summary>
        /// 读取版本
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        public virtual IntPtr SX_Version()
        {
            return ImageImportDll.SX_Version();
        }
        /**
        * @brief            设置配置文件
        * @param[in]        path            配置文件路径
        * @return           成功：0；失败：-1
        */
        public virtual int IX_SetConfig(string path)
        {
            return ImageImportDll.IX_SetConfig(path);
        }

        /// <summary>
        /// 车头信号
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        public virtual int SX_SetHead()
        {
            return ImageImportDll.SX_SetHead(DetectorIntPtr);
        }
        /// <summary>
        /// 车尾信号
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        public virtual int SX_SetTail()
        {
            return ImageImportDll.SX_SetTail(DetectorIntPtr);
        }

        /**
        * @brief            获取探测器时间
        * @param[in]        handle          扫描句柄
        * @param[in]        bootTime        开机时间
        * @param[in]        scanTime        扫描时间
        * @return           成功：0；失败：-1
        */
        public virtual int SX_GetTimes(out int bootTime, out int scanTime)
        {
            return ImageImportDll.SX_GetTimes(DetectorIntPtr,out bootTime, out scanTime);
        }


        /// <summary>
        /// 设置分隔图片参数
        /// </summary>
        /// <returns> /returns>
        public virtual int SX_Partition(string src)
        {
            return ImageImportDll.SX_Partition(src);
        }
        /**
        * @brief            获取探测器版本
        * @param[in]        handle          扫描句柄
        * @param[in]        version         探测器版本
        * @return           成功：0；失败：-1
        */
        public virtual int SX_GetVersion([MarshalAs(UnmanagedType.LPStr)] StringBuilder buf)
        {
            return ImageImportDll.SX_GetVersion(DetectorIntPtr, buf);
        }

        /**
        * @brief            获取探测器序列号
        * @param[in]        handle          扫描句柄
        * @param[in]        number          探测器序列号
        * @return           成功：0；失败：-1
        */
        public virtual int SX_GetSerialNumber(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] StringBuilder buf)
        {
            return ImageImportDll.SX_GetSerialNumber(DetectorIntPtr, buf);
        }

        /**
        * @brief            设置车厢分割信息
        * @param[in]        handle          扫描句柄
        * @param[in]        id              车厢ID
        * @param[in]        length          车厢长度
        * @param[in]        speed           车厢速度
        * @return           成功：0；失败：-1
        */
        public virtual int SX_Segmentation(long id, float length, float speed)
        {
            return ImageImportDll.SX_Segmentation(DetectorIntPtr, id, length, speed);
        }



        /**
        * @brief            获取能量模式
        * @param[in]        himage          图像句柄
        * @return           能量模式：0=低能，1=高能，2=双能
        */
        public virtual int IX_GetEnergy()
        {
            return ImageImportDll.IX_GetEnergy(DetectorIntPtr);
        }
        /**
        * @brief            获取图像句柄
        * @param[in]        handle          文件句柄
        * @param[in]        rect            图像区域
        * @return           成功：图像句柄；失败：NULL
        */
        public virtual IntPtr IX_GetImage(_Rect rect)
        {
            return ImageImportDll.IX_GetImage(DetectorIntPtr, rect);
        }
        /**
        * @brief            设置图像属性
        * @param[in]        himage          图像句柄
        * @param[in]        property        图像属性
        * @return           成功：0；失败：-1
        */
        public virtual int IX_SetProperty(_Property property)
        {
            return ImageImportDll.IX_SetProperty(DetectorIntPtr, property);
        }
        /**
        * @brief            打开文件句柄
        * @param[in]        path            扫描文件路径
        * @return           成功：文件句柄；失败：NULL
        */
        public virtual IntPtr IX_Open(string path)
        {
            return ImageImportDll.IX_Open(path);
        }
        /**
        * @brief            扫描图像分割
        * @param[in]        handle          文件句柄
        * @param[in]        path            分割图像文件路径
        * @return           分割图像个数
        */
        public virtual int IX_Partition(IntPtr handle, string path)
        {
            return ImageImportDll.IX_Partition(DetectorIntPtr,path);
        }
        /// <summary>
        /// 保存标准图片文件
        /// </summary>
        /// <param name="handle">文件句柄</param>
        /// <param name="dst">标准图片文件路径</param>
        /// <returns></returns>
        public virtual int IX_Save(string dst)
        {
            return ImageImportDll.IX_Save(DetectorIntPtr, dst);
        }
        /**
        * @brief            多视角扫描文件合并
        * @param[in]        path1           视角1扫描文件路径
        * @param[in]        path2           视角2扫描文件路径
        * @param[in]        path            合并文件保存路径
        * @param[in]        mode            合并模式：0=垂直合并，1=水平合并
        * @return           成功：0；失败：-1
        */
        public virtual int IX_FileMerge(string path1, string path2, string path, int mode) 
        {
            return ImageImportDll.IX_FileMerge(path1, path2, path, mode);
        }
        /**
        * @brief            保存标准图片文件
        * @param[in]        himage          图像句柄
        * @param[in]        path            标准图片文件路径
        * @return           成功：0；失败：-1
        */
        public virtual int IX_SaveImage(string path)
        {
            return ImageImportDll.IX_SaveImage(DetectorIntPtr, path);
        }
        /// <summary>
        /// 保存标准图片文件
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="dst">标准图片文件路径</param>
        /// <returns></returns>
        //[DllImport("ImageX.dll", EntryPoint = "IX_SaveImage", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public virtual int IX_SaveImage(,string dst);
        /**
        * @brief            设置扫描目录
        * @param[in]        handle          扫描句柄
        * @param[in]        path            扫描文件保存目录
        * @return           成功：0；失败：-1
        */
        public virtual int SX_SetPath(string path)
        {
            return ImageImportDll.SX_SetPath(DetectorIntPtr, path);
        }

        /**
        * @brief            设置日志
        * @param[in]        handle          扫描句柄
        * @param[in]        path            日志文件路径
        * @param[in]        lev             日志等级：V=全部，D=调试，I=信息，W=警告，E=错误
        * @return           成功：0；失败：-1
        */
        public virtual int SX_SetLog(string path, byte lev)
        {
            return ImageImportDll.SX_SetLog(DetectorIntPtr, path, lev);
        }
        /**
       * @brief            设置日志
       * @param[in]        handle          扫描句柄
       * @param[in]        path            日志文件路径
       * @param[in]        lev             日志等级：V=全部，D=调试，I=信息，W=警告，E=错误
       * @return           成功：0；失败：-1
       */
        public virtual int SX_SetLog(byte[] path, byte lev)
        {
            return ImageImportDll.SX_SetLog(DetectorIntPtr, path, lev);
        }
        /// <summary>
        /// 设置状态回调
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        public virtual int SX_SetStatusCallback(CALLBACK_STATUS ck)
        {
            return ImageImportDll.SX_SetStatusCallback(DetectorIntPtr, ck);
        }
        /// <summary>
        /// 设置状态回调
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        public virtual int SX_SetScanCallback(CALLBACK_SCAN ck)
        {
            return ImageImportDll.SX_SetScanCallback(DetectorIntPtr, ck);
        }


        /// <summary>
        /// 设置刷新回调
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        public virtual int SX_SetUpdateCallback(CALLBACK_UPDATE ck)
        {
            return ImageImportDll.SX_SetUpdateCallback(DetectorIntPtr, ck);
        }

        /// <summary>
        /// 设置扫描模式
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        public virtual int SX_SetMode(int mode)
        {
            return ImageImportDll.SX_SetMode(DetectorIntPtr, mode);
        }
        /// <summary>
        /// 设置扫描方向
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        public virtual int SX_SetDirection(int direction)
        {
            return ImageImportDll.SX_SetDirection(DetectorIntPtr, direction);
        }
        /// <summary>
        /// 暗校正
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        public virtual int SX_SetDark()
        {
            return ImageImportDll.SX_SetDark(DetectorIntPtr);
        }

        /// <summary>
        /// 亮校正
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        public virtual int SX_SetLight()
        {
            return ImageImportDll.SX_SetLight(DetectorIntPtr);
        }
        /// <summary>
        /// 绘制图像数据
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        public virtual int SX_DrawBitmap(IntPtr bitmap, int width, int height)
        {
            return ImageImportDll.SX_DrawBitmap(DetectorIntPtr, bitmap, width, height);
        }
        /// <summary>
        /// 连接探测器
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        public virtual int SX_Connect(string host, Int16 cmdPort, Int16 imgPort)
        {
            return ImageImportDll.SX_Connect(DetectorIntPtr, host, cmdPort, imgPort);
        }
        /// <summary>
        /// 断开探测器
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        public virtual int SX_Disconnect()
        {
            return ImageImportDll.SX_Disconnect(DetectorIntPtr);
        }
        /// <summary>
        /// 开始扫描
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        public virtual int SX_Start()
        {
            return ImageImportDll.SX_Start(DetectorIntPtr);
        }
        /// <summary>
        /// 停止扫描
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        public virtual int SX_Stop()
        {
            return ImageImportDll.SX_Stop(DetectorIntPtr);
        }

        /**
        * @brief            获取探测器错误码
        * @param[in]        handle          扫描句柄
        * @return           错误码成功：0；失败-1
        */
        public virtual int SX_GetLastError()
        {
            return ImageImportDll.SX_GetLastError(DetectorIntPtr);
        }
        /**
         * @brief            获取探测器办卡号
         * @param[in]        handle          扫描句柄
         * @return           错误码
         */
        public virtual int SX_GetBoardNumber(out int number, int BoardLine)
        {
            return ImageImportDll.SX_GetBoardNumber(DetectorIntPtr,out number, BoardLine);
        }

        /**
        * @brief            关闭文件句柄
        * @param[in]        handle          文件句柄
        * @return           成功：0；失败：-1
        */
        public virtual int IX_Close()
        {
            return ImageImportDll.IX_Close(DetectorIntPtr);
        }

        /**
        * @brief            释放图像句柄
        * @param[in]        himage          图像句柄
        * @return           成功：0；失败：-1
        */
        public virtual int IX_Release(IntPtr ImagePtr)
        {
            return ImageImportDll.IX_Release(ImagePtr);
        }
        /**
        * @brief            生成高能物质识别文件（HEMD）
        * @param[in]        handle          文件句柄
        * @param[in]        path            高能物质识别文件路径
        * @return           成功：0；失败：-1
        */
        public virtual int IX_Hemd(IntPtr handle, string path)
        {
            return ImageImportDll.IX_Hemd(handle, path);
        }


        /**
        * @brief            获取探测器板卡错误
        * @param[in]        handle          扫描句柄
        * @param[in]        number          板卡数量
        * @param[in]        board           出错板链序号
        * @param[in]        index           出错板卡序号
        * @return           成功：0；失败：-1
        */
        public virtual int SX_GetBoardError( out int number, out int board, out int index)
        {
            return ImageImportDll.SX_GetBoardError(DetectorIntPtr,out number,out board, out index);
        }

        /**
        * @brief            初始化物质表颜色表
        * @param[in]        matex           物质表文件路径
        * @param[in]        color           颜色表文件路径
        * @return           成功：0；失败：-1
        */
        public virtual int IX_Init(string matex, string color) 
        {
            return ImageImportDll.IX_Init(matex, color);
        }
        /**
        * @brief            设置物质识别滤波参数
        * @param[in]        filter          滤波参数：0=中值5，1=均值5，2=均值11
        * @return           成功：0；失败：-1
        */
        public virtual int IX_SetFilter(int matex)
        {
            return ImageImportDll.IX_SetFilter(matex);
        }
      
    }
}
