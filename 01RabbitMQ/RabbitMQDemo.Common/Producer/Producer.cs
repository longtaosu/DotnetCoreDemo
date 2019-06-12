using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQDemo.Common
{
    public class Producer
    {
        #region 属性
        public string HostName { get; set; }
        private readonly ConnectionFactory factory; 
        #endregion

        #region 构造函数
        public Producer(string hostName = "localhost")
        {
            HostName = hostName;
            factory = new ConnectionFactory() { HostName = HostName };
        } 
        #endregion

        public void Send(string queue,string routingKey,  string message, string exchange = "", bool durable = false,bool exclusive = false,bool autoDelete = false)
        {
            using (var connection = factory.CreateConnection())
            {
                using(var channel = connection.CreateModel())
                {
                    //声明queue
                    channel.QueueDeclare(queue: queue,
                                         durable: durable,
                                         exclusive: exclusive,
                                         autoDelete: autoDelete,
                                         arguments: null);
                    //将消息转换为bytes数组，发送消息
                    var messageSend = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: exchange,
                                         routingKey: routingKey,
                                         basicProperties: null,
                                         body: messageSend);
                }
            }
        }

    }
}
