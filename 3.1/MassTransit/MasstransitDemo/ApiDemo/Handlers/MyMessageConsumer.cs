using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDemo.Handlers
{
    public class MyMessageConsumer : IConsumer<MyMessage>
    {
        public Task Consume(ConsumeContext<MyMessage> context)
        {
            return Task.Run(() =>
            {
                Console.WriteLine("次数:{0}", context.GetRetryCount());
                if(DateTime.Now.Second%2==0)

                Console.WriteLine("Customer address was updated: {0},from comsume", context.Message.CustomerId);
                else
                {
                    Console.WriteLine("exception occured, time : {0}", DateTime.Now);
                    throw new Exception();
                }
                //context.Publish(new MyMessage()
                //{
                //    CustomerId = DateTime.Now.Second.ToString()
                //});
            });
        }
    }

    public class MyMessageObserver : IObserver<ConsumeContext<MyMessage>>
    {
        public void OnCompleted()
        {
            //throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            //throw new NotImplementedException();
        }

        public void OnNext(ConsumeContext<MyMessage> context)
        {
            Console.WriteLine("Customer address was updated: {0}, from observer", context.Message.CustomerId);
        }
    }

    public class MyMessage
    {
        public string CustomerId { get; set; }
    }
}
