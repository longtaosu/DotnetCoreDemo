using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RabbitMQDemo.Common
{
    public class Customer
    {
        #region 属性
        public string HostName { get; set; }
        private readonly ConnectionFactory factory;
        #endregion

        #region 构造函数
        public Customer(string hostName = "localhost")
        {
            HostName = hostName;
            factory = new ConnectionFactory() { HostName = HostName };
        }
        #endregion

        public void Receive(string queue, Action<string> action, string exchange = "", bool durable = false, bool exclusive = false, bool autoDelete = false)
        {
            //消息结果
            string message;
            //创建连接
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //声明队列
                channel.QueueDeclare(queue: queue,
                                     durable: durable,
                                     exclusive: exclusive,
                                     autoDelete: autoDelete,
                                     arguments: null);
                //通过事件订阅消息
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    message = Encoding.UTF8.GetString(body);
                    action.Invoke(message);
                };
                channel.BasicConsume(queue: queue,
                                     autoAck: true,
                                     consumer: consumer);
                //一旦退出则无法接收消息
                while(true)
                {
                    //Console.ReadLine();
                }

            }
        }


    }
}
