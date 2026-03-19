using BG_Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CMW.Common.Utilities
{
    /// <summary>
    /// 校验工厂类
    /// </summary>
    public class ValidationFactory: BaseInstance<ValidationFactory>
    {
        /// <summary>
        /// 校验方法
        /// </summary>
        /// <param name="ValidDataStr"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public List<VaildData> CreateVaildData(string ValidDataStr)
        {
            return AnslyValidData(ValidDataStr);
        }
        /// <summary>
        /// 具体校验逻辑
        /// </summary>
        /// <param name="ValidDatStr"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        private List<VaildData> AnslyValidData(string ValidDatStr)
        {
            string ValidResult = string.Empty;
            Assembly assembly = Assembly.GetExecutingAssembly();
            List<VaildData> vm = new List<VaildData>();
            string[] ValidData = ValidDatStr.TrimEnd(']').Trim('[').Split(';');
            string[] ValidModels = ValidData[0].Split(',');
            string[] ValidValues = new string[2];
            if (ValidData.Length >= 2)
            {
                ValidValues = ValidData[1].Split(',');
            }
            
            foreach (var ValidModelItem in ValidModels)
            {
                VaildData vdItem = assembly.CreateInstance( $"CMW.Common.Utilities.{ValidModelItem.Trim('(').Trim(')')}") as VaildData;
                if (vdItem is RangeValidation)
                {
                    if(ValidValues.Length != 2) break;
                    (vdItem as RangeValidation).Min = ValidValues[0].ToTrimSpecialStr();
                    (vdItem as RangeValidation).Max = ValidValues[1].ToTrimSpecialStr();
                }
                vm.Add(vdItem);
            }

            return vm;
        }

      
    }

    public static class StringSpecialTrim
    {
        public static string ToTrimSpecialStr(this string Str)
        {
            return Str.Trim('(').Trim(')').Trim('{').Trim('}').Trim(',').Trim('?').Trim('/').Trim('.').Trim(';')
                .Trim('"').Trim('|');
        }
    }

}
