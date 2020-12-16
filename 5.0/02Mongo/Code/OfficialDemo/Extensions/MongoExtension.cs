using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OfficialDemo
{
    public static class MongoExtension
    {
        public static IServiceCollection AddMongoService(this IServiceCollection services,IConfiguration configuration)
        {
            var mongoCoon = configuration.GetValue<string>("MongoDb:ConnectionString");
            var mongoDatabase = configuration.GetValue<string>("MongoDb:DbName");
            services.AddSingleton<IMongoDatabase>(m => new MongoClient(mongoCoon).GetDatabase(mongoDatabase));

            return services;
        }
    }
}
