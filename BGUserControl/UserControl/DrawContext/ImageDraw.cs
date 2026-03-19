using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;

namespace BGUserControl
{
    public class ImageDraw
    {
        public UIElement ParentUIElement;
        public DrawingVisual drawingVisual = new DrawingVisual();
        public ContainerVisual ContainerVisual { get; }
        public Rect drawingRect = new Rect();
        public ImageSource Bitmap = null;
        public IntPtr StartByte;
        bool isAdd = false;
        ImageBrush imageBrush = new ImageBrush();
        public ImageDraw(ContainerVisual containerVisual)
        {
            ContainerVisual = containerVisual;
        }
        public void Init(int width,int height)
        {
            Bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr24, null);
            StartByte = (Bitmap as WriteableBitmap).BackBuffer;
            imageBrush.ImageSource = Bitmap;
        }


        public void DrawImage()
        {
            using (DrawingContext dc = drawingVisual.RenderOpen())
            {
                dc.DrawImage(Bitmap, drawingRect);
            }
            if (!isAdd)
            {
                ContainerVisual.Children.Add(drawingVisual);
                isAdd = true;
            }
        }
        public void ReDrawImage(Rect Rect)
        {
            using (DrawingContext dc = drawingVisual.RenderOpen())
            {
                dc.DrawImage(Bitmap, Rect);
            }
            if (!isAdd)
            {
                ContainerVisual.Children.Add(drawingVisual);
                isAdd = true;
            }
        }


        public void DrawImageByImageEllipse()
        {
            // 创建一个椭圆的矩形
            Rect rect = new Rect(10, 10, 100, 50);
            using (DrawingContext dc = drawingVisual.RenderOpen())
            {
                dc.DrawEllipse(imageBrush, new Pen(Brushes.Black, 1), new Point(0,0), rect.Width / 2, rect.Height / 2);
            }
            if(!isAdd)
            {
                ContainerVisual.Children.Add(drawingVisual);
                isAdd = true;
            }
        }

        public void ReDrawImageByImageEllipse(Point position)
        {
            // 创建一个椭圆的矩形
            using (DrawingContext dc = drawingVisual.RenderOpen())
            {
                //dc.DrawEllipse(imageBrush, new Pen(Brushes.BurlyWood, 1), position, Bitmap.Width / 2, Bitmap.Height / 2);
                dc.DrawRectangle(imageBrush, new Pen(Brushes.Red, 1),new Rect((int)position.X - Bitmap.Width / 2, (int)position.Y - Bitmap.Height / 2, Bitmap.Width, Bitmap.Height));
                //dc.DrawImage(Bitmap, new Rect((int)position.X - Bitmap.Width / 2, (int)position.Y - Bitmap.Height / 2, Bitmap.Width, Bitmap.Height)); 
            }
            if (!isAdd)
            {
                ContainerVisual.Children.Add(drawingVisual);
                isAdd = true;
            }
        }
    }
}
