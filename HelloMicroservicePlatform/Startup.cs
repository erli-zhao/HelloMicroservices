using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroserviceNET.Auth;
using MicroserviceNET.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using Nancy.Owin;
using Serilog;
using Dapper;

namespace HelloMicroservicePlatform
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseOwin()
                .UseMonitoringAndLogging(ConfigureLogger(), HealthCheck)
                .UseAuthPlatform("test-scope")
                .UseNancy();
        }

        private readonly string connectionString = @"server=192.168.18.171;port=3306;uid=user;pwd=!wszlj2390;database=microservice;";
        private readonly int threshold = 100;
        public async Task<bool> HealthCheck()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                int count = (await conn.QueryAsync<int>("select count(id) from shoppingcart")).Single();

                return count > threshold;
            }
        }

        private ILogger ConfigureLogger()
        {
            return new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.ColoredConsole(Serilog.Events.LogEventLevel.Verbose,
                "{NewLine}{Timestamp:HH:mm:ss}[{level}]({CorrelationToken}){Message}{NewLine}{Exception}")
                .CreateLogger();
        }
    }
}
