using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RetryDemo.Handlers;

namespace RetryDemo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private IBusControl _busControl;
        public TestController(IBusControl busControl)
        {
            _busControl = busControl;
        }

        public string Log(string name, int age)
        {
            _busControl.Publish<TestInfo>(new TestInfo()
            {
                Name = name,
                Age = age
            });
            return string.Format("于时间:{0}，发布消息", DateTime.Now);
        }

    }
}