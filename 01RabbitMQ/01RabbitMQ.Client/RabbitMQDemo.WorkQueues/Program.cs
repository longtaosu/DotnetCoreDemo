using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQDemo.Common;

namespace RabbitMQDemo.WorkQueues
{
    class Program
    {
        static void Main(string[] args)
        {
            Task task = Task.Run(() =>
            {
                Console.WriteLine("消费者");
                Customer customer = new Customer("localhost");
                customer.Receive_WorkQueues("hello", (message) =>
                {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    int count = message.Split('.').Length - 1;

                    Thread.Sleep(1000 * count);

                    Console.WriteLine(string.Format("线程1，{0}接收消息：{1}", DateTime.Now.ToString("HH:mm:ss"), message));

                    watch.Stop();                    
                    Console.WriteLine(string.Format("线程1，操作耗时：{0}秒",watch.Elapsed.Seconds));
                });
            });
            Task.Run(() =>
            {
                Console.WriteLine("消费者");
                Customer customer = new Customer("localhost");
                customer.Receive_WorkQueues("hello", (message) =>
                {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    int count = message.Split('.').Length - 1;

                    Thread.Sleep(1000 * count);

                    Console.WriteLine(string.Format("线程2，{0}接收消息：{1}", DateTime.Now.ToString("HH:mm:ss"), message));

                    watch.Stop();
                    Console.WriteLine(string.Format("线程2，操作耗时：{0}秒", watch.Elapsed.Seconds));
                });
            });

            //生产者发送消息
            while (true)
            {
                string message = Console.ReadLine();
                if (message == "exit")
                    break;
                    
                Producer producer = new Producer("localhost");
                producer.Send_WorkQueues("hello", "hello", message);
            }


            Console.ReadLine();
        }
    }
}
