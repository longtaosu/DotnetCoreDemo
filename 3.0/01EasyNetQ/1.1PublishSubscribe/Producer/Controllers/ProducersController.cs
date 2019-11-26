using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyNetQ;
using Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Producer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProducersController : ControllerBase
    {
        private readonly IBus _bus;
        public ProducersController(IBus bus)
        {
            _bus = bus;
        }

        [HttpGet]
        public JsonResult Send()
        {
            _bus.Publish(new TextMessage { Text = "Send Message from the Producer ," + DateTime.Now.ToString() });
            return new JsonResult("");
        }
    }
}