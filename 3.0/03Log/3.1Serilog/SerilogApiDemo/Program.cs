using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace SerilogApiDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Error)
                .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Error)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                //.WriteTo.File(Path.Combine("Logs", DateTime.Now.ToString("yyyy-MM-dd") + ".txt"), Serilog.Events.LogEventLevel.Information, encoding: Encoding.UTF32)

                .WriteTo.Logger(lg => lg
                    .Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Error || evt.Level == LogEventLevel.Fatal)
                    .WriteTo.File(Path.Combine("CustomLogs", DateTime.Now.ToString("yyyy-MM-dd") + "-Error.txt"), encoding: Encoding.UTF8))
                .WriteTo.Logger(lg => lg
                    .Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Information)
                    .WriteTo.File(Path.Combine("CustomLogs", DateTime.Now.ToString("yyyy-MM-dd") + "-Info.txt"), encoding: Encoding.UTF8))

                .CreateLogger();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().UseSerilog();
                });
    }
}
