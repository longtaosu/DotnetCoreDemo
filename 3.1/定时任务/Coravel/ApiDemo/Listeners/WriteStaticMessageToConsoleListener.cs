using ApiDemo.Events;
using Coravel.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDemo.Listeners
{
    public class WriteStaticMessageToConsoleListener : IListener<DemoEvent>
    {
        public Task HandleAsync(DemoEvent broadcasted)
        {
            Console.WriteLine("Listener writing a static message.");
            return Task.CompletedTask;
        }
    }
}
