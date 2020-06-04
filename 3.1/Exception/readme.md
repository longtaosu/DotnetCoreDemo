# 在Razor页处理异常

所有的.Net应用都会产生异常并抛出异常，所以如何在中间件中处理异常就很重要。

举例来说，如果创建了一个基于 *Razor Pages* 的 *Web* 应用，则会在 `Startup.Configure `中看到下面的中间件。

```c#
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Error");
    }

    // .. other middleware not shown
}
```

如果运行在 `Development` 环境下，当处理请求时应用会捕获异常，通过UseDeveloperExceptionPage()方法将异常显示到web页上。

![ex_handler_01.png](https://gitee.com/imstrive/ImageBed/raw/master/20200604/ex_handler_01.png)

这在本地的开发环境很重要，可以快速的检查请求信息、路由信息等。

但是这些敏感信息，我们并不希望在生产环境中暴露。所以如果环境不再是 `Development`，我们使用异常处理中间件：`UseExceptionHandler`。这个中间件默认设定一个请求路径 `"/Error"` 

![ex_handler_02.svg](https://gitee.com/imstrive/ImageBed/raw/master/20200604/ex_handler_02.svg)

应用程序最后的结果是，在 *Production* 环境中发生异常后会返回 *Error.cshtml* Razor Page。

![ex_handler_03.png](https://gitee.com/imstrive/ImageBed/raw/master/20200604/ex_handler_03.png)


# 在WebApi中处理异常

在WebApi程序中发生异常和Razor Pages的异常处理很像

```c#
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    // .. other middleware not shown
}
```

上面的代码中，`UseDeveloperExceptionPage` 仍然添加到了 `Development` 环境中，但是在 `Production` 环境中发生的异常没有异常处理！听起来似乎没那么糟糕：虽然没有异常处理的中间件，Asp.net Core框架会捕获异常，做log并向客户端返回500。

![ex_handler_04.png](https://gitee.com/imstrive/ImageBed/raw/master/20200604/ex_handler_04.png)

如果使用的是 `[ApiController]` 属性，异常则来自 Web Api 控制器，默认会得到一个 `ProblemDetails` 结果，或者可以进行自定义。

举例来说，可能你希望使用标准的格式处理错误，比方说 *ProblemDetails* 格式。如果希望所有的错误都使用这个格式，那么在某些情况下产生的empty response则会导致客户端崩溃。同样的，在`Development`环境下，返回一个HTML异常页，而不是期望的Json信息也会导致问题。

## 异常处理

首先定义一个`UseCustomErrors`静态方法，在development环境下，最终调用 `WriteResponse` 方法并设置 `includeDetails: true`，在其他环境下，该值则设置为false。

```c#
public static class CustomErrorHandler
{
    public static void UseCustomErrors(this IApplicationBuilder app, IWebHostEnvironment environment)
    {
        if (environment.EnvironmentName=="Development")
        {
            app.Use(WriteDevelopmentResponse);
        }
        else
        {
            app.Use(WriteProductionResponse);
        }
    }

    private static Task WriteDevelopmentResponse(HttpContext httpContext, Func<Task> next)
        => WriteResponse(httpContext, includeDetails: true);

    private static Task WriteProductionResponse(HttpContext httpContext, Func<Task> next)
        => WriteResponse(httpContext, includeDetails: false);

    private static async Task WriteResponse(HttpContext httpContext, bool includeDetails)
    {
        // .. to implement
    }
}
```

上面的方法 `WriteResponse` 用于输出响应，该方法接收来自 `ExceptionHandlerMiddleware` 的异常，然后构建 `ProblemDetails` 对象。然后使用 `System.Text.Json` 方法将结果进行序列化。

```c#
private static async Task WriteResponse(HttpContext httpContext, bool includeDetails)
{
    // Try and retrieve the error from the ExceptionHandler middleware
    var exceptionDetails = httpContext.Features.Get<IExceptionHandlerFeature>();
    var ex = exceptionDetails?.Error;

    // Should always exist, but best to be safe!
    if (ex != null)
    {
        // ProblemDetails has it's own content type
        httpContext.Response.ContentType = "application/problem+json";

        // Get the details to display, depending on whether we want to expose the raw exception
        var title = includeDetails ? "An error occured: " + ex.Message : "An error occured";
        var details = includeDetails ? ex.ToString() : null;

        var problem = new ProblemDetails
        {
            Status = 500,
            Title = title,
            Detail = details
        };

        // This is often very handy information for tracing the specific request
        var traceId = Activity.Current?.Id ?? httpContext?.TraceIdentifier;
        if (traceId != null)
        {
            problem.Extensions["traceId"] = traceId;
        }

        //Serialize the problem details object to the Response as JSON (using System.Text.Json)
        var stream = httpContext.Response.Body;
        await JsonSerializer.SerializeAsync(stream, problem);
    }
}
```

可以在 `ProblemDetails` 对象上记录任何的信息，在序列化前可以从 `HttpContext` 得到。

现在如果程序在`Development`环境发生异常，可以得到下面的 `json` 结果。

![ex_handler_05.png](https://gitee.com/imstrive/ImageBed/raw/master/20200604/ex_handler_05.png)

但是在生产环境，则会得到 `ProblemDetails` 响应，但是相信信息会被省略。

![ex_handler_06.png](https://gitee.com/imstrive/ImageBed/raw/master/20200604/ex_handler_06.png)

## Startup配置

```c#
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseExceptionHandler(err => err.UserCustomError(env));

    //其他配置
}
```





# 翻译

<https://andrewlock.net/creating-a-custom-error-handler-middleware-function/>