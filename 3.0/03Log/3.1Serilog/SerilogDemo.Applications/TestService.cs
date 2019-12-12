using Microsoft.Extensions.Logging;
using System;

namespace SerilogDemo.Applications
{
    public class TestService : ITestService
    {
        private readonly ILogger<TestService> _logger;
        public TestService(ILogger<TestService> logger)
        {
            _logger = logger;
        }
        public bool TestLog(string str)
        {
            _logger.LogInformation("info - " + DateTime.Now.ToString());
            _logger.LogError("error - " + DateTime.Now.ToString());
            _logger.LogCritical("this is service log中文测试 ：{0}", DateTime.Now.ToString());
            return true;
        }
    }
}
