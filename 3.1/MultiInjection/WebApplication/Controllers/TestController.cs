using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.Services;

namespace WebApplication.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private ITestService _testService;
        private IEnumerable<ITestService> _testServices;
        public TestController(ITestService testService,IEnumerable<ITestService> testServices)
        {
            _testService = testService;
            _testServices = testServices;
        }
        [HttpGet]
        public void Test()
        {
            _testService.PrintInfo();
        }

        [HttpGet]
        public void TestAll()
        {
            foreach (var item in _testServices)
            {
                item.PrintInfo();
            }
        }

        public void TestSingle()
        {
            _testServices.Where(t => t.GetType().Name.Contains("bd",StringComparison.OrdinalIgnoreCase)).FirstOrDefault().PrintInfo();
        }
    }
}
