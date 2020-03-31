using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lts.Models;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Test.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        private IBusControl _busControl;
        private IPublishEndpoint _endPoint;
        public InfoController(IBusControl busControl,IPublishEndpoint endPoint)
        {
            _busControl = busControl;
            _endPoint = endPoint;
        }

        public string Log(string info, string key)
        {
            //_endPoint.Publish<TestInfo>(new TestInfo()
            //{
            //    info = info,
            //    time = DateTime.Now
            //},c=>c.SetRoutingKey(key));
            _busControl.Publish<TestInfo>(new TestInfo()
            {
                info = info + "空",
                time = DateTime.Now
            });

            _busControl.Publish<TestInfo>(new TestInfo()
            {
                info = info + " _key",
                time = DateTime.Now
            }, t => t.SetRoutingKey(key));

            return string.Format("于时间:{0}，发布消息：{1}", DateTime.Now, info);
        }
    }
}