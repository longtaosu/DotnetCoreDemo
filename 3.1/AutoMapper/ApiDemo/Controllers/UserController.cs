using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDemo.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiDemo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        
        public UserController(IMapper mapper)
        {
            _mapper = mapper;
        }

        public IActionResult GetUser()
        {
            var user = new User()
            {
                Id = 1,
                Address = "地址" + DateTime.Now.ToString(),
                Email = DateTime.Now.ToString(),
                FirstName = DateTime.Now.Hour.ToString(),
                LastName = DateTime.Now.Minute.ToString()
            };

            var userModel = _mapper.Map<UserViewModel>(user);
            return Ok(userModel);
        }
    }
}