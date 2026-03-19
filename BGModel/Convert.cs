
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using static CMW.Common.Utilities.CommonFunc;
using static CMW.Common.Utilities.CommonDeleget;

namespace BGModel
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
            return value.ToString().ToLower() ==  parameter.ToString().ToLower();// toin value.ToString() == parameter.ToString() ? true :false;
        }
        //当值从绑定目标传播给绑定源时，调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
    /// <summary>
    /// 显隐性反向的转换器
    /// </summary>
    public class VisibleConvert : IValueConverter
    {
        //当值从绑定源传播给绑定目标时，调用方法Convert
        //向前向后绑定值 0：向后；1向前
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = (Visibility)value;
            return visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
        //当值从绑定目标传播给绑定源时，调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
    /// <summary>
    /// 将图片上传的状态转化为文本
    /// </summary>
    public class LoadUploadImageConvert : IValueConverter
    {

        //当值从绑定源传播给绑定目标时，调用方法Convert
        //向前向后绑定值 0：向后；1向前
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return UpdateStatusNameAction("UnCommited");
            string val = value.ToString();
            return UpdateStatusNameAction(val);
            //switch (val)
            //{
            //    case "PutFTPFaild":
            //        return UpdateStatusNameAction("PutFTPFaild");
            //    case "CommitImageInfoFaild":
                    
            //    case "CommitFaild":
            //        return "提交失败";
            //    case "UpLoading":
            //        return "上传中";
            //    case "GetTaskIDing":
            //        return "正在获取任务ID";
            //    case "PutFTPing":
            //        return "正在推送至文件服务器";
            //    case "SubmitImageInfoing":
            //        return "正在提交图片信息";
            //    case "UploadComplete":
            //        return "上传完成";
            //    case "SubmitStatus":
            //        return "SubmitStatus";
            //    default:
            //        return "未提交";
                
            //}
            
        }
        //当值从绑定目标传播给绑定源时，调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    /// <summary>
    /// 故障颜色修改
    /// </summary>
    public class HitchLambColorConvert : IValueConverter
    {
        //当值从绑定源传播给绑定目标时，调用方法Convert
        //向前向后绑定值 0：向后；1向前
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string _vaule = value as string;
            if (_vaule == "0")
            {
                return "RedPoliceLight";
            }
            //(_vaule == "2")  
            else
            {
                return "GreenPoliceLight";    
            }
        }
        //当值从绑定目标传播给绑定源时，调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
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
            string _vaule = value as string;
            if (_vaule == "0")
            {
                return StrToBrush("#FF0000");
            }
            else if (_vaule == "1")
            {
                return StrToBrush("#464646");
            }
            //(_vaule == "2")
            else
            {
                return StrToBrush("#006600");
            }
        }
        //当值从绑定目标传播给绑定源时，调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// 故障颜色修改
    /// </summary>
    public class ColorConvert : IValueConverter
    {
        //当值从绑定源传播给绑定目标时，调用方法Convert
        //向前向后绑定值 0：向后；1向前
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string _vaule = value as string;
            if (_vaule.ToLower() == "false")
            {
                return StrToBrush("#FF0000");
            }
            else
            {
                return StrToBrush("#00FF00");
            }
        }
        //当值从绑定目标传播给绑定源时，调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
