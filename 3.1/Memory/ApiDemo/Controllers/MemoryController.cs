using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace ApiDemo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MemoryController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        public MemoryController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public string GetMemory()
        {
            _memoryCache.TryGetValue("test", out string info);
            return info;
        }

        [HttpPost]
        public bool RemoveMemory()
        {
            try
            {
                _memoryCache.Remove("test");
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        [HttpPost]
        public bool SetMemory(string info)
        {
            try
            {
                _memoryCache.Set("test", info + DateTime.Now.ToString(), DateTimeOffset.Now.AddSeconds(150) );
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

    }
}