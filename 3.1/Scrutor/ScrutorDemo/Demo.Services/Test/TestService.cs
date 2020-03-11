using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Services
{
    public class TestService : ITestService
    {
        public string GetTime()
        {
            return DateTime.Now.ToString();
        }
    }
}
