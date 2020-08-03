using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;

namespace LoyaltyProgram
{
    public class UsersModule:NancyModule
    {
        private static IDictionary<int, LoyaltyProgramUser> registerUsers = new Dictionary<int, LoyaltyProgramUser>();
        public UsersModule() : base("/users")
        { 
            Post("", _ =>
            {
                var newUser = this.Bind<LoyaltyProgramUser>(); 
                this.AddRegisteredUser(newUser);
                return this.CreateResponse(newUser); 
            });

            Put("/{userId:int}", parameters =>
            {
                int userId = parameters.userId;
                var updateUser = this.Bind<LoyaltyProgramUser>();
                registerUsers[userId] = updateUser;
                return updateUser;
            });

            Get("", _ => registerUsers.Values);

            Get("/{userId:int}", paramerters =>
            {
                int userId = paramerters.userId;
                if (registerUsers.ContainsKey(userId))
                {
                    return registerUsers[userId];
                }
                else
                {
                    return HttpStatusCode.NotFound;
                }
            });
        }

        private dynamic CreateResponse(LoyaltyProgramUser newUser)
        {
            return this.Negotiate  // Negotiate is an entry point to Nancy's fluent API for create responses
                 .WithStatusCode(HttpStatusCode.Created) //Uses the 201 Create status code for response
                 .WithHeader("Location", $"{this.Request.Url.SiteBase}/users/{newUser.Id}") // Adds a location header to the response 
                                                                                            // because this is expected by HTTP for 201 Create responses
                .WithContentType("application/yaml")
                 .WithModel(newUser);  // Retruenss teh user in the reponse for convenience
        }

        private void AddRegisteredUser(LoyaltyProgramUser newUser)
        {
            var userId = registerUsers.Count;
            newUser.Id = userId;
            registerUsers[userId] = newUser;
        }
    
    
        
    }

    public class LoyaltyProgramUser
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int LoyaltyPoints { get; set; }

        public LoyaltyProgramSetting Settings{ get; set; }
    }

    public class LoyaltyProgramSetting
    {
        public string[] Interests { get; set; }
    }
}
