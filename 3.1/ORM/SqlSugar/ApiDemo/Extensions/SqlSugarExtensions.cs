using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDemo.Extensions
{
    public static class SqlSugarExtensions
    {

        public static IServiceCollection AddSqlSugar(this IServiceCollection services,IConfiguration configuration)
        {
            return services.AddScoped<ISqlSugarClient>((provider) =>
            {
                return new SqlSugarClient(new ConnectionConfig()
                {
                    ConnectionString = configuration.GetValue<string>("SqlSugar:ConnectionString"),
                    DbType = DbType.MySql
                });
            });
        }
    }
}
