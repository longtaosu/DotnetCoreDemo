using System;
using System.Collections.Generic;
using System.Text;
using EasyNetQ;

namespace Messages
{
    [Queue(queueName: "ClientQueue", ExchangeName = "ClientExchange")]
    public class TextMessage
    {
        public string Text { get; set; }
    }
}
