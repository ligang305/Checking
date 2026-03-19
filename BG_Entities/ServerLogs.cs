using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG_Entities
{
    public struct ServerLogs
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string Equipment { get; set; }
        /// <summary>
        /// 日志级别 L2、L3、L99
        /// </summary>
        public string LogLevel { get; set; }
        /// <summary>
        /// 模块编号 001
        /// </summary>
        public string ModulesNo { get; set; }
        /// <summary>
        /// 日志编号 001
        /// </summary>
        public string LogNo { get; set; }
        /// <summary>
        /// 日志信息
        /// </summary>
        public string Log { get; set; }
        /// <summary>
        /// 获取日志编码 CMWL2001001
        /// </summary>
        /// <returns></returns>
        public string LogCode()
        {
            return $@"{Equipment}{LogLevel}{ModulesNo}{LogNo}";
        }
    }
    /// <summary>
    /// 服务器日志请求实体
    /// </summary>
    public class ServerLogsRequestModel:BaseRequestClientTypeWithToken
    {
        /// <summary>
        /// 请求时间
        /// </summary>
        public string Time { get; set; }
        /// <summary>
        /// 编码 类似于CMWL2001001
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 日志内容
        /// </summary>
        public string Data { get; set; }
    }
}
