using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace BGCommonOperation
{
    /// <summary>
    /// 车辆向前或向后的转换器
    /// </summary>
    public class CarPreOrBackConvert : IValueConverter
    {
        //当值从绑定源传播给绑定目标时，调用方法Convert
        //向前向后绑定值 0：向后；1向前
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(parameter as string == "0")
            {
                string Status = value as string;
                return Status == "1";
            }
            else
            {
                string Status = value as string;
                return Status == "0";
            }
        }
        //当值从绑定目标传播给绑定源时，调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter as string == "0")
            {
                bool isChecked = (bool)value;
                if (isChecked)
                {
                    return "1";
                }
                return "0";
            }
            else
            {
                bool isChecked = (bool)value;
                if (isChecked)
                {
                    return "0";
                }
                return "1";
            }
           
        }
    }


    /// <summary>
    /// 故障颜色修改
    /// </summary>
    public class HitchColorConvert : IValueConverter
    {
        //当值从绑定源传播给绑定目标时，调用方法Convert
        //向前向后绑定值 0：向后；1向前
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value as string == "0")
            {
                return Common.StrToBrush("#FF0000");
            }
            else
            {
                return Common.StrToBrush("#464646");
            }
        }
        //当值从绑定目标传播给绑定源时，调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
