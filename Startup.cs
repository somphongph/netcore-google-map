using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace netcore_google_map
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
         readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            var allowedOrigins = Configuration.GetSection("CorsSettings:AllowOrigins").Get<string[]>();
            var allowedHeaders = Configuration.GetSection("CorsSettings:AllowHeaders").Get<string[]>();
            var allowedMethods = Configuration.GetSection("CorsSettings:AllowMethods").Get<string[]>();
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder
                            .WithOrigins(allowedOrigins)
                            .WithHeaders(allowedHeaders)
                            .WithMethods(allowedMethods)
                            .AllowCredentials();
                    });
            });

            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = Configuration["Redis:ConnectionString"];
                options.InstanceName = Configuration["Redis:InstanceName"];                
            });

            // IConnectionMultiplexer redis = ConnectionMultiplexer.Connect("10.0.75.1");
            // services.AddScoped(s => redis.GetDatabase());

            // services.AddStackExchangeRedisCache(options =>
            // {
            //     options.Configuration = "localhost";
            //     options.InstanceName = "SampleInstance";
            // });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(MyAllowSpecificOrigins);

            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}