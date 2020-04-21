using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ApiDemo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EnvironmentController : ControllerBase
    {
        IHostEnvironment _env;
        IConfiguration _configuration;
        public EnvironmentController (IHostEnvironment env,IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }

        [HttpGet]
        public string Get()
        {
            return _env.EnvironmentName + _configuration.GetValue<string>("info");
        }
    }
}