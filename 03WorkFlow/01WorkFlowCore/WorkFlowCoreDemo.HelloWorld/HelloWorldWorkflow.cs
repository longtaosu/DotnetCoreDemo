using System;
using System.Collections.Generic;
using System.Text;
using WorkflowCore.Interface;

namespace WorkFlowCoreDemo.HelloWorld
{
    public class HelloWorldWorkflow : IWorkflow
    {
        public string Id => "Hello World";

        public int Version => 1;

        public void Build(IWorkflowBuilder<object> builder)
        {
            builder.StartWith<Steps.HelloWorld>()
                .Then<Steps.GoodbyeWorld>();
        }
    }
}
