using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeUsageDemo.AttributeUsages
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class NewAttribute : Attribute
    {
    }
}
