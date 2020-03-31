using FanoutDemo.Handlers;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FanoutDemo.Extensions
{
    public static class MasstransitMQExtension
    {
    public static void ConfigureMQ(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<TestInfoConsumer1>();
            x.AddConsumer<TestInfoConsumer2>();

            x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });                    

                cfg.ReceiveEndpoint("log111111", ep =>
                {
                    ep.PrefetchCount = 1;
                    ep.UseMessageRetry(r => r.Interval(10, 100));

                    ep.Consumer<TestInfoConsumer1>(provider);
                });

                cfg.ReceiveEndpoint("log22222", ep =>
                {
                    ep.PrefetchCount = 1;
                    ep.UseMessageRetry(r => r.Interval(10, 100));

                    ep.ConfigureConsumer<TestInfoConsumer2>(provider);
                });
            }));
        });

        services.AddMassTransitHostedService();
    }
    }
}
