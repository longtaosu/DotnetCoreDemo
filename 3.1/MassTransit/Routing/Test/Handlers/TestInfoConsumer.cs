using Lts.Models;
using Lts.Services;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test.Handlers
{
    public class TestInfoConsumer1 : IConsumer<TestInfo>
    {
        //private ILogService _logService;
        //public TestInfoConsumer1(ILogService logService)
        //{
        //    _logService = logService;
        //}

        public Task Consume(ConsumeContext<TestInfo> context)
        {
            var info = context.Message;
            return Task.Run(() =>
            {
                Console.WriteLine(info.info);
                //_logService.PrintLog(string.Format("{0}：{1}", info.time, info.info));
            });
        }
    }
}
