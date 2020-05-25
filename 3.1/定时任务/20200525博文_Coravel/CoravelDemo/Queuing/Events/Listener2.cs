using Coravel.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Queuing.Events
{
    public class Listener2 : IListener<DemoEvent>
    {
        public Task HandleAsync(DemoEvent broadcasted)
        {
            Console.WriteLine($"Listener 2 接收到消息：{broadcasted.Message}");
            return Task.CompletedTask;
        }
    }
}
