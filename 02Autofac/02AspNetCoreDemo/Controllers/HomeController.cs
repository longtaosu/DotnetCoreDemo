using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutofacDemo.AspNetCoreDemo.Models;
using AutofacDemo.AspNetCoreDemo.Services;

namespace AutofacDemo.AspNetCoreDemo.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/[controller]/[action]")]
    public class HomeController : Controller
    {
        private readonly IValuesService _valuesService;
        public HomeController(IValuesService valuesService)
        {
            _valuesService = valuesService;
        }
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var request = HttpContext.Request;
            return this._valuesService.FindAll();
        }
        //[HttpGet("{id}")]
        [HttpGet]
        public string Get1(int id)
        {
            return this._valuesService.Find(id);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
