using System;
using System.IO;
using System.Reflection;
using Topshelf;

namespace TopShelfDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //https://github.com/Topshelf/Topshelf/issues/473
            //issue：服务没有及时响应启动或者控制请求
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Directory.SetCurrentDirectory(path);
            //issue结束


            HostFactory.Run(x => {
                x.Service<ServiceRunner>();

                //x.RunAsLocalSystem();
                x.SetDescription("windows服务");
                x.SetDisplayName("windows服务");
                x.SetServiceName("windows服务");

                x.StartAutomatically();
            });
        }
    }
}
