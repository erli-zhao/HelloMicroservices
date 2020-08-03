using IdentityServer4.Models;
using System.Collections.Generic;

namespace Login
{
    public class Resources
    {
        public static IEnumerable<IdentityResource> Get() =>
            new[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource
                {
                    Name="role",
                    UserClaims=new List<string>{"role"}
                }
            };

        public static IEnumerable<Scope> GetApiScopes()
        {
            return new[]
            {
            new Scope("api1.read", "Read Access to API #1"),
            new Scope("api1.write", "Write Access to API #1")
             };
        }
    }
}
