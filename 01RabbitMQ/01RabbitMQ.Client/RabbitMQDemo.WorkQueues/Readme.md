# Work Queues

![img](https://www.rabbitmq.com/img/tutorials/python-two.png)

本次创建一个队列，用于分布式的耗时任务处理。

工作队列的核心思想是避免立即处理耗费资源且需要等待完成的操作，相反我们会让任务在晚些时候进行处理。我们将任务当作一个消息并将其发送到队列。一个后台运行的工作线程会拿到这个任务并最终执行。当有很多个工作线程时，任务在工作线程中共享。

这个概念在web应用中是很有用的，尤其是一个http请求需要处理一个很复杂的请求任务时。



# 准备

我们用发送字符串代替复杂的任务，使用Thread.Sleep()方法假设这是个耗时任务。字符串中“.”的个数表示任务的复杂度，每个“.”表示耗时1s。比方说"Hello..."需要耗时3s。

此处稍微修改 Send 代码，允许在命令行输入任意的消息。程序会将任务分配到工作队列，



# 发布消息

```c#
var message = GetMessage(args);
var body = Encoding.UTF8.GetBytes(message);

var properties = channel.CreateBasicProperties();
properties.Persistent = true;

channel.BasicPublish(exchange: "",
                     routingKey: "task_queue",
                     basicProperties: properties,
                     body: body);
```

从命令行获取消息

```c#
private static string GetMessage(string[] args)
{
    return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
}
```



# 接收消息

接收消息的处理需要根据文本的内容做延时。

```c#
var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body;
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine(" [x] Received {0}", message);
    
    //模拟耗时处理
    int dots = message.Split('.').Length - 1;
    Thread.Sleep(dots * 1000);

    Console.WriteLine(" [x] Done");
};
channel.BasicConsume(queue: "task_queue", autoAck: true, consumer: consumer);
```



# 循环分发

使用任务队列的一个好处就是可以很容易的实现并行工作。如果需要处理大量堆积的工作，只需要设置更多的workers就可以了。

首先我们同时运行两个 ***worker*** 实例，他们都可以从队列获取消息。

打开3个控制台程序，两个运行 ***Worker***  程序，这两个程序作为我们的两个消费者——C1和C2。

```bash
# shell 1
cd Worker
dotnet run
# => [*] Waiting for messages. To exit press CTRL+C
```

```bash
# shell 2
cd Worker
dotnet run
# => [*] Waiting for messages. To exit press CTRL+C
```

第3个程序用来发布任务。当启动消费者的代码后就可以发布消息。

```bash
# shell 3
cd NewTask
dotnet run "First message."
dotnet run "Second message.."
dotnet run "Third message..."
dotnet run "Fourth message...."
dotnet run "Fifth message....."
```

看下 ***Workers*** 接收到了什么

```bash
# shell 1
# => [*] Waiting for messages. To exit press CTRL+C
# => [x] Received 'First message.'
# => [x] Received 'Third message...'
# => [x] Received 'Fifth message.....'
```

```bash
# shell 2
# => [*] Waiting for messages. To exit press CTRL+C
# => [x] Received 'Second message..'
# => [x] Received 'Fourth message....'
```

默认情况下，RabbitMQ会***顺序的***将每条消息发送到下一个消费者，而每个消费者会接收到相同数量的消息。这种消息分发的方式称作 **轮询调度** (round-robin)。

# 消息确认

任务的执行可能会很耗时间，而你会很好奇如果其中一个消费者执行一个耗时任务完成一部分（原文为die）便挂了的情况。我们当前的处理逻辑是当RabbitMQ将消息发送到消费者后，会立即在队列中删除消息，这种情况下如果删除一个正在工作的worker会导致信息的丢失。我们也会丢失分配到这个worker但是尚未处理的所有消息。

但是我们不希望丢失这些任务，当一个worker die之后，我们希望的是这个任务分配到其他的worker。

为了确保消息不丢失，RabbitMQ支持消息确认机制。Ack由消费者发送给RabbitMQ告知该消息已接收并处理，而RabbitMQ可以对消息进行删除操作。

如果一个消费者挂了（channel关闭，connection关闭或者TCP连接丢失）而未发送Ack消息，RabbitMQ会认为该消息完全没有处理从而重新放到队列。如果此时在线的有其他的消费者，该消息会立即被发送到其他消费者。这样可以确保消息不丢失，即使workers偶尔挂了。

消息不存在超时，如果有Consumer挂了RabbitMQ会立即将消息发送到其他的消费者，即使消息处理是一个很耗时的操作也不会导致超时。

默认是打开 ***手动消息确认*** 的，在之前的实例中我们通过参数设置关闭了 AutoAck（automatic acknowledgement mode）模式，现在删除这个标志位并手动的发送ack消息。

```c#
var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body;
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine(" [x] Received {0}", message);

    int dots = message.Split('.').Length - 1;
    Thread.Sleep(dots * 1000);

    Console.WriteLine(" [x] Done");

    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
};
channel.BasicConsume(queue: "task_queue", autoAck: false, consumer: consumer);
```

上面的代码保证了即使我们中断了消息的处理，也不会丢失数据。当工作线程挂掉之后所有未发送ack信息的消息会被重新发送。

ack必须发送到与消息接收相同的channel上，在不同channel尝试获取ack会导致channel级别的Exception。



# 消息持久化

我们已经学习了即使在消费者挂掉后如何保证task的不丢失，但当RabbitMQ停止工作时该消息仍然会丢失。

当RabbitMQ退出或者崩溃后，会丢失队列和消息。为了保证信息不丢失，我们需要做两件事：将队列和消息标记为持久化的。

`首先`，我们需要保证RabbitMQ不丢失我们的队列，因而我们需要将其设置为持久化的：

```c#
channel.QueueDeclare(queue: "hello",
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);
```

尽管这条命令语法上没有问题，但是当前并不会生效。因为我们已经定义了一个名为“hello”的队列，而这个队列不是持久化的。RabbitMQ不允许使用不同的参数重新定义已经存在的队列，而程序会接收到异常信息。

现在可以定义一个不同名字的队列，比方说task_queue。

```c#
channel.QueueDeclare(queue: "task_queue",
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);
```

（QueueDeclare）队列的声明需要在 Producer 和 Consumer 同时声明。

现在我们确定 *task_queue* 队列即使在RabbitMQ重启也不会丢失，现在将我们的信息标记为持久化的——设置

*IBasicProperties.SetPersistent* 为 *true*。

```c#
var properties = channel.CreateBasicProperties();
properties.Persistent = true;
```



## 注意

消息持久化并不能一定保证消息永不丢失。尽管持久化让RabbitMQ把消息存放在了硬盘上，仍然存在这么一个时刻RabbitMQ接收到消息但是尚未保存。RabbitMQ并不是对每个消息都做fsync（同步内存中所有已修改数据到存储）操作，可能只是将消息保存到缓存而并没有写入磁盘。持久性也不一定能保证，但是对于我们简单的任务队列是足够的。如果需要一个强制的持久化可以使用 [发布确认](https://www.rabbitmq.com/confirms.html) 。



# 均等分发

可能会注意到，现在的分发机制依旧不是我们期望的。比方说有2个Workers，所有偶数的消息都很复杂但是基数的消息都很简单，那么会出现一个worker一直很忙而另一个worker很闲。然而RabbitMQ并不知道这些却依然均等的派发消息。

这种情况的发生原因在于RabbitMQ在消息进入队列时就进行消息分发，此时并不确认consumer未完成的消息数量，只是将消息发送到对应的消费者。

![img](https://www.rabbitmq.com/img/tutorials/prefetch-count.png)

为了改变这种情况，我们使用 *BasicQos* 方法设置 prefetchCount = 1，这会使得RabbitMQ在每次派发给worker的消息不超过一条。换句话说，只有当worker处理完消息并确认后才会派发新的消息。如果其他的worker不忙的话会被派发该消息。

```c#
channel.BasicQos(0, 1, false);
```

## 注意队列的大小

如果所有的worker都很忙，需要注意队列会被填满的情况，此时可以添加workers或者换用其他策略。



# 整理

*NewTask.cs* 代码如下：

```c#
using System;
using RabbitMQ.Client;
using System.Text;

class NewTask
{
    public static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using(var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "task_queue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var message = GetMessage(args);
            var body = Encoding.UTF8.GetBytes(message);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: "",
                                 routingKey: "task_queue",
                                 basicProperties: properties,
                                 body: body);
            Console.WriteLine(" [x] Sent {0}", message);
        }

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }

    private static string GetMessage(string[] args)
    {
        return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
    }
}
```

Work.cs 代码如下：

```c#
using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;

class Worker
{
    public static void Main()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using(var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "task_queue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);

                int dots = message.Split('.').Length - 1;
                Thread.Sleep(dots * 1000);

                Console.WriteLine(" [x] Done");

                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };
            channel.BasicConsume(queue: "task_queue",
                                 autoAck: false,
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
```

使用消息确认和 *BasicQos* 可以设置工作队列，持久化的选项可以在RabbitMQ重启的情况下仍然使任务正常执行。

关于 IModel 和  *IBasicProperties* 的详细信息，可以查阅[RabbitMQ .NET client API reference online](http://www.rabbitmq.com/releases/rabbitmq-dotnet-client/v3.6.10/rabbitmq-dotnet-client-3.6.10-client-htmldoc/html/index.html)。



下一章我们会介绍如何将同样的信息发送给多个消费者。

