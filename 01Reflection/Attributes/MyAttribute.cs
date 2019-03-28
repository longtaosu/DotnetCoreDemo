using System;
using System.Collections.Generic;
using System.Text;

namespace _01Reflection
{
    /// <summary>
    /// 自定义属性
    /// </summary>
    public class MyAttribute : Attribute
    {
        #region 属性
        /// <summary>
        /// 姓名
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 年龄
        /// </summary>
        public int? Value { get; set; }
        #endregion
        #region 构造函数
        public MyAttribute(string description,int value)
        {
            this.Description = description;
            this.Value = value;
        }
        public MyAttribute(string name)
        {
            this.Description = name;
        }
        #endregion
    }
}
