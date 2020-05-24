using ConsoleDemo.Jobs;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Text;
using Topshelf;

namespace ConsoleDemo
{
    public class ServiceRunner : ServiceControl, ServiceSuspend
    {
        private readonly IScheduler scheduler;
        public ServiceRunner()
        {
            scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
        }
        public bool Start(HostControl hostControl)
        {
            //启动调度器
            scheduler.Start();
            //具体任务启动可在配置文件编写
            string cron1 = "0/1 * * * * ?";//ConfigurationManager.AppSettings["cron1"].ToString();
            if (!string.IsNullOrEmpty(cron1))
            {
                IJobDetail job1 = JobBuilder.Create<TestJob1>().Build();
                ITrigger trigger1 = TriggerBuilder.Create().StartNow().WithCronSchedule(cron1).Build();
                scheduler.ScheduleJob(job1, trigger1);
            }

            return true;
        }
        public bool Continue(HostControl hostControl)
        {
            scheduler.ResumeAll();
            return true;
        }

        public bool Pause(HostControl hostControl)
        {
            scheduler.PauseAll();
            return true;
        }



        public bool Stop(HostControl hostControl)
        {
            scheduler.Shutdown(false);
            return true;
        }
    }
}
