using LibOwin;
using Microsoft.IdentityModel.Tokens; 
using System.IdentityModel.Tokens.Jwt;
using AppFunc =  System.Func< System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

namespace MicroserviceNET.Auth
{
   public  class IdToken
    {
        public static AppFunc Middleware(AppFunc next)
        {
            return env =>
            {
                var ctx = new OwinContext(env);
                if (ctx.Request.Headers.ContainsKey("microservice.Net-end-user"))
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    SecurityToken token;
                    var userPrincipal = tokenHandler.ValidateToken(
                        ctx.Request.Headers["microservice.Net-end-user"], new TokenValidationParameters(), out token);
                    ctx.Set("microservice.Net-end-user",userPrincipal);
                }
                return next(env);
            };
        }
    }
}
