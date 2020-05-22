using Coravel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace ConsoleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.SetBasePath(Directory.GetCurrentDirectory());
                    configApp.AddEnvironmentVariables(prefix: "PREFIX_");
                    configApp.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // Add Coravel's Scheduling...
                    services.AddScheduler();
                })
                .Build();

            // Configure the scheduled tasks....
            host.Services.UseScheduler(scheduler =>
            {
                scheduler
                    .Schedule(() => Console.WriteLine("This was scheduled every minute." + DateTime.Now.ToString()))
                    .Cron("*/2 * * * *");
            }
            );

            // Run it!
            host.Run();
        }
    }
}
