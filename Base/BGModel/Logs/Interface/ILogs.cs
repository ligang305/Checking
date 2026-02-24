using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGModel
{
    public interface ILogs
    {
        string LogsName();
        string Logs();
        string LogsDataTime();
        string LogsType();
        string WriteLogs();
        string ReadLogs();
    }
}
