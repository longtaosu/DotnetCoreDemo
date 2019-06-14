using RabbitMQDemo.Common;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQDemo.Topics
{
    /// <summary>
    /// 测试：
    /// 发送：charge.*，c1处理
    /// 发送：*.error，c2处理
    /// 发送：charge.error，c1、c2都会处理
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(() =>
            {
                Console.WriteLine("消费者1");
                Customer customer = new Customer("localhost");
                customer.Receive_Topics("hello", (message) =>
                {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    int count = message.Split('.').Length - 1;

                    Thread.Sleep(1000 * count);

                    Console.WriteLine(string.Format("线程1，{0}接收消息：{1}", DateTime.Now.ToString("HH:mm:ss"), message));

                    watch.Stop();
                    Console.WriteLine(string.Format("线程1，操作耗时：{0}秒", watch.Elapsed.Seconds));
                }, "charge.*");
            });
            Task.Run(() =>
            {
                Console.WriteLine("消费者2");
                Customer customer = new Customer("localhost");
                customer.Receive_Topics("hello", (message) =>
                {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    int count = message.Split('.').Length - 1;

                    Thread.Sleep(1000 * count);

                    Console.WriteLine(string.Format("线程2，{0}接收消息：{1}", DateTime.Now.ToString("HH:mm:ss"), message));

                    watch.Stop();
                    Console.WriteLine(string.Format("线程2，操作耗时：{0}秒", watch.Elapsed.Seconds));
                }, "*.error");
            });

            //生产者发送消息
            while (true)
            {
                string message = Console.ReadLine();
                if (message == "exit")
                    break;

                Producer producer = new Producer("localhost");
                producer.Send_Topics("hello", "hello", message);
            }


            Console.ReadLine();
        }
    }
}
