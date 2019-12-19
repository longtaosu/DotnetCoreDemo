using System;
using System.Collections.Generic;
using System.Linq;

namespace ReflectionDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Info info = new Info() { Age = 16, Height = 120.0m, Name = "张三", Sex = true, Weight = 121.5m };
            BaseInfo bas = info as BaseInfo;
            //
            var name = info.GetType().Name;
            var fullName = info.GetType().FullName;
            var isGenericType = info.GetType().IsGenericType;
            var attributes = info.GetType().Attributes;
            var baseType = info.GetType().BaseType;
            var fields = info.GetType().GetFields();
            //var attributes = info.GetType().CustomAttributes;
            var customAttributes = info.GetType().GetCustomAttributes(false);
            var genericArguments = info.GetType().GetGenericArguments();

            var b_name = info.GetType().Name;
            var b_fullName = info.GetType().FullName;
            var b_isGenericType = info.GetType().IsGenericType;
            var b_attributes = info.GetType().Attributes;
            var b_baseType = info.GetType().BaseType;
            var b_fields = info.GetType().GetFields();
            var b_customAttributes = info.GetType().CustomAttributes;
            var b_genericArguments = info.GetType().GetGenericArguments();

            List<Info> lInfo = new List<Info> { new Info { Age = 16, Height = 45m, Name = "李四", Sex = false, Weight = 48m } };
            var test = lInfo.GetType().GetGenericArguments();


            //PropertyInfo 获取实体的信息
            var propertities = info.GetType().GetProperties();

            //获取所有的属性
            Console.WriteLine("获取所有属性及值");
            foreach (var item in propertities)
            {
                Console.WriteLine("字段名：{0}，字段值：{1}", item.Name, item.GetValue(info));
            }


            //获取自定义属性 PropertyInfo.Name：字段名，
            //               PropertyInfo.GetValue：字段值
            Console.WriteLine("\r\n获取所有的自定义属性");
            foreach (var item in propertities)
            {
                CustomAttribute attr = (CustomAttribute)item.GetCustomAttributes(typeof(CustomAttribute), true).FirstOrDefault();
                if (attr != null)
                    Console.WriteLine("字段：" + item.Name + "，描述信息为：" + attr.Description + "，值为：" + item.GetValue(info).ToString());
            }








            Console.ReadLine();
        }
    }


}
