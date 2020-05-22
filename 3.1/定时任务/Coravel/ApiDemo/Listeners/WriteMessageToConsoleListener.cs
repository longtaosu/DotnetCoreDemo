using ApiDemo.Events;
using Coravel.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDemo.Listeners
{
    public class WriteMessageToConsoleListener : IListener<DemoEvent>
    {
        public Task HandleAsync(DemoEvent broadcasted)
        {
            Console.WriteLine($"WriteMessageToConsoleListener receive this message from DemoEvent: ${broadcasted.Message}");
            return Task.CompletedTask;
        }
    }
}
