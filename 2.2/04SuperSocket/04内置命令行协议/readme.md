# 内置的命令行协议

## 什么是协议

什么是协议? 很多人会回答 "TCP" 或者 "UDP"。 但是构建一个网络应用程序, 仅仅知道是 TCP 还是 UDP 是远远不够的。 TCP 和 UDP 是传输层协议。仅仅定义了传输层协议是不能让网络的两端进行通信的。你需要定义你的应用层通信协议把你接收到的二进制数据转化成你程序能理解的请求。

## 内置的命令行协议

命令行协议是一种被广泛应用的协议。一些成熟的协议如 Telnet, SMTP, POP3 和 FTP 都是基于命令行协议的。 在SuperSocket 中， 如果你没有定义自己的协议，SuperSocket 将会使用命令行协议, 这会使这样的协议的开发变得很简单。

**命令行协议定义了每个请求必须以回车换行结尾 "\r\n"。**

如果你在 SuperSocket 中使用命令行协议，所有接收到的数据将会翻译成 StringRequestInfo 实例。

StringRequestInfo 是这样定义的:

```c#
public class StringRequestInfo
{
    public string Key { get; }

    public string Body { get; }

    public string[] Parameters { get; }

    /*
    Other properties and methods
    */
}
```

由于 SuperSocket 中内置的命令行协议用空格来分割请求的Key和参，因此当客户端发送如下数据到服务器端时:

```c#
"LOGIN kerry 123456" + NewLine
```

SuperSocket 服务器将会收到一个 StringRequestInfo 实例，这个实例的属性为:

```c#
Key: "LOGIN"
Body: "kerry 123456";
Parameters: ["kerry", "123456"]
```

如果你定义了名为 "LOGIN" 的命令, 这个命令的 ExecuteCommand 方法将会被执行，服务器所接收到的StringRequestInfo实例也将作为参数传给这个方法:

```c#
public class LOGIN : CommandBase<AppSession, StringRequestInfo>
{
    public override void ExecuteCommand(AppSession session, StringRequestInfo requestInfo)
    {
        //Implement your business logic
    }
}
```

## 自定义你的命令行协议

有些用户可能会有不同的请求格式, 比如:

```c#
"LOGIN:kerry,12345" + NewLine
```

请求的 key 和 body 通过字符 ':' 分隔, 而且多个参数被字符 ',' 分隔。 支持这种类型的请求非常简单, 你只需要用下面的代码扩展命令行协议:

```
public class YourServer : AppServer<YourSession>
{
    public YourServer()
        : base(new CommandLineReceiveFilterFactory(Encoding.Default, new BasicRequestInfoParser(":", ",")))
    {

    }
}
```

如果你想更深度的定义请求的格式, 你可以基于接口 IRequestInfoParser 来实现一个 RequestInfoParser 类, 然后当实例化 CommandLineReceiveFilterFactory 时传入拟定一个 RequestInfoParser 实例:

```c#
public class YourServer : AppServer<YourSession>
{
    public YourServer()
        : base(new CommandLineReceiveFilterFactory(Encoding.Default, new YourRequestInfoParser()))
    {

    }
}
```

## 文本编码

命令行协议的默认编码是 Ascii，但是你也可以通过修改配置中的服务器节点的**"textEncoding"**属性来改变编码:

```c#
<server name="TelnetServer"
      textEncoding="UTF-8"
      serverType="YourAppServer, YourAssembly"
      ip="Any" port="2020">
</server>
```



# 内置的常用协议实现模版

阅读了前面一篇文档之后, 你可能会觉得用 SuperSocket 来实现你的自定义协议并不简单。 为了让这件事变得更容易一些, SuperSocket 提供了一些通用的协议解析工具, 你可以用他们简单而且快速的实现你自己的通信协议:

- **TerminatorReceiveFilter** (SuperSocket.SocketBase.Protocol.TerminatorReceiveFilter, SuperSocket.SocketBase)
- **CountSpliterReceiveFilter** (SuperSocket.Facility.Protocol.CountSpliterReceiveFilter, SuperSocket.Facility)
- **FixedSizeReceiveFilter** (SuperSocket.Facility.Protocol.FixedSizeReceiveFilter, SuperSocket.Facility)
- **BeginEndMarkReceiveFilter** (SuperSocket.Facility.Protocol.BeginEndMarkReceiveFilter, SuperSocket.Facility)
- **FixedHeaderReceiveFilter** (SuperSocket.Facility.Protocol.FixedHeaderReceiveFilter, SuperSocket.Facility)

## TerminatorReceiveFilter - 结束符协议

与命令行协议类似，一些协议用结束符来确定一个请求.

例如, 一个协议使用两个字符 "##" 作为结束符, 于是你可以使用类 "TerminatorReceiveFilterFactory":

```c#
/// <summary>
/// TerminatorProtocolServer
/// Each request end with the terminator "##"
/// ECHO Your message##
/// </summary>
public class TerminatorProtocolServer : AppServer
{
    public TerminatorProtocolServer()
        : base(new TerminatorReceiveFilterFactory("##"))
    {

    }
}
```

默认的请求类型是 StringRequestInfo, 你也可以创建自己的请求类型, 不过这样需要你做一点额外的工作:

基于TerminatorReceiveFilter实现你的接收过滤器(ReceiveFilter):

```c#
public class YourReceiveFilter : TerminatorReceiveFilter<YourRequestInfo>
{
    //More code
}
```

实现你的接收过滤器工厂(ReceiveFilterFactory)用于创建接受过滤器实例:

```c#
public class YourReceiveFilterFactory : IReceiveFilterFactory<YourRequestInfo>
{
    //More code
}
```

然后在你的 AppServer 中使用这个接收过滤器工厂(ReceiveFilterFactory).

## CountSpliterReceiveFilter - 固定数量分隔符协议

有些协议定义了像这样格式的请求 "#part1#part2#part3#part4#part5#part6#part7#". 每个请求有7个由 '#' 分隔的部分. 这种协议的实现非常简单:

```c#
/// <summary>
/// Your protocol likes like the format below:
/// #part1#part2#part3#part4#part5#part6#part7#
/// </summary>
public class CountSpliterAppServer : AppServer
{
    public CountSpliterAppServer()
        : base(new CountSpliterReceiveFilterFactory((byte)'#', 8)) // 7 parts but 8 separators
    {

    }
}
```

你也可以使用下面的类更深入的定制这种协议:

```c#
CountSpliterReceiveFilter<TRequestInfo>
CountSpliterReceiveFilterFactory<TReceiveFilter>
CountSpliterReceiveFilterFactory<TReceiveFilter, TRequestInfo>
```

## FixedSizeReceiveFilter - 固定请求大小的协议

在这种协议之中, 所有请求的大小都是相同的。如果你的每个请求都是有9个字符组成的字符串，如"KILL BILL", 你应该做的事就是想如下代码这样实现一个接收过滤器(ReceiveFilter):

```c#
class MyReceiveFilter : FixedSizeReceiveFilter<StringRequestInfo>
{
    public MyReceiveFilter()
        : base(9) //传入固定的请求大小
    {

    }

    protected override StringRequestInfo ProcessMatchedRequest(byte[] buffer, int offset, int length, bool toBeCopied)
    {
        //TODO: 通过解析到的数据来构造请求实例，并返回
    }
}
```

然后在你的 AppServer 类中使用这个接受过滤器 (ReceiveFilter):

```c#
public class MyAppServer : AppServer
{
    public MyAppServer()
        : base(new DefaultReceiveFilterFactory<MyReceiveFilter, StringRequestInfo>()) //使用默认的接受过滤器工厂 (DefaultReceiveFilterFactory)
    {

    }
}
```

## BeginEndMarkReceiveFilter - 带起止符的协议

在这类协议的每个请求之中 都有固定的开始和结束标记。例如, 我有个协议，它的所有消息都遵循这种格式 "!xxxxxxxxxxxxxx$"。因此，在这种情况下， "!" 是开始标记， "$" 是结束标记，于是你的接受过滤器可以定义成这样:

```c#
class MyReceiveFilter : BeginEndMarkReceiveFilter<StringRequestInfo>
{
    //开始和结束标记也可以是两个或两个以上的字节
    private readonly static byte[] BeginMark = new byte[] { (byte)'!' };
    private readonly static byte[] EndMark = new byte[] { (byte)'$' };

    public MyReceiveFilter()
        : base(BeginMark, EndMark) //传入开始标记和结束标记
    {

    }

    protected override StringRequestInfo ProcessMatchedRequest(byte[] readBuffer, int offset, int length)
    {
        //TODO: 通过解析到的数据来构造请求实例，并返回
    }
}
```

然后在你的 AppServer 类中使用这个接受过滤器 (ReceiveFilter):

```c#
public class MyAppServer : AppServer
{
    public MyAppServer()
        : base(new DefaultReceiveFilterFactory<MyReceiveFilter, StringRequestInfo>()) //使用默认的接受过滤器工厂 (DefaultReceiveFilterFactory)
    {

    }
}
```

## FixedHeaderReceiveFilter - 头部格式固定并且包含内容长度的协议

这种协议将一个请求定义为两大部分, 第一部分定义了包含第二部分长度等等基础信息. 我们通常称第一部分为头部.

例如, 我们有一个这样的协议: 头部包含 6 个字节, 前 4 个字节用于存储请求的名字, 后两个字节用于代表请求体的长度:

```
/// +-------+---+-------------------------------+
/// |request| l |                               |
/// | name  | e |    request body               |
/// |  (4)  | n |                               |
/// |       |(2)|                               |
/// +-------+---+-------------------------------+
```

使用 SuperSocket, 你可以非常方便的实现这种协议:

```
class MyReceiveFilter : FixedHeaderReceiveFilter<BinaryRequestInfo>
{
    public MyReceiveFilter()
        : base(6)
    {

    }

    protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
    {
        return (int)header[offset + 4] * 256 + (int)header[offset + 5];
    }

    protected override BinaryRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
    {
        return new BinaryRequestInfo(Encoding.UTF8.GetString(header.Array, header.Offset, 4), bodyBuffer.CloneRange(offset, length));
    }
}
```

你需要基于类FixedHeaderReceiveFilter实现你自己的接收过滤器.

- 传入父类构造函数的 6 表示头部的长度;
- 方法"GetBodyLengthFromHeader(...)" 应该根据接收到的头部返回请求体的长度;
- 方法 ResolveRequestInfo(....)" 应该根据你接收到的请求头部和请求体返回你的请求类型的实例.

然后你就可以使用接收或者自己定义的接收过滤器工厂来在 SuperSocket 中启用该协议.