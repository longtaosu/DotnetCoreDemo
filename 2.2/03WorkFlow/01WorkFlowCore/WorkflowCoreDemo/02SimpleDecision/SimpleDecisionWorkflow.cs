﻿using _02SimpleDecision.Steps;
using System;
using System.Collections.Generic;
using System.Text;
using WorkflowCore.Interface;

namespace _02SimpleDecision
{
    public class SimpleDecisionWorkflow : IWorkflow
    {
        public string Id => "Simple Decision Workflow";

        public int Version => 1;

        public void Build(IWorkflowBuilder<object> builder)
        {
            builder
                .StartWith<HelloWorld>()
                .Then<RandomOutput>(randomOutput =>
                {
                    randomOutput
                        .When(0)
                            .Then<CustomMessage>(cm =>
                            {
                                cm.Name("Print custom message");
                                cm.Input(step => step.Message, data => "Looping back....");
                            })
                            .Then(randomOutput);  //loop back to randomOutput

                    randomOutput
                        .When(1)
                            .Then<GoodbyeWorld>();
                });
        }
    }
}
