using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using Point = System.Windows.Point;

namespace BGUserControl
{
    public class ImageContent : UIElement
    {
        public ImageDraw imageDraw;
        public TextDraw textDraw;
        public RectDraw rectDraw;
        public ImageDraw magnifierDraw;

        public ImageSource Bitmap = null;
        public ImageSource BackBitmap = null;
        public Rect drawingRect = new Rect();
        public Action<MouseModeRecord> MouseMovePiex;
        public Action<double,double> UpdateWindowAndLevel;

        /// <inheritdoc />
        public ImageContent()
        {
            ContainerVisual = new ContainerVisual();
            AddVisualChild(ContainerVisual);
            imageDraw = new ImageDraw(ContainerVisual);
            textDraw = new TextDraw(ContainerVisual);
            rectDraw = new RectDraw(ContainerVisual);
            magnifierDraw = new ImageDraw(ContainerVisual);
        }

        public void Init(ImageSource bitmap, ImageSource _BackBitmap, Rect Rect)
        {
            Bitmap = bitmap;
            BackBitmap = _BackBitmap;
            drawingRect = Rect;
            imageDraw.Bitmap = Bitmap;
            imageDraw.drawingRect = Rect;
            magnifierDraw.Init(400, 300);
            DrawImage();
        }


        /// <inheritdoc />
        protected override Visual GetVisualChild(int index)
        {
            return ContainerVisual;
        }

        public ContainerVisual ContainerVisual { get; }

        /// <inheritdoc />
        protected override int VisualChildrenCount => 1;



        #region Bussiness Code
        public void DrawImage()
        {
            imageDraw?.DrawImage();
        }

        public void ReDrawImage(Rect Rect)
        {
            imageDraw?.ReDrawImage(Rect);
        }

        public void DrawText(string DrawText)
        {
            textDraw?.DrawText(DrawText);
        }
   
        public void DrawText(string DrawText,Point point)
        {
            textDraw?.DrawText(DrawText, point);
        }
        public void ReDrawMove(int pointX,int pointY)
        {
            textDraw?.MoveText(pointX, pointY);
        }

        public void DrawRect(Rect rect)
        {
            rectDraw?.DrawRect(rect);
        }

        public void DrawMagnifier(Point point)
        {
            magnifierDraw?.ReDrawImageByImageEllipse(point);
        }

        public void UpdateMousePosition(MouseModeRecord mouseModeRecord)
        {
            MouseMovePiex?.Invoke(mouseModeRecord);
        }
        public void UpdateWindowAndLevelAction(double min,double max)
        {
            UpdateWindowAndLevel?.Invoke(min, max);
        }
        
        #endregion
    }


    public class MouseModeRecord
    {
        public System.Windows.Point LeftTopPosition;

        public System.Windows.Point ImagePxPosition;

        public System.Windows.Point ImagePxScalePosition;

        public double ScaleValue;

        public double min = 0;

        public double max = 65535;
    }
}
