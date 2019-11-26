using AutofacDemo.ConfigurationInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutofacDemo.ConfigurationDemo
{
    public class InternalPlugin : IPlugin
    {
        public string Name
        {
            get { return "InternalPlugin"; }
        }
    }
}
