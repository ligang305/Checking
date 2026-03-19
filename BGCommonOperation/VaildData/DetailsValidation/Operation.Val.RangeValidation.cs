using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CMW.Common.Utilities.CommonDeleget;

namespace CMW.Common.Utilities
{
    public class RangeValidation: VaildData
    {
        public string Min;
        public string Max;

        public override string Vaild(string Value)
        {
            string ErrorMsg = string.Empty;
            if (string.IsNullOrWhiteSpace(Value))
            {
                ErrorMsg = UpdateStatusNameAction("ValueNotNull");
                return ErrorMsg;
            }
            if (string.IsNullOrWhiteSpace(Min))
            {
                ErrorMsg = UpdateStatusNameAction("MinNotNull"); 
                return ErrorMsg;
            }
            if (string.IsNullOrWhiteSpace(Max))
            {
                ErrorMsg = UpdateStatusNameAction("MaxNotNull"); 
                return ErrorMsg;
            }
            double CheckValue = 0;
            if (!double.TryParse(Value, out CheckValue))
            {
                ErrorMsg = UpdateStatusNameAction("NotNum"); 
                return ErrorMsg;
            }
            double MaxValue = 0;
            if (!double.TryParse(Max, out MaxValue))
            {
                ErrorMsg = UpdateStatusNameAction("MaxNotNum");
                return ErrorMsg;
            }
            double MinValue = 0;
            if (!double.TryParse(Min, out MinValue))
            {
                ErrorMsg = UpdateStatusNameAction("MinNotNum");
                return ErrorMsg;
            }

            if (CheckValue < MinValue)
            {
                ErrorMsg = UpdateStatusNameAction("ValueNotInRange") + $"[{Min}-{Max}]"; 
                return ErrorMsg;
            }
            if (CheckValue > MaxValue)
            {
                ErrorMsg = UpdateStatusNameAction("ValueNotInRange") + $"[{Min}-{Max}]";
                return ErrorMsg;
            }

            return ErrorMsg;
        }
    }
}
