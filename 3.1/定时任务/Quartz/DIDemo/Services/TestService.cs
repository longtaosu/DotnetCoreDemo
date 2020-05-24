using System;
using System.Collections.Generic;
using System.Text;

namespace DIDemo.Services
{
    public interface ITestService
    {
        void Print();
    }
    public class TestService : ITestService
    {
        public void Print()
        {
            Console.WriteLine($"时间打印：{DateTime.Now}");
        }
    }
}
