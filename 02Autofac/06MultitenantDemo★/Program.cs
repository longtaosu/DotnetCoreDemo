using Autofac;
using Autofac.Multitenant;
using System;

namespace AutofacDemo.MultitenantDemo
{
    class Program
    {
        /// <summary>
        /// 依赖解析的容器
        /// </summary>
        private static IContainer _container;
        /// <summary>
        /// 
        /// </summary>
        private static ManualTenantIdentificationStrategy _tenantIdentifier;


        static void Main(string[] args)
        {
            // Initialize the tenant identification strategy.
            _tenantIdentifier = new ManualTenantIdentificationStrategy();

            // Set the application container to the multitenant container.
            _container = ConfigureDependencies();

            // Explain what you're looking at.
            WriteInstructions();

            // Start listening for input.
            ListenForInput();
        }

        private static IContainer ConfigureDependencies()
        {
            //注册依赖
            var builder = new ContainerBuilder();
            builder.RegisterType<Consumer>().As<IDependencyConsumer>().InstancePerDependency();
            builder.RegisterType<BaseDependency>().As<IDependency>().SingleInstance();
            var appContainer = builder.Build();

            //创建多租户容器
            var mtc = new MultitenantContainer(_tenantIdentifier, appContainer);

            //重写租户1，注册依赖
            mtc.ConfigureTenant('1', b => b.RegisterType<Tenant1Dependency>().As<IDependency>().InstancePerDependency());

            //重写租户2，注册依赖
            mtc.ConfigureTenant('2', b => b.RegisterType<Tenant2Dependency>().As<IDependency>().InstancePerDependency());
           
            //重写默认租户，注册依赖
            mtc.ConfigureTenant(null, b => b.RegisterType<DefaultTenantDependency>().As<IDependency>().SingleInstance());

            return mtc;
        }


        /// <summary>
        /// Loops and listens for input until the user quits.
        /// </summary>
        private static void ListenForInput()
        {
            ConsoleKeyInfo input;
            do
            {
                Console.Write("Select a tenant (1-9, 0=default) or 'q' to quit: ");
                input = Console.ReadKey();
                Console.WriteLine();

                if (input.KeyChar >= 48 && input.KeyChar <= 57)
                {
                    // Set the "contextual" tenant ID based on the input, then
                    // put it to the test.
                    _tenantIdentifier.CurrentTenantId = input.KeyChar;
                    ResolveDependencyAndWriteInfo();
                }
                else if (input.Key != ConsoleKey.Q)
                {
                    Console.WriteLine("Invalid key pressed.");
                }
            } while (input.Key != ConsoleKey.Q);
        }

        /// <summary>
        /// Resolves the dependency from the container and displays some information
        /// about it.
        /// </summary>
        private static void ResolveDependencyAndWriteInfo()
        {
            var consumer = _container.Resolve<IDependencyConsumer>();
            Console.WriteLine("Tenant ID:       {0}", _tenantIdentifier.CurrentTenantId);
            Console.WriteLine("Dependency Type: {0}", consumer.Dependency.GetType().Name);
            Console.WriteLine("Instance ID:     {0}", consumer.Dependency.InstanceId);
            Console.WriteLine();
        }

        /// <summary>
        /// Writes the application instructions to the screen.
        /// </summary>
        private static void WriteInstructions()
        {
            Console.WriteLine("Multitenant Example: Console Application");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Select a tenant ID (1 - 9) to see the dependencies that get resolved for that tenant. You will see the dependency type as well as the instance ID so you can verify the proper type and lifetime scope registration is being used.");
            Console.WriteLine();
            Console.WriteLine("* Tenant 1 has an override registered as InstancePerDependency.");
            Console.WriteLine("* Tenant 2 has an override registered as SingleInstance.");
            Console.WriteLine("* The default tenant has an override registered as SingleInstance.");
            Console.WriteLine("* Tenants that don't have overrides fall back to the application/base singleton.");
            Console.WriteLine();
        }
    }
}
