# Routing

之前我们创建了一个日志系统，将日志信息广播到很多的接收端中。

现在我们追加一个功能，使得消费者程序可以只订阅一部分消息。比方说，我们只将严重错误的消息记录日志（保存到磁盘），但是控制台仍希望打印所有的消息。

## Bindings

之前的示例中我们已经创建了绑定，相关代码如下：

```c#
channel.QueueBind(queue: queueName,
                  exchange: "logs",
                  routingKey: "");
```

绑定是指 *Exchange* 和 Queue之间的关系，可以简单的理解为：队列只关心来自 Exchange的消息。

绑定需要一个额外的 *routingKey* 参数，不要和 BasicPublish 参数中的 binding key混淆。我们演示下如何使用key创建一个绑定：

```c#
channel.QueueBind(queue: queueName,
                  exchange: "direct_logs",
                  routingKey: "black");
```

binding 的key与exchange的type有关。fanout类型的 exchanges会忽略这个值。



## Direct exchange

我们的日志系统向所有的消费者广播消息，我们希望对系统进行扩展后能实现对消息的过滤。比方说，我们希望打印日志的脚本程序只接受严重错误的信息，对于info信息不进行保存。

我们使用fanout交换器，这种类型不易扩展，只能进行不加过滤的广播。

现在使用direct类型的Exchange，direct交换器背后的路由算法很简单——如果消息的routing key与队列的binding key相匹配，则消息发送到该队列。

图示如下：

![img](https://www.rabbitmq.com/img/tutorials/direct-exchange.png)

上图中的交换器类型为direct，与之绑定的队列有两个。第一个队列的binding keu是orange，第二个有两个绑定key：black和green。

此时一条routing key是orange的消息会发送到Q1，routing key是black或者green的消息会发送到Q2。其他的消息则会被忽略。



## Multiple bindings

![img](https://www.rabbitmq.com/img/tutorials/direct-exchange-multiple.png)

多个队列使用相同的binding key也是可以的，如图所示，在X和Q1之间建立的binding key为black，这种绑定条件下信息会发送到所有匹配的队列，实现的效果和fanout类型一样。routing key是black的消息会发送到Q1和Q2。



## Emitting logs

现在我们使用direct类型的exchange发送消息，发送的日志会带有routing key。接收数据的脚本可以选择性的接收消息，我们先看下如何提交日志。

首先，我们需要创建日志：

```c#c
channel.ExchangeDeclare(exchange: "direct_logs", type: "direct");
```

然后发送消息：

```c#
var body = Encoding.UTF8.GetBytes(message);
channel.BasicPublish(exchange: "direct_logs",
                     routingKey: severity,
                     basicProperties: null,
                     body: body);
```

为了简化代码，我们假设severity的取值可以是：“info”、“warning”和“error”。



## Subscribing

消息的接收程序和之前类似，区别在于我们为每个severity值创建一个绑定关系。

```c#c
var queueName = channel.QueueDeclare().QueueName;

foreach(var severity in args)
{
    channel.QueueBind(queue: queueName,
                      exchange: "direct_logs",
                      routingKey: severity);
}
```



# 整理

![img](https://www.rabbitmq.com/img/tutorials/python-four.png)

日志的发送代码如下：

```c#
using System;
using System.Linq;
using RabbitMQ.Client;
using System.Text;

class EmitLogDirect
{
    public static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using(var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare(exchange: "direct_logs",
                                    type: "direct");

            var severity = (args.Length > 0) ? args[0] : "info";
            var message = (args.Length > 1)
                          ? string.Join(" ", args.Skip( 1 ).ToArray())
                          : "Hello World!";
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "direct_logs",
                                 routingKey: severity,
                                 basicProperties: null,
                                 body: body);
            Console.WriteLine(" [x] Sent '{0}':'{1}'", severity, message);
        }

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}
```

日志的接收代码：

```c#
using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

class ReceiveLogsDirect
{
    public static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using(var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare(exchange: "direct_logs",
                                    type: "direct");
            var queueName = channel.QueueDeclare().QueueName;

            if(args.Length < 1)
            {
                Console.Error.WriteLine("Usage: {0} [info] [warning] [error]",
                                        Environment.GetCommandLineArgs()[0]);
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
                Environment.ExitCode = 1;
                return;
            }

            foreach(var severity in args)
            {
                channel.QueueBind(queue: queueName,
                                  exchange: "direct_logs",
                                  routingKey: severity);
            }

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;
                Console.WriteLine(" [x] Received '{0}':'{1}'",
                                  routingKey, message);
            };
            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
```

