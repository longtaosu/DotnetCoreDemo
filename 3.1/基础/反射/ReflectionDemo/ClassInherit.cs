using System;
using System.Collections.Generic;
using System.Text;

namespace ReflectionDemo
{
    /// <summary>
    /// 测试类的继承
    /// </summary>
    class ClassInherit
    {
    }
    public class BaseInfo
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public bool Sex { get; set; }
    }

    public class Info : BaseInfo
    {
        [Custom("身高")]
        public decimal Height { get; set; }

        public decimal Weight { get; set; }
    }
}
