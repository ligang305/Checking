using BG_Entities;
using BGModel;
using BGUserControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMW.ViewModel
{
    public partial class CMWWindowViewModel
    {
        public IUserControlView currentModule = null;

        bool _IsClick = false;
        public bool IsClicking
        {
            get { return _IsClick; }
            set
            {
                _IsClick = value;
            }
        }
    }
}
