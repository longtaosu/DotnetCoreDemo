using System;
using System.Collections.Generic;
using System.Text;

namespace AutofacDemo.MultitenantDemo
{
    public class BaseDependency : IDependency
    {
        public BaseDependency()
        {
            this.InstanceId = Guid.NewGuid();
        }
        public Guid InstanceId { get; private set; }
    }
}
