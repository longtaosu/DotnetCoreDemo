using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimitDemo.Handlers
{
    public class TestInfoConsumer : IConsumer<TestInfo>
    {
        public Task Consume(ConsumeContext<TestInfo> context)
        {
            var message = context.Message;
            Thread.Sleep(1500);
            return Task.Run(() =>
            {
                Console.WriteLine("time:{0},name:{1},age:{2}", DateTime.Now, message.Name, message.Age);
            });
        }
    }

    public class TestInfo
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
