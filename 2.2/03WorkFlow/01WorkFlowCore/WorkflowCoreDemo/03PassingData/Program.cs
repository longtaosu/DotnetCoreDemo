﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using WorkflowCore.Interface;

namespace _03PassingData
{
    class Program
    {
        static void Main(string[] args)
        {
            IServiceProvider serviceProvider = ConfigureServices();

            //start the workflow host
            var host = serviceProvider.GetService<IWorkflowHost>();
            host.RegisterWorkflow<PassingDataWorkflow, MyDataClass>();
            host.RegisterWorkflow<PassingDataWorkflow2, Dictionary<string, int>>();
            host.Start();

            var initialData = new MyDataClass
            {
                Value1 = 2,
                Value2 = 3
            };

            //host.StartWorkflow("PassingDataWorkflow", 1, initialData);


            var initialData2 = new Dictionary<string, int>
            {
                ["Value1"] = 7,
                ["Value2"] = 2
            };

            host.StartWorkflow("PassingDataWorkflow2", 1, initialData2);

            Console.ReadLine();
            host.Stop();
        }

        private static IServiceProvider ConfigureServices()
        {
            //setup dependency injection
            IServiceCollection services = new ServiceCollection();
            services.AddLogging();
            services.AddWorkflow();
            //services.AddWorkflow(x => x.UseSqlServer(@"Server=.\SQLEXPRESS;Database=WorkflowCore;Trusted_Connection=True;", true, true));
            var serviceProvider = services.BuildServiceProvider();

            //config logging
            //var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            //loggerFactory.AddDebug();
            return serviceProvider;
        }
    }
}