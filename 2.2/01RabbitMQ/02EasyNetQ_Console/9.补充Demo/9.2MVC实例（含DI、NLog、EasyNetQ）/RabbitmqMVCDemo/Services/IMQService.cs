using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitmqMVCDemo.Services
{
    public interface IMQService
    {
        /// <summary>
        /// 
        /// </summary>
        void InitMQ();
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        void PublishMessage<T>(T message)
            where T : class;
        /// <summary>
        /// 订阅消息
        /// </summary>
        void SubscribeMessage();
    }
}
