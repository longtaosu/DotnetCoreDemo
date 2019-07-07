using System;
using System.Collections.Generic;
using System.Text;

namespace AutofacDemo.MultitenantDemo
{
    public class Consumer : IDependencyConsumer
    {
        public IDependency Dependency { get; private set; }

        public Consumer(IDependency dependency)
        {
            this.Dependency = dependency;
        }
    }
}
