using EasyNetQ;
using EasyNetQDemo.Common.Polymorphic;
using System;

namespace EasyNetQDemo.PolymorphicPublish
{
    class Program
    {
        static int count = 0;
        static void Main(string[] args)
        {
            var bus = RabbitHutch.CreateBus("host=localhost");

            while(true)
            {
                var msg = Console.ReadLine();
                count++;
                if (count % 2 == 0)
                {
                    var message = new PurchaseOrder()
                    {
                        Amount = DateTime.Now.Second,
                        CompanyName = msg,
                        PaymentDayTerms = DateTime.Now.Millisecond,
                        PoNumber = DateTime.Now.Day.ToString()
                    };
                    bus.Publish<IPayment>(message);
                }

                else
                {
                    var message = new CardPayment()
                    {
                        Amount = DateTime.Now.Second,
                        CardHolderName = msg,
                        CardNumber = DateTime.Now.Millisecond.ToString(),
                        ExpiryDate = DateTime.Now.Day.ToString()
                    };
                    bus.Publish<IPayment>(message);
                }


            }
        }
    }
}
