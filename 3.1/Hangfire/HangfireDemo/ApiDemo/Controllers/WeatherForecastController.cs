using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApiDemo.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        #region Test1_Cron表达式
        [HttpGet]
        public void TestHangfire()
        {
            RecurringJob.AddOrUpdate("testRequireJob", () => Test(DateTime.Now.ToString()), "*/5 * * * * ?");
            
            //每分钟执行一次
            //RecurringJob.AddOrUpdate("testRequireJob", () => Test(DateTime.Now.ToString()), Cron.Minutely);
        }
        #endregion

        #region Test2_立即执行
        [HttpGet]
        public void TestHangfire2()
        {
            BackgroundJob.Enqueue(() => Test("test2"));
        }
        #endregion

        #region Test3_延时执行
        [HttpGet]
        public void TestHangfire3()
        {
            BackgroundJob.Schedule(() => Test("test3"),TimeSpan.FromSeconds(10));
        }
        #endregion

        #region Test4_任务后执行
        public void TestHangfire4()
        {
            var jobId = BackgroundJob.Enqueue(() => Test("test4_1"));

            BackgroundJob.ContinueJobWith(jobId, () => Test("test4_2"));
        }

        #endregion

        public void Test(string info)
        {
            var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _logger.LogInformation(info + " ," + time);
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.LogInformation(DateTime.Now.ToString());
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
