# 概念

静态文件（如HTML、CSS、图像和JS）是ASP.NET Core应用直接提供给客户端的资产，需要进行一些配置才能提供这些文件。

静态文件存储在项目的web根目录中，默认目录是 `{content root}/wwwroot` ，但是可以通过 `UseWebRoot` 方法修改。



# 使用默认路径

新建 `wwwroot` 文件夹，添加images文件夹，添加图片两张，分别为 .net.png 和 vue.png（文件设置为较新则复制）。

![](F:\03Github\06DotnetCoreDemo\3.1\静态文件\Images\02新建wwwroot文件夹.png)

在 Startup.Configure 中使用 `UseStaticFiles` 方法：

```c#
public void Configure(IApplicationBuilder app)
{
    app.UseStaticFiles();
}
```

无参数的 UseStaticFiles 方法将 `Web根目录` 中的文件标记为可用，引用方法如下

```html
<img src="~/images/banner1.svg" alt="ASP.NET" class="img-responsive" />
```

也可以通过地址栏直接访问

![](F:\03Github\06DotnetCoreDemo\3.1\静态文件\Images\03访问vue.png.png)



# 使用其他路径

- **wwwroot**
  - **css**
  - **images**
  - **js**
- **MyStaticFiles**
  - **images**
    - java.png

如果此时想访问 java.png，可以修改设置如下：

```c#
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "MyStaticFiles")),
    RequestPath = "/StaticFiles"
});
```

> 两个UseStaticFiles，无参的提供对wwwroot目录中静态文件，第二个通过 “/StaticFiles”浏览 “MyStaticFiles”文件夹下的静态文件

引用方法如下：

```html
<img src="~/StaticFiles/images/java.png" alt="ASP.NET" class="img-responsive" />
```

也可以通过地址栏访问

![](F:\03Github\06DotnetCoreDemo\3.1\静态文件\Images\04其他目录.png)



# 设置HTTP响应标头

`StaticFileOptions` 对象可用于设置HTTP响应标头。除配置从 `Web根目录` 提供静态文件wait，还可以设置 `Cache-Control` 标头

> [HeaderDictionaryExtensions.Append](https://docs.microsoft.com/zh-cn/dotnet/api/microsoft.aspnetcore.http.headerdictionaryextensions.append) 方法存在于 [Microsoft.AspNetCore.Http](https://www.nuget.org/packages/Microsoft.AspNetCore.Http/) 包中。

```c#
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx => {
        // 需要添加引用：using Microsoft.AspNetCore.Http;
        ctx.Context.Response.Headers.Append("Cache-Control", "public, max-age=600");
    }
});
```

效果如下：

![](F:\03Github\06DotnetCoreDemo\3.1\静态文件\Images\05添加缓存.png)



# 启用目录浏览

通过目录浏览，Web应用的用户可查看目录列表和指定目录中的文件。

> 出于安全考虑，目录浏览默认处于禁用状态

在StartUp.Configure 中的 `UseDirectoryBrowser` 方法来启用目录浏览。

```c#
app.UseDirectoryBrowser(new DirectoryBrowserOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "MyStaticFiles", "images")),
    RequestPath = "/Images"
});
```

效果如下

![](F:\03Github\06DotnetCoreDemo\3.1\静态文件\Images\06目录浏览.png)



# 参考

[ASP.NET Core 中的静态文件](https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/static-files?view=aspnetcore-3.1)

[ASP.NET Core 高性能系列》静态文件中间件](https://www.cnblogs.com/humble/p/12288903.html)