using System;
using System.Reflection;
using System.Linq;

namespace _01Reflection
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("通过反射获取自定义属性");
            //通过反射获取自定义的属性
            TestMyAttribute();
            //通过反射获取枚举的定义
            TestMyEnum();
            //通过反射对比实体字段
            TestEntityCompare();

            Console.ReadLine();
        }

        #region 自定义属性
        /// <summary>
        /// 自定义属性测试
        /// </summary>
        /// <returns></returns>
        private static void TestMyAttribute()
        {
            MyEntity entity = new MyEntity
            {
                Property1 = "张三",
                Property2 = "哈哈"
            };
            //循环打印所有实体中相应字段的属性值
            PropertyInfo[] properties = entity.GetType().GetProperties();
            foreach (var item in properties)
            {
                MyAttribute attr = ReflectionHelper.GetAttribute<MyAttribute>(item);
                if (!(attr is null))
                {
                    Console.WriteLine(attr.Description);
                }
            }
            //获取指定实体类 MyEntity 的属性 MyAttribute
            PropertyInfo info = typeof(MyEntity).GetProperties().FirstOrDefault();
            MyAttribute test = ReflectionHelper.GetAttribute<MyAttribute,MyEntity>();
        }
        #endregion

        #region 枚举
        private static void TestMyEnum()
        {
            Type type = ReflectionHelper.GetTypeFromDLL(EEnums.ESex.ToString());
            string str = Enum.GetName(type, 1);
            Console.WriteLine(str);
        }

        private static void TestEntityCompare()
        {
            MyEntity entity1 = new MyEntity
            {
                Property1 = "1",
                Property2 = "3"
            };
            MyEntity entity2 = new MyEntity
            {
                Property1 = "2",
                Property2 = "4"
            };
            string str = ReflectionHelper.CompareModel<MyEntity>(entity1, entity2);
            Console.WriteLine(str);
        }
        #endregion

    }
}
