using System;
using System.Threading;
using EasyNetQ;

namespace EasyNetQDemo.QuickStart_Sender
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("开始发送信息....");
            //Thread.Sleep(100);

            var bus = RabbitHutch.CreateBus("host=localhost");
            for (int i = 0; i < 10; i++)
            {

                bus.Publish(new TextMessage
                {
                    Text = i.ToString()
                });
            }

            Console.WriteLine("信息发送完毕....");
            Console.ReadLine();
        }
    }
}
