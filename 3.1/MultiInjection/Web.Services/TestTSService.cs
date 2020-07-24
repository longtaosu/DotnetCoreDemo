using System;
using System.Collections.Generic;
using System.Text;

namespace Web.Services
{
    public class TestTSService : ITestService
    {
        public void PrintInfo()
        {
            Console.WriteLine("TS");
        }
    }
}
