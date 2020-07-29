using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;
using Polly.CircuitBreaker;

namespace APIGateway
{
    public class LoyaltyProgramClient
    {
        private  string hostName;

        //Retry Strategy
        private static AsyncRetryPolicy exponentialRetryPolicy =
            Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                3,
                attempt =>
                TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt))
                );

        // Circuit Breaker Strategy
        private static AsyncCircuitBreakerPolicy circuitBreaker =
            Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(5, TimeSpan.FromMinutes(3));

        public async Task<HttpResponseMessage> RegisterUser(LoyaltyProgramUser newUser)
        {
            return await exponentialRetryPolicy.ExecuteAsync(() => DoRegisterUser(newUser));
        }

        private async Task<HttpResponseMessage> DoRegisterUser(LoyaltyProgramUser newUser)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri($"http://{this.hostName}");
                var response = await httpClient.PostAsync("/users/",
                    new StringContent(JsonConvert.SerializeObject(newUser), Encoding.UTF8, "application/json"));
                ThrowOnTransientFailure(response);
                return response;
            }
        }

        private static void ThrowOnTransientFailure(HttpResponseMessage response)
        {
            if (((int)response.StatusCode) < 200 || ((int)response.StatusCode) > 499) throw new Exception(response.StatusCode.ToString());
        }


        public async Task<LoyaltyProgramUser> UpdateUser(LoyaltyProgramUser user)
        {
            using (var httpClient=new HttpClient())
            {
                httpClient.BaseAddress = new Uri($"http://{hostName}");
                var responses = await
                    httpClient.PutAsync(
                        $"/users/{user.Id}",
                        new StringContent(
                            JsonConvert.SerializeObject(user),
                            Encoding.UTF8,
                            "application/json"));
                ThrowOnTransientFailure(responses);
                return JsonConvert.DeserializeObject<LoyaltyProgramUser>(await responses.Content.ReadAsStringAsync());
            }
        }

        public async Task<HttpResponseMessage> QueryUser(int userId)
        {
            return await circuitBreaker.ExecuteAsync(() => DoUserQuery(userId));
        }

        private async Task<HttpResponseMessage> DoUserQuery(int userId)
        {
            var userResource = $"/users/{userId}";
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri($"http://{this.hostName}");
                var response = await httpClient.GetAsync(userResource);
                ThrowOnTransientFailure(response);
                return response;
            }
        }
    }


    public class LoyaltyProgramUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int LoyaltyPoints { get; set; }
        public LoyaltyProgramSettings Settings { get; set; }
    }

    public class LoyaltyProgramSettings
    {
        public string[] Interests { get; set; }
    }
}
