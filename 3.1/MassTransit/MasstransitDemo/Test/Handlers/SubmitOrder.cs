using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Test.Services;

namespace Test.Handlers
{
    public class SubmitOrder : IConsumer<Order>
    {
        private ITestService _testService;

        public SubmitOrder(ITestService testService)
        {
            _testService = testService;
        }
        public Task Consume(ConsumeContext<Order> context)
        {
            return Task.Run(() =>
            {
                //Thread.Sleep(10 * 1000);
                //if (DateTime.Now.Second % 2 == 0)
                //{
                //    Console.WriteLine("测试失败:" + DateTime.Now);
                //    throw new Exception("测试失败:" + DateTime.Now);
                //}

                Console.WriteLine("执行完成：" + context.Message.OrderId);
                Console.WriteLine("Guid：" + _testService.GetGuid());
            });            
        }
    }

    public class Order
    {
        public Guid OrderId { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
