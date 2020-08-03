using LibOwin;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ShoppingCart.Infrastructure
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class CorrelationToken
    {
        public static AppFunc Middleware(AppFunc next)
        {
            return async env =>
            {
                OwinContext owinContext = new OwinContext(env);
                if (!(owinContext.Request.Headers["Correlation-Token"] != null
                    &&
                    Guid.TryParse(owinContext.Request.Headers["Correlation-Token"], out Guid correlationToken)))
                {
                    correlationToken = Guid.NewGuid();
                }
                owinContext.Set("correlationToken", correlationToken.ToString());
                using (LogContext.PushProperty("CorrelationToken", correlationToken))
                {
                    await next(env);
                }
            };
        }
    }

    public class RequestLogging
    {
        public static AppFunc Middleware(AppFunc next, ILogger log)
        {
            return async env =>
            {
                OwinContext owinContext = new OwinContext(env);
                log.Information(
                    "Incoming request:{@Method},{@Path},{@Headers}",
                    owinContext.Request.Method,
                    owinContext.Request.Path,
                    owinContext.Request.Headers);
                await next(env);
                log.Information(
                    "Outgoing response: {@StatusCode}, {@Headers}",
                    owinContext.Response.StatusCode,
                    owinContext.Response.Headers
                    );
            };
        }
    }

    public class PerformanceLogging
    {
        public static AppFunc Middleware(AppFunc next, ILogger log)
        {
            return async env =>
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                await next(env);
                stopWatch.Stop();
                OwinContext owinContext = new OwinContext();
                log.Information(
                    "Request: {@Method} {@Path} executed in {RequestTime:000} ms",
                    owinContext.Request.Method, owinContext.Request.Path,
                    stopWatch.ElapsedMilliseconds);
            };
        }
    }
}
