using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LibOwin;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

namespace MicroserviceNET.Logging
{
    public class MonitoringMiddleware
    {
        private AppFunc next;
        private Func<Task<bool>> healthCheck;

        private static readonly PathString monitorPath = new PathString("/_monitor");

        private static readonly PathString monitorShallowPath = new PathString("/_monitor/shallow");

        private static readonly PathString monitorDeepPath = new PathString("/_monitor/deep");

        public MonitoringMiddleware(AppFunc next,Func<Task<bool>> healthCheck)
        {
            this.healthCheck = healthCheck;
            this.next = next;
        }

        public Task Invoke(IDictionary<string,object> env)
        {
            var context = new OwinContext(env);
            if (context.Request.Path.StartsWithSegments(monitorPath))
            {
                return HandleMonitorEndPoint(context);
            }
            else
            {
                return next(env);
            }
        }

        private Task HandleMonitorEndPoint(OwinContext context)
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
            if (await healthCheck())
            {
                context.Response.StatusCode = 204;
            }

            else
            {
                context.Response.StatusCode = 503;
            }
        }

        private Task ShallowEndpoint(OwinContext context)
        {
            context.Response.StatusCode = 204;
            return Task.FromResult(0);
        }
    }
}
