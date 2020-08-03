using Dapper;
using global::ShoppingCart.Infrastructure;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using Nancy.Owin;
using Serilog;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShoppingCart
{
    public interface IHttpClientFactory
    {
        Task<HttpClient> CreateAsync(Uri uri, string scope);
    }
    public class HttpClientFactory : IHttpClientFactory
    {
        private readonly string correlationToken;
        private readonly TokenClient tokenClient;
        private readonly string idToken;

        public HttpClientFactory(string token,string idToken)
        {
            correlationToken = token;
            this.idToken = idToken;
        }
        public async Task<HttpClient> CreateAsync(Uri uri,string scope)
        {
            var response = await tokenClient.RequestClientCredentialsAsync(scope).ConfigureAwait(false);
            HttpClient client = new HttpClient() { BaseAddress = uri };
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", response.AccessToken);
            client.DefaultRequestHeaders.Add("Correlation-Token", correlationToken);
            client.DefaultRequestHeaders.Add("pos-end-user", idToken);
            return client;
        }

    }
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
            ILogger log = ConfigureLogger();
            app.UseOwin(buildFunc =>
            {
                buildFunc(next => CorrelationToken.Middleware(next));
                buildFunc(next => RequestLogging.Middleware(next, log));
                buildFunc(next => PerformanceLogging.Middleware(next, log));
                buildFunc(next => new MonitoringMiddleware(next, HealthCheck).Invoke);
                //buildFunc.UseNancy();
                //creates bootstrapper and gives it to Nancy
                buildFunc.UseNancy(opt => opt.Bootstrapper = new Bootstrapper(log));
            });
            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
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
