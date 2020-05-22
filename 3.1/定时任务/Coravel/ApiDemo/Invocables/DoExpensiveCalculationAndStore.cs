using Coravel.Invocable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDemo.Invocables
{
    public class DoExpensiveCalculationAndStore : IInvocable
    {
        public async Task Invoke()
        {
            Console.Write("Doing expensive calculation for 15 sec...");
            await Task.Delay(15000);
            Console.Write("Expensive calculation done.");
        }
    }
}
