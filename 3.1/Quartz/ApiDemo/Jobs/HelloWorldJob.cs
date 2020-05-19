using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDemo.Jobs
{
    [DisallowConcurrentExecution]
    public class HelloWorldJob : IJob
    {
        private readonly ILogger<HelloWorldJob> _logger;

        public HelloWorldJob(ILogger<HelloWorldJob> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Execute(IJobExecutionContext context)
        {
            //throw new NotImplementedException();
            _logger.LogInformation("Hello world by lts at {0}!", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            return Task.CompletedTask;
        }
    }
}
