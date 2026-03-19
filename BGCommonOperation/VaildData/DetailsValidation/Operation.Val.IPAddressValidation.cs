using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CMW.Common.Utilities.CommonDeleget;

namespace CMW.Common.Utilities
{
    public class IPAddressValidation : VaildData
    {
        public override string Vaild(string Value)
        {
            string ErrorMsg = string.Empty;
            if (string.IsNullOrWhiteSpace(Value))
            {
                ErrorMsg = UpdateStatusNameAction("ValueNotNull");
                return ErrorMsg;
            }

            if (!CommonFunc.IsAddress(Value))
            {
                ErrorMsg = UpdateStatusNameAction("IpAddressNotMatch");
                return ErrorMsg;
            }
            return ErrorMsg;
        }
    }
}
