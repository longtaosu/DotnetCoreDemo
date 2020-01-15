using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jwt_RoleBased.Models;
using Jwt_RoleBased.Propertities;
using Jwt_RoleBased.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Jwt_RoleBased.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private List<User> _users = new List<User>
        {
            new User{ Id = 1,FirstName="Admin", LastName="User", Username="admin", Password="admin",Role = Role.Admin},
            new User{ Id = 2,FirstName="Admin", LastName="User", Username="user", Password="user",Role = Role.User}
        };

        private AppSettings _appSettings;
        private TokenService _tokenService;
        public UserController(IOptions<AppSettings> appSettings,TokenService tokenService)
        {
            _appSettings = appSettings.Value;
            _tokenService = tokenService;
        }

        [HttpPost]
        [AllowAnonymous]
        public string Authenticate(User user)
        {
            var usr = _users.FirstOrDefault(t => t.Username == user.Username && t.Password == user.Password);
            if (usr == null)
                return "no user";

            return _tokenService.GenerateToken(usr);
        }

        [HttpGet]
        [Authorize(Roles = Role.Admin)]
        public IActionResult GetAll()
        {
            //var users = _users;
            return Ok(_users);
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetByID(int id)
        {
            var user = _users.FirstOrDefault(t => t.Id == id);
            return Ok(user);
        }
    }
}