using Coravel.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Queuing.Events
{
    public class Listener1 : IListener<DemoEvent>
    {
        public Task HandleAsync(DemoEvent broadcasted)
        {
            Console.WriteLine($"Listener 1 接收到消息：{broadcasted.Message}");
            return Task.CompletedTask;
        }
    }
}
