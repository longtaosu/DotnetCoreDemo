using DIDemo.Jobs;
using DIDemo.Quartz;
using DIDemo.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.IO;
using Topshelf;

namespace DIDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IJobFactory, JobFactory>();
                    services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
                    services.AddScoped<ITestService, TestService>();
                    services.AddHostedService<QuartzHostedService>();

                    services.AddSingleton<PrintJob>();
                    services.AddSingleton(new JobSchedule(jobType: typeof(PrintJob), cronExpression: "0/5 * * * * ?"));
                })
                .UseConsoleLifetime()
                .Build();

            host.Run();
        }
    }
}
