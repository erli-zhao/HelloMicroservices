using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Services;
using IdentityServer4.Test;

namespace Login
{
    public class Users
    {
        public static List<TestUser> Get()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId="818727",
                    Username    ="alice",
                    Password="alice",
                    Claims=new List<Claim>
                    {
                        new Claim(JwtClaimTypes.Email,"AliceSmith@email.com"),
                        new Claim(JwtClaimTypes.Name,"Alice Smith"),
                        new Claim(JwtClaimTypes.GivenName,"Alice"),
                        new Claim(JwtClaimTypes.FamilyName,"Smith"),
                        new Claim(JwtClaimTypes.EmailVerified,"true",ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.Role,"User"),
                        new Claim(JwtClaimTypes.Id,"1",ClaimValueTypes.Integer64)
                    }
                },

            };

        }
    }
}
