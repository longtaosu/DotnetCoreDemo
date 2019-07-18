# 通过配置启动SuperSocket

## 为什么通过配置启动

1. 避免硬编码
2. SuperSocket提供了很多有用的配置选项
3. 可以充分利用SuperSocket提供的工具

## 如何使用Bootstrap来通过配置启动SuperSocket

- SuperSocket配置section SuperSocket使用 .NET 自带的配置技术，SuperSocket有一个专门的配置Section：

```xml
<configSections>
    <section name="superSocket"
         type="SuperSocket.SocketEngine.Configuration.SocketServiceConfig, SuperSocket.SocketEngine" />
</configSections>
```

- Server实例的配置

```xml
<superSocket>
    <servers>
      <server name="TelnetServer"
          serverType="SuperSocket.QuickStart.TelnetServer_StartByConfig.TelnetServer, SuperSocket.QuickStart.TelnetServer_StartByConfig"
          ip="Any" port="2020">
      </server>
    </servers>
</superSocket>
```

现在，我在这里解释配置的服务器节点：

```yml
name: 实例名称
serverType: 实例运行的AppServer类型
ip: 侦听ip
port: 侦听端口
```

我们将在下一份文档中有关于配置的更加完整的介绍

- 使用BootStrap启动SuperSocket

```c#
static void Main(string[] args)
{
    Console.WriteLine("Press any key to start the server!");

    Console.ReadKey();
    Console.WriteLine();

    var bootstrap = BootstrapFactory.CreateBootstrap();

    if (!bootstrap.Initialize())
    {
        Console.WriteLine("Failed to initialize!");
        Console.ReadKey();
        return;
    }

    var result = bootstrap.Start();

    Console.WriteLine("Start result: {0}!", result);

    if (result == StartResult.Failed)
    {
        Console.WriteLine("Failed to start!");
        Console.ReadKey();
        return;
    }

    Console.WriteLine("Press key 'q' to stop it!");

    while (Console.ReadKey().KeyChar != 'q')
    {
        Console.WriteLine();
        continue;
    }

    Console.WriteLine();

    //Stop the appServer
    bootstrap.Stop();

    Console.WriteLine("The server was stopped!");
    Console.ReadKey();
}
```

- 一些配置示例
- Server types节点

```c#
    <superSocket>
        <servers>
          <server name="TelnetServer"
              serverTypeName="TelnetServer"
              ip="Any" port="2020">
          </server>
        </servers>
        <serverTypes>
            <add name="TelnetServer" type="SuperSocket.QuickStart.TelnetServer_StartByConfig.TelnetServer, SuperSocket.QuickStart.TelnetServer_StartByConfig"/>
        </serverTypes>
    </superSocket>
```

多服务器实例：

```c#
    <superSocket>
        <servers>
          <server name="TelnetServerA"
              serverTypeName="TelnetServer"
              ip="Any" port="2020">
          </server>
          <server name="TelnetServerB"
              serverTypeName="TelnetServer"
              ip="Any" port="2021">
          </server>
        </servers>
        <serverTypes>
            <add name="TelnetServer" type="SuperSocket.QuickStart.TelnetServer_StartByConfig.TelnetServer, SuperSocket.QuickStart.TelnetServer_StartByConfig"/>
        </serverTypes>
      </superSocket>
```



## SuperSocket.SocketService.exe，SuperSocket提供的运行容器

1. 直接使用SuperSocket.SocketService.exe
2. 无比使你的Server所需要的所有程序集都和 SuperSocket.SocketService.exe 在同一目录
3. 将你的SuperSocket配置放置于SuperSocket.SocketService.exe.config文件中
4. 直接运行“SuperSocket.SocketService.exe”，你定义的服务器将会运行
5. 安装SuperSocket.SocketService.exe为Windows服务

通过在命令行下加参数 “-i” 运行 SuperSocket.SocketService.exe ，你可以安装它成为一个Windows服务：

```shell
SuperSocket.SocketService.exe -i
```

这个Windows服务的名字定义在配置文件之中，可以根据需要进行修改

```c#
<appSettings>
    <add key="ServiceName" value="SuperSocketService" />
</appSettings>
```

也可以通过参数“-u”来卸载服务

```c#
SuperSocket.SocketService.exe -u
```

