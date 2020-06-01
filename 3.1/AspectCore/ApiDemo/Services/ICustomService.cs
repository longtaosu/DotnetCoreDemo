using ApiDemo.Interceptors;
using AspectCore.DynamicProxy.Parameters;
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

        
        void Call_Params();
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

        public void Call_Params()
        {
            if ((new Random().Next(1, 10)) % 2 == 0)
                Console.WriteLine("Service calling ...");
            else
                throw new Exception("error happening ...");
        }
    }
}
