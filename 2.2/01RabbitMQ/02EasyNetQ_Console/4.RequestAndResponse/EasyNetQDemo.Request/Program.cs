using System;
using EasyNetQ;
using EasyNetQDemo.Common;

namespace EasyNetQDemo.Request
{
    class Program
    {
        static void Main(string[] args)
        {
            var bus = RabbitHutch.CreateBus("host=localhost");
            while(true)
            {
                var msg = Console.ReadLine();
                var message = new CardPaymentRequestMessage
                {
                    Amount = DateTime.Now.Hour,
                    CardHolderName = msg,
                    CardNumber = DateTime.Now.Year.ToString(),
                    ExpiryDate = DateTime.Now.Month.ToString()
                };
                var response = bus.Request<CardPaymentRequestMessage, CardPaymentResponseMessage>(message);
                Console.WriteLine(response.AuthCode);
            }
        }
    }
}
