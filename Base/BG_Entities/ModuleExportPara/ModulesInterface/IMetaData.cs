using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BG_Entities
{
    public interface IMetaData
    {
        /// <summary>
        /// 优先级
        /// </summary>
        [DefaultValue(0)]
        int Priority { get; }
        /// <summary>
        /// 名称（不能重复）
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 描述
        /// </summary>
        string Description { get; }
        /// <summary>
        /// 作者
        /// </summary>
        string Author { get; }
        /// <summary>
        /// 版本
        /// </summary>
        string Version { get; }
    }
}
