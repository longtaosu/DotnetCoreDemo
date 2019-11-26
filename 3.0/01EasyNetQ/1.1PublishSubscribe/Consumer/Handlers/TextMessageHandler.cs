using EasyNetQ.AutoSubscribe;
using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Consumer.Handlers
{
    public class TextMessageHandler:IConsume<TextMessage>
    {
        public void Consume(TextMessage message)
        {
            //some business code 
        }
    }
}
