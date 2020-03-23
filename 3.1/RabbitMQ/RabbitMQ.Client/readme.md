# 简介

RabbitMQ是一个消息代理，可以接收并传递消息。

- 生产者：发送消息

  ![img](https://www.rabbitmq.com/img/tutorials/producer.png)

- 队列：消息在RabbitMQ中以队列的形式存在，受到主机内存及硬盘的限制，将消息的缓冲区设置的大一些是很有必要的。可能很多生产者将消息发送到同一个队列，很多的消费者从同一个队列中获取消息。

![img](https://www.rabbitmq.com/img/tutorials/queue.png)

- 消费者：用于接收消息

![img](https://www.rabbitmq.com/img/tutorials/consumer.png)

> 生产者、消费者不一定在同一个主机，事实上很多的应用确实没用在同一个主机。并且同一个应用也可以即是生产者又是消费者。

# HelloWorld

本测试我们编写两个程序：用于发送消息的生产者和用于接收消息并打印的消费者。

![(P) -> [|||] -> (C)](https://www.rabbitmq.com/img/tutorials/python-one.png)

上图中P是生产者，C是消费者，中间的表示RabbitMQ的消息缓冲区。

## 项目初始化

创建两个控制台程序，分别为Send和Receive，安装 `RabbitMQ.Client`

![01安装依赖.png](https://gitee.com/imstrive/ImageBed/raw/master/20200318/01安装依赖.png)

## Send

![(P) -> [|||]](https://www.rabbitmq.com/img/tutorials/sending.png)

编写消息发布程序，程序会连接到RabbitMQ，发送一个简单的消息，然后退出。

```c#
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using(var channel = connection.CreateModel())
                {
                    //业务
                }
            }

            Console.ReadLine();
        }
    }
```

此处我们连接到本地的消息队列，如果需要连接到其他机器的队列，只需要替换为IP地址。

下面我们创建一个Channel，声明一个消息发送的队列。

```c#
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        {
            using(var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
                string message = "Hello World";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                    routingKey: "hello",
                                    basicProperties: null,
                                    body: body);
                Console.WriteLine(" [x] Sent {0}", message);
            }
        }

        Console.ReadLine();
    }
```

>  队列的声明是幂等的，只有当队列不存在时才会创建。消息的内容是byte数组，所以可以使用任何的编码方式。

![02HelloWorld_Publish.png](https://gitee.com/imstrive/ImageBed/raw/master/20200318/02HelloWorld_Publish.png)

## Receiving

消费者负责监听RabbitMQ上的消息，所以消费者需要持续运行，监听消息并将消息打印

```c#
static void Main(string[] args)
{
    var factory = new ConnectionFactory() { HostName = "localhost" };
    using (var connection = factory.CreateConnection())
    {
        using(var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "hello",
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);
        }
    }

    Console.ReadLine();
}
```

此处我们也需要声明队列，因为我们的消费者可能启动早于生产者，在我们从队列接收消息时需要确认队列的存在。

我们指定接收服务器从队列中发送的消息，因为消息的推送是异步的，我们需要提供回调，即 `EventingBasicConsumer.Received`。

```c#
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        {
            using(var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);
                };
                channel.BasicConsume(queue: "hello",
                                    autoAck: true,
                                    consumer: consumer);
            }
        }

        Console.ReadLine();
    }
```

![03HelloWorld_Receive.png](https://gitee.com/imstrive/ImageBed/raw/master/20200318/03HelloWorld_Receive.png)

通过RabbitMQ后台，可以看到消息已经被消费掉。



# WorkQueue

上例中我们实现了在一个名为 `Hello` 的队列中发送和接收消息，本例中我们创建一个Work Queue，用于在多个消费者中分发耗时的任务。

![img](https://www.rabbitmq.com/img/tutorials/python-two.png)

WorkQueue用于处理需要立即执行、消耗资源且需要等待完成的任务。我们可以将消息延时处理，将任务封装成一个消息发送到队列，后台运行的Worker线程会最终执行该任务。当存在多个Worker时，后台任务会进行分发。

此处我们使用 `Thread.Sleep` 模拟真实环境下的耗时任务，我们使用"."代表任务的复杂性，比方说 `Hello...` 表示任务需要3秒执行。

创建两个控制台程序，分别命名为：NewTask和Worker，添加依赖 `RabbitMQ.Client` 。

## NewTask

```c#
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        {
            using (var channel = connection.CreateModel())
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
            }
        }

        Console.ReadLine();
    }

    private static string GetMessage(string[] args)
    {
        return ((args.Length > 0) ? String.Join(" ", args) : "Hello World!");
    }
```

## Worker

```c#
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "task_queue",
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);
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

                // here channel could also be accessed as ((EventingBasicConsumer)sender).Model
                //channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };
            channel.BasicConsume(queue: "task_queue", autoAck: true, consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
```

## 轮询分发

使用任务队列的好处之一就是可以轻松的并行工作，如果我们想创建工作的后台日志，我们通过添加更多的`Worker` 就可以简单的扩展。

通过 `dotnet run` 命令同时运行两个Worker实例，两个worker都可以从队列中获取消息。同时运行一个发布者，结果如下图。

![04消息分发.png](https://gitee.com/imstrive/ImageBed/raw/master/20200318/04消息分发.png)

> 默认情况下RabbitMQ按照顺序依次将消息发送到下一个消费者，整体上每个消费者会得到同样数量的消息，这种消息分发方式称作 round-robin。

## 消息确认

任务的处理有时会占用一定的时间。你可能会很好奇，如果一个消费者处理一个很长时间的任务，在部分完成时便死掉了，这时会发生什么。

我们当前的代码，一但RabbitMQ将消息发送到消费者，消息就会被立即删除。在这种情况下，如果一个正在处理消息的worker被杀掉，则该信息会丢失，并且所有发送到这个worker的信息也都会丢失。但是我们不希望任何的任务发生丢失，当一个worker死掉后，我们希望这个任务被发送到另一个worker。

为了保证消息不丢失，RabbitMQ支持消息确认。消费者需要发送 `ack` 告知RabbitMQ特定的消息已经接收到，已经完成处理且RabbitMQ可以删除消息。

如果消费者没有发送 `ack` 便死掉（channel关闭，连接关闭或者TCP连接丢失），RabbitMQ会认为消息完全没有处理并将消息重新排队。如果此时刚好有其他的消费者存在，消息会立即发送到其他消费者，这样便可以认为消息不丢失。

> RabbitMQ消息处理没有超时的概念，当消费者死掉后，消息会立即被重发，即使消息处理占用了很长时间也不存在超时的问题。

默认 [手动消息确认](https://www.rabbitmq.com/confirms.html) 是打开的，之前的例子当中我们通过将 autoAck 参数设置为true进行了关闭。现在我们删除这个参数，在任务完成后手动发送 `ack` 。

将下面的代码注释去掉，启用该代码

```c#
 channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
```

将autoack设置为false

```c#
channel.BasicConsume(queue: "task_queue", autoAck: false, consumer: consumer);
```

![05消息手动ack.png](https://gitee.com/imstrive/ImageBed/raw/master/20200318/05消息手动ack.png)

测试中将下面消费者（接收消息1）接收消息时，关闭掉，可以发现消息会被重新发送到上面的消费者（原本接收消息2）。

> 忘记确认
>
> 如果忘记了 `BasicAck` ，会产生严重的影响。当客户端退出后，消息会被重新发送。因为RabbitMQ无法释放未 Ack 的消息，所以占用的内存会越来越多。
>
> 如果想要调试这种问题，可以使用 rabbitmqctl 打印 messages_unacknowledged ：
>
> ```shell
> sudo rabbitmqctl list_queues name messages_ready messages_unacknowledged
> ```
>
> Windows下
>
> ```shell
> rabbitmqctl.bat list_queues name messages_ready messages_unacknowledged
> ```

## 消息持久化

之前我们解决了当消费者死掉后如何保证任务不丢失，但是RabbitMQ服务器停掉后，任务还是会丢失。

当RabbitMQ退出或者崩溃后，队列和消息会丢失（除非单独设置）。为了保证消息不丢失，需要做两件事：将队列、消息设置为可持久化的。

首先，我们需要确认RabbitMQ不会丢失队列，将队列声明为 `durable`。

```c#
channel.QueueDeclare(queue: "task_queue",
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);
```

>  尽管这条命令没有任何问题，但是在我们当前的设置下并不会工作。这是因为之前我们已经定义了名为 `hello`的队列，而这个队列不是持久化的。RabbitMQ不允许使用不同的参数重新定义一个已经存在的队列。
>
>  此处为了方便，我们定义一个名为 task_queue 的队列，队列的声明需要同时修改生产者和消费者的代码。

上面的代码，我们可以保证即使RabbitMQ重启，task_queue队列不会丢失。现在我们将消息也标记为持久化的，通过设置 IBasicProperties.SetPersistent 为 true。

```c#
var properties = channel.CreateBasicProperties();
properties.Persistent = true;
```

> 将消息设置为持久化并不能保证消息不丢失，尽管设置RabbitMQ将消息存储到硬盘了，仍然存在一定的几率消息没有被保存。如果需要强保证消息的存储，可以使用 [publisher confirms](https://www.rabbitmq.com/confirms.html)。

## Fair Dispatch

可以注意到，消息的分发仍不是我们所期望的。比方说我们有两个worker，奇数的消息很重量级而偶数的消息很轻量级，那其中一个worker就会一直很忙而另一个可能什么都不做。而此时RabbitMQ并不知道所发生的，仍然在均匀的分发。

会发生这种情况是因为当消息进入队列后，RabbitMQ只做消息分发，而并不关心消费者unack消息的数量，只是根据消息的顺序进行了简单的分发。

![img](https://www.rabbitmq.com/img/tutorials/prefetch-count.png)

为了改变这种方式，我们使用 `BasicQos` 方法将 参数 prefetchCount 设置为1。这会告诉RabbitMQ对worker发送的消息数量，一次不要超过1个。换句话说，只有在之前的消息处理完成后，才可以分发新的消息。此时会将消息发送到另一个非忙碌的worker。

```c#
channel.BasicQos(0, 1, false);
```

> 注意队列的大小
>
> 如果所有的worker都是忙碌中，队列可能会被占满。这种情况需要留意，可以添加新的worker或者使用其他的策略。



# Publish/Subscribe

在之前的示例中，我们创建了队列，并假设每个任务都可以发送到一个worker。此处我们可以将一个消息发送给多个消费者，这种模式称作“Pub/Sub”。

为了说明这种模式，我们创建一个简单的日志系统，系统包含两个部分：一个提交日志信息，另一个接收并打印日志。

在我们的日志系统中，每一个接收程序都会获得消息。其中一个接收程序接收日志并存储到磁盘，与此同时另一个程序将日志打印到屏幕。所有的日志信息会被广播到所有的消息接收者。

## 交换器

在之前的实例中我们从队列中发送和接收消息，现在介绍Rabbit中完整的消息模型。

之前我们提到的概念：

- Producer：生产者，系统中消息的发送方
- Queue：存储消息的缓冲区
- Consumer：消费者，系统中消息的接收方

实际上，生产者将消息发送到 Exchange，exchange一方面接收生产者的消息，另一方面将消息发送到队列。exchange需要明确的知道如何处理接收到的消息。追加到特定的队列？追加到多个队列？或者忽略这个消息。

![img](https://www.rabbitmq.com/img/tutorials/exchanges.png)

交换器的类型有以下几种：`direct`、`topic`、`headers` 和 `fanout`。我们先考虑最后一种——Fanout。

创建名为 logs 的交换器，类型为fanout。

```c#
channel.ExchangeDeclare("logs", ExchangeType.Fanout);
```

fanout类型的交换器很简单，将接收到的所有消息广播到所有的队列。

> **交换器**
>
> 想要查看服务器上的交换器，可以使用以下命令
>
> ```shell
> sudo rabbitmqctl list_exchanges
> ```
>
> 这个列表中会有一些 `amq.*` 交换器和默认的交换器（未命名的），这些是默认创建的交换器，但是当前还不会去使用。
>
> **默认交换器**
>
> 之前的例子中我们不了解交换器，但是我们依旧可以将消息发送到队列，这是因为women是用了默认的交换器，我们使用空字符串“”标识。
>
> 回忆之前我们发布消息
>
> ```c#
>     var message = GetMessage(args);
>     var body = Encoding.UTF8.GetBytes(message);
>     channel.BasicPublish(exchange: "",
>                          routingKey: "hello",
>                          basicProperties: null,
>                          body: body);
> ```
>
> 第一个参数是交换器的名字，空字符串表示默认的交换器，消息发送到名为`routingkey`指定的队列

现在我们发布到指定的交换器

```c#
var message = GetMessage(args);
var body = Encoding.UTF8.GetBytes(message);
channel.BasicPublish(exchange: "logs",
                     routingKey: "",
                     basicProperties: null,
                     body: body);
```

## 临时队列

之前我们使用的是指定名字的队列（`hello`和`task_queue`）。命名一个队列很重要，我们需要将workers指向同一个队列，而生产者和消费者分享队列就需要指定队列的名称。

但这个不适用我们的情况，我们想要监听所有的日志消息，不单单只是一个子集。我们只对当前的消息感兴趣，而不关心旧的消息，这里需要解决两个问题。

首先，无论何时我们连接Rabbit时，我们希望得到一个干净、空队列。我们可以使用随机的名字创建一个队列，或者，让服务器为我们选择一个随机的队列。

其次，当消费者断开时，队列应该自动删除。

我们使用`QueueDeclare()`生成的名字创建非持久、关键的、自动删除的队列。

```c#
var queueName = channel.QueueDeclare().QueueName;
```

队列的名字会包含一个随机的名字，例如 `amq.gen-JzTY20BRgKO-HjmUJj0wLg`。

## 绑定

![img](https://www.rabbitmq.com/img/tutorials/bindings.png)

我们创建了fanout类型的交换器和一个队列，现在我们需要交换器将消息发送到队列，在交换器和队列之间建立关系的操作称作绑定。

```c#
channel.QueueBind(queue: queueName,
                  exchange: "logs",
                  routingKey: "");
```

现在开始，名为 `logs` 的交换器会将消息追加到我们的队列。

> 显示绑定
>
> 使用下面的命令显示bangding
>
> ```shell
> rabbitmqctl list_bindings
> ```

## 整理

![img](https://www.rabbitmq.com/img/tutorials/python-three-overall.png)

生产者程序，提交日志消息，和之前的很相像。区别在于我们现在将消息发送到名为 `logs`  的交换器而不是之前的默认交换器。发送时我们需要提供一个 `routingKey`，但是在`fanout`交换器下我们可以缺省。

发布者

```c#
public static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using(var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

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
```

接收者

```c#
    public static void Main()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using(var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

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
```

说明：`logs` 交换器出来的数据根据服务器指定的名字发送到两个队列。

![PubSub.gif](https://gitee.com/imstrive/ImageBed/raw/master/20200318/PubSub.gif)



# Routing

之前的系统中我们创建了一个简单的日志系统，我们可以将消息广播到很多的接收者。

本示例我们增加一个功能——日志系统只订阅消息的一个子集。比方说，我们只将严重错误的信息存储，但是所有的错误日志均打印。

## 绑定

之前的例子中我们已经使用过绑定，代码如下：

```c#
channel.QueueBind(queue: queueName,
                  exchange: "logs",
                  routingKey: "");
```

绑定是交换器和队列之间的关系，简单的理解为：队列所关心的是来自交换器的消息。

绑定需要 `routingKey` 参数，为了避免跟 BasicPublish 参数混淆，我们将这个参数称作 `binding key`。binding key的创建方法如下：

```c#
channel.QueueBind(queue: queueName,
                  exchange: "direct_logs",
                  routingKey: "black");
```

binding key的含义和exchange的类型有关，对于`fanout`类型的交换器，我们可以忽略这个值。

## Direct exchange

我们之前的日志系统向所有的消费者广播消息，我们现在系统根绝日志的级别进行过滤。比方说，我们只将严重错误的日志存盘，对于warning和info级别的日志则不存储。

我们使用`fanout`类型的交换器，这个不是很灵活——只能无脑的进行消息广播。

现在我们使用`direct`类型的交换器，消息会根据binding key找到对应的消息队列。如下图所示

![img](https://www.rabbitmq.com/img/tutorials/direct-exchange.png)

上图中，我们可以看到`direct`类型的交换器`X`上绑定了两个队列，第一个队列的binding key是`orange`，第二个队列有两个绑定，一个key是`black`，另一个是`green`。

在这种设置下，routing key是`orange`的消息会发送到队列Q1，routing key是`black`或者`green`的消息会发送到Q2，其他所有的消息会被忽略。

## Multiple bindings

![img](https://www.rabbitmq.com/img/tutorials/direct-exchange-multiple.png)

多个队列也可以有相同的binding key，我们可以在`X`和`Q1`之间使用`black`添加绑定关系，这种情况下`direct`类型的交换器行为就会和`fanout`一样（消息向多个消费者广播）。routing key是`black`的消息会被发送到Q1和Q2两个队列。

## 提交日志

现在我们使用`direct`类型的交换器发送消息，将日志的等级作为`routing key`。日志接收程序可以根据日志的等级选择接收。

首先，我们创建一个交换器

```c#
channel.ExchangeDeclare(exchange: "direct_logs", type: "direct");
```

然后发送消息

```c#
var body = Encoding.UTF8.GetBytes(message);
channel.BasicPublish(exchange: "direct_logs",
                     routingKey: severity,
                     basicProperties: null,
                     body: body);
```

为了简化，我们假设日志的等级有“info”、“warning”和“error”。

## 订阅

消息的接收和之前一样，不同在于需要为每个我们关心的日志等级创建绑定。

```c#
var queueName = channel.QueueDeclare().QueueName;

foreach(var severity in args)
{
    channel.QueueBind(queue: queueName,
                      exchange: "direct_logs",
                      routingKey: severity);
}
```

## 整理

![img](https://www.rabbitmq.com/img/tutorials/python-four.png)

日志提交代码

```c#
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
```

日志接收代码

```c#
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
```

发布日志（Publisher目录下）

```shell
dotnet run warning error
```

接收日志（Subscriber目录下）

```shell
dotnet run info warning error
```

![Routing.gif](https://gitee.com/imstrive/ImageBed/raw/master/20200319/Routing.gif)



# Topics

在之前的示例中，我们使用`fanout`类型的交换器实现了广播，是用`direct`类型的交换器实现选择性的接收日志。

尽管使用`direct`类型的交换器可以改进系统，但是仍然有局限性——不能基于多个标准进行数据路由。

我们可能不仅仅想基于日志的等级进行订阅，可能也想考虑日志的来源。这样会有很大的灵活性，我们可能不仅仅想监听来自“cron”的等级为严重的错误，也想监听来自“kern”的所有日志。

为了实现这一点，我们需要使用更复杂的 `topic` 交换器。

## Topics Exchange

进入`topic`交换器的消息不能使任意的 `routing_key`——可以是一组词。词可以是任何的，通常是跟消息的一些特征相关的，比方说：“stock.usd.nyse”、"nyse.vmw"、"quick.orange.rabbit"。路由中的词数随意，最多255个字节。

binding key需要是相同的格式，topic交换器背后的逻辑跟direct类似——消息发送到与routing key相匹配的消息队列。

binding keys有两个重要的符号：

- *：表示一个词
- #：表示0或者多个词

![img](https://www.rabbitmq.com/img/tutorials/python-five.png)

下面的例子中，我们会发送用于描述动物的消息。routing key包含3个词，分别表示速度、颜色和种类

```shell
<speed>.<colour>.<species>
```

我们创建了3个绑定关系，Q1约束于`*.orange.*`，Q2约束于`*.*.rabbit` 和 `lazy.#`

绑定关系含义如下：

- Q1关心所有颜色是orange的动物。
- Q2关心所有动物是rabbit和速度是lazy的动物

routing key是`quick.orange.rabbit`、`lazy.orange.elephant`的消息会发送到两个队列。`quick.orange.fox`只会进Q1，`lazy.brown.fox`只会进Q2。而`lazy.pink.rabbit`尽管匹配Q2的两个绑定，也只会进一次。`quick.brown.fox`不匹配任何绑定，所以会被忽略。

如果我们不遵循协议，发送消息的routing key有1个或者4个词呢？比方说 `orange` 或者 `quick.orange.male.rabbit`，这些消息因为无法匹配会丢失。而`lazy.orange.male.rabbit`尽管有4个词，但是匹配最后的绑定关系因此会发送到到Q2。

> Topic交换器
>
> Topic交换器功能强大，可以当做任何其他的交换器使用
>
> 当队列的binding key是`#`时，可以接收所有的消息，这类似于`fanout`交换器。
>
> 当没有使用特殊字符 `*` 或者 `#`，时，topic交换器就像direct交换器。

## 整理

我们在日志系统中使用topic交换器，假设routing key由2个字符组成。

发布代码

```c#
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
```

接收代码

```c#
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
```

![](F:\03Github\06DotnetCoreDemo\3.1\RabbitMQ\Images\Topics.gif)





# RPC





# 参考

官方：

<https://www.rabbitmq.com/getstarted.html>

<https://github.com/rabbitmq/rabbitmq-tutorials/tree/master/dotnet>

博文：

[EdisonZhou](https://www.cnblogs.com/edisonchou/category/1288572.html)

[张阳](https://www.cnblogs.com/richieyang/p/5492432.html)