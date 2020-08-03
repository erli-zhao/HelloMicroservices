using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using LibOwin;
using Nancy.Owin;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace LoyaltyProgram
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication().AddCookie();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap=new Dictionary<string,string>();

            

            //var oidcOptions = new OpenIdConnectOptions
            //{ 
            //    SignInScheme = "Cookies",
            //    Authority = "http://localhost:5001",
            //    RequireHttpsMetadata = false,
            //    ClientId = "web",
            //    ResponseType = "id_token token",
            //    GetClaimsFromUserInfoEndpoint = true,
            //    SaveTokens = true
            //};

            //oidcOptions.Scope.Clear();
            //oidcOptions.Scope.Add("openid");
            //oidcOptions.Scope.Add("profile");
            //oidcOptions.Scope.Add("api1");

            app.UseOwin(buildFunc => {
                buildFunc(next => en =>
                {
                    var ctx = new OwinContext(en);
                    if (ctx.Request.Headers.ContainsKey("pos-end-user"))
                    {
                        var tokenHandler = new JwtSecurityTokenHandler();
                        SecurityToken token;
                        var userPricipal = tokenHandler.ValidateToken(
                            ctx.Request.Headers["pos-end-user"],
                            new TokenValidationParameters(),
                            out token);
                        ctx.Set("pos-end-user", userPricipal);
                    }
                    var pricipal = ctx.Request.User;
                    if (pricipal.HasClaim("scope","loyalty_program_write"))
                    {
                        return next(en);
                    }
                    ctx.Response.StatusCode = 403;
                    return Task.FromResult(0);
                });
                buildFunc.UseNancy();
            });
        }
    }
}
