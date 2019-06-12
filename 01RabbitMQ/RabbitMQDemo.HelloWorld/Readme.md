# 1.Hello World

本例中需要写两个程序：用于发送消息的生产者和用于接受消息的消费者。

![(P) -> [|||] -> (C)](https://www.rabbitmq.com/img/tutorials/python-one.png)

如图所示，P是生产者，C是消费者。中间的Box表示队列，是RabbitMQ的缓冲区。



# 2.依赖

```c#
using RabbitMQ.Client;
```



# 3.生产者

![(P) -> [|||]](https://www.rabbitmq.com/img/tutorials/sending.png)

## 3.1建立连接

```c#
class Send
{
    public static void Main()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        {
            using (var channel = connection.CreateModel())
            {
                ...
            }
        }
    }
}
```

连接对Socket连接进行了抽象，并且为我们考虑了协议的版本隔离、授权等等。我们连接的是本地的Host，所以连接“localhost”。如果连接的是其他机器，则指定IP即可。



## 3.2创建Channel

如果想发送消息，则需要声明发送的队列，然后将消息发送到该队列。

```c#
using System;
using RabbitMQ.Client;
using System.Text;

class Send
{
    public static void Main()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using(var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "hello",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            string message = "Hello World!";
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "",
                                 routingKey: "hello",
                                 basicProperties: null,
                                 body: body);
            Console.WriteLine(" [x] Sent {0}", message);
        }

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}
```

创建一个队列是[幂等](https://www.cnblogs.com/leechenxiang/p/6626629.html)的——只有不存在时才会进行创建。消息的内容时byte数组，可以根据自己的喜好进行编码。

**当上面的代码执行完毕后，创建的Connection和Channel会消失。**



# 4.消费者

![(P) -> [|||]](https://www.rabbitmq.com/img/tutorials/sending.png)

## 4.1建立连接

```c#
class Receive
{
    public static void Main()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        {
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                ...
            }
        }
    }
}
```

此处也需要声明队列，因为消费者的创建可能会早于生产者，所以订阅消息前需要确认队列存在。

我们从队列中接收消息，服务器会异步的将消息推送给我们，我们需要提供回调函数。所以此处需要用到 **EventingBasicConsumer.Received**

## 4.2创建Channel

```c#
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

class Receive
{
    public static void Main()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using(var connection = factory.CreateConnection())
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

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
```























