using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BG_Services
{
    public class HttpException:Exception
    {
        public HttpException(string message) : base(message) { }

        public override string Message
        {
            get
            {
                return base.Message;
            }
        }
    }
}
