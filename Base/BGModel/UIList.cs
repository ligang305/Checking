using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGModel
{
    public class UIList<T>: ObservableCollection<T> where T:class,new()
    {
        public void InsertAt(int index,T Item)
        {
            InsertItem(index, Item);
        }
    }
}
