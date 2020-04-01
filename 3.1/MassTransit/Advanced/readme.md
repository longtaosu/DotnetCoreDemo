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

![](F:\03Github\06DotnetCoreDemo\3.1\MassTransit\Advanced\Images\retry.gif)

> 异常发生时，会生成一个异常队列，发生异常的信息会保存至该异常队列。如上例中创建队列“lts111”，异常发生时会创建异常队列“lts111_error”

![](F:\03Github\06DotnetCoreDemo\3.1\MassTransit\Advanced\Images\error_queue.png)



# 限流







# 短路





# 参考





