using EasyNetQ;
using System;
using System.Threading.Tasks;

namespace EasyNetQDemo.SubscribeAsync
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            var bus = RabbitHutch.CreateBus("host=localhost");
            bus.SubscribeAsync<CardPaymentRequestMessage>("cardPayment", HandleCardPaymentMessage);

            Console.ReadLine();
        }

        static Task HandleCardPaymentMessage(CardPaymentRequestMessage paymentMessage)
        {
            return Task.Run(() =>
            {
                Console.WriteLine("Payment = <" +
                  paymentMessage.CardNumber + ", " +
                  paymentMessage.CardHolderName + ", " +
                  paymentMessage.ExpiryDate + ", " +
                  paymentMessage.Amount + ">");
            });

        }
    }
}
