using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGModel
{
    public class VersionParamater
    {
        public string CurrentVersion { get; set; }
        public string ClientCode { get; set; }
        public string UpdateFilePath { get; set; }
        /// <summary>
        /// 主程序路径，关闭主程序使用
        /// </summary>
        public string MainApplicationPath { get; set; }
        /// <summary>
        /// 后台升级服务IP地址
        /// </summary>
        public string UgrServerIp { get; set; } = @"192.168.0.1";
        /// <summary>
        /// 语言
        /// </summary>
        public string Language { get; set; } = "zh-cn";
    }

    /// <summary>
    /// 版本验证-下发
    /// </summary>
    public class VersionInfo
    {
        public ServerSoftware serverSoftware { get; set; }

        public List<Description> Descriptions { get; set; }
    }


    /// <summary>
    /// 服务器软件信息
    /// </summary>
    public class ServerSoftware
    {
        /// <summary>
        /// 客户端类型 CMW;IOW等
        /// </summary>
        public string AppCode { get; set; }
        /// <summary>
        /// 名称监测控制站/检入检出站
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 当前版本
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 生产时间
        /// </summary>
        public string ReleaseTime { get; set; }
    }
    /// <summary>
    /// 更新的软件包
    /// </summary>
    public class Description
    {
        /// <summary>
        /// 更新包版本
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 更新时间 
        /// </summary>
        public string ReleaseTime { get; set; }
        /// <summary>
        /// 更新包内容
        /// </summary>
        public List<UpdateContent> UpdateContents { get; set; }
    }

    public class UpdateContent
    {
        public string UpdateDescription { get; set; }
    }
}
