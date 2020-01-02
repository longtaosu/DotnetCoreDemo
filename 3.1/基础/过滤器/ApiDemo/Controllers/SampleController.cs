using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDemo.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ApiDemo.Controllers
{
    [AddHeader("Author","Joe Smith")]
    public class SampleController : Controller
    {
        public IActionResult Index()
        {
            //return View();
            return Content("Examine the headers using the F12 developer tools.");
        }

        [ShortCircuitingResourceFilter]
        public IActionResult SomeResource()
        {
            return Content("Successful access to resource - header is set.");
        }
    }
}