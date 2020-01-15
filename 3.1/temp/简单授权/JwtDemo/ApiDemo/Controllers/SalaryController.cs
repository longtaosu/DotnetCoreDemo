using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "HRManager,Finance")]//角色是HRManager或Finance

    //角色是 PowerUser 且 ControlPanelUser
    //[Authorize(Roles = "PowerUser")]
    //[Authorize(Roles = "ControlPanelUser")]

    //[Authorize(Policy = "RequireAdministratorRole"]
    public class SalaryController : ControllerBase
    {

    }
}