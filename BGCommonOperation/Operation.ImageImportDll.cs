using BG_Entities;
using BGCommunication;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace CMW.Common.Utilities
{
    public class ImageImportDll
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct ConfigX
        {
            public ConfigX(int _EngDec, int _ImageFZ, int _DataDec, int _TimeCorrect,int _LowEngerTimeCorrect, int _background, int _DarkMove,
                int _LightMove, int _insertion, int _preview,int _boardCheck,int _mergeSort,int _reserve1,int _reserve2,int _reserve3,int _reserve4)
            {
                EngDec = _EngDec;
                ImageFZ = _ImageFZ;
                DataDec = _DataDec;
                TimeCorrect = _TimeCorrect;
                LowEngerTimeCorrect = _LowEngerTimeCorrect;
                background = _background;
                DarkMove = _DarkMove;
                LightMove = _LightMove;
                insertion = _insertion;
                preview = _preview;

                boardCheck = _boardCheck;
                mergeSort = _mergeSort;
                reserve1 = _reserve1;
                reserve2 = _reserve2;
                reserve3 = _reserve3;
                reserve4 = _reserve4;
            }


            public int EngDec;//能量顺序 || 探测器端口 【临时】
            public int ImageFZ;//图像翻转 || 探测器端口 【临时】
            public int DataDec;//数据方向
            public int TimeCorrect;//时间轴校正
            public int LowEngerTimeCorrect;//低能时间轴校正
            public int background;       // 本底统计方式:0=平均值,1=最大最小值,2=最大最小列
            public int DarkMove;          // 暗本底偏移值
            public int LightMove;          // 亮本底偏移值
            public int insertion; //插值列数：1,2,3,4,5,6,7
            public int preview;   //扫描预览

            public int boardCheck;  //探测器板卡校验
            public int mergeSort;   //高低能融合顺序
            public int reserve1;    //预留1
            public int reserve2;    //预留2
            public int reserve3;    //预留3
            public int reserve4;    //预留4
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct _Rect
        {
            int x, y, w, h;
            //_Rect() {  }
            public _Rect(int _x, int _y, int _w, int _h) { x = _x; y = _y; w = _w; h = _h; }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct _Property
        {
            public Single gamma ;           // 灰度系数
            public Single reserve;           // 预留系数
            public Single brightness ;     // 亮度0~100
            public Single contrast ;       // 对比度0~100
            public Single blursharpen ;    // 模糊锐化0~100
            public Single superenhanced ;   // 超级增强0~100
            public Single habsorptivity ; // 高吸收率0~100
            public Single labsorptivity;   // 低吸收率0~100
            public int autocontrast ;         // 自动亮度对比度
            public int colour;               // 着色：0=灰度，1=伪彩，2=物质识别
            public int equalization;         // 直方图均衡
            public int anticolor;            // 反色
            public int wiping;               // 剔除：0=不剔除，1=剔除有机物，2=剔除无机物
            public int scanning;             // 亮度扫描
            public int energy;               // 能量显示：0=低能，1=高能，2=双能
            public int inversion;            // 图像翻转

            public int stretching;            // 灰度拉伸
            public int stripping;            // 图像剥离
            public int filter;            // 滤波
            public int emboss;            // 浮雕
        }
        //static _RectObject = new _Rect(0,0,0,0);
        [DllImport("gdi32")]
        public static extern int DeleteObject(IntPtr o);
        #region 扫描站SDK接口
        /// <summary>
        /// 全局扫描站句柄
        /// </summary>
        public static IntPtr intPtr = IntPtr.Zero;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CALLBACK_STATUS(int status, int error);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CALLBACK_UPDATE();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CALLBACK_SCAN(int status, int carid, string path);


        /// <summary>
        /// 创建句柄
        /// </summary>
        /// <returns></returns>
        [DllImport("ImageX.dll", EntryPoint = "SX_Create", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr SX_Create();
        /// <summary>
        /// 销毁句柄
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        [DllImport("ImageX.dll", EntryPoint = "SX_Destroy", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SX_Destroy(IntPtr handle);

        /// <summary>
        /// 设置配置参数
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        [DllImport("ImageX.dll", EntryPoint = "SX_SetConfig", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SX_SetConfig(IntPtr handle, ConfigX config);
        /// <summary>
        /// 设置配置参数
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        [DllImport("ImageX.dll", EntryPoint = "SX_SetConfigFile", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SX_SetConfigFile(IntPtr handle, string path);
        /// <summary>
        /// 读取版本
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        [DllImport("ImageX.dll", EntryPoint = "IX_Version", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static string IX_Version(); 
        /// <summary>
        /// 读取版本
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns> ExactSpelling
        [DllImport("ImageX.dll", EntryPoint = "SX_Version", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr SX_Version();
        /**
        * @brief            设置配置文件
        * @param[in]        path            配置文件路径
        * @return           成功：0；失败：-1
        */
        [DllImport("ImageX.dll", EntryPoint = "IX_SetConfig", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int IX_SetConfig( string path);

        /// <summary>
        /// 车头信号
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        [DllImport("ImageX.dll", EntryPoint = "SX_SetHead", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SX_SetHead(IntPtr handle); 
        /// <summary>
        /// 车尾信号
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        [DllImport("ImageX.dll", EntryPoint = "SX_SetTail", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SX_SetTail(IntPtr handle);

        /**
        * @brief            获取探测器时间
        * @param[in]        handle          扫描句柄
        * @param[in]        bootTime        开机时间
        * @param[in]        scanTime        扫描时间
        * @return           成功：0；失败：-1
        */
        [DllImport("ImageX.dll", EntryPoint = "SX_GetTimes", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SX_GetTimes(IntPtr handle, out int bootTime, out int scanTime); // extern static


        /// <summary>
        /// 设置分隔图片参数
        /// </summary>
        /// <returns> /returns>
        [DllImport("ImageX.dll", EntryPoint = "SX_Partition", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SX_Partition(string src);
        /**
        * @brief            获取探测器版本
        * @param[in]        handle          扫描句柄
        * @param[in]        version         探测器版本
        * @return           成功：0；失败：-1
        */
        [DllImport("ImageX.dll", EntryPoint = "SX_GetVersion", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SX_GetVersion(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] StringBuilder buf);

        /**
        * @brief            获取探测器序列号
        * @param[in]        handle          扫描句柄
        * @param[in]        number          探测器序列号
        * @return           成功：0；失败：-1
        */
        [DllImport("ImageX.dll", EntryPoint = "SX_GetSerialNumber", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SX_GetSerialNumber(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] StringBuilder buf);
          
        /**
        * @brief            设置车厢分割信息
        * @param[in]        handle          扫描句柄
        * @param[in]        id              车厢ID
        * @param[in]        length          车厢长度
        * @param[in]        speed           车厢速度
        * @return           成功：0；失败：-1
        */
        [DllImport("ImageX.dll", EntryPoint = "SX_Segmentation", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SX_Segmentation(IntPtr handle,long id,float length, float speed);

        /**
        * @brief            获取能量模式
        * @param[in]        himage          图像句柄
        * @return           能量模式：0=低能，1=高能，2=双能
        */
        [DllImport("ImageX.dll", EntryPoint = "IX_GetEnergy", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int IX_GetEnergy(IntPtr himage);
        /**
        * @brief            获取图像句柄
        * @param[in]        handle          文件句柄
        * @param[in]        rect            图像区域
        * @return           成功：图像句柄；失败：NULL
        */
        [DllImport("ImageX.dll", EntryPoint = "IX_GetImage", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr IX_GetImage(IntPtr handle, _Rect rect);
        /**
        * @brief            设置图像属性
        * @param[in]        himage          图像句柄
        * @param[in]        property        图像属性
        * @return           成功：0；失败：-1
        */
        [DllImport("ImageX.dll", EntryPoint = "IX_SetProperty", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int IX_SetProperty(IntPtr himage, _Property property);
        /**
        * @brief            打开文件句柄
        * @param[in]        path            扫描文件路径
        * @return           成功：文件句柄；失败：NULL
        */
        [DllImport("ImageX.dll", EntryPoint = "IX_Open", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr IX_Open(string path);
        /**
        * @brief            扫描图像分割
        * @param[in]        handle          文件句柄
        * @param[in]        path            分割图像文件路径
        * @return           分割图像个数
        */
        [DllImport("ImageX.dll", EntryPoint = "IX_Partition", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int IX_Partition(IntPtr handle, string path);
        /// <summary>
        /// 保存标准图片文件
        /// </summary>
        /// <param name="handle">文件句柄</param>
        /// <param name="dst">标准图片文件路径</param>
        /// <returns></returns>
        [DllImport("ImageX.dll", EntryPoint = "IX_Save", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int IX_Save(IntPtr handle, string dst);
        /**
        * @brief            多视角扫描文件合并
        * @param[in]        path1           视角1扫描文件路径
        * @param[in]        path2           视角2扫描文件路径
        * @param[in]        path            合并文件保存路径
        * @param[in]        mode            合并模式：0=垂直合并，1=水平合并
        * @return           成功：0；失败：-1
        */
        [DllImport("ImageX.dll", EntryPoint = "IX_FileMerge", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int IX_FileMerge(string path1, string path2, string path, int mode);
        /**
        * @brief            保存标准图片文件
        * @param[in]        himage          图像句柄
        * @param[in]        path            标准图片文件路径
        * @return           成功：0；失败：-1
        */
        [DllImport("ImageX.dll", EntryPoint = "IX_SaveImage", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int IX_SaveImage(IntPtr himage, string path);
        /// <summary>
        /// 保存标准图片文件
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="dst">标准图片文件路径</param>
        /// <returns></returns>
        //[DllImport("ImageX.dll", EntryPoint = "IX_SaveImage", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public extern static int IX_SaveImage(,string dst);
        /**
        * @brief            设置扫描目录
        * @param[in]        handle          扫描句柄
        * @param[in]        path            扫描文件保存目录
        * @return           成功：0；失败：-1
        */
        [DllImport("ImageX.dll", EntryPoint = "SX_SetPath", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SX_SetPath(IntPtr handle, string path);

        /**
        * @brief            设置日志
        * @param[in]        handle          扫描句柄
        * @param[in]        path            日志文件路径
        * @param[in]        lev             日志等级：V=全部，D=调试，I=信息，W=警告，E=错误
        * @return           成功：0；失败：-1
        */
        [DllImport("ImageX.dll", EntryPoint = "SX_SetLog", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SX_SetLog(IntPtr handle, string path, byte lev);
        /**
   * @brief            设置日志
   * @param[in]        handle          扫描句柄
   * @param[in]        path            日志文件路径
   * @param[in]        lev             日志等级：V=全部，D=调试，I=信息，W=警告，E=错误
   * @return           成功：0；失败：-1
   */
        [DllImport("ImageX.dll", EntryPoint = "SX_SetLog", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SX_SetLog(IntPtr handle, byte[] path, byte lev);
        /// <summary>
        /// 设置状态回调
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        [DllImport("ImageX.dll", EntryPoint = "SX_SetStatusCallback", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SX_SetStatusCallback(IntPtr handle, CALLBACK_STATUS ck);
        /// <summary>
        /// 设置状态回调
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        [DllImport("ImageX.dll", EntryPoint = "SX_SetScanCallback", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SX_SetScanCallback(IntPtr handle, CALLBACK_SCAN ck);


        /// <summary>
        /// 设置刷新回调
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        [DllImport("ImageX.dll", EntryPoint = "SX_SetUpdateCallback", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SX_SetUpdateCallback(IntPtr handle, CALLBACK_UPDATE ck);

        /// <summary>
        /// 设置扫描模式
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        [DllImport("ImageX.dll", EntryPoint = "SX_SetMode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SX_SetMode(IntPtr handle, int mode);
        /// <summary>
        /// 设置扫描方向
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        [DllImport("ImageX.dll", EntryPoint = "SX_SetDirection", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SX_SetDirection(IntPtr handle, int direction);
        /// <summary>
        /// 暗校正
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        [DllImport("ImageX.dll", EntryPoint = "SX_SetDark", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SX_SetDark(IntPtr handle);

        /// <summary>
        /// 能量设置
        ///   handle          扫描句柄
        ///   energy          能量设置：0=单能，1=双能
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        [DllImport("ImageX.dll", EntryPoint = "SX_SetEnergy", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SX_SetEnergy(IntPtr handle, int energy);

        /// <summary>
        /// 亮校正
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        [DllImport("ImageX.dll", EntryPoint = "SX_SetLight", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SX_SetLight(IntPtr handle);
        /// <summary>
        /// 绘制图像数据
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        [DllImport("ImageX.dll", EntryPoint = "SX_DrawBitmap", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SX_DrawBitmap(IntPtr handle, IntPtr bitmap, int width, int height);

        /// <summary>
        /// 绘制图像数据2
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        [DllImport("ImageX.dll", EntryPoint = "SX_DrawBitmap2", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SX_DrawBitmap2(IntPtr handle, IntPtr bitmap, int width, int height);
        /// <summary>
        /// 连接探测器
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        [DllImport("ImageX.dll", EntryPoint = "SX_Connect", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SX_Connect(IntPtr handle, string host, Int16 cmdPort, Int16 imgPort);
        /// <summary>
        /// 断开探测器
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        [DllImport("ImageX.dll", EntryPoint = "SX_Disconnect", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SX_Disconnect(IntPtr handle);
        /// <summary>
        /// 开始扫描
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        [DllImport("ImageX.dll", EntryPoint = "SX_Start", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SX_Start(IntPtr handle);
        /// <summary>
        /// 停止扫描
        /// </summary>
        /// <returns> 成功：0；失败：-1</returns>
        [DllImport("ImageX.dll", EntryPoint = "SX_Stop", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SX_Stop(IntPtr handle);

        /**
        * @brief            获取探测器错误码
        * @param[in]        handle          扫描句柄
        * @return           错误码成功：0；失败-1
        */
        [DllImport("ImageX.dll", EntryPoint = "SX_GetLastError", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SX_GetLastError(IntPtr handle);
        /**
         * @brief            获取探测器办卡号
         * @param[in]        handle          扫描句柄
         * @return           错误码
         */
        [DllImport("ImageX.dll", EntryPoint = "SX_GetBoardNumber", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SX_GetBoardNumber(IntPtr handle, out int number, int BoardLine);
        /// <summary>
        /// 设置探测器频率
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="frequency"></param>
        /// <returns>0：成功 -1：失败</returns>
        [DllImport("ImageX.dll", EntryPoint = "SX_SetFrequency", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SX_SetFrequency(IntPtr handle, int frequency);
        /// <summary>
        /// 设置车速
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="frequency"></param>
        /// <returns>0：成功 -1：失败</returns>
        [DllImport("ImageX.dll", EntryPoint = "SX_SetSpeed", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SX_SetSpeed(IntPtr handle, int speed);

        /**
        * @brief            关闭文件句柄
        * @param[in]        handle          文件句柄
        * @return           成功：0；失败：-1
        */
        [DllImport("ImageX.dll", EntryPoint = "IX_Close", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int IX_Close(IntPtr handle);

        /**
        * @brief            释放图像句柄
        * @param[in]        himage          图像句柄
        * @return           成功：0；失败：-1
        */
        [DllImport("ImageX.dll", EntryPoint = "IX_Release", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int IX_Release(IntPtr himage);
        /**
        * @brief            生成高能物质识别文件（HEMD）
        * @param[in]        handle          文件句柄
        * @param[in]        path            高能物质识别文件路径
        * @return           成功：0；失败：-1
        */
        [DllImport("ImageX.dll", EntryPoint = "IX_Hemd", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int IX_Hemd(IntPtr handle, string path);


        /**
        * @brief            获取探测器板卡错误
        * @param[in]        handle          扫描句柄
        * @param[in]        number          板卡数量
        * @param[in]        board           出错板链序号
        * @param[in]        index           出错板卡序号
        * @return           成功：0；失败：-1
        */
        [DllImport("ImageX.dll", EntryPoint = "SX_GetBoardError", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SX_GetBoardError(IntPtr handle, out int number, out int board, out int index);

        /**
        * @brief            初始化物质表颜色表
        * @param[in]        matex           物质表文件路径
        * @param[in]        color           颜色表文件路径
        * @return           成功：0；失败：-1
        */
        [DllImport("ImageX.dll", EntryPoint = "IX_Init", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int IX_Init(string matex, string color);
        /**
        * @brief            设置物质识别滤波参数
        * @param[in]        filter          滤波参数：0=中值5，1=均值5，2=均值11
        * @return           成功：0；失败：-1
        */
        [DllImport("ImageX.dll", EntryPoint = "IX_SetFilter", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public extern static int IX_SetFilter(int matex);


        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="SourceFilePath">原始图raw</param>
        /// <param name="DstFilePath">保存路径</param>
        /// <param name="IsThubm">是否保存缩略图</param>
        /// <returns></returns>
        public static bool OpenImage(string SourceFilePath, string DstFilePath, bool IsThubm,bool isHemd =false)
        {
            bool Result = false;
            //return false;
            try
            {
                //CommonDeleget.WriteLogAction($"Source File： {SourceFilePath}，Des File{DstFilePath} Image", LogType.NormalLog);
                //CommonDeleget.WriteLogAction($@"Realtime Process Memory ImageImportDll.IX_Open({DstFilePath}){CommonFunc.GetApplicationMemory()}", LogType.NormalLog, false);
                IntPtr Tempptr = IX_Open(SourceFilePath);
                if (Tempptr == null)
                {
                    return Result;
                }
                //判断是不是高能物质识别图
                if (isHemd)
                {
                    Result = ImageImportDll.IX_Hemd(Tempptr, DstFilePath) == 1;
                }
                else if (!IsThubm)
                {
                    Result = IX_Save(Tempptr, DstFilePath) == 1;
                }
                else
                {
                    IntPtr ImagePtr = IX_GetImage(Tempptr, new _Rect(0, 0, 0, 0));
                    if (ImagePtr != null)
                    {
                        _Property propty = new _Property()
                        {
                            gamma = 1,
                            brightness = 50,
                            contrast = 50,
                            blursharpen = 50,
                            superenhanced = 0,
                            habsorptivity = 0,
                            labsorptivity = 100,
                            energy = IX_GetEnergy(ImagePtr)
                         };
                        IX_SetProperty(ImagePtr, propty);
                        Result = IX_SaveImage(ImagePtr, DstFilePath) == 1;
                        IX_Release(ImagePtr);
                    }
                }
               
                IX_Close(Tempptr);
                //CommonDeleget.WriteLogAction($"DestFile {DstFilePath} Image,IX_CLOSE End", LogType.NormalLog);
                //CommonDeleget.WriteLogAction($@"Realtime Process Memory ImageImportDll.IX_Close({DstFilePath}){CommonFunc.GetApplicationMemory()}", LogType.NormalLog, false);
            }
            catch (Exception ex)
            {
                CommonDeleget.WriteLogAction(ex.Message, LogType.ImageImportDllError, true);
            }
            //CommonDeleget.WriteLogAction($"Source File： {SourceFilePath}，DestFile{DstFilePath} Image End。", LogType.NormalLog);
            return Result;
        }

        #endregion
    }
}
