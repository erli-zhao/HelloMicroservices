﻿using System;
using System.Collections.Generic;
using System.Text;
using BuildFunc = System.Action<System.Func<
     System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>,
     System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>>>;

namespace MicroserviceNET.Auth
{
   public static class BuildFuncExtensions
    {
        public static BuildFunc UseAuthPlatform(this BuildFunc buildFunc,string requiredScope)
        {
            buildFunc(next => Authorization.Middleware(next, requiredScope));
            buildFunc(next => IdToken.Middleware(next));
            return buildFunc;
        }
    }
}
