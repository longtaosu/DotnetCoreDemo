using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetryDemo.Handlers
{
    public class TestInfoConsumer : IConsumer<TestInfo>
    {
        public Task Consume(ConsumeContext<TestInfo> context)
        {
            var message = context.Message;
            if (context.GetRetryCount() < 5)
            {
                Console.WriteLine("测试第{0}次", context.GetRetryCount());
                throw new Exception("测试异常重试!");
            }
                

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
