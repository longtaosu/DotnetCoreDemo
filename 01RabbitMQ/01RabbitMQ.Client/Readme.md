# RabbitMQ简介

官方地址：https://www.rabbitmq.com/getstarted.html

安装说明：https://www.rabbitmq.com/download.html

官方Demo：https://github.com/rabbitmq/rabbitmq-tutorials

开发工具：https://www.rabbitmq.com/devtools.html



# 简介

RabbitMQ是一个消息队列，可以接收并传递消息。

想象一个邮局，在邮箱中放入邮件，邮差会将信件送到收件人的手中。在这个案例中，RabbitMQ就充当了邮箱、邮局和邮差的作用。

最大的不同点在于，RabbitMQ并不传递真正的信件，二十接受、存储并传递二进制数据——Messages。



# 专业术语

- 生产者

  用于发送消息

  ![img](https://www.rabbitmq.com/img/tutorials/producer.png)

- 队列

  队列相当于RabbitMQ内部的邮箱。尽管消息在RabbitMQ和应用中传递，他们只可以在队列中存储，队列的大小受宿主机内存和硬盘的限制。

  消息的缓冲区大小是很重要的，很多的生产者可以向同一个队列发送消息，很多的消费者可能会从同一个队列中接收消息。

  ![img](https://www.rabbitmq.com/img/tutorials/queue.png)

- 消费者

  消费者用于接收消息



**注意：生产者、队列和消费者不必在同一个主机（并且大部分程序确实部署在不同的主机上），一个应用程序可以既是生产者又是消费者。**



# 案例

- HelloWorld
- WorkQueues
- Publish/Subscribe
- Routing
- Topics
- RPC（尚未完成）

