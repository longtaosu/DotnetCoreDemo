using EasyNetQ.Consumer;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyNetQDemo.ErrorHandler
{
    public sealed class AlwaysRequeueErrorStrategy : IConsumerErrorStrategy
    {
        public void Dispose()
        {
        }

        public AckStrategy HandleConsumerError(ConsumerExecutionContext context, Exception exception)
        {
            return AckStrategies.NackWithRequeue;
        }

        public AckStrategy HandleConsumerCancelled(ConsumerExecutionContext context)
        {
            return AckStrategies.NackWithRequeue;
        }
    }
}
