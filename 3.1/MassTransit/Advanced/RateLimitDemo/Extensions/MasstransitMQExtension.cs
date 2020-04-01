using GreenPipes;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using RateLimitDemo.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RateLimitDemo.Extensions
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
    //ep.UseMessageRetry(r => r.Interval(3, 1500));

    ep.UseRateLimit(5, TimeSpan.FromSeconds(5));

    ep.Consumer<TestInfoConsumer>(provider);
});


                }));
            });

            services.AddMassTransitHostedService();
        }
    }
}
