using ConsoleModels;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace ConsoleConsumer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host("localhost");

                cfg.ReceiveEndpoint("customer_queue", e =>
                {
                    e.Consumer<UpdateCustomerConsumer>();
                });
            });


            await busControl.StartAsync();

            Console.ReadLine();
            //try
            //{
            //    do
            //    {
            //        string value = await Task.Run(() =>
            //        {
            //            Console.WriteLine("Enter message (or quit to exit)");
            //            Console.WriteLine("> ");
            //            return Console.ReadLine();
            //        });

            //        if ("q".Equals(value, StringComparison.OrdinalIgnoreCase))
            //            break;

            //        await busControl.Publish<ValueEntered>(new
            //        {
            //            Value = value
            //        }); 
            //    } while (true);
            //}
            //finally
            //{
            //    await busControl.StopAsync();
            //}
        }
    }




}
