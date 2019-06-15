# Publish/Subscribe

之前的demo中我们创建了工作队列。假设在工作队列背后，每一个任务可以准确的分配到worker。现在我们需要做些不一样的——将消息发送到多个消费者，这种模式称作：发布/订阅。

为了说明这种模式，我们创建一个简单的日志系统，该系统包含两部分：日志的提交，日志的接收打印。

在我们的日志系统中，每一个接收程序的副本都会获取消息。我们可以让一个接收程序把日志存储到磁盘而另一个接收程序把日志打印到屏幕。

最重要的是，消息的发布会通过广播的方式发送到每一个接收程序。

## Exchanges

在之前的部分，我们收发的消息都来自同一个队列。现在介绍Rabbit中完整的消息模型。

回顾之前我们介绍的知识点：

- Producer：发送消息的程序
- Queue：存储消息的缓冲区
- Consumer：接收消息的程序

RabbitMQ消息模型的核心思想是生产者永远不要直接把消息发送到队列。事实上，通常生产者也根本不知道消息发送的队列。

我们将生产者的消息发送到 *Exchange* 。*Exchange* 一方面接收生产者的消息，同事将消息发送到队列。*Exchange* 必须明确的知道如何处理接收到的消息，将消息追加到特定的队列？追加到多个队列？或者忽略这个消息？这个规则由 *Exchange* 的type决定。

![img](https://www.rabbitmq.com/img/tutorials/exchanges.png)

Echange的类型有以下几种：direct、topic、headers和fanout。

- direct：根据routing key绑定队列
- topic：routing key满足多规则模糊匹配
- header：
- fanout：Exchange将接收到的消息转发到所有队列

现在使用最后一个：fanout，创建该类型的 *Exchange* ，取名为logs。

```c#
channel.ExchangeDeclare("logs", "fanout");
```

如名所见，fanout类型的 *Exchange* 非常简单，仅仅是将接收到的消息广播到它所致的所有队列。



### **默认的Exchange**

之前的内容我们并未介绍过 Exchanges，但是仍旧可以将消息发送到队列。这是因为我们使用了默认的Exchange，使用【“”】定义。

回想之前我们定义的代码：

```c#
   var message = GetMessage(args);
   var body = Encoding.UTF8.GetBytes(message);
   channel.BasicPublish(exchange: "",
                        routingKey: "hello",
                        basicProperties: null,
                        body: body);
```

第一个参数是Exchange的名称，空的字符串表示使用默认的Exchange：消息路由到名字同routingKey相同的队列（在该队列存在的情况下）



### 将消息发送到特定名称的Exchange

```c#
var message = GetMessage(args);
var body = Encoding.UTF8.GetBytes(message);
channel.BasicPublish(exchange: "logs",
                     routingKey: "",
                     basicProperties: null,
                     body: body);
```



## 临时队列

之前我们使用的队列都有指定名称（“hello”和“task_queue”），为队列命名是很重要的事——我们需要知名相同队列下的workers。这个名称用于在生产者和消费者之间共享队列。

我们现在监听所有的日志信息，而不是他们的一个子集，我们关心的是当前的消息而不是旧的消息，为了解决这些问题需要做两件事。

1. 当我们连接到Rabbit时，我们需要一个新的、空的队列，我们可以使用是一个随机的名字创建队列；
2. 消费者断开连接时，队列应该自动删除。

我们使用方法QueueDeclare()创建一个非持久、专用、自动删除的队列。

```c#
var queueName = channel.QueueDeclare().QueueName;
```

关于队列的exclusive标志和其他属性可以在 [guide on queues](https://www.rabbitmq.com/queues.html) 了解详细信息。

此时的queueName包含一个随机队列名称，可能如下：amq.gen-JzTY20BRgKO-HjmUJj0wLg



## 绑定

![img](https://www.rabbitmq.com/img/tutorials/bindings.png)

现在我们创建了fanout类型的交换器和队列。现在我们需要让交换器将消息发送到队列，而交换器和队列之间的关系叫做绑定。

```c#
channel.QueueBind(queue: queueName,
                  exchange: "logs",
                  routingKey: "");
```

现在开始名为“logs”的交换器会将消息追加到队列上。

可以通过下面的指令打印绑定信息

```bash
rabbitmqctl list_bindings
```



## 组合代码

![img](https://www.rabbitmq.com/img/tutorials/python-three-overall.png)

生产者程序中，与之前的程序并无太大不同。最重要的不同点在于我们将消息发送到“logs”交换器而非名称缺省的交换器。在发送消息时需要提供routingKey，但是在fanout模式下这个值可以忽略。

下面是 EmitLog.cs代码：

```c#
using System;
using RabbitMQ.Client;
using System.Text;

class EmitLog
{
    public static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using(var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare(exchange: "logs", type: "fanout");

            var message = GetMessage(args);
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "logs",
                                 routingKey: "",
                                 basicProperties: null,
                                 body: body);
            Console.WriteLine(" [x] Sent {0}", message);
        }

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }

    private static string GetMessage(string[] args)
    {
        return ((args.Length > 0)
               ? string.Join(" ", args)
               : "info: Hello World!");
    }
}
```

如你所见，在建立连接后我们声明了交换器，这一步骤很重要，因为向一个不存在的交换器发送消息时禁止的。

如果没有队列与exchange绑定的话消息会丢失，但是对于本文的案例来说并无影响。

ReceiveLogs.cs代码：

```c#
using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

class ReceiveLogs
{
    public static void Main()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using(var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare(exchange: "logs", type: "fanout");

            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName,
                              exchange: "logs",
                              routingKey: "");

            Console.WriteLine(" [*] Waiting for logs.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] {0}", message);
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

