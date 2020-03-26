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


                        //������������
                        //ep.Consumer(() => new MyMessageConsumer());

                        //�Ѵ��ڵ������߹���
                        //ep.Consumer(consumerFactory);

                        //�������͵Ĺ���������object���ͣ��������Ѻã�
                        //ep.Consumer(consumerType, type => Activator.CreateInstance(type));

                        //�����Ĺ�������
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
