using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CMW.Common.Utilities
{
    public static class WriteableBitmapExtentions
    {
        public static bool WriteableBitmapToJpg(this WriteableBitmap writeableBitmap)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "save to jpg";
            saveFileDialog.FileName = $"{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}_{DateTime.Now.Millisecond}.jpg";
            saveFileDialog.Filter = "Images (*.bmp)|*.bmp|Images (*.emf)|*.emf|Images (*.exif)|*.exif|Images (*.gif)|*.gif|Images (*.ico)|*.ico|Images (*.jpg)|*.jpg|Images (*.png)|*.png|Images (*.tiff)|*.tiff|Images (*.wmf)|*.wmf";
            saveFileDialog.ShowDialog();
            outputFile(writeableBitmap, saveFileDialog.FileName);
            return false;
        }

        public static bool outputFile(WriteableBitmap writeableBitmap,string FilePath)
        {
            if (writeableBitmap == null)
            {
                return false;
            }
            try
            {
                RenderTargetBitmap rtbitmap = new RenderTargetBitmap(writeableBitmap.PixelWidth, writeableBitmap.PixelHeight, writeableBitmap.DpiX, writeableBitmap.DpiY, PixelFormats.Default);
                DrawingVisual drawingVisual = new DrawingVisual();
                using (var dc = drawingVisual.RenderOpen())
                {
                    dc.DrawImage(writeableBitmap, new Rect(0, 0, writeableBitmap.Width, writeableBitmap.Height));
                }
                rtbitmap.Render(drawingVisual);
                JpegBitmapEncoder bitmapEncoder = new JpegBitmapEncoder();
                bitmapEncoder.Frames.Add(BitmapFrame.Create(rtbitmap));
                if (!File.Exists(FilePath))
                {
                    FileStream fileStream = File.OpenWrite(FilePath);
                    bitmapEncoder.Save(fileStream);
                    fileStream.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            return false;
        }

        public static bool WriteableBitmapToPng(this WriteableBitmap writeableBitmap)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "save to jpg";
            saveFileDialog.FileName = $"{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}_{DateTime.Now.Millisecond}.png";
            saveFileDialog.Filter = "Images (*.bmp)|*.bmp|Images (*.emf)|*.emf|Images (*.exif)|*.exif|Images (*.gif)|*.gif|Images (*.ico)|*.ico|Images (*.jpg)|*.jpg|Images (*.png)|*.png|Images (*.tiff)|*.tiff|Images (*.wmf)|*.wmf";
            saveFileDialog.ShowDialog();
            outputFilePng(writeableBitmap, saveFileDialog.FileName);
            return false;
        }

        public static bool outputFilePng(WriteableBitmap writeableBitmap, string FilePath)
        {
            if (writeableBitmap == null)
            {
                return false;
            }
            try
            {
                RenderTargetBitmap rtbitmap = new RenderTargetBitmap(writeableBitmap.PixelWidth, writeableBitmap.PixelHeight, writeableBitmap.DpiX, writeableBitmap.DpiY, PixelFormats.Default);
                DrawingVisual drawingVisual = new DrawingVisual();
                using (var dc = drawingVisual.RenderOpen())
                {
                    dc.DrawImage(writeableBitmap, new Rect(0, 0, writeableBitmap.Width, writeableBitmap.Height));
                }
                rtbitmap.Render(drawingVisual);
                PngBitmapEncoder bitmapEncoder = new PngBitmapEncoder();
                bitmapEncoder.Frames.Add(BitmapFrame.Create(rtbitmap));
                if (!File.Exists(FilePath))
                {
                    FileStream fileStream = File.OpenWrite(FilePath);
                    bitmapEncoder.Save(fileStream);
                    fileStream.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            return false;
        }
    }
}
