using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Jobs
{
    public class TestJob1 : IJob
    {

        Task IJob.Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            });
        }
    }
}
