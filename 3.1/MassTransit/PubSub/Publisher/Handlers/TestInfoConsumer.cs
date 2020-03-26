using Lts.Models;
using Lts.Services;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Publisher.Handlers
{
    public class TestInfoConsumer : IConsumer<TestInfo>
    {
        private ILogService _logService;
        public TestInfoConsumer(ILogService logService)
        {
            _logService = logService;
        }

        public Task Consume(ConsumeContext<TestInfo> context)
        {
            var info = context.Message;
            return Task.Run(() =>
            {
                _logService.PrintLog(string.Format("{0}：{1}", info.time, info.info));
            });
        }
    }
}
