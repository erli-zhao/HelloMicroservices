using System.Threading.Tasks;
using LibOwin;
using AppFunc =System.Func<System.Collections.Generic.IDictionary<string, object>,System.Threading.Tasks.Task>;

namespace MicroserviceNET.Auth
{
    public class Authorization
    {
        public static  AppFunc Middleware(AppFunc next,string requiredScope)
        {
            return env =>
            {
                var ctx = new OwinContext(env);
                var principal = ctx.Request.User;
                if (principal.HasClaim("scope", requiredScope))
                    return next(env);
                ctx.Response.StatusCode = 403;
                return Task.FromResult(0);
            };
        }
    }
}
