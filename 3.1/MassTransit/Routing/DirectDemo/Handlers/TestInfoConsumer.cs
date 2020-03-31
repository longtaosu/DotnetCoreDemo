using Lts.Models;
using Lts.Services;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectDemo.Handlers
{
    public class TestInfoConsumer1 : IConsumer<TestInfo>
    {
        private ILogService _logService;
        public TestInfoConsumer1(ILogService logService)
        {
            _logService = logService;
        }

        public Task Consume(ConsumeContext<TestInfo> context)
        {
            var info = context.Message;
            return Task.Run(() =>
            {
                _logService.PrintLog(string.Format("{0}：{1}_consumer1", info.time, info.info));
            });
        }
    }

    public class TestInfoConsumer2 : IConsumer<TestInfo>
    {
        private ILogService _logService;
        public TestInfoConsumer2(ILogService logService)
        {
            _logService = logService;
        }

        public Task Consume(ConsumeContext<TestInfo> context)
        {
            var info = context.Message;
            return Task.Run(() =>
            {
                _logService.PrintLog(string.Format("{0}：{1}_consumer2", info.time, info.info));
            });
        }
    }
}
