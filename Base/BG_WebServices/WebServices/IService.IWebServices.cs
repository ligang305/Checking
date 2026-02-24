using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG_Services
{
    public interface IWebServices
    {
        string GetWebServicesName();

        string UploadData(string Data);
    }
}
