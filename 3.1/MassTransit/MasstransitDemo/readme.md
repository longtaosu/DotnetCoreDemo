# 配置

Masstransit可以用于.Net平台的所有应用，但是应用类型的不同会影响到 `Bus` 的配置。

## Console

控制台程序有一个 `Main` 入口，下面用代码说明如何进行配置

> 需要添加引用 [MassTransit.RabbitMQ](https://nuget.org/packages/MassTransit.RabbitMQ/)

```c#
namespace EventPublisher
{
    using MassTransit;

    public interface ValueEntered
    {
        string Value { get; }
    }

    public class Program
    {
        // be sure to set the C# language version to 7.3 or later
        public static async Task Main()
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg => cfg.Host("localhost"));

            // Important! The bus must be started before using it!
            await busControl.StartAsync();
            try
            {
                do
                {
                    string value = await Task.Run(() =>
                    {
                        Console.WriteLine("Enter message (or quit to exit)");
                        Console.Write("> ");
                        return Console.ReadLine();
                    });

                    if("quit".Equals(value, StringComparison.OrdinalIgnoreCase))
                        break;

                    await busControl.Publish<ValueEntered>(new
                    {
                        Value = value
                    });
                }
                while (true);
            }
            finally
            {
                await busControl.StopAsync();
            }
        }
    }
}
```



## Asp.net Core

MassTransit支持以下功能：

- 跟随应用的生命周期启动、停止；
- 在需要的接口下以单例的方式进行注册；
- 为 `bus` 实例和接收 `endpoints` 添加健康检查；
- 配置 `bus` 使用 ILoggerFactory 实例。

> 需要添加引用：[MassTransit.AspNetCore](https://nuget.org/packages/MassTransit.AspNetCore/), [MassTransit.RabbitMQ](https://nuget.org/packages/MassTransit.RabbitMQ/)
>
> 默认使用 Fanout 类型的交换器

```c#
services.AddMassTransit(cfg =>
{
    //cfg.AddConsumer<>

    cfg.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        var host = cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("order", ep =>
        {
            ep.PrefetchCount = 16;
            ep.UseMessageRetry(r => r.Interval(10, 1000));

            //ep.ConfigureConsumer<>(provider);
        });
    }));
});
services.AddMassTransitHostedService();
```



## Windows服务

> 我们建议使用 TopShelf 创建Windows服务，简单易用且没有依赖。

```c#
public class EventConsumerService : ServiceControl
{
    IBusControl _bus;


    public bool Start(HostControl hostControl)
    {
        _bus = ConfigureBus();
        _bus.Start();

        return true;
    }

    public bool Stop(HostControl hostControl)
    {
        _bus?.Stop(TimeSpan.FromSeconds(5));

        return true;
    }

    IBusControl ConfigureBus()
    {
        return Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            cfg.Host("localhost");

            cfg.ReceiveEndpoint("event_queue", e =>
            {
                e.Handler<ValueEntered>(context => Console.Out.WriteLineAsync($"Value was entered: {context.Message.Value}"));
            });
        });
    }
}
```



# 基本概念

## 发布

当消息发布时，流程如下

1. 发布 `MySystem.Messages.SomeMessage` 消息
2. 发送消息到名为 `MySystem.Messages.SomeMessage` 的交换器下
3. 消息路由到名为 `my_endpoint` 的队列下

> 如果消息发布时消费者还未创建，但此时交换器已经创建，交换器在消费者创建前不受任何约束。因而此时发布的消息会丢失。

## 队列

- 每个应用应该有一个唯一的队列名称
- 如果将消费者的服务运行了多个副本，他们会监听同一个队列，会出现消费竞争的场景
- 如果消费者发生异常，消息会发送到 `my_endpoint_error`  队列下
- 如果消费者从队列中接收到一个不知该如何处理的消息，消息会发送到 `my_endpoint_skipped` 队列

## 设计优势

- 任何程序都可以监听任何的消息，这不影响其他的程序对这个消息的监听
- 任何的程序如果绑定到了同一个消息队列，会导致消费的竞争
- 不需要关心生产的消息类型和消费的消息类型

# 消息

有两种主要的消息类型，事件和命令。

## 命令



## 事件



## 消息头





# 生产者



# 消费者



# 异常



# 请求



# 容器





