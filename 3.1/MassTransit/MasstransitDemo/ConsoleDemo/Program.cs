using MassTransit;
using System;
using System.Threading.Tasks;

namespace ConsoleDemo
{
    class Program
    {
        static async void Main(string[] args)
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg => cfg.Host("localhost"));

            await busControl.StartAsync();
            try
            {
                do
                {
                    string value = await Task.Run(() =>
                    {
                        Console.WriteLine("Enter message (or quit to exit)");
                        Console.Write("> ");
                        return Console.ReadLine();
                    });

                    if ("quit".Equals(value, StringComparison.OrdinalIgnoreCase))
                        break;

                    await busControl.Publish<ValueEntered>(new ValueEntered
                    {
                        Value = value
                    });

                } while (true);
            }
            finally
            {
                await busControl.StopAsync();
            }


            Console.ReadLine();
        }
    }

    public class ValueEntered
    {
        public string Value { get; set; }
    }
}
