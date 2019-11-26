using EasyNetQ;
using EasyNetQDemo.Common.Polymorphic;
using System;

namespace EasyNetQDemo.PurchaseOrderTopicSubscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var bus = RabbitHutch.CreateBus("host=localhost"))
            {
                bus.Subscribe<IPayment>("purchaseorders", Handler, x => x.WithTopic("payment.purchaseorder"));

                Console.WriteLine("Listening for (payment.purchaseorer) messages. Hit <return> to quit.");
                Console.ReadLine();

            }
        }

        public static void Handler(IPayment payment)
        {
            var purchaseOrder = payment as PurchaseOrder;

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
