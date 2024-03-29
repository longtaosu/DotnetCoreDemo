﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LTS.Services.Entities;
using LTS.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApiDemo.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IGroupMemberService _groupMemberService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IGroupMemberService groupMemberService)
        {
            _logger = logger;
            _groupMemberService = groupMemberService;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {



            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet]
        public List<GroupMember> GetUsers()
        {
            var users = _groupMemberService.GetUsers();
            return users;
        }
    }
}
