using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDemo.Events;
using Coravel.Events.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiDemo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BroadcastController : ControllerBase
    {
        private IDispatcher _dispatcher;

        public BroadcastController(IDispatcher dispatcher)
        {
            this._dispatcher = dispatcher;
        }

        public IActionResult Publish()
        {
            DemoEvent demo = new DemoEvent($"event time {DateTime.Now}");
            _dispatcher.Broadcast(demo);
            return Ok( DateTime.Now);
        }
    }
}