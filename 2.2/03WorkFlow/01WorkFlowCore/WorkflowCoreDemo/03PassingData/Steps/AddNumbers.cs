using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace _03PassingData.Steps
{
    public class AddNumbers : StepBodyAsync
    {
        public int Input1 { get; set; }

        public int Input2 { get; set; }

        public int Output { get; set; }
        public override Task<ExecutionResult> RunAsync(IStepExecutionContext context)
        {
            Output = (Input1 + Input2);
            return Task.FromResult( ExecutionResult.Next());
        }
    }
}
