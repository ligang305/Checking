using BG_Services;
using CMW.Common.Utilities;
using BGModel;
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
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonDeleget;
using BG_WorkFlow;
using BG_Entities;
using Section = BG_Entities.Section;

namespace BGUserControl
{
    /// <summary>
    /// LanguageComboxCtl.xaml 的交互逻辑
    /// </summary>
    public partial class LanguageComboxCtl : UserControl
    {
        public LanguageComboxCtl()
        {
            InitializeComponent();
            InitLanguage();
        }

        public void InitLanguage()
        {
            ddlLanguage.ItemsSource = LanguageServices.GetInstance().LanguageList;
            ddlLanguage.DisplayMemberPath = "languageName";
            ddlLanguage.SelectedValuePath = "LanguageKey";
            ddlLanguage.SelectionChanged -= DdlLanguage_SelectionChanged;
            ddlLanguage.SelectionChanged += DdlLanguage_SelectionChanged;
            try
            {
                foreach (var item in ddlLanguage.Items)
                {
                    if ((item as LanguageModel).LanguageKey == ConfigServices.GetInstance().localConfigModel.LANGUAGE)
                    {
                        ddlLanguage.SelectedItem = item;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLogAction(ex.Message, LogType.ApplicationError, true);
            }
        }

        private void DdlLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox mi = sender as ComboBox;
            CommonDeleget.UpdateConfigs("language", mi.SelectedValue as string, Section.SOFT);
            ConfigServices.GetInstance().localConfigModel.LANGUAGE = mi.SelectedValue as string;
            BuryingPoint($"{UpdateStatusNameAction("SwitchLanguage")}:{ConfigServices.GetInstance().localConfigModel.LANGUAGE}");
            if (ButtonInvoke.SwitchLanguageEvent != null &&
                ButtonInvoke.SwitchLanguageEvent.GetInvocationList().Count() != 0)
            {
                ButtonInvoke.SwitchLanguageEvent?.Invoke(mi.SelectedValue as string);
            }
        }
    }
}
