using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Topshelf;
using TopShelfDemo.Extensions;

namespace TopShelfDemo
{
    public class ServiceRunner : ServiceControl
    {
        private List<Type> LocalJobs = new List<Type>();
        private IServiceProvider _serviceProvider;

        public ServiceRunner()
        {
            //加载本地Jobs
            LocalJobs = LoadLocalJobs();
            //加载配置信息
            var configurationRoot = ConfigureConfiguration();
            //配置服务
            _serviceProvider = ConfigureServices(configurationRoot);
        }

        private IConfiguration ConfigureConfiguration()
        {
            //配置文件
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }
        private IServiceProvider ConfigureServices(IConfiguration configuration)
        {
            //依赖注入
            IServiceCollection services = new ServiceCollection();

            services.AddOptions().Configure<AppSettings>(x => configuration.Bind(x));
            services.AddLocalServices();

            //NLog
            services.AddNLog();

            //Chloe
            var chloe = configuration.GetSection("DBConfig").Get<DBOption>();
            services.AddChloe(chloe);

            //添加 job
            var lstJobs = configuration.GetSection("Jobs").Get<List<CronJob>>();
            foreach (var item in lstJobs)
            {
                if (item.Status > 0)
                {
                    var type = LocalJobs.Where(t => t.Name == item.Name).FirstOrDefault();
                    services.AddScoped(type);
                }
            }

            services.AddScoped<IJobFactory, JobFactory>();
            services.AddSingleton(service =>
            {
                var scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
                scheduler.JobFactory = service.GetService<IJobFactory>();
                return scheduler;
            });

            //构建容器
            return services.BuildServiceProvider();
        }

        public bool Start(HostControl hostControl)
        {
            var scheduler = _serviceProvider.GetService(typeof(IScheduler)) as IScheduler;
            scheduler.Start();

            //添加 job
            var setting = (_serviceProvider.GetService(typeof(IOptions<AppSettings>)) as IOptions<AppSettings>).Value;
            foreach (var item in setting.Jobs)
            {
                if (item.Status > 0)
                {
                    var type = LocalJobs.Where(t => t.Name == item.Name).FirstOrDefault();
                    IJobDetail job = JobBuilder.Create(type).Build();
                    ITrigger trigger = TriggerBuilder.Create().StartNow().WithCronSchedule(item.Cron).Build();
                    scheduler.ScheduleJob(job, trigger);
                }
            }

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            var scheduler = _serviceProvider.GetService(typeof(IScheduler)) as IScheduler;
            scheduler.Shutdown(true);
            return true;
        }

        /// <summary>
        /// Job定义：xxx_Job
        /// </summary>
        /// <returns></returns>
        private List<Type> LoadLocalJobs()
        {
            var exportJobs = Assembly.GetExecutingAssembly().GetExportedTypes();
            var jobs = exportJobs.Where(t => t.Name.Contains("_Job")).ToList();
            return jobs;
        }
    }
}
