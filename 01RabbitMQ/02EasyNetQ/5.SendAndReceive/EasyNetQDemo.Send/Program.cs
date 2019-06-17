using EasyNetQ;
using EasyNetQDemo.Common;
using System;

namespace EasyNetQDemo.Send
{
    class Program
    {
        static void Main(string[] args)
        {
            var payment1 = new CardPaymentRequestMessage
            {
                CardNumber = "1234123412341234",
                CardHolderName = "Mr F Bloggs",
                ExpiryDate = "12/12",
                Amount = 99.00m
            };

            var payment2 = new CardPaymentRequestMessage
            {
                CardNumber = "3456345634563456",
                CardHolderName = "Mr S Claws",
                ExpiryDate = "03/11",
                Amount = 15.00m
            };

            var payment3 = new CardPaymentRequestMessage
            {
                CardNumber = "6789678967896789",
                CardHolderName = "Mrs E Curry",
                ExpiryDate = "01/03",
                Amount = 1250.24m
            };

            var payment4 = new CardPaymentRequestMessage
            {
                CardNumber = "9991999299939994",
                CardHolderName = "Mrs D Parton",
                ExpiryDate = "04/07",
                Amount = 34.87m
            };

            var purchaseOrder1 = new PurchaseOrderRequestMessage
            {
                Amount = 134.25m,
                CompanyName = "Wayne Enterprises",
                PaymentDayTerms = 30,
                PoNumber = "BM666"
            };

            var purchaseOrder2 = new PurchaseOrderRequestMessage
            {
                Amount = 99.00m,
                CompanyName = "HeadBook",
                PaymentDayTerms = 30,
                PoNumber = "HB123"
            };

            var bus = RabbitHutch.CreateBus("host=localhost");
            Console.WriteLine("Publishing messages with send and receive.");
            Console.ReadLine();
            bus.Send("my.paymentsqueue", payment1);
            bus.Send("my.paymentsqueue", purchaseOrder1);
            bus.Send("my.paymentsqueue", payment2);
            bus.Send("my.paymentsqueue", payment3);
            bus.Send("my.paymentsqueue", purchaseOrder2);
            bus.Send("my.paymentsqueue", payment4);

            int count = 0;
            while(true)
            {
                count++;
                Console.ReadLine();
                if(count%3==0)
                {
                    bus.Send("my.paymentsqueue", payment1);
                }
                else
                {
                    bus.Send("my.paymentsqueue", purchaseOrder2);
                }
            }

            Console.ReadLine();
        }
    }
}
