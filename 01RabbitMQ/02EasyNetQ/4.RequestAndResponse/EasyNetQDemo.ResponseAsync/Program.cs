using EasyNetQ;
using EasyNetQDemo.Common;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace EasyNetQDemo.ResponseAsync
{
    class Program
    {
        public class MyWorker
        {
            public int order { get; set; }
            public CardPaymentResponseMessage Execute(CardPaymentRequestMessage request)
            {
                //Thread.Sleep(5000);
                CardPaymentResponseMessage responseMessage = new CardPaymentResponseMessage();
                responseMessage.AuthCode = "worker：" + order.ToString();
                Console.WriteLine("Worker activated to process response.");

                return responseMessage;
            }
        }
        static void Main(string[] args)
        {
            // Create a group of worker objects
            var workers = new BlockingCollection<MyWorker>();
            for (int i = 0; i < 10; i++)
            {
                workers.Add(new MyWorker() {  order = i});
            }

            using (var bus = RabbitHutch.CreateBus("host=localhost"))
            {
                // Respond to requests
                bus.RespondAsync<CardPaymentRequestMessage, CardPaymentResponseMessage>(request =>
                    Task.Factory.StartNew(() =>
                    {
                        var worker = workers.Take();
                        try
                        {
                            return worker.Execute(request);
                        }
                        finally
                        {
                            workers.Add(worker);
                        }
                    }));

                Console.WriteLine("Listening for messages. Hit <return> to quit.");
                Console.ReadLine();
            }
        }
    }
}
