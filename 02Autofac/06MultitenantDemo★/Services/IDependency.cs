using System;
using System.Collections.Generic;
using System.Text;

namespace AutofacDemo.MultitenantDemo
{
    public interface IDependency
    {
        Guid InstanceId { get; }
    }
}
