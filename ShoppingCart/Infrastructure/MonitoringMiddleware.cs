using LibOwin;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ShoppingCart.Infrastructure
{
    using AppFunc = Func<IDictionary<string, object>, Task>;
    public class MonitoringMiddleware
    {
        private readonly AppFunc next;

        private readonly Func<Task<bool>> healthCheck;

        private static readonly PathString monitorPath = new PathString("/_monitor");

        private static readonly PathString monitorShallowPath = new PathString("/_monitor/shallow");

        private static readonly PathString monitorDeepPath = new PathString("/_monitor/deep");

        public MonitoringMiddleware(AppFunc next, Func<Task<bool>> healthCheck)
        {
            this.next = next;
            this.healthCheck = healthCheck;
        }

        public Task Invoke(IDictionary<string, object> env)
        {
            OwinContext context = new OwinContext(env);
            if (context.Request.Path.StartsWithSegments(monitorPath))
            {
                return HandleMonitorEndpoint(context);
            }
            else
            {
                return next(env);
            }
        }

        private Task HandleMonitorEndpoint(OwinContext context)
        {
            if (context.Request.Path.StartsWithSegments(monitorShallowPath))
            {
                return ShallowEndpoint(context);
            }
            else if (context.Request.Path.StartsWithSegments(monitorDeepPath))
            {
                return DeepEndpoint(context);
            }
            return Task.FromResult(0);
        }

        private async Task DeepEndpoint(OwinContext context)
        {
            if (await this.healthCheck())
            {
                context.Response.StatusCode = (int)HttpStatusCode.NoContent;
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
            }
        }

        private Task ShallowEndpoint(OwinContext context)
        {
            context.Response.StatusCode = 204;
            return Task.FromResult(0);
        }


    }
}
