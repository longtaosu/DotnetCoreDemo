﻿using RabbitMQ.Client;
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

        #region HelloWorld
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
                while (true)
                {
                    //Console.ReadLine();
                }

            }
        }
        #endregion

        #region WorkQueues
        public void Receive_WorkQueues(string queue, Action<string> action, string exchange = "", bool durable = false, bool exclusive = false, bool autoDelete = false)
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
                //将消息分发到空闲的worker
                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                //通过事件订阅消息
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    message = Encoding.UTF8.GetString(body);
                    action.Invoke(message);
                    /*手动发送消息确认信息*/
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: queue,
                                     autoAck: false,//消息的默认ack关闭
                                     consumer: consumer);
                //一旦退出则无法接收消息
                while (true)
                {
                    //Console.ReadLine();
                }

            }
        }
        #endregion

        #region PublishSubscribe
        public void Receive_PublishSubscribe(string queue, Action<string> action, string exchange = "", bool durable = false, bool exclusive = false, bool autoDelete = false)
        {
            //消息结果
            string message;
            //创建连接
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //声明Exchange
                channel.ExchangeDeclare(exchange: "logs", type: "fanout");
                //获取queueName队列名称
                var queueName = channel.QueueDeclare().QueueName;
                //队列绑定Exchange
                channel.QueueBind(queue: queueName,
                                  exchange: "logs",
                                  routingKey: "");

                //声明队列
                channel.QueueDeclare(queue: queue,
                                     durable: durable,
                                     exclusive: exclusive,
                                     autoDelete: autoDelete,
                                     arguments: null);
                //将消息分发到空闲的worker
                //channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                //通过事件订阅消息
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    message = Encoding.UTF8.GetString(body);
                    action.Invoke(message);
                    /*手动发送消息确认信息*/
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: queueName,
                                     autoAck: false,//消息的默认ack关闭
                                     consumer: consumer);
                //一旦退出则无法接收消息
                while (true)
                {
                    //Console.ReadLine();
                }

            }
        }
        #endregion

        #region Routing
        public void Receive_Routing(string queue, Action<string> action, string routingKey, string exchange = "", bool durable = false, bool exclusive = false, bool autoDelete = false)
        {
            //消息结果
            string message;
            //创建连接
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //声明Exchange
                channel.ExchangeDeclare(exchange: "direct_logs",
                                        type: "direct");
                //获取queueName队列名称
                var queueName = channel.QueueDeclare().QueueName;
                //队列绑定Exchange
                channel.QueueBind(queue: queueName,
                                  exchange: "direct_logs",
                                  routingKey: routingKey);

                //声明队列
                //channel.QueueDeclare(queue: queue,
                //                     durable: durable,
                //                     exclusive: exclusive,
                //                     autoDelete: autoDelete,
                //                     arguments: null);
                //将消息分发到空闲的worker
                //channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                //通过事件订阅消息
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    message = Encoding.UTF8.GetString(body);
                    action.Invoke(message);
                    /*手动发送消息确认信息*/
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: queueName,
                                     autoAck: false,//消息的默认ack关闭
                                     consumer: consumer);
                //一旦退出则无法接收消息
                while (true)
                {
                    //Console.ReadLine();
                }

            }
        }
        #endregion

        #region Topics
        public void Receive_Topics(string queue, Action<string> action, string routingKey, string exchange = "", bool durable = false, bool exclusive = false, bool autoDelete = false)
        {
            //消息结果
            string message;
            //创建连接
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //声明Exchange
                channel.ExchangeDeclare(exchange: "topic_logs",
                                        type: "topic");
                //获取queueName队列名称
                var queueName = channel.QueueDeclare().QueueName;
                //队列绑定Exchange
                channel.QueueBind(queue: queueName,
                                  exchange: "topic_logs",
                                  routingKey: routingKey);

                //声明队列
                channel.QueueBind(queue: queueName,
                                  exchange: "topic_logs",
                                  routingKey: routingKey);
                //将消息分发到空闲的worker
                //channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                //通过事件订阅消息
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    message = Encoding.UTF8.GetString(body);
                    action.Invoke(message);
                    /*手动发送消息确认信息*/
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: queueName,
                                     autoAck: false,//消息的默认ack关闭
                                     consumer: consumer);
                //一旦退出则无法接收消息
                while (true)
                {
                    //Console.ReadLine();
                }

            }
        }
        #endregion

        #region RPC
        public void Receive_RPC(string queue, Action<string> action, string routingKey, string exchange = "", bool durable = false, bool exclusive = false, bool autoDelete = false)
        {
            //消息结果
            string message;
            //创建连接
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //声明Exchange
                channel.ExchangeDeclare(exchange: "topic_logs",
                                        type: "topic");
                //获取queueName队列名称
                var queueName = channel.QueueDeclare().QueueName;
                //队列绑定Exchange
                channel.QueueBind(queue: queueName,
                                  exchange: "topic_logs",
                                  routingKey: routingKey);

                //声明队列
                channel.QueueBind(queue: queueName,
                                  exchange: "topic_logs",
                                  routingKey: routingKey);
                //将消息分发到空闲的worker
                //channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                //通过事件订阅消息
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    message = Encoding.UTF8.GetString(body);
                    action.Invoke(message);
                    /*手动发送消息确认信息*/
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: queueName,
                                     autoAck: false,//消息的默认ack关闭
                                     consumer: consumer);
                //一旦退出则无法接收消息
                while (true)
                {
                    //Console.ReadLine();
                }

            }
        }
        #endregion
    }



}
