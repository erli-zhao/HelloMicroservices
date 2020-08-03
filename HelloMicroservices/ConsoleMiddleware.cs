using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibOwin;

namespace HelloMicroservices
{

    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class ConsoleMiddleware
    { 
        private AppFunc next;

        public ConsoleMiddleware(AppFunc next)
        {
            this.next = next;
        }

        public Task Invoke(IDictionary<string,object> env)
        {
            var context = new OwinContext(env);
            var method = context.Request.Method;
            var path = context.Request.Path;
            System.Console.WriteLine($"Got request: {method} {path}");
            return next(env);
        }
    }
}
