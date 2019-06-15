using System;
using System.Threading;
using EasyNetQ;

namespace EasyNetQDemo.QuickStart_Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(2000);
            var bus = RabbitHutch.CreateBus("host=localhost");
            bus.Subscribe<TextMessage>("test", HandleTextMessage);

            Console.ReadLine();
        }

        static void HandleTextMessage(TextMessage msg)
        {
            Console.WriteLine(msg.Text);
        }
    }
}
