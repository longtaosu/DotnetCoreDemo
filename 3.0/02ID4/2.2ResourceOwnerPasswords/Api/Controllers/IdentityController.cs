using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    //[Route("api/[controller]")]
    [Route("identity")]
    [Authorize]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        public IActionResult Get()
        {
            var result = new JsonResult(from c in User.Claims select new { c.Type, c.Value });
            return result;
        }
    }
}