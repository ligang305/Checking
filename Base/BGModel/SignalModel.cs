using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BGModel
{
    public class SignalModel : BaseNotifyPropertyChanged
    {
        /// <summary>
        /// 通过这个信号，来判断当前 是背散的还是 
        /// 的
        /// </summary>
        private int _SignalSource { get; set; } = 0;
        /// <summary>
        /// 信号来源
        /// </summary>
        public int SignalSource
        {
            get { return _SignalSource; }
            set { _SignalSource = value; RaisePropertyChanged(new PropertyChangedEventArgs("SignalSource")); }
        }

        /// <summary>
        /// 通过这个信号，来判断当前 需要取的值是true 正常还是 false是正常
        /// </summary>
        private int _SignalType { get; set; } = (int)SignalModelTypeEnum.Label;
        /// <summary>
        /// 信号类型，显示图标还是文字
        /// </summary>
        public int SignalType
        {
            get { return _SignalType; }
            set { _SignalType = value; RaisePropertyChanged(new PropertyChangedEventArgs("SignalType")); }
        }
        /// <summary>
        /// 通过这个信号，来判断当前 需要取的值是true 正常还是 false是正常
        /// </summary>
        private string _SignalValue { get; set; }
        /// <summary>
        ///  通过这个信号，来判断当前 需要取的值是true 正常还是 false是正常
        /// </summary>
        public string SignalValue
        {
            get { return _SignalValue; }
            set { _SignalValue = value; RaisePropertyChanged(new PropertyChangedEventArgs("SignalValue")); }
        }

        /// <summary>
        /// 信号展现颜色
        /// </summary>
        private Brush _SignalColor { get; set; }

        public Brush SignalColor
        {
            get { return _SignalColor; }
            set { _SignalColor = value; RaisePropertyChanged(new PropertyChangedEventArgs("SignalColor")); }
        }

        /// <summary>
        /// 信号查询所在的位置
        /// </summary>
        public int SignalIndex { get; set; }

        /// <summary>
        /// 信号名称
        /// </summary>
        private string _SignalName { get; set; }
        /// <summary>
        ///  信号名称
        /// </summary>
        public string SignalName
        {
            get { return _SignalName; }
            set { _SignalName = value; RaisePropertyChanged(new PropertyChangedEventArgs("SignalName")); }
        }

        /// <summary>
        ///  需要异步查询的方法
        /// </summary>
        public Func<string> SearchAction { get; set; }
    }

    public enum SignalModelTypeEnum
    {
        Label,
        Icon,
        StartingDynamoIcon
    }
}
