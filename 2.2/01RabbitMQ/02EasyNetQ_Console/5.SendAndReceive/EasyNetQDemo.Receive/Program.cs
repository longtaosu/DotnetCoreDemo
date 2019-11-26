using EasyNetQ;
using System;

namespace EasyNetQDemo.Receive
{
    class Program
    {
        static void Main(string[] args)
        {
            var bus = RabbitHutch.CreateBus("host=localhost");
            bus.Receive<CardPaymentRequestMessage>("my.paymentsqueue", message => HandleCardPaymentMessage(message));

            Console.WriteLine("Listening for messages. Hit <return> to quit.");
            Console.ReadLine();

        }

        static void HandleCardPaymentMessage(CardPaymentRequestMessage paymentMessage)
        {
            Console.WriteLine("Processing Payment = <" +
                              paymentMessage.CardNumber + ", " +
                              paymentMessage.CardHolderName + ", " +
                              paymentMessage.ExpiryDate + ", " +
                              paymentMessage.Amount + ">");
        }
    }
}
