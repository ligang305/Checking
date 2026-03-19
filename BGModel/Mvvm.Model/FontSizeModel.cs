using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGModel
{
    public class FontConfigModel : BaseNotifyPropertyChanged
        {
            private double _small;
            public double small
            {
                get => _small;
                set
                {
                    _small = value;
                    RaisePropertyChanged("small");
                }
            }
            private double _normal;
            public double normal
            {
                get => _normal;
                set
                {
                    _normal = value;
                    RaisePropertyChanged("normal");
                }
            }
            private double _normalMiddle;
            public double normalMiddle
            {
                get => _normalMiddle;
                set
                {
                    _normalMiddle = value;
                    RaisePropertyChanged("normalMiddle");
                }
            }
            private double _middle;
            public double middle
            {
                get => _middle;
                set
                {
                    _middle = value;
                    RaisePropertyChanged("middle");
                }
            }

            private double _big;
            public double big
            {
                get => _big;
                set
                {
                    _big = value;
                    RaisePropertyChanged("big");
                }
            }

            private double _superBig;
            public double superBig
            {
                get => _superBig;
                set
                {
                    _superBig = value;
                    RaisePropertyChanged("superBig");
                }
            }
        }
}
