using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RateLimitDemo.Handlers;

namespace RateLimitDemo.Controllers
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
            for (int i = 0; i < 11; i++)
            {
                _busControl.Publish<TestInfo>(new TestInfo()
                {
                    Name = name + i.ToString(),
                    Age = age
                }); 
            }
            return string.Format("于时间:{0}，发布消息", DateTime.Now);
        }
    }
}