using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGCommonOperation
{
   [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited = true)]
    public class BaseAttribute:Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }

        private bool _IsInsertDB = true;
        private bool _IsShowTitle = false;
        private bool _IsPrimaryKey = false;
        public bool IsInsertDB 
        {
            get { return _IsInsertDB; }
            set { _IsInsertDB = value; } 
        }
        public bool IsShowTitle
        {
            get { return _IsShowTitle; }
            set { _IsShowTitle = value; }
        }
        public bool IsPrimaryKey
        {
            get { return _IsPrimaryKey; }
            set { _IsPrimaryKey = value; }
        }
    }
}
