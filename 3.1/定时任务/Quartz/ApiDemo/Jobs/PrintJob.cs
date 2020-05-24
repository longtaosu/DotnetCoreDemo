using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDemo.Quartz
{
    [DisallowConcurrentExecution]
    public class PrintJob : IJob
    {
        private ILogger<PrintJob> _logger;
        private IServiceProvider _provider;

        public PrintJob(ILogger<PrintJob> logger,IServiceProvider provider)
        {
            this._provider = provider;
            this._logger = logger;
        }
        public Task Execute(IJobExecutionContext context)
        {
            // Create a new scope
            //using (var scope = _provider.CreateScope())
            //{
            //    // Resolve the Scoped service
            //    var service = scope.ServiceProvider.GetService<IScopedService>();
            //    _logger.LogInformation("Hello world!");
            //}

            _logger.LogInformation($"执行时间：{DateTime.Now}");
            return Task.CompletedTask;
        }
    }
}
