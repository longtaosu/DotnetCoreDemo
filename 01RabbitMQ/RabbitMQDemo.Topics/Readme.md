# Topics

在之前的内容中我们改进了我们的日志系统。fanout类型的交换器只能进行广播，所以我们替换为direct类型，这样消息可以进行选择性的接收。

尽管使用direct类型的交换器可以改进我们的系统，它仍然是有限制的，它无法完成基于多个准则的路由。

在我们的日志系统中，我们可能不仅仅是想基于severity接收消息，也可能会考虑提交日志的源头。可能你了解unix下的工具syslog，它对日志的路由不仅包含secerity（info/warn/crit...），还包括facility（auth/cron/kern...）。

这会让我们的系统更具弹性，可能我们想只监听来自“cron”或者“kern”的重要error信息。

想在我们的日志系统实现这一点，我们需要了解exchange中更为复杂的topic概念。



## Topic exchange

发送到topic交换器的消息的routing_key不能使任意的，必须是一组词的列表，用“.”分割。词可以是任意的，但是通常会有一些特征。比方说可以是“stock.usd.nyse”、"nyse.vmw"、"quick.orange.rabbit"，routing key中的单词数可以是任意的，但是不要超过255个字节。

binding key必须是相同的格式，topic类型的exchange和direct类型的exchange背后的算法是相似的——带有特定routing key的消息发送到所有的受routing key约束的队列。这里有两个关于binding keys很重要的情况：

- *（star），可以被任意的一个词替换
- #（hash），可以被任意多的词替换（0个或多个）

说明图如下所示：

![img](https://www.rabbitmq.com/img/tutorials/python-five.png)

在这个示例中，我们发送的消息被描述成动物，信息的routing key包含3个单词（2个“.”），key中的第一个单词描述速度，第二个是颜色，第三个是种类："<speed>.<color>.<species>"。

我们创建了3个绑定：（md文件此处打不出这个表达式，[原文链接](https://www.rabbitmq.com/tutorials/tutorial-five-dotnet.html)）

绑定的描述信息如下：

- Q1关注所有颜色是orange的动物
- Q2关注动物是rabbit的动物，和速度是lazy的动物

routing key是“quick.orange.rabbit”的消息会发送到两个队列，同样的道理"lazy.orange.elephant"也会发送到两个队列。但是"quick.orange.fox"只会分到第一个队列，"lazy.brown.fox"会分到第二个队列。需要注意的是"lazy.pink.rabbit"只会分到第二个队列一次，即使它满足了两条规则。消息"quick.brown.fox"不满足任何绑定条件所以会被忽略。

如果我们忽略发送一条带有1个或4个词的消息呢？比方说"orange"或者"quick.orange.male.rabbit"，这些消息会因为不匹配任何的绑定而会被忽略。

但是"lazy.orange.male.rabbit"虽然有4个单词，但是因为匹配了最后一条规则所以会发送到第二个队列。



### topic exchange

topic功能强大，可以实现其他exchange的类似功能。

- 当一个队列的约束是"#"时，它可以接收所有的消息，而不用关心routing key是什么，这个类似于fanout exchange。
- 当特殊字符"*"、"#"没有使用时，topic的交换器又类似于direct exchange。



## 组合

我们在日志系统中使用topic类型的exchange，假设所有的routing keys由两个单词组成："<facility>.<severity>"。

日志发送的代码：

```c#
using System;
using System.Linq;
using RabbitMQ.Client;
using System.Text;

class EmitLogTopic
{
    public static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using(var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare(exchange: "topic_logs",
                                    type: "topic");

            var routingKey = (args.Length > 0) ? args[0] : "anonymous.info";
            var message = (args.Length > 1)
                          ? string.Join(" ", args.Skip( 1 ).ToArray())
                          : "Hello World!";
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "topic_logs",
                                 routingKey: routingKey,
                                 basicProperties: null,
                                 body: body);
            Console.WriteLine(" [x] Sent '{0}':'{1}'", routingKey, message);
        }
    }
}
```

日志接收的代码

```c#
using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

class ReceiveLogsTopic
{
    public static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using(var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare(exchange: "topic_logs", type: "topic");
            var queueName = channel.QueueDeclare().QueueName;

            if(args.Length < 1)
            {
                Console.Error.WriteLine("Usage: {0} [binding_key...]",
                                        Environment.GetCommandLineArgs()[0]);
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
                Environment.ExitCode = 1;
                return;
            }

            foreach(var bindingKey in args)
            {
                channel.QueueBind(queue: queueName,
                                  exchange: "topic_logs",
                                  routingKey: bindingKey);
            }

            Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;
                Console.WriteLine(" [x] Received '{0}':'{1}'",
                                  routingKey,
                                  message);
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

