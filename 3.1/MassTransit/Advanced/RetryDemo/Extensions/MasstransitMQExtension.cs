using GreenPipes;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using RetryDemo.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetryDemo.Extensions
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

                    cfg.ReceiveEndpoint("lts111", ep =>
                    {
                        ep.UseMessageRetry(r => r.Interval(3, 1500));

                        ep.Consumer<TestInfoConsumer>(provider);
                    });


                }));
            });

            services.AddMassTransitHostedService();
        }
    }
}
