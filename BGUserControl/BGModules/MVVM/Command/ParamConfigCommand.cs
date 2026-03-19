using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BGUserControl
{
    /// <summary>
    /// the command without input parameter
    /// </summary>

    public class ParamConfigCommand : ICommand
    {
        public event EventHandler CanExecuteChanged = (sender, e) => { };
        private Action<object> mActionWithParameter;
        public ParamConfigCommand(Action<object> action)
        {
            this.mActionWithParameter = action;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {
            mActionWithParameter.Invoke(parameter);
        }
    }
}
