using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RabbitmqMVCDemo.Models;
using RabbitmqMVCDemo.Services;

namespace RabbitmqMVCDemo.Controllers
{
    public class TestMQController : Controller
    {
        IMQService _mqService;
        public TestMQController(IMQService mqService)
        {
            _mqService = mqService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string PublishMessage()
        {
            _mqService.PublishMessage<Question>(new Question(DateTime.Now.ToString()));
            return DateTime.Now.ToString();
        }
    }
}