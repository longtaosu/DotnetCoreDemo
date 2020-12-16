using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ElasticModule
{
public static class ElasticExtension
{
    public static IServiceCollection AddElasticService(this IServiceCollection services,IConfiguration configuration)
    {
        //services.AddOptions().Configure<ElasticSetting>(x => configuration.Bind(x));

        services.Configure<ElasticSetting>(configuration.GetSection("ES"));
        services.AddScoped<IESClientProvider, ESClientProvider>();

        return services;
    }
}
}
