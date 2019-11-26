using RabbitmqMVCDemo.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitmqMVCDemo.Services
{
    public interface ITest
    {
        void TestLog();
    }
    public class Test : ITest
    {
        public void TestLog()
        {
            NLogHelper.Error("dddd");
        }
    }
}
