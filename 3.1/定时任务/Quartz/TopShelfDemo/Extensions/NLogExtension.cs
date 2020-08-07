using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace TopShelfDemo.Extensions
{
    public static class NLogExtension
    {
        public static IServiceCollection AddNLog(this IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddNLog("nlog.config");
            });

            return services;
        }
    }
}
