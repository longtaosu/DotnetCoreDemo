using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Test.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HangfireController : ControllerBase
    {
        private ILogger<HangfireController> _logger;
        public HangfireController(ILogger<HangfireController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Cron 表达式
        /// </summary>
        [HttpGet]
        public void TestCron()
        {
            RecurringJob.AddOrUpdate("testCronJob", () => Test.PrintInfo("CronJob"), "0/5 * * * * ?");

            //RecurringJob.AddOrUpdate("testCronJob", () => Test.PrintInfo("CronJob"), Cron.Minutely);
        }

        /// <summary>
        /// 立即执行
        /// </summary>
        [HttpGet]
        public void TestImmediate()
        {
            BackgroundJob.Enqueue(() => Test.PrintInfo("ImmediateJob"));
        }

        /// <summary>
        /// 延时执行
        /// </summary>
        [HttpGet]
        public void TestDelay()
        {
            Console.WriteLine(DateTime.Now);
            BackgroundJob.Schedule(() => Test.PrintInfo("DelayJob"),TimeSpan.FromSeconds(10));
        }

        [HttpGet]
        public void TestMultiTask()
        {
            Console.WriteLine(DateTime.Now);

            var jobId = BackgroundJob.Schedule(() => Test.PrintInfo("task1"),TimeSpan.FromSeconds(10));

            BackgroundJob.ContinueJobWith(jobId, () => Test.PrintInfo("task2"));
        }
    }


    public static class Test
    {
        public static void PrintInfo(string info)
        {
            Console.WriteLine(info + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}