# 总结

该Demo为 Publish/Subscribe 下最简单的Demo

注：

- 消息订阅支持多个消费者，当新增一个消费者时，消息会轮询到新的消费者
- 消息均等发送到消费者，即使有一个处理未完成，仍会继续接收消息

