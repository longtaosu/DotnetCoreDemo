using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jwt_RoleBased.Extensions;
using Jwt_RoleBased.Propertities;
using Jwt_RoleBased.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jwt_RoleBased
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
            //Configuration.GetSection("AppSettings").Get<AppSettings>();
            //services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddOptions().Configure<AppSettings>(x => Configuration.Bind(x));

            services.AddSingleton<TokenService>();

            services.AddSwaggerExt();

            services.AddJwtExt(Configuration);

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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwaggerExt();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
