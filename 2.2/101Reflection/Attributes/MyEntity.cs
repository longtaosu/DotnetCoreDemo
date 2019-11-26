using System;
using System.Collections.Generic;
using System.Text;

namespace _01Reflection
{
    [MyAttribute("demo1",22)]
    public class MyEntity
    {
        [MyAttribute("属性1",16)]
        public string Property1 { get; set; }

        [MyAttribute("属性2",18)]
        public string Property2 { get; set; }
    }
}
