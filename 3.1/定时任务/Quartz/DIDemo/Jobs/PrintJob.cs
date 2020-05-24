using DIDemo.Services;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DIDemo.Jobs
{
    public class PrintJob : IJob
    {
        private readonly ILogger _logger;
        private ITestService _testService;

        public PrintJob(ILogger<PrintJob> logger,ITestService testService)
        {
            _logger = logger;
            this._testService = testService;
        }

        public Task Execute(IJobExecutionContext context)
        {
            //_logger.LogInformation(string.Format("[{0:yyyy-MM-dd hh:mm:ss:ffffff}]任务执行！", DateTime.Now));
            //Console.WriteLine($"{DateTime.Now}：任务执行");
            _testService.Print();
            return Task.CompletedTask;
        }
    }
}
