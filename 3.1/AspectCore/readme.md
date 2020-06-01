# AspectCore

## 安装依赖

```shell
Install-Package AspectCore.Extensions.DependencyInjection
```

## 定义特性类

使用抽象的 `AbstractInterceptorAttribute` 自定义特性类（该类实现了 `IInterceptor` 接口）。AspectCore默认实现了基于 `Attribute` 的拦截器配置，自定义的拦截器如下：

```c#
public class CustomInterceptorAttribute : AbstractInterceptorAttribute 
{
    public async override Task Invoke(AspectContext context, AspectDelegate next)
    {
        try
        {
            Console.WriteLine("Before service call");
            await next(context);
        }
        catch (Exception)
        {
            Console.WriteLine("Service threw an exception!");
            throw;
        }
        finally
        {
            Console.WriteLine("After service call");
        }
    }
}
```

## 定义服务

自定义的特性类可以放在服务接口上

```c#
public interface ICustomService
{
    [CustomInterceptor]
    void Call();
}

public class CustomService : ICustomService
{
    public void Call()
    {
        Console.WriteLine("service calling...");
    }
}
```

## 控制器

```c#
public class HomeController : Controller
{
    private readonly ICustomService _service;
    public HomeController(ICustomService service)
    {
        _service = service;
    }

    public IActionResult Index()
    {
        _service.Call();
        return View();
    }
}
```

因为控制器层需要调用我们的服务，所以需要在程序启动时添加DI：

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddScoped<ICustomService, CustomService>();
    services.ConfigureDynamicProxy();
    
    services.AddControllers();
}
```

## Program.cs

```c#
public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            // 略
            .UseServiceProviderFactory(new DynamicProxyServiceProviderFactory());
```



# 拦截器配置

## 全局拦截器

使用 `ConfigureDynamicProxy(Action<IAspectConfiguration>)` 的重载方法，其中 `IAspectConfiguration` 提供 `Interceptors` 注册全局拦截器。

```c#
//全局拦截器
services.ConfigureDynamicProxy(config =>
{
    config.Interceptors.AddTyped<CustomInterceptorAttribute>();
});
```



## 带参数的全局拦截器

在 `CustomInterceptorAttribute` 中添加带参数的构造器

```c#
    public class ParamsInterceptorAttribute : AbstractInterceptorAttribute
    {
        private string _name;
        public ParamsInterceptorAttribute(string name)
        {
            this._name = name;
        }

        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            try
            {
                Console.WriteLine("Before service call");
                await next(context);
            }
            catch (Exception)
            {
                Console.WriteLine("Service threw an exception!");
                throw;
            }
            finally
            {
                Console.WriteLine("After service call");
            }
        }
    }
```

在Startup中注册

```c#
services.ConfigureDynamicProxy(config =>
{
    config.Interceptors.AddTyped<ParamsInterceptorAttribute>(args: new object[] { "custom" });
});
```



## 作为服务的全局拦截器

```c#
services.AddTransient<ParamsInterceptorAttribute>(provider => new ParamsInterceptorAttribute("custom"));
services.ConfigureDynamicProxy(config =>
{
    config.Interceptors.AddServiced<ParamsInterceptorAttribute>();
});
```



## 作用域特定Service或Method的全局拦截器

```c#
//作用于特定Service或Method的全局拦截器，本例为作用于带有 Service 后缀的类的全局拦截器
services.ConfigureDynamicProxy(config =>
{
    //根据 Method 做全局拦截(Controller中的 Method)
    config.Interceptors.AddTyped<CustomInterceptorAttribute>(method => method.Name.EndsWith("Params"));
    
    //根据 Service 做全局拦截（是用了通配符）
    //config.Interceptors.AddTyped<CustomInterceptorAttribute>(Predicates.ForService("*Service"));
});
```





## NonAspectAttribute

在 AspectCore中提供了 NonAspectAttribute 来使得Service或Method不被代理

```c#
[NonAspect]
public interface ICustomService
{
    void Call();
}
```

同时支持全局忽略配置，亦支持通配符

```c#
services.ConfigureDynamicProxy(config =>
{
    //App1命名空间下的Service不会被代理
    config.NonAspectPredicates.AddNamespace("App1");

    //最后一级为App1的命名空间下的Service不会被代理
    config.NonAspectPredicates.AddNamespace("*.App1");

    //ICustomService接口不会被代理
    config.NonAspectPredicates.AddService("ICustomService");

    //后缀为Service的接口和类不会被代理
    config.NonAspectPredicates.AddService("*Service");

    //命名为Query的方法不会被代理
    config.NonAspectPredicates.AddMethod("Query");

    //后缀为Query的方法不会被代理
    config.NonAspectPredicates.AddMethod("*Query");
}); 
```



## 拦截器的依赖注入

在拦截器中支持属性注入，构造器注入和服务定位器模式。

属性注入，在拦截器中拥有`public get and set`权限的属性标记`[AspectCore.DependencyInjection.FromServiceContextAttribute]`特性，即可自动注入该属性，如：

```c#
public class CustomInterceptorAttribute : AbstractInterceptorAttribute 
{
    //ps : 只有使用 config.Interceptors.AddTyped<CustomInterceptorAttribute>(); 时，属性注入才生效， 
    //     不能使用以下这种方式 services.AddSingleton<CustomInterceptorAttribute>(); + services.ConfigureDynamicProxy(config => { config.Interceptors.AddServiced<CustomInterceptorAttribute>(); });
    [FromServiceContext]
    public ILogger<CustomInterceptorAttribute> Logger { get; set; }


    public override Task Invoke(AspectContext context, AspectDelegate next)
    {
        Logger.LogInformation("call interceptor");
        return next(context);
    }
}
```

构造器注入需要使拦截器作为`Service`，除全局拦截器外，仍可使用`ServiceInterceptor`使拦截器从DI中激活：

```c#
public class CustomInterceptorAttribute : AbstractInterceptorAttribute 
{
    private readonly ILogger<CustomInterceptor> ctorlogger;

    // ps : 当全局配置 config.Interceptors.AddTyped<CustomInterceptorAttribute>(); 时，构造器注入无法自动注入，需要手动处理
    //      只有使用 services.AddSingleton<CustomInterceptorAttribute>(); + services.ConfigureDynamicProxy(config => { config.Interceptors.AddServiced<CustomInterceptorAttribute>(); }); 才会自动注入
    public CustomInterceptor(ILogger<CustomInterceptor> ctorlogger)
    {
        this.ctorlogger = ctorlogger;
    }
}
```

服务定位器模式。拦截器上下文`AspectContext`可以获取当前Scoped的`ServiceProvider`：

```c#
public class CustomInterceptorAttribute : AbstractInterceptorAttribute 
{
    public override Task Invoke(AspectContext context, AspectDelegate next)
    {
        var logger = context.ServiceProvider.GetService<ILogger<CustomInterceptorAttribute>>();
        logger.LogInformation("call interceptor");
        return next(context);
    }
}
```



# 参考

<https://github.com/dotnetcore/AspectCore-Framework/blob/master/sample/AspectCore.Extensions.AspectScope.Sample/Program.cs>

[https://github.com/dotnetcore/AspectCore-Framework/blob/master/docs/1.%E4%BD%BF%E7%94%A8%E6%8C%87%E5%8D%97.md](https://github.com/dotnetcore/AspectCore-Framework/blob/master/docs/1.使用指南.md)