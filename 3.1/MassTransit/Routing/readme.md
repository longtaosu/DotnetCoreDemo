# 简介

RabbitMQ的交换器有4中类型：`Fanout`、`Direct`、`Topic`和`Headers`。

- `Fanout` 将消息广播到所有绑定的队列
- `Direct` 根据 RoutingKey 将消息从交换器分发到不同的队列
- `Topic` 的 RoutingKey 是一组词（可以包含通配符*和#），实现比Direct更复杂的消息分发

**消息**

消息包含了2个字段，分别为时间和信息

```c#
public class TestInfo
{
    public DateTime time { get; set; }
    public string info { get; set; }
}
```



# Fanout

## 消费者

消费者同之前一样，接收消息并进行打印

```c#
public class TestInfoConsumer1 : IConsumer<TestInfo>
{
    private ILogService _logService;
    public TestInfoConsumer1(ILogService logService)
    {
        _logService = logService;
    }

    public Task Consume(ConsumeContext<TestInfo> context)
    {
        var info = context.Message;
        return Task.Run(() =>
        {
            _logService.PrintLog(string.Format("{0}：{1}_consumer1", info.time, info.info));
        });
    }
}
```

​	

## 生产者

生产者用于发布消息，此处不设置`RoutingKey`

```c#
public class InfoController : ControllerBase
{
    private IBusControl _busControl;
    public InfoController(IBusControl busControl)
    {
        _busControl = busControl;
    }

    public string Log(string info,string key)
    {
        _busControl.Publish<TestInfo>(new TestInfo()
        {
            info = info,
            time = DateTime.Now
        } );
        return string.Format("于时间:{0}，发布消息：{1}", DateTime.Now, info);
    }
}
```



## 配置

```c#
public static void ConfigureMQ(this IServiceCollection services)
{
    services.AddMassTransit(x =>
    {
        x.AddConsumer<TestInfoConsumer1>();
        x.AddConsumer<TestInfoConsumer2>();

        x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            var host = cfg.Host("localhost", "/", h =>
            {
                h.Username("guest");
                h.Password("guest");
            });                    

            cfg.ReceiveEndpoint("log11111", ep =>
            {
                ep.PrefetchCount = 1;
                ep.UseMessageRetry(r => r.Interval(10, 100));

                ep.Consumer<TestInfoConsumer1>(provider);
            });

            cfg.ReceiveEndpoint("log22222", ep =>
            {
                ep.PrefetchCount = 1;
                ep.UseMessageRetry(r => r.Interval(10, 100));

                ep.ConfigureConsumer<TestInfoConsumer2>(provider);
            });
        }));
    });

    services.AddMassTransitHostedService();
}
```

此处配置两个消费者，注意 `ReceiveEndpoint`和`Consumer`的不同，启动项目，可以看到创建的交换器（默认类型）是Fanout类型。

![Fanout.png](https://gitee.com/imstrive/ImageBed/raw/master/20200331/Fanout.png)

## 测试

测试可以看到，发出去的消息均被发送到两个消费者，两个消费者接收到同样的消息。

![Fanout.gif](https://gitee.com/imstrive/ImageBed/raw/master/20200331/Fanout.gif)



# Direct

本例目标是实现两个消费者，根据 RoutingKey 的不同，将消息进行分发。RoutingKey 是regular的分发到消费者1，RoutingKey 是normal的分发到消费者2.



## 消费者

此处我们需要定义2个消费者，以消费者1为例，定义如下：

```c#
public class TestInfoConsumer1 : IConsumer<TestInfo>
{
    private ILogService _logService;
    public TestInfoConsumer1(ILogService logService)
    {
        _logService = logService;
    }

    public Task Consume(ConsumeContext<TestInfo> context)
    {
        var info = context.Message;
        return Task.Run(() =>
        {
            _logService.PrintLog(string.Format("{0}：{1}_consumer1", info.time, info.info));
        });
    }
}
```

消费者中有依赖注入，可以调用接口 `ILogService`。

消费者的主要功能是将信息进行打印，两个消费者的区别在于消费者2在消息打印时，结尾是consumer2做区分。



## 生产者

```c#
public class InfoController : ControllerBase
{
    private IBusControl _busControl;
    public InfoController(IBusControl busControl)
    {
        _busControl = busControl;
    }

    public string Log(string info, string key)
    {
        _busControl.Publish<TestInfo>(new TestInfo()
        {
            info = info,
            time = DateTime.Now
        }, x => x.SetRoutingKey(key));
        return string.Format("于时间:{0}，发布消息：{1}", DateTime.Now, info);
    }
}
```

消息发送使用 `IBusControl`，此处与之前的不同在于需要使用方法 `SetRoutingKey` 设置路由键。



## 配置

```c#
public static void ConfigureMQ(this IServiceCollection services)
{
    services.AddMassTransit(x =>
    {
        x.AddConsumer<TestInfoConsumer1>();
        x.AddConsumer<TestInfoConsumer2>();

        x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            var host = cfg.Host("localhost", "/", h =>
            {
                h.Username("guest");
                h.Password("guest");
            });

            cfg.ReceiveEndpoint("tls1234567", ep =>
            {
                ep.Consumer<TestInfoConsumer1>(provider);

                ep.Bind("test", x =>
                {
                    x.RoutingKey = "regular";
                    x.ExchangeType = ExchangeType.Direct;
                });
            });
            
            cfg.ReceiveEndpoint("tls123456", ep =>
            {
                ep.ConfigureConsumer<TestInfoConsumer2>(provider);

                ep.Bind("test", x =>
                {
                    x.RoutingKey = "normal";
                    x.ExchangeType = ExchangeType.Direct;
                });
            });

            cfg.Publish<TestInfo>(x =>
            {
                x.AlternateExchange = "test";
                x.AutoDelete = true;
                x.Durable = false;
                x.ExchangeType = ExchangeType.Direct;
            });
        }));
    });

    services.AddMassTransitHostedService();
}
```

上例中需要针对消费者1和消费者2分别创建不同的 `ReceiveEndpoint`，将两个消费者都绑定到交换器 `test` 下。test交换器需要声明为 `Direct` 类型，为两个队列分别绑定RoutingKey，分别为**regular**和**normal**。

> 此处需要注意一定声明消息的发布配置，AlternateExchange设置为交换器的名称，ExchangeType设置为Direct。

## 测试

通过携带的不同参数key设置RoutingKey，进而实现了消息的分发，分别进入消费者1和消费者2.

![Direct.gif](https://gitee.com/imstrive/ImageBed/raw/master/20200331/Direct.gif)



# Topic

Topic和Direct一样可以根据RoutingKey进行消息的分发，但是Topic的RoutingKey更为复杂，是一组词，因而可以实现更为复杂的逻辑

## 消费者

消费者1和消费者2这里没什么区别，仅在于打印消息时，后缀consumer1和consumer2的不同。

```c#
public class TestInfoConsumer1 : IConsumer<TestInfo>
{
    private ILogService _logService;
    public TestInfoConsumer1(ILogService logService)
    {
        _logService = logService;
    }

    public Task Consume(ConsumeContext<TestInfo> context)
    {
        var info = context.Message;
        return Task.Run(() =>
        {
            _logService.PrintLog(string.Format("{0}：{1}_consumer1", info.time, info.info));
        });
    }
}
```



## 生产者

Topic中的生产者代码和Direct中的生产者一样，需要设置`RoutingKey`。

```c#
public class InfoController : ControllerBase
{
    private IBusControl _busControl;
    public InfoController(IBusControl busControl)
    {
        _busControl = busControl;
    }

    public string Log(string info, string key)
    {
        _busControl.Publish<TestInfo>(new TestInfo()
        {
            info = info,
            time = DateTime.Now
        }, x => x.SetRoutingKey(key));
        return string.Format("于时间:{0}，发布消息：{1}", DateTime.Now, info);
    }
}
```



## 配置

Topic的配置类似于Direct，区别在于ExchangeType的不同。

> 需要注意Topic中的RoutingKey是一组词，中间使用“.”分割，使用“*”代表一个词，“#”代表0个或者任意多个词。

```c#
public static void ConfigureMQ(this IServiceCollection services)
{
    services.AddMassTransit(x =>
    {
        x.AddConsumer<TestInfoConsumer1>();
        x.AddConsumer<TestInfoConsumer2>();

        x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            var host = cfg.Host("localhost", "/", h =>
            {
                h.Username("guest");
                h.Password("guest");
            });

            cfg.ReceiveEndpoint("lts111", ep =>
            {
                ep.Consumer<TestInfoConsumer1>(provider);

                ep.Bind("test", x =>
                {
                    x.RoutingKey = "*.black";
                    x.ExchangeType = ExchangeType.Topic;
                });
            });
                    
            cfg.ReceiveEndpoint("lts222", ep =>
            {
                ep.ConfigureConsumer<TestInfoConsumer2>(provider);

                ep.Bind("test", x =>
                {
                    x.RoutingKey = "animal.*";
                    x.ExchangeType = ExchangeType.Topic;
                });
            });

            cfg.Publish<TestInfo>(x =>
            {
                x.AlternateExchange = "test";
                x.AutoDelete = true;
                x.Durable = false;
                x.ExchangeType = ExchangeType.Topic;
            });
        }));
    });

    services.AddMassTransitHostedService();
}
```



## 测试

消费者1订阅了`*.black`类型的消息，消费者2订阅了`animal.*`类型的消息。

- 发送animal.black类型消息时，两个消费者均收到了消息；
- 发送animal.red类型消息时，消费者2接收到消息；
- 发送mouse.black类型消息时，消费者1接收到消息。

![Topics.gif](https://gitee.com/imstrive/ImageBed/raw/master/20200331/Topics.gif)



# 代码

<https://github.com/longtaosu/DotnetCoreDemo/tree/master/3.1/MassTransit/Routing>

