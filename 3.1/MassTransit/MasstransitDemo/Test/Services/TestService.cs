using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test.Services
{
    public interface ITestService
    {
        Guid GetGuid();
    }
    public class TestService : ITestService
    {
        private Guid _guid;
        public TestService()
        {
            _guid = Guid.NewGuid();
        }

        public Guid GetGuid()
        {
            return _guid;
        }
    }
}
