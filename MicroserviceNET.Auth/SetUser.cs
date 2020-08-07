using Nancy;
using Nancy.Bootstrapper;
using Nancy.Owin;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace MicroserviceNET.Auth
{
    public class SetUser : IRequestStartup
    {
        public void Initialize(IPipelines pipelines, NancyContext context) => 
            context.CurrentUser = context.GetOwinEnvironment()["microservice.NET-end-user"] as ClaimsPrincipal;
    }
}
