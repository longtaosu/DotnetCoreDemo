﻿using EasyNetQ;
using System;

namespace EasyNetQDemo.Publish
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
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

            var bus = RabbitHutch.CreateBus("host=localhost;publisherConfirms=true");
            bus.Publish(payment1);
            bus.Publish(payment2);
            bus.Publish(payment3);
            bus.Publish(payment4);


            while(true)
            {
                string msg = Console.ReadLine();
                CardPaymentRequestMessage message = new CardPaymentRequestMessage
                {
                    Amount = DateTime.Now.Second,
                     CardHolderName=msg,
                      CardNumber=DateTime.Now.Millisecond.ToString(),
                       ExpiryDate=DateTime.Now.Hour.ToString()
                };
                bus.Publish(message);
            }


            Console.ReadLine();
        }
    }
}
