using DIDemo.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ConsoleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            #region Singleton 测试
            //services.AddSingleton<IOperationSingleton, Operation>();
            //services.AddSingleton<IOperationSingleton>(new Operation(Guid.Empty));
            //services.AddSingleton<IOperationSingleton>(new Operation(Guid.NewGuid()));

            //var provider = services.BuildServiceProvider();

            //var singletone1 = provider.GetService<IOperationSingleton>();
            //Console.WriteLine($"signletone1: {singletone1.OperationId}");

            //// 输出singletone2的Guid
            //var singletone2 = provider.GetService<IOperationSingleton>();
            //Console.WriteLine($"signletone2: {singletone2.OperationId}");
            //Console.WriteLine($"singletone1 == singletone2 ? : { singletone1 == singletone2 }"); 
            #endregion

            #region Tranisent 测试
            //services.AddTransient<IOperationTransient, Operation>();
            //var provider = services.BuildServiceProvider();
            //var transient1 = provider.GetService<IOperationTransient>();
            //var transient2 = provider.GetService<IOperationTransient>();
            //Console.WriteLine($"transient1:{transient1.OperationId}");
            //Console.WriteLine($"transient2:{transient2.OperationId}");
            //Console.WriteLine($"transient1 == transient2 ? : { transient1 == transient2 }");
            #endregion


            #region Scoped 测试
            services.AddScoped<IOperationScoped, Operation>()
                    .AddTransient<IOperationTransient, Operation>()
                    .AddSingleton<IOperationSingleton, Operation>();

            var provider = services.BuildServiceProvider();
            using (var scope1 = provider.CreateScope())
            {
                var p = scope1.ServiceProvider;

                var scopeobj1 = p.GetService<IOperationScoped>();
                var transient1 = p.GetService<IOperationTransient>();
                var singleton1 = p.GetService<IOperationSingleton>();

                var scopeobj2 = p.GetService<IOperationScoped>();
                var transient2 = p.GetService<IOperationTransient>();
                var singleton2 = p.GetService<IOperationSingleton>();

                Console.WriteLine(
                    $"scope1: { scopeobj1.OperationId }, " +
                    $"transient1: {transient1.OperationId}, " +
                    $"singleton1: {singleton1.OperationId}");

                Console.WriteLine($"scope2: { scopeobj2.OperationId }, " +
                    $"transient2: {transient2.OperationId}, " +
                    $"singleton2: {singleton2.OperationId}");
            }
            Console.WriteLine();
            using (var scope1 = provider.CreateScope())
            {
                var p = scope1.ServiceProvider;

                var scopeobj1 = p.GetService<IOperationScoped>();
                var transient1 = p.GetService<IOperationTransient>();
                var singleton1 = p.GetService<IOperationSingleton>();

                var scopeobj2 = p.GetService<IOperationScoped>();
                var transient2 = p.GetService<IOperationTransient>();
                var singleton2 = p.GetService<IOperationSingleton>();

                Console.WriteLine(
                    $"scope1: { scopeobj1.OperationId }, " +
                    $"transient1: {transient1.OperationId}, " +
                    $"singleton1: {singleton1.OperationId}");
                Console.WriteLine($"scope2: { scopeobj2.OperationId }, " +
                    $"transient2: {transient2.OperationId}, " +
                    $"singleton2: {singleton2.OperationId}");
            }
            #endregion

            Console.ReadLine();
        }
    }
}
