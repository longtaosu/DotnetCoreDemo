using ConsoleDemo.Obsoletes;
using System;

namespace ConsoleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            new Obsolete_A().Method();
            new Obsolete_B().NewMethod();
            new Obsolete_B().OldMethod();

            Console.WriteLine("Hello World!");
        }
    }
}
