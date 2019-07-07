using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AutofacDemo.GenericHostBuilderDemo
{
    public class Logger : ILogger
    {
        public async Task Log(string value)
        {
            await Console.Out.WriteLineAsync($"Logger:{value}");
        }
    }
}
