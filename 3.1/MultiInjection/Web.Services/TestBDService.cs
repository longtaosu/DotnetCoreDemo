using System;
using System.Collections.Generic;
using System.Text;

namespace Web.Services
{
    public class TestBDService : ITestService
    {
        public void PrintInfo()
        {
            Console.WriteLine("bd");
        }
    }
}
