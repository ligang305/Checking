using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGModel
{
    /// <summary>
    /// 分页查询用的通用Model层
    /// zhuzhiwu 2019/5/24
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ParamaterModel<T> where T:class,new()
    {
        public ParamaterModel()
        {
            Model = new T();
        }
        public ParamaterModel(T _Model)
        {
            Model = _Model;
        }
        public int start { get; set; }

        public int num { get; set; }

        public T Model { get; set; }
    }
}
