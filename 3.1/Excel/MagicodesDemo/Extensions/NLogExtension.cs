using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog.Extensions.Logging;

namespace MagicodesDemo.Extensions
{
    public static class NLogExtension
    {
        public static IServiceCollection AddNLog(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddLogging(log =>
            {
                log.ClearProviders();
                log.SetMinimumLevel(LogLevel.Information);
                log.AddNLog(configuration);
            });
        }
    }
}
