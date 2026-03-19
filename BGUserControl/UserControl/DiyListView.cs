using CMW.Common.Utilities;
using BGModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static CMW.Common.Utilities.CommonDeleget;

namespace BGUserControl
{
    public class DiyListView<T> : BaseWindows where T:class,new()
    {
        StackPanel sp = new StackPanel()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Orientation = Orientation.Horizontal
        };
        ListView lv;
        ObservableCollection<T> TList = new ObservableCollection<T>();
        public DiyListView(ObservableCollection<T> IList)
        {
            TList = IList;
            InitContent();
            ButtonInvoke.SwitchLanguageEvent -= SwitchLanguage;
            ButtonInvoke.SwitchLanguageEvent += SwitchLanguage;
            ButtonInvoke.SwitchFontSizeEvent -= SwitchFontSize;
            ButtonInvoke.SwitchFontSizeEvent += SwitchFontSize;
        }

        private void InitContent()
        {
            Content = sp;
            sp.Children.Add(MakeLiv(TList));
        }
        

        /// <summary>
        /// 切换字体
        /// </summary>
        /// <param name="Language"></param>
        private void SwitchFontSize(string Language)
        {
            InitContent();
        }
        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="Language"></param>
        private void SwitchLanguage(string Language)
        {
            Base_SwitchLanguage(Language);
            InitContent();
            //DiyPreviewOrBack_Loaded(null, null);
        }
        public ListView MakeLiv(ObservableCollection<T> IList)
        {
            lv = new ListView()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(0, 0, 30, 0),
                ItemContainerStyle = (Style)this.TryFindResource("diyListImage")
            };
            GridView gv = new GridView() { AllowsColumnReorder = false};
            gv.ColumnHeaderContainerStyle = (Style)this.TryFindResource("diyGridViewHeader");
            AddHeadColumn<T>(gv);
            //gv.Columns.Add(new GridViewColumn() { DisplayMemberBinding = new Binding ("IMAGE_NAME") { }, Width = 200, Header = UpdateStatusNameAction("ImageName") });
            //gv.Columns.Add(new GridViewColumn() { DisplayMemberBinding = new Binding("IAMGE_LOCALFILE_CREATETIME") { }, Width = 200, Header = UpdateStatusNameAction("ImageCreateTime") });
            //gv.Columns.Add(new GridViewColumn() { DisplayMemberBinding = new Binding("IMAGE_UPDATETIME") { }, Width = 200, Header = UpdateStatusNameAction("ImageUploadTime") });
            //gv.Columns.Add(new GridViewColumn() { DisplayMemberBinding = new Binding("IMAGE_UPLOAD_STATUS") { }, Width = 200, Header = UpdateStatusNameAction("SubmitStatus") });
            lv.View = gv;
            return lv;
        }
        /// <summary>
        /// 通过动态列配置列头
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Parentgv"></param>
        private void AddHeadColumn<T>(GridView Parentgv)
        {
            var TType = typeof(T);
            var Props = TType.GetProperties();
            foreach (var property in Props)
            {
                var propertyColumn = Attribute.GetCustomAttribute(property, typeof(DataGridViewAttribute)) as DataGridViewAttribute;
                if (propertyColumn == null) continue;
                if (propertyColumn.IsShow)
                {
                    Parentgv.Columns.Add(new GridViewColumn() { DisplayMemberBinding = new Binding(propertyColumn.ColumnBindingName) { }, 
                        Width = propertyColumn.Width, Header = UpdateStatusNameAction(propertyColumn.ColumnDisplayName) });
                }
            }
        }


        public void Reflash(ObservableCollection<T> IList)
        {
            TList = IList;
            lv.ItemsSource = TList;
        }
    }
}
