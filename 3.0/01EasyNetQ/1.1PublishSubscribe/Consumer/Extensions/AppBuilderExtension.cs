using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Consumer.Extensions
{
    public static class AppBuilderExtension
    {
        public static IApplicationBuilder UseSubscribe(this IApplicationBuilder appBuilder,string subscribetionPrefix,Assembly assembly)
        {
            var services = appBuilder.ApplicationServices.CreateScope().ServiceProvider;

            var lifeTime = services.GetService<IHostApplicationLifetime>();
            var Bus = services.GetService<IBus>();
            lifeTime.ApplicationStarted.Register(() =>
            {
                var subscriber = new AutoSubscriber(Bus, subscribetionPrefix);
                subscriber.Subscribe(assembly);
                subscriber.SubscribeAsync(assembly);
            });

            lifeTime.ApplicationStopped.Register(() => Bus.Dispose());

            return appBuilder;
        }
    }
}
