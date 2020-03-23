using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyCaching.Core;
//using EasyCaching.Core;
using EasyCaching.CSRedis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiDemo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RedisController : ControllerBase
    {
        private string _key = "time";
        private readonly IEasyCachingProvider _provider;

        public RedisController(IEasyCachingProvider provider)
        {
            _provider = provider;
        }

        [HttpGet]
        public string GetTime()
        {
            var result = _provider.Get<string>(_key).Value;
            return result;
        }

        [HttpPost]
        public void SetTime()
        {
            _provider.Set(_key, DateTime.Now.ToString(), TimeSpan.FromSeconds(10));
        }
    }
}