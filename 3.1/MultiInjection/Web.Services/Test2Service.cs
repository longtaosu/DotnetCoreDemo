using System;
using System.Collections.Generic;
using System.Text;

namespace Web.Services
{
    public class Test2Service : ITestService
    {
        public void PrintInfo()
        {
            Console.WriteLine($"test2,{DateTime.Now}");
        }
    }
}
