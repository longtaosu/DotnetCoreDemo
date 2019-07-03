using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace WorkFlowCoreDemo.HelloWorld.Steps
{
    public class GoodbyeWorld : StepBody
    {
        private ILogger _logger;
        public GoodbyeWorld(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GoodbyeWorld>();
        }
        public override ExecutionResult Run(IStepExecutionContext context)
        {
            Console.WriteLine("Goodbye World");
            _logger.LogInformation("hi there");
            return ExecutionResult.Next();
        }
    }
}
