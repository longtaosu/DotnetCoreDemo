using ConsoleModels;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleConsumer
{
    public class UpdateCustomerConsumer :
    IConsumer<ValueEntered>
    {
        public async Task Consume(ConsumeContext<ValueEntered> context)
        {
            await Console.Out.WriteLineAsync($"Updating customer: {context.Message.Value}");

            // update the customer address
        }
    }
}
