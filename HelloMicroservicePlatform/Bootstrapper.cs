using MicroserviceNET.Platform;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloMicroservicePlatform
{
    public class Bootstrapper:DefaultNancyBootstrapper
    {
        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);
            container.UseHttpClientFactory(context);
        }
    }
}
