using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace BGUserControl
{
    public class RectDraw
    {
        public UIElement ParentUIElement;
        public DrawingVisual drawingVisual = new DrawingVisual();
        public ContainerVisual ContainerVisual { get; }
        bool isAdd = false;

        public RectDraw(ContainerVisual containerVisual)
        {
            ContainerVisual = containerVisual;
        }
        /// <summary>
        /// 绘制方框
        /// </summary>
        public void DrawRect(Rect Rect)
        {
            using (DrawingContext dc = drawingVisual.RenderOpen())
            {
                dc.DrawRectangle(Brushes.Transparent,new Pen(Brushes.Red,1), Rect);
            }
            if (!isAdd)
            {
                ContainerVisual.Children.Add(drawingVisual);
                isAdd = true;
            }
        }
    }
}
