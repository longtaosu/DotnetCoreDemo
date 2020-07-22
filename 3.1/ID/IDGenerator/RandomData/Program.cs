using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RandomData
{
    class Program
    {
        private static object oo = new object();
        static void Main(string[] args)
        {
            DateTime timeBegin = DateTime.Now;

            List<string> data = new List<string>();
            for (int i = 0; i < 10000; i++)
            {
                var id = GetID();
                data.Add(id);
            }
            DateTime timeEnd = DateTime.Now;
            var span = timeEnd.Subtract(timeBegin);
            Console.WriteLine($"共产生数字：{data.Count} 个");
            Console.WriteLine($"去重后数字有：{data.Distinct().Count()} 个");
            Console.WriteLine($"共耗时{span.TotalSeconds}");

            Console.WriteLine("模拟并发请求");
            data.Clear();
            timeBegin = DateTime.Now;
            for (int i = 0; i < 1000; i++)
            {
                Thread.Sleep(200);
                for (int j = 0; j < 10; j++)
                {
                    Task.Run(() =>
                    {
                        var id = GetID();
                        //Console.WriteLine($"时间：{DateTime.Now.ToString("yyyyMMddHHmmss")}，id:{id}");
                        data.Add(id);
                    }).Wait();
                }
            }

            
            span = DateTime.Now.Subtract(timeBegin);
            Console.WriteLine($"共产生数字：{data.Count()} 个");
            Console.WriteLine($"去重后数字有：{data.Distinct().ToList().Count()} 个");
            Console.WriteLine($"共耗时{span.TotalSeconds}");

            Console.ReadLine();
        }

        private static string GetID()
        {
            
            string id = string.Empty;
            lock(oo)
            {
                id = DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 9999).ToString();
            }
            return id;
        }
    }
}
