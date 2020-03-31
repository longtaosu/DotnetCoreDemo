using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lts.Models;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TopicDemo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        private IBusControl _busControl;
        public InfoController(IBusControl busControl)
        {
            _busControl = busControl;
        }

        public string Log(string info, string key)
        {
            _busControl.Publish<TestInfo>(new TestInfo()
            {
                info = info,
                time = DateTime.Now
            }, x => x.SetRoutingKey(key));
            return string.Format("于时间:{0}，发布消息：{1}", DateTime.Now, info);
        }
    }
}