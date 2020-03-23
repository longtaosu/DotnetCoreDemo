using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Test.Handlers;

namespace Test.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private IBusControl _busControl;
        public OrderController(IBusControl busControl)
        {
            _busControl = busControl;
        }

        [HttpGet]
        public string Test()
        {
            _busControl.Publish<Order>(new Order()
            {
                OrderId = Guid.NewGuid(),
                 Timestamp = DateTime.Now
            });

            return DateTime.Now.ToString();
        }
    }
}