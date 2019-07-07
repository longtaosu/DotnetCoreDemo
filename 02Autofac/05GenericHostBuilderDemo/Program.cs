using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace AutofacDemo.GenericHostBuilderDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {

            await new HostBuilder()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory()).ConfigureContainer<ContainerBuilder>((context, builder) =>
                {
                    builder.RegisterType<Logger>().As<ILogger>();
                    builder.RegisterType<HostedService>().As<IHostedService>();
                }).RunConsoleAsync();
        }
    }
}
