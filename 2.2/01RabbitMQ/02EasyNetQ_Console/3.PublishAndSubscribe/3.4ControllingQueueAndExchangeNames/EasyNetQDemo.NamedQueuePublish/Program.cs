using EasyNetQDemo.Common;
using EasyNetQ;
using System;

namespace EasyNetQDemo.NamedQueuePublish
{
    class Program
    {
        static void Main(string[] args)
        {
            while(true)
            {
                var msg = Console.ReadLine();
                var payment = new CardPaymentNamedQueue
                {
                    Amount = DateTime.Now.Second,
                    CardHolderName = msg,
                    CardNumber = DateTime.Now.Year.ToString(),
                    ExpiryDate = DateTime.Now.Millisecond.ToString()
                };
                var bus = RabbitHutch.CreateBus("host=localhost");
                bus.Publish(payment);
            }
        }
    }
}
