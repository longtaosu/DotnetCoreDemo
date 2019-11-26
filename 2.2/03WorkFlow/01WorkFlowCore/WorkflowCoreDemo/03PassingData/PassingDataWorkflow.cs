using _03PassingData.Steps;
using System;
using System.Collections.Generic;
using System.Text;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace _03PassingData
{
    public class PassingDataWorkflow : IWorkflow<MyDataClass>
    {
        public string Id => "PassingDataWorkflow";

        public int Version => 1;

        public void Build(IWorkflowBuilder<MyDataClass> builder)
        {
            builder
                .StartWith(context =>
                {
                    Console.WriteLine("Starting workflow...");
                    return ExecutionResult.Next();
                })
                .Then<AddNumbers>()
                    .Input(step => step.Input1, data => data.Value1)
                    .Input(step => step.Input2, data => data.Value2)
                    .Output(data => data.Value3, step => step.Output)
                .Then<CustomMessage>()
                    .Name("Print custom message")
                    .Input(step => step.Message, data => "The answer is " + data.Value3.ToString())
                .Then(context =>
                {
                    Console.WriteLine("Workflow complete");
                    return ExecutionResult.Next();
                });
        }
    }
}
