using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleDemo.Obsoletes
{
    [Obsolete("class a is outtime")]
    public class Obsolete_A
    {
        public void Method()
        {
            Console.WriteLine("class a method");
        }
    }


    public class Obsolete_B
    {
        [Obsolete("use new method", true)]
        public void OldMethod()
        {
            Console.WriteLine("class b old method");
        }

        public void NewMethod()
        {
            Console.WriteLine("class b new method");
        }
    }
}
