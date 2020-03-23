using System;
using System.Threading;
using System.Threading.Tasks;
using CSRedis;

namespace ConsoleDemo
{
    class Program
    {
        private static string key = "time";
        static void Main(string[] args)
        {
            RedisHelper.Initialization(new CSRedis.CSRedisClient("127.0.0.1:6379"));
            RedisHelper.Publish(key, DateTime.Now.ToString());
            RedisHelper.Subscribe((key,msg => Console.WriteLine(msg.Body)));
            Console.ReadLine();
        }


    }
}
