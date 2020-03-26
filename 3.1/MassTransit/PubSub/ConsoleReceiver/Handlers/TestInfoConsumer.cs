using Lts.Models;
using Lts.Services;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleReceiver.Handlers
{
    public class TestInfoConsumer :  IConsumer<TestInfo>
    {
        public Task Consume(ConsumeContext<TestInfo> context)
        {
            var info = context.Message;
            return Task.Run(() =>
            {
                Console.WriteLine("控制台，{0}：{1}", info.time, info.info);
            });
        }
    }
}
