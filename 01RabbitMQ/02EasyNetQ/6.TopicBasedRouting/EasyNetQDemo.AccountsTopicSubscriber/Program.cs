using EasyNetQ;
using EasyNetQDemo.Common.Polymorphic;
using System;

namespace EasyNetQDemo.AccountsTopicSubscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var bus = RabbitHutch.CreateBus("host=localhost"))
            {
                bus.Subscribe<IPayment>("accounts", Handler, x => x.WithTopic("payment.*"));

                Console.WriteLine("Listening for (payment.*) messages. Hit <return> to quit.");
                Console.ReadLine();
            }
        }

        public static void Handler(IPayment payment)
        {
            var cardPayment = payment as CardPayment;
            var purchaseOrder = payment as PurchaseOrder;

            if (cardPayment != null)
            {
                Console.WriteLine("Processing Card Payment = <" +
                                  cardPayment.CardNumber + ", " +
                                  cardPayment.CardHolderName + ", " +
                                  cardPayment.ExpiryDate + ", " +
                                  cardPayment.Amount + ">");
            }

            if (purchaseOrder != null)
            {
                Console.WriteLine("Processing Purchase Order = <" +
                                  purchaseOrder.CompanyName + ", " +
                                  purchaseOrder.PoNumber + ", " +
                                  purchaseOrder.PaymentDayTerms + ", " +
                                  purchaseOrder.Amount + ">");
            }
        }
    }
}
