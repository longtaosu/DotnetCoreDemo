using IdGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SnowFlake
{
    class Program
    {
        static void Main(string[] args)
        {
            List<long> data = new List<long>();

            DateTime timeBegin = DateTime.Now;
            //var generator = new IdGenerator(0);

            var epoch = new DateTime(2020, 7, 22, 0, 0, 0, DateTimeKind.Utc);
            var structure = new IdStructure(45, 2, 16);
            var options = new IdGeneratorOptions(structure, new DefaultTimeSource(epoch));
            var generator = new IdGenerator(0, options);
            for (int i = 0; i < 100000000; i++)
            {
                var id = generator.CreateId();
                data.Add(id);
                //Console.WriteLine($"生成id：{id}");
            }
            DateTime timeEnd = DateTime.Now;
            TimeSpan span = timeEnd.Subtract(timeBegin);

            data.Add(data.Last());
            Console.WriteLine($"添加一个测试数据后：{data.Count}");
            data = data.Distinct().ToList();
            Console.WriteLine($"取出重复数据后：{data.Count}");
            Console.WriteLine($"耗时：{span.TotalSeconds}");
            Console.WriteLine($"平均每秒产生：{data.Count / span.TotalSeconds}");

            Console.ReadLine();




        }
    }
}
