using System;
using System.Collections.Generic;
using System.Text;

namespace Web.Services
{
    public class Test1Service : ITestService
    {
        public void PrintInfo()
        {
            Console.WriteLine($"test1,{DateTime.Now}");
        }
    }
}
