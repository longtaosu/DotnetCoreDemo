using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDemo.Interceptors;
using ApiDemo.Services;
using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ApiDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ICustomService, CustomService>();

            //默认动态代理
            //services.ConfigureDynamicProxy();

            //全局拦截器
            //services.ConfigureDynamicProxy(config =>
            //{
            //    config.Interceptors.AddTyped<CustomInterceptorAttribute>();
            //});

            //带参数的全局拦截器
            //services.ConfigureDynamicProxy(config =>
            //{
            //    config.Interceptors.AddTyped<ParamsInterceptorAttribute>(args: new object[] { "custom" });
            //});

            //作为服务的全局拦截器
            //services.AddTransient<ParamsInterceptorAttribute>(provider => new ParamsInterceptorAttribute("custom"));
            //services.ConfigureDynamicProxy(config =>
            //{
            //    config.Interceptors.AddServiced<ParamsInterceptorAttribute>();
            //});

            //作用于特定Service或Method的全局拦截器，本例为作用于带有 Service 后缀的类的全局拦截器
            services.ConfigureDynamicProxy(config =>
            {
                //根据 Service 做全局拦截
                config.Interceptors.AddTyped<CustomInterceptorAttribute>(method => method.Name.EndsWith("Params"));
                //根据 Service 做全局拦截
                //config.Interceptors.AddTyped<CustomInterceptorAttribute>(Predicates.ForService("*Service"));
            });



            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
