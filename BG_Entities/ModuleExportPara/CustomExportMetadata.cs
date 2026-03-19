using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace BG_Entities
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CustomExportMetadata : ExportAttribute, IMetaData
    {
        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { get; private set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; private set; }
        /// <summary>
        /// 作者
        /// </summary>
        public string Author { get; private set; }
        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; private set; }

        public string UId { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CustomExportMetadata() : base(typeof(IMetaData))
        {
            this.UId = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 构造重载
        /// </summary>
        /// <param name="priority">优先级</param>
        public CustomExportMetadata(int priority) : this()
        {
            this.Priority = priority;
        }

        // <summary>
        /// 构造重载
        /// </summary>
        /// <param name="priority">优先级</param>
        /// <param name="name">名称</param>
        public CustomExportMetadata(int priority, string name) : this(priority)
        {
            this.Name = name;
        }

        /// <summary>
        /// 构造重载
        /// </summary>
        /// <param name="priority">优先级</param>
        /// <param name="name">名称</param>
        /// <param name="description">描述</param>
        public CustomExportMetadata(int priority, string name, string description) : this(priority, name)
        {
            this.Description = description;
        }

        /// <summary>
        /// 构造重载
        /// </summary>
        /// <param name="priority">优先级</param>
        /// <param name="name">名称</param>
        /// <param name="description">描述</param>
        /// <param name="author">作者</param>
        public CustomExportMetadata(int priority, string name, string description, string author) : this(priority, name, description)
        {
            this.Author = author;
        }

        /// <summary>
        /// 构造重载
        /// </summary>
        /// <param name="priority">优先级</param>
        /// <param name="name">名称</param>
        /// <param name="description">描述</param>
        /// <param name="author">作者</param>
        /// <param name="version">版本</param>
        public CustomExportMetadata(int priority, string name, string description, string author, string version) : this(priority, name, description, author)
        {
            this.Version = version;
        }
    }
}
