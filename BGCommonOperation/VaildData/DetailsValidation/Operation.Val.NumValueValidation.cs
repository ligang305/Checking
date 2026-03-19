using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CMW.Common.Utilities.CommonDeleget;
namespace CMW.Common.Utilities
{
    public class NumValueValidation : VaildData
    {
        public override string Vaild(string Value)
        {
            string ErrorMsg = string.Empty;
            if (string.IsNullOrWhiteSpace(Value))
            {
                ErrorMsg = UpdateStatusNameAction("ValueNotNull");
                return ErrorMsg;
            }
            
            int CheckValue = 0;
            if (!int.TryParse(Value, out CheckValue))
            {
                ErrorMsg = UpdateStatusNameAction("NotNum"); 
                return ErrorMsg;
            }
            return ErrorMsg;
        }
    }
    public class DemicalValueValidation : VaildData
    {
        public override string Vaild(string Value)
        {
            string ErrorMsg = string.Empty;
            if (string.IsNullOrWhiteSpace(Value))
            {
                ErrorMsg = UpdateStatusNameAction("ValueNotNull");
                return ErrorMsg;
            }

            double CheckValue = 0;
            if (!double.TryParse(Value, out CheckValue))
            {
                ErrorMsg = UpdateStatusNameAction("NotNum");
                return ErrorMsg;
            }
            return ErrorMsg;
        }
    }
    public class FloatValueValidation : VaildData
    {
        public override string Vaild(string Value)
        {
            string ErrorMsg = string.Empty;
            if (string.IsNullOrWhiteSpace(Value))
            {
                ErrorMsg = UpdateStatusNameAction("ValueNotNull");
                return ErrorMsg;
            }

            float CheckValue = 0;
            if (!float.TryParse(Value, out CheckValue))
            {
                ErrorMsg = UpdateStatusNameAction("NotNum");
                return ErrorMsg;
            }
            return ErrorMsg;
        }
    }
}
