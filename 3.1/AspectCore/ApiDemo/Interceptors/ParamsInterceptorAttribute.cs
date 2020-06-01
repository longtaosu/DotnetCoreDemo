using AspectCore.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDemo.Interceptors
{
    public class ParamsInterceptorAttribute : AbstractInterceptorAttribute
    {
        private string _name;
        public ParamsInterceptorAttribute(string name)
        {
            this._name = name;
        }

        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            try
            {
                Console.WriteLine("Before service call" + _name);
                await next(context);
            }
            catch (Exception)
            {
                Console.WriteLine("Service threw an exception!" + _name);
                throw;
            }
            finally
            {
                Console.WriteLine("After service call" + _name);
            }
        }
    }
}
