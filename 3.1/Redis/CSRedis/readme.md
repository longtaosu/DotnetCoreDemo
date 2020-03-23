# SDK

<https://github.com/2881099/csredis>

安装依赖：CSRedisCore



# 基本操作

初始化

```c#
RedisHelper.Initialization(new CSRedis.CSRedisClient(Configuration.GetValue<string>("Redis:ConnectString")));
```

查询

```c#
var result = RedisHelper.Get(key);
```

设置

```c#
var result = RedisHelper.Set(key, DateTime.Now.ToString());
```

设置过期时间

```c#
var result = RedisHelper.Set(key, DateTime.Now, 60);
```



# 发布订阅

发布

```c#
RedisHelper.Publish(key, DateTime.Now.ToString());
```

订阅

```c#
RedisHelper.Subscribe((key,msg => Console.WriteLine(msg.Body)));
```

