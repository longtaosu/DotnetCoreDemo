using Autofac;
using System;

namespace AutofacDemo
{
    class Program
    {
        private static IContainer container;
        static void Main(string[] args)
        {
            AutoDI();
            //可以直接从 Container 解析组件，但是这种方法并不推荐
            container.Resolve<IDateWriter>().WriteDate();
            container.Resolve<IDateWriter>().WriteDate();

            //从容器中创建一个 子生命周期 并从中解析，当完成了解析组件，释放掉生命周期，其他所有也就随之清理
            //服务定位器模式是一种反模式，在代码中四处人为地创建生命周期而少量的使用容器并不是最佳的方式
            //using (var scope = container.BeginLifetimeScope())
            //{
            //    var writer = scope.Resolve<IDateWriter>();
            //    writer.WriteDate();
            //}
            //using (var scope = container.BeginLifetimeScope())
            //{
            //    var writer = scope.Resolve<IDateWriter>();
            //    writer.WriteDate();
            //}

            Console.ReadLine();
        }

        private static void AutoDI()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ConsoleOutput>().As<IOutput>();
            builder.RegisterType<TodayWriter>().AsSelf().As<IDateWriter>();

            container = builder.Build();
        }
    }
}
