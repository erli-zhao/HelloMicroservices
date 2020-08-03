using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Login
{
    public class Startup
    {
        private readonly IHostingEnvironment environment;
        public Startup(IHostingEnvironment env)
        {
            environment = env;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var cert = new X509Certificate2(
                Path.Combine(environment.ContentRootPath, "idsrv3test.pfx"), "idsrv3test");
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            var builder=services.AddIdentityServer().AddSigningCredential(cert);

            builder.AddInMemoryClients(Clients.Get());
            builder.AddInMemoryIdentityResources(Resources.Get());
            builder.AddTestUsers(Users.Get());

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Trace);
            loggerFactory.AddDebug(LogLevel.Trace);
             
            app.UseIdentityServer();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();

        }
    }
}
