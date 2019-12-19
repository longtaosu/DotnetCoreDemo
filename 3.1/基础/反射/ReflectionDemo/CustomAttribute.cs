using System;
using System.Collections.Generic;
using System.Text;

namespace ReflectionDemo
{
    /// <summary>
    /// 用于属性的标注
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CustomAttribute:Attribute
    {
        public string Description { get; set; }

        public DateTime CreateTime { get; set; }

        public CustomAttribute(string description)
        {
            Description = description;
            CreateTime = DateTime.Now ;
        }
        public CustomAttribute(string description, DateTime time)
        {
            Description = description;
            CreateTime = time;
        }
    }
}
