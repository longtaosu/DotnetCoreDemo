using EasyNetQ;
using EasyNetQDemo.Common;
using System;

namespace EasyNetQDemo.ReceiveMultiple
{
    class Program
    {
        static void Main(string[] args)
        {
            var bus = RabbitHutch.CreateBus("host=localhost");

            bus.Receive("my.paymentsqueue", x => x
                        .Add<CardPaymentRequestMessage>(message => HandleCardPaymentMessage(message))
                        .Add<PurchaseOrderRequestMessage>(message => HandlePurchaseOrderPaymentMessage(message)));

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

        static void HandlePurchaseOrderPaymentMessage(PurchaseOrderRequestMessage purchaseOrder)
        {
            Console.WriteLine("Processing Purchase Order = <" +
                               purchaseOrder.CompanyName + ", " +
                               purchaseOrder.PoNumber + ", " +
                               purchaseOrder.PaymentDayTerms + ", " +
                               purchaseOrder.Amount + ">");
        }
    }
}
