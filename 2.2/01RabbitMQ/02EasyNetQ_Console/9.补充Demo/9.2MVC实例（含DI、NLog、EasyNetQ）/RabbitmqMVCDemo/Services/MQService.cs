using EasyNetQ;
using EasyNetQ.Consumer;
using RabbitmqMVCDemo.Common;
using RabbitmqMVCDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitmqMVCDemo.Services
{
    public class MQService : IMQService
    {
        #region 构造函数
        IBus bus;
        ITest _test;
        public MQService(ITest test)
        {
            _test = test;
        }
        #endregion

        #region MQ的发布、订阅及初始化
        public void InitMQ()
        {
            //声明队列并指定异常处理程序
            bus = RabbitHutch.CreateBus("host=192.144.189.146", x => x.Register<IConsumerErrorStrategy>(_ => new AlwaysRequeueErrorStrategy()));

            //订阅消息
            SubscribeMessage();
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        public void PublishMessage<T>(T message)
            where T : class
        {
            bus.Publish<T>(message);
        }
        /// <summary>
        /// 订阅消息
        /// </summary>
        public void SubscribeMessage()
        {
            //订阅 Question 类型的消息
            bus.SubscribeAsync<Question>("subscribe_question", x => HandleMessageAsync(x).Invoke(1));
        } 
        #endregion

        #region 业务程序
        private Func<int, Task> HandleMessageAsync(Question question)
        {
            _test.TestLog();
            return async (id) =>
            {
                if (new Random().Next(0, 2) == 0)
                {
                    Console.WriteLine("Exception Happened!!!!");
                    NLogHelper.Info("Exception Happened!!!!" + "   " + question.Text);
                    throw new Exception("Error Hanppened!" + "   " + question.Text);
                }
                else
                {
                    NLogHelper.Info("BEGIN");
                    Thread.Sleep(10000);
                    Console.WriteLine(string.Format("worker：{0}，content：{1}", id, question.Text));
                    NLogHelper.Info(string.Format("worker：{0}，content：{1}", id, question.Text));
                }
            };
        } 
        #endregion
    }
}
