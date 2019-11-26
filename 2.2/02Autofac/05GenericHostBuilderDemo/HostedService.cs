using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutofacDemo.GenericHostBuilderDemo
{
    public class HostedService : IHostedService
    {
        private readonly ILogger _logger;
        public HostedService(ILogger logger)
        {
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _logger.Log("Starting hosted service");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _logger.Log("Stopping hosted service");
        }
    }
}
