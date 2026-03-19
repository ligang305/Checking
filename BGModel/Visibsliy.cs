using BGModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BGModel
{
    public class Visibsliy : BaseNotifyPropertyChanged
    {
        private Visibility isShow = Visibility.Visible;
        public Visibility IsShow
        {
            get
            {
                return isShow;
            }
            set
            {
                isShow = value;
                RaisePropertyChanged("IsShow");
            }
        }

        private string displayName = string.Empty;
        public string DisplayName
        {
            get
            {
                return displayName;
            }
            set
            {
                displayName = value;
                RaisePropertyChanged("DisplayName");
            }
        }

        private BitmapImage imageBitmapSource;
        public BitmapImage ImageBitmapSource
        {
            get
            {
                return imageBitmapSource;
            }
            set
            {
                imageBitmapSource = value;
                RaisePropertyChanged("ImageBitmapSource");
            }
        }

        private string status = string.Empty;
        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                RaisePropertyChanged("Status");
            }
        }

        private Thickness margin = new Thickness(0);
        public Thickness Margin
        {
            get
            {
                return margin;
            }
            set
            {
                margin = value;
                RaisePropertyChanged("Margin");
            }
        }

        private Stretch _streach;
        public Stretch Streath
        {
            get
            {
                return _streach;
            }
            set
            {
                _streach = value;
                RaisePropertyChanged("Streath");
            }
        }
    }
    /// <summary>
    /// 登录model
    /// </summary>
    public class LoginModel : BaseNotifyPropertyChanged
    {
        private string _UserName = string.Empty;
        private string _Password = string.Empty;
        private string _License = string.Empty;
        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; RaisePropertyChanged("UserName"); }
        }
        public string Password
        {
            get { return _Password; }
            set { _Password = value; RaisePropertyChanged("Password"); }
        }

        public string License
        {
            get { return _License; }
            set { _License = value; RaisePropertyChanged("License"); }
        }
    }

    public class ImageVm : BaseNotifyPropertyChanged
    {
        private BitmapSource bitmapSource = null;
        public BitmapSource _BitmapSource
        {
            get { return bitmapSource; }
            set {
                bitmapSource = value;
                RaisePropertyChanged("_BitmapSource");
            }
        }
    }


    public class Armlgb : BaseNotifyPropertyChanged
    {
        private LinearGradientBrush _AroundArm = null;
        public LinearGradientBrush AroundArm
        {
            get
            {
                return _AroundArm;
            }
            set
            {
                _AroundArm = value;
                RaisePropertyChanged("AroundArm");
            }
        }

        private LinearGradientBrush _MoveArm = null;
        public LinearGradientBrush MoveArm
        {
            get
            {
                return _MoveArm;
            }
            set
            {
                _MoveArm = value;
                RaisePropertyChanged("MoveArm");
            }
        }
    }

}
