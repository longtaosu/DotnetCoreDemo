using Lts.Models;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TopicDemo.Handlers;

namespace TopicDemo.Extensions
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

                    cfg.ReceiveEndpoint("lts111", ep =>
                    {
                        ep.Consumer<TestInfoConsumer1>(provider);

                        ep.Bind("test", x =>
                        {
                            x.RoutingKey = "*.black";
                            x.ExchangeType = ExchangeType.Topic;
                        });
                    });
                    
                    cfg.ReceiveEndpoint("lts222", ep =>
                    {
                        ep.ConfigureConsumer<TestInfoConsumer2>(provider);

                        ep.Bind("test", x =>
                        {
                            x.RoutingKey = "animal.*";
                            x.ExchangeType = ExchangeType.Topic;
                        });
                    });

                    cfg.Publish<TestInfo>(x =>
                    {
                        x.AlternateExchange = "test";
                        x.AutoDelete = true;
                        x.Durable = false;
                        x.ExchangeType = ExchangeType.Topic;
                    });
                }));
            });

            services.AddMassTransitHostedService();
        }
    }
}
