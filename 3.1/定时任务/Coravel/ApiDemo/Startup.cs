using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDemo.Events;
using ApiDemo.Invocables;
using ApiDemo.Listeners;
using Coravel;
using Coravel.Events.Interfaces;
using Coravel.Queuing.Interfaces;
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
            //DI
            services.AddScheduler();

            //Queue
            services.AddQueue();

            //services.AddScoped<SendNightlyReportsEmailJob>();
            services.AddScoped<DoExpensiveCalculationAndStore>();

            services.AddEvents();

            services.AddTransient<WriteMessageToConsoleListener>()
                    .AddTransient<WriteStaticMessageToConsoleListener>();

            services.AddControllers();
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

            IEventRegistration registration = app.ApplicationServices.ConfigureEvents();

            registration.Register<DemoEvent>()
                .Subscribe<WriteMessageToConsoleListener>()
                .Subscribe<WriteStaticMessageToConsoleListener>();

            //添加定时任务1：每秒打印
            app.ApplicationServices.UseScheduler(scheduler =>
            {
                scheduler.Schedule(() => Console.WriteLine($"Runs every minute Ran at: {DateTime.UtcNow}")).Cron("* * * * *");//.EverySecond();
            });

            //添加队列
            
            app.ApplicationServices.ConfigureQueue().LogQueuedTaskProgress(app.ApplicationServices.GetService<ILogger<IQueue>>());
        }
    }
}
