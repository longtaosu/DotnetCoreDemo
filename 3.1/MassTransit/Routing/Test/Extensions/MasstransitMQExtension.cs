using GreenPipes;
using Lts.Models;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test.Handlers;

namespace Test.Extensions
{
    public static class MasstransitMQExtension
    {
        public static void ConfigureMQ(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<TestInfoConsumer1>();

                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var host = cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    //cfg.ReceiveEndpoint("priority-orders", ep =>
                    //{
                    //    ep.ConfigureConsumeTopology = false;

                    //    ep.Consumer<TestInfoConsumer1>(provider);

                    //    ep.Bind("exchange-name", cc =>
                    //    {
                    //        //cc.Durable = false;
                    //        //cc.AutoDelete = true;
                    //        cc.ExchangeType = ExchangeType.Direct;
                    //        cc.RoutingKey = "priority";
                    //    });
                    //});

                    //cfg.Publish<TestInfo>(x => x.ExchangeType = ExchangeType.Topic);
                    //cfg.ReceiveEndpoint("regular-orders", ep =>
                    //{
                    //    ep.ConfigureConsumeTopology = false;
                    //    ep.Consumer<TestInfoConsumer1>(provider);
                    //    //ep.ConfigureConsumer<TestInfoConsumer1>(provider);

                    //    ep.Bind("exchange-name", cc =>
                    //    {
                    //        cc.ExchangeType = ExchangeType.Direct;
                    //        cc.RoutingKey = "regular";
                    //    });
                    //});

                    //cfg.Publish<TestInfo>(c => c.ExchangeType = ExchangeType.Topic);
                    //cfg.Message<TestInfo>(c => c.SetEntityName("Models.TestInfo"));

                    //cfg.Message<TestInfo>(x => x.SetEntityName("TestInfo"));
                    cfg.ReceiveEndpoint("regular-orders", ep =>
                    {
                        //ep.ConfigureConsumeTopology = false;
                        //ep.BindMessageExchanges = false;
                        ep.Consumer<TestInfoConsumer1>();
                        ep.ExchangeType = ExchangeType.Topic;

                        //ep.ConfigureConsumer<TestInfoConsumer1>(provider);

                        ep.Bind("exchange-name", cc =>
                        {
                            cc.ExchangeType = ExchangeType.Topic;
                            cc.RoutingKey = "regular";
                        });
                    });

                    //cfg.Publish<TestInfoConsumer1>(cc => 
                    //{ 
                    //    cc.ExchangeType = ExchangeType.Topic;
                    //    cc.Durable = false;
                    //    cc.AutoDelete = false;
                    //});


                    //cfg.ReceiveEndpoint("log1122","", ep =>
                    //{

                    //    //ep.PrefetchCount = 1;
                    //    //ep.UseMessageRetry(r => r.Interval(10, 100));

                    //    //https://github.com/MassTransit/MassTransit/issues/1685
                    //    //https://github.com/MassTransit/MassTransit/blob/develop/src/MassTransit.RabbitMqTransport.Tests/RoutingKeyTopic_Specs.cs#L29




                    //    ep.ConfigureConsumer<TestInfoConsumer1>(provider);
                    //});

                }));
            });

            services.AddMassTransitHostedService();
        }
    }
}
