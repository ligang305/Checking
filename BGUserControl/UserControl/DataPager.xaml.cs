using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using BG_Entities;
using CMW.Common.Utilities;

namespace BGUserControl
{
    /// <summary>
    /// DataPager.xaml 的交互逻辑
    /// </summary>
    public partial class DataPager : BaseWindows
    {
        public int pageIndex = 1;
        public int num = 100;
        public int pageSize = 1;
        public int count;
        /// <summary>
        /// 当前页码
        /// </summary>
        public int CurrentIndex { get; set; }
        /// <summary>
        /// 总数
        /// </summary>
        public int Total { get; set; }
        public DataPager(List<int> NumList)
        {
            InitializeComponent();
            ReflashControlLabel();
            ReflashControlLabelFontSize();
            InitNumList(NumList);
            cbxPageIndex.SelectionChanged += CbxPageIndex_SelectionChanged;
            num = Convert.ToInt32(cbxPageIndex.Text);
            txtJumpPage.Text = "1";
            ButtonInvoke.SwitchLanguageEvent -= SwitchLanguage;
            ButtonInvoke.SwitchLanguageEvent += SwitchLanguage;
            ButtonInvoke.SwitchFontSizeEvent -= SwitchFontSize;
            ButtonInvoke.SwitchFontSizeEvent += SwitchFontSize;
        }

        private void ReflashControlLabel()
        {
            lblTotal.Content = CommonDeleget.UpdateStatusNameAction("total");
            lblEachPage.Content = CommonDeleget.UpdateStatusNameAction("EachPage");
            lblpre.Content = CommonDeleget.UpdateStatusNameAction("pre");
            lblnext.Content = CommonDeleget.UpdateStatusNameAction("next");
            lblRefresh.Content = CommonDeleget.UpdateStatusNameAction("Refresh");
            lblgo.Content = CommonDeleget.UpdateStatusNameAction("go");
            lblPageNum.Content = CommonDeleget.UpdateStatusNameAction("PageNum");
        }
        private void ReflashControlLabelFontSize()
        {
            lblTotal.FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.Normal);
            lblEachPage.FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.Normal);
            lblpre.FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.Normal);
            lblnext.FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.Normal);
            lblRefresh.FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.Normal);
            lblgo.FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.Normal);
            lblPageNum.FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.Normal);
            lblPageCount.FontSize = CommonDeleget.UpdateFontSizeAction(CMWFontSize.Normal);
        }

        private void SwitchFontSize(string FontSize)
        {
            ReflashControlLabelFontSize();
        }
        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="Language"></param>
        private void SwitchLanguage(string Language)
        {
            Base_SwitchLanguage(Language);
            ReflashControlLabel();
            btnRefresh_MouseClick(null, null);
        }
        private void InitNumList(List<int> NumList)
        {
            cbxPageIndex.ItemsSource = NumList;
            cbxPageIndex.SelectedIndex = 0;
        }

        public delegate int BackToCallBack(int pageIndex, int num, ref int count);
        public event BackToCallBack OnPageGetList;

        /// <summary>
        /// 分页选项下拉改变触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxPageIndex_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBox cb = sender as ComboBox;
                var selectItem = cb.SelectedItem as int?;
                num = selectItem == null
                    ?5:Convert.ToInt32(selectItem);
                int iEnd = count % num == 0 ? count / num : count / num + 1;
                pageIndex = string.IsNullOrEmpty(txtJumpPage.Text.Trim()) ? 0 : Convert.ToInt32(txtJumpPage.Text.Trim());
                if (pageIndex > iEnd)
                {
                    pageIndex = iEnd;
                }
                DgvListBind(pageIndex, num, ref count);
                ImgControl(pageIndex, count);
            }
            catch { }
        }



        /// <summary>
        /// 刷新绑定的list
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="num"></param>
        /// <param name="count"></param>
        private void DgvListBind(int pageIndex, int num, ref int count)
        {
            if (OnPageGetList != null)
            {
                int start = pageIndex;
                count = OnPageGetList(pageIndex, num, ref count);
                lblTotal.Content = string.Format($"{count.ToString()} " + CommonDeleget.UpdateStatusNameAction("total"));
                if (count == 0)
                {
                    lblPageCount.Content = CommonDeleget.UpdateStatusNameAction("Intal") + ":" +CommonDeleget.UpdateStatusNameAction("Pages");// string.Format($"共1 页"); 
                }
                else
                {
                    lblPageCount.Content = string.Format($"{CommonDeleget.UpdateStatusNameAction("Intal")}{(count % num == 0 ? count / num : count / num + 1).ToString()} {CommonDeleget.UpdateStatusNameAction("Pages")}"); 
                }
            }
        }

        private void ImgControl(int pageIndex, int pageCount)
        {
            pageCount = pageCount % num == 0 ? pageCount / num : (pageCount / num) + 1;
            if (pageIndex <= 1 && pageCount <= 1)
            {
                btnLeft.IsEnabled = false;
                btnRight.IsEnabled = false;
            }
            else if (pageIndex == 1 && pageCount > 1)
            {
                btnLeft.IsEnabled = false;
                btnRight.IsEnabled = true;
            }
            else if (pageIndex > 1 && pageIndex < pageCount)
            {
                btnLeft.IsEnabled = true;
                btnRight.IsEnabled = true;
            }
            else if (pageIndex > 1 && pageIndex == pageCount)
            {
                btnLeft.IsEnabled = true;
                btnRight.IsEnabled = false;
            }
        }

        private void btnLeft_MouseClick(object sender, MouseEventArgs e)
        {
            pageIndex -= 1;
            txtJumpPage.Text = pageIndex.ToString();
            num = Convert.ToInt32(cbxPageIndex.SelectedItem.ToString());
            DgvListBind(pageIndex, num, ref count);
            ImgControl(pageIndex, count);
        }

        private void btnRight_MouseClick(object sender, MouseEventArgs e)
        {
            pageIndex += 1;
            txtJumpPage.Text = pageIndex.ToString();
            num = Convert.ToInt32(cbxPageIndex.SelectedItem.ToString());
            DgvListBind(pageIndex, num, ref count);
            ImgControl(pageIndex, count);
        }
        public void btnFirst_MouseClick(object sender, MouseEventArgs e)
        {
            pageIndex = 1;
            count = Convert.ToInt32(cbxPageIndex.SelectedItem.ToString()); 
            txtJumpPage.Text = pageIndex.ToString();
            num = Convert.ToInt32(cbxPageIndex.SelectedItem.ToString());
            ImgControl(pageIndex, count);
            DgvListBind(pageIndex, num, ref count);
        }

        public void btnRefresh_MouseClick(object sender, MouseEventArgs e)
        {
            DgvListBind(pageIndex, num, ref count);
            ImgControl(pageIndex, count);
        }

        private void btnJumpPage_MouseClick(object sender, MouseEventArgs e)
        {
            int index = 0;
            try
            {
                index = Convert.ToInt32(txtJumpPage.Text);
            }
            catch
            {
                throw;
            }
            num = Convert.ToInt32(cbxPageIndex.SelectedItem.ToString());
            int end = count % num == 0 ? count / num : count / num + 1;
            if (index > end)
            {
                index = end;
                txtJumpPage.Text = end.ToString();
            }
            if (index < 0)
            {
                index = 0;
                txtJumpPage.Text = "0";
            }
            pageIndex = index;
            DgvListBind(pageIndex, num, ref count);
            txtJumpPage.Text = pageIndex.ToString();
            DgvListBind(pageIndex, num, ref count);
            ImgControl(pageIndex, count);
        }
    }
}
