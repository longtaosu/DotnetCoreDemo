using EasyNetQDemo.Common;
using System;
using EasyNetQ;
using System.Threading;

namespace EasyNetQDemo.Response
{
    class Program
    {
        static void Main(string[] args)
        {
            var bus = RabbitHutch.CreateBus("host=localhost");
            bus.Respond<CardPaymentRequestMessage, CardPaymentResponseMessage>(Responder);
        }

        static CardPaymentResponseMessage Responder(CardPaymentRequestMessage request)
        {
            Thread.Sleep(2000);
            return new CardPaymentResponseMessage { AuthCode = DateTime.Now.ToString() };
        }
    }
}
