using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SettingApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })

            .ConfigureAppConfiguration((context,config)=> {
                config.AddIniFile("Configs/iniSetting.ini", optional: true, reloadOnChange: true);
                config.AddJsonFile("Configs/jsonSetting.json", optional: true, reloadOnChange: true);
                config.AddXmlFile("Configs/xmlSetting.xml", optional: true, reloadOnChange: true);
            });
    }
}
