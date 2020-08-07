using System;
using System.Collections.Generic;
using System.Text;
using Nancy;
using System.Security.Claims;
using Nancy.Owin;
using Nancy.TinyIoc;
using LibOwin;

namespace MicroserviceNET.Platform
{
   public static class MicroservicePlatformHelper
    {
        private static string _tokenUrl;
        private static string _clientName;
        private static string _clientSecret;

        public static void Configure(string tokenUrl,string clientName,string clientSecret)
        {
            _tokenUrl = tokenUrl;
            _clientName = clientName;
            _clientSecret = clientSecret;
        }

        public static TinyIoCContainer UseHttpClientFactory(this TinyIoCContainer self,NancyContext context)
        {
            var correlationToken = context.GetOwinEnvironment()?["correlationToken"] as string;
            object key = null;
            context.GetOwinEnvironment()?.TryGetValue(OwinConstants.RequestUser, out key);
            var principal = key as ClaimsPrincipal;
            var idToken = principal?.FindFirst("id_token");
            self.Register<IHttpClientFactory>(
                new HttpClientFactory(
                    _tokenUrl, _clientName, _clientSecret, correlationToken ?? "", idToken?.Value
                    )) ;
            return self;
        }
    }
}
