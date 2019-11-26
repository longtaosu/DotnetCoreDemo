using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace _01Reflection
{
    public static class ReflectionHelper
    {
        /// <summary>
        /// 获取字段的自定义属性 T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(PropertyInfo prop)
            where T:Attribute
        {
            T attr = (T)prop.GetCustomAttributes(true)
                            .Where(t => t is T)
                            .FirstOrDefault() as T;
            return attr;
        }
        /// <summary>
        /// 获取类 N 的自定义属性 T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="N"></typeparam>
        /// <returns></returns>
        public static T GetAttribute<T,N>()
            where T : Attribute
        {
            T attr = typeof(N).GetCustomAttributes(true)
                            .Where(t => t is T)
                            .FirstOrDefault() as T;
            return attr;
        }
        /// <summary>
        /// 通过枚举名字从指定dll加载
        /// </summary>
        /// <param name="EnumName"></param>
        /// <param name="dll"></param>
        /// <returns></returns>
        public static Type GetTypeFromDLL(string EnumName,string dll="")
        {
            if(string.IsNullOrEmpty(dll))
            {
                dll = Assembly.GetExecutingAssembly().Location;
            }
            Type type = Assembly.LoadFrom(dll)
                                .GetTypes()
                                .Where(t => t.FullName.Contains(EnumName))
                                .FirstOrDefault();
            return type;
        }
        /// <summary>
        /// 对比同一个实体的两个类字段的不同
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oldModel"></param>
        /// <param name="newModel"></param>
        /// <returns></returns>
        public static string CompareModel<T>(T oldModel, T newModel)
            where T : class
        {
            string result = "";

            PropertyInfo[] properties = oldModel.GetType().GetProperties();
            foreach (PropertyInfo item in properties)
            {
                object oldValue = item.GetValue(oldModel);
                object newValue = item.GetValue(newModel);
                //排除两种相同的情况
                if (oldValue is null && newValue is null)
                    continue;
                if (!(oldValue is null) && oldValue.Equals(newValue))
                    continue;
                //以后的均为不等，但oldValue可能为空
                else
                {
                    MyAttribute info = GetAttribute<MyAttribute>(item);
                    if (string.IsNullOrEmpty(info.Description))
                        continue;
                    string oldStr = oldValue is null ? "" : oldValue.ToString();
                    string newStr = newValue is null ? "" : newValue.ToString();
                    result += string.Format("{0}({1})由 {2} 变为 {3};", info.Description, item.Name, oldStr, newStr);
                }
            }
            return result;
        }
    }
}
