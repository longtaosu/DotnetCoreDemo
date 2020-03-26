using GreenPipes;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Publisher.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Publisher.Extensions
{
    public static class MasstransitMQExtension
    {
        public static void ConfigureMQ(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<TestInfoConsumer>();

                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var host = cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ReceiveEndpoint("log", ep =>
                    {
                        //ep.Bind("test", cb =>
                        //{
                        //    cb.Durable = true;
                        //    cb.ExchangeType = "topic";
                        //    cb.RoutingKey = "post,#";
                        //});

                        ep.PrefetchCount = 16;
                        ep.UseMessageRetry(r => r.Interval(10,100));

                        ep.ConfigureConsumer<TestInfoConsumer>(provider);
                    });
                }));
            });

            services.AddMassTransitHostedService();
        }
    }
}
