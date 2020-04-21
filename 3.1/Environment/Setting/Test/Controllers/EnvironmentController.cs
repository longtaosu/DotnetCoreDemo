using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnvironmentController : ControllerBase
    {
        IConfiguration _configuration;
        public EnvironmentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet]
        public string Get()
        {
            return _configuration.GetValue<string>("info");
        }
    }
}