# Consul

Consul：服务治理、配置中心

Polly：服务熔断

Ocelot：API网关



# 参考

http://beckjin.com/2019/05/18/aspnet-consul-discovery/



# 简介

Consul是一个用来实现分布式系统服务发现与配置的开源工具。内置了服务注册与发现框架、分布式一致性协议实现、健康检查、Key/Value存储、多数据中心方案，不再需要依赖其他工具（比如ZooKeeper等）。



# Consul架构

![Consul 架构图](http://beckjin.com/img/aspnet-consul-discovery/arch.png)

Consul集群支持多数据中心，在上图中有两个DataCenter，通过Internet互联，为了提高通信效率，只有Server节点才能加入跨数据中心的通信。在单个数据中心中，Consul分为Client和Server两个节点（所有的节点也被称为Agent），Server节点保存数据，Client负责健康检查及转发数据请求到Server，本身不保存注册信息；Server节点有一个Leader和多个Follower，Leader节点会将数据同步到Follower，Server节点的数量推荐是3个或者5个，在Leader挂掉的时候会启动选举机制产生一个新Leader。



# Consul集群搭建

这里使用 Docker 搭建 3个 Server 节点 + 1 个 Client 节点，API 服务通过 Client 节点进行服务注册和发现。

#### 从 Docker Hub 拉取 Consul 镜像

```shell
docker pull consul
```

#### 启动 3个 Server 节点 + 1 个 Client 节点

```shell
// Server  节点 1
docker run --name cs1 -p 8500:8500  -v /data/cs1:/data consul agent -server -bind 172.17.0.2 -node consul-server-1  -data-dir /data -bootstrap-expect 3 -client 0.0.0.0 -ui

// Server  节点 2
docker run --name cs2 -p 7500:8500  -v /data/cs2:/data consul agent -server -bind 172.17.0.3 -node consul-server-2  -data-dir /data -bootstrap-expect 3 -client 0.0.0.0 -ui -join 172.17.0.2

// Server  节点 3
docker run --name cs3 -p 6500:8500 -v /data/cs3:/data consul agent -server -bind 172.17.0.4 -node consul-server-3  -data-dir /data -bootstrap-expect 3 -client 0.0.0.0 -ui -join 172.17.0.2

// Client 节点 1
docker run --name cc1 -p 5500:8500 -v /data/cc1:/data consul agent -bind 172.17.0.5 -node consul-client-1 -data-dir /data -client 0.0.0.0 -ui -join 172.17.0.2
```

参数说明（–name、-p、-v 为 Docker 相关参数）：

| 参数名                | 解释                                                         |
| :-------------------- | :----------------------------------------------------------- |
| **–name**             | Docker 容器名称（每个 Consul 节点一个容器）                  |
| **-p**                | 容器内部 8500 端口映射到当前主机端口，因为使用的同一台主机，所以这里每个容器内的 8500 端口映射到当前主机的不同端口 |
| **-v**                | 将节点相关注册数据挂载到当前主机的指定位置，否则重启后会丢失 |
| **-server**           | 设置为 Server 类型节点，不加则为 Client 类型节点             |
| **-bind**             | 指定节点绑定的地址                                           |
| **-node**             | 指定节点名称                                                 |
| **-data-dir**         | 数据存放位置                                                 |
| **-bootstrap-expect** | 集群期望的 Server 节点数，只有达到这个值才会选举 Leader      |
| **-client**           | 注册或者查询等一系列客户端对它操作的IP，默认是127.0.0.1      |
| **-ui**               | 启用 UI 界面                                                 |
| **-join**             | 指定要加入的节点地址（组建集群）                             |

#### 集群状态

（*cs1 为容器名称，任意一个即可*）

##### 查看节点状态和类型

```
docker exec -t cs1 consul members
```

[![consul members](http://beckjin.com/img/aspnet-consul-discovery/consulMembers.png)](http://beckjin.com/img/aspnet-consul-discovery/consulMembers.png)

当前为3 个 Server 类型节点 ，1 个 Client 类型节点。

##### 查看 Server 节点类型

```
docker exec -t cs1 consul operator raft list-peers
```

[![consul raft](http://beckjin.com/img/aspnet-consul-discovery/consulRaft.png)](http://beckjin.com/img/aspnet-consul-discovery/consulRaft.png)

当前为 consul-server-1 为 leader，可以测试将 consul-server-1 kill 观察 leader 的重新选举。

通过 [http://192.168.124.8:8500](http://192.168.124.8:8500/) UI 界面查看 Consul 节点状态如下：
*:7500、:6500、:5500 均可，192.168.124.8 是当前主机网络的 IPv4 地址*
[![consul ui](http://beckjin.com/img/aspnet-consul-discovery/consulUI.png)](http://beckjin.com/img/aspnet-consul-discovery/consulUI.png)

### .NET Core 接入 Consul

[![service discovery](http://beckjin.com/img/aspnet-consul-discovery/serviceDiscovery.png)](http://beckjin.com/img/aspnet-consul-discovery/serviceDiscovery.png)

1. 创建 .NET Core WebAPI 服务 ServiceA（2个实例） 和 ServiceB

2. NuGet 安装 Consul

3. 注册到 Consul 的核心代码如下（[源码下载](https://github.com/beckjin/ConsulDotnetSamples)）：

   ```
   public static class ConsulBuilderExtensions
   {
     public static IApplicationBuilder RegisterConsul(this IApplicationBuilder app, IApplicationLifetime lifetime, ConsulOption consulOption)
     {
       var consulClient = new ConsulClient(x =>
       {
         // consul 服务地址
         x.Address = new Uri(consulOption.Address);
       });
   
       var registration = new AgentServiceRegistration()
       {
         ID = Guid.NewGuid().ToString(),
         Name = consulOption.ServiceName,// 服务名
         Address = consulOption.ServiceIP, // 服务绑定IP
         Port = consulOption.ServicePort, // 服务绑定端口
         Check = new AgentServiceCheck()
         {
           DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),//服务启动多久后注册
           Interval = TimeSpan.FromSeconds(10),//健康检查时间间隔
           HTTP = consulOption.ServiceHealthCheck,//健康检查地址
           Timeout = TimeSpan.FromSeconds(5)
         }
       };
   
       // 服务注册
       consulClient.Agent.ServiceRegister(registration).Wait();
   
       // 应用程序终止时，服务取消注册
       lifetime.ApplicationStopping.Register(() =>
       {
         consulClient.Agent.ServiceDeregister(registration.ID).Wait();
       });
       return app;
     }
   }
   ```

4. 添加配置如下：

   ```
   "ServiceName": "ServiceA",
   "ServiceIP": "192.168.124.8",
   "ServicePort": 8000,
   "ServiceHealthCheck": "http://192.168.124.8:8000/healthCheck",
   "ConsulAddress": "http://192.168.124.8:8500"
   ```

5. 注册成功结果如下：

   [![service register](http://beckjin.com/img/aspnet-consul-discovery/serviceRegister.png)](http://beckjin.com/img/aspnet-consul-discovery/serviceRegister.png)

6. ServiceB 调用 ServiceA 接口

   ServiceB 通过 ConsulClient 进行服务发现，获取到 ServiceA 的地址，然后随机任意一台进行请求，核心代码如下：

   ```
   var url = _configuration["ConsulAddress"].ToString();
   
   using (var consulClient = new ConsulClient(a => a.Address = new Uri(url)))
   {
     var services = consulClient.Catalog.Service("ServiceA").Result.Response;
     if (services != null && services.Any())
     {
       // 模拟随机一台进行请求，这里只是测试，可以选择合适的负载均衡工具或框架
       Random r = new Random();
       int index = r.Next(services.Count());
       var service = services.ElementAt(index);
   
       using (HttpClient client = new HttpClient())
       {
         var response = await client.GetAsync($"http://{service.ServiceAddress}:{service.ServicePort}/values/test");
         var result = await response.Content.ReadAsStringAsync();
         return result;
       }
     }
   }
   ```

7. 多次调用 ServiceB 接口结果如下：

[![result-1](http://beckjin.com/img/aspnet-consul-discovery/result1.png)](http://beckjin.com/img/aspnet-consul-discovery/result1.png)

[![result-2](http://beckjin.com/img/aspnet-consul-discovery/result2.png)](http://beckjin.com/img/aspnet-consul-discovery/result2.png)

### 参考链接

- [Consul](https://www.consul.io/api/index.html)
- [使用 Consul 做服务发现的若干姿势](https://www.cnblogs.com/bossma/p/9756809.html)
- [ConsulDotnetSamples 源码](https://github.com/beckjin/ConsulDotnetSamples)