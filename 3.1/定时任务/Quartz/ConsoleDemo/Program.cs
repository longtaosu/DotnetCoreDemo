using System;
using Topshelf;

namespace ConsoleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<ServiceRunner>();

                x.RunAsLocalSystem();

                x.SetDescription("Quartz测试服务");
                x.SetDisplayName("Quartz定时服务");
                x.SetServiceName("Quartz定时服务");

                x.EnablePauseAndContinue();
            });
        }
    }
}
