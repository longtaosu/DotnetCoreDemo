using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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

        #region HelloWorld
        public void Send(string queue, string routingKey, string message, string exchange = "", bool durable = false, bool exclusive = false, bool autoDelete = false)
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
                    channel.BasicPublish(exchange: exchange,
                                         routingKey: routingKey,
                                         basicProperties: null,
                                         body: messageSend);
                }
            }
        }
        #endregion

        #region WorkQueues
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
        #endregion

        #region PublishSubscribe
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
        #endregion

        #region Routing
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
        #endregion

        #region Topics
        public void Send_Topics(string queue, string routingKey, string message, string exchange = "", bool durable = false, bool exclusive = false, bool autoDelete = false)
        {
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "topic_logs",
                                            type: "topic");
                    string[] msg = message.Split(' ');
                    routingKey = msg.Length > 0 ? msg[0] : "anonymous.info";

                    //将消息转换为bytes数组，发送消息
                    var messageSend = Encoding.UTF8.GetBytes(message);
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    /*消息持久化*/
                    channel.BasicPublish(exchange: "topic_logs",
                                         routingKey: routingKey,
                                         basicProperties: properties,
                                         body: messageSend);
                }
            }
        }
        #endregion

        #region RPC
        public void Send_RPC(string queue, string routingKey, string message, string exchange = "", bool durable = false, bool exclusive = false, bool autoDelete = false)
        {
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //声明队列
                    channel.QueueDeclare(queue: "rpc_queue"
                                        , durable: false
                                        , exclusive: false
                                        , autoDelete: false
                                        , arguments: null);
                    channel.BasicQos(0, 1, false);

                    var consumer = new EventingBasicConsumer(channel);
                    channel.BasicConsume(queue: "rpc_queue",
                                        autoAck: false,
                                        consumer: consumer);
                    consumer.Received += (model,ea)=> {
                        string response = null;

                        var body = ea.Body;
                        var props = channel.CreateBasicProperties();
                        var replyProps = channel.CreateBasicProperties();
                        replyProps.CorrelationId = props.CorrelationId;

                        try
                        {
                            var messages = Encoding.UTF8.GetString(body);
                            int n = int.Parse(messages);
                            Console.WriteLine(" [.]fib({0})", messages);
                            response = fib(n).ToString();
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(" [.]" + ex.Message);
                            response = "";
                        }
                        finally
                        {
                            var responseBytes = Encoding.UTF8.GetBytes(response);
                            channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                              basicProperties: replyProps, body: responseBytes);
                            channel.BasicAck(deliveryTag: ea.DeliveryTag,
                              multiple: false);
                        }
                    };



                    channel.ExchangeDeclare(exchange: "topic_logs",
                                            type: "topic");
                    string[] msg = message.Split(' ');
                    routingKey = msg.Length > 0 ? msg[0] : "anonymous.info";

                    //将消息转换为bytes数组，发送消息
                    var messageSend = Encoding.UTF8.GetBytes(message);
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    /*消息持久化*/
                    channel.BasicPublish(exchange: "topic_logs",
                                         routingKey: routingKey,
                                         basicProperties: properties,
                                         body: messageSend);
                }
            }
        }

        private static int fib(int n)
        {
            if (n == 0 || n == 1)
                return n;

            return fib(n - 1) + fib(n - 2);
        }


        #endregion

    }
}
