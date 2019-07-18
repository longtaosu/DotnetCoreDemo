# 参考

https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters?view=aspnetcore-2.2

https://docs.microsoft.com/zh-cn/aspnet/core/mvc/controllers/filters?view=aspnetcore-2.2



ASP.NET Core中的过滤器允许代码在请求处理过程中，先于或后于某个阶段执行。

内置的过滤器可以处理以下的任务：

- 授权：防止用户在未授权的情况下访问资源
- 响应缓存：短接请求的管道，返回缓存的结果

自定义的过滤器可以解决切面问题。切面问题常见的例子包括：异常处理、缓存、配置、授权、日志等。过滤器可以避免重复的代码，比方说一个异常处理的过滤器可以增强异常的解决。

示例程序：

[View or download sample](https://github.com/aspnet/AspNetCore.Docs/tree/master/aspnetcore/mvc/controllers/filters/sample) ([how to download](https://docs.microsoft.com/en-us/aspnet/core/index?view=aspnetcore-2.2#how-to-download-a-sample)).



# 过滤器如何工作

ASP.NET Core方法内部的过滤器调用管道，有时是过滤器管道，过滤器管道在ASP.NET Core选定执行执行方法后运行。

![The request is processed through Other Middleware, Routing Middleware, Action Selection, and the ASP.NET Core Action Invocation Pipeline. The request processing continues back through Action Selection, Routing Middleware, and various Other Middleware before becoming a response sent to the client.](https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters/_static/filter-pipeline-1.png?view=aspnetcore-2.2)

# 过滤器类型

每个过滤器在过滤器管道的不同阶段执行。

- **Authorization filters**

最先运行，用于确定发起请求的用户是否授权。如果未授权，该过滤器会短路请求的管道。

- **Resource filter**

1. Authorization之后运行；
2. OnResourceExecuting 的代码在其他过滤器管道前运行。例如，OnResourceExecuting可以在模型绑定前运行；
3. OnResourceExecuted 的代码在其他管道结束后执行

- **Action filters**

1. 代码可以在单独的Action方法调用前或后立即运行。可以用于控制方法的参数和方法的返回结果。
2. Action过滤器不支持Razor视图

- **Exception filters**

1. 用于全局的处理异常的策略，可以在返回信息之前执行。
2. Result filters在单独的action执行结果前或后执行，只有当方法运行成功后才会执行，常用于视图或者执行结果的格式化

下图显示过滤器的类型以及在过滤器管道的执行顺序

![The request is processed through Authorization Filters, Resource Filters, Model Binding, Action Filters, Action Execution and Action Result Conversion, Exception Filters, Result Filters, and Result Execution. On the way out, the request is only processed by Result Filters and Resource Filters before becoming a response sent to the client.](https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters/_static/filter-pipeline-2.png?view=aspnetcore-2.2)











