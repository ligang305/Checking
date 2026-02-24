using BG_Entities;
using BG_Services;
using BGLogs;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
using static CMW.Common.Utilities.ImageImportDll;

namespace BGUserControl
{
    /// <summary>
    /// ScanImage.xaml 的交互逻辑
    /// </summary>
    public partial class ScanImage : UserControl
    {
        public WriteableBitmap Bigwbmp = null;
        IntPtr _BackBuffer = IntPtr.Zero;
        int ImageHeight; int ImageWidth;
        [DllImport("msvcrt.dll", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        private static extern void memset(IntPtr dst,int filler,int count);

        public ScanImage()
        {
            InitializeComponent();
        }
        protected override void OnRenderSizeChanged(System.Windows.SizeChangedInfo sizeInfo)
        {
            try
            {
                base.OnRenderSizeChanged(sizeInfo);
                this.InvalidateVisual();
                ImageHeight = Convert.ToInt32(sizeInfo.NewSize.Height);
                ImageWidth = Convert.ToInt32(sizeInfo.NewSize.Width);
                //DetecotrControllerManager.GetInstance().SX_GetBoardError(out var boradNum, out var boardLineNo, out var Index);
                //ImageHeight = ImageHeight < boradNum * 16 * 2 ? boradNum * 16 * 2 : ImageHeight;
                Bigwbmp = new WriteableBitmap(ImageWidth, ImageHeight, 96, 96, PixelFormats.Bgr32, null);
                _BackBuffer = Bigwbmp.BackBuffer;

                memset(Bigwbmp.BackBuffer, 205, Bigwbmp.BackBufferStride * ImageHeight);
                Bigwbmp.Lock();
                Bigwbmp.AddDirtyRect(new System.Windows.Int32Rect(0, 0, Convert.ToInt32(Bigwbmp.Width), Convert.ToInt32(Bigwbmp.Height)));
                Bigwbmp.Unlock();

                ScrollImageServices.Service.ScrollImageEvent += StartScrollImage;
                ScrollImageServices.Service.ReflashPreviewImageAction += ReflashPreviewImage;
                RollImage.Source = Bigwbmp;
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 开始卷图
        /// </summary>
        /// <param name="IsScrollImage"></param>
        public void StartScrollImage(bool IsScrollImage)
        {
            if (IsScrollImage)
            {
                Task.Run(() =>
                {
                    while (true)
                    {
                        //停止卷图或者当前是背散插件则不进行卷图
                        if (!ScrollImageServices.Service.IsScrollImage || controlVersion == ControlVersion.BS) break;
                        Thread.Sleep(20);
                        Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            Bigwbmp.Lock();
                            Bigwbmp.AddDirtyRect(new System.Windows.Int32Rect(0, 0, Convert.ToInt32(Bigwbmp.Width), Convert.ToInt32(Bigwbmp.Height)));
                            Bigwbmp.Unlock();
                        }));
                    }
                });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void ReflashPreviewImage()
        {
            try
            {
                if(intPtr == IntPtr.Zero)
                {
                    BG_MESSAGEBOX.Show(Application.Current?.MainWindow, "intPtr == IntPtr.Zero", "intPtr == IntPtr.Zero", 400, 600);
                    return;
                }
                if (_BackBuffer == IntPtr.Zero) {
                    BG_MESSAGEBOX.Show(Application.Current?.MainWindow, "_BackBuffer == IntPtr.Zero", "_BackBuffer == IntPtr.Zero", 400, 600);
                    return;
                }
                //Log.GetDistance().WriteInfoLogs($"ImageX.dll info intPtr={intPtr},_BackBuffer={_BackBuffer}, ImageWidth={ImageWidth}, ImageHeight={ImageHeight}");
                SX_DrawBitmap(intPtr, _BackBuffer, ImageWidth, ImageHeight);
            }
            catch (Exception ex)
            {
                WriteLogAction($"{ex.StackTrace}", LogType.ImageImportDllError);
            }
        }
    }
}
