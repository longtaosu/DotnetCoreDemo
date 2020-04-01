# 重试

当消息处理异常时，将消息进行重试，尝试再次发送到消费者进行处理。

该情况多见于消费者round-roubin模式下，当一个消费者处理失败后，可以发送到下一个消费者进行处理

```c#
cfg.ReceiveEndpoint("lts111", ep =>
{
    ep.UseMessageRetry(r => r.Interval(3, 1500));

    ep.Consumer<TestInfoConsumer>(provider);
});
```

![retry.gif](https://gitee.com/imstrive/ImageBed/raw/master/20200401/retry.gif)


> 异常发生时，会生成一个异常队列，发生异常的信息会保存至该异常队列。如上例中创建队列“lts111”，异常发生时会创建异常队列“lts111_error”

![error_queue.png](https://gitee.com/imstrive/ImageBed/raw/master/20200401/error_queue.png)


# 限流

当设定时间内，队列访问次数超过限制时会提示异常。

使用 `UseRateLimit` 方法实现限流

```c#
cfg.ReceiveEndpoint("lts111", ep =>
{
    ep.UseRateLimit(5, TimeSpan.FromSeconds(5));
    ep.Consumer<TestInfoConsumer>(provider);
});
```

上面的代码设置在5秒内，最多处理5个请求，剩余的请求会在设定的时间之后执行。

```c#
public string Log(string name, int age)
{
    for (int i = 0; i < 11; i++)
    {
        _busControl.Publish<TestInfo>(new TestInfo()
        {
            Name = name + i.ToString(),
            Age = age
        }); 
    }
    return string.Format("于时间:{0}，发布消息", DateTime.Now);
}
```

在消息的发布接口中，我们设定循环发布11个消息，因为我们设置了每5秒只能处理5个请求，所以11个请求会被分成三次进行分发。

![ratelimit.gif](https://gitee.com/imstrive/ImageBed/raw/master/20200401/ratelimit.gif)


# 短路

<https://github.com/MassTransit/MassTransit/blob/6aeb5acfdb/docs/advanced/middleware/circuit-breaker.md>



# 代码

<https://github.com/longtaosu/DotnetCoreDemo/tree/master/3.1/MassTransit/Advanced>



# 参考

<https://github.com/MassTransit/MassTransit/blob/6aeb5acfdb/docs/advanced/middleware/rate-limiter.md>

<https://github.com/MassTransit/MassTransit/blob/6aeb5acfdb1f1cf308c16128713a86692a8ea9f1/docs/advanced/middleware/rate-limiter.md>