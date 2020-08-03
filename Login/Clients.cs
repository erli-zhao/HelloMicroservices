using IdentityServer4.Models;
using System.Collections.Generic;

namespace Login
{
    public class Clients
    {
        public static IEnumerable<Client> Get() => new List<Client>
            {
                new Client
                {
                    ClientId = "api_gateWay",
                ClientName = "API_GateWay",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = new List<Secret> {new Secret("secret".Sha256())}, // change me!
                AllowedScopes = new List<string> { "loyalty_program_write" }
                },
                new Client
                {
                    ClientId="web",
                    ClientName="Web Client",
                    RedirectUris=new List<string>{"http://localhost:5003/signin-oidc",},
                    PostLogoutRedirectUris=new List<string>{"http://localhost:5003",},
                    AllowedScopes=new List<string>{"openid","email","profile",}
                }
        };
    }
}
