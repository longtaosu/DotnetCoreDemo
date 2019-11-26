using System;
using System.Threading;
using EasyNetQ;

namespace EasyNetQDemo.Subscribe
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            var bus = RabbitHutch.CreateBus("host=localhost");
            bus.Subscribe<CardPaymentRequestMessage>("cardPayment", HandleCardPaymentMessage);

            Console.ReadLine();
        }

        static void HandleCardPaymentMessage(CardPaymentRequestMessage paymentMessage)
        {
            Thread.Sleep(3000);

                //Thread.Sleep(new TimeSpan(1, 0, 0));
            Console.WriteLine("Payment = <" +
                              paymentMessage.CardNumber + ", " +
                              paymentMessage.CardHolderName + ", " +
                              paymentMessage.ExpiryDate + ", " +
                              paymentMessage.Amount + ">");
        }
    }
}
