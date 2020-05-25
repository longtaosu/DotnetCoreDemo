# 功能

| 功能               | 描述                                                         |
| ------------------ | ------------------------------------------------------------ |
| Task Scheduling    | 通过简单、优雅的语法创建Windows任务                          |
| Queuing            | Coracel提供基于内存的零配置的队列，将耗时任务变为后台任务进而避免用户长时间等待Http请求 |
| Caching            | 提供简单的Api简化应用的缓存。默认使用 in-memory缓存，对于复杂场景提供数据库驱动 |
| Event Broadcasting | 用于构建松耦合可维护的应用                                   |
| Mailing            | 邮件服务                                                     |



# Task Scheduling

一般通过 cron job 配置windows 任务，这些任务会执行单词或多次。

## Config

在ConfigureServices()，添加如下代码：

```c#
services.AddScheduler()
```

在Configure()，进行如下配置：

```c#
var provider = app.ApplicationServices;
provider.UseScheduler(scheduler =>
{
    scheduler.Schedule(
        () => Console.WriteLine("Every minute during the week.")
    )
    .EveryMinute()
    .Weekday();
});
```

![](images\01周期执行.png)



## Scheduling Tasks

> 建议创建类型为 Invocables 类型的任务

创建 Invocables 类型的任务，需要注意：

- 使用provider注册的服务需要是scoped或者transient类型
- 使用 `Schedule` 方法

```c#
scheduler
    .Schedule<GrabDataFromApiAndPutInDBInvocable>()
    .EveryTenMinutes();
```

上面基于表达式语法的代码很简洁。

>  取消长时间运行的 Invocables
>
> 让长期运行 Invocables 的类实现 `Coravel.Invocable.ICancellableInvocable` 接口，进而可以在程序关闭时可以简单的关闭任务。
>
> 接口包含一个属性 CancellationToken，可以使用 CancellationToken.IsCancellationRequested 进行检测。



### Async Tasks

Coravel可以通过使用 `ScheduleAsync()` 方法处理 `async` 任务。

> 提示
>
> `ScheduleAsync` 不需要 awaited，提供的方法或者 `Func` 需要是awaitable（需要返回Task或者Task<T>类型）。 

```c#
scheduler.ScheduleAsync(async () =>
{
    await Task.Delay(500);
    Console.WriteLine("async task");
})
.EveryMinute();
```



### 带参数的任务

使用 ScheduleWithParams<T> 方法可以创建带有不同参数的 Invocable。

> 参数中接口会根据DI自动调用（无需操作），参数需要声明即可

```c#
private class BackupDatabaseTableInvocable : IInvocable
{
    private DbContext _dbContext;
    private string _tableName;

    public BackupDatabaseTableInvocable(DbContext dbContext, string tableName)
    {
        this._dbContext = dbContext; // Injected via DI.
        this._tableName = tableName; // injected via schedule configuration (see next code block).
    }

    public Task Invoke()
    {
        // Do the logic.
    }
}
```

配置代码如下

```c#
// In this case, backing up products 
// more often than users is required.

scheduler
    .ScheduleWithParams<BackupDatabaseTableInvocable>("[dbo].[Users]")
    .Daily();

scheduler
    .ScheduleWithParams<BackupDatabaseTableInvocable>("[dbo].[Products]")
    .EveryHour();
```



## Intervals

在调用 `Schedule` 或者 `ScheduleAsync` 方法时，可以指定时间间隔

> 默认使用 UTC 时间

| 方法               | 描述               |
| ------------------ | ------------------ |
| EverySecond()      | 每秒               |
| EveryFiveSeconds() | 每5秒              |
| EverySeconds(3)    | 每3秒              |
| EveryMinute()      | 每分钟             |
| EveryFiveMinutes() | 每5分钟            |
| Hourly()           | 每小时             |
| HourlyAt(12)       | 每个小时的第12分钟 |
| Daily()            | 每天午夜一次       |
| DailyAtHour(13)    | 每天下午一点       |
| DailyAt(13, 30)    | 每天13：30执行     |
| Weekly()           | 每周一次           |
| Cron("* * * * *")  | Cron表达式         |

Cron表达式

- `* * * * *`：每分钟
- `00 13 * * *`：每天下午1点
- `00 1,2,3 * * *`：每天1点，2点，3点
- `00 1-3 * * *`：时间同上
- `00 */2 * * *`：每2个小时执行一次



## Day Constraints

当指定了时间间隔后，需要进一步的限制执行的day。

- Monday()
- Weekday()
- Weekend()

同时可以调用链式表达式 Mondy().Wednesday() ，该任务会在周一和周三执行。



## Extras

### 时区

如果运行时需要限定时区，可以使用 `Zoned` 方法：

```c#
scheduler
    .Schedule<SendWelcomeUserEmail>()
    .DailyAt(13, 30)
    .Zoned(TimeZoneInfo.Local);
```

### 自定义Bool约束

使用 When 方法可以添加额外的约束条件，用于决定任务是否可以执行

```c#
scheduler
    .Schedule(() => DoSomeStuff())
    .EveryMinute()
    .When(SomeMethodThatChecksStuff);
```

### 全局错误处理

任何抛出的异常会被跳过，这时会顺序触发下一个任务。

如果想要捕获特定的异常，可以使用 OnError() 方法

```c#
provider.UseScheduler(scheduler =>
    // Assign your schedules
)
.OnError((exception) =>
    DoSomethingWithException(exception)
);
```

### Prevent Overlapping Tasks

有时会有一些长时间运行的任务，任务的执行时间是变化的。一般是任务需要执行时立即触发。

但是如果任务需要执行时，上一个任务还在执行如何处理呢？

这种情况下，使用 `PreventOverlapping` 方法保证只有1个任务在执行（**即跳过一次触发**）。

```c#
scheduler
    .Schedule<SomeInvocable>()
    .EveryMinute()
    .PreventOverlapping("SomeInvocable");
```

## Schedule Workers

在web场景下，任务会按照顺序执行（即使是异步的）。如果这时存在一个耗时的任务，会使其他任务的预期执行时间。

![](images\02任务冲突.png)

使用方法 `OnWorker(string workerName)` 方法分配 worker。

```c#
scheduler.OnWorker("EmailTasks");
scheduler
    .Schedule<SendNightlyReportsEmailJob>().Daily();
scheduler
    .Schedule<SendPendingNotifications>().EveryMinute();

scheduler.OnWorker("CPUIntensiveTasks");
scheduler
    .Schedule<RebuildStaticCachedData>().Hourly();
```

![](images\03解决冲突.png)



## On App Closing

当应用停止的时候，Coravel会在所有运行的任务都完成后才停止。此时应用会在后台运行（只要父级进程没被杀死）





# Queuing

Coravel提供了一个基于内存、无需配置的队列。

## Config

```c#
//ConfigureServices()
services.AddQueue();
```



## Essentials

### Setup

在控制器注入接口 Coravel.Queuing.Interfaces.IQueue的实例。

```c#
IQueue _queue;

public HomeController(IQueue queue) {
    this._queue = queue;
}
```

### Queuing Invocables

> BroadcastInvocable需要在ConfigureServices中进行配置
>
> ```c#
> services.AddQueue();
> services.AddScoped<BroadcastInvocable>();
> ```

使用方法 `QueueInvocable` 将任务放入队列：

```c#
this._queue.QueueInvocable<BroadcastInvocable>();
```

![](images\05Broadcast.png)

### Queue An invocable with a payload

当将一个后台任务放入队列时，需要提供 payload/parameters。

比方说需要执行一个名为 `SendWelcomeUserEmailInvocable` 的任务，任务需要提供用户的信息。这时任务需要继承 IInvocableWithPayload<T> 接口：

```c#
                                                         // This one 👇
public class SendWelcomeUserEmailInvocable : IInvocable, IInvocableWithPayload<UserModel>
{
  // This is the implementation of the interface 👇
  public UserModel Payload { get; set; }

  /* Constructor, etc. */

  public async Task Invoke()
  {
    // `this.Payload` will be available to use now.
  }
}
```

使用方法 QueueInvocableWithPayload，将任务放入队列：

```c#
var userModel = await _userService.Get(userId);
queue.QueueInvocableWithPayload<SendWelcomeUserEmailInvocable, UserModel>(userModel);
```

![](images\06payload.png)

### Queuing An Async Task

使用方法 `QueueAsyncTask` 将异步任务放入队列。

```c#
this._queue.QueueAsyncTask(async() => {
    await Task.Delay(1000);
    Console.WriteLine("This was queued!");
 });
```

###  Queuing A Synchronous Task

使用方法 QueueTask 将同步任务放入队列。

```c#
public IActionResult QueueTask() {
    this._queue.QueueTask(() => Console.WriteLine("This was queued!"));
    return Ok();
}
```

### Queuing An Event Broadcast

系统中事件广播很重要，如果监听者在处理耗时的任务呢？

通过 `QueueBroadcast` 方法，可以将事件在后台进行广播

创建一个消息的载体

```c#
public class DemoEvent : IEvent
{
    public string Message { get; set; }

    public DemoEvent(string message)
    {
        Message = message;
    }
}
```

创建消息的监听程序（同样的程序创建Listener2）

```c#
public class Listener1 : IListener<DemoEvent>
{
    public Task HandleAsync(DemoEvent broadcasted)
    {
        Console.WriteLine($"Listener 1 接收到消息：{broadcasted.Message}");
        return Task.CompletedTask;
    }
}
```

在StartUp中进行配置

```c#
//ConfigureServices
services.AddEvents();
services.AddTransient<Listener1>()
        .AddTransient<Listener2>();

//Configure
IEventRegistration registration = app.ApplicationServices.ConfigureEvents();

registration.Register<DemoEvent>()
    .Subscribe<Listener1>()
    .Subscribe<Listener2>();
```

发布消息

```c#
public IActionResult TriggerBroadcastEvent()
{
    _queue.QueueBroadcast(new DemoEvent(DateTime.Now.ToString()));
    return Ok();
}
```

![](images\04BroadcastEvent.png)

## Extras

### Global Error Handling

在Startup中可以进行异常处理配置

```c#
var provider = app.ApplicationServices;
provider
    .ConfigureQueue()
    .OnError(e =>
    {
        //... handle the error
    });
```

### 调整消费者的延时时间

通常队列中的消息每间隔30s进行一次消息的消费，可以在 appsettings.json文件中进行配置：

```json
"Coravel": {
  "Queue": {
    "ConsummationDelay": 1
  }
}
```











# Caching



# Event Broadcasting





# 参考

<https://github.com/jamesmh/coravel>

<https://docs.coravel.net/Installation/>