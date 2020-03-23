using GreenPipes;
using MassTransit;
using MassTransit.AspNetCoreIntegration;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test.Handlers;

namespace Test.Configures
{
    public static class ConfigureMQ
    {
        /// <summary>
        /// 推荐
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureMQ1(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<SubmitOrder>();

                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var host = cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ReceiveEndpoint("order", ep =>
                    {
                        //ep.Bind("test", a =>
                        //{
                        //    a.Durable = true;
                        //    a.ExchangeType = "topic";
                        //    a.RoutingKey = "post.#";
                        //});

                        ep.PrefetchCount = 16;
                        ep.UseMessageRetry(r => r.Interval(10, 1700));

                        ep.ConfigureConsumer<SubmitOrder>(provider);
                    });
                }));
            });

            services.AddMassTransitHostedService();
        }

        public static void ConfigureMQ2(this IServiceCollection services)
        {
            //services.AddMassTransit(x =>
            //{
            //    x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
            //    {
            //        //var host = cfg.Host("172.30.10.84", "/", h =>
            //        var host = cfg.Host("localhost", "/", h =>
            //        {
            //            h.Username("guest");
            //            h.Password("guest");
            //        });

            //        cfg.ReceiveEndpoint("order", ep =>
            //        {
            //            //ep.Bind("test", a =>
            //            //{
            //            //    a.Durable = true;
            //            //    a.ExchangeType = "topic";
            //            //    a.RoutingKey = "post.#";
            //            //});

            //            ep.PrefetchCount = 16;
            //            ep.UseMessageRetry(r => r.Interval(10, 1700));

            //            ep.Consumer<SubmitOrder>();
            //        });
            //    }));
            //});

            //services.AddMassTransitHostedService();
        }

        public static void ConfigureMQ3(this IServiceCollection services)
        {
            IBusControl CreateBus(IServiceProvider serviceProvider)
            {
                return Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var host = cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ReceiveEndpoint("order", ep =>
                    {
                        ep.PrefetchCount = 16;
                        ep.UseMessageRetry(r => r.Interval(10, 1700));

                        ep.Consumer<SubmitOrder>(serviceProvider);
                    });
                });
            }
            void ConfigureMassTransit(IServiceCollectionConfigurator configurator)
            {
                configurator.AddConsumer<SubmitOrder>();
            }

            services.AddMassTransit(CreateBus, ConfigureMassTransit);
        }
    }
}
