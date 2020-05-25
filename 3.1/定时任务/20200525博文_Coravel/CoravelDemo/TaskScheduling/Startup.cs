using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coravel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TaskScheduling.Schedules;

namespace TaskScheduling
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
            services.AddScheduler();

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

            var provider = app.ApplicationServices;
            provider.UseScheduler(scheduler =>
            {

                //��������
                //scheduler.Schedule(() =>
                //{
                //    Console.WriteLine($"Every second during the week,{DateTime.Now}");
                //})
                //.EverySecond()
                //.Weekday();


                //���������
                //scheduler
                //.ScheduleWithParams<BackupDatabaseTableInvocable>("[dbo].[Users]")
                //.EverySecond()
                //.Weekday();

                //ִ�г�ͻ
                scheduler.OnWorker("db");
                scheduler
                .ScheduleWithParams<BackupDatabaseTableInvocable>("[dbo].[Users]")
                .EverySeconds(10)
                .Weekday();

                scheduler.OnWorker("print");
                scheduler.Schedule(() =>
                {
                    Console.WriteLine($"Every second during the week,{DateTime.Now}");
                })
                .EverySecond()
                .Weekday();
            });


        }
    }
}
