using ApiDemo.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDemo.Services
{
    public interface ICustomService
    {
        [CustomInterceptor]
        void Call();
    }

    public class CustomService : ICustomService
    {
        public void Call()
        {
            if ((new Random().Next(1, 10)) % 2 == 0)
                Console.WriteLine("Service calling ...");
            else 
                throw new Exception("error happening ...");
        }
    }
}
