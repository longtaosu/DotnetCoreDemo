using MassTransit;
using System;
using Topshelf;

namespace WindowsServiceDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            
        }
    }

    public class EventConsumerService : ServiceControl
    {
        IBusControl _bus;


        public bool Start(HostControl hostControl)
        {
            _bus = ConfigureBus();
            _bus.Start();

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _bus?.Stop(TimeSpan.FromSeconds(5));

            return true;
        }

        IBusControl ConfigureBus()
        {
            return Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host("localhost");

                cfg.ReceiveEndpoint("event_queue", e =>
                {
                    e.Handler<ValueEntered>(context => Console.Out.WriteLineAsync($"Value was entered: {context.Message.Value}"));
                });
            });
        }
    }

    public class ValueEntered
    {
        public string Value { get; set; }
    }
}
