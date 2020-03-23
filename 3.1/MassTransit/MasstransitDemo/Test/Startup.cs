using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MassTransit;
using Microsoft.Extensions.Options;
using MassTransit.AspNetCoreIntegration;
using Test.Handlers;
using GreenPipes;
using Test.Services;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Test.Configures;

namespace Test
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddScoped<ITestService, TestService>();

            #region 不带接口调用
            //services.AddMassTransit(x =>
            //{
            //    x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
            //    {
            //        //var host = cfg.Host("172.30.10.84", "/", h =>
            //        var host = cfg.Host("localhost", "/", h =>
            //{
            //    h.Username("guest");
            //    h.Password("guest");
            //});

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
            #endregion

            #region 带接口注入
            //IBusControl CreateBus(IServiceProvider serviceProvider)
            //{
            //    return Bus.Factory.CreateUsingRabbitMq(cfg =>
            //    {
            //        var host = cfg.Host("localhost", "/", h =>
            //        {
            //            h.Username("guest");
            //            h.Password("guest");
            //        });

            //        cfg.ReceiveEndpoint("order", ep =>
            //        {
            //            ep.PrefetchCount = 16;
            //            ep.UseMessageRetry(r => r.Interval(10, 1700));

            //            ep.Consumer<SubmitOrder>(serviceProvider);
            //        });
            //    });
            //}
            //void ConfigureMassTransit(IServiceCollectionConfigurator configurator)
            //{
            //    configurator.AddConsumer<SubmitOrder>();
            //}

            //services.AddMassTransit(CreateBus, ConfigureMassTransit);
            #endregion

            #region 推荐版本
            //services.AddMassTransit(x =>
            //{
            //    x.AddConsumer<SubmitOrder>();

            //    x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
            //    {
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

            //            ep.ConfigureConsumer<SubmitOrder>(provider);
            //        });
            //    }));
            //});

            //services.AddMassTransitHostedService();
            #endregion


            services.ConfigureMQ3();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }


    }
}
