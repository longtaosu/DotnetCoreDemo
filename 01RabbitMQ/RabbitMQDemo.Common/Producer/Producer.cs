using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQDemo.Common
{
    /// <summary>
    /// exchange type：direct、fanout
    /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queue">队列名称</param>
        /// <param name="routingKey"></param>
        /// <param name="message">发送的消息</param>
        /// <param name="exchange"></param>
        /// <param name="durable">是否持久化</param>
        /// <param name="exclusive"></param>
        /// <param name="autoDelete"></param>
        public void Send_WorkQueues(string queue, string routingKey, string message, string exchange = "", bool durable = false, bool exclusive = false, bool autoDelete = false)
        {
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //声明queue
                    channel.QueueDeclare(queue: queue,
                                         durable: durable,
                                         exclusive: exclusive,
                                         autoDelete: autoDelete,
                                         arguments: null);
                    //将消息转换为bytes数组，发送消息
                    var messageSend = Encoding.UTF8.GetBytes(message);
                    var properties = channel.CreateBasicProperties();
                    /*消息持久化*/
                    properties.Persistent = true;
                    channel.BasicPublish(exchange: exchange,
                                         routingKey: routingKey,
                                         basicProperties: properties,
                                         body: messageSend);
                }
            }
        }

        public void Send_PublishSubscribe(string queue, string routingKey, string message, string exchange = "", bool durable = false, bool exclusive = false, bool autoDelete = false)
        {
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "logs", type: "fanout");

                    //将消息转换为bytes数组，发送消息
                    var messageSend = Encoding.UTF8.GetBytes(message);
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    /*消息持久化*/
                    channel.BasicPublish(exchange: "logs",
                                         routingKey: "",
                                         basicProperties: properties,
                                         body: messageSend);
                }
            }
        }
        public void Send_Routing(string queue, string routingKey, string message, string exchange = "", bool durable = false, bool exclusive = false, bool autoDelete = false)
        {
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "direct_logs",
                                            type: "direct");
                    var severity = "info";
                    if (message.Contains("/"))
                        severity = "error";
                    else if (message.Contains("*"))
                        severity = "warn";
                    else
                        severity = "info";

                    //将消息转换为bytes数组，发送消息
                    var messageSend = Encoding.UTF8.GetBytes(message);
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    /*消息持久化*/
                    channel.BasicPublish(exchange: "direct_logs",
                                         routingKey: severity,
                                         basicProperties: properties,
                                         body: messageSend);
                }
            }
        }

    }
}
