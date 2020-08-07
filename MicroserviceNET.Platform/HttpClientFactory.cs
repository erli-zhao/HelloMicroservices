using IdentityModel.Client;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MicroserviceNET.Platform
{

    public interface IHttpClientFactory
    {
         Task<HttpClient> Create(Uri uri, string requestScope);
    }

    public class HttpClientFactory : IHttpClientFactory
    {
        private readonly TokenClient tokenClient;
        private readonly string correlationToken;
        private readonly string idToken;

        public HttpClientFactory(string tokenUrl,string clientName,string clientSecret,string correlationToken,string idToken)
        { 
            this.tokenClient =new TokenClient(tokenUrl,clientName,clientSecret);
            this.correlationToken = correlationToken;
            this.idToken = idToken;
        } 

        public async Task<HttpClient> Create(Uri uri, string requestScope)
        {
            var response = await tokenClient.RequestClientCredentialsAsync(requestScope);
            var client = new HttpClient() { BaseAddress = uri };
            client.DefaultRequestHeaders.Authorization = new  AuthenticationHeaderValue("Bearer",
                response.AccessToken);
            client.DefaultRequestHeaders.Add("Correlation-Token", correlationToken);
            if (!string.IsNullOrEmpty(idToken))
            {
                client.DefaultRequestHeaders.Add("microservice.NET-end-user", idToken);
            }
            return client;
        }
    }
}
