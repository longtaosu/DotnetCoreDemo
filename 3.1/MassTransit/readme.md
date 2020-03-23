# 官方

**Github：**

<https://github.com/MassTransit/MassTransit>

<https://github.com/MassTransit/MassTransit/blob/6a035f2571/docs/usage/README.md>

**Document：**

<https://masstransit-project.com/getting-started/>



# 配置

```c#
ep.UseMessageRetry(r => r.Interval(10, 2000));
```

如果失败，重试10次，每次间隔2000ms。如果仍然失败，消息会进入异常队列。

如果不做声明，则失败后不进行重试，直接进入异常队列。







# 生产者







# 消费者





# Pub/Sub





# Send/Receive



# Observer





# 参考

<https://www.cnblogs.com/edisonchou/p/dnc_microservice_masstransit_foundation_part1.html>

<https://www.cnblogs.com/richieyang/p/5492432.html>