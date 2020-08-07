using Chloe;
using Chloe.Infrastructure;
using Chloe.MySql;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace TopShelfDemo.Extensions
{
    public static class ChloeExtension
    {
        public static IServiceCollection AddChloe(this IServiceCollection services, DBOption options)
        {
            services.AddScoped<IDbContext>((provider) =>
            {
                return new MySqlContext(new MySqlConnectionFactory(options.Coon));
            });

            return services;
        }
    }

    public class MySqlConnectionFactory : IDbConnectionFactory
    {
        string _strCoon = string.Empty;

        public MySqlConnectionFactory(string strCoon)
        {
            this._strCoon = strCoon;
        }

        public IDbConnection CreateConnection()
        {
            IDbConnection conn = new MySqlConnection(this._strCoon);
            return conn;
        }
    }
}
