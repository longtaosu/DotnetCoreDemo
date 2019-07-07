using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AutofacDemo.GenericHostBuilderDemo
{
    public interface ILogger
    {
        Task Log(string value);
    }
}
