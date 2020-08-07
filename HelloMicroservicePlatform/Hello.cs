using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroserviceNET.Platform;
using Nancy;

namespace HelloMicroservicePlatform
{
    public class Hello:NancyModule
    {
        public Hello(IHttpClientFactory clientFactory)
        {
            Get("/", async (_,__) =>
             {
                 var client = await clientFactory.Create(new Uri("http://otherservice"), "scope_for_other_microservice");
                 var resp = await client.GetAsync("some/path").ConfigureAwait(false);
                 return resp.StatusCode;
             });
        }
    }
}
