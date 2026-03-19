using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGModel
{
   [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited = true)]
    public class BaseAttribute:Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
        private bool _IsUniqueKey = false;
        private bool _IsInsertDB = true;
        private bool _IsShowTitle = false;
        private bool _IsPrimaryKey = false;
        private bool _IsSearch = false;

        /// <summary>
        /// 是否是唯一的值
        /// </summary>
        public bool IsUniqueKey
        {
            get { return _IsUniqueKey; }
            set { _IsUniqueKey = value; }
        }
        /// <summary>
        /// 是否插入到DB库
        /// </summary>
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
        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsPrimaryKey
        {
            get { return _IsPrimaryKey; }
            set { _IsPrimaryKey = value; }
        }
        /// <summary>
        /// 是否是查询条件
        /// </summary>
        public bool IsSearch
        {
            get { return _IsSearch; }
            set { _IsSearch = value; }
        }
    }



    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited = true)]
    public class DataGridViewAttribute : Attribute
    {
        private double _Width = 200;
        private bool _IsShow = true;
        private string _ColumnBindingName = string.Empty;
        private string _ColumnDisplayName = string.Empty;

        /// <summary>
        /// 宽度
        /// </summary>
        public double Width
        {
            get { return _Width; }
            set { _Width = value; }
        }
        /// <summary>
        /// 是否显示到界面
        /// </summary>
        public bool IsShow
        {
            get { return _IsShow; }
            set { _IsShow = value; }
        }
        /// <summary>
        /// 界面绑定值
        /// </summary>
        public string ColumnBindingName
        {
            get { return _ColumnBindingName; }
            set { _ColumnBindingName = value; }
        }

        /// <summary>
        /// 界面显示值
        /// </summary>
        public string ColumnDisplayName
        {
            get { return _ColumnDisplayName; }
            set { _ColumnDisplayName = value; }
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited = true)]
    public class TimeAttribute : Attribute
    {
        private bool _IsStartTime = false;
        private bool _IsEndTime = false;

        /// <summary>
        /// 是否最小时间
        /// </summary>
        public bool IsStartTime
        {
            get { return _IsStartTime; }
            set { _IsStartTime = value; }
        }
        /// <summary>
        /// 是否查询最大时间
        /// </summary>
        public bool IsEndTime
        {
            get { return _IsEndTime; }
            set { _IsEndTime = value; }
        }
    }
}
