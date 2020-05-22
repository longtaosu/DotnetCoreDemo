# 消息广播

可实现消息的发布订阅：<https://github.com/jamesmh/coravel/blob/9f8f32d6f0b1b0f0cda1d13ba925ff1768ea42ac/DocsV2/docs/Events/README.md>



## 消息

```c#
public class BlogPostCreated : IEvent
{
    public BlogPost Post { get; set; }

    public BlogPostCreated(BlogPost post)
    {
        this.Post = post;
    }
}
```

## 订阅者

```c#
public class TweetNewPost : IListener<BlogPostCreated>
{
    private TweetingService _tweeter;

    public TweetNewPost(TweetingService tweeter){
        this._tweeter = tweeter;
    }

    public async Task HandleAsync(BlogPostCreated broadcasted)
    {
        var post = broadcasted.Post;
        await this._tweeter.TweetNewPost(post);
    }
}
```

## 发布者

```c#
public BlogController : Controller
{
    private IDispatcher _dispatcher;

    public BlogController(IDispatcher dispatcher)
    {
        this._dispatcher = dispatcher;
    }

    public async Task<IActionResult> NewPost(BlogPost newPost)
    {
        var postCreated = new BlogPostCreated(newPost);
        await _dispatcher.Broadcast(postCreated); // All listeners will fire.
    }
}
```

## 配置

```c#
//Configgure
IEventRegistration registration = app.ApplicationServices.ConfigureEvents();

registration.Register<DemoEvent>()
    .Subscribe<WriteMessageToConsoleListener>()
    .Subscribe<WriteStaticMessageToConsoleListener>();


//ConfigureServices
services.AddEvents();

services.AddTransient<WriteMessageToConsoleListener>()
        .AddTransient<WriteStaticMessageToConsoleListener>();
```



# 定时任务

实现周期执行的定时任务：

<https://github.com/jamesmh/coravel/blob/9f8f32d6f0/DocsV2/docs/Invocables/README.md>

```c#
//Configure
app.ApplicationServices.UseScheduler(scheduler =>
{
    scheduler.Schedule(() => Console.WriteLine($"Runs every minute Ran at: {DateTime.UtcNow}")).EverySecond();
});

//ConfigureServices
services.AddScheduler();

services.AddTransient<SendDailyStatsReport>();
services.AddTransient<SomeOtherInvocable>();
```



# 队列

```c#
//ConfigureServices
services.AddQueue();
```

## 控制器

```c#
IQueue _queue;

public HomeController(IQueue queue) {
    this._queue = queue;}

this._queue.QueueInvocable<GrabDataFromApiAndPutInDBInvocable>();
```

## 队列任务

```c#
public class DoExpensiveCalculationAndStore : IInvocable
{
    public async Task Invoke()
    {
        Console.Write("Doing expensive calculation for 15 sec...");
        await Task.Delay(15000);
        Console.Write("Expensive calculation done.");
    }
}
```





# 参考

<https://github.com/jamesmh/coravel>

<https://www.cnblogs.com/jerrymouseli/p/11054546.html>