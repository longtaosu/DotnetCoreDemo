# 说明

## 1.创建连接

```c#
bus = RabbitHutch.CreateBus("host=localhost;publisherConfirms=true;timeout=10");
```

参数说明:

1. host："localhost"或"192.168.2.56"或者"myhost.mydomain.com"，可以指明端口；
2. virtualHost：“myVirtualHost”，默认“/”；
3. username：默认“guest”
4. password：默认“guest”
5. requestedHeartbeat：（requestHeartbeat=1）默认值是10s，如果不需要心跳则设置为0；
6. prefetchcount：（prefetchcount=1）默认值是50，指在EasyNetQ发送“ack”信号前可以向RabbitMQ发送信息的最大数量，设置为0时不进行预读取（不建议），设置为1则会均等的发送到每一个消费者；
7. publisherConfirms：（publisherConfirms=true）默认值是false，具体参考：https://github.com/EasyNetQ/EasyNetQ/wiki/Publisher-Confirms，需要注意的是仅作为发送完成的标志；
8. persistentMessages：（persistentMessages=false）默认值是true，当值为true时数据会持久化到硬盘，但是性能会受到影响；
9. product：
10. platform：
11. timeout：（timeout=60）默认值10s，



## 2.发布/订阅

### 2.1发布

发布/订阅中双方是不考虑对方的，发布者仅仅是说明“this has happened”，订阅者只是说明“i care about this”。消息的订阅者可能是1个，200个或者一个都没有，但发布者不关心。EasyNetQ也实现了这种方式，即在发布消息的时候，如果此时没有订阅者，消息便会直接消失，这是默认的设置。

### 2.2订阅

EasyNetQ的订阅者订阅的是一个消息类型（是.net概念上的类），一但通过Subscribe方法建立起对某个类型的订阅，RabbitMQ上会建立起一个持久化的队列，符合该类型的消息都会被放到这个队列上。

```c#
bus.Subscribe<MyMessage>("my_subscription_id", msg => Console.WriteLine(msg.Text));
```

"my_subscription_id"参数非常重要，EasyNetQ会在RabbitMQ上根绝消息的类型和“my_subscription_id”创建一个唯一的队列。

如果使用相同的数据类型和“my_subscription_id”创建两次订阅，RabbitMQ会对同一个队列创建两个消费者，并对消费者进行轮询，这种情况在分布式上很常见。如果创建了一个处理特定消息的服务，这个服务已经负载很重了，就可以创建一个新的服务实例，并行的处理消息。

