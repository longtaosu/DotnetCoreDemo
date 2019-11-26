# Telnet示例

## 1.添加引用

- SuperSocket
- SuperSocket.Engine



## 2.创建服务

```c#
var appServer = new AppServer(); //实例化
appServer.Setup(2012);           //设置端口
appServer.Start();               //启动服务
```



## 3.处理连接

注册会话新建事件处理方法

```c#
appServer.NewSessionConnected += new SessionHandler<AppSession>(appServer_NewSessionConnected);
```

在事件处理代码中发送欢迎信息给客户端

```c#
static void appServer_NewSessionConnected(AppSession session)
{
    session.Send("Welcome to SuperSocket Telnet Server");
}
```



## 4.处理请求

### 4.1连接请求处理方法

```c#
appServer.NewRequestReceived += new RequestHandler<AppSession, StringRequestInfo>(appServer_NewRequestReceived);
```

### 4.2实现请求处理

```c#
static void appServer_NewRequestReceived(AppSession session, StringRequestInfo requestInfo)
{
    switch (requestInfo.Key.ToUpper())
    {
        case("ECHO"):
            session.Send(requestInfo.Body);
            break;

        case ("ADD"):
            session.Send(requestInfo.Parameters.Select(p => Convert.ToInt32(p)).Sum().ToString());
            break;

        case ("MULT"):

            var result = 1;

            foreach (var factor in requestInfo.Parameters.Select(p => Convert.ToInt32(p)))
            {
                result *= factor;
            }

            session.Send(result.ToString());
            break;
    }
}
```

requestinfo.Key是请求的命令行用空格分隔开的第一部分

requestinfo.Parameters是用空格分隔开的其余部分

### 4.3通过Telnet客户端进行测试

当然和服务器端建立连接之后，你可以通过下面的方式与服务器端交互("C:"之后的信息代表客户端的请求，"S:"之后的信息代表服务器端的响应):

注：客户端数据后需要接

```c#
C: ECHO ABCDEF
S: ABCDEF
C: ADD 1 2
S: 3
C: ADD 250 250
S: 500
C: MULT 2 8
S: 16
C: MULT 125 2
S: 250
```



## 5.Command的用法

服务器端包含有很多复杂的业务逻辑，这样的switch/case代码将会很长而且非常难看，并且没有遵循OOD的原则。 在这种情况下，SuperSocket提供了一个让你在多个独立的类中处理各自不同的请求的命令框架。



例如，你**可以定义一个名为"ADD"的类去处理Key为"ADD"的请求**:

```c#
public class ADD : CommandBase<AppSession, StringRequestInfo>
{
    public override void ExecuteCommand(AppSession session, StringRequestInfo requestInfo)
    {
        session.Send(requestInfo.Parameters.Select(p => Convert.ToInt32(p)).Sum().ToString());
    }
}
```

定义一个**名为"MULT"的类去处理Key为"MULT"的请求**:

```c#
public class MULT : CommandBase<AppSession, StringRequestInfo>
{
    public override void ExecuteCommand(AppSession session, StringRequestInfo requestInfo)
    {
        var result = 1;

        foreach (var factor in requestInfo.Parameters.Select(p => Convert.ToInt32(p)))
        {
            result *= factor;
        }

        session.Send(result.ToString());
    }
}
```

同时你要**移除请求处理方法的注册，因为它和命令不能同时被支持**：

```c#
//Remove this line
appServer.NewRequestReceived += new RequestHandler<AppSession, StringRequestInfo>(appServer_NewRequestReceived);
```

