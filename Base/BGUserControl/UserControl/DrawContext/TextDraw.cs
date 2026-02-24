using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using static System.Net.Mime.MediaTypeNames;
using Brushes = System.Windows.Media.Brushes;

namespace BGUserControl
{
    public class TextDraw
    {
        public UIElement ParentUIElement;
        public DrawingVisual drawingVisual = new DrawingVisual();
        public ContainerVisual ContainerVisual { get; }
        bool isAdd = false;
        #region Member
        Typeface typeface;
        List<TextStruct> textStructs = new List<TextStruct>();
        #endregion

        public TextDraw(ContainerVisual containerVisual)
        {
            ContainerVisual = containerVisual;
            InitFont();
        }

        private void InitFont()
        {
            var fontFamily = new System.Windows.Media.FontFamily("黑体");
            typeface = fontFamily.GetTypefaces().First();
        }

        /// <summary>
        /// 绘制字体
        /// </summary>
        public void DrawText(string FontStr)
        {
            // 创建文字格式
            FormattedText formattedText = new FormattedText(
                FontStr,   // 文字内容
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeface,
                36,                 // 文字大小
                Brushes.Black       // 文字笔刷
            );
           
            using (DrawingContext dc = drawingVisual.RenderOpen())
            {
                dc.DrawText(formattedText, new System.Windows.Point(0,0));
            }
            if(!isAdd)
            {
                ContainerVisual.Children.Add(drawingVisual);
                isAdd = true;
            }
        }
        /// <summary>
        /// 绘制字体
        /// </summary>
        public void MoveText(int xDistance,int yDistence)
        {
            
            using (DrawingContext dc = drawingVisual.RenderOpen())
            {
                foreach (var TextStructItem in textStructs)
                {
                    TextStructItem.textPosition.X -= xDistance;
                    dc.DrawText(TextStructItem.formattedText, TextStructItem.textPosition);
                }
            }
            if (!isAdd)
            {
                ContainerVisual.Children.Add(drawingVisual);
                isAdd = true;
            }
        }

        /// <summary>
        /// 绘制字体
        /// </summary>
        public void DrawText(string FontStr,System.Windows.Point point)
        {
            // 创建文字格式
            FormattedText formattedText;
            TextStruct _TextStruct = textStructs.FirstOrDefault(q=>q.Text == FontStr);
            if(_TextStruct !=null)
            {
                formattedText = _TextStruct.formattedText;
                _TextStruct.textPosition = point;
            }
            else 
            {
                formattedText = new FormattedText(
                FontStr,   // 文字内容
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeface,
                36,                 // 文字大小
                Brushes.Black       // 文字笔刷
                );
                textStructs.Add(new TextStruct() { Text = FontStr ,formattedText = formattedText,textPosition = point });
            }
        }
        /// <summary>
        /// 重新计算坐标
        /// </summary>

        public void ReDrawText()
        {

        }
    }

    public class TextStruct
    {
        public string Text;
        public FormattedText formattedText;
        public System.Windows.Point textPosition;
    }
}
