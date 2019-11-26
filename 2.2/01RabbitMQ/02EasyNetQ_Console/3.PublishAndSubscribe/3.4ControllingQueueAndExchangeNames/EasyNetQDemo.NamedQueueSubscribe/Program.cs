using EasyNetQDemo.Common;
using EasyNetQ;
using System;

namespace EasyNetQDemo.NamedQueueSubscribe
{
    class Program
    {
        static void Main(string[] args)
        {
            var bus = RabbitHutch.CreateBus("host=localhost");
            bus.Subscribe<CardPaymentNamedQueue>(string.Empty, HandleCardPaymentMessage);
        }

        static void HandleCardPaymentMessage(CardPaymentNamedQueue paymentMessage)
        {
            Console.WriteLine("Payment = <" +
                              paymentMessage.CardNumber + ", " +
                              paymentMessage.CardHolderName + ", " +
                              paymentMessage.ExpiryDate + ", " +
                              paymentMessage.Amount + ">");
        }
    }
}
