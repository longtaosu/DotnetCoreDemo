# 自定义AppServer和AppSession

## 1.什么是AppSession？

AppSession代表一个和客户端的逻辑连接，基于连接的操作应该定于在该类之中。可以用该类的实例发送数据到客户端，接收客户端发送的数据或者关闭连接。

## 2.什么是AppServer

AppServer代表了监听客户端连接，承载了TCP连接的服务器实例。理想情况下，可以通过AppServer实例获取任何你想要的客户端连接，服务器级别的操作和逻辑应该定义在此类之中。

## 3.创建AppSession

### 3.1重写基类的方法

```c#
public class TelnetSession : AppSession<TelnetSession>
{
    protected override void OnSessionStarted()
    {
        this.Send("Welcome to SuperSocket Telnet Server");
    }

    protected override void HandleUnknownRequest(StringRequestInfo requestInfo)
    {
        this.Send("Unknow request");
    }

    protected override void HandleException(Exception e)
    {
        this.Send("Application error: {0}", e.Message);
    }

    protected override void OnSessionClosed(CloseReason reason)
    {
        //add you logics which will be executed after the session is closed
        base.OnSessionClosed(reason);
    }
}
```

在上面的代码中，当一个新的连接连接上时，服务器立即向客户端发送欢迎信息。这段代码还重写了其他的AppSession的方法用以实现自己的业务逻辑。

### 3.2增加Session属性

根据业务增加Session的属性，创建一个用在游戏服务器中的AppSession类

```c#
public class PlayerSession ：AppSession<PlayerSession>
{
    public int GameHallId { get; internal set; }

    public int RoomId { get; internal set; }
}
```

### 3.3和Command之间的关系

上一篇中，提到了Command

```c#
public class ECHO : CommandBase<AppSession, StringRequestInfo>
{
    public override void ExecuteCommand(AppSession session, StringRequestInfo requestInfo)
    {
        session.Send(requestInfo.Body);
    }
}
```

在这个Command代码中，可以看到ECHO的父类是CommandBase<AppSession, StringRequestinfo>，它有一个泛型参数AppSession。是的，它是默认的AppSession类。如果在系统中使用自己建立的AppSession类，则必须将自己定义的AppSession类传进去，否则服务器无法发现这个Command。

```c#
public class ECHO : CommandBase<PlayerSession, StringRequestInfo>
{
    public override void ExecuteCommand(PlayerSession session, StringRequestInfo requestInfo)
    {
        session.Send(requestInfo.Body);
    }
}
```

## 4.创建AppServer

### 4.1新建AppServer

如果想使用自定义的AppSession作为会话，必须修改AppServer来使用它

```c#
public class TelnetServer : AppServer<TelnetSession>
{

}
```

### 4.2重写protected方法

```c#
public class TelnetServer : AppServer<TelnetSession>
{
    protected override bool Setup(IRootConfig rootConfig, IServerConfig config)
    {
        return base.Setup(rootConfig, config);
    }

    protected override void OnStartup()
    {
        base.OnStartup();
    }

    protected override void OnStopped()
    {
        base.OnStopped();
    }
}
```

### 4.3优点

实现自己的AppSession和AppServer允许你根据业务的需求来方便的扩展SuperSocket，可以绑定session的连接和断开事件，服务器实例的启动和停止事件。

还可以在AppServer的Setup方法中读取你的自定义配置信息。