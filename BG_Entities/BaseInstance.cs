using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BG_Entities
{
    public class BaseInstance<T> where T:class,new()
    {
        private static T _Instance;
        //private static readonly object _lock = new object();
        public static T GetInstance()
        {
            //lock (this)
            //{
                if (_Instance == null)
                {

                    if (_Instance == null)
                    {
                        _Instance = new T();
                    }
                }
            //}
            return _Instance;
        }
    }
}
