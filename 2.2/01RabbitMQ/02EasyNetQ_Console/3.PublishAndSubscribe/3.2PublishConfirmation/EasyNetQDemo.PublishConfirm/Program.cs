using EasyNetQ;
using System;

namespace EasyNetQDemo.PublishConfirm
{
    class Program
    {
        static void Main(string[] args)
        {
            var bus = RabbitHutch.CreateBus("host=localhost;publisherConfirms=true;timeout=10");

            while (true)
            {
                string msg = Console.ReadLine();
                CardPaymentRequestMessage message = new CardPaymentRequestMessage
                {
                    Amount = DateTime.Now.Second,
                    CardHolderName = msg,
                    CardNumber = DateTime.Now.Millisecond.ToString(),
                    ExpiryDate = DateTime.Now.Hour.ToString()
                };
                Publish(bus, message);
            }


        }
        public static void Publish(IBus bus, CardPaymentRequestMessage message)
        {
            bus.PublishAsync(message).ContinueWith(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                {
                    Console.WriteLine("Task completed and not faulted.");
                }
                if (task.IsFaulted)
                {
                    Console.WriteLine("\n\n");
                    Console.WriteLine(task.Exception);
                    Console.WriteLine("\n\n");
                }
            });
        }
    }
}
