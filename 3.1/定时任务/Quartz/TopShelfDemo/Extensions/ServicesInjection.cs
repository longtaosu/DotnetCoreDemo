using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TopShelfDemo.Services;

namespace TopShelfDemo.Extensions
{
    public static class ServicesInjection
    {
        public static IServiceCollection AddLocalServices(this IServiceCollection services)
        {
            services.AddScoped<ITestService, TestService>();

            return services;
        }
    }
}
