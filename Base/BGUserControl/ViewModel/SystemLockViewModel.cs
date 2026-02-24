using BGModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BGUserControl
{
    public class SystemLockViewModel : BaseNotifyPropertyChanged
    {
        private Visibility isMaskVisible = Visibility.Visible;
        public Visibility IsMaskVisible
        {
            get
            {
                return isMaskVisible;
            }
            set
            {
                isMaskVisible = value;
                RaisePropertyChanged("IsMaskVisible");
            }
        }

        private string isMaskCode = string.Empty;
        public string MaskCode
        {
            get { return isMaskCode; }
            set
            {
                isMaskCode = value;
                RaisePropertyChanged("MaskCode");
            }
        }
    }
}
