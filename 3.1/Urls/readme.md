# 设定Asp.net Core应用启动地址的5种方式

默认情况下，Asp.net Core应用启动后监听下面的Urls：

- http://localhost:5000
- http://localhost:5001

下面将介绍在Asp.net Core 3.x下设定监听地址的几种方式：

- `UseUrls()`：在 *Program.cs* 下设定Urls。
- 环境变量：设置 `DOTNET_URLS` 或者 `ASPNETCORE_URLS`。
- 命令行参数：在命令行启动时，使用 `--urls` 参数。
- 使用 `launchSetting.json` 文件：使用 *Properties/launchSettings.json* 文件中的参数 `applicationUrl` 。
- KestrelServerOptions.Listen()：使用 Listen() 方法手动配置 Kestrel 服务器的地址。



# UseUrls()

使用UseUrls()进行硬编码：

```c#
public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseUrls("http://localhost:5003", "https://localhost:5004");
            });
}
```

该方法通常仅仅用于演示，并不用于实际环境。



# Enviroment variables

.Net Core使用两种类型的配置：

- **App Configuration** ：应用中常用的配置，从 `appSettings.json` 文件或者 环境变量 中加载。
- **Host Configuration**：用于配置应用最底层，比方说 [hosting environment](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-3.1) 和使用的Urls。

而 `Host Configuration` 是应用加载监听地址的配置文件。默认情况下，`Host Configuration`配置信息来自3个不同的来源：

- 以 `DOTNET_` 作为前缀的环境变量，环境变量移除了这个前缀后加入到了 Collection。
- 命令行参数
- 以 `ASPNETCORE_` 作为前缀的环境变量，这类环境变量只适用于Asp.net Core应用。

如果不是手动的使用 `UseUrls()` 方法，Asp.net Core会使用配置信息中的 URLS 信息。根据刚才的描述，我们可以通过下面两个环境变量设置：

- DOTNET_URLS
- ASPNETCORE_URLS

> 如果这两个环境变量都设置了，会优先使用ASPNETCORE_URLS.



# 命令行

命令行中使用 --urls 参数，会覆盖环境变量中的信息。

```shell
dotnet *.dll --urls="http://*:[port]"
```

环境变量和命令行参数可能是生产环境设置URLs最常用的方式，但是对于本地的调试环境可能还是有些麻烦。

本地环境我们更倾向于配置 `launchSettings.json` 。



# launchingSettings.json

*launchingSettings.json* 文件在Properties文件夹下，这个文件包含了程序启动的很多配置信息。

```json
"ApiDemo": {
  "commandName": "Project",
  "launchBrowser": true,
  "launchUrl": "weatherforecast",
  "applicationUrl": "https://localhost:5001;http://localhost:5000",
  "environmentVariables": {
    "ASPNETCORE_ENVIRONMENT": "Development"
  }
```



# 获取随机地址

```shell
dotnet *.dll --urls="http://*:0"
```





# 参考

<https://andrewlock.net/5-ways-to-set-the-urls-for-an-aspnetcore-app/>