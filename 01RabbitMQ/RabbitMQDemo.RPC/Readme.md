# RPC(Remote prodecure call)

**注：该部分尚未完成**



在之前的示例中，我们已经介绍了如何将工作队列用于分布式的耗时任务处理。

但如果我们需要调用一个远程计算机上的方法，并且需要等待结果呢？这种方式称作远程过程调用——RPC。

这次我们使用RabbitMQ构建一个RPC系统：一个Client和一个可扩展的RPC服务器。因为我们没有什么耗时的任务需要做分布式，所以创建一个虚拟的RPC服务器，这个服务器只返回 Fibonacci数字。



## 客户端接口

为了说明如何使用RPC服务，我们创建一个简单的客户端类，这个类会暴露一个名为“Call”的方法，这个服务会发送RPC请求并会在接收到响应前保持阻塞。

```c#
var rpcClient = new RPCClient();

Console.WriteLine(" [x] Requesting fib(30)");
var response = rpcClient.Call("30");
Console.WriteLine(" [.] Got '{0}'", response);

rpcClient.Close();
```



### 注意：

尽管RPC在计算上是一种很常见的模式，但是也饱受批评。当一个开发人员没有意识到调用的方法事本低或者方法是一个耗时的RPC服务时，很容易出问题。这种不确定性会导致系统的不可预测并且会增加系统调试的复杂性。

请谨记下面的建议：

- 确定方法时本地还是远程的；
- 系统文档化。明确组件之间的依赖关系
- 处理错误。当RPC服务器挂了很长时间，客户端该如何响应



## 队列回调

一般来讲使用RabbitMQ实现RPC是很容易的，客户端发送请求后服务端做出响应。为了接收响应，我们会在请求上发送一个回调（callback）。

```c#
var props = channel.CreateBasicProperties();
props.ReplyTo = replyQueueName;

var messageBytes = Encoding.UTF8.GetBytes(message);
channel.BasicPublish(exchange: "",
                     routingKey: "rpc_queue",
                     basicProperties: props,
                     body: messageBytes);

// ... then code to read a response message from the callback_queue ...
```

### 消息属性

AMQP协议预定义了消息的14种属性，除了下面列出的几项，其他大部分属性都很少使用。

- Persistent：消息标记为持久化；
- DeliveryMode：该属性经常被当作Persistent使用，它们的功能一样；
- ContentType：描述编码的方式，比方说我们经常使用json格式，这是最好将属性设置为：application/json；
- ReplyTo：通常用于命名一个回调队列；
- CorrelationId：用于关联请求和RPC的响应信息



### Correlation Id

在之前提到的方法里，我们建议为每一个RPC请求创建一个回调的队列，这样效率会很低，但很庆幸又另外一种方法——为每一个客户端创建一个回调队列。

现在有新的问题，队列中接收到的响应信息很难判断是属于哪个请求，而CorrelationId属性要解决的问题。我们对每次请求会为其分配唯一的值，然后当我们接收到回调队列中的消息时我们会检查这个属性，根绝这个属性我们可以实现请求与响应的匹配。如果我们发现了一个未知的CorrelationId值，我们可以选择进行忽略，因为这个响应不属于我们的请求。

可能你会有疑问，为什么我们要忽略回调队列中的未知消息，而不是返回错误信息？这是因为在服务器端可能会产生静态，尽管可能性很小，但RPC服务器存在发送完消息后就挂掉的可能性。但此时可能还未发送请求的ack信息。如果这种情况发生了，重启后的RPC服务器会重新处理请求。这也是为什么我们要在客户端处理处理重复的响应，而RPC服务器应该是幂等的。



## 总结

![img](https://www.rabbitmq.com/img/tutorials/python-six.png)

我们的RPC服务器工作流程如下：

1. 当客户端启动后，创建一个匿名的专用回调队列
2. 对于RPC请求，客户端发送一条消息，这条消息带有两个属性：ReplyTo（用于设置回调队列）和CorrelationId（针对每个请求具有唯一值）
3. 请求发送到 rpc_queue队列
4. RPC Worker等待队列中的请求信息，当请求出现后，完成job并发送来自客户端的消息，消息使用来自ReplyTo属性指定的队列。
5. 客户端等待回调队列的数据，当收到消息后，会对CoreelationId进行检查。如果该属性匹配来自请求的的值便会返回应用的相应消息。



## 整理

### Fibonacci任务

```c#
private static int fib(int n)
{
    if (n == 0 || n == 1) return n;
    return fib(n - 1) + fib(n - 2);
}
```

我们声明了fibonacci函数，假定输入的参数是有效的正数。

### RPC服务端的代码

```c#
using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

class RPCServer
{
    public static void Main()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "rpc_queue", durable: false,
              exclusive: false, autoDelete: false, arguments: null);
            channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queue: "rpc_queue",
              autoAck: false, consumer: consumer);
            Console.WriteLine(" [x] Awaiting RPC requests");

            consumer.Received += (model, ea) =>
            {
                string response = null;

                var body = ea.Body;
                var props = ea.BasicProperties;
                var replyProps = channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                try
                {
                    var message = Encoding.UTF8.GetString(body);
                    int n = int.Parse(message);
                    Console.WriteLine(" [.] fib({0})", message);
                    response = fib(n).ToString();
                }
                catch (Exception e)
                {
                    Console.WriteLine(" [.] " + e.Message);
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

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }

    /// 

    /// Assumes only valid positive integer input.
    /// Don't expect this one to work for big numbers, and it's
    /// probably the slowest recursive implementation possible.
    /// 

    private static int fib(int n)
    {
        if (n == 0 || n == 1)
        {
            return n;
        }

        return fib(n - 1) + fib(n - 2);
    }
}
```

服务端的代码很简单：

1. 建立connection、channel并声明queue；
2. 我们可能会运行不止一个服务端进程，为了实现多个服务器间的负载均衡，我们需要设置prefetchCount，该这只在channel.BasicQos中；
3. 使用BasicConsume处理队列，然后注册一个处理的方法，该方法处理消息并回发响应消息。

### RPC客户端的代码

```c#
using System;
using System.Collections.Concurrent;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class RpcClient
{
    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly string replyQueueName;
    private readonly EventingBasicConsumer consumer;
    private readonly BlockingCollection<string> respQueue = new BlockingCollection<string>();
    private readonly IBasicProperties props;

public RpcClient()
{
        var factory = new ConnectionFactory() { HostName = "localhost" };

        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        replyQueueName = channel.QueueDeclare().QueueName;
        consumer = new EventingBasicConsumer(channel);

        props = channel.CreateBasicProperties();
        var correlationId = Guid.NewGuid().ToString();
        props.CorrelationId = correlationId;
        props.ReplyTo = replyQueueName;

        consumer.Received += (model, ea) =>
        {
            var body = ea.Body;
            var response = Encoding.UTF8.GetString(body);
            if (ea.BasicProperties.CorrelationId == correlationId)
            {
                respQueue.Add(response);
            }
        };
    }

    public string Call(string message)
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(
            exchange: "",
            routingKey: "rpc_queue",
            basicProperties: props,
            body: messageBytes);

        channel.BasicConsume(
            consumer: consumer,
            queue: replyQueueName,
            autoAck: true);

        return respQueue.Take(); ;
    }

    public void Close()
    {
        connection.Close();
    }
}

public class Rpc
{
    public static void Main()
    {
        var rpcClient = new RpcClient();

        Console.WriteLine(" [x] Requesting fib(30)");
        var response = rpcClient.Call("30");

        Console.WriteLine(" [.] Got '{0}'", response);
        rpcClient.Close();
    }
}

```

客户端处理流程：

1. 建立connection、channel并声明专用的“callback”队列，该队列用于消息回复；
2. 订阅“callback”队列，进而可以接收RPC响应。
3. Call方法模拟真实的RPC请求
4. 生成一个唯一的CorrelationId数值，该数值用于识别响应消息；
5. 发布请求消息，消息具有两个属性：ReplyTo和CorrelationId
6. 等待响应消息
7. 客户端检查响应消息，跟据CorrelationId做出判断。如果是期望的消息，保存响应信息；
8. 将响应消息返回给用户

### 客户端发送请求

```c#
var rpcClient = new RPCClient();

Console.WriteLine(" [x] Requesting fib(30)");
var response = rpcClient.Call("30");
Console.WriteLine(" [.] Got '{0}'", response);

rpcClient.Close();
```

