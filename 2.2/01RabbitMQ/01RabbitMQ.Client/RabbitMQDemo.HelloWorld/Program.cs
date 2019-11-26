using RabbitMQDemo.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQDemo.HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(() =>
            {
                Console.WriteLine("消费者");
                Customer customer = new Customer("localhost");
                customer.Receive("hello", (message) =>
                {
                    Thread.Sleep(3000);
                    Console.WriteLine(string.Format("{0}接收消息：{1}", DateTime.Now.ToString("HH:mm:ss"), message));
                });
            });

            //生产者发送消息
            while (true)
            {
                string message = Console.ReadLine();
                if (message == "exit")
                    break;
                Producer producer = new Producer("localhost");
                producer.Send("hello", "hello", message);
            }


            Console.ReadLine();
        }
    }
}
