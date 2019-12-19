using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace LTS.Services
{
    public interface ITestService
    {
        void Test();
    }
    public class TestService:ITestService
    {
        private readonly ILogger<TestService> _logger;
        public TestService(ILogger<TestService> logger)
        {
            _logger = logger;
        }
        public void Test()
        {
            _logger.LogInformation("该记录来自服务，时间" + DateTime.Now.ToString());
            int number = 0;
            var result = 10 / number;

        }
    }
}
