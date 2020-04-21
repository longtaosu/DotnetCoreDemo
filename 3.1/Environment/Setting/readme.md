# 目标

- [x] 开发时根据环境变量选择配置文件
- [x] 部署时根据环境变量选择配置文件



# 开发

## launchSettings.json

在 `launchSetting.json` 中设置的环境值可以替代系统环境中设置的值，里面包含了3个配置文件：

- IIS
- IIS Express
- Project（启动 Kestrel 项目）

> 在Windows和macOS上，环境变量和值不区分大小写。默认情况下，Linux环境变量和值要区分大小写

可以通过修改 `launchSetting.json` 中的 `applicationUrl` 属性指定服务器Url 的列表，列表中的URL之间使用分号：

```json
"EnvironmentsSample": {
   "commandName": "Project",
   "launchBrowser": true,
   "applicationUrl": "https://localhost:5001;http://localhost:5000",
   "environmentVariables": {
     "ASPNETCORE_ENVIRONMENT": "Development"
   }
}
```

使用 `dotnet run` 启动应用时，如果launchSettings.json可用，则里面的设置会替代环境变量。

![调试.png](https://gitee.com/imstrive/ImageBed/raw/master/20200421/调试.png)

## 使用命令行

```shell
dotnet *.dll --environment=Test
```



# 部署

> 如果未设置环境，默认值是 Production

## 设置环境变量

- Windows

```shell
set ASPNETCORE_ENVIRONMENT=Development
```

- 系统设置

“控制面板” >“系统” >“高级系统设置” ，再添加或编辑“`ASPNETCORE_ENVIRONMENT`”值

## pubxml/webconfig

> 该方法最终都是通过webconfig文件实现配置，但是webcofnig依赖于iis，所以即使使用cli也无法控制环境变量

pubxml

```xml
<PropertyGroup>
  <EnvironmentName>LTS</EnvironmentName>
</PropertyGroup>
```

web.config

```xml
<aspNetCore processPath="%LAUNCHER_PATH%" arguments="%LAUNCHER_ARGS%" stdoutLogEnabled="false" hostingModel="inprocess">
  <environmentVariables>
    <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Test" />
  </environmentVariables>
</aspNetCore>
```

## IHostBuilder

```c#
public static void Main(string[] args)
{
    CreateHostBuilder(args)
    .UseEnvironment("Environment")
    .Build()        
    .Run();
}
```



## Service文件

service中通过环境变量实现对应用的单独配置

```ini
[Unit]
Description=Example .NET Web API App running on Ubuntu

[Service]
WorkingDirectory=/var/www/helloapp
ExecStart=/usr/bin/dotnet /var/www/helloapp/helloapp.dll
Restart=always
# Restart service after 10 seconds if the dotnet service crashes:
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=dotnet-example
User=root
Environment=ASPNETCORE_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target
```



# 设定Url

## UseUrls

在Programs.cs文件中，修改 `CreateHostBuilder` 方法

```c#
webBuilder.UseUrls("http://localhost:5003", "https://localhost:5004");
```

## 环境变量

- DOTNET_URLS
- ASPNETCORE_URL

## 命令行

通过cli工具设定启动的url

```c#
dotnet run --urls "http://localhost:5100"
```

## launchSettings.json

修改applicationUrl配置：

```json
"TestApp": {
      "commandName": "Project",
      "launchBrowser": true,
      "applicationUrl": "https://localhost:5001;http://localhost:5000",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
```

## Listen

```c#
public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseKestrel(opts =>
                {
                    // Bind directly to a socket handle or Unix socket
                    // opts.ListenHandle(123554);
                    // opts.ListenUnixSocket("/tmp/kestrel-test.sock");
                    opts.Listen(IPAddress.Loopback, port: 5002);
                    opts.ListenAnyIP(5003);
                    opts.ListenLocalhost(5004, opts => opts.UseHttps());
                    opts.ListenLocalhost(5005, opts => opts.UseHttps());
                });

            });
```

## hosting.json

添加配置文件hosting.json

```json
"urls": "http://*:5005;"
```

修改Program.cs文件

```c#
.ConfigureAppConfiguration((context, config) =>
{
    config.Sources.Clear();

    config.AddJsonFile("asssettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("hosting.json", optional: true)
    ;

    config.AddEnvironmentVariables();
})
```



# 参考

<https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/environments?view=aspnetcore-3.1>

https://andrewlock.net/5-ways-to-set-the-urls-for-an-aspnetcore-app/