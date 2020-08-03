
using LibOwin;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Owin;
using Nancy.TinyIoc;
using Serilog;
using System.Security.Claims;

namespace ShoppingCart
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private readonly ILogger log;

        public Bootstrapper(ILogger log)
        {
            this.log = log;
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            container.Register(this.log);
            //container.Register<IHttpClientFactory>(new HttpClientFactory());
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);
            context.CurrentUser = context.GetOwinEnvironment()["pos-end-user"] as ClaimsPrincipal;
            var correlationToken = context.GetOwinEnvironment()["correlationToken"] as string;
            var pricipal = context.GetOwinEnvironment()[OwinConstants.RequestUser] as ClaimsPrincipal;
            var idToken = pricipal
                .FindFirst("id_token");
            container.Register<IHttpClientFactory>(new HttpClientFactory(idToken.ToString(),correlationToken));
        }
    }
}
