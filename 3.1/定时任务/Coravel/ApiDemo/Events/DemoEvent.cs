using Coravel.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDemo.Events
{
    public class DemoEvent : IEvent
    {
        public string Message { get; set; }

        public DemoEvent(string message)
        {
            this.Message = message;
        }
    }
}
