using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiDemo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RedisController : ControllerBase
    {
        private string key = "time";

        [HttpGet]
        public string GetTime()
        {
            var result = RedisHelper.Get(key);
            return result;
        }

        [HttpPost]
        public bool SetTime()
        {
            var result = RedisHelper.Set(key, DateTime.Now.ToString());
            return result;
        }

        public bool SetLifeTime()
        {
            var result = RedisHelper.Set(key, DateTime.Now, 60);
            return result;
        }

        [HttpGet]
        public void Publish()
        {
            RedisHelper.Publish(key, DateTime.Now.ToString());
        }
    }
}