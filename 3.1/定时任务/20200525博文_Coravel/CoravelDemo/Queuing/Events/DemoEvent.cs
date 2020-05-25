using Coravel.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Queuing.Events
{
    public class DemoEvent : IEvent
    {
        public string Message { get; set; }

        public DemoEvent(string message)
        {
            Message = message;
        }
    }
}
