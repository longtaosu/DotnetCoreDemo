using DirectDemo.Handlers;
using GreenPipes;
using Lts.Models;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectDemo.Extensions
{
    /// <summary>
    /// https://github.com/MassTransit/MassTransit/blob/6aeb5acfdb1f1cf308c16128713a86692a8ea9f1/docs/advanced/topology/rabbitmq.md
    /// 
    /// https://github.com/ambarprajapati/myMassTransit/blob/1f6a8f0509698705a8d892dd10e3cc2a96e14ed3/docs/advanced/topology/rabbitmq/binding.md
    /// </summary>
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

                    //cfg.PublishTopology.BrokerTopologyOptions = MassTransit.RabbitMqTransport.Topology.PublishBrokerTopologyOptions.MaintainHierarchy;

                    cfg.ReceiveEndpoint("tls1234567", ep =>
                    {
                        //ep.ConfigureConsumeTopology = false;
                        //ep.BindMessageExchanges = false;

                        //ep.ExchangeType = ExchangeType.Direct;

                        //ep.PrefetchCount = 1;
                        //ep.UseMessageRetry(r => r.Interval(10, 100));

                        ep.Consumer<TestInfoConsumer1>(provider);

                        ep.Bind("test", x =>
                        {
                            //x.Durable = false;
                            //x.AutoDelete = true;
                            x.RoutingKey = "regular";
                            x.ExchangeType = ExchangeType.Direct;
                        });
                    });

                    cfg.Publish<TestInfo>(x =>
                    {
                        x.AlternateExchange = "test";
                        x.AutoDelete = true;
                        x.Durable = false;
                        x.ExchangeType = ExchangeType.Direct;
                    });

                    cfg.ReceiveEndpoint("tls123456", ep =>
                    {
                        //ep.BindMessageExchanges = false;

                        //ep.PrefetchCount = 1;
                        //ep.UseMessageRetry(r => r.Interval(10, 100));

                        ep.ConfigureConsumer<TestInfoConsumer2>(provider);

                        ep.Bind("test", x =>
                        {
                            //x.Durable = false;
                            //x.AutoDelete = true;
                            x.RoutingKey = "normal";
                            x.ExchangeType = ExchangeType.Direct;
                        });
                    });
                }));
            });

            services.AddMassTransitHostedService();
        }
    }
}
