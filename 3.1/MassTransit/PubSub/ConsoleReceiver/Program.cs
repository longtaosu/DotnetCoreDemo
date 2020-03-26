using ConsoleReceiver.Handlers;
using GreenPipes;
using MassTransit;
using System;

namespace ConsoleReceiver
{
    class Program
    {
        static void Main(string[] args)
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ReceiveEndpoint("log", ep =>
                {
                    ep.PrefetchCount = 16;
                    ep.UseMessageRetry(r => r.Interval(10, 100));

                    ep.Consumer<TestInfoConsumer>();
                });
            });
            busControl.Start();



            //try
            //{
            //    do
            //    {

            //        Console.WriteLine("Enter message (or quit to exit)");
            //        Console.Write("> ");
            //        string value = Console.ReadLine();

            //        if ("quit".Equals(value, StringComparison.OrdinalIgnoreCase))
            //            break;

            //        busControl.Publish<ValueEntered>(new ValueEntered
            //        {
            //            Value = value
            //        });

            //    } while (true);
            //}
            //finally
            //{
            //    await busControl.Stop();
            //}

            Console.ReadLine();
            //Console.WriteLine("Hello World!");
        }
    }
}
