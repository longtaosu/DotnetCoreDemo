using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDemo.Handlers;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiDemo.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class MQController : ControllerBase
    {
        private IBusControl _busControl;
        public MQController(IBusControl busControl)
        {
            _busControl = busControl;
        }

        [HttpGet]
        public bool Publish()
        {
            _busControl.Publish(new MyMessage()
            {
                CustomerId = DateTime.Now.Second.ToString()
            });

           


            return true;
        }
    }
}