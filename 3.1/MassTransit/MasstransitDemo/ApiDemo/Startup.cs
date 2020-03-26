using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDemo.Handlers;
using GreenPipes;
using MassTransit;
using MassTransit.AspNetCoreIntegration.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ApiDemo
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

            services.AddMassTransit(cfg =>
            {
                //cfg.AddConsumer<>

                cfg.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(s =>
                {
                    var host = s.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    s.ReceiveEndpoint("order", ep =>
                    {
                        ep.PrefetchCount = 16;
                        //ep.UseScheduledRedelivery(r => r.Intervals(TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(20)));
                        ep.UseMessageRetry(r => r.Interval(3, 100));

                        ep.UseInMemoryOutbox();

                        //ep.ConfigureConsumer<MyMessageConsumer>(provider);
                        ep.Consumer<MyMessageConsumer>();
                        //ep.Observer<MyMessage>(new MyMessageObserver());


                        //匿名工厂方法
                        //ep.Consumer(() => new MyMessageConsumer());

                        //已存在的消费者工厂
                        //ep.Consumer(consumerFactory);

                        //基于类型的工厂，返回object类型（对容器友好）
                        //ep.Consumer(consumerType, type => Activator.CreateInstance(type));

                        //匿名的工厂方法
                        //ep.Consumer(() => new MyMessage(), x =>
                        //{
                        //    x.UseExecuteAsync(context => Console.Out.WriteLineAsync("Consumer created"));
                        //});
                    });
                }));
            });
            services.AddMassTransitHostedService();
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
